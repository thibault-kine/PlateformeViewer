import { Canvas, useFrame } from '@react-three/fiber';
import { useGLTF, Environment } from '@react-three/drei';
import { useRef, useEffect, useCallback, Component } from 'react';
import * as THREE from 'three';

// ── Logo mesh ──────────────────────────────────────────────────────────────
function Logo({ mousePos }) {
  const { scene } = useGLTF('/LaPlateformeLogo.glb');
  const groupRef = useRef();
  const autoRotation = useRef(0);

  useEffect(() => {
    scene.traverse((child) => {
      if (child.isMesh) {
        child.material = new THREE.MeshStandardMaterial({
          color: new THREE.Color('#00d4ff'),
          emissive: new THREE.Color('#0066ff'),
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
    autoRotation.current += delta * 0.4;

    const targetY = mousePos.current.x !== 0 || mousePos.current.y !== 0
      ? mousePos.current.x * Math.PI * 0.5
      : autoRotation.current;
    const targetX = -mousePos.current.y * Math.PI * 0.18;

    groupRef.current.rotation.y += (targetY - groupRef.current.rotation.y) * 0.04;
    groupRef.current.rotation.x += (targetX - groupRef.current.rotation.x) * 0.04;
  });

  return (
    <group ref={groupRef}>
      <group rotation={[Math.PI / 2, 0, 0]} scale={3}>
        <primitive object={scene} />
      </group>
    </group>
  );
}

// ── Error boundary for the 3D canvas ──────────────────────────────────────
class CanvasBoundary extends Component {
  state = { failed: false };
  static getDerivedStateFromError() { return { failed: true }; }
  render() {
    if (this.state.failed) {
      return (
        <div className="w-full h-full flex items-center justify-center">
          <span className="font-mono text-[rgba(0,180,255,0.4)] text-sm tracking-widest">[ 3D N/A ]</span>
        </div>
      );
    }
    return this.props.children;
  }
}

// ── Landing page ───────────────────────────────────────────────────────────
export default function LandingPage({ loadingProgression, isLoaded, onEnter }) {
  const mousePos = useRef({ x: 0, y: 0 });

  const handleMouseMove = useCallback((e) => {
    mousePos.current = {
      x: (e.clientX / window.innerWidth)  * 2 - 1,
      y: (e.clientY / window.innerHeight) * 2 - 1,
    };
    clearTimeout(handleMouseMove._t);
    handleMouseMove._t = setTimeout(() => {
      mousePos.current = { x: 0, y: 0 };
    }, 3000);
  }, []);

  const pct = Math.round(loadingProgression * 100);

  return (
    <div
      className="fixed inset-0 bg-navy-950 flex flex-col items-center justify-center overflow-hidden px-6"
      onMouseMove={handleMouseMove}
    >
      {/* Animated grid */}
      <div className="absolute inset-0 bg-grid-neon animate-grid-drift" />

      {/* Radial glow blob */}
      <div className="absolute left-1/2 top-[30%] w-[600px] h-[600px] rounded-full bg-radial-glow pointer-events-none animate-glow-pulse" />

      {/* Three.js canvas */}
      <div className="relative z-10 w-full max-w-[420px] h-[min(380px,45vw)] cursor-grab drop-shadow-[0_0_30px_rgba(0,160,255,0.5)]">
        <CanvasBoundary>
          <Canvas camera={{ position: [0, 0, 4], fov: 45 }} gl={{ antialias: true, alpha: true }}>
            <ambientLight intensity={0.3} color="#0033aa" />
            <pointLight position={[3, 3, 3]}   intensity={8} color="#00aaff" />
            <pointLight position={[-3, -2, 2]} intensity={5} color="#ffffff" />
            <pointLight position={[0, -3, 1]}  intensity={3} color="#0055ff" />
            <Logo mousePos={mousePos} />
            <Environment preset="night" />
          </Canvas>
        </CanvasBoundary>
      </div>

      {/* Bottom section */}
      <div className="relative z-10 flex flex-col items-center gap-3 -mt-2 w-full max-w-[560px]">

        {/* Title */}
        <h1 className="m-0 font-mono text-[clamp(1.6rem,8vw,3.8rem)] font-bold tracking-[0.35em] text-white text-shadow-neon text-center">
          LA PLATEFORME
        </h1>
        <p className="m-0 font-mono text-[clamp(0.6rem,3vw,1rem)] tracking-[0.3em] text-[rgba(0,180,255,0.6)] uppercase text-center">
          Viewer Immersif · Marseille
        </p>

        {/* Loading bar */}
        <div className="w-full flex flex-col gap-1.5 mt-2">
          <div className="relative w-full h-1 bg-[rgba(0,80,180,0.25)] rounded-sm overflow-visible shadow-[0_0_0_1px_rgba(0,120,255,0.15),inset_0_0_6px_rgba(0,40,120,0.4)]">
            {/* Fill */}
            <div
              className="h-full rounded-sm bg-bar-fill shadow-neon-bar transition-[width] duration-300"
              style={{ width: `${pct}%` }}
            />
            {/* Tip glow */}
            <div
              className="absolute top-1/2 -translate-x-1/2 -translate-y-1/2 w-3 h-3 rounded-full bg-white shadow-neon-bar-tip transition-[left] duration-300"
              style={{ left: `${pct}%` }}
            />
          </div>

          <div className="flex justify-between items-center font-mono text-[0.65rem] tracking-[0.2em]">
            <span className={`text-[rgba(0,180,255,0.7)] ${!isLoaded ? 'animate-blink' : ''}`}>
              {isLoaded ? 'PRÊT' : 'CHARGEMENT...'}
            </span>
            <span className="text-[rgba(255,255,255,0.5)]">{pct}%</span>
          </div>
        </div>

        {/* Enter button */}
        {isLoaded && (
          <button
            onClick={onEnter}
            className="mt-4 px-12 py-2.5 bg-transparent border border-[rgba(0,180,255,0.6)] rounded-sm font-mono text-[0.8rem] tracking-[0.35em] text-neon-blue cursor-pointer shadow-neon-btn hover:bg-[rgba(0,120,255,0.12)] hover:border-[rgba(0,220,255,0.9)] hover:text-white hover:shadow-neon-btn-hover active:scale-95 transition-all duration-200 animate-enter-appear"
          >
            ENTRER
          </button>
        )}
      </div>
    </div>
  );
}
