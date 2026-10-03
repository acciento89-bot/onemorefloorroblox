# One More Floor runtime smoke — 2026-10-03

## Source under test

- Canonical repo: `acciento89-bot/onemorefloorroblox`
- Runtime source commit: `74ae65900577eb871d56425d3f5589e45a5438c0`
- GitHub Actions run: `37112006626` — completed successfully.
- QA artifacts stayed under `/tmp/onemorefloorroblox-qa`.

## Session hygiene

A stale Perfect Jump Rojo server was found listening on localhost port 34872 and was terminated before the accepted One More Floor run. The generated One More Floor `.rbxlx` was inspected directly and contained One More Floor markers with no Perfect Jump markers before runtime acceptance continued.

## Verified in real Studio Play

- Fresh Floor 1 spawned into the authored **Precision Jump** tutorial floor.
- The player's Roblox avatar was visible in the custom third-person camera.
- HUD rendered Floor / Best / Streak / Coins, objective copy and countdown progress.
- Timeout produced the failure panel and obvious Retry action; Retry recreated the same challenge.
- A real keyboard movement + jump sequence landed on the Precision Jump landing deck.
- Continuing through the exit completed Floor 1 exactly once in the observed run.
- Server progression advanced to **Floor 2 / Best 1 / Streak x1 / Coins 3**.
- Floor 2 loaded as the authored **Balance Run** tutorial family and the previous floor geometry was replaced.
- Camera/avatar framing remained valid after the Floor 1 → Floor 2 transition.

## Still open

- Floor 2 completion and Floor 3 Switch Dash have not yet been runtime-accepted.
- The remaining launch families have not yet received individual live completion/failure acceptance.
- Touch, tablet and physical controller input remain open.
- Persistence, economy ownership, monetization, revive/skip, audio/VFX and published-place rejoin remain open.
- The first live visual pass exposed an overly dark room composition. A follow-up source patch increases ambient/exposure, lightens structural materials, adds four practical room lights and reduces/repositions the floor identity board. That follow-up must be re-run in Studio before its visual gate can be marked complete.
