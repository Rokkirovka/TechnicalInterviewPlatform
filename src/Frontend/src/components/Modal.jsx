import styles from './modal.module.css';

export default function Modal({ children, onClose }) {
  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.dialog} onClick={(e) => e.stopPropagation()}>
        {children}
      </div>
    </div>
  );
}
