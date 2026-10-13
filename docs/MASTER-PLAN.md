# One More Floor V1 Master Plan

## Current product contract — October13

The approved October10 lift board and FLOOR-LIFT-TO-TOWER-20261012.md supersede the original joystick/orbit/neon platformer tasks below. Current gameplay is a single vertical elevator to fixed tower balconies, one-thumb timing, fixed camera, checkpoint recovery and exactly-once banking. ART-DIRECTION.md describes the active target. Historical P02/P03/P06 entries are not instructions to restore the old mechanic. TestFlight remains blocked until exact-source actual Concept similarity and native validation pass.

Latest actual native graphics evidence: [Coordinated final polish](FLOOR-POLISH-20261013.md). User now explicitly accepts approximately90% visual resemblance; literal pixel identity is no longer required. Actual native renders pass the subjective approximation review, with simpler material/model detail disclosed. Native iPhone and platform/provider checks remain required before distribution. Original reflections, menu-assets and proportion receipts remain historical source-labelled evidence.

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


## 2026-10-13 — rectangular world cards and slender same-height carriage

User's concrete corrections applied together: width-derived2.1portrait world cards, matching slender imported/procedural cabin/guides/visible floor/fittings, supported original walk-out tread and smaller menu-only passenger. Actual original collision/timing/trajectory/gameplay actor/fixed view preserved. Native defect REDs, strict validation/build and fresh native player/GPU/proportion comparisons in QA/floor-proportions-20261013.json and FLOOR-PROPORTIONS-20261013.md. Requested proportions verified; full Concept quality gate still FAILED.

## 2026-10-13 — coordinated final polish under approximate target

User accepts previous proportions as near target and requests a little refinement, explicitly replacing literal100%1:1 with an approximate90% visual direction. Actual serif wordmark/card spacing/navigation, softer marked marble with shared-room reflection, and restrained hair highlights are improved together. Native pitch12 floor-ray regression restored to17 and recorded; no weakened checks. Corrected manual/build validation14,711/29,423cumulative zero errors, native Mac build succeeds; fresh Showcase2,437checks and actual GPU marble A/B/no-facade-leak pass. Actual comparisons and exact-source receipt QA/floor-polish-20261013.json; visual gate passes this subjective approximation, never a numeric similarity measurement. Physical iPhone/release/provider checks unrun, no TestFlight upload.
