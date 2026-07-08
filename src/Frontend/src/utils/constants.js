export const ROLES = {
  ADMIN: 'admin',
  HR: 'hr',
  APPROVER: 'approver',
};

export const ROLE_LABELS = {
  [ROLES.ADMIN]: 'Администратор',
  [ROLES.HR]: 'Отдел кадров',
  [ROLES.APPROVER]: 'Решала',
};

export const CANDIDATE_STATUS = {
  NEW: 'new',
  IN_PROGRESS: 'in_progress',
  ACCEPTED: 'accepted',
};

export const CANDIDATE_STATUS_LABELS = {
  [CANDIDATE_STATUS.NEW]: 'Новый',
  [CANDIDATE_STATUS.IN_PROGRESS]: 'В процессе',
  [CANDIDATE_STATUS.ACCEPTED]: 'Принят',
};

export const CANDIDATE_STATUS_TONE = {
  [CANDIDATE_STATUS.NEW]: 'neutral',
  [CANDIDATE_STATUS.IN_PROGRESS]: 'info',
  [CANDIDATE_STATUS.ACCEPTED]: 'success',
};

export const INTERVIEW_STATUS = {
  SCHEDULED: 'scheduled',
  PASSED: 'passed',
  APPROVED: 'approved',
  REJECTED: 'rejected',
  TO_NEXT_STAGE: 'to_next_stage',
};

export const INTERVIEW_STATUS_LABELS = {
  [INTERVIEW_STATUS.SCHEDULED]: 'Запланировано',
  [INTERVIEW_STATUS.PASSED]: 'Проведено',
  [INTERVIEW_STATUS.APPROVED]: 'Принят',
  [INTERVIEW_STATUS.REJECTED]: 'Отклонён',
  [INTERVIEW_STATUS.TO_NEXT_STAGE]: 'Следующий этап',
};

export const INTERVIEW_STATUS_TONE = {
  [INTERVIEW_STATUS.SCHEDULED]: 'warning',
  [INTERVIEW_STATUS.PASSED]: 'info',
  [INTERVIEW_STATUS.APPROVED]: 'success',
  [INTERVIEW_STATUS.REJECTED]: 'danger',
  [INTERVIEW_STATUS.TO_NEXT_STAGE]: 'warning',
};

export const DECISION_LABELS = {
  [INTERVIEW_STATUS.APPROVED]: 'Принять',
  [INTERVIEW_STATUS.REJECTED]: 'Отклонить',
  [INTERVIEW_STATUS.TO_NEXT_STAGE]: 'На следующий этап',
};

export const SKILL_LEVELS = ['Базовый', 'Средний', 'Высокий'];

export const ARCHIVE_REASON_SUGGESTIONS = [
  'Дубликат записи',
  'Создано по ошибке',
  'Больше не актуально',
];

export const STORAGE_KEYS = {
  DB: 'itp_mock_db_v1',
  AUTH: 'itp_auth_v1',
};
