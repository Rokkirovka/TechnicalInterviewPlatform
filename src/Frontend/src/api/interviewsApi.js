import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const INTERVIEWS_BASE_URL = `${API_BASE_URL}/interviews`;

export async function fetchInterviews(search = '', showArchived = false) {
  const params = new URLSearchParams();
  if (search) params.set('search', search);
  if (showArchived) params.set('showArchived', 'true');

  const response = await fetch(`${INTERVIEWS_BASE_URL}?${params.toString()}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить собеседования'), response.status);
  }
  return response.json();
}

export async function fetchInterviewById(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить собеседование'), response.status);
  }
  return response.json();
}

export async function createInterview(payload) {
  const response = await fetch(INTERVIEWS_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось создать собеседование'), response.status);
  }
  return response.json();
}

export async function updateInterview(id, payload) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload),
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сохранить изменения'), response.status);
  }
  return response.json();
}

export async function markInterviewPassed(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/mark-passed`, {
    method: 'POST',
    credentials: 'include',
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось отметить собеседование как проведённое'), response.status);
  }
  return response.json();
}

export async function submitDecision(interviewId, decision) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${interviewId}/decision`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ decision }),
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось зафиксировать решение'), response.status);
  }
  return response.json();
}

export async function archiveInterview(id, reason) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/archive`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ reason }),
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось архивировать собеседование'), response.status);
  }
  return response.json();
}

export async function restoreInterview(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/restore`, {
    method: 'POST',
    credentials: 'include',
  });
  if (response.status === 404) {
    throw new ApiError('Собеседование не найдено', 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось восстановить собеседование'), response.status);
  }
  return response.json();
}
