import { Unity } from 'react-unity-webgl';
import ControlsModal from './ControlsModal';
import WelcomeModal from './WelcomeModal';
import StatusLegend from './StatusLegend';

export default function UnityPlayer({ unityProvider }) {
  return (
    <div style={{ position: 'relative', width: '100%', height: '100vh' }}>
      <Unity unityProvider={unityProvider} style={{ width: '100%', height: '100vh' }} />
      <WelcomeModal />
      <ControlsModal />
      <StatusLegend />
    </div>
  );
}
