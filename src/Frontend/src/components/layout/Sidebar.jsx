import { NavLink } from 'react-router-dom';
import { User, Briefcase, Contact, Calendar } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { ROLES } from '../../utils/constants';
import styles from './layout.module.css';

const NAV_ITEMS = [
  {
    to: '/users',
    label: 'Пользователи',
    roles: [ROLES.ADMIN],
    icon: <User size={18} strokeWidth={1.8} />,
  },
  {
    to: '/vacancies',
    label: 'Вакансии',
    roles: [ROLES.ADMIN, ROLES.HR],
    icon: <Briefcase size={18} strokeWidth={1.8} />,
  },
  {
    to: '/candidates',
    label: 'Кандидаты',
    roles: [ROLES.ADMIN, ROLES.HR, ROLES.APPROVER],
    icon: <Contact size={18} strokeWidth={1.8} />,
  },
  {
    to: '/interviews',
    label: 'Собеседования',
    roles: [ROLES.HR, ROLES.APPROVER],
    icon: <Calendar size={18} strokeWidth={1.8} />,
  },
];

export default function Sidebar() {
  const { hasRole } = useAuth();
  const visibleItems = NAV_ITEMS.filter((item) => hasRole(...item.roles));

  return (
    <aside className={styles.sidebar}>
      <div className={styles.logo}>
        <span className={styles.logoMark}>ТС</span>
        Платформа технических
        <br />
        собеседований
      </div>

      <nav className={styles.nav}>
        {visibleItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `${styles.navLink} ${isActive ? styles.navLinkActive : ''}`
            }
          >
            <span className={styles.navIcon}>{item.icon}</span>
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
