---
name: Delicate Naval Fleet
colors:
  surface: '#fff7fe'
  surface-dim: '#e8d2f9'
  surface-bright: '#fff7fe'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#fbf0ff'
  surface-container: '#f7e9ff'
  surface-container-high: '#f3e2ff'
  surface-container-highest: '#f0dbff'
  on-surface: '#231532'
  on-surface-variant: '#544248'
  inverse-surface: '#392a48'
  inverse-on-surface: '#f9ecff'
  outline: '#867278'
  outline-variant: '#d9c0c8'
  surface-tint: '#9c3b6a'
  primary: '#993968'
  on-primary: '#ffffff'
  primary-container: '#b75181'
  on-primary-container: '#fffbff'
  inverse-primary: '#ffb0cf'
  secondary: '#69548b'
  on-secondary: '#ffffff'
  secondary-container: '#dac1ff'
  on-secondary-container: '#614c82'
  tertiary: '#366172'
  on-tertiary: '#ffffff'
  tertiary-container: '#507a8c'
  on-tertiary-container: '#fafdff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffd9e5'
  primary-fixed-dim: '#ffb0cf'
  on-primary-fixed: '#3d0023'
  on-primary-fixed-variant: '#7e2352'
  secondary-fixed: '#ecdcff'
  secondary-fixed-dim: '#d5bbfa'
  on-secondary-fixed: '#240f43'
  on-secondary-fixed-variant: '#513d71'
  tertiary-fixed: '#bde9fd'
  tertiary-fixed-dim: '#a1cde1'
  on-tertiary-fixed: '#001f29'
  on-tertiary-fixed-variant: '#1f4c5c'
  background: '#fff7fe'
  on-background: '#231532'
  surface-variant: '#f0dbff'
typography:
  display:
    fontFamily: Segoe UI
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
  headline-lg:
    fontFamily: Segoe UI
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  headline-md:
    fontFamily: Segoe UI
    fontSize: 18px
    fontWeight: '600'
    lineHeight: 24px
  headline-sm:
    fontFamily: Segoe UI
    fontSize: 15px
    fontWeight: '600'
    lineHeight: 20px
  body-lg:
    fontFamily: Segoe UI
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  body-md:
    fontFamily: Segoe UI
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 16px
  label-lg:
    fontFamily: Segoe UI
    fontSize: 13px
    fontWeight: '600'
    lineHeight: 18px
  label-md:
    fontFamily: Segoe UI
    fontSize: 11px
    fontWeight: '600'
    lineHeight: 14px
  label-grid:
    fontFamily: Segoe UI
    fontSize: 12px
    fontWeight: '700'
    lineHeight: 14px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  gutter: 1rem
  margin: 1.5rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 0.75rem
  space-lg: 1rem
  space-xl: 1.5rem
---

## Brand & Style

This design system defines a desktop video game experience developed for Windows Forms in C# at a target canvas resolution of 1366×768. The brand personality blends tactical naval strategy with a gentle, chic, and "girlie modern" visual tone. It avoids aggressive military tropes and juvenile cartoon clutter in favor of clean pastel palettes, balanced typographic hierarchies, and soft flat tactile surfaces.

The UI aesthetic is minimalist modern desktop software:
- Crisp 2D vector motifs with elegant flat shapes.
- Purposeful window frame chrome reflecting native desktop controls (minimize, maximize, close) integrated into an understated lavender-tinted custom title bar.
- Generous breathing space with soft container panels that structure game stats, player fleets, and coordinate grids without visual noise.

## Colors

The palette balances soft blush tones with deep plum neutrals to ensure accessible contrast and a delicate aesthetic.

### Functional Palette
- **Primary (`#D66A9B`)**: Primary buttons, call-to-actions, game banners, and critical interactive focus states.
- **Secondary / Accent (`#B29AD6`)**: Window title bars, active highlights, secondary action links, and hover fills.
- **Canvas / Window Background (`#FFF4F8`)**: Global backdrop across forms and base container surfaces.
- **Text Main / Ink (`#3E2F4D`)**: Deep plum tone used for primary typography, icons, borders, and sharp graphic elements.
- **Surface Card / Input (`#FFFFFF`)**: Pure white surfaces for inputs, card centers, and elevated stat panels.

### Naval Board States (Grid Tokens)
- **Cell Empty (`#EDE7F6`)**: Unexplored waters without action.
- **Cell Ship (`#C5B0E3`)**: Placed friendly vessel cell (player board only).
- **Cell Water Miss (`#B7E3F7`)**: Shot fired resulting in a splash/water miss.
- **Cell Hit (`#F8A5B8`)**: Vessel coordinate damaged.
- **Cell Sunk (`#C94F76`)**: Sunk vessel coordinate (completed ship destruction).

