import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const AUTH_BASE_URL = `${API_BASE_URL}/auth`;

export async function login(loginValue, password) {
  const response = await fetch(`${AUTH_BASE_URL}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ login: loginValue, password }),
  });

  if (response.status === 401) {
    throw new ApiError('Неверный логин или пароль', 401);
  }
  if (response.status === 403) {
    throw new ApiError('Учётная запись отключена администратором', 403);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось войти в систему'), response.status);
  }

  const data = await response.json();
  return data.user;
}

export async function logout() {
  const response = await fetch(`${AUTH_BASE_URL}/logout`, {
    method: 'POST',
    credentials: 'include',
  });

  if (!response.ok && response.status !== 204) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось выйти из системы'), response.status);
  }
  return true;
}

export async function getCurrentUser() {
  const response = await fetch(`${AUTH_BASE_URL}/me`, {
    method: 'GET',
    credentials: 'include',
  });

  if (response.status === 401) {
    return null;
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось получить данные пользователя'), response.status);
  }
  return response.json();
}

export async function refreshSession() {
  const response = await fetch(`${AUTH_BASE_URL}/refresh`, {
    method: 'POST',
    credentials: 'include',
  });

  if (!response.ok) {
    throw new ApiError('Сессия истекла, нужно войти заново', response.status);
  }
  return response.json();
}
