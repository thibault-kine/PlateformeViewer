import { useState } from 'react';

const STATUSES = [
  { color: '#2ECC71', glow: 'rgba(46, 204, 113, 0.6)',  label: 'Libre',   desc: 'Salle disponible' },
  { color: '#E74C3C', glow: 'rgba(231, 76, 60, 0.6)',   label: 'Occupée', desc: 'Salle en cours d\'utilisation' },
  { color: '#F39C12', glow: 'rgba(243, 156, 18, 0.6)',  label: 'Bientôt', desc: 'Réservation dans < 30 min' },
  { color: '#7F8C8D', glow: 'rgba(127, 140, 141, 0.4)', label: 'Inconnu', desc: 'Statut non disponible' },
];

export default function StatusLegend() {
  const [expanded, setExpanded] = useState(false);

  return (
    <div className="fixed bottom-7 left-7 z-[100]">
      <div className="relative bg-[rgba(3,14,36,0.92)] border border-[rgba(0,160,255,0.3)] rounded-lg overflow-hidden shadow-neon-modal">

        <div className="absolute inset-0 bg-grid-modal pointer-events-none" />
        <div className="absolute top-0 left-0 right-0 h-[1px] bg-gradient-to-r from-transparent via-neon-blue to-transparent" />

        {/* Header — tappable on touch devices to toggle */}
        <button
          onClick={() => setExpanded(o => !o)}
          className="relative w-full flex items-center justify-between gap-4 px-8 py-4 touch:px-4 touch:py-3 touch:cursor-pointer"
        >
          <p className="m-0 font-mono text-sm touch:text-xs tracking-[0.35em] text-[rgba(0,180,255,0.7)] uppercase">
            Statut des salles
          </p>
          {/* Dot row shown collapsed on touch */}
          <div className="hidden touch:flex gap-1.5">
            {STATUSES.map(({ color, glow }) => (
              <span
                key={color}
                className="w-2.5 h-2.5 rounded-full"
                style={{ background: color, boxShadow: `0 0 5px 2px ${glow}` }}
              />
            ))}
          </div>
          <span className={`hidden touch:inline font-mono text-[rgba(0,180,255,0.6)] text-xs transition-transform duration-200 ${expanded ? 'rotate-180' : ''}`}>
            ▾
          </span>
        </button>

        {/* Status rows — always visible on non-touch, toggle on touch */}
        <div className={`px-8 pb-6 touch:px-4 touch:pb-3 touch:hidden ${expanded ? '!block' : ''}`}>
          <div className="relative flex flex-col gap-4 touch:gap-2">
            {STATUSES.map(({ color, glow, label, desc }) => (
              <div key={label} className="flex items-center gap-4 touch:gap-3">
                <span
                  className="w-5 h-5 touch:w-3 touch:h-3 rounded-full shrink-0"
                  style={{ background: color, boxShadow: `0 0 8px 3px ${glow}` }}
                />
                <div className="flex items-baseline gap-2">
                  <span className="font-mono text-base touch:text-sm font-bold text-white tracking-wide">
                    {label}
                  </span>
                  <span className="font-mono text-sm text-[rgba(180,220,255,0.75)] tracking-wide touch:hidden">
                    — {desc}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>

      </div>
    </div>
  );
}
