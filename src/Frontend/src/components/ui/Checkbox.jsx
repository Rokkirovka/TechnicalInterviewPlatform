import styles from './ui.module.css';

export default function Checkbox({ label, checked, onChange, ...rest }) {
  return (
    <label className={styles.checkboxRow}>
      <input type="checkbox" checked={checked} onChange={onChange} {...rest} />
      {label}
    </label>
  );
}
