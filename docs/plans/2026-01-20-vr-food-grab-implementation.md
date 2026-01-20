# VR Food Grab - Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Allow players to grab food with VR controllers and eat it by bringing it to their head.

**Architecture:** Two new scripts (HandGrabber on controllers, GrabbableFood on food) handle the grab/release/eat logic. Food uses Rigidbody for physics-based throwing and wall bouncing.

**Tech Stack:** Unity 6, XR Interaction Toolkit 3.3.1, C#

---

## Task 1: Create GrabbableFood Script

**Files:**
- Create: `Assets/SnakeVR/Scripts/Interaction/GrabbableFood.cs`

**Step 1: Create the Interaction folder and GrabbableFood.cs**

```csharp
using UnityEngine;

namespace SnakeVR
{
    public enum FoodState
    {
        Free,
        Held
    }

    public class GrabbableFood : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float eatDistance = 0.3f;

        private FoodState currentState = FoodState.Free;
        private Transform holdingHand;
        private Rigidbody rb;
        private Collider col;
        private Transform headTransform;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            // Find the VR camera (head)
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                headTransform = mainCam.transform;
            }
        }

        private void Update()
        {
            if (currentState == FoodState.Held && holdingHand != null)
            {
                // Follow the hand
                transform.position = holdingHand.position;

                // Check if close to head (eat)
                if (headTransform != null)
                {
                    float distanceToHead = Vector3.Distance(transform.position, headTransform.position);
                    if (distanceToHead < eatDistance)
                    {
                        Eat();
                    }
                }
            }
        }

        public bool CanBeGrabbed()
        {
            return currentState == FoodState.Free;
        }

        public void Grab(Transform hand)
        {
            if (currentState != FoodState.Free) return;

            currentState = FoodState.Held;
            holdingHand = hand;

            // Disable physics while held
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            if (col != null)
            {
                col.enabled = false;
            }

            Debug.Log("Food grabbed");
        }

        public void Release(Vector3 velocity)
        {
            if (currentState != FoodState.Held) return;

            currentState = FoodState.Free;
            holdingHand = null;

            // Re-enable physics
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = velocity;
            }
            if (col != null)
            {
                col.enabled = true;
            }

            Debug.Log("Food released with velocity: " + velocity);
        }

        private void Eat()
        {
            // Notify game systems
            SnakeController snake = FindObjectOfType<SnakeController>();
            if (snake != null)
            {
                snake.AddSegment();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFoodEaten();
            }

            Debug.Log("Food eaten!");
            Destroy(gameObject);
        }

        public FoodState GetState()
        {
            return currentState;
        }
    }
}
```

**Step 2: Verify file created**

Open Unity Editor and check that `Assets/SnakeVR/Scripts/Interaction/GrabbableFood.cs` compiles without errors.

**Step 3: Commit**

```bash
git add Assets/SnakeVR/Scripts/Interaction/
git commit -m "feat: add GrabbableFood script for VR food grabbing"
```

---

## Task 2: Create HandGrabber Script

**Files:**
- Create: `Assets/SnakeVR/Scripts/Interaction/HandGrabber.cs`

**Step 1: Create HandGrabber.cs**

```csharp
using UnityEngine;
using UnityEngine.XR;
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

namespace SnakeVR
{
    public class HandGrabber : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private XRNode controllerNode = XRNode.RightHand;
        [SerializeField] private float grabRadius = 0.15f;
        [SerializeField] private float gripThreshold = 0.5f;

        private XRInputDevice controller;
        private GrabbableFood heldFood;
        private bool wasGripping = false;

        // For velocity calculation
        private Vector3 previousPosition;
        private Vector3 currentVelocity;

        private void Start()
        {
            controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            previousPosition = transform.position;
        }

        private void Update()
        {
            // Refresh controller if not valid
            if (!controller.isValid)
            {
                controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            }

            // Calculate velocity
            currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            // Get grip value
            float gripValue = GetGripValue();
            bool isGripping = gripValue > gripThreshold;

            // Grip just pressed
            if (isGripping && !wasGripping)
            {
                TryGrab();
            }
            // Grip just released
            else if (!isGripping && wasGripping)
            {
                TryRelease();
            }

            wasGripping = isGripping;
        }

        private float GetGripValue()
        {
            float gripValue = 0f;

            if (controller.isValid)
            {
                controller.TryGetFeatureValue(XRCommonUsages.grip, out gripValue);
            }

            // Keyboard fallback for testing (G key)
            if (gripValue == 0f && UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.gKey.isPressed)
                {
                    gripValue = 1f;
                }
            }

            return gripValue;
        }

        private void TryGrab()
        {
            if (heldFood != null) return;

            // Find food in range
            Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Food"))
                {
                    GrabbableFood food = col.GetComponent<GrabbableFood>();
                    if (food != null && food.CanBeGrabbed())
                    {
                        food.Grab(transform);
                        heldFood = food;
                        Debug.Log($"Hand grabbed food: {controllerNode}");
                        return;
                    }
                }
            }
        }

        private void TryRelease()
        {
            if (heldFood != null)
            {
                heldFood.Release(currentVelocity);
                heldFood = null;
                Debug.Log($"Hand released food: {controllerNode}");
            }
        }

        // Clear reference if food is destroyed (eaten)
        private void LateUpdate()
        {
            if (heldFood == null) return;

            // Check if food was destroyed
            if (heldFood.gameObject == null)
            {
                heldFood = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}
```

