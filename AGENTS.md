# AGENTS.md

## Product
One More Floor is a native iOS/Android Unity game.

## Rules
- Read README -> MASTER-PLAN -> ART-DIRECTION -> V1 LEDGER before implementation.
- Ledger is canonical state.
- Portrait-first, touch-first, safe-area aware, 60 FPS target.
- A fall must recover to the latest checkpoint immediately. Never restart at floor zero unless the player explicitly restarts the run.
- Camera rotation belongs to the player; do not force yaw behind the character every frame.
- Do not add legacy runtime/platform files, Lua/Luau, Rojo or place files.
- No production UI may cover the next jump.
- Commit coherent verified work to main.

## Quality
- Compact-phone readability.
- Touch targets >= 48 logical points.
- Continue/Retry flow must be one tap.
- Checkpoints are deterministic.
- DE/EN required before release.
