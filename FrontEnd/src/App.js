import { useState } from 'react';
import { useUnityContext } from 'react-unity-webgl';
import LandingPage from './LandingPage';
import UnityPlayer from './UnityPlayer';

export default function App() {
  const [visible, setVisible] = useState(true);

  const { unityProvider, loadingProgression, isLoaded } = useUnityContext({
    loaderUrl: 'unity/Build/unity.loader.js',
    dataUrl: 'unity/Build/unity.data',
    frameworkUrl: 'unity/Build/unity.framework.js',
    codeUrl: 'unity/Build/unity.wasm',
  });

  const handleEnter = () => {
    setVisible(false);
  };

  return (
    <div style={{ width: '100vw', height: '100vh', overflow: 'hidden', background: '#020b1a' }}>
      {/* Unity always mounts so it starts loading immediately */}
      <div style={{ width: '100%', height: '100%', opacity: visible ? 0 : 1, transition: 'opacity 0.8s ease' }}>
        <UnityPlayer unityProvider={unityProvider} />
      </div>

      {/* Landing page overlays Unity while loading */}
      {visible && (
        <LandingPage
          loadingProgression={loadingProgression}
          isLoaded={isLoaded}
          onEnter={handleEnter}
        />
      )}
    </div>
  );
}
