import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { fetchUserById, createUser, updateUser } from '../../api/usersApi';
import { ROLES, ROLE_LABELS } from '../../utils/constants';
import Input from '../../components/ui/Input';
import Checkbox from '../../components/ui/Checkbox';
import Button from '../../components/ui/Button';
import Card from '../../components/ui/Card';
import { Loader } from '../../components/ui/Loader';
import styles from '../shared/form.module.css';

const EMPTY_FORM = {
  login: '',
  password: '',
  lastName: '',
  firstName: '',
  middleName: '',
  roles: [],
  active: true,
};

export default function UserFormPage() {
  const { id } = useParams();
  const isEditMode = Boolean(id);
  const navigate = useNavigate();

  const [form, setForm] = useState(EMPTY_FORM);
  const [isLoading, setIsLoading] = useState(isEditMode);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!isEditMode) return;
    (async () => {
      setIsLoading(true);
      try {
        const user = await fetchUserById(id);
        setForm({ ...EMPTY_FORM, ...user, password: '' });
      } finally {
        setIsLoading(false);
      }
    })();
  }, [id, isEditMode]);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  function toggleRole(role) {
    setForm((prev) => ({
      ...prev,
      roles: prev.roles.includes(role)
        ? prev.roles.filter((r) => r !== role)
        : [...prev.roles, role],
    }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    if (form.roles.length === 0) {
      setError('Выберите хотя бы одну роль для пользователя');
      return;
    }
    if (!isEditMode && !form.password) {
      setError('Укажите пароль для нового пользователя');
      return;
    }

    setIsSaving(true);
    try {
      if (isEditMode) {
        await updateUser(id, form);
      } else {
        await createUser(form);
      }
      navigate('/users');
    } catch (err) {
      setError(err.message || 'Не удалось сохранить пользователя');
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) return <Loader label="Загружаем карточку пользователя…" />;

  return (
    <div>
      <Link to="/users" className={styles.backLink}>← К реестру пользователей</Link>
      <div className={styles.header}>
        <h2 className={styles.heading}>
          {isEditMode ? 'Карточка пользователя' : 'Новый пользователь'}
        </h2>
      </div>

      <Card>
        <form onSubmit={handleSubmit}>
          {error && <div className={`${styles.banner} ${styles.bannerError}`}>{error}</div>}

          <div className={styles.grid}>
            <Input
              label="Логин"
              required
              value={form.login}
              onChange={(e) => update('login', e.target.value)}
              disabled={isEditMode}
              hint={isEditMode ? 'Логин нельзя изменить' : undefined}
            />
            <Input
              label={isEditMode ? 'Новый пароль' : 'Пароль'}
              type="password"
              required={!isEditMode}
              value={form.password}
              onChange={(e) => update('password', e.target.value)}
              placeholder={isEditMode ? 'Оставьте пустым, чтобы не менять' : ''}
            />
            <Input
              label="Фамилия"
              required
              value={form.lastName}
              onChange={(e) => update('lastName', e.target.value)}
            />
            <Input
              label="Имя"
              required
              value={form.firstName}
              onChange={(e) => update('firstName', e.target.value)}
            />
            <Input
              label="Отчество"
              value={form.middleName}
              onChange={(e) => update('middleName', e.target.value)}
            />
          </div>

          <div className={styles.sectionGap}>
            <div className={styles.sectionTitle}>Роли</div>
            <div style={{ display: 'flex', gap: 20, flexWrap: 'wrap' }}>
              {Object.values(ROLES).map((role) => (
                <Checkbox
                  key={role}
                  label={ROLE_LABELS[role]}
                  checked={form.roles.includes(role)}
                  onChange={() => toggleRole(role)}
                />
              ))}
            </div>
          </div>

          {isEditMode && (
            <div className={styles.sectionGap}>
              <Checkbox
                label="Учётная запись активна"
                checked={form.active}
                onChange={() => update('active', !form.active)}
              />
            </div>
          )}

          <div className={styles.formFooter}>
            <Button variant="secondary" type="button" onClick={() => navigate('/users')}>
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
