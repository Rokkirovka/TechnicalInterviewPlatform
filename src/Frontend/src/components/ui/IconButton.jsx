import styles from './ui.module.css';

export default function IconButton({ children, label, onClick, tone = 'neutral', ...rest }) {
  return (
    <button
      type="button"
      className={`${styles.iconButton} ${tone === 'danger' ? styles.iconButtonDanger : ''}`}
      onClick={onClick}
      aria-label={label}
      title={label}
      {...rest}
    >
      {children}
    </button>
  );
}
