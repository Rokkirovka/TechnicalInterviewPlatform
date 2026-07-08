import styles from './ui.module.css';

export default function Badge({ children, tone = 'neutral' }) {
  const toneClass = {
    neutral: styles.badgeNeutral,
    info: styles.badgeInfo,
    success: styles.badgeSuccess,
    warning: styles.badgeWarning,
    danger: styles.badgeDanger,
  }[tone];

  return <span className={`${styles.badge} ${toneClass}`}>{children}</span>;
}
