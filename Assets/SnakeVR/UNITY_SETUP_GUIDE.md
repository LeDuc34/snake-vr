# Guide de Configuration Unity - Snake VR
## Configuration complète étape par étape

---

## 📋 ÉTAPE 1: Créer la nouvelle scène

1. Dans Unity, en haut: **File > New Scene**
2. Choisissez **Basic (Built-in)** ou **Empty**
3. Cliquez **Create**
4. Tout de suite après: **File > Save As...**
5. Naviguez vers: `Assets/SnakeVR/Scenes/`
6. Nom du fichier: **`SnakeGame`**
7. Cliquez **Save**

✅ **Vérification:** Dans le Project panel en bas, vous devez voir `Assets/SnakeVR/Scenes/SnakeGame.unity`

---

## 📋 ÉTAPE 2: Supprimer la Main Camera par défaut

1. Dans le **Hierarchy** panel (à gauche), trouvez **Main Camera**
2. Clic droit sur **Main Camera** → **Delete**

> ⚠️ Important: On va la remplacer par le XR Origin qui a sa propre caméra VR

---

## 📋 ÉTAPE 3: Ajouter le XR Origin (pour VR)

### A. Ajouter XR Origin

1. Dans **Hierarchy**, clic droit dans le vide
2. **XR > XR Origin (VR)**
3. Cela crée automatiquement:
   ```
   XR Origin (VR)
   ├── Camera Offset
   │   ├── Main Camera
   │   ├── Left Controller
   │   └── Right Controller
   └── ...
   ```

### B. Positionner le XR Origin

1. Sélectionnez **XR Origin (VR)** dans Hierarchy
2. Dans **Inspector** (panneau de droite):
   - **Transform**
   - Position: `X: 0, Y: 0, Z: 0`
   - Rotation: `X: 0, Y: 0, Z: 0`
   - Scale: `X: 1, Y: 1, Z: 1`

✅ **Vérification:** Le XR Origin doit être à l'origine du monde (0,0,0)

---

## 📋 ÉTAPE 4: Configurer les Tags Unity

Les tags permettent d'identifier les objets pour les collisions.

1. Menu en haut: **Edit > Project Settings**
2. Dans la fenêtre qui s'ouvre, cliquez **Tags and Layers** (dans la liste de gauche)
3. Sous **Tags**, cliquez le **+** (petit plus)
4. Ajoutez les tags suivants UN PAR UN:
   - `Food`
   - `Wall`
   - `SnakeBody`
5. Fermez la fenêtre Project Settings

✅ **Vérification:** Vous avez maintenant 3 nouveaux tags disponibles

---

## 📋 ÉTAPE 5: Créer le GameManager

### A. Créer l'objet vide

1. Dans **Hierarchy**, clic droit → **Create Empty**
2. Renommez-le: **`GameManager`**
3. Dans Inspector:
   - Position: `X: 0, Y: 0, Z: 0`

### B. Ajouter le script

1. GameManager toujours sélectionné
2. Dans **Inspector**, en bas: **Add Component**
3. Tapez: `GameManager`
4. Cliquez sur **Game Manager (Script)** dans les résultats

### C. Configurer les paramètres

Vous verrez maintenant les paramètres du script dans Inspector:

**Game Settings:**
- Initial Speed: `2`
- Speed Increase Per Food: `0.1`
- Score Per Food: `10`

**References:**
- ⚠️ Laissez vide pour l'instant (on les connectera plus tard)

✅ **Vérification:** GameManager a le script attaché et les paramètres sont visibles

---

## 📋 ÉTAPE 6: Créer le Prefab SnakeSegment (segment du corps)

### A. Créer un cube

1. Hierarchy, clic droit → **3D Object > Cube**
2. Renommez-le: **`SnakeSegment`**

### B. Configurer le Transform

Avec SnakeSegment sélectionné, dans Inspector:

**Transform:**
- Position: `X: 10, Y: 0, Z: 0` (hors du chemin)
- Rotation: `X: 0, Y: 0, Z: 0`
- Scale: `X: 0.25, Y: 0.25, Z: 0.25`

### C. Configurer le Collider

1. Dans Inspector, trouvez **Box Collider**
2. ✅ Cochez **Is Trigger**

### D. Ajouter le script SnakeSegment

1. Dans Inspector: **Add Component**
2. Tapez: `SnakeSegment`
3. Sélectionnez **Snake Segment (Script)**

**Paramètres du script:**
- Segment Color: Choisissez un **vert clair**
- Smooth Time: `0.1`

### E. Changer la couleur

