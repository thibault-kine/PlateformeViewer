import { Unity } from 'react-unity-webgl';

export default function UnityPlayer({ unityProvider }) {
  return (
    <div style={{ position: 'relative', width: '100%', height: '100vh' }}>
      <Unity unityProvider={unityProvider} style={{ width: '100%', height: '100vh' }} />

      <div
        style={{
          position: 'absolute', top: 10, left: 10,
          background: 'rgba(0, 20, 50, 0.85)',
          padding: '12px 16px',
          border: '1px solid rgba(0, 180, 255, 0.4)',
          borderRadius: '8px',
          boxShadow: '0 0 20px rgba(0, 140, 255, 0.2)',
          zIndex: 10,
          color: '#8de3ff',
          fontSize: '13px',
          fontFamily: 'monospace',
          lineHeight: '1.7',
        }}
      >
        <p style={{ margin: 0 }}>
          Avancer: Z<br />
          Reculer: S<br />
          Aller à gauche: Q<br />
          Aller à droite: D<br />
          Monter: A<br />
          Descendre: E<br />
          Caméra: Bouton droit souris
        </p>
      </div>
    </div>
  );
}
