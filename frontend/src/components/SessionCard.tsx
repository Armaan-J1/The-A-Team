import type { Session } from '../types';

interface Props {
  session: Session;
  selected: boolean;
  onSelect: () => void;
}

const SLOT_CONFIG = {
  Morning: {
    icon: '🌅',
    time: '09:00 – 10:30',
    gradient: 'from-amber-400 to-orange-500',
    ring: 'ring-amber-400',
    badge: 'bg-amber-100 text-amber-800',
  },
  Midday: {
    icon: '☀️',
    time: '11:00 – 12:30',
    gradient: 'from-sky-400 to-blue-500',
    ring: 'ring-sky-400',
    badge: 'bg-sky-100 text-sky-800',
  },
  Afternoon: {
    icon: '🌇',
    time: '13:00 – 14:30',
    gradient: 'from-violet-400 to-purple-600',
    ring: 'ring-violet-400',
    badge: 'bg-violet-100 text-violet-800',
  },
};

export default function SessionCard({ session, selected, onSelect }: Props) {
  const config = SLOT_CONFIG[session.slot];
  const occupied = session.totalCapacity - session.remainingSeats;
  const pct = Math.round((occupied / session.totalCapacity) * 100);
  const isFull = session.remainingSeats === 0;

  return (
    <button
      onClick={onSelect}
      disabled={isFull}
      className={[
        'w-full text-left rounded-2xl border-2 p-5 transition-all duration-200 cursor-pointer',
        selected
          ? `border-transparent ring-3 ${config.ring} bg-white shadow-lg scale-[1.02]`
          : 'border-slate-200 bg-white hover:border-slate-300 hover:shadow-md',
        isFull ? 'opacity-60 cursor-not-allowed' : '',
      ].join(' ')}
    >
      {/* Header */}
      <div className="flex items-start justify-between mb-4">
        <div>
          <div className="flex items-center gap-2 mb-1">
            <span className="text-xl">{config.icon}</span>
            <h3 className="font-semibold text-lg text-slate-800">{session.slot}</h3>
          </div>
          <span className="text-sm text-slate-500">{config.time}</span>
        </div>
        {isFull ? (
          <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-red-100 text-red-700">
            Full
          </span>
        ) : selected ? (
          <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-green-100 text-green-700">
            Selected
          </span>
        ) : (
          <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${config.badge}`}>
            {session.remainingSeats} left
          </span>
        )}
      </div>

      {/* Capacity bar */}
      <div className="mb-3">
        <div className="flex justify-between text-xs text-slate-500 mb-1.5">
          <span>{occupied} / {session.totalCapacity} seats</span>
          <span>{pct}%</span>
        </div>
        <div className="h-2 bg-slate-100 rounded-full overflow-hidden">
          <div
            className={`h-full rounded-full bg-gradient-to-r ${config.gradient} transition-all duration-500`}
            style={{ width: `${pct}%` }}
          />
        </div>
      </div>

      {/* Footer */}
      <div className="flex items-center gap-1.5 text-sm text-slate-600">
        <span className="w-2 h-2 rounded-full bg-slate-300 inline-block" />
        {session.remainingSeats === 0
          ? 'No seats available'
          : `${session.remainingSeats} seat${session.remainingSeats !== 1 ? 's' : ''} available`}
      </div>
    </button>
  );
}
