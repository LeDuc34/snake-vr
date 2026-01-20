# Special Food Types - Design Document

**Goal:** Add 7 special food types with unique effects to increase gameplay variety.

**Tech Stack:** Unity 6, XR Interaction Toolkit 3.3.1, C#

---

## Food Types

### Normal Food (80-85% spawn rate)
- **Color:** Green (#4CAF50)
- **Effect:** Adds 1 segment, awards base points

### Special Foods (15-20% spawn rate combined)

| Type | Color | Hex | Effect | Duration | Rarity |
|------|-------|-----|--------|----------|--------|
| Speed Boost | Orange | #FF9800 | 1.5x movement speed | 8 sec | Common (25%) |
| Slow-Mo | Blue | #2196F3 | 0.5x game time scale | 5 sec | Common (25%) |
| Super Growth | Yellow | #FFEB3B | Add 3 segments | Instant | Common (25%) |
| Shrink | Pink | #E91E63 | Lose 2 segments, 3x points | Instant | Uncommon (15%) |
| Point Multiplier | Gold | #FFD700 | Next 5 foods worth 2x | 5 foods or 15 sec | Uncommon (10%) |
| Ghost Mode | Cyan | #00BCD4 | Pass through own body | 4 sec | Rare (5%) |
| Magnet | Purple | #9C27B0 | Pull nearby food toward head | 10 sec | Rare (5%) |

**Effective spawn rates per food:**
- Normal: ~82%
- Speed Boost: ~4.5%
- Slow-Mo: ~4.5%
- Super Growth: ~4.5%
- Shrink: ~2.7%
- Point Multiplier: ~1.8%
- Ghost Mode: ~0.9%
- Magnet: ~0.9%

---

## Architecture

### New Scripts

**FoodType.cs** - Enum defining all food types
```csharp
namespace SnakeVR
{
    public enum FoodType
    {
        Normal,
        SpeedBoost,
        SlowMo,
        Shrink,
        SuperGrowth,
        GhostMode,
        PointMultiplier,
        Magnet
    }
}
```

**SpecialFoodEffect.cs** - ScriptableObject for each food's properties
- Color
- FoodType enum value
- Effect duration
- Spawn weight
- Any effect-specific values (multiplier amount, segments to add/remove, etc.)

**SpecialFoodManager.cs** - Singleton managing active effects
- Tracks active effect timers
- Applies effects to SnakeController/GameManager
- Handles effect expiration and cleanup

**FoodVisual.cs** - Applies color to food based on type
- Attached to spawned food
- Sets material color from SpecialFoodEffect data

**ControllerEffectGlow.cs** - Visual feedback on VR controllers
- Shows active effect color on controllers
- Pulses during final 2 seconds before expiry

### Modified Scripts

**FoodSpawner.cs**
- Roll for food type on spawn using weighted random
- Assign FoodType to GrabbableFood
- Apply color via FoodVisual

**GrabbableFood.cs**
- Store FoodType field
- On eat: notify SpecialFoodManager.ApplyEffect(foodType)

**SnakeController.cs**
- Add `speedMultiplier` field (default 1.0)
- Add `isGhostMode` flag
- Add `SetSpeedMultiplier(float)` method
- Add `RemoveSegments(int)` method
- Modify `OnTriggerEnter` to check ghost flag before SnakeBody collision

**GameManager.cs**
- Add `pointMultiplier` field (default 1)
- Add `multiplierFoodsRemaining` counter
- Add `SetTimeScale(float)` method
- Scale `Time.fixedDeltaTime` proportionally for physics

---

## Data Flow

```
FoodSpawner.SpawnFood()
    │
    ├─► Roll random type (weighted)
    ├─► Instantiate food
    ├─► GrabbableFood.SetType(foodType)
    └─► FoodVisual.ApplyColor(foodType)

Player grabs and eats food
    │
    └─► GrabbableFood.Eat()
            │
            └─► SpecialFoodManager.ApplyEffect(foodType)
                    │
                    ├─► SpeedBoost: SnakeController.SetSpeedMultiplier(1.5f)
                    ├─► SlowMo: GameManager.SetTimeScale(0.5f)
                    ├─► Shrink: SnakeController.RemoveSegments(2)
                    ├─► SuperGrowth: SnakeController.AddSegments(3)
                    ├─► GhostMode: SnakeController.SetGhostMode(true)
                    ├─► PointMultiplier: GameManager.SetPointMultiplier(2, 5)
                    └─► Magnet: SpecialFoodManager.EnableMagnet()
```

---

## Effect Implementation Details

### Speed Boost
- `SnakeController.speedMultiplier` applied in `Move()`
- Set to 1.5, coroutine resets to 1.0 after 8 seconds

### Slow-Mo
- `GameManager.SetTimeScale(0.5f)`
- `Time.fixedDeltaTime` scaled proportionally
- Timer uses `Time.unscaledDeltaTime` (real-time countdown)
- Reset to 1.0 after 5 seconds

### Shrink
- `SnakeController.RemoveSegments(2)` destroys last 2 segments
- If fewer than 2 segments exist, remove what's there (no game over)
- Awards 3x base points as compensation

### Super Growth
- Calls existing `AddSegment()` three times
- Normal point value

### Ghost Mode
- Set `isGhostMode = true` on SnakeController
- `OnTriggerEnter` checks flag before SnakeBody game over
- 0.5 second grace period when effect expires (in case inside body)

### Point Multiplier
- `GameManager.pointMultiplier = 2`
- `GameManager.multiplierFoodsRemaining = 5`
- On food eat: apply multiplier, decrement counter
- Reset when counter hits 0 or 15 seconds pass

### Magnet
- Each frame in `SpecialFoodManager.Update()`
- Find all food within radius (5 units) of player head
- Apply force toward head using Rigidbody.AddForce()
- Works with existing food Rigidbody physics

---

## Effect Stacking Rules

| Scenario | Behavior |
|----------|----------|
| Same effect eaten twice | Reset duration (no stacking multipliers) |
| Different effects active | Both run simultaneously |
| Slow-Mo + Speed Boost | Both apply: 0.5x world, 1.5x snake speed |
| Point Multiplier refresh | Reset food counter to 5, keep 2x |

---

## Edge Cases

| Scenario | Behavior |
|----------|----------|
| Shrink when 0-1 segments | Remove what exists, no game over |
| Ghost expires while inside body | 0.5 second grace period before collision |
| Magnet pulls food into wall | Normal bounce (existing Rigidbody physics) |
| Eat special while paused | Effect timer pauses (except Slow-Mo uses unscaledTime) |
| Death during effect | All effects cleared, timers stop, multipliers reset |

---

## Visual Feedback

### Food Colors
Each food type gets a simple unlit material with its designated color, applied at spawn.

### Active Effect Indication
- VR controllers glow with the active effect's color
- Multiple effects: blend colors or show dominant/most recent
- Final 2 seconds: glow pulses to warn of expiration

### Controller Glow Implementation
- `ControllerEffectGlow.cs` on each controller
- `SpecialFoodManager` notifies when effects start/end
- Emission color on controller material or child glow object

---

## File Summary

### New Files
| File | Purpose |
|------|---------|
| `Scripts/Food/FoodType.cs` | Enum for all food types |
| `Scripts/Food/SpecialFoodEffect.cs` | ScriptableObject for food properties |
| `Scripts/Food/FoodVisual.cs` | Applies color to food |
| `Scripts/Managers/SpecialFoodManager.cs` | Manages active effects |
| `Scripts/Interaction/ControllerEffectGlow.cs` | Controller visual feedback |
| `ScriptableObjects/FoodEffects/` | 8 .asset files (one per food type) |

### Modified Files
| File | Changes |
|------|---------|
| `Scripts/Food/FoodSpawner.cs` | Weighted type selection, apply type to food |
| `Scripts/Interaction/GrabbableFood.cs` | Store type, trigger effect on eat |
| `Scripts/Snake/SnakeController.cs` | Speed multiplier, ghost mode, RemoveSegments() |
| `Scripts/Managers/GameManager.cs` | Time scale, point multiplier |
