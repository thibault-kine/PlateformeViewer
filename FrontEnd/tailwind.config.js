/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        mono: ['"Courier New"', 'Courier', 'monospace'],
      },
      colors: {
        navy: {
          950: '#020b1a',
          900: '#030d1f',
          800: '#031426',
        },
        neon: {
          blue:   '#00cfff',
          indigo: '#0055ff',
        },
      },
      keyframes: {
        'grid-drift': {
          '0%':   { backgroundPosition: '0 0' },
          '100%': { backgroundPosition: '60px 60px' },
        },
        'glow-pulse': {
          '0%, 100%': { opacity: '1',   transform: 'translate(-50%, -50%) scale(1)' },
          '50%':       { opacity: '0.7', transform: 'translate(-50%, -50%) scale(1.08)' },
        },
        blink: {
          '0%, 100%': { opacity: '1' },
          '50%':       { opacity: '0.3' },
        },
        'enter-appear': {
          from: { opacity: '0', transform: 'translateY(10px)' },
          to:   { opacity: '1', transform: 'translateY(0)' },
        },
        'overlay-in': {
          from: { opacity: '0' },
          to:   { opacity: '1' },
        },
        'modal-in': {
          from: { opacity: '0', transform: 'translateY(14px) scale(0.97)' },
          to:   { opacity: '1', transform: 'translateY(0) scale(1)' },
        },
      },
      animation: {
        'grid-drift':   'grid-drift 20s linear infinite',
        'glow-pulse':   'glow-pulse 4s ease-in-out infinite',
        'blink':        'blink 1.4s step-start infinite',
        'enter-appear': 'enter-appear 0.6s ease both',
        'overlay-in':   'overlay-in 0.2s ease both',
        'modal-in':     'modal-in 0.25s ease both',
      },
    },
  },
  plugins: [],
};