**Step 2: Verify file created**

Open Unity Editor and check that `Assets/SnakeVR/Scripts/Interaction/HandGrabber.cs` compiles without errors.

**Step 3: Commit**

```bash
git add Assets/SnakeVR/Scripts/Interaction/HandGrabber.cs
git commit -m "feat: add HandGrabber script for VR controller grabbing"
```

---

## Task 3: Modify FoodSpawner to Add GrabbableFood and Rigidbody

**Files:**
- Modify: `Assets/SnakeVR/Scripts/Food/FoodSpawner.cs:56-60` (SpawnFood method)
- Modify: `Assets/SnakeVR/Scripts/Food/FoodSpawner.cs:91-117` (CreateDefaultFoodPrefab method)

**Step 1: Update SpawnFood to add components**

In `FoodSpawner.cs`, after line 58 (`currentFood.tag = "Food";`), add:

```csharp
// Add GrabbableFood component
if (currentFood.GetComponent<GrabbableFood>() == null)
{
    currentFood.AddComponent<GrabbableFood>();
}

// Add Rigidbody for physics
Rigidbody rb = currentFood.GetComponent<Rigidbody>();
if (rb == null)
{
    rb = currentFood.AddComponent<Rigidbody>();
}
rb.useGravity = false;
rb.linearDamping = 2f; // Slow down thrown food
rb.angularDamping = 2f;
```

**Step 2: Update CreateDefaultFoodPrefab to use non-trigger collider**

In `CreateDefaultFoodPrefab()`, change the collider setup (around line 96-101):

```csharp
// Use non-trigger collider for physics interactions
Collider col = food.GetComponent<Collider>();
if (col != null)
{
    col.isTrigger = false; // Changed from true - needed for physics bouncing
}
```

**Step 3: Verify changes compile**

Open Unity Editor and verify FoodSpawner.cs compiles without errors.

**Step 4: Commit**

```bash
git add Assets/SnakeVR/Scripts/Food/FoodSpawner.cs
git commit -m "feat: add GrabbableFood and Rigidbody to spawned food"
```

---

## Task 4: Remove Food Collision from SnakeController

**Files:**
- Modify: `Assets/SnakeVR/Scripts/Snake/SnakeController.cs:275-296` (OnTriggerEnter method)

**Step 1: Remove food handling from OnTriggerEnter**

Replace the `OnTriggerEnter` method to only handle Wall and SnakeBody collisions:

```csharp
private void OnTriggerEnter(Collider other)
{
    // Food is now handled by GrabbableFood + HandGrabber
    // Only handle game over conditions here
    if (other.CompareTag("Wall") || other.CompareTag("SnakeBody"))
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
```

**Step 2: Verify changes compile**

Open Unity Editor and verify SnakeController.cs compiles without errors.

**Step 3: Commit**

```bash
git add Assets/SnakeVR/Scripts/Snake/SnakeController.cs
git commit -m "refactor: remove food collision from SnakeController (now handled by VR grab)"
```

---

## Task 5: Unity Editor Setup

**This task is done manually in Unity Editor.**

**Step 1: Add HandGrabber to Left Controller**

1. In Hierarchy, find XR Origin → Camera Offset → Left Controller
2. Add Component → HandGrabber
3. Set Controller Node to "Left Hand"
4. Set Grab Radius to 0.15

**Step 2: Add HandGrabber to Right Controller**

1. In Hierarchy, find XR Origin → Camera Offset → Right Controller
2. Add Component → HandGrabber
3. Set Controller Node to "Right Hand"
4. Set Grab Radius to 0.15

**Step 3: Test in Editor**

1. Press Play
2. Use IJKL to move the snake
3. Press G key to simulate grip (should grab food if hand is near)
4. Move the "grabbed" food near the camera to eat it

**Step 4: Commit scene changes**

```bash
git add Assets/Scenes/
git commit -m "feat: add HandGrabber components to VR controllers"
```

---

## Task 6: Test on Quest (Optional)

**Step 1: Build and deploy**

File → Build Settings → Android → Build And Run

**Step 2: Test grab mechanics**

1. Start game
2. Move hand near food
3. Press grip button → food should attach to hand
4. Bring food near face → food should be eaten
5. Or release grip → food should be thrown and bounce off walls

---

## Summary

| Task | Description | Files |
|------|-------------|-------|
| 1 | Create GrabbableFood.cs | New: Interaction/GrabbableFood.cs |
| 2 | Create HandGrabber.cs | New: Interaction/HandGrabber.cs |
| 3 | Modify FoodSpawner | Modify: Food/FoodSpawner.cs |
| 4 | Remove food collision from SnakeController | Modify: Snake/SnakeController.cs |
| 5 | Unity Editor setup | Scene configuration |
| 6 | Quest testing | Build & test |
