import { useState, useEffect } from 'react';
import { Unity } from 'react-unity-webgl';
import ControlsModal from './ControlsModal';
import WelcomeModal from './WelcomeModal';
import StatusLegend from './StatusLegend';
import SettingsPanel from './SettingsPanel';

export default function UnityPlayer({ unityProvider, sendMessage, isLoaded, requestFullscreen }) {
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [showEscHint, setShowEscHint] = useState(false);

  useEffect(() => {
    const handleFsChange = () => {
      const entered = !!document.fullscreenElement;
      setIsFullscreen(entered);
      if (entered) {
        setShowEscHint(true);
        const timer = setTimeout(() => setShowEscHint(false), 3000);
        return () => clearTimeout(timer);
      }
    };
    document.addEventListener('fullscreenchange', handleFsChange);
    return () => document.removeEventListener('fullscreenchange', handleFsChange);
  }, []);

  const toggleFullscreen = () => {
    if (isFullscreen) {
      document.exitFullscreen();
    } else {
      requestFullscreen(true);
    }
  };

  return (
    <div style={{ position: 'relative', width: '100%', height: '100vh' }}>
      <Unity unityProvider={unityProvider} style={{ width: '100%', height: '100vh' }} />
      <WelcomeModal />
      <StatusLegend />
      <SettingsPanel sendMessage={sendMessage} isLoaded={isLoaded} requestFullscreen={requestFullscreen} />
      <ControlsModal />

      {/* Standalone fullscreen button — top-right */}
      <button
        onClick={toggleFullscreen}
        title={isFullscreen ? 'Quitter le plein écran' : 'Plein écran'}
        className="fixed top-7 touch:top-4 left-7 touch:left-4 z-[200] w-14 h-14 touch:w-10 touch:h-10 rounded-full bg-[rgba(0,20,50,0.85)] border border-[rgba(0,180,255,0.5)] text-neon-blue font-mono text-xl touch:text-sm shadow-neon-fab flex items-center justify-center cursor-pointer transition-all duration-200 hover:bg-[rgba(0,40,90,0.95)] hover:border-neon-blue hover:shadow-neon-fab-hover hover:text-white"
      >
        {isFullscreen ? '⊡' : '⛶'}
      </button>

      {/* ESC hint — appears when entering fullscreen, auto-hides after 3 s */}
      {showEscHint && (
        <div className="fixed bottom-10 left-1/2 -translate-x-1/2 z-[300] px-5 py-2.5 bg-[rgba(3,14,36,0.88)] border border-[rgba(0,180,255,0.35)] rounded-sm font-mono text-xs tracking-[0.2em] text-[rgba(180,220,255,0.85)] shadow-neon-modal animate-enter-appear pointer-events-none">
          Appuyez sur <span className="text-neon-blue font-bold">ESC</span> pour quitter le plein écran
        </div>
      )}
    </div>
  );
}
