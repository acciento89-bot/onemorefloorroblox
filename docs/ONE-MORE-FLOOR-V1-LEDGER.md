# One More Floor V1 Quality Ledger

**Portfolio order:** 4 / 4 — largest of the four because quality depends on a real library of varied micro-challenges, transitions and content pacing.

Status: `[ ]` open · `[~]` implemented but not fully runtime-verified · `[x]` verified complete · `[!]` externally/runtime blocked

## Release quality contract

One More Floor must feel like a compact, replayable tower of authored-quality micro-challenges. Procedural assembly may choose and parameterize rooms, but it may not produce repetitive or broken filler.

Mandatory V1 rules:
- Avatar is always visible and controllable.
- Every floor’s objective is understood within roughly 1–2 seconds.
- Individual floors take approximately 5–10 seconds at intended skill.
- Transition to the next floor is immediate and visually clear.
- Challenge library has genuine mechanical variety, not recolored copies.
- No impossible combination of challenge parameters.
- Camera adapts per floor without losing control continuity.
- Death/retry/revive behavior is fast and deterministic.
- Progression, best floor, currency and purchases are server-authoritative and persistent.
- Mobile/tablet/desktop/controller all receive fair versions of each challenge.
- Production art/audio/VFX and screenshot quality are mandatory.
- Full published-private-place journey and rejoin test are release gates.
- QA output uses `/tmp/onemorefloorroblox-qa`.

## P00 Product and content lock
- [x] P00-T01 Lock score vocabulary: current floor, best floor, streak/bonus where applicable
- [x] P00-T02 Lock floor target duration and overall run pacing
- [x] P00-T03 Lock challenge families and minimum V1 content count
- [x] P00-T04 Lock difficulty bands and parameter ranges per challenge family
- [x] P00-T05 Lock revive/skip rules and anti-pay-to-win boundaries
- [x] P00-T06 Measurable content-variety and release-quality criteria

## P01 Technical foundation
- [x] P01-T01 Rojo client/server/shared architecture
- [x] P01-T02 Challenge registry/schema/config system
- [x] P01-T03 Round/floor state and remote definitions
- [x] P01-T04 Lint/format/tests/build toolchain
- [x] P01-T05 CI + release readiness
- [~] P01-T06 Dev/prod place and canonical build policy

## P02 Character, camera and control continuity
- [x] P02-T01 Safe spawn/lobby/start floor
- [~] P02-T02 Unified movement/jump/interact controls
- [ ] P02-T03 Camera profile system supports different floor layouts without abrupt disorientation
- [~] P02-T04 Camera returns cleanly after death/revive/transition
- [~] P02-T05 Touch controls and safe areas
- [~] P02-T06 Keyboard/mouse/controller parity
- [ ] P02-T07 Runtime multi-floor camera/control acceptance

## P03 Floor lifecycle
- [~] P03-T01 Server-authoritative floor state machine: prepare → active → success/fail → transition
- [~] P03-T02 Floor objective appears only when needed and never blocks play
- [~] P03-T03 Completion triggers exactly once
- [~] P03-T04 Timeout/fall/hazard failures trigger exactly once
- [~] P03-T05 Transition cleans prior floor geometry/connections/effects
- [~] P03-T06 Next floor starts within target transition time
- [~] P03-T07 Retry/revive recreates a valid floor state

## P04 Challenge library — V1 content
- [~] P04-T01 Precision jump challenge family
- [~] P04-T02 Moving-platform timing family
- [~] P04-T03 Dodge/hazard pattern family
- [~] P04-T04 Narrow-path/balance family
- [~] P04-T05 Door/switch/short interaction family
- [~] P04-T06 Falling/disappearing platform family
- [~] P04-T07 Moving-wall/gap reaction family
- [ ] P04-T08 At least one additional visually distinct family after playtest evidence
- [x] P04-T09 Each family has multiple safe parameterized variants
- [ ] P04-T10 Every family has runtime acceptance and device fairness evidence

## P05 Selection, variety and difficulty
- [x] P05-T01 Deterministic seeded floor selector for QA
- [x] P05-T02 Anti-repeat rules prevent same family/variant spam
- [x] P05-T03 Difficulty rises by parameter changes, not unfair speed spikes
- [~] P05-T04 Challenge prerequisites prevent impossible device/layout combinations
- [x] P05-T05 500-floor simulation validates variety and parameter bounds
- [ ] P05-T06 100-floor live soak validates cleanup/memory/collision
- [ ] P05-T07 Early/mid/late difficulty samples are human-playtested

## P06 Score, best floor and rewards
- [~] P06-T01 Current/best floor model
- [ ] P06-T02 Completion speed/clean-play bonus if retained by product lock
- [~] P06-T03 Server-only floor advancement
- [ ] P06-T04 PB update and leaderboard-safe validation
- [ ] P06-T05 Reward cadence does not interrupt transitions
- [~] P06-T06 Anti-skip/replay/fake-completion guards

## P07 Progression and persistence
- [x] P07-T01 Currency model
- [~] P07-T02 Trails, win effects, tower/floor themes and avatar-adjacent cosmetics
- [~] P07-T03 Server purchase/equip
- [x] P07-T04 Versioned profile/migration
- [~] P07-T05 Save/lock/recovery
- [~] P07-T06 New-session rejoin retains best floor, currency, cosmetics and settings

