import { Search } from 'lucide-react';
import styles from './ui.module.css';

export default function SearchBar({ value, onChange, placeholder = 'Поиск по ФИО' }) {
  return (
    <div className={styles.searchBar}>
      <span className={styles.searchIcon} aria-hidden="true">
        <Search size={16} strokeWidth={2} />
      </span>
      <input
        className={`${styles.input} ${styles.searchInput}`}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        aria-label={placeholder}
      />
    </div>
  );
}
