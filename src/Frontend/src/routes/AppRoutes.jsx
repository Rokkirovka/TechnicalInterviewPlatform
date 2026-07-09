import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import ProtectedRoute from './ProtectedRoute';
import AppLayout from '../components/layout/AppLayout';

import LoginPage from '../pages/Login/LoginPage';
import UsersRegistryPage from '../pages/Users/UsersRegistryPage';
import UserFormPage from '../pages/Users/UserFormPage';
import VacanciesRegistryPage from '../pages/Vacancies/VacanciesRegistryPage';
import VacancyFormPage from '../pages/Vacancies/VacancyFormPage';
import CandidatesRegistryPage from '../pages/Candidates/CandidatesRegistryPage';
import CandidateFormPage from '../pages/Candidates/CandidateFormPage';
import CandidateCardPage from '../pages/Candidates/CandidateCardPage';
import InterviewsRegistryPage from '../pages/Interviews/InterviewsRegistryPage';
import InterviewFormPage from '../pages/Interviews/InterviewFormPage';
import InterviewCardPage from '../pages/Interviews/InterviewCardPage';
import NotFoundPage from '../pages/NotFound/NotFoundPage';

import { Loader } from '../components/ui/Loader';
import { ROLES } from '../utils/constants';

export default function AppRoutes() {
  const { user, isCheckingSession } = useAuth();

  return (
    <Routes>
      <Route
        path="/login"
        element={
          isCheckingSession ? (
            <Loader label="Проверяем сессию…" />
          ) : user ? (
            <Navigate to="/candidates" replace />
          ) : (
            <LoginPage />
          )
        }
      />

      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN]} />}>
            <Route path="/users" element={<UsersRegistryPage />} />
            <Route path="/users/new" element={<UserFormPage />} />
            <Route path="/users/:id" element={<UserFormPage />} />
          </Route>

            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.HR]} />}>
                <Route path="/vacancies" element={<VacanciesRegistryPage />} />
                <Route path="/vacancies/new" element={<VacancyFormPage />} />
                <Route path="/vacancies/:id" element={<VacancyFormPage />} />
            </Route>
            
          <Route path="/candidates" element={<CandidatesRegistryPage />} />
          <Route element={<ProtectedRoute allowedRoles={[ROLES.HR]} />}>
            <Route path="/candidates/new" element={<CandidateFormPage />} />
          </Route>
          <Route path="/candidates/:id" element={<CandidateCardPage />} />
            
          <Route element={<ProtectedRoute allowedRoles={[ROLES.HR]} />}>
            <Route path="/interviews" element={<InterviewsRegistryPage />} />
            <Route path="/interviews/new" element={<InterviewFormPage />} />
          </Route>
          <Route element={<ProtectedRoute allowedRoles={[ROLES.HR, ROLES.APPROVER]} />}>
            <Route path="/interviews/:id" element={<InterviewCardPage />} />
          </Route>

          <Route path="/" element={<Navigate to="/candidates" replace />} />
        </Route>
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
