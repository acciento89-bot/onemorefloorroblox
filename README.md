# One More Floor

A compact third-person micro-challenge tower. The visible avatar clears one 5-10 second floor challenge and immediately enters the next, chasing a best-floor record.

## Product rule

This is intentionally a **small-scope, high-quality Roblox game**. Small scope does not permit placeholder presentation, debug-looking UI, inaccessible geometry, broken mobile layouts or unverified monetization.

## Core loop

Clear one tiny obstacle room quickly, transition immediately to the next floor, survive escalating variants and push the personal best.

## Non-negotiables

- The Roblox avatar remains visible during core gameplay.
- Retry from failure must be fast and obvious.
- First-time understanding target: under 10 seconds.
- Short-session loop with score, best score and readable progression.
- Server-authoritative rewards, purchases and persistent progression.
- Mobile, tablet, desktop and controller support.
- No surprise purchase prompt on spawn.
- Monetization accelerates/revives/cosmetics; it must not directly buy leaderboard placement.
- Production-quality UI, lighting, sound/VFX and environment treatment before public release.
- No QA screenshots or temporary artifacts on the user's Desktop. Use `/tmp/onemorefloorroblox-qa`; only intentionally retained evidence belongs under `docs/evidence/`.

## Monetization direction

Revive at previous floor, limited skip token, temporary coin multiplier, trails, floor themes and celebration effects. No purchased leaderboard score.

## Canonical execution order

1. `README.md`
2. `docs/MASTER-PLAN.md`
3. `docs/ART-DIRECTION.md`
4. `docs/ONE-MORE-FLOOR-V1-LEDGER.md`
5. Detail plan for the next open phase

The ledger is the source of truth for implementation state.
