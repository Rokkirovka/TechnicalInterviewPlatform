import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Loader } from '../components/ui/Loader';

export default function ProtectedRoute({ allowedRoles }) {
  const { user, hasRole, isCheckingSession } = useAuth();
  const location = useLocation();

  if (isCheckingSession) {
    return <Loader label="Проверяем сессию…" />;
  }

  if (!user) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (allowedRoles && !hasRole(...allowedRoles)) {
    return <Navigate to="/candidates" replace />;
  }

  return <Outlet />;
}