## Typography

The design system relies on the native Windows standard `Segoe UI` across all UI roles to ensure crisp GDI+/DirectWrite rendering, zero external runtime asset dependencies, and high legibility at 100% and 125% DPI scales.

- **Display & Headline**: Bold and semibold weights for game titles, modal headers, and turn announcements.
- **Body**: Regular weight, optimized for system messages, combat logs, and helper descriptions.
- **Label Grid**: Monospaced-like visual balance using Semibold and Bold for column coordinates (`A–J`) and row headers (`1–10`).

## Layout & Spacing

The layout is optimized for desktop game play anchored to 1366×768 with letterboxing/scaling capability up to 1920×1080:

- **Window Canvas**: Fixed base aspect ratio within a structured 1366×768 window container.
- **Title Bar**: 36px fixed height header containing game icon, title string, and window controls (`Min`, `Max`, `Close`).
- **Game Screen Structure**:
  - Dual 10×10 grid layout: Left panel (Player fleet), Right panel (Enemy radar grid).
  - Central control column (180px–220px) housing game timer, turn state, score, and action triggers.
  - Coordinate cells are fixed at 36×36px per square with a 4px gap (`space-xs`) between cells.
- **Padding & Alignment**: Panels and modal dialogs maintain `space-lg` (16px) internal padding with `margin` (24px) outer boundaries from the main window frame.

## Elevation & Depth

To remain compatible with Windows Forms GDI+ performance and maintain a modern flat-pastel aesthetic, elevation is achieved through **tonal layering and low-contrast outlines** rather than heavy blurred drop shadows:

- **Canvas Base**: Surface color `#FFF4F8`.
- **Containers & Cards**: Clean solid `#FFFFFF` surfaces bounded by a 1px solid stroke in `#B29AD6` at 30% opacity or `#E8DDF2`.
- **Interactive Tiles (Cells)**: Solid color fills with a 1px crisp border in `#FFFFFF` to visually separate cells without heavy drop shadows.
- **Modal Popups & Dialogs**: Raised using a 2px outer border `#B29AD6` and an underlying overlay fill `#3E2F4D` at 25% opacity.

## Shapes

The shape system utilizes soft, rounded geometry (`roundedness: 2`):

- **Grid Cells**: 6px corner radius for individual coordinate tiles, balancing grid structure with soft feminine touches.
- **Text Inputs & Standard Buttons**: 8px corner radius (`rounded-md`), presenting a welcoming pill-like feel without circular distortion.
- **Panels & Dialog Cards**: 12px to 16px corner radius (`rounded-lg`) for major board enclosures and modal overlays.
- **Window Controls**: Rectangular touchpoints with subtle 4px internal rounded hover backgrounds in the title bar.

## Components

### Buttons
- **Primary Button**: Background `#D66A9B`, text `#FFFFFF`, 8px radius. Hover state transitions to `#C94F76`. Pressed state shifts 1px down.
- **Secondary Button**: Background `#FFFFFF`, border 1.5px `#B29AD6`, text `#3E2F4D`. Hover fills with `#EDE7F6`.
- **Text / Link Action**: No border, `#B29AD6` text, underline on hover.

### Text Inputs (WinForms Custom TextBox)
- Background `#FFFFFF`, 1.5px border `#B29AD6`, text color `#3E2F4D`.
- Height: 38px with vertical padding 8px, horizontal padding 12px.
- Left-aligned icon slot for authentication screens (e.g., user icon, lock icon).

### Naval Grid Cells (`DataGridView` / Custom Panel Tiles)
- Size: 36×36px, corner radius 6px.
- States:
  - **Empty**: Fill `#EDE7F6`, hover color `#DDD5EB`.
  - **Ship**: Fill `#C5B0E3` with crisp centered ship glyph `#3E2F4D`.
  - **Water Miss**: Fill `#B7E3F7` with subtle wave icon in `#FFFFFF` or `#79B4D9`.
  - **Hit**: Fill `#F8A5B8` with an `✕` mark in `#FFFFFF`.
  - **Sunk**: Fill `#C94F76` with a solid `✕` in `#FFFFFF`.

### Window Control Box
- Min/Max buttons: Flat icon strokes `#3E2F4D` over `#B29AD6` (light tint hover).
- Close button: Standard close glyph, hovering turns to `#C94F76` with white glyph.

### Cards & Stats Badges
- Background `#FFFFFF` with `#EADBF3` soft border.
- Fleet health indicators styled as soft rectangular pill badges filled with state-specific ship colors.