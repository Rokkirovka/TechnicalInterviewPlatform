import styles from './ui.module.css';

export default function Input({
  label,
  required = false,
  error,
  hint,
  multiline = false,
  id,
  ...rest
}) {
  const inputId = id || rest.name;

  return (
    <div className={styles.field}>
      {label && (
        <label htmlFor={inputId} className={`${styles.label} ${required ? styles.required : ''}`}>
          {label}
        </label>
      )}
      {multiline ? (
        <textarea id={inputId} className={styles.textarea} {...rest} />
      ) : (
        <input id={inputId} className={styles.input} {...rest} />
      )}
      {error && <span className={styles.errorText}>{error}</span>}
      {!error && hint && <span className={styles.hint}>{hint}</span>}
    </div>
  );
}
