export function formatDateTime(isoString) {
  if (!isoString) return '—';
  const date = new Date(isoString);
  if (Number.isNaN(date.getTime())) return '—';
  return date.toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function averageScore(matrix) {
  if (!matrix || matrix.length === 0) return null;
  const sum = matrix.reduce((acc, item) => acc + (item.score || 0), 0);
  return Math.round((sum / matrix.length) * 10) / 10;
}

export function buildStageCommentsTemplate(stages) {
  const hasStages = stages && stages.length > 0;

  const stageNotes = hasStages
      ? stages
          .map((stage) => {
            const title = stage.name ? stage.name : 'без названия';
            return `Этап ${stage.stageNumber} - ${title}:\n- \n`;
          })
          .join('\n')
      : null;

  const sections = [
    'Общее впечатление:',
    '- \n\n',
    'Сильные стороны:',
    '- \n\n',
    'Зоны развития/риски:',
    '- \n\n',
  ];

  if (hasStages) {
    sections.push('Заметки по этапам:\n', stageNotes);
  }

  sections.push('\nРекомендация HR:', '-');

  return sections.join('\n');
}


