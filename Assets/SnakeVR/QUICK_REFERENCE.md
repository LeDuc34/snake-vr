# Aide-Mémoire Rapide - Configuration Snake VR

## 🎯 Checklist complète

### 1. Créer la scène
- [ ] File > New Scene > Save As: `Assets/SnakeVR/Scenes/SnakeGame.unity`
- [ ] Supprimer Main Camera par défaut

### 2. Ajouter XR Origin
- [ ] Hierarchy > XR > XR Origin (VR)
- [ ] Position: (0, 0, 0)

### 3. Créer les Tags
- [ ] Edit > Project Settings > Tags and Layers
- [ ] Ajouter: `Food`, `Wall`, `SnakeBody`

### 4. GameManager
- [ ] Create Empty → Nommer: `GameManager`
- [ ] Add Component: `GameManager`
- [ ] Position: (0, 0, 0)
- [ ] Paramètres: Speed 2, Increase 0.1, Score 10
- [ ] ⚠️ References: À remplir plus tard

### 5. SnakeSegment Prefab
- [ ] 3D Object > Cube → Nommer: `SnakeSegment`
- [ ] Transform: Position (10,0,0), Scale (0.25, 0.25, 0.25)
- [ ] Box Collider: Is Trigger ✅
- [ ] Add Component: `SnakeSegment`
- [ ] Couleur: Vert clair
- [ ] Tag: `SnakeBody`
- [ ] Glisser dans `Assets/SnakeVR/Prefabs/`
- [ ] Supprimer de Hierarchy

### 6. SnakeHead
- [ ] 3D Object > Cube → Nommer: `SnakeHead`
- [ ] Transform: Position (0, 1.5, 0), Scale (0.3, 0.3, 0.3)
- [ ] Add Component: `Rigidbody`
  - [ ] Use Gravity: ❌
  - [ ] Is Kinematic: ✅
- [ ] Box Collider: Is Trigger ✅
- [ ] Add Component: `SnakeController`
- [ ] **Segment Prefab: Glisser `SnakeSegment.prefab`** ⚠️ IMPORTANT
- [ ] Paramètres: Spacing 0.3, Initial Count 3, Speed 2, Turn 90
- [ ] Couleur: Vert foncé

### 7. FoodSpawner
- [ ] Create Empty → Nommer: `FoodSpawner`
- [ ] Position: (0, 1.5, 0)
- [ ] Add Component: `FoodSpawner`
- [ ] Paramètres: Size 0.2, Color Rouge, Area (5,3,5), Grid 0.3

### 8. VRInputManager
- [ ] Create Empty → Nommer: `V  RInputManager`
- [ ] Position: (0, 0, 0)
- [ ] Add Component: `VRInputManager`
- [ ] Control Scheme: `Left Joystick`
- [ ] **Camera Transform: Glisser `Main Camera`** ⚠️ IMPORTANT
  - (depuis XR Origin > Camera Offset > Main Camera)

### 9. GridManager
- [ ] Create Empty → Nommer: `GridManager`
- [ ] Position: (0, 1.5, 0)
- [ ] Add Component: `GridManager`
- [ ] Grid Size: (5, 3, 5), Step: 0.3
- [ ] Boundary Type: `Walls`
- [ ] Show Grid ✅, Show Boundaries ✅

### 10. Connecter GameManager
- [ ] Sélectionner `GameManager` dans Hierarchy
- [ ] **Snake Controller:** Glisser `SnakeHead`
- [ ] **Food Spawner:** Glisser `FoodSpawner`
- [ ] **Grid Manager:** Glisser `GridManager`

### 11. Sauvegarder et tester
- [ ] File > Save (Ctrl+S)
- [ ] Vérifier Console (pas d'erreurs)
- [ ] Cliquer Play ▶️ pour tester

### 12. Git
- [ ] git add Assets/
- [ ] git commit -m "Create SnakeGame scene with complete setup"
- [ ] git push

---

## 🔗 Connexions importantes

```
GameManager
├─► Snake Controller → SnakeHead
├─► Food Spawner → FoodSpawner
└─► Grid Manager → GridManager

SnakeHead (SnakeController)
└─► Segment Prefab → SnakeSegment.prefab

VRInputManager
└─► Camera Transform → Main Camera
```

---

## 🎮 Hiérarchie finale

```
SnakeGame
├── XR Origin (VR)
│   ├── Camera Offset
│   │   ├── Main Camera ← Connectée à VRInputManager
│   │   ├── Left Controller
│   │   └── Right Controller
├── GameManager [Script: GameManager]
├── SnakeHead [Scripts: SnakeController + Rigidbody]
├── FoodSpawner [Script: FoodSpawner]
├── VRInputManager [Script: VRInputManager]
└── GridManager [Script: GridManager]
```

---

## ⚙️ Paramètres clés

| GameObject | Composant | Paramètre | Valeur |
|------------|-----------|-----------|--------|
| GameManager | GameManager | Initial Speed | 2 |
| GameManager | GameManager | Speed Increase | 0.1 |
| GameManager | GameManager | Score Per Food | 10 |
| SnakeHead | SnakeController | Segment Spacing | 0.3 |
| SnakeHead | SnakeController | Initial Segment Count | 3 |
| SnakeHead | SnakeController | Move Speed | 2 |
| SnakeHead | SnakeController | Turn Speed | 90 |
| SnakeHead | Rigidbody | Use Gravity | ❌ |
| SnakeHead | Rigidbody | Is Kinematic | ✅ |
| SnakeHead | Box Collider | Is Trigger | ✅ |
| FoodSpawner | FoodSpawner | Food Size | 0.2 |
| FoodSpawner | FoodSpawner | Spawn Area Size | (5, 3, 5) |
| FoodSpawner | FoodSpawner | Grid Step | 0.3 |
| VRInputManager | VRInputManager | Control Scheme | Left Joystick |
| VRInputManager | VRInputManager | Joystick Deadzone | 0.3 |
| GridManager | GridManager | Grid Size | (5, 3, 5) |
| GridManager | GridManager | Grid Step | 0.3 |
| GridManager | GridManager | Boundary Type | Walls |

---

## 🏷️ Tags nécessaires

- `Food` (pour la nourriture)
- `Wall` (pour les murs)
- `SnakeBody` (pour les segments)

---

## ⚠️ Points d'attention

1. **Ne pas oublier:**
   - Glisser le prefab SnakeSegment dans SnakeHead
   - Connecter Main Camera dans VRInputManager
   - Connecter les 3 références dans GameManager

2. **Colliders:**
   - Tous les colliders doivent être en **Is Trigger**

3. **Tags:**
   - SnakeSegment prefab doit avoir le tag `SnakeBody`

4. **Positions:**
   - SnakeHead et FoodSpawner à Y: 1.5 (hauteur des yeux)
   - GridManager à Y: 1.5 (même hauteur)
   - XR Origin à (0, 0, 0)

---

## 🐛 Dépannage rapide

| Problème | Solution |
|----------|----------|
| Erreur NullReference | Vérifier toutes les connexions dans GameManager |
| Serpent ne bouge pas | Vérifier Camera Transform dans VRInputManager |
| Pas de nourriture | Vérifier FoodSpawner connecté dans GameManager |
| Erreur compilation | Assets > Reimport All |
| Missing script | Vérifier que les scripts sont dans les bons dossiers |

---

## 📱 Tester sur Quest

1. File > Build Settings
2. Android
3. Switch Platform
4. Connecter Quest en USB
5. Build And Run
