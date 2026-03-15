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
    <div className="w-screen h-screen overflow-hidden bg-navy-950">
      {/* Unity always mounts so it starts loading immediately */}
      <div className={`w-full h-full transition-opacity duration-[800ms] ${visible ? 'opacity-0' : 'opacity-100'}`}>
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
