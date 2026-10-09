# Elevator Timing and Risk Implementation Plan

> **For agentic workers:** Use superpowers:executing-plans to implement task-by-task. The user explicitly authorized immediate inline execution on 2026-10-09.

**Goal:** Replace free parkour with a fixed-camera, one-thumb 30-floor elevator timing game.
**Architecture:** Keep FloorCourse/profile/commerce as owners. Add deterministic ElevatorRules and guided transfers; reuse authored decks, runner, city and menus. Preserve legacy rule tests as compatibility coverage.
**Tech Stack:** Unity6000.6.4f1, C#, uGUI, existing IAP/AdMob adapters.
**Spec:** docs/superpowers/specs/2026-10-09-elevator-timing-risk-design.md

## Global Constraints
- 30 floors, checkpoints every five, fall to last checkpoint.
- One timing action; no joystick/orbit; fixed camera rotation.
- Rewards bank exactly once; falls lose only pending round coins.
- Compatible wallet, purchases, designs and settings; DE/EN, >=48point targets.
- Internal delivery only; retain native proof and source before cleaning generated caches.

## Review Focus
- Duplicate banking/landing after reload must never mint rewards.
- Pause/background/purchase dialogs freeze clock and discard held input.
- Early or late launch must fail based on actual arrival, not target homing.
- Portrait compact and landscape must show both decks and trajectory outside HUD.
- Legacy profile conversion preserves entitlements and unknown-schema protections.

### Task1: Deterministic rules and compatible persistence
**Files:** Create Assets/Scripts/Core/ElevatorRules.cs, Assets/Editor/ElevatorValidation.cs; modify FloorProfile.cs.
**Interfaces:** MotionX(int floor,int realm,int seed,float clock), HalfBay(int floor), Land(FloorProfile,int,bool), Recover(FloorProfile), Bank(FloorProfile), Migrate(FloorProfile), NewRun(FloorProfile).
- [ ] Write tests for centered/edge/miss, clocks/tower variation, all30floors, perfect/replay/CP recovery, bank twice and reload, legacy migration.
- [ ] Run tests RED, implement rules, run GREEN with Unity Editor validation.
- [ ] Commit rules and tests.

### Task2: Runtime, art, camera and touch UI
**Files:** Modify FloorCourse.cs, PlayerMotor.cs, OrbitCamera.cs, FloorHud.cs, GameBootstrap.cs; add ElevatorMotion.cs and ElevatorFlightPreview.cs.
**Interfaces:** FloorCourse.LaunchTransfer(), IsTransferring, FlightVelocity, TimingActive; same Changed/checkpoint/landing/commerce hooks.
- [ ] Add runtime QA for full routes, misses, clock freeze, repeated input and actual camera quaternion/framing.
- [ ] Integrate guided arc evaluated against moving bay at actual arrival; carry grounded runner on source; freeze paused state.
- [ ] Fixed camera frames source, target and arc inside safe gameplay pane; hide parkour controls, wide centered action and clear pending reward/risk menus.
- [ ] Run Editor compile/commerce/rules suite; build desktop and run actual player QA with portrait/landscape screenshots.
- [ ] Commit complete gameplay integration.

### Task3: Native validation and internal delivery
**Files:** QA harness, BuildAutomation.cs version metadata and delivery ledger.
- [ ] Native compact/16ProMax portrait+landscape routes, intentional misses, camera constancy, framing, reload and lifecycle.
- [ ] Fresh whole-change review, fix important findings with regression coverage.
- [ ] iOS archive/sign/export/internal upload, Android native/sign/payload proofs. Verify source remote and unpublished binary backup.
- [ ] Record exact tested evidence, external gates and retained artifacts; clean only regenerated caches after proof.
