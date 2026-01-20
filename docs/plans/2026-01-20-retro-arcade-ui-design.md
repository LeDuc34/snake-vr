# Retro Arcade UI System Design

## Overview

A complete UI system for Snake VR featuring a retro arcade aesthetic with chunky pixel fonts, neon colors, and head-locked overlay positioning for VR comfort.

## Design Principles

- **Retro arcade aesthetic**: Chunky pixel fonts, neon glow effects, rainbow color palette
- **Head-locked overlay**: UI fixed to player view like playing inside an arcade cabinet
- **VR comfort**: Elements in peripheral vision, comfortable depth (2m), slight transparency
- **Minimal interaction**: Controller pointer with trigger to select

---

## HUD (In-Game)

### Layout: Corner Clusters

The in-game HUD uses a head-locked overlay positioned at a comfortable 2m depth with slight transparency (80% opacity).

### Top-Left Corner - Score
- Large chunky pixel font in **yellow**
- Format: `SCORE 00000`
- Pulses/flashes briefly when score increases

### Top-Right Corner - Performance Stats
- **Green** for length: `LENGTH 03`
- **Cyan** for speed: `SPEED LV.2`
- Stacked vertically, slightly smaller than score

### Bottom-Left Corner - High Score
- **Magenta/Pink**: `HI-SCORE 00000`
- Flashes when current score exceeds it

### Bottom-Right Corner - Time
- **White**: `TIME 00:00`
- Simple minutes:seconds counter

### Visual Treatment
- All text uses chunky pixel font (8-bit arcade style)
- Subtle neon glow/bloom around text
- Dark semi-transparent backing behind each cluster for readability
- No borders or boxes - just floating arcade text with glow

### VR Comfort
- Elements sit in peripheral vision, not obstructing center view
- Fixed to head rotation but with very slight lag (2-3 frames) to reduce rigidity
- Never closer than the game world to avoid focus conflict

---

## Main Menu

### Trigger
Appears when game launches, before gameplay starts.

### Layout (top to bottom)
- **Game title**: "SNAKE VR" in large chunky pixels, rainbow gradient or cycling colors, with strong neon glow
- **Decorative line**: Simple pixel art divider
- **START GAME** button - Yellow text, highlights with glow on hover
- **QUIT** button - White text, highlights on hover

### Visual Treatment
- Dark semi-transparent backdrop (rounded rectangle, arcade cabinet feel)
- Buttons are text-only with neon glow, no boxes
- Hovered button scales up slightly (1.1x) and glow intensifies
- Selected button flashes briefly before action triggers
- Optional: small pixel-art snake animation coiling around the title

### Interaction
- Controller pointer (ray from either hand)
- Ray visualized as thin cyan laser line
- Circular reticle dot where ray hits
- Trigger press to select highlighted option
- Audio: retro "blip" on hover, "confirm" beep on select

### Flow
- START GAME → menu fades out, brief countdown (3-2-1-GO!), gameplay begins
- QUIT → exits application

---

## Pause Menu

### Trigger
Menu button on controller. Game freezes (Time.timeScale = 0).

### Layout (top to bottom)
- **Header**: "PAUSED" in cyan, chunky pixels with pulsing glow
- **RESUME** button - Green text (go/continue association)
- **RESTART** button - Yellow text
- **QUIT TO MENU** button - White text

### Visual Treatment
- Same dark semi-transparent backdrop as main menu
- Gameplay world visible but dimmed/desaturated behind the menu
- Same hover effects: scale up, glow intensifies
- Current run stats shown subtly at bottom: `SCORE: 150 | LENGTH: 5 | TIME: 1:23`

### Interaction
- Same controller pointer + trigger as main menu
- Menu button again also triggers RESUME (quick unpause)
- Same audio feedback: blip on hover, beep on select

### Flow
- RESUME → menu fades, brief "3-2-1" countdown, gameplay resumes
- RESTART → immediate reset, same countdown as new game
- QUIT TO MENU → returns to main menu (current run lost)

---

## Game Over Screen

### Trigger
Snake collides with wall or own body. Game freezes.

### Layout (top to bottom)
- **Header**: "GAME OVER" in red, chunky pixels, dramatic flicker/flash effect
- **Stats block** (centered, stacked):
  - `FINAL SCORE` - Large yellow number
  - `LENGTH REACHED` - Green, e.g., "12 SEGMENTS"
  - `TIME SURVIVED` - Cyan, e.g., "2:47"
  - `HIGH SCORE` - Magenta, shows the high score value
- **New high score indicator**: If beaten, "NEW HIGH SCORE!" flashes in rainbow colors between header and stats
- **Buttons**:
  - **PLAY AGAIN** - Yellow text
  - **QUIT TO MENU** - White text

### Visual Treatment
- Slightly larger backdrop than other menus
- Stats appear with staggered animation (score first, then length, then time) for dramatic reveal
- Background world frozen and heavily dimmed
- If new high score: celebratory particle effects (pixel confetti/sparkles)

### Interaction
- Same controller pointer + trigger
- Same audio: hover blips, select beeps
- New high score plays a special retro jingle

### Flow
- PLAY AGAIN → stats fade, countdown, new game starts
- QUIT TO MENU → returns to main menu

---

## Color Reference

| Element | Color | Hex (approximate) |
|---------|-------|-------------------|
| Score | Yellow | #FFD700 |
| Length | Green | #00FF00 |
| Speed | Cyan | #00FFFF |
| High Score | Magenta | #FF00FF |
| Time | White | #FFFFFF |
| Game Over | Red | #FF0000 |
| Paused | Cyan | #00FFFF |
| Resume | Green | #00FF00 |
| Backgrounds | Dark gray | #1A1A1A (80% opacity) |

---

## Technical Considerations

### Unity Implementation
- Use Unity UI (Canvas) set to World Space, parented to camera with offset
- TextMeshPro for crisp pixel fonts at any resolution
- Custom shader for neon glow effect (bloom post-processing or sprite glow)
- XR Ray Interactor from XR Interaction Toolkit for pointer interaction

### Font
- Need a chunky pixel/8-bit style font (e.g., "Press Start 2P", "VCR OSD Mono", or similar)
- Import as TextMeshPro font asset

### Audio
- Retro 8-bit sound effects for UI feedback
- Hover blip, select beep, countdown beeps, game over jingle, high score fanfare

### State Integration
- HUD reads from GameManager (score, speed) and SnakeController (segment count)
- Menus trigger GameManager state changes (StartGame, PauseGame, ResumeGame, GameOver)
- Add time tracking to GameManager (elapsed play time)
- Add high score persistence (PlayerPrefs)
