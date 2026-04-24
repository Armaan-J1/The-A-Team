export interface Coordinator {
  id: number;
  name: string;
  department: string;
}

export interface Session {
  id: number;
  name: string;
  slot: 'Morning' | 'Midday' | 'Afternoon';
  totalCapacity: number;
  remainingSeats: number;
}

export interface Participant {
  id: number;
  name: string;
  isAssigned: boolean;
  sessionName: string | null;
}

export interface AssignmentRequest {
  coordinatorId: number;
  participantId: number;
  sessionId: number;
}

export interface ApiError {
  message: string;
}
