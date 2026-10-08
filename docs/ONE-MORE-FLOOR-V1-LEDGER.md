# One More Floor V1 Ledger

Status: `[ ]` open · `[~]` implemented/not device verified · `[x]` verified · `[!]` blocked

## P00 Reset/product
- [x] P00-T01 native iOS/Android direction
- [x] P00-T02 portrait concept lock
- [x] P00-T03 checkpoint/failure rules locked
- [x] P00-T04 legacy runtime removed from current tree
- [x] P00-T05 bundle IDs locked

## P01 Unity foundation
- [x] P01-T01 Unity/C# structure
- [x] P01-T02 editor Main-scene bootstrap
- [x] P01-T03 portrait + 60 FPS
- [x] P01-T04 safe-area Canvas
- [x] P01-T05 first editor compile
- [ ] P01-T06 iOS dev build
- [ ] P01-T07 Android dev build

## P02 Controls/camera
- [x] P02-T01 touch joystick
- [x] P02-T02 jump action
- [x] P02-T03 editor keyboard fallback
- [x] P02-T04 player-controlled orbit camera
- [~] P02-T05 phone ergonomics
- [ ] P02-T06 physical iOS verification
- [ ] P02-T07 physical Android verification

## P03 Floor gameplay
- [x] P03-T01 30-stage vertical slice
- [x] P03-T02 ordered stage recognition
- [x] P03-T03 moving-platform variation
- [x] P03-T04 checkpoint every 5 stages
- [x] P03-T05 instant fall recovery
- [x] P03-T06 Continue action
- [x] P03-T07 Retry-current-checkpoint action
- [~] P03-T08 full 30-stage runtime balance
- [ ] P03-T09 final pattern library/hazards

## P04 UI
- [x] P04-T01 Stage/progress HUD
- [x] P04-T02 Score HUD
- [x] P04-T03 Coins HUD
- [x] P04-T04 checkpoint panel
- [x] P04-T05 mobile controls
- [ ] P04-T06 pause/settings
- [ ] P04-T07 DE/EN
- [ ] P04-T08 reduced motion

## P05 Art/game feel
- [~] P05-T01 neon palette/material foundation
- [~] P05-T02 procedural skyline foundation
- [ ] P05-T03 final runner character/animations
- [ ] P05-T04 authored platform kit
- [ ] P05-T05 checkpoint VFX
- [ ] P05-T06 final lighting/post FX
- [ ] P05-T07 screenshot-quality gate

## P06 Persistence/progression
- [ ] P06-T01 versioned save
- [ ] P06-T02 best stage/tower
- [ ] P06-T03 coins
- [ ] P06-T04 unlocks/cosmetics
- [ ] P06-T05 migration tests

## P07 Retention/monetization
- [ ] P07-T01 daily challenge
- [ ] P07-T02 cosmetics
- [ ] P07-T03 optional convenience products
- [ ] P07-T04 StoreKit sandbox
- [ ] P07-T05 Google Play Billing sandbox
- [ ] P07-T06 restore/retry receipts

## P08 QA/release
- [ ] P08-T01 EditMode tests
- [ ] P08-T02 PlayMode checkpoint/fall tests
- [ ] P08-T03 iPhone safe-area matrix
- [ ] P08-T04 Android aspect matrix
- [ ] P08-T05 30-minute stability
- [ ] P08-T06 performance/thermal pass
- [ ] P08-T07 App Store package
- [ ] P08-T08 Play Store package
- [ ] P08-T09 TestFlight RC archive + upload
- [ ] P08-T10 TestFlight processing + internal tester assignment
- [ ] P08-T11 TestFlight install/smoke test on physical iPhone
- [ ] P08-T12 staged release

## Next open task
P01-T05: Unity compile/import and first Play Mode verification.


## Unity 6.6 bootstrap verification - 2026-10-06
- [x] Project imported and compiled successfully with Unity 6000.6.4f1.
- [x] Canonical Assets/Scenes/Main.unity generated and registered in Build Settings.
- [x] iOS and Android application identifiers are configured in PlayerSettings.


## Mobile platform build verification - 2026-10-07
- [x] Android IL2CPP development APK builds successfully with Unity 6000.6.4f1.
- [x] Android manifest verified: application ID `com.kamilunavo.onemorefloor`, versionName `1.0`, versionCode `1`.
- [x] Unity iOS Xcode export builds successfully.
- [x] Generic iOS device Debug build succeeds in Xcode 27.0 with automatic signing.
- [x] Code signature verified: identifier `com.kamilunavo.onemorefloor`, Apple Team `TKG684N5GL`.
- [ ] Store-ready 1024x1024 app icon and final release/archive validation remain release tasks.
- [ ] Local iOS Simulator QA is blocked by the currently installed CoreSimulator runtime mismatch; device builds are not blocked.

##2026-10-08 autonomous completion
Task1 e401052: actual Unity pure rule suite5371 PASS after missing Core types RED.30floors/29jumps,CP5/10/15/20/25/30, ordered/replayed landing rewards, stars/unlocks/daily UTC/save schema/reachability covered. Actual bundledMono negative-offset UTCdaily source probe PASS(PST).
Task2 in progress: adapted own verified UI/input/camera/optionalSDK/native-safe-region/geometry helpers from Rising Steps; generated actual panorama/industrial atlas/icon, authored chamferedplatforms/optimizedneoncity/blackhoodie cyancrest. Editor rules/UI/input/geometry/lifetime/compactscroll regressions7470 plus32moving-clock checks PASS. ActualMaccontroller87advances/movingcarry/checkpointretry still pending; no runtime/build acceptance yet.
Ruling: native SDK adapters introduced inTask2 to compile actual shop hooks — avoids test-only mock storefront — cost if wrong: dependency resolution may delay art QA. Own AdMob appIDs/catalogs remain required before native internal delivery.

