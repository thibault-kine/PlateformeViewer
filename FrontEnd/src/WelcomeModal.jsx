import { useState } from 'react';

const FEATURES = [
  {
    icon: '⬡',
    title: '39 Salles modélisées',
    desc: 'Chaque salle de La Plateforme est reproduite en 3D et navigable librement.',
  },
  {
    icon: '◈',
    title: 'Données en temps réel',
    desc: 'Disponibilité, capacité et statut de chaque salle via appels API live.',
  },
  {
    icon: '◎',
    title: 'Navigation immersive',
    desc: 'Explorez le bâtiment Hangar des Docks des Suds en vue libre.',
  },
];

function Divider() {
  return (
    <div className="h-px bg-gradient-to-r from-transparent via-[rgba(0,160,255,0.4)] to-transparent my-7" />
  );
}

export default function WelcomeModal() {
  const [open, setOpen] = useState(true);

  if (!open) return null;

  return (
    /* Full-screen overlay — dark + animated grid so Unity scene shows through */
    <div
      className="fixed inset-0 z-[300] animate-overlay-in"
      onClick={() => setOpen(false)}
    >
      {/* Dark tint */}
      <div className="absolute inset-0 bg-[rgba(2,11,26,0.65)]" />
      {/* Animated neon grid */}
      <div className="absolute inset-0 bg-grid-neon animate-grid-drift opacity-50 pointer-events-none" />

      {/* Centred modal */}
      <div className="relative flex items-center justify-center w-full h-full">
        <div
          className="relative w-[900px] bg-[rgba(3,14,36,0.96)] border border-[rgba(0,160,255,0.3)] rounded-xl px-14 py-12 overflow-hidden shadow-neon-modal animate-modal-in"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Inner grid */}
          <div className="absolute inset-0 bg-grid-modal pointer-events-none" />

          {/* Top accent line */}
          <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-neon-blue to-transparent" />

          {/* Header */}
          <div className="relative flex flex-col items-center gap-3 mb-1">
            <p className="m-0 font-mono text-base tracking-[0.45em] text-[rgba(0,180,255,0.6)] uppercase">
              Bienvenue dans
            </p>
            <h1 className="m-0 font-mono text-6xl font-bold tracking-[0.2em] text-white text-shadow-neon">
              LA PLATEFORME
            </h1>
            <p className="m-0 font-mono text-base tracking-[0.35em] text-[rgba(0,180,255,0.55)] uppercase">
              Viewer Immersif · Marseille
            </p>
          </div>

          <Divider />

          {/* Description */}
          <p className="relative m-0 font-mono text-lg leading-relaxed tracking-wide text-[rgba(200,230,255,0.9)] text-center">
            Ce projet est un{' '}
            <span className="text-neon-blue font-bold">visualiseur interactif en 3D</span>{' '}
            du bâtiment <span className="text-white font-bold">La Plateforme</span>, situé
            aux Docks des Suds à Marseille. Explorez l'ensemble des espaces du Hangar et
            consultez les informations de chaque salle en temps réel.
          </p>

          <Divider />

          {/* Feature cards */}
          <div className="relative flex flex-col gap-4">
            {FEATURES.map(({ icon, title, desc }) => (
              <div
                key={title}
                className="flex items-start gap-5 bg-[rgba(0,30,70,0.45)] border border-[rgba(0,140,255,0.18)] rounded-lg px-6 py-4"
              >
                <span className="text-neon-blue text-3xl mt-1 shrink-0">{icon}</span>
                <div>
                  <p className="m-0 font-mono text-xl font-bold tracking-wide text-white mb-1">
                    {title}
                  </p>
                  <p className="m-0 font-mono text-base leading-relaxed text-[rgba(180,220,255,0.75)]">
                    {desc}
                  </p>
                </div>
              </div>
            ))}
          </div>

          <Divider />

          {/* CTA */}
          <div className="relative flex flex-col items-center gap-4">
            <button
              onClick={() => setOpen(false)}
              className="px-16 py-4 bg-transparent border border-[rgba(0,180,255,0.6)] rounded-sm font-mono text-xl tracking-[0.35em] text-neon-blue cursor-pointer shadow-neon-btn hover:bg-[rgba(0,120,255,0.12)] hover:border-[rgba(0,220,255,0.9)] hover:text-white hover:shadow-neon-btn-hover active:scale-95 transition-all duration-200"
            >
              EXPLORER
            </button>
            <p className="m-0 font-mono text-sm tracking-[0.2em] text-white/40">
              Cliquez en dehors pour fermer
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
