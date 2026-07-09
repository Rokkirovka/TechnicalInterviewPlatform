import styles from './ui.module.css';

export default function Select({ label, required = false, error, options = [], id, placeholder, ...rest }) {
  const selectId = id || rest.name;

  return (
    <div className={styles.field}>
      {label && (
        <label htmlFor={selectId} className={`${styles.label} ${required ? styles.required : ''}`}>
          {label}
        </label>
      )}
      <select id={selectId} className={styles.select} {...rest}>
        {placeholder && <option value="">{placeholder}</option>}
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <span className={styles.errorText}>{error}</span>}
    </div>
  );
}
