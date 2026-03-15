import { useState } from 'react';

const MOVEMENT = [
  { key: 'Z', label: 'Avancer' },
  { key: 'Q', label: 'Gauche' },
  { key: 'S', label: 'Reculer' },
  { key: 'D', label: 'Droite' },
];

const ALTITUDE = [
  { key: 'A', label: 'Monter' },
  { key: 'E', label: 'Descendre' },
];

function Key({ label, hint }) {
  return (
    <div className="flex flex-col items-center gap-1.5">
      <kbd className="inline-flex items-center justify-center w-14 h-14 bg-[rgba(0,25,65,0.9)] border border-[rgba(0,180,255,0.45)] border-b-[3px] border-b-[rgba(0,180,255,0.65)] rounded-md font-mono text-xl font-bold text-white shadow-neon-key select-none">
        {label}
      </kbd>
      <span className="font-mono text-sm tracking-wide text-[rgba(180,220,255,0.85)] whitespace-nowrap">
        {hint}
      </span>
    </div>
  );
}

function SectionLabel({ children }) {
  return (
    <p className="m-0 mb-3 font-mono text-xs tracking-[0.35em] text-[rgba(0,180,255,0.55)] uppercase">
      {children}
    </p>
  );
}

function Divider() {
  return (
    <div className="h-px bg-gradient-to-r from-transparent via-[rgba(0,160,255,0.35)] to-transparent my-4" />
  );
}

export default function ControlsModal() {
  const [open, setOpen] = useState(true);

  return (
    <>
      {/* Floating help button */}
      <button
        onClick={() => setOpen(true)}
        title="Contrôles"
        className="fixed bottom-7 right-7 z-[200] w-12 h-12 rounded-full bg-[rgba(0,20,50,0.85)] border border-[rgba(0,180,255,0.5)] text-neon-blue font-mono text-xl font-bold shadow-neon-fab flex items-center justify-center cursor-pointer transition-all duration-200 hover:bg-[rgba(0,40,90,0.95)] hover:border-neon-blue hover:shadow-neon-fab-hover hover:text-white"
      >
        ?
      </button>

      {/* Overlay */}
      {open && (
        <div
          className="fixed inset-0 z-[300] bg-[rgba(2,11,26,0.65)] backdrop-blur-md flex items-center justify-center animate-overlay-in"
          onClick={() => setOpen(false)}
        >
          {/* Modal */}
          <div
            className="relative w-[520px] bg-[rgba(3,14,36,0.97)] border border-[rgba(0,160,255,0.3)] rounded-lg px-10 py-8 overflow-hidden shadow-neon-modal animate-modal-in"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Grid background */}
            <div className="absolute inset-0 bg-grid-modal pointer-events-none" />

            {/* Header */}
            <div className="relative flex items-center justify-between mb-5">
              <span className="font-mono text-lg font-bold tracking-[0.4em] text-white text-shadow-neon-sm">
                CONTRÔLES
              </span>
              <button
                onClick={() => setOpen(false)}
                className="bg-transparent border-none text-[rgba(0,180,255,0.45)] text-lg cursor-pointer px-1 transition-colors duration-150 hover:text-white font-mono"
              >
                ✕
              </button>
            </div>

            <Divider />

            {/* Déplacement */}
            <SectionLabel>Déplacement</SectionLabel>
            <div className="relative flex flex-col items-center gap-2 mb-1">
              {/* Top row: Z */}
              <div className="flex justify-center">
                <Key label="Z" hint="Avancer" />
              </div>
              {/* Bottom row: Q S D */}
              <div className="flex gap-2">
                <Key label="Q" hint="Gauche" />
                <Key label="S" hint="Reculer" />
                <Key label="D" hint="Droite" />
              </div>
            </div>

            <Divider />

            {/* Altitude */}
            <SectionLabel>Altitude</SectionLabel>
            <div className="relative flex justify-center gap-6">
              {ALTITUDE.map(({ key, label }) => <Key key={key} label={key} hint={label} />)}
            </div>

            <Divider />

            {/* Caméra */}
            <SectionLabel>Caméra</SectionLabel>
            <div className="relative flex items-center justify-center gap-6">
              {/* Mouse SVG */}
              <div className="flex flex-col items-center gap-2">
                <svg className="w-9 h-14 drop-shadow-[0_0_6px_rgba(0,180,255,0.5)]" viewBox="0 0 40 60" fill="none">
                  <rect x="1" y="1" width="38" height="58" rx="19" stroke="rgba(0,180,255,0.5)" strokeWidth="2"/>
                  <line x1="20" y1="1" x2="20" y2="28" stroke="rgba(0,180,255,0.5)" strokeWidth="2"/>
                  <path d="M20 2 Q38 2 38 20 L38 28 L20 28 Z" fill="rgba(0,180,255,0.2)"/>
                  <circle cx="29" cy="16" r="3" fill="#00cfff" opacity="0.8"/>
                </svg>
                <span className="font-mono text-sm tracking-wide text-[rgba(180,220,255,0.85)]">Clic Droit</span>
              </div>
              <span className="font-mono text-base tracking-wide text-white">
                Orienter la caméra
              </span>
            </div>

            <Divider />

            <p className="relative m-0 text-center font-mono text-xs tracking-[0.2em] text-white/60">
              Cliquez en dehors pour fermer
            </p>
          </div>
        </div>
      )}
    </>
  );
}
