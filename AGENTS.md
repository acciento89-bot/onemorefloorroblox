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


## Store icon accuracy gate — 2026-10-15

The user explicitly requires icon accuracy checks for every app after Apple cited guideline2.3 for One More Floor1.0(6). Before any new TestFlight/App Review upload, compare the assigned icon with actual gameplay, character, activity and world; do not approve a beautiful illustration that advertises a different interaction. Verify the exported native store icon and small/large variants use the intended same design. Record exact Unity source and source/native PNG/gameplay hashes, reviewer, date and decision in `docs/QA/store-icon-review.json`. `scripts/verify-visual-release.py` now fails closed for pending/failed/stale icon reviews as well as graphical reviews. Do not interpret PNG size or a passed gameplay test as icon-content approval. Complete this review from evidence; no new user permission is required. Existing public submissions are unchanged by this future-upload guard.
