import styles from './ui.module.css';

export function Loader({ label = 'Загрузка…' }) {
  return (
    <div className={styles.loaderWrap}>
      <span className={styles.spinner} />
      {label}
    </div>
  );
}

export function EmptyState({ title, description }) {
  return (
    <div className={styles.emptyState}>
      <div className={styles.emptyStateTitle}>{title}</div>
      {description && <p>{description}</p>}
    </div>
  );
}
