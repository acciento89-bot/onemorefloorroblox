# Unity Project Context

<!-- unity-onboarding:generated:start -->
Analyzed2026-10-08, source4bcc71afaf5cf067cb42448c2e53190bb1544b2d. Root `/Users/piotrkaminski/Developer/OneMoreFloorRoblox`; legacy folder name, game is native Unity.

## Confirmed environment
Unity6000.6.4f1/12bfff696524; Built-in render pipeline (GraphicsSettings/noSRP package, Standard materials); legacy InputManager0 plus UGUI pointer events. iOS/Android bundle `com.kamilunavo.onemorefloor`, minimumAndroid26/ARM64. No networking or installed Unity MCP provider found; filesystem/batch-editor/native build tools available, live Editor inspection unverified.

## Structure and startup
Assets/Scripts/{Core,Gameplay,Camera,Input,UI}; Assets/Editor bootstrap/build helpers; only Main.unity enabled in EditorBuildSettings. No first-party asmdef/tests/prefabs/custom shaders detected. Main GameBootstrap constructs scene, CharacterController, camera, HUD andFloorCourse at runtime; generated-style C#Assembly-CSharp plusEditor boundary. NamespacesKamilunavo.OneMoreFloor; compact single-line methods/privateunderscore fields, MonoBehaviour-centric composition, no async/service architecture.

## Confirmed current behavior / gaps
FloorCourse builds30 primitive cube stages, ordered landing, checkpoint every5, immediate CC teleport on fall, Continue/Retry panel. Best/save/unlocks/daily/catalog are not implemented. No score replay guard: returning to checkpoint allows already traversed stages to mint rewards again. MovingPlatform animates horizontal sin with Time.time but motor does not carry platform delta; checkpoint stage15 can also move. Respawn does not reset vertical velocity. Modal/pause uses Time.timeScale without explicit input/camera ownership clearing; joystick has no finger ownership. Avatar is a visible capsule; skyline cubes/materials are prototypes. These are code observations, not yet reproduced runtime claims.

## Validation / tooling
UnityTestFramework1.8.0 installed, no game tests found. Existing BuildAutomation exports only Development iOS/APK; lacks releaseAAB/simulator/archive/version arguments. Historical ledger records deviceDebug and AndroidAPK builds; no current production validation established. Native hardware/thermal/sandbox gates unknown. SDK27.1 officialDuo inner-pose UI unavailable in current host; genuine outer and synthetic division tests remain separate evidence.

## Constraints / intended completion
AGENTS/README/MASTER-PLAN/ART-DIRECTION/CONCEPT-SPEC/ONE-MORE-FLOOR-V1-LEDGER are authoritative: 30floors, deterministic checkpoint every5, fall/Retry never full-run restart, Continue one tap, free camera, next jump unobscured, minimum48logicalpoints, DE/EN/accessibility. Main canonical commits; no Lua/Rojo/Roblox reintroduction. Sunset neon city, industrial authored platforms, cyan/amber hierarchy and actual black-hoodie runner are binding concept goals. Internal delivery only; preserve full Assets/metas/Packages/ProjectSettings source remotely before removing generated caches. User authorizes autonomous completion and existing universal Android signing identity; do not create a new keystore or public store release.

Evidence: README/AGENTS, docs/product files; Packages/manifest+lock, ProjectVersion/GraphicsSettings/ProjectSettings/EditorBuildSettings; Core/GameBootstrap, Gameplay/FloorCourse/PlayerMotor/MovingPlatform, Input/VirtualJoystick/PressButton, Camera/OrbitCamera, Editor/BuildAutomation. Current Unity runtime/Console not examined during onboarding; source inspection only, no Unity assets modified.
<!-- unity-onboarding:generated:end -->
