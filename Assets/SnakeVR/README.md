# Snake VR

Jeu Snake en réalité virtuelle pour Meta Quest développé avec Unity 6 et XR Interaction Toolkit.

## 📁 Structure des fichiers

```
Assets/SnakeVR/
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs          - Gestion d'état et score
│   │   └── VRInputManager.cs       - Contrôles VR Quest
│   ├── Snake/
│   │   ├── SnakeController.cs      - Mouvement et logique du serpent
│   │   └── SnakeSegment.cs         - Segments du corps
│   ├── Food/
│   │   └── FoodSpawner.cs          - Apparition de nourriture
│   └── Game/
│       └── GridManager.cs          - Zone de jeu et limites
├── Prefabs/                        - Objets réutilisables
├── Materials/                      - Matériaux 3D
├── Scenes/                         - Scènes Unity
└── Audio/                          - Sons et musiques
```

## 🚀 Démarrage rapide

### Prérequis
- Unity 6.0 ou supérieur
- XR Interaction Toolkit installé
- Meta Quest avec mode développeur activé

### Installation

1. **Ouvrir le projet dans Unity Hub**

2. **Suivre les instructions de configuration:**
   - Lisez `SETUP_INSTRUCTIONS.md` pour la configuration complète pas à pas

3. **Configurer pour Quest:**
   - File > Build Settings > Android
   - Switch Platform
   - Player Settings > XR Plug-in Management > Oculus ✓

4. **Créer votre première scène:**
   - Suivez les étapes dans `SETUP_INSTRUCTIONS.md`

## 🎮 Comment jouer

- **Joystick gauche/droit**: Diriger le serpent
- **Bouton A**: Démarrer le jeu
- **Bouton Menu**: Pause/Reprendre

## 🏗️ Architecture du code

### GameManager
Singleton qui gère:
- États du jeu (Menu, Playing, Paused, GameOver)
- Score
- Vitesse du jeu
- Événements globaux

### SnakeController
Contrôle le serpent:
- Mouvement basé sur grille
- Croissance dynamique
- Détection de collisions

### VRInputManager
Gère les inputs VR:
- 4 schémas de contrôle disponibles
- Support complet Meta Quest
- Fallback pour l'éditeur Unity

### FoodSpawner
Gère la nourriture:
- Spawn aléatoire dans la grille
- Évite les collisions avec le serpent

### GridManager
Définit l'espace de jeu:
- 3 types de limites (Walls, Wraparound, None)
- Visualisation dans l'éditeur
- Génération de murs

## 🔧 Paramètres ajustables

Dans Unity Inspector, vous pouvez modifier:

**GameManager:**
- Vitesse initiale
- Augmentation de vitesse par nourriture
- Points par nourriture

**SnakeController:**
- Espacement des segments
- Nombre de segments initiaux
- Vitesse de rotation

**VRInputManager:**
- Schéma de contrôle (Joystick/Gaze/Controller)
- Deadzone du joystick

**GridManager:**
- Taille de la grille
- Type de limites
- Visualisation

## 🎨 Prochaines fonctionnalités

- [ ] UI VR pour le score
- [ ] Menu principal immersif
- [ ] Effets sonores
- [ ] Effets de particules
- [ ] Power-ups
- [ ] Niveaux de difficulté
- [ ] Obstacles dynamiques
- [ ] Mode multijoueur

## 📚 Ressources

- [Documentation complète](../../SNAKE_VR_GUIDE.md)
- [Instructions de setup](SETUP_INSTRUCTIONS.md)
- [Unity XR Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest)

## 🐛 Bugs connus

Aucun pour le moment. Si vous en trouvez, documentez-les !

## 📝 Notes de développement

### Optimisation Quest
- Viser 72 FPS minimum
- Limiter les polygones
- Utiliser l'occlusion culling

### Git
N'oubliez pas de commit régulièrement:
```bash
git add Assets/ ProjectSettings/
git commit -m "Description des changements"
```

---

**Bon développement ! 🎮**
