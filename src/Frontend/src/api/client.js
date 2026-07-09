export const API_BASE_URL = '/api';

export class ApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

export async function extractErrorMessage(response, fallback) {
  try {
    const data = await response.json();
    return data.message || fallback;
  } catch {
    return fallback;
  }
}
