import styles from './ui.module.css';

export default function Button({
  children,
  variant = 'primary',
  size = 'md',
  type = 'button',
  disabled = false,
  onClick,
  ...rest
}) {
  const variantClass = {
    primary: styles.buttonPrimary,
    secondary: styles.buttonSecondary,
    danger: styles.buttonDanger,
    ghost: styles.buttonGhost,
  }[variant];

  const classNames = [styles.button, variantClass, size === 'sm' ? styles.buttonSm : '']
    .filter(Boolean)
    .join(' ');

  return (
    <button type={type} className={classNames} disabled={disabled} onClick={onClick} {...rest}>
      {children}
    </button>
  );
}
