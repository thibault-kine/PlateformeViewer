import { useState, useEffect, useRef } from 'react';

const DEFAULTS = {
  mouseSensitivity: 1.5,
  cameraSpeed:      10,
  scrollSpeed:      20,
  ambientIntensity: 0.25,
  statusVisible:    true,
};

// ── Sub-components ────────────────────────────────────────────────────────────

function SectionLabel({ children }) {
  return (
    <p className="m-0 mb-3 font-mono text-xs tracking-[0.35em] text-[rgba(0,180,255,0.6)] uppercase">
      {children}
    </p>
  );
}

function Divider() {
  return (
    <div className="h-px bg-gradient-to-r from-transparent via-[rgba(0,160,255,0.3)] to-transparent my-5" />
  );
}

function SliderRow({ label, value, min, max, step, onChange }) {
  const pct = ((value - min) / (max - min)) * 100;
  return (
    <div className="flex flex-col gap-1.5">
      <div className="flex justify-between items-baseline">
        <span className="font-mono text-sm text-[rgba(180,220,255,0.85)] tracking-wide">{label}</span>
        <span className="font-mono text-xs text-neon-blue tabular-nums">{value}</span>
      </div>
      <input
        type="range"
        min={min} max={max} step={step}
        value={value}
        onChange={onChange}
        className="w-full h-1 rounded-full appearance-none cursor-pointer"
        style={{
          background: `linear-gradient(to right, #00cfff ${pct}%, rgba(0,60,120,0.5) ${pct}%)`,
        }}
      />
    </div>
  );
}

function Toggle({ value, onChange }) {
  return (
    <button
      onClick={onChange}
      className={`relative w-12 h-6 rounded-full transition-colors duration-200 focus:outline-none ${
        value
          ? 'bg-[rgba(0,180,255,0.35)] shadow-[0_0_10px_rgba(0,180,255,0.5)]'
          : 'bg-[rgba(0,40,80,0.7)]'
      }`}
    >
      <span
        className={`absolute top-1 left-0 w-4 h-4 rounded-full bg-white shadow transition-transform duration-200 ${
          value ? 'translate-x-6' : 'translate-x-1'
        }`}
      />
    </button>
  );
}

function ActionBtn({ onClick, children }) {
  return (
    <button
      onClick={onClick}
      className="flex-1 py-2.5 bg-transparent border border-[rgba(0,180,255,0.4)] rounded-sm font-mono text-xs tracking-[0.25em] text-neon-blue cursor-pointer transition-all duration-200 hover:bg-[rgba(0,120,255,0.12)] hover:border-[rgba(0,220,255,0.8)] hover:text-white shadow-neon-btn"
    >
      {children}
    </button>
  );
}

// ── Main component ─────────────────────────────────────────────────────────────

