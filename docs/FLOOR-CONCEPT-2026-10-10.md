# Approved October 10 lift visual revision

The approved three-view board (lift lobby / timing game / bank-or-risk) supersedes the older floating-platform art reference. The player's one-thumb transfer, deterministic moving-bay timing, fixed camera quaternion, exact-arrival reward, checkpoint recovery, wallet, purchases and tutorial save isolation stay authoritative.

## Implementation contract

- Warm ivory surfaces, dark teal text and amber action hierarchy; Sunset / Neon / Skyline are illustrated chapter cards, not a colored pill stack.
- Lobby hero renders the real authored 3D cabin, rails, runner and architectural bay into a bounded menu preview. The chapter drawings are presentation only; active gameplay remains real 3D.
- Existing landing surface/collider dimensions are retained. Copper cabin crowns/posts/balustrades and cream arched floor bays are non-colliding decoration. The existing stationary track remains owned by ElevatorMotion.
- City silhouette gains cream masonry, teal domes/spires, warm windows and atmospheric depth. Architecture sits outside the landing/flight corridor.
- The actual predicted arrival drives a compact teal/amber timing meter above the one-thumb action. It consumes no pointer input and is included in the camera-free viewport; guided practice retains its coach reserve instead.
- Bank-or-risk becomes a measured ivory lower sheet with illustrated current/next floor cards and fixed amber bank / teal risk actions. DE/EN, landscape, compact phones and >=48 logical-point targets remain gates.
- No packages, paid assets, save changes, native settings/version changes, external publication or heavy build operations by the implementation owner.

## Verification/handoff

Extend FloorMenuValidation (wired into FloorValidation.ValidateAll) for real 3D preview ownership, authored cabin/two rails/bay kit, unchanged collider, compact decision bounds and all chapter targets. Extend -qaElevatorShowcase for warm lobby/risk surfaces and actual presentation geometry, then root runs that showcase and full -qaElevator 87-transfer route. Source checks cannot establish cinematic parity or native device acceptance; root must inspect rendered captures at portrait/landscape before delivery.

## Exact root-owned engine commands

```sh
UNITY='/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity'
PROJECT='/Volumes/SSK SSD/Kamilunavo/Projects/OneMoreFloorRoblox'
DELIVERY='/Volumes/SSK SSD/Kamilunavo/Builds/VisualRevision-20261010'
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -executeMethod FloorValidation.ValidateAll -logFile "$DELIVERY/floor-editor-concept.log"
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -executeMethod BuildAutomation.BuildMacPreview -buildOutput "$DELIVERY/OneMoreFloor-ConceptQa.app" -logFile "$DELIVERY/floor-concept-build.log"
env -u FLOOR_QA -u ELEVATOR_QA ELEVATOR_QA_SHOWCASE=1 ELEVATOR_QA_ID=floor-concept-showcase ELEVATOR_QA_OUTPUT="$DELIVERY/floor-concept-showcase" "$DELIVERY/OneMoreFloor-ConceptQa.app/Contents/MacOS/OneMoreFloor" -qaElevatorShowcase -qaExit
env -u FLOOR_QA -u ELEVATOR_QA_SHOWCASE ELEVATOR_QA=1 ELEVATOR_QA_ID=floor-concept-full ELEVATOR_QA_OUTPUT="$DELIVERY/floor-concept-full" "$DELIVERY/OneMoreFloor-ConceptQa.app/Contents/MacOS/OneMoreFloor" -qaElevator -qaExit
```

Confirm the executable name from the generated app bundle before launching. Inspect `lobby-de`, `lobby-en`, `lobby-landscape`, `risk-portrait-de/en`, `risk-landscape-de/en`, tutorial and full-game floor captures. The whole-source fallback response file and successful compiler log are at `.utmp/concept-check/source.rsp` and `compile.log` (ignored transient validation outputs). RenderTexture creation/rendering is deferred in Editor-only layout fixtures; actual player capture is required to prove pixels. Remaining material risks are preview lighting/roof occlusion, route visibility with the new cabin crown, and device frame cost of the authored city. The new scenes/targets are disposed per menu ownership and render only on first display/size changes, rather than adding ongoing menu-camera updates.

## Actual-render defect investigation and focused correction

The first actual October10 lobby rendered the cabin hero and all three chapter cards as the same combined city. Source trace confirmed every live `LiftScenePreview` root occupied `(1000,0,1000)` with the same layer 31 culling mask: excluding gameplay did not isolate the previews from each other. Actual editor regression `FloorMenuValidation.ValidatePreviewIsolation` reproduced RED at `simultaneous preview roots have independent spatial slots` before production changes; root receipt `preview-isolation-red.log` is retained.

Each preview now leases one of 64 recyclable spatial slots, 128 world units apart, while retaining its layer 31 mask and 40-unit camera far plane. On destruction it deactivates its old scene before recycling the slot, so delayed Unity destruction cannot show old geometry in a replacement render. Existing target/mesh/material ownership and close/disposal remain intact. The fixture checks four simultaneous actual preview roots/camera frusta, excludes all foreign live renderer bounds, recycles a released slot, preserves the other live preview and repeatedly releases/reuses its RenderTexture.

Actual yellow cream/gold borders and solid ink utility blocks had separate deterministic causes: FloorPanel's amber classifier omitted the blue channel, and Dock recolored the transparent icon container as well as the glyph. The shader now distinguishes amber from cream and uses a restrained neutral cream border/sheen. Dock colors only HudIcons; all 12 DE/EN layout fixtures additionally require transparent icon panes and visible ink glyphs. Sun/rim/ambient, ivory/copper/brass/deck and chapter stone values are modestly reduced/neutralized to retain real cabin form and shadow contrast.

Whole-source runtime/editor type checking succeeds with 0 errors using the installed Unity/SDK references; existing obsolete API/netstandard warnings remain. Proof `.utmp/preview-isolation/source.rsp` and `compile.log`. `git diff --check` passes. Timing, recovery, fixed-camera behavior, QA dispatch, wallet/profile/economy/purchases, collision and geometry dimensions are unchanged by this correction. Coordinated engine GREEN and actual showcase/87-transfer rerender are still root-owned acceptance steps; no physical performance or pixel acceptance follows from source compilation.

Actual tutorial captures also show mirrored floor numerals. The architectural TextMesh was rotated 180 degrees around Y while the fixed camera sees the bay from negative Z. Its label now uses the default identity orientation; the deck fixture asserts the readable facing. This changes presentation only and still awaits the coordinated rendered confirmation.
