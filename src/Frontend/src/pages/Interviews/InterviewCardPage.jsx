import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import {
  fetchInterviewById,
  updateInterview,
  markInterviewPassed,
  submitDecision,
} from '../../api/interviewsApi';
import { printProtocol, printInvite, printReject } from '../../api/printApi';
import { useAuth } from '../../context/AuthContext';
import {
  ROLES,
  INTERVIEW_STATUS,
  INTERVIEW_STATUS_LABELS,
  INTERVIEW_STATUS_TONE,
  DECISION_LABELS,
} from '../../utils/constants';
import { buildStageCommentsTemplate } from '../../utils/format';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import Badge from '../../components/ui/Badge';
import CompetencyMatrix from '../../components/CompetencyMatrix';
import { Loader, EmptyState } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

const DECIDED_STATUSES = [
  INTERVIEW_STATUS.APPROVED,
  INTERVIEW_STATUS.REJECTED,
  INTERVIEW_STATUS.TO_NEXT_STAGE,
  INTERVIEW_STATUS.PASSED
];

export default function InterviewCardPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { hasRole } = useAuth();
  const isHr = hasRole(ROLES.HR);
  const canDecide = hasRole(ROLES.APPROVER);

  const [interview, setInterview] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isMarkingPassed, setIsMarkingPassed] = useState(false);
  const [isDeciding, setIsDeciding] = useState(false);
  const [error, setError] = useState(null);
  const [notice, setNotice] = useState(null);

  async function load() {
    setIsLoading(true);
    const data = await fetchInterviewById(id);
    if (!data.comment) {
      data.comment = buildStageCommentsTemplate(data.stages);
    }
    setInterview(data);
    setIsLoading(false);
  }

  useEffect(() => {
    load();
  }, [id]);
  
  const isDecided = DECIDED_STATUSES.includes(interview?.status);
  const canEditFields = isHr && !isDecided;

  function update(field, value) {
    setInterview((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSave(e) {
    e.preventDefault();
    setError(null);
    setIsSaving(true);
    try {
      const updated = await updateInterview(id, interview);
      setInterview({ ...interview, ...updated });
      setNotice('Изменения сохранены.');
    } catch (err) {
      setError(err.message || 'Не удалось сохранить изменения');
    } finally {
      setIsSaving(false);
    }
  }

  async function handleMarkPassed() {
    setError(null);
    setIsMarkingPassed(true);
    try {
      const updated = await markInterviewPassed(id);
      setInterview(updated);
      setNotice('Собеседование отмечено как проведённое. Теперь Решала может зафиксировать решение.');
    } catch (err) {
      setError(err.message || 'Не удалось отметить собеседование как проведённое');
    } finally {
      setIsMarkingPassed(false);
    }
  }

  async function handleDecision(decision) {
    setError(null);
    setIsDeciding(true);
    try {
      const updated = await submitDecision(id, decision);
      setInterview(updated);
      setNotice(`Решение зафиксировано: «${DECISION_LABELS[decision]}»`);
    } catch (err) {
      setError(err.message || 'Не удалось зафиксировать решение');
    } finally {
      setIsDeciding(false);
    }
  }

  async function handlePrint(action, targetId) {
    setNotice(null);
    const message = await action(targetId);
    setNotice(message);
  }

  if (isLoading) return <Loader label="Загружаем карточку собеседования…" />;
  if (!interview) return <EmptyState title="Собеседование не найдено" />;

  return (
    <div>
      <Link to={`/candidates/${interview.candidateId}`} className={styles.backLink}>
        ← К карточке кандидата
      </Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>
          Собеседование · {interview.candidateName}
        </h2>
        <Badge tone={INTERVIEW_STATUS_TONE[interview.status]}>
          {INTERVIEW_STATUS_LABELS[interview.status]}
        </Badge>
      </div>

      {notice && <div className={`${styles.banner} ${styles.bannerSuccess}`}>{notice}</div>}
      {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

      <div className={styles.twoColumn}>
        <div>
          <Card>
            <div className={styles.sectionTitle}>Общая информация</div>
            <div className={styles.grid}>
              <div className={styles.field}>
                <span className={styles.label}>Кандидат</span>
                <div className={styles.readOnlyValue}>{interview.candidateName}</div>
              </div>
              <div className={styles.field}>
                <span className={styles.label}>Вакансия</span>
                <div className={styles.readOnlyValue}>{interview.vacancyTitle}</div>
              </div>
              <Input
                label="Дата и время"
                type="datetime-local"
                value={interview.scheduledAt}
                disabled={!canEditFields}
                onChange={(e) => update('scheduledAt', e.target.value)}
              />
              {interview.createdByName && (
                <div className={styles.field}>
                  <span className={styles.label}>Создал</span>
                  <div className={styles.readOnlyValue}>{interview.createdByName}</div>
                </div>
              )}
            </div>

            <div className={styles.sectionGap}>
              <div className={styles.sectionTitle}>Этапы собеседования</div>
              {interview.stages.map((stage) => (
                <div key={stage.stageNumber} style={{ display: 'flex', gap: 12, padding: '8px 0', borderBottom: '1px solid var(--color-border)' }}>
                  <Badge tone="neutral">Этап {stage.stageNumber}</Badge>
                  <div>
                    <div style={{ fontWeight: 600, fontSize: 14 }}>
                      {stage.name || '—'}
                      {stage.duration ? (
                        <span style={{ fontWeight: 400, color: 'var(--color-text-secondary)' }}>
                          {' '}· {stage.duration} мин
                        </span>
                      ) : null}
                    </div>
                    <div style={{ fontSize: 13, color: 'var(--color-text-secondary)' }}>{stage.description || '—'}</div>
                  </div>
                </div>
              ))}
            </div>

            <div className={styles.sectionGap}>
              <div className={styles.sectionTitle}>Матрица компетенций</div>
              <CompetencyMatrix
                matrix={interview.matrix}
                onChange={(matrix) => update('matrix', matrix)}
                readOnly={!canEditFields}
              />
            </div>

            <div className={styles.sectionGap}>
              <Input
                label="Комментарии к собеседованию"
                multiline
                rows={35}
                style={{ fontFamily: "'Courier New', monospace", fontSize: 16 }}
                value={interview.comment}
                disabled={!canEditFields}
                onChange={(e) => update('comment', e.target.value)}
                placeholder="Ключевые впечатления, риски, рекомендации…"
              />
            </div>

            {isHr && (
              <div className={styles.formFooter}>
                {interview.status === INTERVIEW_STATUS.SCHEDULED && (
                  <Button variant="secondary" onClick={handleMarkPassed} disabled={isMarkingPassed}>
                    {isMarkingPassed ? 'Отмечаем…' : 'Отметить как проведённое'}
                  </Button>
                )}
                {canEditFields && (
                  <Button onClick={handleSave} disabled={isSaving}>
                    {isSaving ? 'Сохраняем…' : 'Сохранить изменения'}
                  </Button>
                )}
              </div>
            )}
          </Card>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
          {canDecide && (
            <Card>
              <div className={styles.sectionTitle}>Решение по кандидату</div>
              {interview.status === INTERVIEW_STATUS.SCHEDULED ? (
                <p style={{ fontSize: 13.5, color: 'var(--color-text-secondary)' }}>
                  Собеседование ещё не отмечено HR как проведённое — решение пока недоступно.
                </p>
              ) : (
                <>
                  <p style={{ fontSize: 13.5, color: 'var(--color-text-secondary)', marginBottom: 4 }}>
                    Текущее решение:{' '}
                    <b>{isDecided ? DECISION_LABELS[interview.status] : 'не зафиксировано'}</b>
                  </p>
                  <div className={styles.decisionBar}>
                    <Button
                      variant="secondary"
                      disabled={isDeciding}
                      onClick={() => handleDecision(INTERVIEW_STATUS.APPROVED)}
                    >
                      Принять
                    </Button>
                    <Button
                      variant="danger"
                      disabled={isDeciding}
                      onClick={() => handleDecision(INTERVIEW_STATUS.REJECTED)}
                    >
                      Отклонить
                    </Button>
                    <Button
                      variant="ghost"
                      disabled={isDeciding}
                      onClick={() => handleDecision(INTERVIEW_STATUS.TO_NEXT_STAGE)}
                    >
                      На следующий этап
                    </Button>
                  </div>
                </>
              )}
            </Card>
          )}

          <Card>
            <div className={styles.sectionTitle}>Печать документов</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              <Button variant="secondary" onClick={() => handlePrint(printProtocol, id)}>
                Печать протокола собеседования
              </Button>
              <Button variant="secondary" onClick={() => handlePrint(printInvite, interview.candidateId)}>
                Печать приглашения
              </Button>
              <Button variant="secondary" onClick={() => handlePrint(printReject, interview.candidateId)}>
                Печать отказа
              </Button>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
