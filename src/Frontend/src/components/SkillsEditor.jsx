import { useState } from 'react';
import Select from './ui/Select';
import Button from './ui/Button';
import { SKILL_LEVELS } from '../utils/constants';
import styles from './forms.module.css';

// Редактор навыков кандидата.
export default function SkillsEditor({ skills, skillOptions, onChange }) {
  const [draftSkill, setDraftSkill] = useState('');
  const [draftLevel, setDraftLevel] = useState(SKILL_LEVELS[0]);

  function addSkill() {
    if (!draftSkill) return;
    onChange([
      ...skills,
      { id: `skill_${Date.now()}`, skillName: draftSkill, level: draftLevel },
    ]);
    setDraftSkill('');
    setDraftLevel(SKILL_LEVELS[0]);
  }

  function removeSkill(id) {
    onChange(skills.filter((s) => s.id !== id));
  }

  return (
    <div>
      <div className={styles.chipList}>
        {skills.length === 0 && (
          <span style={{ color: 'var(--color-text-muted)', fontSize: 13.5 }}>
            Навыки ещё не добавлены
          </span>
        )}
        {skills.map((skill) => (
          <span key={skill.id} className={styles.chip}>
            {skill.skillName} · {skill.level}
            <button
              type="button"
              className={styles.chipRemove}
              onClick={() => removeSkill(skill.id)}
              aria-label={`Удалить навык ${skill.skillName}`}
            >
              ✕
            </button>
          </span>
        ))}
      </div>

      <div className={styles.addRow}>
        <Select
          label="Навык"
          placeholder="Выберите навык"
          value={draftSkill}
          onChange={(e) => setDraftSkill(e.target.value)}
          options={skillOptions.map((s) => ({ value: s, label: s }))}
        />
        <Select
          label="Уровень"
          value={draftLevel}
          onChange={(e) => setDraftLevel(e.target.value)}
          options={SKILL_LEVELS.map((l) => ({ value: l, label: l }))}
        />
        <Button variant="secondary" onClick={addSkill} disabled={!draftSkill}>
          Добавить
        </Button>
      </div>
    </div>
  );
}
