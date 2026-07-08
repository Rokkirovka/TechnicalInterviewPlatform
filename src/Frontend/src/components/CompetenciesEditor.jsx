import { useState } from 'react';
import Input from './ui/Input';
import Button from './ui/Button';
import Checkbox from './ui/Checkbox';
import styles from './forms.module.css';

export default function CompetenciesEditor({ competencyIds, allCompetencies, onChange, onCreateCompetency }) {
  const [draftName, setDraftName] = useState('');
  const [isCreating, setIsCreating] = useState(false);

  function toggle(id) {
    onChange(
      competencyIds.includes(id)
        ? competencyIds.filter((existingId) => existingId !== id)
        : [...competencyIds, id]
    );
  }

  async function handleCreate() {
    const trimmed = draftName.trim();
    if (!trimmed) return;
    setIsCreating(true);
    try {
      const created = await onCreateCompetency(trimmed);
      onChange([...competencyIds, created.id]);
      setDraftName('');
    } finally {
      setIsCreating(false);
    }
  }

  return (
    <div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 4, marginBottom: 16 }}>
        {allCompetencies.length === 0 && (
          <span style={{ color: 'var(--color-text-muted)', fontSize: 13.5 }}>
            В справочнике пока нет компетенций — добавьте первую ниже.
          </span>
        )}
        {allCompetencies.map((c) => (
          <Checkbox
            key={c.id}
            label={c.name}
            checked={competencyIds.includes(c.id)}
            onChange={() => toggle(c.id)}
          />
        ))}
      </div>

      <div className={styles.addRow} style={{ gridTemplateColumns: '1fr auto' }}>
        <Input
          label="Новая компетенция в справочник"
          value={draftName}
          onChange={(e) => setDraftName(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              e.preventDefault();
              handleCreate();
            }
          }}
        />
        <Button variant="secondary" onClick={handleCreate} disabled={!draftName.trim() || isCreating}>
          {isCreating ? 'Добавляем…' : 'Добавить'}
        </Button>
      </div>
    </div>
  );
}
