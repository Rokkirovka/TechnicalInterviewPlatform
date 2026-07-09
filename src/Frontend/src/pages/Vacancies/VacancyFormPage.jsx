import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { fetchVacancyById, createVacancy, updateVacancy } from '../../api/vacanciesApi';
import { fetchCompetencies, createCompetency } from '../../api/competenciesApi';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import CompetenciesEditor from '../../components/CompetenciesEditor';
import { Loader } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

const EMPTY_FORM = {
  title: '',
  description: '',
  department: '',
  competencyIds: [],
};

export default function VacancyFormPage() {
  const { id } = useParams();
  const isEditMode = Boolean(id);
  const navigate = useNavigate();

  const [form, setForm] = useState(EMPTY_FORM);
  const [allCompetencies, setAllCompetencies] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      setIsLoading(true);
      const [competencies, vacancy] = await Promise.all([
        fetchCompetencies(),
        isEditMode ? fetchVacancyById(id) : Promise.resolve(null),
      ]);
      setAllCompetencies(competencies);
      if (vacancy) setForm({ ...EMPTY_FORM, ...vacancy });
      setIsLoading(false);
    })();
  }, [id, isEditMode]);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }
  
  async function handleCreateCompetency(name) {
    const created = await createCompetency(name);
    setAllCompetencies((prev) => [...prev, created]);
    return created;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    if (!form.title.trim()) {
      setError('Укажите название вакансии');
      return;
    }

    setIsSaving(true);
    try {
      if (isEditMode) {
        await updateVacancy(id, form);
      } else {
        await createVacancy(form);
      }
      navigate('/vacancies');
    } catch (err) {
      setError(err.message || 'Не удалось сохранить вакансию');
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) return <Loader label="Загружаем карточку вакансии…" />;

  return (
    <div>
      <Link to="/vacancies" className={styles.backLink}>← К реестру вакансий</Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>
          {isEditMode ? 'Карточка вакансии' : 'Новая вакансия'}
        </h2>
      </div>

      <Card>
        <form onSubmit={handleSubmit}>
          {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

          <div className={styles.grid}>
            <Input
              label="Название вакансии"
              required
              value={form.title}
              onChange={(e) => update('title', e.target.value)}
            />
            <Input
              label="Отдел"
              value={form.department}
              onChange={(e) => update('department', e.target.value)}
            />
            <div className={styles.gridFull}>
              <Input
                label="Описание вакансии"
                multiline
                value={form.description}
                onChange={(e) => update('description', e.target.value)}
                placeholder="Краткое описание обязанностей и требований"
              />
            </div>
          </div>

          <div className={styles.sectionGap}>
            <div className={styles.sectionTitle}>Компетенции для матрицы собеседования</div>
            <p style={{ fontSize: 13.5, color: 'var(--color-text-secondary)', marginBottom: 14 }}>
              Отметьте компетенции из общего справочника, которые войдут в матрицу оценки
              при создании нового собеседования на эту вакансию. Если ни одна не выбрана —
              будет использован общий шаблон.
            </p>
            <CompetenciesEditor
              competencyIds={form.competencyIds}
              allCompetencies={allCompetencies}
              onChange={(competencyIds) => update('competencyIds', competencyIds)}
              onCreateCompetency={handleCreateCompetency}
            />
          </div>

          <div className={styles.formFooter}>
            <Button variant="secondary" type="button" onClick={() => navigate('/vacancies')}>
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
