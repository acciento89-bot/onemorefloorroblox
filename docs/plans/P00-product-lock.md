# P00 — One More Floor product lock

## Core run

- Player-facing progress is **FLOOR**, **BEST** and **STREAK**.
- One floor targets 5–10 seconds of active play.
- Successful transition target: <= 1.1 seconds from completion to controllable next floor.
- Failure shows one obvious Retry action immediately.
- The Roblox avatar remains visible throughout play, transition, failure and revive.

## V1 challenge library

Launch requires seven mechanically distinct families:
1. `precision_jump` — short landing/ledge commitment.
2. `moving_platform` — time one moving surface.
3. `dodge_gates` — cross a lane through alternating hazards.
4. `narrow_path` — controlled balance path with widened mobile bounds.
5. `switch_dash` — touch a switch then reach the exit.
6. `disappearing_tiles` — route across tiles that retract after contact.
7. `moving_wall` — react to one safe opening in a moving wall.

## Difficulty bands

- Floors 1–3: authored tutorial order; no random family selection.
- Floors 4–10: early; broad timing windows and slower hazards.
- Floors 11–25: mid; narrower windows and combined movement pressure.
- Floors 26+: high; faster parameters only inside tested safe bounds.
- Difficulty changes parameters, never control rules or collision truth.

## Selection and variety

- Deterministic seeded selector exists for QA.
- Same challenge family may not appear on consecutive generated floors.
- A variant key may not repeat inside the previous four generated floors.
- Every generated challenge must validate against its family bounds before construction.
- Static acceptance includes at least 500 generated floors across multiple seeds.

## Monetization boundary

- Revive restores the current valid floor and marks the run assisted.
- Skip advances one floor only and marks the run assisted.
- Assisted runs may earn normal cosmetic currency but cannot improve competitive Best Floor.
- Coin multiplier affects currency only.
- Cosmetics: trail, completion burst and tower theme.
- No purchase prompt on spawn and no paid leaderboard progress.

## Release-quality criteria

- First objective understandable in <= 2 seconds.
- Every family exposes shape/material cues independent of color.
- Compact-phone target controls remain >= 48 px where custom UI is used.
- Camera keeps avatar + actionable geometry visible without first-person clipping.
- Runtime geometry is bounded to current/next transition content; old floors are destroyed.
- Full build, lint and deterministic tests must pass before any ledger item is marked complete.
