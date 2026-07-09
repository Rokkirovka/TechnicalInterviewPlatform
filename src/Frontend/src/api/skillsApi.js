import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

const SKILLS_BASE_URL = `${API_BASE_URL}/skills`;

export async function fetchSkills() {
  const response = await fetch(SKILLS_BASE_URL, { method: 'GET', credentials: 'include' });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось загрузить справочник навыков'), response.status);
  }
  const skills = await response.json();
  return skills.map((s) => s.name);
}

export async function createSkill(name) {
  const response = await fetch(SKILLS_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({ name }),
  });
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось добавить навык'), response.status);
  }
}
