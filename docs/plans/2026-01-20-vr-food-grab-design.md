# VR Food Grab - Design Document

## Overview

Collecte de nourriture par geste VR : saisir la food avec la main et la ramener vers soi pour la manger.

## User Flow

1. Le joueur approche sa main (contrôleur) de la food
2. Il appuie sur le **grip** → la food s'attache à sa main
3. Tant qu'il tient le grip, la food suit sa main
4. **S'il relâche le grip** → la food est lâchée/lancée (avec vélocité de la main)
5. **S'il ramène la food près de sa tête** (~30cm) → la food est consommée, score +1, le snake grandit

La collision directe tête/food ne déclenche plus la collecte. Seul le geste main→bouche fonctionne.

## Architecture

### New Scripts

| Script | Location | Responsibility |
|--------|----------|----------------|
| `HandGrabber.cs` | `Assets/SnakeVR/Scripts/Interaction/` | Attaché à chaque contrôleur. Détecte la proximité avec la food, écoute le grip input, gère l'attachement |
| `GrabbableFood.cs` | `Assets/SnakeVR/Scripts/Interaction/` | Attaché à la food. Gère l'état (libre, tenue, consommée) et le comportement physique |

### Modified Scripts

| Script | Change |
|--------|--------|
| `FoodSpawner.cs` | Ajoute `GrabbableFood` + `Rigidbody` au prefab spawné |
| `SnakeController.cs` | Supprime la détection de collision avec la food |

### Unity Setup

| GameObject | Components |
|------------|------------|
| LeftHand Controller | `HandGrabber` + Sphere Collider (trigger, radius 0.15) |
| RightHand Controller | `HandGrabber` + Sphere Collider (trigger, radius 0.15) |
| Food Prefab | `GrabbableFood` + Rigidbody + existing Collider |

## Detailed Behavior

### HandGrabber.cs

- Sphere collider trigger (~15cm radius) pour détecter les foods à portée
- Écoute l'input grip via XR Interaction Toolkit (`XRI RightHand/Grip` ou LeftHand)
- Quand grip pressé + food à portée → appelle `food.Grab(this.transform)`
- Quand grip relâché → appelle `food.Release(controllerVelocity)`

### GrabbableFood.cs

**État Libre** :
- La food reste en place normalement
- Rigidbody permet les rebonds sur les murs

**État Tenue** :
- Désactive son propre collider (évite les collisions parasites)
- Se positionne dans la main (`transform.position = handTransform.position`) chaque frame
- Vérifie la distance avec `Camera.main.transform.position`
- Si distance < 0.3m → déclenche consommation

**Consommation** :
- Notifie le `SnakeController` pour grandir
- Joue un feedback (optionnel)
- Se détruit

**Release** :
- Réactive le collider
- Applique la vélocité de la main comme force de lancer
- La physique Unity gère les rebonds sur les murs du cube

## Edge Cases

| Situation | Behavior |
|-----------|----------|
| Deux mains attrapent la même food | Seule la première main qui grip l'attrape |
| Food lâchée | Rebondit sur les murs, reste dans le cube, peut être re-attrapée |
| Joueur meurt en tenant une food | La food est détruite avec le game over |

## Feedback (Optional for MVP)

| Action | Feedback |
|--------|----------|
| Food à portée de main | Léger changement de couleur ou outline |
| Food saisie | Son de "grab" |
| Food consommée | Son de manger + particules |
| Food lâchée | Son de "whoosh" |

## Confinement

La food reste confinée dans le cube de jeu via la physique Unity : Rigidbody sur la food + colliders sur les murs = rebond automatique.
