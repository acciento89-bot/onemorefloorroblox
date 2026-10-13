# Floor composition and baked reflections implementation plan

> **For agentic workers:** Use superpowers:executing-plans for inline execution. The user has already authorized this direction, serial One More Floor work and a combined graphical pass; do not add another approval round.

**Goal:** Improve actual character/carriage prominence, physical surface response and complete lobby-floor composition against the unchanged approved board.

**Architecture:** Extend the existing fixed camera and palette. Bake one reusable original atrium environment map in the native Mac Editor, feed its immutable shared cubemap into metal/glass materials, and keep authored prefab and gameplay authority unchanged. All graphical changes are combined before native player visual validation.

**Tech Stack:** Unity6000.6.4f1, URP17.6, native Editor, uGUI/TMP, ShaderLab/HLSL.

**Spec:** docs/ART-DIRECTION.md and the user's approved original board/character/lift/light/UI briefs.

## Global constraints

- One More Floor first; no other-game edits or TestFlight while graphical acceptance fails.
- Fixed gameplay quaternion12/-18; no orbit, camera yaw animation or altered controller/timing/checkpoint/economy.
- Complete landing footprint, cabin silhouette and current actor above controls; >=48-point DE/EN controls.
- Preserve original artist edits, persistent prefab references, shared texture/material lifetime and the user-owned historical Concept deletion.
- No extra packages or per-frame cubemap captures/material allocations. One shared128-cubemap with mipmaps in portable RGBA32 (~512KiB GPU); captured HDR is tonemapped once at authoring time; honest Mac/device limits.

## Review focus

- Tight phone/tutorial/landscape bounds still contain the moving cabin, actual jump/boarding actor and next landing.
- Orthographic reflection vectors must use camera direction rather than perspective position.
- First-load missing-map fallback must remain visible and valid; a successful bake must enter actual shipped materials.
- Probe capture must restore sun/fog/ambient/active-scene state and release only its own objects/targets on success or failure.
- Lobby camera must project a continuous original floor through the lower safe area at both orientations, with source-filled room reflections distinct from gameplay claims.

### Task1 — composition

**Files:** CameraSystem/OrbitCamera.cs (existing Assets/Scripts/Camera/OrbitCamera.cs), Visuals/RunnerArt.cs, UI/LiftScenePreview.cs, Editor/FloorCompositionReflectionValidation.cs.

- [x] Validate actual shipping framing fixture with a real camera/course across aspect .46/.5625/1.78/2.17, ride/air/boarding samples and tutorial-limited panes; preserve all TargetFramed support points.
- [x] Replace the inflated orthographic full-kit boxes with actual cabin silhouette and landing support points; preserve quaternion and flight authority. Increase display scale1.10 to1.24 without altering collider.
- [x] Raise portrait lobby pitch from8 to17 degrees and move its orthographic eye20units behind focus; keep wide8 degrees and existing screen anchors. Check actual floor-ray coverage, not hardcoded object names.

### Task2 — original baked environment and surface polish

**Files:** Editor/FloorReflectionAuthoring.cs; existing Editor/FloorArtStudio.cs command bridge; Visuals/FloorArchitecturePalette.cs; Resources/FloorArchitecturalMetal.shader, FloorCabinGlass.shader, FloorRunnerSurface.shader; Resources/Rendering/CapturedAtriumReflection.asset and meta.

- [x] Add a native Editor six-face URP camera bake using a temporary own original GrandLobby fixture on excluded layer30, HDR128 faces; RenderProbe returned -1, so supported SingleCameraRequest renders each face, no collider/profile changes; restore scene lighting and release owned objects.
- [x] Save a mipped immutable Cubemap asset and bind it once per owning palette to metal/glass, with zero-strength fallback if unavailable.
- [x] Use URP orthographic-correct view directions, roughness-weighted reflection mip sampling, restrained glass Fresnel and improved soft skin/cloth/hair response; keep property names/renderer budgets.
- [x] Validate actual imported cube completeness/nonflat image data and actual palette consumption; inspect scene/lifetime/serialization.

### Task3 — native combined validation and evidence

**Files:** Editor validation/QA hooks as needed, docs/QA new batch receipt and visual-release-gate, README/context/ledger.

- [x] Open actual native Editor, run focused meaningful failures/green checks, bake original cubemap, full strict validation and Mac development build.
- [x] Inspect actual final player portrait/landscape, menus, default/premium styles and three worlds beside unchanged Concept; GPU reflection enabled/disabled control if required.
- [x] Run affected full route/reload scenario and capture exact-source receipts; no weakening thresholds or changing saved live profile.
- [x] Read-only review; fix concrete findings. Bind source/bundle/image hashes, retain failed historical candidates, report residual visual limits and block TestFlight until actually accepted.
- [x] Commit coherent verified work to main, push, show genuine comparison images.


## Implemented review rulings

- Capture `CabinFramingBounds` from original source vertices before Unity static batching; never interpret combined child mesh buffers again through child transforms.
- Native six-face capture uses supported URP SingleCameraRequest; RenderProbe returned -1 in this Editor and that failure is retained in the native log. Exclude previous lights; set deterministic ambient SH; restore sun/sky/fog/ambient/light enable states; clone reflective source materials with zero reflection strength to avoid recursive rebaking.
- Persist 128×128×6 RGBA32 linear UNorm with eight mips (524280 GPU bytes, plus editor/readable CPU storage); no runtime probe or render camera. Texture-format portability takes precedence over earlier compressed-HDR suggestion.
- Validate actual consumed catalogue room with ray/triangle intersections, not a surrogate plane/rectangle; each reflection face is individually nonflat/complete/finite.
- Validate 160 samples of the actual shipping FrameTower camera across ride/air/landing, phone/landscape and tutorial panes. Snapshot chapter-changed source scene light/sky/fog/ambient fields. Procedural Default-Skybox has no _MainTex; guard material property access, preserving the failed strict-log candidate as evidence.
- Native player GPU tests toggle reflection on actual Brass/Glass materials and verify orthographic translation invariance, alongside existing point-light/Bloom/forced-depth focus controls.

Completion evidence: docs/QA/floor-reflection-composition-20261013.json. Actual Concept acceptance is deliberately FAILED; completion of this combined technical pass does not authorize TestFlight. Latest user steering opens a separate menu-assets pass against the same approved board.
