# Exemple de Workflow Complet - Snake VR

## Scénario: Créer la scène à l'école, continuer chez soi

### 🏫 À L'ÉCOLE - Session 1 (2 heures)

#### Dans Unity:
1. Ouvrir le projet
2. File > New Scene
3. Sauvegarder: `Assets/SnakeVR/Scenes/SnakeGame.unity`
4. Créer tous les GameObjects:
   - XR Origin
   - GameManager
   - SnakeHead
   - FoodSpawner
   - VRInputManager
   - GridManager
5. Connecter les références dans l'Inspector
6. Créer le prefab SnakeSegment
7. **File > Save (Ctrl+S)**
8. Tester dans l'éditeur

#### Dans Git Bash / Terminal:
```bash
# Voir ce qui a changé
git status

# Résultat:
# modified:   Assets/...
# new file:   Assets/SnakeVR/Scenes/SnakeGame.unity
# new file:   Assets/SnakeVR/Scenes/SnakeGame.unity.meta
# new file:   Assets/SnakeVR/Prefabs/SnakeSegment.prefab
# new file:   Assets/SnakeVR/Prefabs/SnakeSegment.prefab.meta

# Committer TOUT
git add Assets/ ProjectSettings/
git commit -m "Create SnakeGame scene with complete setup

- XR Origin configured for Quest
- All game managers instantiated
- Snake prefab created
- References connected
- Ready for testing"

git push
```

**Résultat:** Tout est sur GitHub ! 🚀

---

### 🏠 CHEZ VOUS - Session 2 (même jour ou plus tard)

#### Récupérer le projet:

**Première fois:**
```bash
# Terminal / Git Bash
cd Documents/Unity
git clone https://github.com/VOTRE_USERNAME/snake-vr.git
cd snake-vr

# Ouvrir Unity Hub
# Add > Sélectionner le dossier snake-vr
# Attendre 5-10 min (Library/ se régénère)
```

**Les fois suivantes:**
```bash
cd snake-vr
git pull
# Ouvrir dans Unity Hub
```

#### Dans Unity:

1. **Project** panel > `Assets/SnakeVR/Scenes/`
2. Double-cliquer sur **SnakeGame.unity**
3. **TOUT est là !**
   - Tous les GameObjects
   - Toutes les références connectées
   - Tous les prefabs
   - Même configuration qu'à l'école

4. Continuer le travail:
   - Ajouter des matériaux
   - Créer l'UI
   - Ajouter des effets sonores

5. **File > Save**

#### Dans Git:
```bash
git add Assets/
git commit -m "Add materials and UI to SnakeGame scene"
git push
```

---

### 🏫 RETOUR À L'ÉCOLE - Session 3

```bash
git pull
# Ouvrir Unity
# Tous vos changements de chez vous sont là !
```

---

## 📊 Visualisation du transfert

### Ce qui est transféré par Git:

```
École (après commit + push)
└── GitHub (repository distant)
    └── Chez vous (après clone/pull)

✅ Scènes (.unity)
✅ Prefabs (.prefab)
✅ Scripts (.cs)
✅ Materials (.mat)
✅ Textures (.png, .jpg)
✅ Audio (.mp3, .wav)
✅ Configurations (ProjectSettings/)
✅ Packages (manifest.json)
```

### Ce qui N'est PAS transféré (régénéré automatiquement):

```
❌ Library/ (cache Unity)
❌ Temp/ (fichiers temporaires)
❌ .csproj (fichiers de solution)
❌ Logs/
```

---

## 🔍 Vérification que tout est bien transféré

### Avant de quitter l'école:

```bash
# Vérifier qu'il n'y a rien d'oublié
git status

# Doit afficher:
# On branch master
# nothing to commit, working tree clean

# Vérifier que c'est bien sur GitHub
git log --oneline -5

# Vérifier le dernier push
git remote -v
```

### En arrivant chez vous:

```bash
# Vérifier que vous êtes à jour
git pull

# Doit afficher:
# Already up to date.
# OU télécharger les nouveaux fichiers
```

---

## 🎯 Checklist pour chaque session

### Début de session:
- [ ] `git pull` (récupérer les derniers changements)
- [ ] Ouvrir Unity
- [ ] Vérifier que la scène s'ouvre sans erreurs

### Pendant la session:
- [ ] Sauvegarder régulièrement (Ctrl+S)
- [ ] Tester souvent

### Fin de session:
- [ ] File > Save pour tout sauvegarder
- [ ] Fermer Unity (important !)
- [ ] `git status` (voir ce qui a changé)
- [ ] `git add Assets/ ProjectSettings/`
- [ ] `git commit -m "Description claire"`
- [ ] `git push`
- [ ] Vérifier sur GitHub que le commit est là

---

## 🐛 Troubleshooting

### "Merge conflict" dans un fichier .unity:

```bash
# Si deux personnes modifient la même scène simultanément
# (ou si vous avez oublié de pull)

# Solution 1: Garder votre version
git checkout --ours Assets/SnakeVR/Scenes/SnakeGame.unity
git add Assets/SnakeVR/Scenes/SnakeGame.unity
git commit -m "Resolved merge conflict - kept local changes"

# Solution 2: Garder la version distante
git checkout --theirs Assets/SnakeVR/Scenes/SnakeGame.unity
git add Assets/SnakeVR/Scenes/SnakeGame.unity
git commit -m "Resolved merge conflict - kept remote changes"
```

**Meilleure solution:** Toujours `git pull` AVANT de commencer à travailler !

### La scène ne s'ouvre pas:

```
# Dans Unity:
Assets > Reimport All
# Puis réouvrir la scène
```

### Des GameObjects manquent dans la scène:

- Vérifier que vous avez bien fait `git pull`
- Vérifier que tous les fichiers .meta sont présents
- Assets > Reimport All

---

## 💡 Bonnes pratiques

1. **Toujours pull avant de travailler**
   ```bash
   git pull
   ```

2. **Commit souvent** (toutes les 30 min - 1h)
   ```bash
   git add Assets/
   git commit -m "Add snake movement logic"
   ```

3. **Push à la fin de chaque session**
   ```bash
   git push
   ```

4. **Fermer Unity avant de push** (évite les problèmes de lock)

5. **Messages de commit descriptifs**
   ```bash
   ✅ "Add food spawning system with grid-based positioning"
   ❌ "update"
   ```

6. **Tester avant de commit**
   - Vérifier qu'il n'y a pas d'erreurs dans la Console
   - Tester que le jeu fonctionne

---

## 🎓 Résumé ultra-simple

### Les scènes Unity = fichiers texte = Git les gère automatiquement !

**Vous créez dans Unity → Vous sauvegardez → Vous commit → C'est sur GitHub → Vous pull ailleurs → C'est là !**

Aucune étape spéciale pour les scènes. Elles se comportent comme n'importe quel autre fichier de code.
