import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const CANDIDATES_BASE_URL = `${API_BASE_URL}/candidates`;

const LEVEL_TO_BACKEND = {
  'Базовый': 0,
  'Средний': 1,
  'Высокий': 2,
};

const LEVEL_FROM_BACKEND = {
  0: 'Базовый',
  1: 'Средний',
  2: 'Высокий',
};

function mapSkillsToBackend(skills = []) {
  return skills.map((s) => ({
    ...s,
    level: LEVEL_TO_BACKEND[s.level] ?? s.level,
  }));
}

function mapSkillsFromBackend(skills = []) {
  return skills.map((s) => ({
    ...s,
    level: LEVEL_FROM_BACKEND[s.level] ?? s.level,
  }));
}

function mapCandidateFromBackend(candidate) {
  if (!candidate?.skills) return candidate;
  return { ...candidate, skills: mapSkillsFromBackend(candidate.skills) };
}

function mapCandidatesFromBackend(candidates) {
  return candidates.map(mapCandidateFromBackend);
}

export async function fetchCandidates(search = '', showArchived = false) {
  const params = new URLSearchParams();
  if (search) params.set('search', search);
  if (showArchived) params.set('showArchived', 'true');

  const response = await fetch(`${CANDIDATES_BASE_URL}?${params.toString()}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить кандидатов'), response.status);
  }
  const candidates = await response.json();
  return mapCandidatesFromBackend(candidates);
}

export async function fetchCandidateNames() {
  const response = await fetch(`${CANDIDATES_BASE_URL}/names`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить список кандидатов'), response.status);
  }
  return response.json();
}

export async function fetchCandidateById(id) {
  const response = await fetch(`${CANDIDATES_BASE_URL}/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить кандидата'), response.status);
  }
  const candidate = await response.json();
  return mapCandidateFromBackend(candidate);
}

export async function createCandidate(payload) {
  const body = { ...payload, skills: mapSkillsToBackend(payload.skills) };

  const response = await fetch(CANDIDATES_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось создать карточку кандидата'), response.status);
  }
  const candidate = await response.json();
  return mapCandidateFromBackend(candidate);
}

export async function updateCandidate(id, payload) {
  const body = { ...payload, skills: mapSkillsToBackend(payload.skills) };

  const response = await fetch(`${CANDIDATES_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(body),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сохранить изменения'), response.status);
  }
  const candidate = await response.json();
  return mapCandidateFromBackend(candidate);
}

export async function fetchInterviewsOfCandidate(candidateId) {
  const response = await fetch(`${CANDIDATES_BASE_URL}/${candidateId}/interviews`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить собеседования кандидата'), response.status);
  }
  return response.json();
}

export async function archiveCandidate(id, reason) {
  const response = await fetch(`${CANDIDATES_BASE_URL}/${id}/archive`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ reason }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось архивировать кандидата'), response.status);
  }
  const candidate = await response.json();
  return mapCandidateFromBackend(candidate);
}

export async function restoreCandidate(id) {
  const response = await fetch(`${CANDIDATES_BASE_URL}/${id}/restore`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось восстановить кандидата'), response.status);
  }
  const candidate = await response.json();
  return mapCandidateFromBackend(candidate);
}