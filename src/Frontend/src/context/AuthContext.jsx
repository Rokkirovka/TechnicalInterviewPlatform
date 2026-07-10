import { createContext, useContext, useState, useCallback, useMemo, useEffect } from 'react';
import * as authApi from '../api/authApi';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isCheckingSession, setIsCheckingSession] = useState(true);
  
  useEffect(() => {
    (async () => {
      try {
        const currentUser = await authApi.getCurrentUser();
        setUser(currentUser);
      } catch {
        setUser(null);
      } finally {
        setIsCheckingSession(false);
      }
    })();
  }, []);

  const isLoggedIn = Boolean(user);
  useEffect(() => {
    if (!isLoggedIn) return;

    const REFRESH_INTERVAL_MS = 10 * 60 * 1000;

    const intervalId = setInterval(async () => {
      try {
        await authApi.refreshSession();
      } catch {
        setUser(null);
      }
    }, REFRESH_INTERVAL_MS);

    return () => clearInterval(intervalId);
  }, [isLoggedIn]);

  const signIn = useCallback(async (login, password) => {
    setIsLoading(true);
    setError(null);
    try {
      const loggedInUser = await authApi.login(login, password);
      setUser(loggedInUser);
      return loggedInUser;
    } catch (err) {
      setError(err.message || 'Не удалось войти в систему');
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);
  
  const signOut = useCallback(async () => {
    try {
      await authApi.logout();
    } catch {
    } finally {
      setUser(null);
    }
  }, []);

  const hasRole = useCallback(
    (...roles) => !!user && user.roles.some((role) => roles.includes(role)),
    [user]
  );

  const value = useMemo(
    () => ({ user, isLoading, isCheckingSession, error, signIn, signOut, hasRole }),
    [user, isLoading, isCheckingSession, error, signIn, signOut, hasRole]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth должен использоваться внутри <AuthProvider>');
  return context;
}
