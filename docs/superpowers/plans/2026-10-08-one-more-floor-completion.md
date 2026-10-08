# One More Floor Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans inline task-by-task. Steps use checkbox syntax.

**Goal:** Finish concept-faithful30floor native arcade game to verified internal delivery and full source backup.
**Architecture:** Preserve controller/camera; pure persisted FloorRules, FloorCourse orchestrates static checkpoint and moving platform, modular visuals/HUD/feedback; isolated development QA.
**Tech Stack:** Unity6000.6.4f1 Built-in C#/UGUI2.6 pinned IAP5.4.4/GMA11.5/EDM1.2.187.
**Spec:** docs/superpowers/specs/2026-10-08-one-more-floor-completion.md

## Global Constraints
- Main canonical, com.kamilunavo.onemorefloor/ASC6819872214, no public release.
- Floors1..30/index0..29; CPindex4/9/14/19/24/29 stationary; fall to latest checkpoint.
- Reward frontier survives retry/fall/reload; explicit new run resets frontier.
- DE/EN,48logicalpoints, free yaw, test-ad mode/real-commerce gates explicit.

## Review Focus
- Replaying rewarded floor after fall/relaunch must not mint coins or score (Task1).
- Pausing on a moving floor freezes clock and retains support; resume has no accumulated carry delta (Task2).
- Moving edge landing cannot become a CP outside platform or cause startup fall (Task2/3).
- Transparent root/text does not steal camera but modal notes/gaps start scrolling (Task2).
- UTC negative-offset consecutive daily and unknown-schema economy remain safe (Task1).

### Task1 persisted floor rules
Create Core/FloorProfile.cs,FloorRules.cs,FloorSave.cs; Editor/FloorValidation.cs; Gameplay/CoursePatterns.cs. Interfaces FloorRules.Land(FloorProfile,int,bool)->bool,Recover(FloorProfile)->void,Complete(FloorProfile,DateTime)->int,ClaimDaily(...)->bool; CoursePatterns.Points(int,int)->Vector3[30].
- [ ] Write/run missing-type RED rules for29ordered landings,checkpoint5,fall/replay frontier,score/coins/perfects idempotence,stars/unlocks/dailyUTC16perfect reward/normalization/unknownschema/save/reachability.
- [ ] Implement exact spec rules and save, GREEN bundledMono/editor, commit.

### Task2 actual world/input/UI
Modify FloorCourse/PlayerMotor/MovingPlatform/GameBootstrap; adopt own verified input/camera/UI helpers and native plugins; create Visuals/CityArt,PlatformArt,RunnerArt,FloorFeedback,FloorBloom; UI/FloorHud; own Resources assets. Interfaces FloorCourse.Profile/Height/Steps/Paused/SafePosition/StartRun/Land/Respawn/CompletePortal/CheckpointReached,Continue,RetryCheckpoint; MovingPlatform.Tick(float)/Delta; motor.ResetMotion/Paused.
- [ ] Actual fixtures RED→GREEN for moving clock/delta/pause/CCcarry,raycast/joystick/stalejump/min49points/viewport/mesh lifetime.
- [ ] Generate/inspect/bind sunset panorama,industrial atlas,icon; actual authored geometry/runner/city/audio/VFX.
- [ ] Integrate checkpoint overlay/freecamera,DEEN/settings/daily/styles/store hooks and persist callbacks. Compile/run actualMac29jump routes/replay/fall/CP/movingcarry; inspect screenshots and commit.

### Task3 native delivery/backup
Modify Editor/BuildAutomation,ProjectBootstrap,CommerceBuildHooks/imports; QA/FloorRuntimeQa actualinputs/isolatedprofiles. Interface -qaFloor/-qaFloorSoak,BuildMacPreview/BuildIOSSimulatorQa/BuildIOS/BuildAndroidRelease; shipping excludes QA.
- [ ] Own catalogs/appIDs optionalcommerce rule regressions and realSDKbuild; retain genuine sandbox/consent gates.
- [ ] Native compact/iPad/orientations/officialDuo available genuine pose and30minute soak.
- [ ] One fresh whole-change reviewer,fix justified findings with regressions.
- [ ] Internal signed iOS/TestFlight tester and central-key Android; verify signatures/manifests/payloads.
- [ ] Commit/push allUnityinputs,remote tree verification,retain finals/evidence,delete regenerable caches,continue Repair Empire.