export default function SettingsPanel({ sendMessage, isLoaded }) {
  const [open, setOpen]         = useState(false);
  const [settings, setSettings] = useState({ ...DEFAULTS });
  const drawerRef               = useRef(null);

  useEffect(() => {
    if (!open) return;
    const handleClick = (e) => {
      if (drawerRef.current && !drawerRef.current.contains(e.target)) {
        setOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, [open]);

  // Single guarded send helper
  const send = (method, value) => {
    if (isLoaded) sendMessage('UnityBridge', method, String(value));
  };

  const handleSlider = (key, method) => (e) => {
    const val = parseFloat(e.target.value);
    setSettings(prev => ({ ...prev, [key]: val }));
    send(method, val);
  };

  const handleToggle = (key, method) => () => {
    const next = !settings[key];
    setSettings(prev => ({ ...prev, [key]: next }));
    send(method, next);
  };

  const handleReset = () => {
    setSettings({ ...DEFAULTS });
    send('SetMouseSensitivity', DEFAULTS.mouseSensitivity);
    send('SetCameraSpeed',      DEFAULTS.cameraSpeed);
    send('SetScrollSpeed',      DEFAULTS.scrollSpeed);
    send('SetAmbientIntensity', DEFAULTS.ambientIntensity);
    send('SetStatusVisible',    DEFAULTS.statusVisible);
  };

  return (
    <>
      {/* Gear button — sits above the ? button */}
      <button
        onClick={() => setOpen(true)}
        title="Paramètres"
        className="fixed bottom-[5.5rem] right-7 z-[200] w-14 h-14 rounded-full bg-[rgba(0,20,50,0.85)] border border-[rgba(0,180,255,0.5)] text-neon-blue font-mono text-xl shadow-neon-fab flex items-center justify-center cursor-pointer transition-all duration-200 hover:bg-[rgba(0,40,90,0.95)] hover:border-neon-blue hover:shadow-neon-fab-hover hover:text-white"
      >
        ⚙
      </button>

      {/* Drawer */}
      <div
        ref={drawerRef}
        className={`fixed top-0 right-0 h-full z-[250] w-80 transition-transform duration-300 ease-in-out ${
          open ? 'translate-x-0' : 'translate-x-full'
        }`}
      >
        {/* Card */}
        <div className="relative h-full bg-[rgba(3,14,36,0.97)] border-l border-[rgba(0,160,255,0.3)] overflow-hidden shadow-neon-modal flex flex-col">

          {/* Inner grid */}
          <div className="absolute inset-0 bg-grid-modal pointer-events-none" />

          {/* Left accent line */}
          <div className="absolute top-0 left-0 bottom-0 w-[2px] bg-gradient-to-b from-transparent via-neon-blue to-transparent" />

          {/* Scrollable content */}
          <div className={`relative flex flex-col flex-1 overflow-y-auto px-7 py-6 ${!isLoaded ? 'pointer-events-none' : ''}`}>

            {/* Header */}
            <div className="flex items-center justify-between mb-5">
              <span className="font-mono text-base font-bold tracking-[0.35em] text-white text-shadow-neon-sm">
                PARAMÈTRES
              </span>
              <button
                onClick={() => setOpen(false)}
                className="pointer-events-auto bg-transparent border-none text-[rgba(0,180,255,0.45)] text-lg cursor-pointer transition-colors hover:text-white font-mono"
              >
                ✕
              </button>
            </div>

            {/* Not loaded warning */}
            {!isLoaded && (
              <p className="font-mono text-xs tracking-wide text-[rgba(243,156,18,0.8)] mb-4 text-center animate-blink pointer-events-none">
                En attente du chargement Unity…
              </p>
            )}

            <div className={!isLoaded ? 'opacity-40' : ''}>

              {/* ── VISUEL ── */}
              <SectionLabel>Visuel</SectionLabel>
              <div className="flex items-center justify-between">
                <span className="font-mono text-sm text-[rgba(180,220,255,0.85)] tracking-wide">
                  Couleurs de statut
                </span>
                <Toggle
                  value={settings.statusVisible}
                  onChange={handleToggle('statusVisible', 'SetStatusVisible')}
                />
              </div>

              <Divider />

              {/* ── CAMÉRA ── */}
              <SectionLabel>Caméra</SectionLabel>
              <div className="flex flex-col gap-4">
                <SliderRow
                  label="Sensibilité souris"
                  value={settings.mouseSensitivity}
                  min={0.1} max={5} step={0.1}
                  onChange={handleSlider('mouseSensitivity', 'SetMouseSensitivity')}
                />
                <SliderRow
                  label="Vitesse déplacement"
                  value={settings.cameraSpeed}
                  min={1} max={30} step={0.5}
                  onChange={handleSlider('cameraSpeed', 'SetCameraSpeed')}
                />
                <SliderRow
                  label="Vitesse scroll"
                  value={settings.scrollSpeed}
                  min={1} max={60} step={1}
                  onChange={handleSlider('scrollSpeed', 'SetScrollSpeed')}
                />
              </div>

              <Divider />

              {/* ── ÉCLAIRAGE ── */}
              <SectionLabel>Éclairage</SectionLabel>
              <SliderRow
                label="Intensité ambiante"
                value={settings.ambientIntensity}
                min={0} max={2} step={0.05}
                onChange={handleSlider('ambientIntensity', 'SetAmbientIntensity')}
              />

              <Divider />

              {/* ── APPLICATION ── */}
              <SectionLabel>Application</SectionLabel>
              <div className="flex gap-3">
                <ActionBtn onClick={handleReset}>
                  RÉINITIALISER
                </ActionBtn>
              </div>

            </div>
          </div>
        </div>
      </div>
    </>
  );
}
