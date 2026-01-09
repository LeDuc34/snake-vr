# Snake VR - Unity Project

Jeu Snake en réalité virtuelle pour Meta Quest.

## 🚀 Démarrage rapide

### Cloner et ouvrir le projet

```bash
# Cloner
git clone https://github.com/VOTRE_USERNAME/snake-vr.git
cd snake-vr

# Ouvrir dans Unity Hub (version 6000.3.1f1 recommandée)
# Unity va automatiquement:
# - Régénérer Library/ (5-10 min la première fois)
# - Télécharger les packages XR Toolkit
# - Compiler les scripts
```

### Configuration

Lisez la documentation complète dans l'ordre:
1. **`SNAKE_VR_GUIDE.md`** - Vue d'ensemble et workflow Git
2. **`Assets/SnakeVR/SETUP_INSTRUCTIONS.md`** - Configuration Unity pas-à-pas
3. **`Assets/SnakeVR/README.md`** - Architecture du code

## 🔄 Workflow Git

```bash
# Avant de travailler
git pull

# Après avoir travaillé
git add Assets/ ProjectSettings/ Packages/
git commit -m "Description"
git push
```

## 📦 Prérequis

- Unity 6000.3.1f1 (ou Unity 6.0.x)
- Git
- Meta Quest avec mode développeur (pour tester)

## 📝 Structure

```
Assets/SnakeVR/
├── Scripts/           - Code C# du jeu
├── Scenes/            - Scènes Unity
├── Prefabs/           - Objets réutilisables
└── SETUP_INSTRUCTIONS.md
```

## ⚠️ Important

- Les dossiers `Library/`, `Temp/`, `Logs/` sont régénérés automatiquement
- Ne committez JAMAIS ces dossiers (déjà dans .gitignore)
- Les fichiers `.meta` DOIVENT être versionnés

## 🐛 Troubleshooting

**Erreurs de packages:**
```
Window > Package Manager > Refresh
```

**Erreurs de compilation:**
```
Assets > Reimport All
```

---

Pour plus de détails, consultez `SNAKE_VR_GUIDE.md`
