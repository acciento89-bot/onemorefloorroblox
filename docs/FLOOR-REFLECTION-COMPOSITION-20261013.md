# One More Floor — native reflection and composition pass

This candidate changes One More Floor only. It uses the unchanged approved Oct10 board, the existing original editable models and the actual Mac Unity6000.6.4f1 Editor. Overall Concept acceptance remains separate from compilation and functional checks. No TestFlight upload, iOS version bump or other-game change.

## Combined visual changes

- Character display scale1.24, previously1.10, with unchanged character controller, collision and deterministic transfer. Skin/cloth/hair use softer specular response and correct orthographic viewing direction.
- Fixed12/-18 gameplay camera fits the original carriage silhouette, required landing corners, target header and actor envelope. `CabinFramingBounds` captures original vertices before Unity static batching; the camera reads that component only when the carriage changes. There are no per-frame mesh reads or orbit/yaw changes.
- Portrait lobby pitch17 degrees and eye distance20 prevent lower rays starting beneath the floor. Wide lobby retains8 degrees; the original room now includes a continuous marble base and wider inlaid tiles. Native export preserves the new room as GrandLobbyGeometry-v4 and the shipping Resources catalogue references its prefab child.
- One original128-cubemap supplies restrained actual scene reflections for brass/copper and transparent glass. Shader view vectors are orthographic-correct; metal roughness selects mip level. The old authored reflection remains the missing-map fallback. No runtime probe/capture camera is introduced.

## Original environment capture

`FloorReflectionAuthoring.Begin` runs in the native Editor through the existing bounded Art Studio command bridge (`bake-reflection`). Native RenderProbe returned -1; the recorded failed attempt is retained. Supported URP SingleCameraRequest renders six90-degree original room faces to a temporary HDR128 target. Capture excludes unrelated geometry/lights and uses a controlled warm key/ambient SH plus the room's own cabin lamps. Reflected source materials are cloned with zero map strength to avoid recursively capturing the previous bake. Sun, sky, fog, ambient SH/mode/light and prior light enable states are restored; temporary objects, materials and render targets are released.

The source HDR capture is tonemapped once into `Assets/Resources/Rendering/CapturedAtriumReflection.asset`: linear RGBA32 UNorm,128×128×6,eight mips,524280 GPU bytes. Editor/readable CPU storage and serialization are additional. This portable shared LDR map deliberately replaces the plan's earlier compressed-HDR suggestion. Each palette loads the persistent resource once and never owns/destroys it. Main-texture budgets exclude this separately documented cubemap.

## Validation and provenance

Frozen source and bundle proofs are in `/Volumes/SSK SSD/Kamilunavo/Builds/FloorReflect-20261013/reflect1-source-proof.json`. Current source8e20b2679eede802213fa797c66005bcc5b884cb7cc473716c9f0f0ab5b49ab0; bundle6dc08a0e401a204c66d6d826a4eb33fda48a448b3850ad87dd0b7c9ab1633410. Native final build succeeds with FLOOR_ALL_PASS checks14712/loggedErrors0.

Actual imported room ray/triangle tests cover the lower phone/wide viewport. Each captured cube face is individually complete, finite and nonflat.160 samples invoke the actual shipping FrameTower camera across phone/wide/tutorial panes, ride/air/landing poses and multiple floors. Scene chapter lighting changed by the fixture is restored; a procedural skybox lacking _MainTex is handled explicitly. The earlier strict-log rejection is retained, rather than called a pass.

Native player GPU negative controls show actual reflection deltas0.1176471 for Brass and0.007843137 for Glass, with orthographic translation invariance. Existing actual point light, bloom and forced-depth focus probes also pass. Full runtime/menu/reload evidence and image manifests are recorded in `docs/QA/floor-reflection-composition-20261013.json` after completion.

## Review and remaining visual work

Read-only review corrected pre-batch silhouette capture, actual consumed floor intersection, per-face capture checks, controlled capture lighting and validator scene-state restoration. Original character/architecture geometry caps, paid styles, DE/EN and parent Back flows remain.

Actual early portrait/wide player images show the floor strip removed and the larger character. The entire scene still falls short of the approved illustration: face/hair finish, thin ornate carriage proportions, carved stone/recess detail, scenic card richness, reflective marble and complete atrium framing need further authoring. A native Mac render is not physical iPhone performance/touch/provider acceptance. The source-bound visual release gate stays FAILED until that visual review truly passes. Perfect Drop settings action bleed/startup cohesion and approved advertising work remain queued after One More Floor.
