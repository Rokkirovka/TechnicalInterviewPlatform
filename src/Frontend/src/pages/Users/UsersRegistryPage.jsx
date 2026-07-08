import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Archive, RotateCcw } from 'lucide-react';
import { fetchUsers, archiveUser, restoreUser } from '../../api/usersApi';
import { ROLE_LABELS } from '../../utils/constants';
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

export default function UsersRegistryPage() {
  const navigate = useNavigate();
  const { user: currentUser } = useAuth();
  const [search, setSearch] = useState('');
  const [showArchived, setShowArchived] = useState(false);
  const [users, setUsers] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [archiveTarget, setArchiveTarget] = useState(null);

  const load = useCallback(async (query, showArchived) => {
    setIsLoading(true);
    const data = await fetchUsers(query, showArchived);
    setUsers(data);
    setIsLoading(false);
  }, []);

  useEffect(() => {
    const timeout = setTimeout(() => load(search, showArchived), 250);
    return () => clearTimeout(timeout);
  }, [search, showArchived, load]);

  async function handleRestore(e, targetUser) {
    e.stopPropagation();
    await restoreUser(targetUser.id);
    load(search, showArchived);
  }

  function handleActionClick(e, targetUser) {
    e.stopPropagation();
    if (targetUser.archived) {
      handleRestore(e, targetUser);
    } else {
      setArchiveTarget(targetUser);
    }
  }

  async function handleConfirmArchive(reason) {
    await archiveUser(archiveTarget.id, reason, currentUser.id);
    setArchiveTarget(null);
    load(search, showArchived);
  }

  const columns = [
    { key: 'name', title: 'ФИО', render: (u) => <span className={styles.nameCell}>{formatFullName(u)}</span> },
    { key: 'login', title: 'Логин' },
    {
      key: 'roles',
      title: 'Роли',
      render: (u) => (
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          {u.roles.map((r) => (
            <Badge key={r} tone="info">{ROLE_LABELS[r]}</Badge>
          ))}
        </div>
      ),
    },
    {
      key: 'active',
      title: 'Статус',
      render: (u) => (
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          {u.active ? <Badge tone="success">Активен</Badge> : <Badge tone="neutral">Отключён</Badge>}
          {u.archived && <Badge tone="neutral">В архиве</Badge>}
        </div>
      ),
    },
    {
      key: 'actions',
      title: '',
      render: (u) => (
        <div className={uiStyles.actionsCell}>
          <IconButton
            label={u.archived ? 'Восстановить пользователя' : 'Архивировать пользователя'}
            onClick={(e) => handleActionClick(e, u)}
          >
            {u.archived ? <RotateCcw size={15} /> : <Archive size={15} />}
          </IconButton>
        </div>
      ),
    },
  ];

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.headingBlock}>
          <h2 className={styles.heading}>Реестр пользователей</h2>
          <span className={styles.subheading}>Учётные записи сотрудников платформы</span>
        </div>
        <Button onClick={() => navigate('/users/new')}>+ Добавить пользователя</Button>
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
        <Loader label="Загружаем пользователей…" />
      ) : users.length === 0 ? (
        <EmptyState title="Пользователи не найдены" description="Измените запрос или добавьте нового пользователя." />
      ) : (
        <Table columns={columns} rows={users} onRowClick={(u) => navigate(`/users/${u.id}`)} />
      )}

      {archiveTarget && (
        <ArchiveDialog
          entityLabel={`пользователя «${formatFullName(archiveTarget)}»`}
          onConfirm={handleConfirmArchive}
          onCancel={() => setArchiveTarget(null)}
        />
      )}
    </div>
  );
}
