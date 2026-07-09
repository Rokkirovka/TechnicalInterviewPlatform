import { averageScore } from '../utils/format';
import styles from './forms.module.css';

const SCALE = [1, 2, 3, 4, 5];

export default function CompetencyMatrix({ matrix, onChange, readOnly = false }) {
  function setScore(id, score) {
    if (readOnly) return;
    onChange(matrix.map((item) => (item.id === id ? { ...item, score } : item)));
  }

  const avg = averageScore(matrix.filter((m) => m.score > 0));

  return (
    <div>
      <div className={styles.matrix}>
        {matrix.map((item) => (
          <div key={item.id} className={styles.matrixRow}>
            <span className={styles.matrixLabel}>{item.competencyName}</span>
            <div className={styles.scaleGroup}>
              {SCALE.map((value) => (
                <button
                  key={value}
                  type="button"
                  disabled={readOnly}
                  onClick={() => setScore(item.id, value)}
                  className={`${styles.scaleButton} ${
                    item.score === value ? styles.scaleButtonActive : ''
                  }`}
                  aria-label={`${item.competencyName}: оценка ${value}`}
                  aria-pressed={item.score === value}
                >
                  {value}
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
      <div className={styles.matrixSummary}>
        <span>Средний балл по компетенциям</span>
        <span className={styles.matrixAverage}>{avg ?? '—'}</span>
      </div>
    </div>
  );
}
