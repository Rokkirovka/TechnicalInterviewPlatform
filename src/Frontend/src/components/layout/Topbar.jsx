import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROLE_LABELS } from '../../utils/constants';
import { formatFullName } from '../../utils/format';
import Button from '../ui/Button';
import styles from './layout.module.css';

export default function Topbar() {
  const { user, signOut } = useAuth();
  const navigate = useNavigate();

  const initials = user
    ? `${user.lastName?.[0] || ''}${user.firstName?.[0] || ''}`.toUpperCase()
    : '';

  async function handleLogout() {
    await signOut();
    navigate('/login', { replace: true });
  }

  return (
    <header className={styles.topbar}>
      <div />
      <div className={styles.topbarRight}>
        <div className={styles.userInfo}>
          <span className={styles.avatar}>{initials}</span>
          <div>
            <div className={styles.userName}>{formatFullName(user)}</div>
            <div className={styles.userRole}>{user?.roles.map((r) => ROLE_LABELS[r]).join(', ')}</div>
          </div>
        </div>
        <Button variant="ghost" size="sm" onClick={handleLogout}>
          Выйти
        </Button>
      </div>
    </header>
  );
}
