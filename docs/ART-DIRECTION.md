# One More Floor Art Direction

## Target
Buildable neon-city arcade style. Warm sunset/orange route edges balanced by cyan accent light and dark navy UI.

## Palette
- UI navy: #07121F
- Platform graphite: #171B24
- Route gold: #FFB72E
- Cyan state accent: #28C7F7
- Sunset coral: #FF6A45
- Primary text: #F7FAFF
- Secondary text: #A7B5C6

## Environment
- Modular floating platforms over a dense but optimized city backdrop.
- A visible checkpoint ring every checkpoint band.
- Limited background geometry; baked/lightweight effects later.
- Platform edge light communicates route.
- Avoid visual noise behind the next jump.

## UI
- Pause top-left.
- Stage/progress top-center.
- Score + Coins top-right.
- Checkpoint dialog only when a checkpoint is reached.
- Joystick left, jump right.
- No persistent oversized menu during active movement.

## Primary visual reference

![Primary One More Floor concept](concepts/PRIMARY-CONCEPT.png)

This image is the binding visual target for cyberpunk city depth, sunset grading, neon route edges, checkpoint language, runner presentation and compact mobile HUD. Checkpoint/continue/retry panels are state-driven overlays only and must never remain over active movement.

### Non-negotiable visual gates
- The route must read instantly against the city backdrop.
- Platform geometry needs authored industrial/cyberpunk form; simple cubes are collision/prototype only.
- Orange/gold and cyan lighting must create route/checkpoint hierarchy without saturating the whole scene.
- Falling must recover immediately to the latest valid checkpoint with controls restored on the first playable frame.
- Camera and touch controls must support fast vertical movement without forced recentering.



## Approved lift city revision — 2026-10-10 (current)

The approved three-view board at `/Volumes/SSK SSD/Kamilunavo/Artifacts/KamilunavoDelivery-20261009/concepts-20261010/one-more-floor-3-ansichten.png` and [the focused implementation contract](FLOOR-CONCEPT-2026-10-10.md) supersede the historical floating-platform and orbit/joystick art instructions above. Current gameplay is the fixed-camera one-thumb elevator timing game. The deleted historical primary concept must not be restored.

Warm ivory masonry and lobby surfaces, copper lift crowns, two dark teal guide rails, arched floor balconies, small amber lanterns and restrained greenery provide the current direction. The lobby and Sunset/Neon/Skyline chapter cards render authored 3D kit. Bank-or-risk is a compact ivory lower sheet with permanent amber bank / teal continue actions. All environment decoration is non-colliding; landing dimensions, arrival timing, checkpoint/save/economy and paid ownership remain authoritative. Engine screenshots and device performance still require verification before visual acceptance.
