import { useState, useEffect, useCallback } from 'react';
import { Toaster, toast } from 'react-hot-toast';
import type { Coordinator, Session, Participant } from './types';
import { getCoordinators, getSessions, getParticipants, assignParticipant } from './api';
import SessionCard from './components/SessionCard';
import ParticipantList from './components/ParticipantList';
import StatsBar from './components/StatsBar';

const DEPT_COLORS: Record<string, string> = {
  'Division A': 'bg-rose-100 text-rose-700',
  'Division B': 'bg-cyan-100 text-cyan-700',
  'Division C': 'bg-emerald-100 text-emerald-700',
};

export default function App() {
  const [coordinators, setCoordinators] = useState<Coordinator[]>([]);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [participants, setParticipants] = useState<Participant[]>([]);

  const [selectedCoordinator, setSelectedCoordinator] = useState<Coordinator | null>(null);
  const [selectedSessionId, setSelectedSessionId] = useState<number | null>(null);
  const [assigning, setAssigning] = useState<number | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([getCoordinators(), getSessions()])
      .then(([coords, sess]) => {
        setCoordinators(coords);
        setSessions(sess);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message ?? 'Failed to connect to the server.');
        setLoading(false);
      });
  }, []);

  const refreshData = useCallback(async (coordinatorId: number) => {
    const [sess, parts] = await Promise.all([
      getSessions(),
      getParticipants(coordinatorId),
    ]);
    setSessions(sess);
    setParticipants(parts);
  }, []);

  const handleCoordinatorChange = async (id: number) => {
    const coord = coordinators.find((c) => c.id === id) ?? null;
    setSelectedCoordinator(coord);
    setSelectedSessionId(null);
    if (coord) {
      const parts = await getParticipants(coord.id);
      setParticipants(parts);
    }
  };

  const handleAssign = async (participantId: number) => {
    if (!selectedCoordinator || !selectedSessionId) return;
    setAssigning(participantId);
    try {
      await assignParticipant({
        coordinatorId: selectedCoordinator.id,
        participantId,
        sessionId: selectedSessionId,
      });
      toast.success('Participant assigned successfully!');
      await refreshData(selectedCoordinator.id);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Assignment failed.';
      toast.error(msg);
    } finally {
      setAssigning(null);
    }
  };

  const deptBadge = selectedCoordinator
    ? (DEPT_COLORS[selectedCoordinator.department] ?? 'bg-slate-100 text-slate-700')
    : '';

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50">
        <div className="text-center">
          <div className="w-10 h-10 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin mx-auto mb-4" />
          <p className="text-slate-500 text-sm">Connecting to server…</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50">
        <div className="text-center max-w-sm">
          <div className="text-5xl mb-4">⚠️</div>
          <h2 className="text-xl font-semibold text-slate-800 mb-2">Connection Error</h2>
          <p className="text-slate-500 text-sm mb-4">{error}</p>
          <p className="text-xs text-slate-400">Make sure the backend is running on port 5252.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <Toaster
        position="top-right"
        toastOptions={{
          success: { style: { background: '#f0fdf4', border: '1px solid #86efac', color: '#166534' } },
          error: { style: { background: '#fef2f2', border: '1px solid #fca5a5', color: '#991b1b' } },
        }}
      />

      {/* Nav */}
      <header className="bg-white border-b border-slate-200 sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-indigo-600 flex items-center justify-center">
              <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </div>
            <div>
              <h1 className="font-bold text-slate-900 leading-tight text-base">Smart Seat Allocation</h1>
              <p className="text-xs text-slate-400">Training Programme Manager</p>
            </div>
          </div>

          <div className="flex items-center gap-3">
            {selectedCoordinator && (
              <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${deptBadge}`}>
                {selectedCoordinator.department}
              </span>
            )}
            <select
              value={selectedCoordinator?.id ?? ''}
              onChange={(e) => handleCoordinatorChange(Number(e.target.value))}
              className="text-sm border border-slate-200 rounded-lg px-3 py-1.5 bg-white text-slate-700 focus:outline-none focus:ring-2 focus:ring-indigo-400 cursor-pointer"
            >
              <option value="">— Select Coordinator —</option>
              {coordinators.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name} ({c.department})
                </option>
              ))}
            </select>
          </div>
        </div>
      </header>

      <main className="max-w-6xl mx-auto px-6 py-8 space-y-8">
        {selectedCoordinator && (
          <StatsBar
            sessions={sessions}
            participants={participants}
            departmentName={selectedCoordinator.department}
          />
        )}

        <section>
          <div className="flex items-baseline gap-2 mb-4">
            <h2 className="text-base font-semibold text-slate-800">Training Sessions</h2>
            <span className="text-xs text-slate-400">Click a session to select it for assignment</span>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {sessions.map((session) => (
              <SessionCard
                key={session.id}
                session={session}
                selected={selectedSessionId === session.id}
                onSelect={() =>
                  setSelectedSessionId(selectedSessionId === session.id ? null : session.id)
                }
              />
            ))}
          </div>
        </section>

        <section>
          <div className="flex items-baseline gap-2 mb-4">
            <h2 className="text-base font-semibold text-slate-800">
              {selectedCoordinator ? `${selectedCoordinator.department} Participants` : 'Participants'}
            </h2>
            {selectedCoordinator && (
              <span className="text-xs text-slate-400">Managed by {selectedCoordinator.name}</span>
            )}
          </div>

          {!selectedCoordinator ? (
            <div className="bg-white rounded-2xl border border-slate-200 p-12 text-center">
              <div className="text-4xl mb-3">👋</div>
              <p className="font-medium text-slate-700 mb-1">Select your coordinator profile</p>
              <p className="text-sm text-slate-400">Use the dropdown in the top-right corner to get started.</p>
            </div>
          ) : (
            <div className="bg-white rounded-2xl border border-slate-200 p-6">
              {selectedSessionId && (
                <div className="mb-5 flex items-center gap-2 text-sm text-indigo-700 bg-indigo-50 border border-indigo-200 rounded-lg px-4 py-2.5">
                  <svg className="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  Assigning to:{' '}
                  <strong>{sessions.find((s) => s.id === selectedSessionId)?.slot} Session</strong>
                  &nbsp;·&nbsp;
                  {sessions.find((s) => s.id === selectedSessionId)?.remainingSeats} seats remaining
                  <button
                    onClick={() => setSelectedSessionId(null)}
                    className="ml-auto text-indigo-400 hover:text-indigo-600 cursor-pointer"
                  >
                    ✕ Clear
                  </button>
                </div>
              )}
              <ParticipantList
                participants={participants}
                sessions={sessions}
                selectedSessionId={selectedSessionId}
                assigning={assigning}
                onAssign={handleAssign}
              />
            </div>
          )}
        </section>

        <footer className="border-t border-slate-200 pt-6 pb-8">
          <p className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-3">System Constraints</p>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3">
            {[
              { icon: '🔒', label: 'Max 20 participants per session' },
              { icon: '🚫', label: 'No duplicate session assignments' },
              { icon: '📊', label: 'Dept. per-session limits enforced' },
              { icon: '✅', label: 'Real-time remaining seat count' },
            ].map((rule) => (
              <div key={rule.label} className="flex items-start gap-2 bg-white rounded-lg border border-slate-200 px-3 py-2.5">
                <span className="text-base">{rule.icon}</span>
                <span className="text-xs text-slate-600">{rule.label}</span>
              </div>
            ))}
          </div>
        </footer>
      </main>
    </div>
  );
}
