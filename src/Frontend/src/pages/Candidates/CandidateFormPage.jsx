import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createCandidate } from '../../api/candidatesApi';
import { fetchSkills } from '../../api/dictionariesApi';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import SkillsEditor from '../../components/SkillsEditor';
import { Loader } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

const EMPTY_FORM = {
  lastName: '',
  firstName: '',
  middleName: '',
  phone: '',
  city: '',
  education: '',
  experience: '',
  skills: [],
};

export default function CandidateFormPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState(EMPTY_FORM);
  const [skillOptions, setSkillOptions] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      const skills = await fetchSkills();
      setSkillOptions(skills);
      setIsLoading(false);
    })();
  }, []);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setIsSaving(true);
    try {
      const candidate = await createCandidate(form);
      navigate(`/candidates/${candidate.id}`);
    } catch (err) {
      setError(err.message || 'Не удалось создать карточку кандидата');
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) return <Loader label="Готовим форму…" />;

  return (
    <div>
      <Link to="/candidates" className={styles.backLink}>← К реестру кандидатов</Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>Новая карточка кандидата</h2>
      </div>

      <Card>
        <form onSubmit={handleSubmit}>
          {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

          <div className={styles.sectionTitle}>Личные данные</div>
          <div className={styles.grid}>
            <Input label="Фамилия" required value={form.lastName} onChange={(e) => update('lastName', e.target.value)} />
            <Input label="Имя" required value={form.firstName} onChange={(e) => update('firstName', e.target.value)} />
            <Input label="Отчество" value={form.middleName} onChange={(e) => update('middleName', e.target.value)} />
            <Input label="Телефон" required value={form.phone} onChange={(e) => update('phone', e.target.value)} placeholder="+7 900 000-00-00" />
            <Input label="Город" value={form.city} onChange={(e) => update('city', e.target.value)} />
          </div>

          <div className={styles.sectionGap}>
            <div className={styles.sectionTitle}>Образование и опыт</div>
            <div className={styles.grid}>
              <Input
                label="Образование"
                multiline
                value={form.education}
                onChange={(e) => update('education', e.target.value)}
              />
              <Input
                label="Предыдущий опыт работы"
                multiline
                value={form.experience}
                onChange={(e) => update('experience', e.target.value)}
              />
            </div>
          </div>

          <div className={styles.sectionGap}>
            <div className={styles.sectionTitle}>Навыки</div>
            <SkillsEditor
              skills={form.skills}
              skillOptions={skillOptions}
              onChange={(skills) => update('skills', skills)}
            />
          </div>

          <div className={styles.formFooter}>
            <Button variant="secondary" type="button" onClick={() => navigate('/candidates')}>
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
