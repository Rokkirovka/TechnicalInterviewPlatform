import { useState } from 'react';
import Input from './ui/Input';
import Select from './ui/Select';
import Button from './ui/Button';
import { SKILL_LEVELS } from '../utils/constants';
import styles from './forms.module.css';

export default function SkillsEditor({ skills, skillOptions, onChange, onCreateSkill }) {
  const [draftSkill, setDraftSkill] = useState('');
  const [draftLevel, setDraftLevel] = useState(SKILL_LEVELS[0]);
  const [isSaving, setIsSaving] = useState(false);

  const trimmedDraft = draftSkill.trim();
  const isNewSkill =
    trimmedDraft.length > 0 &&
    !skillOptions.some((s) => s.toLowerCase() === trimmedDraft.toLowerCase());

  async function addSkill() {
    if (!trimmedDraft) return;
    setIsSaving(true);
    try {
      if (isNewSkill) {
        await onCreateSkill(trimmedDraft);
      }
      onChange([
        ...skills,
        { id: `skill_${Date.now()}`, skillName: trimmedDraft, level: draftLevel },
      ]);
      setDraftSkill('');
      setDraftLevel(SKILL_LEVELS[0]);
    } finally {
      setIsSaving(false);
    }
  }

  function removeSkill(id) {
    onChange(skills.filter((s) => s.id !== id));
  }

  function handleKeyDown(e) {
    if (e.key === 'Enter') {
      e.preventDefault();
      addSkill();
    }
  }

  return (
    <div>
      <div className={styles.chipList}>
        {skills.length === 0 && (
          <span style={{ color: 'var(--color-text-muted)', fontSize: 14 }}>
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
        <Input
          label="Навык"
          list="skills-directory"
          value={draftSkill}
          onChange={(e) => setDraftSkill(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Выберите из списка или введите новый"
        />
        <datalist id="skills-directory">
          {skillOptions.map((s) => (
            <option key={s} value={s} />
          ))}
        </datalist>

        <Select
          label="Уровень"
          value={draftLevel}
          onChange={(e) => setDraftLevel(e.target.value)}
          options={SKILL_LEVELS.map((l) => ({ value: l, label: l }))}
        />
        <Button variant="secondary" onClick={addSkill} disabled={!trimmedDraft || isSaving}>
          {isSaving ? 'Добавляем…' : 'Добавить'}
        </Button>
      </div>
    </div>
  );
}
