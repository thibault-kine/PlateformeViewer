const STATUSES = [
  {
    color: '#2ECC71',
    glow:  'rgba(46, 204, 113, 0.6)',
    label: 'Libre',
    desc:  'Salle disponible',
  },
  {
    color: '#E74C3C',
    glow:  'rgba(231, 76, 60, 0.6)',
    label: 'Occupée',
    desc:  'Salle en cours d\'utilisation',
  },
  {
    color: '#F39C12',
    glow:  'rgba(243, 156, 18, 0.6)',
    label: 'Bientôt',
    desc:  'Réservation dans < 30 min',
  },
  {
    color: '#7F8C8D',
    glow:  'rgba(127, 140, 141, 0.4)',
    label: 'Inconnu',
    desc:  'Statut non disponible',
  },
];

export default function StatusLegend() {
  return (
    <div className="fixed bottom-7 left-7 z-[100]">
      {/* Card with grid background */}
      <div className="relative bg-[rgba(3,14,36,0.92)] border border-[rgba(0,160,255,0.3)] rounded-lg px-8 py-6 overflow-hidden shadow-neon-modal">

        {/* Animated grid */}
        <div className="absolute inset-0 bg-grid-modal pointer-events-none" />

        {/* Top accent line */}
        <div className="absolute top-0 left-0 right-0 h-[1px] bg-gradient-to-r from-transparent via-neon-blue to-transparent" />

        {/* Header */}
        <p className="relative m-0 font-mono text-sm tracking-[0.35em] text-[rgba(0,180,255,0.7)] uppercase mb-5">
          Statut des salles
        </p>

        {/* Status rows */}
        <div className="relative flex flex-col gap-4">
          {STATUSES.map(({ color, glow, label, desc }) => (
            <div key={label} className="flex items-center gap-4">
              <span
                className="w-5 h-5 rounded-full shrink-0"
                style={{ background: color, boxShadow: `0 0 8px 3px ${glow}` }}
              />
              <div className="flex items-baseline gap-2">
                <span className="font-mono text-base font-bold text-white tracking-wide">
                  {label}
                </span>
                <span className="font-mono text-sm text-[rgba(180,220,255,0.75)] tracking-wide">
                  — {desc}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