1. Toujours dans Inspector, trouvez **Mesh Renderer**
2. Cliquez sur la petite flèche à côté de **Materials**
3. Sous **Element 0**, vous voyez **Default-Material**
4. Cliquez sur le cercle à droite de **Default-Material**
5. En haut de la fenêtre qui s'ouvre, tapez: "Green" ou créez un nouveau Material
6. OU plus simple: dans **Mesh Renderer > Materials > Element 0**, cliquez sur la couleur et choisissez vert

### F. Ajouter le Tag

1. En haut de l'Inspector, sous le nom "SnakeSegment"
2. Cliquez sur **Tag** → Sélectionnez **SnakeBody**

### G. Créer le Prefab

1. Dans le **Project** panel (en bas), naviguez vers: `Assets/SnakeVR/Prefabs/`
2. **Glissez-déposez** le `SnakeSegment` depuis **Hierarchy** vers le dossier **Prefabs**
3. Le texte devient bleu dans Hierarchy (c'est un prefab maintenant)
4. **Supprimez** le SnakeSegment de la Hierarchy (clic droit > Delete)
   - ⚠️ Le prefab reste dans le dossier Prefabs, c'est normal !

✅ **Vérification:** Dans `Assets/SnakeVR/Prefabs/`, vous voyez `SnakeSegment.prefab`

---

## 📋 ÉTAPE 7: Créer le SnakeHead

### A. Créer un cube

1. Hierarchy, clic droit → **3D Object > Cube**
2. Renommez-le: **`SnakeHead`**

### B. Configurer le Transform

**Transform:**
- Position: `X: 0, Y: 1.5, Z: 0` (au centre, à hauteur des yeux)
- Rotation: `X: 0, Y: 0, Z: 0`
- Scale: `X: 0.3, Y: 0.3, Z: 0.3`

### C. Ajouter un Rigidbody

1. **Add Component** → Tapez: `Rigidbody`
2. Dans les paramètres du Rigidbody:
   - ❌ **Décochez** Use Gravity
   - ✅ **Cochez** Is Kinematic

### D. Configurer le Collider

1. Trouvez **Box Collider** dans Inspector
2. ✅ **Cochez** Is Trigger

### E. Ajouter le script SnakeController

1. **Add Component** → Tapez: `SnakeController`
2. Sélectionnez **Snake Controller (Script)**

**Paramètres Snake Settings:**
- Segment Prefab: **⚠️ IMPORTANT - Glissez le prefab ici !**
  - Dans Project panel, naviguez vers `Assets/SnakeVR/Prefabs/`
  - **Glissez-déposez** `SnakeSegment.prefab` dans le champ **Segment Prefab**
- Segment Spacing: `0.3`
- Initial Segment Count: `3`
- Head Transform: *Laissez vide* (auto-détection)

**Paramètres Movement Settings:**
- Move Speed: `2`
- Turn Speed: `90`

### F. Changer la couleur (vert foncé)

Même procédé qu'avant, choisissez une couleur **vert foncé** pour différencier la tête

✅ **Vérification:** SnakeHead est configuré avec le script et le prefab connecté

---

## 📋 ÉTAPE 8: Créer le FoodSpawner

### A. Créer l'objet vide

1. Hierarchy, clic droit → **Create Empty**
2. Renommez: **`FoodSpawner`**

### B. Positionner

**Transform:**
- Position: `X: 0, Y: 1.5, Z: 0` (même hauteur que le snake)

### C. Ajouter le script

1. **Add Component** → `FoodSpawner`
2. Sélectionnez **Food Spawner (Script)**

**Paramètres Food Settings:**
- Food Prefab: *Laissez vide* (va créer automatiquement)
- Food Size: `0.2`
- Food Color: **Rouge**

**Paramètres Spawn Area:**
- Spawn Area Size: `X: 5, Y: 3, Z: 5`
- Grid Step: `0.3`
- ✅ Visualize Spawn Area: **Coché** (pour voir la zone dans l'éditeur)

✅ **Vérification:** Vous devriez voir un cadre jaune dans la Scene view

---

## 📋 ÉTAPE 9: Créer le VRInputManager

### A. Créer l'objet vide

1. Hierarchy, clic droit → **Create Empty**
2. Renommez: **`VRInputManager`**

### B. Positionner

**Transform:**
- Position: `X: 0, Y: 0, Z: 0`

### C. Ajouter le script

1. **Add Component** → `VRInputManager`
2. Sélectionnez **VRInput Manager (Script)**

**Paramètres Control Settings:**
- Control Scheme: **Left Joystick** (le plus simple pour commencer)
- Joystick Deadzone: `0.3`

**Paramètres Head Gaze Settings:**
- Camera Transform: **⚠️ IMPORTANT - Connecter la caméra !**
  - Dans Hierarchy, développez: `XR Origin (VR) > Camera Offset > Main Camera`
  - **Glissez-déposez** `Main Camera` dans le champ **Camera Transform**

✅ **Vérification:** Main Camera est connectée dans Camera Transform

---

## 📋 ÉTAPE 10: Créer le GridManager

### A. Créer l'objet vide

1. Hierarchy, clic droit → **Create Empty**
2. Renommez: **`GridManager`**

### B. Positionner

**Transform:**
- Position: `X: 0, Y: 1.5, Z: 0`

### C. Ajouter le script

1. **Add Component** → `GridManager`
2. Sélectionnez **Grid Manager (Script)**

**Paramètres Grid Settings:**
- Grid Size: `X: 5, Y: 3, Z: 5`
- Grid Step: `0.3`
- Boundary Type: **Walls** (pour avoir des murs)

**Paramètres Visual Settings:**
- ✅ Show Grid: **Coché**
- ✅ Show Boundaries: **Coché**
- Grid Color: Gris transparent
- Boundary Color: Rouge transparent

**Paramètres Wall Settings:**
- Wall Prefab: *Laissez vide* (créés automatiquement)
- Wall Material: *Laissez vide*

✅ **Vérification:** Vous voyez une grille dans la Scene view

---

## 📋 ÉTAPE 11: Connecter les références dans GameManager

C'est l'étape la plus importante ! On connecte tout ensemble.

### A. Sélectionner GameManager

1. Cliquez sur **GameManager** dans Hierarchy

### B. Connecter les références

Dans Inspector, section **References** du script GameManager:

1. **Snake Controller:**
   - Glissez **SnakeHead** depuis Hierarchy vers ce champ

2. **Food Spawner:**
   - Glissez **FoodSpawner** depuis Hierarchy vers ce champ

3. **Grid Manager:**
   - Glissez **GridManager** depuis Hierarchy vers ce champ

✅ **Vérification:** Les 3 champs References doivent être remplis (pas "None")

---

## 📋 ÉTAPE 12: Hiérarchie finale

Votre Hierarchy devrait ressembler à ça:

```
Scene: SnakeGame
├── XR Origin (VR)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── Left Controller
│   │   └── Right Controller
│   └── ...
├── Directional Light (si présent)
├── GameManager [GameManager Script]
├── SnakeHead [SnakeController Script + Rigidbody + BoxCollider]
├── FoodSpawner [FoodSpawner Script]
├── VRInputManager [VRInputManager Script]
└── GridManager [GridManager Script]
```

---

## 📋 ÉTAPE 13: Sauvegarder !

1. **File > Save** (ou **Ctrl+S**)
2. Vérifiez qu'il n'y a pas d'erreurs dans la **Console** (en bas)

---

## 📋 ÉTAPE 14: Premier test dans l'éditeur

### A. Entrer en Play Mode

1. Cliquez sur le bouton **Play** ▶️ en haut de Unity
2. Le bouton devient bleu

### B. Tester les contrôles

**Depuis l'éditeur (sans casque):**
- Utilisez les touches **W, A, S, D** pour simuler le joystick

### C. Observer

- Le serpent devrait apparaître au centre
- Une nourriture rouge devrait apparaître quelque part
- Le serpent ne bouge pas encore (normal, il attend le bouton Start)

### D. Démarrer le jeu

Pour l'instant, sans UI, vous devez appeler StartGame manuellement:

1. En mode Play, dans Hierarchy, sélectionnez **GameManager**
2. Dans Inspector, trouvez le script **Game Manager**
3. Clic droit sur le nom du script → **StartGame()** (si visible)

OU plus simple pour tester:

1. Dans la **Console**, tapez:
   ```csharp
   GameManager.Instance.StartGame();
   ```

OU modifiez temporairement le GameManager:

### E. Sortir du Play Mode

Cliquez à nouveau sur **Play** ▶️ pour arrêter

⚠️ **Important:** Les changements faits en Play Mode NE SONT PAS SAUVEGARDÉS !

---

## 🎉 FÉLICITATIONS !

Votre scène est configurée ! Il ne reste qu'à:
1. Tester sur le Quest
2. Ajouter une UI pour le menu et le score
3. Améliorer le visuel

---

## 🐛 Si vous avez des erreurs

### "NullReferenceException"
- Vérifiez que TOUTES les références sont connectées dans GameManager
- Vérifiez que le prefab SnakeSegment est bien glissé dans SnakeHead

### "Missing script"
- Assets > Reimport All

### Le serpent ne se déplace pas
- Vérifiez que VRInputManager a la Main Camera connectée
- Vérifiez que le jeu est en état "Playing" (appuyez sur A dans le Quest ou appelez StartGame)

### Pas de nourriture
- Vérifiez que FoodSpawner est connecté dans GameManager
- Vérifiez que le tag "Food" existe

---

## 📝 Prochaine étape

Testez sur votre Meta Quest ! Suivez le guide de Build dans `SETUP_INSTRUCTIONS.md`.
