# One More Floor V1 Master Plan

## Current product contract — October13

The approved October10 lift board and FLOOR-LIFT-TO-TOWER-20261012.md supersede the original joystick/orbit/neon platformer tasks below. Current gameplay is a single vertical elevator to fixed tower balconies, one-thumb timing, fixed camera, checkpoint recovery and exactly-once banking. ART-DIRECTION.md describes the active target. Historical P02/P03/P06 entries are not instructions to restore the old mechanic. TestFlight remains blocked until exact-source actual Concept similarity and native validation pass.

Latest actual native graphics evidence: [Original room reflections and composition](FLOOR-REFLECTION-COMPOSITION-20261013.md). The next authorized pass addresses world cards, typography and coin graphics against the same unchanged board; overall Concept acceptance still fails.

## Objective
Ship a responsive native mobile platformer whose core promise is continuous upward progress with instant checkpoint recovery.

## P00 Product lock
Core loop, checkpoint rules, failure rules, visual identity, progression and monetization.

## P01 Unity mobile foundation
Project structure, identifiers, portrait, safe areas, 60 FPS target and build setup.

## P02 Character/camera
Touch movement, jump, keyboard fallback, player-controlled orbit camera and collision tuning.

## P03 Floor system
30-stage runs, stage patterns, moving platforms, hazards, deterministic layout and checkpoint placement.

## P04 Recovery
Immediate fall detection, latest-checkpoint recovery, Continue/Retry and explicit full-run restart only.

## P05 Score/progression
Score, coins, streak/performance grades, best stage and tower completion.

## P06 Production visual world
Neon dusk city, readable platform edges, checkpoint rings, skyline depth and optimized lighting.

## P07 UI/UX
Top HUD, checkpoint panel, settings, pause, DE/EN and accessibility.

## P08 Audio/VFX/haptics
Jump, checkpoint, fail/recover, tower complete, haptics and reduced-motion support.

## P09 Persistence
Versioned profile, best stage, coins, tower unlocks, settings and migration.

## P10 Retention/monetization
Daily challenge, cosmetics, optional convenience, StoreKit/Play Billing and restore flow.

## P11 QA/release
Rules tests, device matrix, performance, store validation and staged release.

## Definition of Done
- A fall never sends a normal run back to the bottom.
- Recovery is visually complete in under 0.5 seconds.
- Camera never steals yaw from the player.
- Next actionable platform stays visible.
- 60 FPS target on supported devices.
- iOS and Android store builds validated.


## 2026-10-13 — menu artwork, lettering and currency

Original scenic world-card images and alpha gold coin are integrated into actual live One More Floor UI, together with native sliced gold/clip/frame sprites, properly cased derived serif titles/logo, readable heavy sans controls and retained live star/progression/Shop/parent Back flow. Actual player draw-callback and caret mesh regressions were fixed and preserved as meaningful REDs. Final4 clean strict native build/player/GPU evidence and unchanged-board comparisons: QA/floor-menu-assets-20261013.json and FLOOR-MENU-ASSETS-20261013.md. Overall graphical gate remains FAILED; actor/cabin/stone/full-atrium quality remains open. No TestFlight or other-game/provider work.
