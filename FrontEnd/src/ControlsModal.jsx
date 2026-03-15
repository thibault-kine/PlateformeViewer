import { useState } from 'react';

const ALTITUDE = [
  { key: 'A', label: 'Monter' },
  { key: 'E', label: 'Descendre' },
];

function Key({ label, hint }) {
  return (
    <div className="flex flex-col items-center gap-3">
      <kbd className="inline-flex items-center justify-center w-24 h-24 touch:w-14 touch:h-14 bg-[rgba(0,25,65,0.9)] border border-[rgba(0,180,255,0.45)] border-b-[4px] border-b-[rgba(0,180,255,0.65)] rounded-xl font-mono text-4xl touch:text-2xl font-bold text-white shadow-neon-key select-none">
        {label}
      </kbd>
      <span className="font-mono text-lg touch:text-sm tracking-wide text-[rgba(180,220,255,0.9)] whitespace-nowrap">
        {hint}
      </span>
    </div>
  );
}

function SectionLabel({ children }) {
  return (
    <p className="m-0 mb-4 font-mono text-base touch:text-sm tracking-[0.4em] text-[rgba(0,180,255,0.6)] uppercase">
      {children}
    </p>
  );
}

function Divider() {
  return (
    <div className="h-px bg-gradient-to-r from-transparent via-[rgba(0,160,255,0.35)] to-transparent my-6 touch:my-4" />
  );
}

export default function ControlsModal() {
  const [open, setOpen] = useState(false);

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        title="Contrôles"
        className="fixed top-[9.5rem] touch:top-[6.5rem] left-7 touch:left-4 z-[200] w-14 h-14 touch:w-10 touch:h-10 rounded-full bg-[rgba(0,20,50,0.85)] border border-[rgba(0,180,255,0.5)] text-neon-blue font-mono text-2xl touch:text-sm font-bold shadow-neon-fab flex items-center justify-center cursor-pointer transition-all duration-200 hover:bg-[rgba(0,40,90,0.95)] hover:border-neon-blue hover:shadow-neon-fab-hover hover:text-white"
      >
        ?
      </button>

      {open && (
        <div
          className="fixed inset-0 z-[300] animate-overlay-in"
          onClick={() => setOpen(false)}
        >
          <div className="absolute inset-0 bg-[rgba(2,11,26,0.65)]" />
          <div className="absolute inset-0 bg-grid-neon animate-grid-drift opacity-50 pointer-events-none" />

          {/* Scroll wrapper — child div for Firefox compat */}
          <div className="absolute inset-0 overflow-y-auto">
            <div className="relative flex items-center justify-center w-full min-h-full py-4">
              <div
                className="relative w-full max-w-[820px] mx-4 my-auto bg-[rgba(3,14,36,0.96)] border border-[rgba(0,160,255,0.3)] rounded-xl px-12 py-10 touch:px-5 touch:py-8 overflow-hidden shadow-neon-modal animate-modal-in"
                onClick={(e) => e.stopPropagation()}
              >
                <div className="absolute inset-0 bg-grid-modal pointer-events-none" />
                <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-neon-blue to-transparent" />

                {/* Header */}
                <div className="relative flex items-center justify-between mb-6">
                  <span className="font-mono text-3xl touch:text-xl font-bold tracking-[0.4em] text-white text-shadow-neon-sm">
                    CONTRÔLES
                  </span>
                  <button
                    onClick={() => setOpen(false)}
                    className="bg-transparent border-none text-[rgba(0,180,255,0.45)] text-2xl cursor-pointer px-2 transition-colors duration-150 hover:text-white font-mono"
                  >
                    ✕
                  </button>
                </div>

                <Divider />

                <SectionLabel>Déplacement</SectionLabel>
                <div className="relative flex flex-col items-center gap-4 mb-2">
                  <div className="flex justify-center">
                    <Key label="Z" hint="Avancer" />
                  </div>
                  <div className="flex gap-4 touch:gap-2">
                    <Key label="Q" hint="Gauche" />
                    <Key label="S" hint="Reculer" />
                    <Key label="D" hint="Droite" />
                  </div>
                </div>

                <Divider />

                <SectionLabel>Altitude</SectionLabel>
                <div className="relative flex justify-center gap-10 touch:gap-6">
                  {ALTITUDE.map(({ key, label }) => (
                    <Key key={key} label={key} hint={label} />
                  ))}
                </div>

                <Divider />

                <SectionLabel>Caméra</SectionLabel>
                <div className="relative flex items-center justify-center gap-8 touch:gap-5">
                  <div className="flex flex-col items-center gap-3">
                    <svg className="w-14 h-20 touch:w-10 touch:h-14 drop-shadow-[0_0_8px_rgba(0,180,255,0.6)]" viewBox="0 0 40 60" fill="none">
                      <rect x="1" y="1" width="38" height="58" rx="19" stroke="rgba(0,180,255,0.5)" strokeWidth="2"/>
                      <line x1="20" y1="1" x2="20" y2="28" stroke="rgba(0,180,255,0.5)" strokeWidth="2"/>
                      <path d="M20 2 Q38 2 38 20 L38 28 L20 28 Z" fill="rgba(0,180,255,0.2)"/>
                      <circle cx="29" cy="16" r="3" fill="#00cfff" opacity="0.8"/>
                    </svg>
                    <span className="font-mono text-lg touch:text-sm tracking-wide text-[rgba(180,220,255,0.9)]">
                      Clic Droit
                    </span>
                  </div>
                  <span className="font-mono text-2xl touch:text-base tracking-wide text-white">
                    Orienter la caméra
                  </span>
                </div>

                <Divider />

                <p className="relative m-0 text-center font-mono text-base touch:text-sm tracking-[0.2em] text-white/50">
                  Cliquez en dehors pour fermer
                </p>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
