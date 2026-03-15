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

        {/* Animated grid */}
        <div className="absolute inset-0 bg-grid-modal pointer-events-none" />

        {/* Top accent line */}
        <div className="absolute top-0 left-0 right-0 h-[1px] bg-gradient-to-r from-transparent via-neon-blue to-transparent" />

        {/* Header — always visible, tappable on mobile */}
        <button
          onClick={() => setExpanded(o => !o)}
          className="relative w-full flex items-center justify-between gap-4 px-4 py-3 sm:px-8 sm:py-4 sm:cursor-default"
        >
          <p className="m-0 font-mono text-xs sm:text-sm tracking-[0.35em] text-[rgba(0,180,255,0.7)] uppercase">
            Statut des salles
          </p>
          {/* Dot row shown when collapsed on mobile */}
          <div className="flex gap-1.5 sm:hidden">
            {STATUSES.map(({ color, glow }) => (
              <span
                key={color}
                className="w-2.5 h-2.5 rounded-full"
                style={{ background: color, boxShadow: `0 0 5px 2px ${glow}` }}
              />
            ))}
          </div>
          {/* Chevron on mobile only */}
          <span className={`sm:hidden font-mono text-[rgba(0,180,255,0.6)] text-xs transition-transform duration-200 ${expanded ? 'rotate-180' : ''}`}>
            ▾
          </span>
        </button>

        {/* Status rows — always visible on sm+, toggle on mobile */}
        <div className={`px-4 pb-3 sm:px-8 sm:pb-6 sm:block ${expanded ? 'block' : 'hidden'}`}>
          <div className="relative flex flex-col gap-2 sm:gap-4">
            {STATUSES.map(({ color, glow, label, desc }) => (
              <div key={label} className="flex items-center gap-3 sm:gap-4">
                <span
                  className="w-3 h-3 sm:w-5 sm:h-5 rounded-full shrink-0"
                  style={{ background: color, boxShadow: `0 0 8px 3px ${glow}` }}
                />
                <div className="flex items-baseline gap-2">
                  <span className="font-mono text-sm sm:text-base font-bold text-white tracking-wide">
                    {label}
                  </span>
                  <span className="hidden sm:inline font-mono text-sm text-[rgba(180,220,255,0.75)] tracking-wide">
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
