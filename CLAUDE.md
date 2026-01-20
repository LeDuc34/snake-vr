# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Snake VR is a virtual reality Snake game for Meta Quest built with Unity 6 (6000.3.1f1) and XR Interaction Toolkit 3.3.1. The player's VR camera is attached to the snake head, creating an immersive first-person snake experience.

## Build and Test Commands

This is a Unity project - there are no CLI build commands. Work is done through the Unity Editor:

- **Open project**: Unity Hub → Open → select this folder
- **Build for Quest**: File → Build Settings → Android → Build And Run
- **Test in editor**: Press Play button (IJKL keys simulate joystick)

## Architecture

All game code is in the `SnakeVR` namespace under `Assets/SnakeVR/Scripts/`.

### Core Components

**GameManager** (Singleton) - Central game controller
- Manages game states: Menu, Playing, Paused, GameOver
- Controls Time.timeScale for pause functionality
- Orchestrates score and speed progression via events (OnScoreChanged, OnGameStateChanged)

**SnakeController** - Attached to XR Origin (the VR rig IS the snake head)
- Moves the entire XR Origin, so the camera follows the snake
- Maintains position history for segment following
- 90-degree discrete turns in 3D space (horizontal and vertical)
- Prevents 180-degree reversal

**VRInputManager** - Four control schemes available
- LeftJoystick/RightJoystick (Quest controllers)
- HeadGaze (look direction)
- RightControllerDirection (point to steer)
- Editor fallback: IJKL keys

**GridManager** - Defines play area boundaries
- Walls: triggers game over on collision
- Wraparound: teleports to opposite side
- None: no boundaries

**FoodSpawner** - Spawns food avoiding snake positions

### Required Unity Tags

Create these in Edit → Project Settings → Tags and Layers:
- `Food`
- `Wall`
- `SnakeBody`

### Key Prefabs

- **SnakeSegment**: Body segment prefab in `Assets/SnakeVR/Prefabs/`
- Segments follow the head using position history, not physics

### XR Setup

The XR Origin GameObject from XR Interaction Toolkit serves as the snake head. The SnakeController script is attached directly to it, so camera movement = snake movement.

## Git Workflow

```bash
git add Assets/ ProjectSettings/ Packages/
git commit -m "Description"
git push
```

Never commit: Library/, Temp/, Logs/, Obj/, Build/

Always commit: `.meta` files (Unity asset references)
