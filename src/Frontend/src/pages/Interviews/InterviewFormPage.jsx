import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams, Link } from 'react-router-dom';
import { createInterview } from '../../api/interviewsApi';
import { fetchCandidateNames } from '../../api/candidatesApi';
import { fetchVacancies } from '../../api/vacanciesApi';
import { useAuth } from '../../context/AuthContext';
import Select from '../../components/ui/Select';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import StagesEditor from '../../components/StagesEditor';
import { Loader } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

const DEFAULT_STAGE = [{ stageNumber: 1, name: '', duration: '', description: '' }];

export default function InterviewFormPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [searchParams] = useSearchParams();
  const preselectedCandidateId = searchParams.get('candidateId') || '';

  const [candidateOptions, setCandidateOptions] = useState([]);
  const [vacancyOptions, setVacancyOptions] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(null);

  const [form, setForm] = useState({
    candidateId: preselectedCandidateId,
    vacancyId: '',
    scheduledAt: '',
    stages: DEFAULT_STAGE,
  });

  useEffect(() => {
    (async () => {
      const [candidates, vacancies] = await Promise.all([fetchCandidateNames(), fetchVacancies()]);
      setCandidateOptions(candidates);
      setVacancyOptions(vacancies);
      setIsLoading(false);
    })();
  }, []);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    if (!form.candidateId || !form.vacancyId || !form.dateTime) {
      setError('Заполните кандидата, вакансию и дату собеседования');
      return;
    }

    setIsSaving(true);
    try {
      const vacancy = vacancyOptions.find((v) => v.id === form.vacancyId);
      const interview = await createInterview({
        ...form,
        vacancyTitle: vacancy?.title || '',
        createdByUserId: user.id,
      });
      navigate(`/interviews/${interview.id}`);
    } catch (err) {
      setError(err.message || 'Не удалось создать собеседование');
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) return <Loader label="Готовим форму…" />;

  return (
    <div>
      <Link to="/interviews" className={styles.backLink}>← К реестру собеседований</Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>Новое собеседование</h2>
      </div>

      <Card>
        <form onSubmit={handleSubmit}>
          {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

          <div className={styles.grid}>
            <Select
              label="Кандидат"
              required
              placeholder="Выберите кандидата"
              value={form.candidateId}
              onChange={(e) => update('candidateId', e.target.value)}
              options={candidateOptions.map((c) => ({ value: c.id, label: c.fullName }))}
            />
            <Select
              label="Вакансия"
              required
              placeholder="Выберите вакансию"
              value={form.vacancyId}
              onChange={(e) => update('vacancyId', e.target.value)}
              options={vacancyOptions.map((v) => ({ value: v.id, label: v.name }))}
            />
            <Input
              label="Дата и время"
              type="datetime-local"
              required
              value={form.dateTime}
              onChange={(e) => update('dateTime', e.target.value)}
            />
          </div>

          <div className={styles.sectionGap}>
            <div className={styles.sectionTitle}>Этапы собеседования</div>
            <StagesEditor stages={form.stages} onChange={(stages) => update('stages', stages)} />
          </div>

          <div className={styles.formFooter}>
            <Button variant="secondary" type="button" onClick={() => navigate('/interviews')}>
              Отмена
            </Button>
            <Button type="submit" disabled={isSaving}>
              {isSaving ? 'Сохраняем…' : 'Сохранить'}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
