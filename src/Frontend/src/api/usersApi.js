import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const USERS_BASE_URL = `${API_BASE_URL}/users`;

export async function fetchUsers(search = '', showArchived = false) {
  const params = new URLSearchParams();
  if (search) params.set('search', search);
  if (showArchived) params.set('showArchived', 'true');

  const response = await fetch(`${USERS_BASE_URL}?${params.toString()}`, {
    method: 'GET',
    credentials: 'include',
  });

  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить пользователей'), response.status);
  }
  return response.json();
}

export async function fetchUserById(id) {
  const response = await fetch(`${USERS_BASE_URL}/${id}`, {
    method: 'GET',
    credentials: 'include',
  });

  if (response.status === 404) {
    throw new ApiError('Пользователь не найден', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить пользователя'), response.status);
  }
  return response.json();
}

export async function createUser(payload) {
  const response = await fetch(USERS_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload),
  });

  if (response.status === 409) {
    throw new ApiError('Пользователь с таким логином уже существует', 409);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось создать пользователя'), response.status);
  }
  return response.json();
}

export async function updateUser(id, payload) {
  const response = await fetch(`${USERS_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ ...payload, password: payload.password || null }),
  });

  if (response.status === 404) {
    throw new ApiError('Пользователь не найден', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сохранить изменения'), response.status);
  }
  return response.json();
}

export async function archiveUser(id, reason) {
  const response = await fetch(`${USERS_BASE_URL}/${id}/archive`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ reason }),
  });

  if (response.status === 404) {
    throw new ApiError('Пользователь не найден', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось архивировать пользователя'), response.status);
  }
  return response.json();
}

export async function restoreUser(id) {
  const response = await fetch(`${USERS_BASE_URL}/${id}/restore`, {
    method: 'POST',
    credentials: 'include',
  });

  if (response.status === 404) {
    throw new ApiError('Пользователь не найден', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось восстановить пользователя'), response.status);
  }
  return response.json();
}
