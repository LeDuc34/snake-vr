# Snake VR - Guide de Développement

## 🎮 Vue d'ensemble du projet
Jeu Snake en réalité virtuelle pour Meta Quest utilisant Unity 6 et XR Interaction Toolkit.

## 📁 Structure du projet

### Où travailler ?
**IMPORTANT**: Tout votre code et assets vont dans le dossier `Assets/`

```
VR test/                          <- Racine du projet (vous êtes ici)
├── Assets/                       <- VOTRE ZONE DE TRAVAIL
│   ├── SnakeVR/                 <- Dossier principal du jeu
│   │   ├── Scenes/              <- Scènes Unity (.unity)
│   │   ├── Scripts/             <- Code C#
│   │   │   ├── Game/           <- Logique générale du jeu
│   │   │   ├── Snake/          <- Logique du serpent
│   │   │   ├── Food/           <- Logique de la nourriture
│   │   │   ├── UI/             <- Interface utilisateur VR
│   │   │   └── Managers/       <- GameManager, ScoreManager, etc.
│   │   ├── Prefabs/            <- Objets réutilisables
│   │   ├── Materials/          <- Matériaux et shaders
│   │   └── Audio/              <- Sons et musiques
│   └── ...                      <- Autres assets Unity/XR
├── ProjectSettings/             <- Configuration Unity (À VERSIONNER)
├── Packages/                    <- Packages Unity (À VERSIONNER)
└── Library/                     <- IGNORÉ par Git (fichiers générés)
```

## 🔧 Workflow Git

### Configuration initiale (déjà fait)
- ✅ Git initialisé
- ✅ .gitignore configuré pour Unity

### Commandes essentielles

#### Premier commit
```bash
git add .
git commit -m "Initial commit: Unity VR project setup with XR Toolkit"
```

#### Workflow quotidien
```bash
# Avant de commencer à travailler (chez vous)
git pull

# Après avoir travaillé
git add Assets/ ProjectSettings/ Packages/
git commit -m "Description de vos changements"
git push

# Créer une sauvegarde locale
git branch sauvegarde-$(date +%Y%m%d)
```

#### Pour GitHub (recommandé)
```bash
# Créer un repo sur GitHub, puis:
git remote add origin https://github.com/VOTRE_USERNAME/snake-vr.git
git branch -M main
git push -u origin main
```

## 🎯 Architecture du jeu Snake VR

### 1. Concept de base
- Le serpent se déplace dans un espace 3D
- Le joueur contrôle la direction avec les contrôleurs VR
- La nourriture apparaît aléatoirement dans l'espace
- Le serpent grandit quand il mange

### 2. Composants principaux à créer

#### A. GameManager (Assets/SnakeVR/Scripts/Managers/GameManager.cs)
- Gère l'état du jeu (menu, jeu, pause, game over)
- Initialise les systèmes
- Gère le score

#### B. SnakeController (Assets/SnakeVR/Scripts/Snake/SnakeController.cs)
- Mouvement du serpent
- Croissance du serpent
- Détection de collision

#### C. SnakeSegment (Assets/SnakeVR/Scripts/Snake/SnakeSegment.cs)
- Représente un segment du corps
- Suit le segment précédent

#### D. FoodSpawner (Assets/SnakeVR/Scripts/Food/FoodSpawner.cs)
- Fait apparaître la nourriture
- Gère le respawn après consommation

#### E. VRInputManager (Assets/SnakeVR/Scripts/Managers/VRInputManager.cs)
- Capture les inputs des contrôleurs Meta Quest
- Traduit les inputs en directions

#### F. GridManager (Assets/SnakeVR/Scripts/Game/GridManager.cs)
- Définit la zone de jeu
- Gère les limites (murs ou wraparound)

### 3. Système de contrôle VR

**Options de contrôle possibles:**

**Option A: Joystick directionnel**
- Joystick gauche/droit pour changer de direction
- Simple et intuitif

**Option B: Pointage avec le contrôleur**
- Pointer la direction souhaitée
- Plus immersif mais plus complexe

**Option C: Rotation de la tête**
- Le serpent suit la direction du regard
- Très immersif

### 4. Étapes de développement recommandées

**Phase 1: Prototype de base (2-3 sessions)**
1. Créer une grille 3D visible
2. Créer le serpent avec 1 segment
3. Implémenter le mouvement de base
4. Ajouter les inputs VR basiques

**Phase 2: Gameplay core (3-4 sessions)**
1. Système de croissance du serpent
2. Spawn de nourriture
3. Détection de collision (manger/game over)
4. Système de score

**Phase 3: Polish VR (2-3 sessions)**
1. Interface UI en VR (score, menu)
2. Effets visuels et audio
3. Optimisation pour Quest
4. Ajustement des contrôles

**Phase 4: Features avancées (optionnel)**
1. Niveaux de difficulté
2. Power-ups
3. Obstacles
4. Mode multijoueur

## 🎨 Conseils spécifiques VR

### Performance sur Meta Quest
- Viser 72 FPS minimum (idéal: 90 FPS)
- Limiter le nombre de polygones
- Utiliser l'occlusion culling
- Optimiser les materials (utiliser Standard Shader simplifié)

### Design VR
- Éviter les mouvements brusques (nausée)
- Utiliser des repères visuels fixes
- Tester régulièrement dans le casque
- Prévoir une zone de confort (3x3m recommandé)

### Scale et distance
- Un segment de serpent: environ 0.3m x 0.3m x 0.3m
- Zone de jeu: 5m x 5m x 5m (ajustable)
- Nourriture: 0.2m de diamètre

## 🔄 Workflow de développement

### 1. Ouvrir le projet
- Lancer Unity Hub
- Ouvrir "VR test"
- Scène principale: Assets/SnakeVR/Scenes/

### 2. Tester sur Quest
- Connecter le Quest en USB
- Build Settings > Android
- Build And Run

### 3. Itérer
- Faire des petits changements
- Tester immédiatement
- Commit régulièrement

## 📝 Prochaines étapes

1. Créer la première scène de jeu
2. Configurer le XR Rig pour le Snake
3. Créer le script GameManager
4. Implémenter le mouvement de base du serpent

## 🆘 Ressources utiles

- Unity XR Toolkit: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest
- Meta Quest Development: https://developer.oculus.com/documentation/unity/
- Git pour Unity: https://thoughtbot.com/blog/how-to-git-with-unity

## 📌 Notes importantes

- Ne JAMAIS versionner les dossiers: Library/, Temp/, Obj/, Build/
- Toujours tester dans le casque avant de valider
- Faire des commits atomiques (une fonctionnalité = un commit)
- Documenter les contrôles VR pour ne pas oublier

---

**Prêt à coder ? Commencez par créer votre première scène Snake VR !**
