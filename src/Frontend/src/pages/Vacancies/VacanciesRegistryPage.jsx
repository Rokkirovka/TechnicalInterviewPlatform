import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Archive, RotateCcw } from 'lucide-react';
import { fetchVacancies, archiveVacancy, restoreVacancy } from '../../api/vacanciesApi';
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

export default function VacanciesRegistryPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [search, setSearch] = useState('');
  const [showArchived, setShowArchived] = useState(false);
  const [vacancies, setVacancies] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [archiveTarget, setArchiveTarget] = useState(null);

  const load = useCallback(async (query, showArchived) => {
    setIsLoading(true);
    const data = await fetchVacancies(query, showArchived);
    setVacancies(data);
    setIsLoading(false);
  }, []);

  useEffect(() => {
    const timeout = setTimeout(() => load(search, showArchived), 250);
    return () => clearTimeout(timeout);
  }, [search, showArchived, load]);

  async function handleRestore(e, vacancy) {
    e.stopPropagation();
    await restoreVacancy(vacancy.id);
    load(search, showArchived);
  }

  async function handleConfirmArchive(reason) {
    await archiveVacancy(archiveTarget.id, reason, user.id);
    setArchiveTarget(null);
    load(search, showArchived);
  }

  function handleActionClick(e, vacancy) {
    e.stopPropagation();
    if (vacancy.archived) {
      handleRestore(e, vacancy);
    } else {
      setArchiveTarget(vacancy);
    }
  }

  const columns = [
    { key: 'name', title: 'Вакансия', render: (v) => <span className={styles.nameCell}>{v.name}</span> },
    { key: 'department', title: 'Отдел', render: (v) => <span className={styles.secondaryCell}>{v.department || '—'}</span> },
    {
      key: 'competencies',
      title: 'Компетенции для матрицы',
      render: (v) => (
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          {v.competencies.length > 0 ? (
              <Badge tone="info">{v.competencyCount} шт.</Badge>
          ) : (
              <Badge tone="neutral">не заданы</Badge>
          )}
          {v.archived && <Badge tone="neutral">В архиве</Badge>}
        </div>
      ),
    },
    {
      key: 'actions',
      title: '',
      render: (v) => (
        <div className={uiStyles.actionsCell}>
          <IconButton
            label={v.archived ? 'Восстановить вакансию' : 'Архивировать вакансию'}
            onClick={(e) => handleActionClick(e, v)}
          >
            {v.archived ? <RotateCcw size={15} /> : <Archive size={15} />}
          </IconButton>
        </div>
      ),
    },
  ];

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.headingBlock}>
          <h2 className={styles.heading}>Реестр вакансий</h2>
          <span className={styles.subheading}>
            Вакансии и наборы компетенций для матрицы собеседования
          </span>
        </div>
        <Button onClick={() => navigate('/vacancies/new')}>+ Добавить вакансию</Button>
      </div>

      <div className={styles.toolbar}>
        <SearchBar value={search} onChange={setSearch} placeholder="Поиск по названию вакансии" />
        <Checkbox
          label="Только архивные"
          checked={showArchived}
          onChange={() => setShowArchived((v) => !v)}
        />
      </div>

      {isLoading ? (
        <Loader label="Загружаем вакансии…" />
      ) : vacancies.length === 0 ? (
        <EmptyState title="Вакансии не найдены" description="Измените запрос или добавьте новую вакансию." />
      ) : (
        <Table columns={columns} rows={vacancies} onRowClick={(v) => navigate(`/vacancies/${v.id}`)} />
      )}

      {archiveTarget && (
        <ArchiveDialog
          entityLabel={`вакансию «${archiveTarget.name}»`}
          onConfirm={handleConfirmArchive}
          onCancel={() => setArchiveTarget(null)}
        />
      )}
    </div>
  );
}
