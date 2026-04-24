import type { Participant, Session } from '../types';

interface Props {
  participants: Participant[];
  sessions: Session[];
  selectedSessionId: number | null;
  assigning: number | null;
  onAssign: (participantId: number) => void;
}

const SLOT_COLORS: Record<string, string> = {
  Morning: 'bg-amber-100 text-amber-800',
  Midday: 'bg-sky-100 text-sky-800',
  Afternoon: 'bg-violet-100 text-violet-800',
};

export default function ParticipantList({
  participants,
  sessions,
  selectedSessionId,
  assigning,
  onAssign,
}: Props) {
  const selectedSession = sessions.find((s) => s.id === selectedSessionId);
  const unassigned = participants.filter((p) => !p.isAssigned);
  const assigned = participants.filter((p) => p.isAssigned);

  if (participants.length === 0) {
    return (
      <div className="text-center py-16 text-slate-400">
        <div className="text-4xl mb-3">👤</div>
        <p className="font-medium">No participants found</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Unassigned */}
      <div>
        <div className="flex items-center gap-2 mb-3">
          <span className="text-sm font-semibold text-slate-700">
            Unassigned ({unassigned.length})
          </span>
          {!selectedSessionId && unassigned.length > 0 && (
            <span className="text-xs text-amber-600 bg-amber-50 border border-amber-200 px-2 py-0.5 rounded-full">
              Select a session above to assign
            </span>
          )}
        </div>

        {unassigned.length === 0 ? (
          <p className="text-sm text-slate-400 italic">All participants have been assigned.</p>
        ) : (
          <div className="divide-y divide-slate-100 rounded-xl border border-slate-200 overflow-hidden bg-white">
            {unassigned.map((p) => (
              <div
                key={p.id}
                className="flex items-center justify-between px-4 py-3 hover:bg-slate-50 transition-colors"
              >
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 rounded-full bg-slate-200 flex items-center justify-center text-slate-600 text-xs font-bold">
                    {p.name.split(' ').map((n) => n[0]).join('').slice(0, 2).toUpperCase()}
                  </div>
                  <span className="text-sm font-medium text-slate-800">{p.name}</span>
                </div>

                <button
                  onClick={() => onAssign(p.id)}
                  disabled={!selectedSessionId || assigning === p.id || selectedSession?.remainingSeats === 0}
                  className={[
                    'text-xs font-semibold px-3 py-1.5 rounded-lg transition-all',
                    selectedSessionId && selectedSession && selectedSession.remainingSeats > 0
                      ? assigning === p.id
                        ? 'bg-indigo-200 text-indigo-500 cursor-wait'
                        : 'bg-indigo-600 text-white hover:bg-indigo-700 active:scale-95 cursor-pointer'
                      : 'bg-slate-100 text-slate-400 cursor-not-allowed',
                  ].join(' ')}
                >
                  {assigning === p.id ? 'Assigning…' : selectedSessionId ? 'Assign' : 'Select session'}
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Assigned */}
      {assigned.length > 0 && (
        <div>
          <div className="flex items-center gap-2 mb-3">
            <span className="text-sm font-semibold text-slate-700">
              Assigned ({assigned.length})
            </span>
          </div>
          <div className="divide-y divide-slate-100 rounded-xl border border-slate-200 overflow-hidden bg-white">
            {assigned.map((p) => {
              const slotName = sessions.find((s) => s.name === p.sessionName)?.slot;
              const colorClass = slotName ? SLOT_COLORS[slotName] : 'bg-green-100 text-green-800';
              return (
                <div
                  key={p.id}
                  className="flex items-center justify-between px-4 py-3 bg-slate-50"
                >
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full bg-green-200 flex items-center justify-center text-green-700 text-xs font-bold">
                      {p.name.split(' ').map((n) => n[0]).join('').slice(0, 2).toUpperCase()}
                    </div>
                    <span className="text-sm font-medium text-slate-600">{p.name}</span>
                  </div>
                  <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${colorClass}`}>
                    {p.sessionName || 'Assigned'}
                  </span>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
