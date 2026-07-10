import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import logo from '../../assets/images/logo.png';
import styles from './login.module.css';

export default function LoginPage() {
  const { signIn, isLoading } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    try {
      await signIn(login, password);
      const redirectTo = location.state?.from || '/candidates';
      navigate(redirectTo, { replace: true });
    } catch (err) {
      setError(err.message || 'Не удалось войти');
    }
  }

  return (
    <div className={styles.page}>
      <div className={styles.showcase}>
        <div className={styles.showcaseEyebrow}>Платформа технических собеседований</div>
        <img src={logo} alt="Логотип компании" className={styles.showcaseLogo} />
      </div>

      <div className={styles.formSide}>
        <div className={styles.formCard}>
          <h2 className={styles.formTitle}>Вход в систему</h2>
          <p className={styles.formSubtitle}>Введите логин и пароль, выданные администратором</p>

          <form className={styles.form} onSubmit={handleSubmit}>
            {error && <div className={styles.error}>{error}</div>}
            <Input
              label="Логин"
              name="login"
              autoComplete="username"
              required
              value={login}
              onChange={(e) => setLogin(e.target.value)}
            />
            <Input
              label="Пароль"
              name="password"
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Входим…' : 'Войти'}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
