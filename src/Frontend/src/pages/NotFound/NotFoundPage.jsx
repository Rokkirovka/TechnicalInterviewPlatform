import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <div style={{ textAlign: 'center', padding: '80px 24px' }}>
      <h1 style={{ fontSize: 28, marginBottom: 12 }}>Страница не найдена</h1>
      <p style={{ color: 'var(--color-text-secondary)', marginBottom: 20 }}>
        Проверьте адрес или вернитесь на главную страницу.
      </p>
      <Link to="/candidates" style={{ fontWeight: 600 }}>← На главную</Link>
    </div>
  );
}
