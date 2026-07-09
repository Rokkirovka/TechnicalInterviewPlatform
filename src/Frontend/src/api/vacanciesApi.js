import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const VACANCIES_BASE_URL = `${API_BASE_URL}/vacancies`;

export async function fetchVacancies(search = '', showArchived = false) {
  const params = new URLSearchParams();
  if (search) params.set('search', search);
  if (showArchived) params.set('showArchived', 'true');

  const response = await fetch(`${VACANCIES_BASE_URL}?${params.toString()}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить вакансии'), response.status);
  }
  return response.json();
}

export async function fetchVacancyById(id) {
  const response = await fetch(`${VACANCIES_BASE_URL}/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить вакансию'), response.status);
  }
  return response.json();
}

export async function createVacancy(payload) {
  const response = await fetch(VACANCIES_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось создать вакансию'), response.status);
  }
  return response.json();
}

export async function updateVacancy(id, payload) {
  const response = await fetch(`${VACANCIES_BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сохранить вакансию'), response.status);
  }
  return response.json();
}

export async function archiveVacancy(id, reason) {
  const response = await fetch(`${VACANCIES_BASE_URL}/${id}/archive`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ reason }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось архивировать вакансию'), response.status);
  }
  return response.json();
}

export async function restoreVacancy(id) {
  const response = await fetch(`${VACANCIES_BASE_URL}/${id}/restore`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось восстановить вакансию'), response.status);
  }
}
