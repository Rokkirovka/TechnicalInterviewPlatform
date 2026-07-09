import { API_BASE_URL, ApiError, extractErrorMessage } from './client';

async function openPdf(url, notFoundMessage, successMessage) {
  const response = await fetch(url, {
    method: 'GET',
    credentials: 'include',
  });

  if (response.status === 404) {
    throw new ApiError(notFoundMessage, 404);
  }
  if (!response.ok) {
    throw new ApiError(await extractErrorMessage(response, 'Не удалось сформировать документ'), response.status);
  }

  const blob = await response.blob();
  const blobUrl = URL.createObjectURL(blob);
  window.open(blobUrl, '_blank');
  setTimeout(() => URL.revokeObjectURL(blobUrl), 60_000);

  return successMessage;
}

export async function printCandidateCard(candidateId) {
  return openPdf(
      `${API_BASE_URL}/candidates/${candidateId}/pdf`,
      'Кандидат не найден',
      'Карточка кандидата открыта в новой вкладке.'
  );
}

export async function printInvite(candidateId) {
  return openPdf(
      `${API_BASE_URL}/candidates/${candidateId}/invite-pdf`,
      'Кандидат не найден',
      'Приглашение открыто в новой вкладке.'
  );
}

export async function printReject(candidateId) {
  return openPdf(
      `${API_BASE_URL}/candidates/${candidateId}/reject-pdf`,
      'Кандидат не найден',
      'Отказ открыт в новой вкладке.'
  );
}

export async function printProtocol(interviewId) {
  return openPdf(
      `${API_BASE_URL}/interviews/${interviewId}/protocol-pdf`,
      'Собеседование не найдено',
      'Протокол собеседования открыт в новой вкладке.'
  );
}
