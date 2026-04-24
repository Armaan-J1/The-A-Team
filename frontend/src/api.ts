import type { Coordinator, Session, Participant, AssignmentRequest } from './types';

const BASE_URL = 'http://localhost:5252/api';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Request failed: ${res.status}`);
  }
  return res.json() as Promise<T>;
}

export async function getCoordinators(): Promise<Coordinator[]> {
  const res = await fetch(`${BASE_URL}/coordinators`);
  return handleResponse<Coordinator[]>(res);
}

export async function getSessions(): Promise<Session[]> {
  const res = await fetch(`${BASE_URL}/sessions`);
  return handleResponse<Session[]>(res);
}

export async function getParticipants(coordinatorId: number): Promise<Participant[]> {
  const res = await fetch(`${BASE_URL}/participants?coordinatorId=${coordinatorId}`);
  return handleResponse<Participant[]>(res);
}

export async function assignParticipant(request: AssignmentRequest): Promise<void> {
  const res = await fetch(`${BASE_URL}/assignments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Assignment failed: ${res.status}`);
  }
}
