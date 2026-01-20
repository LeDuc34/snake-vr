# Unity UI Setup Guide - Retro Arcade UI System

This guide walks through setting up the retro arcade UI system in Unity.

## Prerequisites

1. **TextMeshPro** - Should already be installed (Unity includes it by default)
2. **Pixel Font** - Download "Press Start 2P" from [Google Fonts](https://fonts.google.com/specimen/Press+Start+2P)

---

## Step 1: Import Pixel Font

1. Download "Press Start 2P" font (TTF file)
2. Create folder: `Assets/SnakeVR/Fonts/`
3. Drag the `.ttf` file into that folder
4. Select the font, then go to **Window → TextMeshPro → Font Asset Creator**
5. Settings:
   - Source Font File: PressStart2P-Regular
   - Sampling Point Size: 64
   - Padding: 5
   - Packing Method: Optimum
   - Atlas Resolution: 1024 x 1024
6. Click **Generate Font Atlas**, then **Save** to `Assets/SnakeVR/Fonts/PressStart2P SDF.asset`

---

## Step 2: Create UI Root Structure

### 2.1 Create UI Manager GameObject

1. In Hierarchy, create empty: **GameObject → Create Empty**
2. Name it `UIManager`
3. Add component: **Add Component → SnakeVR.UI.UIManager**

### 2.2 Create Main UI Canvas (for HUD)

1. Right-click `XR Origin` → **UI → Canvas**
2. Name it `HUDCanvas`
3. Configure Canvas component:
   - **Render Mode**: World Space
   - **Event Camera**: (leave empty, will auto-find)
4. Configure RectTransform:
   - **Position**: (0, 0, 2) — 2 meters in front
   - **Width**: 1.6
   - **Height**: 0.9
   - **Scale**: (0.001, 0.001, 0.001) — scales canvas to meters
5. Add component: **CanvasGroup**
6. Add component: **SnakeVR.UI.ArcadeHUD**

### 2.3 Create Menu Canvas (for menus)

1. Right-click in Hierarchy → **UI → Canvas**
2. Name it `MenuCanvas`
3. Configure Canvas component:
   - **Render Mode**: World Space
4. Configure RectTransform:
   - **Position**: (0, 1.5, 3) — in front of player at eye level
   - **Width**: 2
   - **Height**: 1.5
   - **Scale**: (0.001, 0.001, 0.001)
5. Add component: **CanvasGroup**

---

## Step 3: Create HUD Elements

All elements below are children of `HUDCanvas`.

### 3.1 Score Text (Top-Left)

1. Right-click `HUDCanvas` → **UI → Text - TextMeshPro**
2. Name it `ScoreText`
3. RectTransform:
   - **Anchor**: Top-Left
   - **Pivot**: (0, 1)
   - **Pos X**: 50, **Pos Y**: -50
   - **Width**: 400, **Height**: 60
4. TextMeshPro settings:
   - **Text**: `SCORE 00000`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 36
   - **Color**: #FFD700 (Yellow)
   - **Alignment**: Left, Middle

### 3.2 Length Text (Top-Right)

1. Right-click `HUDCanvas` → **UI → Text - TextMeshPro**
2. Name it `LengthText`
3. RectTransform:
   - **Anchor**: Top-Right
   - **Pivot**: (1, 1)
   - **Pos X**: -50, **Pos Y**: -50
   - **Width**: 300, **Height**: 50
4. TextMeshPro settings:
   - **Text**: `LENGTH 03`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 28
   - **Color**: #00FF00 (Green)
   - **Alignment**: Right, Middle

### 3.3 Speed Text (Top-Right, below Length)

1. Right-click `HUDCanvas` → **UI → Text - TextMeshPro**
2. Name it `SpeedText`
3. RectTransform:
   - **Anchor**: Top-Right
   - **Pivot**: (1, 1)
   - **Pos X**: -50, **Pos Y**: -100
   - **Width**: 300, **Height**: 50
4. TextMeshPro settings:
   - **Text**: `SPEED LV.1`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 28
   - **Color**: #00FFFF (Cyan)
   - **Alignment**: Right, Middle

### 3.4 High Score Text (Bottom-Left)

1. Right-click `HUDCanvas` → **UI → Text - TextMeshPro**
2. Name it `HighScoreText`
3. RectTransform:
   - **Anchor**: Bottom-Left
   - **Pivot**: (0, 0)
   - **Pos X**: 50, **Pos Y**: 50
   - **Width**: 400, **Height**: 50
4. TextMeshPro settings:
   - **Text**: `HI-SCORE 00000`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 28
   - **Color**: #FF00FF (Magenta)
   - **Alignment**: Left, Middle

### 3.5 Time Text (Bottom-Right)

1. Right-click `HUDCanvas` → **UI → Text - TextMeshPro**
2. Name it `TimeText`
3. RectTransform:
   - **Anchor**: Bottom-Right
   - **Pivot**: (1, 0)
   - **Pos X**: -50, **Pos Y**: 50
   - **Width**: 300, **Height**: 50
4. TextMeshPro settings:
   - **Text**: `TIME 00:00`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 28
   - **Color**: #FFFFFF (White)
   - **Alignment**: Right, Middle

### 3.6 Wire Up ArcadeHUD

1. Select `HUDCanvas`
2. In the ArcadeHUD component, drag the text objects to their slots:
   - Score Text → `ScoreText`
   - Length Text → `LengthText`
   - Speed Text → `SpeedText`
   - High Score Text → `HighScoreText`
   - Time Text → `TimeText`

---

## Step 4: Create Main Menu

### 4.1 Main Menu Panel

1. Right-click `MenuCanvas` → **Create Empty**
2. Name it `MainMenu`
3. Add component: **SnakeVR.UI.MainMenu**
4. RectTransform:
   - **Anchor**: Stretch
   - **Left/Right/Top/Bottom**: 0

### 4.2 Menu Background

1. Right-click `MainMenu` → **UI → Image**
2. Name it `Background`
3. RectTransform: Stretch to fill
4. Image settings:
   - **Color**: #1A1A1A with Alpha ~200 (semi-transparent dark)

### 4.3 Title Text

1. Right-click `MainMenu` → **UI → Text - TextMeshPro**
2. Name it `TitleText`
3. RectTransform:
   - **Anchor**: Top-Center
   - **Pos Y**: -150
   - **Width**: 800, **Height**: 120
4. TextMeshPro settings:
   - **Text**: `SNAKE VR`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 72
   - **Color**: #FF0000 (will animate)
   - **Alignment**: Center, Middle

### 4.4 Start Button

1. Right-click `MainMenu` → **Create Empty**
2. Name it `StartButton`
3. Add component: **SnakeVR.UI.ArcadeMenuButton**
4. Add component: **Box Collider**
   - **Size**: (400, 80, 10)
5. RectTransform:
   - **Anchor**: Middle-Center
   - **Pos Y**: 0
   - **Width**: 400, **Height**: 80
6. Create child Text:
   - Right-click `StartButton` → **UI → Text - TextMeshPro**
   - **Text**: `START GAME`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 32
   - **Color**: #FFD700 (Yellow)
   - **Alignment**: Center, Middle
7. In ArcadeMenuButton, set **Normal Color**: #FFD700

### 4.5 Quit Button

1. Duplicate `StartButton`, name it `QuitButton`
2. Move to **Pos Y**: -120
3. Change text to `QUIT`
4. In ArcadeMenuButton, set **Normal Color**: #FFFFFF

### 4.6 Wire Up MainMenu

1. Select `MainMenu`
2. In MainMenu component:
   - Title Text → `TitleText`
   - Start Button → `StartButton`
   - Quit Button → `QuitButton`

---

## Step 5: Create Pause Menu

### 5.1 Pause Menu Panel

1. Right-click `MenuCanvas` → **Create Empty**
2. Name it `PauseMenu`
3. Add component: **SnakeVR.UI.PauseMenu**
4. RectTransform: Stretch to fill

### 5.2 Create Elements

Follow similar pattern to Main Menu:

1. **Background** - Semi-transparent dark (#1A1A1A, alpha 200)
2. **HeaderText** - "PAUSED" in Cyan (#00FFFF), size 56
3. **StatsText** - Small text at bottom for current score/length/time
4. **ResumeButton** - "RESUME" in Green (#00FF00)
5. **RestartButton** - "RESTART" in Yellow (#FFD700)
6. **QuitToMenuButton** - "QUIT TO MENU" in White

### 5.3 Wire Up PauseMenu

Drag each element to its slot in the PauseMenu component.

---

## Step 6: Create Game Over Screen

### 6.1 Game Over Panel

1. Right-click `MenuCanvas` → **Create Empty**
2. Name it `GameOverScreen`
3. Add component: **SnakeVR.UI.GameOverScreen**
4. RectTransform: Stretch to fill

### 6.2 Create Elements

1. **Background** - Semi-transparent dark
2. **HeaderText** - "GAME OVER" in Red (#FF0000), size 64
3. **NewHighScoreText** - "NEW HIGH SCORE!" (hidden by default)
4. **FinalScoreText** - Large yellow text
5. **LengthReachedText** - Green text
6. **TimeSurvivedText** - Cyan text
7. **HighScoreText** - Magenta text
8. **PlayAgainButton** - "PLAY AGAIN" in Yellow
9. **QuitToMenuButton** - "QUIT TO MENU" in White

### 6.3 Wire Up GameOverScreen

Drag each element to its slot.

---

## Step 7: Create Countdown Text

1. Right-click `MenuCanvas` → **UI → Text - TextMeshPro**
2. Name it `CountdownText`
3. RectTransform:
   - **Anchor**: Middle-Center
   - **Width**: 400, **Height**: 200
4. TextMeshPro settings:
   - **Text**: `3`
   - **Font Asset**: PressStart2P SDF
   - **Font Size**: 120
   - **Color**: #FFD700
   - **Alignment**: Center, Middle
5. Disable the GameObject (UIManager will enable it during countdown)

---

## Step 8: Wire Up UIManager

1. Select `UIManager` GameObject
2. In UIManager component, assign:
   - **HUD**: `HUDCanvas` (the ArcadeHUD component)
   - **Main Menu**: `MainMenu` (the MainMenu component)
   - **Pause Menu**: `PauseMenu` (the PauseMenu component)
   - **Game Over Screen**: `GameOverScreen` (the GameOverScreen component)
   - **Countdown Text**: `CountdownText`

---

## Step 9: Set Up Controller Pointers (Optional Enhancement)

For visual laser pointers from controllers:

1. On each XR Controller (Left/Right), create a child LineRenderer
2. Configure:
   - **Start Width**: 0.005
   - **End Width**: 0.005
   - **Material**: Sprites-Default
   - **Start Color / End Color**: Cyan (#00FFFF)

The ArcadeMenuBase script will handle the raycast logic automatically.

---

## Step 10: Add Audio (Optional)

1. Create folder: `Assets/SnakeVR/Audio/`
2. Add retro 8-bit sound effects:
   - `hover.wav` - Short blip
   - `select.wav` - Confirmation beep
   - `countdown.wav` - Beep for 3-2-1
   - `go.wav` - "GO!" sound
3. Assign to UIManager and menu components

---

## Hierarchy Overview

```
Scene
├── UIManager (UIManager script)
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera
│   │       └── HUDCanvas (ArcadeHUD script)
│   │           ├── ScoreText
│   │           ├── LengthText
│   │           ├── SpeedText
│   │           ├── HighScoreText
│   │           └── TimeText
│   ├── Left Controller
│   └── Right Controller
├── MenuCanvas
│   ├── MainMenu (MainMenu script)
│   │   ├── Background
│   │   ├── TitleText
│   │   ├── StartButton (ArcadeMenuButton script)
│   │   └── QuitButton (ArcadeMenuButton script)
│   ├── PauseMenu (PauseMenu script)
│   │   ├── Background
│   │   ├── HeaderText
│   │   ├── StatsText
│   │   ├── ResumeButton
│   │   ├── RestartButton
│   │   └── QuitToMenuButton
│   ├── GameOverScreen (GameOverScreen script)
│   │   ├── Background
│   │   ├── HeaderText
│   │   ├── NewHighScoreText
│   │   ├── FinalScoreText
│   │   ├── LengthReachedText
│   │   ├── TimeSurvivedText
│   │   ├── HighScoreText
│   │   ├── PlayAgainButton
│   │   └── QuitToMenuButton
│   └── CountdownText
├── GameManager
├── GridManager
└── FoodSpawner
```

---

## Testing

1. Press Play in Unity Editor
2. Main menu should appear (game starts in Menu state)
3. Click START GAME → Countdown → Gameplay with HUD
4. Press Menu button → Pause menu
5. Collide with wall → Game over screen
6. Use mouse to simulate controller pointer in editor

---

## Troubleshooting

**Menu buttons not responding:**
- Ensure BoxCollider is on the button GameObject
- Check that the collider size is large enough
- Verify ArcadeMenuButton component is attached

**Text not visible:**
- Check Canvas Render Mode is World Space
- Verify Canvas scale is small (0.001, 0.001, 0.001)
- Ensure TextMeshPro font asset is assigned

**HUD not updating:**
- Verify ArcadeHUD text references are assigned
- Check GameManager events are firing (OnScoreChanged, etc.)

**Pointer not working:**
- In editor, mouse acts as pointer
- For VR, ensure XR controllers are set up and detected
