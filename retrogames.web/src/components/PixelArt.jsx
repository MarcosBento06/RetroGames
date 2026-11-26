// Desenha uma grade de pixel art como SVG (decorativo, sem texto alternativo).
function PixelArt({
  grade,
  cor = 'currentColor',
  corDestaque = 'currentColor',
  className
}) {
  const largura = Math.max(...grade.map((linha) => linha.length))
  const altura = grade.length

  return (
    <svg
      className={className}
      viewBox={`0 0 ${largura} ${altura}`}
      shapeRendering="crispEdges"
      aria-hidden="true"
      focusable="false"
    >
      {grade.flatMap((linha, y) =>
        [...linha].map((simbolo, x) =>
          simbolo === '.' ? null : (
            <rect
              key={`${x}-${y}`}
              x={x}
              y={y}
              width="1"
              height="1"
              fill={simbolo === 'o' ? corDestaque : cor}
            />
          )
        )
      )}
    </svg>
  )
}

export default PixelArt
