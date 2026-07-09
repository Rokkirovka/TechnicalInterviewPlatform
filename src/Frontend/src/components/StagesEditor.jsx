import Input from './ui/Input';
import Button from './ui/Button';
import styles from './forms.module.css';

// Редактор этапов собеседования: таблица "Номер этапа / Название / Описание".
export default function StagesEditor({ stages, onChange }) {
  function updateStage(index, field, value) {
    const updated = stages.map((stage, i) =>
      i === index ? { ...stage, [field]: value } : stage
    );
    onChange(updated);
  }

  function addStage() {
    onChange([...stages, { stageNumber: stages.length + 1, name: '', duration: '', description: '' }]);
  }

  function removeStage(index) {
    const updated = stages
      .filter((_, i) => i !== index)
      .map((stage, i) => ({ ...stage, stageNumber: i + 1 }));
    onChange(updated);
  }

  return (
    <div>
      {stages.map((stage, index) => (
        <div className={styles.stageRow} key={index}>
          <Input label={index === 0 ? '№' : undefined} value={stage.stageNumber} readOnly disabled />
          <Input
            label={index === 0 ? 'Название' : undefined}
            value={stage.name}
            onChange={(e) => updateStage(index, 'name', e.target.value)}
          />
          <Input
              label={index === 0 ? 'Длительность, мин' : undefined}
              type="number"
              min="0"
              value={stage.duration}
              onChange={(e) => updateStage(index, 'duration', e.target.value)}
              placeholder="60"
          />
          <Input
            label={index === 0 ? 'Описание' : undefined}
            value={stage.description}
            onChange={(e) => updateStage(index, 'description', e.target.value)}
            placeholder="Краткое описание этапа"
          />
          <button
            type="button"
            className={styles.removeIconButton}
            onClick={() => removeStage(index)}
            aria-label="Удалить этап"
            style={index === 0 ? { marginTop: 22 } : undefined}
          >
            ✕
          </button>
        </div>
      ))}
      <Button variant="secondary" size="sm" onClick={addStage}>
        + Добавить этап
      </Button>
    </div>
  );
}
