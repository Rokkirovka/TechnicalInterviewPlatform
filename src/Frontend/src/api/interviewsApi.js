import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const INTERVIEWS_BASE_URL = `${API_BASE_URL}/interviews`;

function localDateTimeToUtcIso(localValue) {
  if (!localValue) return null;
  return new Date(localValue).toISOString();
}

function utcIsoToLocalDateTimeInput(utcIso) {
  if (!utcIso) return '';
  const date = new Date(utcIso);
  const pad = (n) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

function mapInterviewFromBackend(interview) {
  if (!interview?.scheduledAt) return interview;
  return { ...interview, scheduledAt: utcIsoToLocalDateTimeInput(interview.scheduledAt) };
}

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
  const interviews = await response.json();
  return interviews.map(mapInterviewFromBackend);
}

export async function fetchInterviewById(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить собеседование'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function createInterview(payload) {
  const body = { ...payload, scheduledAt: localDateTimeToUtcIso(payload.scheduledAt) };

  const response = await fetch(INTERVIEWS_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось создать собеседование'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function updateInterview(id, payload) {
  const body = { ...payload, scheduledAt: localDateTimeToUtcIso(payload.scheduledAt) };

  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сохранить изменения'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function markInterviewPassed(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/mark-passed`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось отметить собеседование как проведённое'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function submitDecision(interviewId, decision) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${interviewId}/decision`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ decision }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось зафиксировать решение'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function archiveInterview(id, reason) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/archive`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ reason }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось архивировать собеседование'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}

export async function restoreInterview(id) {
  const response = await fetch(`${INTERVIEWS_BASE_URL}/${id}/restore`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось восстановить собеседование'), response.status);
  }
  const interview = await response.json();
  return mapInterviewFromBackend(interview);
}