Own unpublished AdMob apps confirmed in actualaccountUI: iOS~3774486179/reward9322326323,Android~7462449743/reward3904594098, publisher8944085355624754. Reward50Coins, voluntary rewarded format, no partner bidding/forced interstitials. Internal official test units remain enabled. Approved privacy URL/live consent, actual reward completion and genuine purchases still open.

## Actual desktop gameplay checkpoint
MacQA3 PASS183:87 actual CharacterController floor advances and3tower completions, stationaryCP5natural-fall recovery/replayed floor5..7rewardguard/one-tapRetry, grounded movingcarry/modal clock/resume, freecamera/pointerownership/lifecycle/modal/min48controls/save/syntheticdivision. Screenshot gameplay-floor2/7/16, home/CP/completion/settings/styles/shop retained in task work/floor-mac-qa3. Mac orientation request does not resize nativewindow; no iPhone orientation/FPS acceptance claimed.
Separate carry Move originally reset isGrounded before jump check; actualQA1 failed outgoingmoving floor7, isolatedrerun floor12. Single final CharacterController.Move(combined movement + carry) fixed all87advances inQA2/QA3. QA2 additionally exposed a harness assumption (portraitMac achievements fit rather than clip); QA3 verifies drag clamps fittingcontent, later native landscape must verify clipping.
Ruling: bestfloor/bestscore add required MasterPlanP05/P09 career progression omitted from initial spec — two compatible schema1 fields; cost if wrong: migration/normalization needs further coverage. Actual missing-field RED, rule suite GREEN includes saved best values.
Native iOS/device/SDK runtime,30minute soak,actual purchase/reward/consent andinternaldelivery remain pending; QA3 predates the harmless career-field/CP-sound additions and latest editor whole-suite runs separately.

## Native QA and own commerce — 2026-10-08 13:10 Berlin
- Full source backed up main151b6921061c3a66b61f34daf8a5d1b467d819a2; allAssets/Packages/ProjectSettings tracked, remote verified before deleting Library. Native SDK simulator QA2 exported with7502editorchecks/23uniquecommercechecks and actual Xcode27.1BUILD SUCCEEDED. Original first Unity export attempt hit native Mono asynchronous domain-reload crash; unchanged immediate retry succeeded; crash report retained in task evidence.
- Actual SE3 iOS26.5 QA2:185PASS,87actual controller advances/3towercompletions, moving carry maximum drift0.003484488m/cumulative travel0.05295312m; natural fall/replay/Retry/lifecycle/modal/scroll/min48logicalpoints, true portrait+landscape. QA1 fresh-process reload restored floor5/index4. Task evidence work/floor-native-compact2 and floor-native-compact1-reload.log. No physical thermal/haptic acceptance inferred.
- Ruling: oscillation movement proof uses cumulative travelled distance plus maximum player-relative drift, rather than net endpoint displacement — sine movement can return to its origin, and identical QA1 binary rerun passed185 — cost if wrong: QA sensitivity; native maximum drift remains bounded. Shipping motor unchanged.
- Actual own AdMob apps iOSca-app-pub-8944085355624754~3774486179/reward9322326323; Android~7462449743/reward3904594098. Voluntary50Coins reward units;InternalTestAds=true; approved privacyURL/liveconsent/genuine reward still open.
- ASC6819872214 Starter AppleID6820478031/productcom.kamilunavo.onemorefloor.starter/nonconsumable GermanyEUR1.99,175territories,DEStarterpaket+ENGStarterPack. NeonCollection AppleID6820486124/productcom.kamilunavo.onemorefloor.neoncollection/nonconsumable GermanyEUR2.99,175territories,DENeonkollektion+ENGNeonCollection. Both actual final Gesichert verified; draft only, no public submission. Review screenshots and actual sandbox buy/restore gates remain.
- Actual TestFlight page says Keine Builds, so first internal build1 is available. No public release authorized.

## Whole-change review correction pass
- Fresh independent reviewer4bcc71a..151b692: noCritical. Delayed consent presentation and session-locked store failure corrected in one pass: actual closed-shop/active-shop presenter fixture and bounded reconnect state transitions RED(missing gate/reconnect)→GREEN. StoreController event subscriptions installed once, reconnect action becomes available after5s, never overlaps its pending async connection. Genuine SDK failure/retry still requires native store acceptance.
- Moving-carry negative control strengthened after reviewer finding: stationary player at0.053m or0.21m excursion must fail; actual0.0035m drift at0.21m excursion passes. RED missing negative control→GREEN; native sampler now seeks sufficient excursion and checks relative drift.
- Duo actual failure exposed upward side-corner counting as a floor landing while player feet were~0.9m below the deck. Extracted original contact predicate, actual17.81/18.7fixture RED→height guard GREEN; valid top and rejected lateral contact covered. Full native rerun pending.
- Final editor suite7509PASS and33uniquecommercechecksPASS (work/floor-final-review-green.log). No second reviewer dispatched per executing-plans; fixes tested directly.
- Deferred minor: route hint labels current floor as next; accurate current-stage HUD remains. No public release readiness claim.
- Review set aside genuine SDK commerce/privacy/reward, unfinished native matrix/soak/signatures, physical thermal/haptics/background and visual acceptance; these remain separate evidence gates.
