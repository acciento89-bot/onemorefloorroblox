# Floor menu and tutorial revision — 2026-10-11

User supplied actual build5 iPhone screenshots expose two failures missed by the earlier settled-layout editor fixtures: a giant tutorial exit covering instruction/action space and Settings returning to gameplay rather than its invoking menu. The requested visual direction is a calmer, inviting architectural lobby with cream, copper and teal, distinct from Rising's atlas. Existing fixed camera, elevator arrival timing, risk/bank rules, saves and provider transactions remain authoritative.

## Baseline and discriminating regression

Source trace: `CreateCoach` calculates a touch target once, when native point scale may be 3 but CanvasScaler still reports 1. That creates a 147-unit square; later `Layout` moves the parent coach but never recalculates the exit. Settings uses the same oversized dark `Row` presentation as commerce. `Begin` wires every default footer directly to `Close`, which resumes gameplay; `_page` has no retained parent.

`FloorPresentationRegression.Validate` runs the real UI construction and callbacks in editor. It forces the native first-frame point scale 3/canvas scale 1 mismatch, settles canvas scale and validates real exit/text/action rectangles through density changes. Navigation uses home → style → settings, refresh, locale change, orientation rebuild and actual Back callback, then checks the retained style view and paused game. Store and rewarded-video presenter state fixtures exercise the gameplay and parent navigation guards without contacting providers. Test saves use an isolated key. Existing menu, preview, gameplay and commerce validations remain required.

Actual baseline Unity RED exited 1 and reproduced both expected failures across all six DE/EN/size fixtures: exit remains 147 units after initialization mismatch, and missing child-parent Back. No compilation errors. Receipt: `/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/OneMoreFloor/baseline-floor.log`. Final engine GREEN/rendered evidence is coordinated by the root worker and must be recorded separately before claiming acceptance. No engine, build or native app launch is performed by this source worker.

## Implemented source; engine/render acceptance pending

- Recompute coach exit and text geometry every settled layout; a compact exit owns at least 48 logical points and remains wholly inside the coach.
- Retain invoking pages through rebuilds and separate a compact Back action from explicit Resume.
- Guard menu navigation while a purchase, video or provider privacy sheet is presenting.
- Replace Settings' loud pills with compact cream rows, dark readable labels and clear toggle/value states; preserve existing action IDs and persistence.
- Reduce the lobby hero to a shallow architectural welcome preview and three skyline gallery cards, restrained copper headings and useful spacing. Existing real 3D chapter previews and resource disposal remain intact.
- Use natural DE/EN practice labels and preserve skippable/replayable tutorial progression.

Numerical palette preparation verifies dark ink/teal/copper on ivory and quiet row surfaces in the darkest disabled tint at minimum 5.48:1. This is source palette evidence, not rendered device acceptance.

## Actual rendered lobby correction

The first actual runtime `Runtime/lobby-de.png` and `Runtime/lobby-landscape.png` passed interaction checks but failed the approved concept comparison: a shallow preview, three identical castle clusters distinguished only by color, arrow/crystal utility placeholders and a large unused portrait gap. These rendered frames are the visual baseline for this correction.

The lobby now gives its available portrait height to the actual cabin, runner, two architectural bays and an atrium landing, with three larger vertical chapter rows layered beside it. Landscape uses a cabin column, three scenic chapter rows, and a compact utility column. The selected profile style reaches the preview runner. The exact chapter names, original button identifiers, fixed targets, pause/history, provider guards and main gameplay camera remain intact.

The UI-only chapter preview geometry is separately authored for a sunset canal with stone bridge/clock tower, a neon waterfront with stepped modern towers and lamp reflections, and a daylight rotunda/plaza with colonnade and trees. No gameplay city kit or global RenderSettings are changed. Spatial preview leases, exclusion layer, collision-free meshes and teardown are retained. Render targets follow the actual card shape within a 768-pixel cap; a local spatially bounded softbox lights only the preview layer.

FloorLobbyGlyph supplies real route-pin/path, hat, shopping bag, wrapped gift, award star, cog and open-book silhouettes. Gameplay HudIcons is unchanged. Root explicitly authorized the narrow obsolete FloorMenuValidation utility-component expectation to use this helper while retaining its contrast, hit bounds, isolation and lifetime checks.

`FloorLobbyVisualRegression.Validate` additionally checks useful architecture height, three scenic rows, genuinely different chapter meshes, six distinct symbol meshes and unchanged wallet/style/progress/pause in DE/EN portrait, landscape and narrow panes. Focused compilation against the actual Unity references passes in `OneMoreFloor/Lobby/Floor.Lobby.SourceCheck.dll`. Actual engine GREEN and a new rendered concept comparison are still required; source checks do not establish visual acceptance.
