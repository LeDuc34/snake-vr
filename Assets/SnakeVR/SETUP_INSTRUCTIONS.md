# Snake VR - Instructions de Configuration Unity

## 🎯 Configuration dans Unity (ÉTAPE PAR ÉTAPE)

### 1. Créer la scène de jeu

1. Dans Unity, allez dans **File > New Scene**
2. Sauvegardez la scène: **File > Save As**
3. Naviguez vers `Assets/SnakeVR/Scenes/`
4. Nommez la scène: `SnakeGame.unity`

### 2. Configurer le XR Origin

1. **Supprimer la Main Camera** par défaut (clic droit > Delete)

2. **Ajouter XR Origin:**
   - Clic droit dans Hierarchy > **XR > XR Origin (VR)**
   - Cela créé automatiquement:
     - XR Origin
     - Camera Offset
     - Main Camera
     - Left/Right Controllers

3. **Positionner le XR Origin:**
   - Sélectionnez XR Origin dans Hierarchy
   - Dans Inspector, Position: (0, 0, 0)

### 3. Créer les GameObjects principaux

#### A. GameManager

1. Clic droit dans Hierarchy > **Create Empty**
2. Nommez-le: `GameManager`
3. Dans Inspector, cliquez **Add Component**
4. Cherchez et ajoutez: `GameManager` (le script que nous avons créé)
5. Position: (0, 0, 0)

#### B. SnakeHead

1. Clic droit dans Hierarchy > **3D Object > Cube**
2. Nommez-le: `SnakeHead`
3. Dans Inspector:
   - Transform:
     - Position: (0, 1.5, 0)
     - Scale: (0.3, 0.3, 0.3)
   - Ajoutez un **Rigidbody**:
     - Cliquez **Add Component** > **Rigidbody**
     - Décochez **Use Gravity**
     - Cochez **Is Kinematic**
   - Ajoutez un **Box Collider** (normalement déjà présent):
     - Cochez **Is Trigger**
   - Ajoutez le script **SnakeController**:
     - Cliquez **Add Component**
     - Cherchez `SnakeController`
   - Changez la couleur:
     - Dans Materials, cliquez sur le material
     - Changez Albedo vers une couleur verte

#### C. Segment Prefab (pour le corps du serpent)

1. Clic droit dans Hierarchy > **3D Object > Cube**
2. Nommez-le: `SnakeSegment`
3. Dans Inspector:
   - Transform > Scale: (0.25, 0.25, 0.25)
   - Ajoutez **Box Collider**:
     - Cochez **Is Trigger**
   - Ajoutez le script **SnakeSegment**
   - Changez la couleur (vert plus clair que la tête)
4. **Créer le Prefab:**
   - Glissez `SnakeSegment` depuis Hierarchy vers `Assets/SnakeVR/Prefabs/`
   - Supprimez le `SnakeSegment` de la Hierarchy (on en a plus besoin)

#### D. FoodSpawner

1. Clic droit dans Hierarchy > **Create Empty**
2. Nommez-le: `FoodSpawner`
3. Position: (0, 1.5, 0)
4. Ajoutez le script **FoodSpawner**

#### E. VRInputManager

1. Clic droit dans Hierarchy > **Create Empty**
2. Nommez-le: `VRInputManager`
3. Position: (0, 0, 0)
4. Ajoutez le script **VRInputManager**

#### F. GridManager

1. Clic droit dans Hierarchy > **Create Empty**
2. Nommez-le: `GridManager`
3. Position: (0, 1.5, 0)
4. Ajoutez le script **GridManager**

### 4. Connecter les références

#### Dans GameManager:

1. Sélectionnez `GameManager` dans Hierarchy
2. Dans Inspector, section GameManager script:
   - **Snake Controller**: Glissez `SnakeHead` depuis Hierarchy
   - **Food Spawner**: Glissez `FoodSpawner` depuis Hierarchy
   - **Grid Manager**: Glissez `GridManager` depuis Hierarchy
3. Ajustez les paramètres si nécessaire:
   - Initial Speed: 2
   - Speed Increase Per Food: 0.1
   - Score Per Food: 10

#### Dans SnakeController (SnakeHead):

1. Sélectionnez `SnakeHead` dans Hierarchy
2. Dans Inspector, section SnakeController:
   - **Segment Prefab**: Glissez le prefab `SnakeSegment` depuis `Assets/SnakeVR/Prefabs/`
   - **Segment Spacing**: 0.3
   - **Initial Segment Count**: 3
   - **Head Transform**: Laissez vide (il va auto-détecter)
   - **Move Speed**: 2
   - **Turn Speed**: 90

