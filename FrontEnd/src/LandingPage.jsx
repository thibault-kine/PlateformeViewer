import { Canvas, useFrame } from '@react-three/fiber';
import { useGLTF, Environment } from '@react-three/drei';
import { useRef, useEffect, useCallback } from 'react';
import * as THREE from 'three';
import './LandingPage.css';

// ── Logo mesh: loads GLB and applies neon material ──────────────────────────
function Logo({ mousePos }) {
  const { scene } = useGLTF('/LaPlateformeLogo.glb');
  const groupRef = useRef();
  const autoRotation = useRef(0);

  useEffect(() => {
    scene.traverse((child) => {
      if (child.isMesh) {
        child.material = new THREE.MeshStandardMaterial({
          color: new THREE.Color('#00cfff'),
          emissive: new THREE.Color('#0055ff'),
          emissiveIntensity: 2.0,
          metalness: 0.9,
          roughness: 0.05,
        });
        child.castShadow = true;
      }
    });
  }, [scene]);

  useFrame((_, delta) => {
    if (!groupRef.current) return;

    // Continuous slow auto-rotation
    autoRotation.current += delta * 0.4;

    const targetY = mousePos.current.x !== 0 || mousePos.current.y !== 0
      ? mousePos.current.x * Math.PI * 0.5
      : autoRotation.current;

    const targetX = -mousePos.current.y * Math.PI * 0.18;

    groupRef.current.rotation.y += (targetY - groupRef.current.rotation.y) * 0.04;
    groupRef.current.rotation.x += (targetX - groupRef.current.rotation.x) * 0.04;
  });

  return (
    // Outer group handles mouse/auto rotation
    <group ref={groupRef}>
      {/* Inner group corrects Blender Z-up → Three.js Y-up axis */}
      <group rotation={[Math.PI / 2, 0, 0]} scale={3}>
        <primitive object={scene} />
      </group>
    </group>
  );
}

// ── Landing page component ────────────────────────────────────────────────────
export default function LandingPage({ loadingProgression, isLoaded, onEnter }) {
  const mousePos = useRef({ x: 0, y: 0 });
  const isInteracting = useRef(false);

  const handleMouseMove = useCallback((e) => {
    const x = (e.clientX / window.innerWidth) * 2 - 1;
    const y = (e.clientY / window.innerHeight) * 2 - 1;
    mousePos.current = { x, y };
    isInteracting.current = true;

    clearTimeout(handleMouseMove._timeout);
    handleMouseMove._timeout = setTimeout(() => {
      isInteracting.current = false;
      mousePos.current = { x: 0, y: 0 };
    }, 3000);
  }, []);

  const pct = Math.round(loadingProgression * 100);

  return (
    <div className="landing-root" onMouseMove={handleMouseMove}>

      {/* Animated grid background */}
      <div className="landing-grid" />

      {/* Radial glow behind logo */}
      <div className="landing-glow" />

      {/* Three.js canvas for GLB logo */}
      <div className="landing-canvas-wrap">
        <Canvas
          camera={{ position: [0, 0, 4], fov: 45 }}
          gl={{ antialias: true, alpha: true }}
        >
          <ambientLight intensity={0.3} color="#0033aa" />
          <pointLight position={[3, 3, 3]} intensity={8} color="#00aaff" />
          <pointLight position={[-3, -2, 2]} intensity={5} color="#ffffff" />
          <pointLight position={[0, -3, 1]} intensity={3} color="#0055ff" />
          <Logo mousePos={mousePos} />
          <Environment preset="night" />
        </Canvas>
      </div>

      {/* Bottom section: title + loading bar */}
      <div className="landing-bottom">
        <h1 className="landing-title">LA PLATEFORME</h1>
        <p className="landing-subtitle">VIEWER IMMERSIF · MARSEILLE</p>

        <div className="loading-section">
          <div className="loading-bar-track">
            <div
              className="loading-bar-fill"
              style={{ width: `${pct}%` }}
            />
            <div
              className="loading-bar-glow"
              style={{ left: `${pct}%` }}
            />
          </div>

          <div className="loading-labels">
            <span className="loading-status">
              {isLoaded ? 'PRÊT' : 'CHARGEMENT...'}
            </span>
            <span className="loading-pct">{pct}%</span>
          </div>
        </div>

        {isLoaded && (
          <button className="enter-btn" onClick={onEnter}>
            ENTRER
          </button>
        )}
      </div>
    </div>
  );
}
