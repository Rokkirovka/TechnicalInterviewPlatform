import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Archive, RotateCcw } from 'lucide-react';
import { fetchCandidates, archiveCandidate, restoreCandidate } from '../../api/candidatesApi';
import { CANDIDATE_STATUS_LABELS, CANDIDATE_STATUS_TONE, ROLES } from '../../utils/constants';
import { formatFullName } from '../../utils/format';
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

export default function CandidatesRegistryPage() {
  const navigate = useNavigate();
  const { user, hasRole } = useAuth();
  const canCreate = hasRole(ROLES.HR);
  const canManage = hasRole(ROLES.ADMIN, ROLES.HR);

  const [search, setSearch] = useState('');
  const [showArchived, setShowArchived] = useState(false);
  const [candidates, setCandidates] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [archiveTarget, setArchiveTarget] = useState(null);

  const load = useCallback(async (query, includeArchived) => {
    setIsLoading(true);
    const data = await fetchCandidates(query, includeArchived);
    setCandidates(data);
    setIsLoading(false);
  }, []);

  useEffect(() => {
    const timeout = setTimeout(() => load(search, showArchived), 250);
    return () => clearTimeout(timeout);
  }, [search, showArchived, load]);

  async function handleRestore(e, candidate) {
    e.stopPropagation();
    await restoreCandidate(candidate.id);
    load(search, showArchived);
  }

  function handleActionClick(e, candidate) {
    e.stopPropagation();
    if (candidate.archived) {
      handleRestore(e, candidate);
    } else {
      setArchiveTarget(candidate);
    }
  }

  async function handleConfirmArchive(reason) {
    await archiveCandidate(archiveTarget.id, reason, user.id);
    setArchiveTarget(null);
    load(search, showArchived);
  }

  const columns = [
    { key: 'name', title: 'ФИО', render: (c) => <span className={styles.nameCell}>{formatFullName(c)}</span> },
    { key: 'phone', title: 'Телефон', render: (c) => <span className={styles.secondaryCell}>{c.phone}</span> },
    {
      key: 'status',
      title: 'Статус',
      render: (c) => (
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          <Badge tone={CANDIDATE_STATUS_TONE[c.status]}>{CANDIDATE_STATUS_LABELS[c.status]}</Badge>
          {c.archived && <Badge tone="neutral">В архиве</Badge>}
        </div>
      ),
    },
  ];

  if (canManage) {
    columns.push({
      key: 'actions',
      title: '',
      render: (c) => (
        <div className={uiStyles.actionsCell}>
          <IconButton
            label={c.archived ? 'Восстановить кандидата' : 'Архивировать кандидата'}
            onClick={(e) => handleActionClick(e, c)}
          >
            {c.archived ? <RotateCcw size={15} /> : <Archive size={15} />}
          </IconButton>
        </div>
      ),
    });
  }

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.headingBlock}>
          <h2 className={styles.heading}>Реестр кандидатов</h2>
          <span className={styles.subheading}>База кандидатов и статус их рассмотрения</span>
        </div>
        {canCreate && <Button onClick={() => navigate('/candidates/new')}>+ Добавить кандидата</Button>}
      </div>

      <div className={styles.toolbar}>
        <SearchBar value={search} onChange={setSearch} placeholder="Поиск по ФИО" />
        <Checkbox
          label="Только архивные"
          checked={showArchived}
          onChange={() => setShowArchived((v) => !v)}
        />
      </div>

      {isLoading ? (
        <Loader label="Загружаем кандидатов…" />
      ) : candidates.length === 0 ? (
        <EmptyState title="Кандидаты не найдены" description="Измените запрос поиска или добавьте нового кандидата." />
      ) : (
        <Table columns={columns} rows={candidates} onRowClick={(c) => navigate(`/candidates/${c.id}`)} />
      )}

      {archiveTarget && (
        <ArchiveDialog
          entityLabel={`кандидата «${formatFullName(archiveTarget)}»`}
          onConfirm={handleConfirmArchive}
          onCancel={() => setArchiveTarget(null)}
        />
      )}
    </div>
  );
}
