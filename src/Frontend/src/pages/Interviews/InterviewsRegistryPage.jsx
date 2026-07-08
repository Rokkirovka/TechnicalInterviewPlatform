import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Archive, RotateCcw } from 'lucide-react';
import { fetchInterviews, archiveInterview, restoreInterview } from '../../api/interviewsApi';
import { INTERVIEW_STATUS_LABELS, INTERVIEW_STATUS_TONE } from '../../utils/constants';
import { formatDateTime } from '../../utils/format';
import { useAuth } from '../../context/AuthContext';
import Table from '../../components/ui/Table';
import SearchBar from '../../components/ui/SearchBar';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import Checkbox from '../../components/ui/Checkbox';
import IconButton from '../../components/ui/IconButton';
import ArchiveDialog from '../../components/ArchiveDialog';
import { Loader, EmptyState } from '../../components/ui/Loader';
import styles from '../shared/registry.module.css';
import uiStyles from '../../components/ui/ui.module.css';

export default function InterviewsRegistryPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [search, setSearch] = useState('');
  const [showArchived, setShowArchived] = useState(false);
  const [interviews, setInterviews] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [archiveTarget, setArchiveTarget] = useState(null);

  const load = useCallback(async (query, includeArchived) => {
    setIsLoading(true);
    const data = await fetchInterviews(query, includeArchived);
    setInterviews(data);
    setIsLoading(false);
  }, []);

  useEffect(() => {
    const timeout = setTimeout(() => load(search, showArchived), 250);
    return () => clearTimeout(timeout);
  }, [search, showArchived, load]);

  async function handleRestore(e, interview) {
    e.stopPropagation();
    await restoreInterview(interview.id);
    load(search, showArchived);
  }

  function handleActionClick(e, interview) {
    e.stopPropagation();
    if (interview.archived) {
      handleRestore(e, interview);
    } else {
      setArchiveTarget(interview);
    }
  }

  async function handleConfirmArchive(reason) {
    await archiveInterview(archiveTarget.id, reason, user.id);
    setArchiveTarget(null);
    load(search, showArchived);
  }

  const columns = [
    { key: 'candidateName', title: 'Кандидат', render: (i) => <span className={styles.nameCell}>{i.candidateName}</span> },
    { key: 'vacancyName', title: 'Вакансия' },
    { key: 'dateTime', title: 'Дата и время', render: (i) => <span className={styles.secondaryCell}>{formatDateTime(i.dateTime)}</span> },
    {
      key: 'status',
      title: 'Статус',
      render: (i) => (
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          <Badge tone={INTERVIEW_STATUS_TONE[i.status]}>{INTERVIEW_STATUS_LABELS[i.status]}</Badge>
          {i.archived && <Badge tone="neutral">В архиве</Badge>}
        </div>
      ),
    },
    {
      key: 'actions',
      title: '',
      render: (i) => (
        <div className={uiStyles.actionsCell}>
          <IconButton
            label={i.archived ? 'Восстановить собеседование' : 'Архивировать собеседование'}
            onClick={(e) => handleActionClick(e, i)}
          >
            {i.archived ? <RotateCcw size={15} /> : <Archive size={15} />}
          </IconButton>
        </div>
      ),
    },
  ];

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.headingBlock}>
          <h2 className={styles.heading}>Реестр собеседований</h2>
          <span className={styles.subheading}>Все запланированные и проведённые интервью</span>
        </div>
        <Button onClick={() => navigate('/interviews/new')}>+ Добавить собеседование</Button>
      </div>

      <div className={styles.toolbar}>
        <SearchBar value={search} onChange={setSearch} placeholder="Поиск по ФИО кандидата" />
        <Checkbox
          label="Только архивные"
          checked={showArchived}
          onChange={() => setShowArchived((v) => !v)}
        />
      </div>

      {isLoading ? (
        <Loader label="Загружаем собеседования…" />
      ) : interviews.length === 0 ? (
        <EmptyState title="Собеседования не найдены" description="Измените запрос или добавьте новое собеседование." />
      ) : (
        <Table columns={columns} rows={interviews} onRowClick={(i) => navigate(`/interviews/${i.id}`)} />
      )}

      {archiveTarget && (
        <ArchiveDialog
          entityLabel={`собеседование с «${archiveTarget.candidateName}»`}
          onConfirm={handleConfirmArchive}
          onCancel={() => setArchiveTarget(null)}
        />
      )}
    </div>
  );
}
