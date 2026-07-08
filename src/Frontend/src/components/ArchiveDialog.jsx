import { useState } from 'react';
import Modal from './Modal';
import Input from './ui/Input';
import Button from './ui/Button';
import { ARCHIVE_REASON_SUGGESTIONS } from '../utils/constants';
import styles from './ui/modal.module.css';

export default function ArchiveDialog({ entityLabel, onConfirm, onCancel }) {
  const [reason, setReason] = useState('');

  function handleConfirm() {
    if (!reason.trim()) return;
    onConfirm(reason.trim());
  }

  return (
    <Modal onClose={onCancel}>
      <div className={styles.title}>Архивировать {entityLabel}?</div>
      <p className={styles.description}>
        Запись перестанет отображаться в реестре, но не будет удалена безвозвратно —
        её можно будет восстановить. Укажите причину архивации.
      </p>

      <div className={styles.suggestions}>
        {ARCHIVE_REASON_SUGGESTIONS.map((s) => (
          <button
            key={s}
            type="button"
            className={styles.suggestionChip}
            onClick={() => setReason(s)}
          >
            {s}
          </button>
        ))}
      </div>

      <Input
        label="Причина архивации"
        required
        multiline
        rows={3}
        value={reason}
        onChange={(e) => setReason(e.target.value)}
      />

      <div className={styles.actions}>
        <Button variant="secondary" onClick={onCancel}>
          Отмена
        </Button>
        <Button variant="danger" onClick={handleConfirm} disabled={!reason.trim()}>
          Архивировать
        </Button>
      </div>
    </Modal>
  );
}
