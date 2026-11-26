import { GRADE_LOGO } from '../lib/pixelArt'

// Logotipo original: um "R" em pixels sobre um bloco limão.
function Logo({ tamanho = 36 }) {
  return (
    <svg
      viewBox="0 0 36 36"
      width={tamanho}
      height={tamanho}
      aria-hidden="true"
      focusable="false"
    >
      <rect width="36" height="36" rx="9" fill="#C8F31D" />

      <g
        transform="translate(8 4)"
        fill="#0A0D0A"
        shapeRendering="crispEdges"
      >
        {GRADE_LOGO.flatMap((linha, y) =>
          [...linha].map((simbolo, x) =>
            simbolo === 'X' ? (
              <rect
                key={`${x}-${y}`}
                x={x * 4}
                y={y * 4}
                width="4"
                height="4"
              />
            ) : null
          )
        )}
      </g>
    </svg>
  )
}

export default Logo