#### Dans VRInputManager:

1. Sélectionnez `VRInputManager` dans Hierarchy
2. Dans Inspector:
   - **Control Scheme**: Choisissez `LeftJoystick` (le plus simple)
   - **Joystick Deadzone**: 0.3
   - **Camera Transform**: Glissez la `Main Camera` depuis XR Origin/Camera Offset/

### 5. Configurer les Tags

Unity utilise des tags pour identifier les objets:

1. **Tag "Food":**
   - Top menu: **Edit > Project Settings > Tags and Layers**
   - Cliquez le **+** sous Tags
   - Ajoutez: `Food`

2. **Tag "Wall":**
   - Ajoutez aussi: `Wall`

3. **Tag "SnakeBody":**
   - Ajoutez: `SnakeBody`

### 6. Configurer les Build Settings pour Quest

1. **File > Build Settings**
2. Cliquez **Android**
3. Cliquez **Switch Platform** (attendez que ça compile)
4. **Player Settings:**
   - **Company Name**: Votre nom
   - **Product Name**: Snake VR
   - Sous **XR Plug-in Management**:
     - Cochez **Oculus**
   - Sous **Other Settings**:
     - **Minimum API Level**: Android 10.0 (API Level 29)
     - **Target API Level**: Automatic (highest installed)

### 7. Premier test dans l'éditeur

Vous pouvez tester sans casque dans l'éditeur:

1. Appuyez sur **Play** en haut
2. Dans la fenêtre Game:
   - Utilisez **WASD** pour simuler le joystick
   - Ou activez **Device Simulator** (Window > Device Simulator)

### 8. Premier test sur Quest

1. **Connectez votre Quest en USB**
2. **Activez le mode développeur** sur le Quest:
   - Dans le casque: Settings > Developer > USB Connection Dialog > Allow
3. Dans Unity:
   - **File > Build Settings**
   - Cliquez **Refresh** à côté de "Run Device"
   - Sélectionnez votre Quest
   - Cliquez **Build And Run**
   - Choisissez un nom: `SnakeVR.apk`

## 🎨 Améliorer le visuel (optionnel)

### Ajouter un Skybox

1. **Window > Rendering > Lighting**
2. Sous **Environment**:
   - **Skybox Material**: Choisissez un skybox ou créez-en un

### Ajouter de la lumière

1. Sélectionnez **Directional Light** dans Hierarchy
2. Ajustez:
   - Rotation: (50, -30, 0)
   - Intensity: 1.5

### Ajouter un sol visuel

1. Clic droit Hierarchy > **3D Object > Plane**
2. Nommez: `Ground`
3. Position: (0, 0, 0)
4. Scale: (1, 1, 1)
5. Ajoutez une texture ou couleur

## 🎮 Contrôles dans le jeu

### Sur Meta Quest:

- **Joystick gauche**: Diriger le serpent
- **Bouton Menu**: Pause
- **Bouton A**: Démarrer le jeu (depuis le menu)

### Dans l'éditeur Unity:

- **WASD**: Simuler le joystick
- **Esc**: Pause

## 🐛 Troubleshooting

### Le serpent ne bouge pas:
- Vérifiez que GameManager est bien configuré
- Vérifiez que le jeu est en état "Playing" (appuyez sur A)
- Vérifiez les références dans SnakeController

### Pas de nourriture:
- Vérifiez que FoodSpawner est connecté dans GameManager
- Vérifiez le tag "Food" existe

### Les contrôleurs VR ne fonctionnent pas:
- Vérifiez XR Origin est bien configuré
- Vérifiez Camera Transform dans VRInputManager
- Vérifiez Oculus est activé dans XR Plug-in Management

### Erreurs de compilation:
- Vérifiez que tous les scripts sont dans les bons dossiers
- **Assets > Reimport All**

## 📝 Prochaines étapes

Une fois que tout fonctionne:

1. Ajoutez une UI VR pour afficher le score
2. Créez un menu principal en VR
3. Ajoutez des effets sonores
4. Ajoutez des particules quand on mange
5. Créez différents niveaux
6. Ajoutez des power-ups

## 💾 N'oubliez pas de sauvegarder !

```bash
git add Assets/ ProjectSettings/
git commit -m "Snake VR: Scene de base configurée"
git push
```

---

**Vous êtes prêt ! Lancez Unity et suivez ces étapes une par une.**
