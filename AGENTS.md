# AGENTS.md

## Product
One More Floor is a native iOS/Android Unity game.

## Rules
- Read README -> MASTER-PLAN -> ART-DIRECTION -> V1 LEDGER before implementation.
- Ledger is canonical state.
- Portrait-first, touch-first, safe-area aware, 60 FPS target.
- A fall must recover to the latest checkpoint immediately. Never restart at floor zero unless the player explicitly restarts the run.
- The approved elevator redesign uses a fixed camera rotation; no orbit or forced yaw behind the character.
- Do not add legacy runtime/platform files, Lua/Luau, Rojo or place files.
- No production UI may cover the next jump.
- Commit coherent verified work to main.

## Quality
- Compact-phone readability.
- Touch targets >= 48 logical points.
- Continue/Retry flow must be one tap.
- Checkpoints are deterministic.
- DE/EN required before release.

## TestFlight visual release gate — 2026-10-11

The user requires actual Unity-rendered gameplay, menus, character/tower and world/design variants to resemble the approved concept before ANY new TestFlight upload. Compilation and functional tests alone do not satisfy this gate. Run `python3 scripts/verify-visual-release.py` before scripted or Xcode GUI distribution. A failed/pending/stale review blocks upload. Inspect actual rendered evidence against the approved board before recording a passed report in `docs/QA/visual-release-gate.json`; bind it to the exact Unity source fingerprint. Never mark illustrations or generated mockups as actual gameplay. All other release checks remain required.
