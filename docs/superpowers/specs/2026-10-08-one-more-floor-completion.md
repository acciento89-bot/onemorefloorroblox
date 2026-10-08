# One More Floor internal completion design

Architectural completion of the existing game. User explicitly delegates autonomous completion, concept graphics, testing, full repository backup then cache cleanup and continuation; main is canonical. This authorization overrides repeated design/plan approval pauses while user is away. Implementation inline; one fresh whole-change reviewer at the end.

## Intent and selected approach
A native, touch-first arcade climb above a sunset neon city matching PRIMARY-CONCEPT.png. Keep30 floors numbered1..30 (starting platform1,29 actual advancing jumps), checkpoint at5/10/15/20/25/30, immediate latest-checkpoint recovery, one-tap Continue/Retry and genuinely free camera. UI never covers an active next jump.
Reuse verified own rendering/input/save/optional-commerce foundations from Rising Steps, adapted to this game's industrial world and checkpoint semantics. Alternative minimal prototype polish fails visual/progression scope; a new physics controller increases risk without benefit. No Roblox files despite historical folder name.

## Rules and persistence
Three towers: Sunset Circuit, Neon Crossing, Skyline Rush. Tower0 initially unlocked; completion unlocks next. Three stars no falls within240s, two no falls after240s, one with falls. Schema1 profile saves tower/current floor/checkpoint/highest rewarded floor, score, coins, stars, settings and cosmetics. Unknown schema preserved and cannot purchase/watch. Ordered landings only. First landing per floor in current run awards100+10*index score,1+index/5 coins plus1 perfect coin; replay below highest rewarded floor adds no score/coins/perfects. Explicit new run resets run reward frontier, score,falls,perfects,time; lifetime coins remain. Fall resets current floor to checkpoint, motion/input, not reward frontier. Native fresh process resumes saved checkpoint, conserving frontier.
Daily UTC100..160 coins capped7day streak; daily seeded30floor run with16 first-pass perfects gives75 coins once current UTC date. Free style0; earned1/2/3 priced75/150/250. Optional starter style4+500coins once; collection styles5..7; no gameplay advantage/forced ads. Native store prices only; rewarded50coins/fiveUTCday/60sec/callback-only. Own app/product IDs required; retain test-ad mode until approved privacy URL/real acceptance.

## Gameplay and rendering
CharacterController verified6.2speed/9.2jump/22gravity, authored black hoodie/cyan crest/shoes with actual limb animation. Industrial beveled panels, metal surface, gold route edges and cyan underside. Checkpointgold ring and final ring. Deterministic stationary checkpoints. Moving floors only indices6/11/16/21/26, sinusoidal lateral travel .7m at .7..1.0rad/s, clock freezes when modal/focus/pause; motor carries grounded platform delta before own movement. Ordered transition must remain reachable across whole moving range. Cosmetic skyline no colliders. New generated sunset panorama/metal atlas/icon committed with provenance; real optimized modular city geometry, own audio/VFX/bloom.

## UI and validation
Compact pause/stage/progress/score/coins HUD, left joystick/right jump, state-driven checkpoint overlay, home/towers/daily/styles/shop/settings, DE/EN, reduced motion/high contrast/sound/haptics. >=48logical-point controls, native safe areas and largest nonreserved camera viewport. Clear input/timers/platform clock for modals/focus/pause. Native C# callback probes are not actual OS background proof.
Pure red-green rule/save/timezone/reward tests; geometry/reachability/moving-delta/input/raycast/min-size tests; real controller29 jumps in each tower, natural fall tocheckpoint/replay/Continue/Retry/movingcarry/freshprocess restore; native compact/iPad/officialDuo genuine available poses,30-minute real-time soak and signatures. Physical thermal/haptics, real store/ad/consent and inaccessible genuine Duo inner remain honest open gates. Internal TestFlight and central-key Android only, no public publishing.

## Backup
All Assets/metas/Packages/ProjectSettings and authored graphics tracked/pushed with remote tree verification before regenerable caches removed. Retain final IPA/AAB/archive and QA evidence.
