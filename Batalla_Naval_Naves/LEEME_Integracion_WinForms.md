# Batalla Naval - pack de naves minimalistas

Cinco embarcaciones vistas desde arriba, con estilo plano rosa y lavanda para el tablero de 15×15 de WinForms.

## Archivos por nave
- `Nombre_H_128.png`: imagen horizontal transparente en alta resolución, de `128 × N` por `128` píxeles.
- `Nombre_V_128.png`: versión vertical ya girada 90°, de `128` por `128 × N`.
- `Nombre_H_36.png`: miniatura horizontal exacta para una celda de 36 px (`36 × N` por `36`).
- `Nombre_V_36.png`: miniatura vertical exacta para celdas de 36 px (`36` por `36 × N`).
- `Nombre_Lista.png`: miniatura para la lista de selección, lienzo transparente de 180×54 px, sin deformaciones.
- `Nombre.svg`: fuente vectorial editable de la versión horizontal.

**Nombres y longitudes:** Batalla_Naval = 5; Portaaviones = 4; Fragatas = 3; Buque = 2; Acorazados = 1.

## Integración recomendada en WinForms
- Usar `_H_128.png` o `_V_128.png` según orientación, dentro de una zona de dibujo de N×tamañoCelda por tamañoCelda (o viceversa). Mantener `AspectRatio`, NO estirar un horizontal a un cuadro vertical.
- Para cuadrículas de 36 px, `_H_36.png` / `_V_36.png` ya están dimensionados y se pueden pintar 1:1.
- Poner el punto de dibujo sobre la primera celda ocupada, alineándolo al origen de la cuadrícula; las imágenes incluyen aire mínimo por diseño, no un margen exterior grande.
- Usar GDI+ Graphics.DrawImage con rectángulo de destino de dimensiones exactas, o PictureBox con tamaño adecuado; evitar `StretchImage` en una celda individual.
- Dibujar el sprite como superposición sobre todas las casillas de la nave (no repetirlo por cada casilla).
- No cambiar la lógica de placement, rotación, hit-testing ni el modelo de las naves: solo su renderizado.

**Paleta:** casco rosa `#D66A9B`, detalles lavanda `#B29AD6`, contorno ciruela `#523B64`.
