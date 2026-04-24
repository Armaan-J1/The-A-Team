import type { Session, Participant } from '../types';

interface Props {
  sessions: Session[];
  participants: Participant[];
  departmentName: string;
}

export default function StatsBar({ sessions, participants, departmentName }: Props) {
  const totalSeats = sessions.reduce((sum, s) => sum + s.totalCapacity, 0);
  const filledSeats = sessions.reduce((sum, s) => sum + (s.totalCapacity - s.remainingSeats), 0);
  const assignedParticipants = participants.filter((p) => p.isAssigned).length;
  const unassignedParticipants = participants.filter((p) => !p.isAssigned).length;

  const stats = [
    { label: 'Total Seats', value: totalSeats, color: 'text-slate-700' },
    { label: 'Seats Filled', value: filledSeats, color: 'text-indigo-600' },
    { label: `${departmentName} Assigned`, value: assignedParticipants, color: 'text-green-600' },
    { label: `${departmentName} Pending`, value: unassignedParticipants, color: 'text-amber-600' },
  ];

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
      {stats.map((stat) => (
        <div key={stat.label} className="bg-white rounded-xl border border-slate-200 px-5 py-4">
          <div className={`text-2xl font-bold ${stat.color}`}>{stat.value}</div>
          <div className="text-xs text-slate-500 mt-0.5">{stat.label}</div>
        </div>
      ))}
    </div>
  );
}
