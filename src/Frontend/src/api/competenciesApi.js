import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const COMPETENCIES_BASE_URL = `${API_BASE_URL}/competencies`;

export async function fetchCompetencies() {
  const response = await fetch(COMPETENCIES_BASE_URL, {
    method: 'GET',
    credentials: 'include' 
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить справочник компетенций'), response.status);
  }
  return response.json();
}

export async function createCompetency(name) {
  const response = await fetch(COMPETENCIES_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ name }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось добавить компетенцию'), response.status);
  }
  return response.json();
}
