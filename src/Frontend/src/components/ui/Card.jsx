import styles from './ui.module.css';

export default function Card({ children, style }) {
  return (
    <div className={styles.card} style={style}>
      {children}
    </div>
  );
}
