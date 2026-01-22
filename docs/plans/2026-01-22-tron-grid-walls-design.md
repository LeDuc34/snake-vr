# Tron Grid Walls Design

## Overview

Add retro/arcade style textures to the interior walls of the play area cube, featuring cyan neon grid lines on a black background, inspired by Tron aesthetics.

## Visual Specification

- **Style**: Retro/Arcade with neon grids
- **Grid Color**: Cyan/Turquoise (#00FFFF)
- **Background Color**: Deep black (#000000)
- **Grid Pattern**: Simple horizontal and vertical lines
- **Glow Effect**: Subtle emission with URP bloom
- **Wall Differentiation**: All 6 faces identical (uniform cube)

## Technical Approach

Procedural shader using Shader Graph (not texture-based) for:
- Infinite resolution at any distance
- Runtime-adjustable parameters
- No texture file dependencies

## File Structure

```
Assets/SnakeVR/
├── Shaders/
│   └── TronGrid.shadergraph
├── Materials/
│   └── TronGridWall.mat
```

## Shader Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Grid Color | Color | #00FFFF | Color of grid lines |
| Background Color | Color | #000000 | Wall background color |
| Line Thickness | Float | 0.02 | Thickness of grid lines |
| Grid Scale | Float | 2.0 | Lines per unit (2 = one line every 0.5m) |
| Emission Intensity | Float | 1.5 | Glow strength |

## Shader Logic

1. Input UV coordinates from mesh
2. Scale UVs by Grid Scale parameter
3. Use `frac(UV)` to get position within each cell
4. Use `step()` to create horizontal and vertical lines
5. Combine H and V lines with `max()` or `saturate(add)`
6. Final color: `lerp(Background, GridColor, lines)`
7. Emission: `GridColor * lines * EmissionIntensity`

## Integration

Update `GridManager.cs` to use `TronGridWall.mat` as the default wall material instead of creating a basic gray material at runtime.