Implementation note (2026-10-03): `ProfileRules` now owns schema v1 sanitizing/migration and starter entitlements; `ProfileService` adds DataStore `UpdateAsync` session locking, autosave, leave/shutdown release and an explicit non-persistent Studio fallback. Best Floor and Coins are loaded into the authoritative course and written back on progression. `CosmeticService` now owns a server-allowlisted coin purchase/equip path for three trail, three win-effect and three room-theme choices; the client Style Garage only sends intent. Equipped trails apply to the avatar, win effects respect Reduced Motion, and room theme rails apply on the next floor. Pure-Luau profile/catalog tests pass. P07-T02/T03/T05/T06 remain runtime-gated until an API-enabled published/private run proves buy/equip/rejoin and lock release.

## P08 Tutorial and retention
- [~] P08-T01 First three floors form a natural tutorial without long text
- [~] P08-T02 New challenge family can display a one-line first-seen hint
- [~] P08-T03 Daily login
- [~] P08-T04 Daily floor/challenge goal
- [~] P08-T05 Achievements for floor milestones/family mastery
- [~] P08-T06 PB celebration and “one more floor” retry hook

Implementation note (2026-10-03): profile schema v2 adds UTC-day login streaks, a five-floor daily goal, idempotent milestone/family-mastery achievements and persistent first-seen family tracking. The authoritative floor service grants rewards and reports PB/daily/milestone feedback without adding blocking interstitials. First-seen hints are retained for tutorial floors and otherwise disappear after the family is learned. Pure-Luau retention tests cover duplicate-login, streak reset, daily idempotency, milestone idempotency and family mastery. Runtime/rejoin evidence is still required before these gates become [x].

## P09 Monetization
- [ ] P09-T01 Final products/passes/prices
- [ ] P09-T02 Revive restores previous/current valid floor state
- [ ] P09-T03 Limited skip token cannot purchase leaderboard floor progress without clearly separated competitive treatment
- [ ] P09-T04 Coin multiplier affects economy only
- [ ] P09-T05 Receipt allowlist/idempotency/serialization
- [ ] P09-T06 Explicit-prompt shop and entitlement UI
- [ ] P09-T07 Duplicate/retry/aborted purchase tests
- [!] P09-T08 Real Developer Product receipt + rejoin verification

## P10 Production UI/UX
- [~] P10-T01 HUD: floor, PB and immediately relevant objective only
- [~] P10-T02 New-floor intro is fast and non-blocking
- [ ] P10-T03 Failure/revive/retry flow
- [~] P10-T04 Challenge-specific hints disappear once learned
- [~] P10-T05 Shop/cosmetic preview
- [~] P10-T06 Compact phone/tablet/desktop
- [ ] P10-T07 Controller focus/accessibility/reduced motion

## P11 Production art
- [~] P11-T01 Tower/floor shell has a coherent recognizable identity
- [~] P11-T02 Each challenge family is visually distinct but belongs to same world
- [~] P11-T03 Hazards and safe surfaces communicate function through shape/material as well as color
- [ ] P11-T04 Transitions conceal generation/cleanup cleanly
- [~] P11-T05 Lighting/material pass avoids unreadable dark floors
- [ ] P11-T06 Screenshot-quality acceptance across multiple floor families

## P12 Audio and VFX
- [ ] P12-T01 Floor start/success transition cues
- [ ] P12-T02 Family-specific action/hazard cues where useful
- [ ] P12-T03 Failure/revive/retry feedback
- [~] P12-T04 PB/milestone/reward feedback
- [ ] P12-T05 VFX do not obscure short reaction challenges
- [ ] P12-T06 Owned/Roblox-safe assets and reduced-motion/audio QA

## P13 Security and persistence hardening
- [~] P13-T01 Remote/rate-limit audit
- [~] P13-T02 Server owns floor selection/completion/current floor
- [ ] P13-T03 Position/teleport/timing/NaN guards
- [~] P13-T04 Challenge-specific spoof checks
- [ ] P13-T05 Economy/purchase serialization
- [ ] P13-T06 DataStore migration/lock/recovery and diagnostics

## P14 Mandatory full runtime journey
- [ ] P14-T01 Fresh player through tutorial floors
- [ ] P14-T02 At least one live completion from every V1 challenge family
- [ ] P14-T03 Representative fail path from every failure type
- [ ] P14-T04 20+ consecutive floors with correct transitions and no leaked geometry
- [ ] P14-T05 Reward/cosmetic purchase/equip
- [ ] P14-T06 Revive and skip-token rules
- [ ] P14-T07 Respawn camera/control recovery
- [ ] P14-T08 PB update
- [ ] P14-T09 New-session persistence/rejoin
- [ ] P14-T10 Extended 30-minute run stability

## P15 Device and performance QA
- [ ] P15-T01 Compact phone touch — every challenge family
- [ ] P15-T02 Tablet touch — every challenge family
- [ ] P15-T03 Desktop keyboard/mouse — every challenge family
- [ ] P15-T04 Controller — every challenge family
- [ ] P15-T05 No challenge relies on precision unavailable to one supported input class
- [ ] P15-T06 Stable transition performance, bounded active geometry and memory

## P16 Release
- [ ] P16-T01 Production icon/thumbnails/metadata represent actual challenge variety
- [ ] P16-T02 Privacy/content questionnaire
- [ ] P16-T03 Canonical private publish
- [ ] P16-T04 Full P14 journey repeated in published private place
- [ ] P16-T05 Build hash/place version/rollback record
- [!] P16-T06 Public release after paid receipt/rejoin evidence and zero known P0/P1 defects

## P17 Post-launch
- [!] P17-T01 First telemetry review
- [!] P17-T02 Evidence-based difficulty/content rotation adjustment
- [ ] P17-T03 New challenge families/themes/cosmetics cadence

## Definition of Done

One More Floor V1 is done only when the challenge library has real mechanical variety, every supported device can fairly complete every launch family, transitions remain clean over long runs, progression survives rejoin, and the published private build passes the complete multi-floor player journey with production-quality presentation.
