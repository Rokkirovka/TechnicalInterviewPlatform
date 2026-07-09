import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { fetchCandidateById, updateCandidate, fetchInterviewsOfCandidate } from '../../api/candidatesApi';
import { fetchSkills, createSkill } from '../../api/skillsApi.js';
import { printCandidateCard, printInvite, printReject } from '../../api/printApi';
import { useAuth } from '../../context/AuthContext';
import {
  ROLES,
  CANDIDATE_STATUS_LABELS,
  CANDIDATE_STATUS_TONE,
  INTERVIEW_STATUS_LABELS,
  INTERVIEW_STATUS_TONE,
} from '../../utils/constants';
import { formatDateTime } from '../../utils/format';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import Badge from '../../components/ui/Badge';
import SkillsEditor from '../../components/SkillsEditor';
import { Loader, EmptyState } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

export default function CandidateCardPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { hasRole } = useAuth();
  const canEdit = hasRole(ROLES.HR);
  const canCreateInterview = hasRole(ROLES.HR);

  const [candidate, setCandidate] = useState(null);
  const [interviews, setInterviews] = useState([]);
  const [skillOptions, setSkillOptions] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(null);
  const [notice, setNotice] = useState(null);

  async function loadAll() {
    setIsLoading(true);
    const [candidateData, interviewsData, skills] = await Promise.all([
      fetchCandidateById(id),
      fetchInterviewsOfCandidate(id),
      fetchSkills(),
    ]);
    setCandidate(candidateData);
    setInterviews(interviewsData);
    setSkillOptions(skills);
    setIsLoading(false);
  }

  useEffect(() => {
    loadAll();
  }, [id]);

  function update(field, value) {
    setCandidate((prev) => ({ ...prev, [field]: value }));
  }
  
  async function handleCreateSkill(name) {
    await createSkill(name);
    setSkillOptions((prev) => [...prev, name]);
  }

  async function handleSave(e) {
    e.preventDefault();
    setError(null);
    setIsSaving(true);
    try {
      const updated = await updateCandidate(id, candidate);
      setCandidate(updated);
      setNotice('Изменения сохранены.');
    } catch (err) {
      setError(err.message || 'Не удалось сохранить изменения');
    } finally {
      setIsSaving(false);
    }
  }

  async function handlePrint(action) {
    setNotice(null);
    const message = await action(id);
    setNotice(message);
  }

  if (isLoading) return <Loader label="Загружаем карточку кандидата…" />;
  if (!candidate) return <EmptyState title="Кандидат не найден" />;

  return (
    <div>
      <Link to="/candidates" className={styles.backLink}>← К реестру кандидатов</Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>
          {candidate.lastName} {candidate.firstName} {candidate.middleName}
        </h2>
        <div className={styles.headerActions}>
          <Badge tone={CANDIDATE_STATUS_TONE[candidate.status]}>
            {CANDIDATE_STATUS_LABELS[candidate.status]}
          </Badge>
        </div>
      </div>

      {notice && <div className={`${styles.banner} ${styles.bannerSuccess}`}>{notice}</div>}
      {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

      <div className={styles.twoColumn}>
        <Card>
          <form onSubmit={handleSave}>
            <div className={styles.sectionTitle}>Личные данные</div>
            <div className={styles.grid}>
              <Input label="Фамилия" value={candidate.lastName} disabled={!canEdit} onChange={(e) => update('lastName', e.target.value)} />
              <Input label="Имя" value={candidate.firstName} disabled={!canEdit} onChange={(e) => update('firstName', e.target.value)} />
              <Input label="Отчество" value={candidate.middleName} disabled={!canEdit} onChange={(e) => update('middleName', e.target.value)} />
              <Input label="Телефон" value={candidate.phone} disabled={!canEdit} onChange={(e) => update('phone', e.target.value)} />
              <Input label="Город" value={candidate.city} disabled={!canEdit} onChange={(e) => update('city', e.target.value)} />
            </div>

            <div className={styles.sectionGap}>
              <div className={styles.sectionTitle}>Образование и опыт</div>
              <div className={styles.grid}>
                <Input label="Образование" multiline value={candidate.education} disabled={!canEdit} onChange={(e) => update('education', e.target.value)} />
                <Input label="Предыдущий опыт работы" multiline value={candidate.experience} disabled={!canEdit} onChange={(e) => update('experience', e.target.value)} />
              </div>
            </div>

            <div className={styles.sectionGap}>
              <div className={styles.sectionTitle}>Навыки</div>
              {canEdit ? (
                <SkillsEditor
                  skills={candidate.skills}
                  skillOptions={skillOptions}
                  onChange={(skills) => update('skills', skills)}
                  onCreateSkill={handleCreateSkill}
                />
              ) : (
                <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
                  {candidate.skills.map((s) => (
                    <Badge key={s.id} tone="neutral">{s.skillName} · {s.level}</Badge>
                  ))}
                </div>
              )}
            </div>

            {canEdit && (
              <div className={styles.formFooter}>
                <Button type="submit" disabled={isSaving}>
                  {isSaving ? 'Сохраняем…' : 'Сохранить изменения'}
                </Button>
              </div>
            )}
          </form>
        </Card>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
          <Card>
            <div className={styles.sectionTitle}>Собеседования</div>
            {interviews.length === 0 && (
              <p style={{ fontSize: 13.5, color: 'var(--color-text-secondary)' }}>
                У кандидата пока нет собеседований.
              </p>
            )}
            {interviews.map((interview) => (
              <div
                key={interview.id}
                className={styles.interviewListItem}
                onClick={() => navigate(`/interviews/${interview.id}`)}
              >
                <div>
                  <div style={{ fontWeight: 600, fontSize: 14 }}>{interview.vacancyTitle}</div>
                  <div style={{ fontSize: 12.5, color: 'var(--color-text-secondary)' }}>
                    {formatDateTime(interview.scheduledAt)}
                  </div>
                </div>
                <Badge tone={INTERVIEW_STATUS_TONE[interview.status]}>
                  {INTERVIEW_STATUS_LABELS[interview.status]}
                </Badge>
              </div>
            ))}
            {canCreateInterview && (
              <Button
                variant="secondary"
                size="sm"
                onClick={() => navigate(`/interviews/new?candidateId=${id}`)}
                style={{ marginTop: 8 }}
              >
                + Новое собеседование
              </Button>
            )}
          </Card>

          <Card>
            <div className={styles.sectionTitle}>Печать документов</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              <Button variant="secondary" onClick={() => handlePrint(printCandidateCard)}>
                Печать карточки кандидата
              </Button>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
