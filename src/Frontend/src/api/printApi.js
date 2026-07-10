import { ApiError, extractErrorMessage } from './client';

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
      `/pdf-templates/2/candidates/${candidateId}`,
      'Кандидат не найден',
      'Карточка кандидата открыта в новой вкладке.'
  );
}

export async function printInvite(candidateId) {
  return openPdf(
      `/pdf-templates/4/candidates/${candidateId}`,
      'Кандидат не найден',
      'Приглашение открыто в новой вкладке.'
  );
}

export async function printReject(candidateId) {
  return openPdf(
      `/pdf-templates/3/candidates/${candidateId}`,
      'Кандидат не найден',
      'Отказ открыт в новой вкладке.'
  );
}

export async function printProtocol(interviewId) {
  return openPdf(
      `/pdf-templates/1/interviews/${interviewId}`,
      'Собеседование не найдено',
      'Протокол собеседования открыт в новой вкладке.'
  );
}
