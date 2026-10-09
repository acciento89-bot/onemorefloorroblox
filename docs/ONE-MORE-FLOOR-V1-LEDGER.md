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
- [x] P08-T10 TestFlight processing + internal tester assignment
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

Shared adapter audit: own RisingSteps and PerfectDrop source still contain the same unguarded delayed-consent callback and initialization-only StoreController connection. Their delivered internal builds are test-ad builds with genuine commerce gates open; queue equivalent regression-backed fixes and refreshed internal binaries before treating monetization as accepted. This finding does not invalidate already recorded source/signature/gameplay evidence.

## Latest native correction build QA4
Main611360ddbc3beb8074b1cd510c9e2634d0e083e1 includes development-only landing/jump diagnostics, same shipping behavior as reviewed correction8168bd4. Actual iPad11M5 and official Duo outer display each185PASS, all87 genuine CharacterController floor advances across3towers. Latest corner trace correctly rejects an upward side contact with feet below the platform; subsequent descending top contact accepted. Work/floor-native-ipad4 and floor-native-duo4 retained. Earlier DuoQA2 andQA3 failures retained; no claim of hitch-free physical performance. Actual Duo inner display3 remains black; inaccessible fold pose stays open. Compact final QA4 runs full matrix followed by actual1800s soak; pending completion at14:20Berlin.
Full source remote HEAD verified611360d with clean Assets/Packages/ProjectSettings before deleting regenerated QA4 Xcode export. Latest QA4 native player267MB and all evidence retained. Default device SDK and release export remain next steps.
App Store Connect session expired14:17Berlin; existing stored passkey belongs to a different account without ASC access. Reauthentication request queued for user's return. Existing saved products unaffected; internal upload may remain possible through authenticated Xcode, but final group assignment requires restored ASC session. No fake delivery status.

FinalcompactQA4 full matrix+real1800.206ssoak PASS198.106237frames,p95frame16.977ms; allocated126004118→126423373bytes,peak126486885;0errors. Evidence work/floor-native-compact4-soak/soak.json/PASS.txt and all30phase captures copied before terminating process. This is simulator/host rendering, not physical thermal acceptance. DefaultSDK restored toDevice988 for release; QA shipping motor remains reviewed correction.
Latest QA4 quick route35actual advances/5restarts PASS then genuinely fresh bootstrap restores saved checkpoint index4/floor5. work/floor-native-compact4-reload.log and copied RELOAD-PASS/preferences prove this. Own temporarySE3clone164F5671 deleted only after all captures/metrics/preferences copied; user devices remain. Simulator player retained; heavy device release build starts after native QA shutdown.

## Internal iOS build1 delivery —14:50Berlin
Actual device release export editor7509/33uniquecommercePASS, nativeARCHIVE SUCCEEDED+EXPORT SUCCEEDED. Final outputs/OneMoreFloor-1.0-build1-AppStore.ipa88478368bytes SHA2565086689b2443a80c180f4729157babe07cdfc3faaae7b1867321fea3ca82d911. Deep strict signature valid/satisfies requirement, AppleDistribution/teamTKG684N5GL/get-task-allowfalse; bundlecom.kamilunavo.onemorefloor/version1.0/build1/ownGADapp~3774486179. Source inputs backed up1ae10748de89c6ebcac22e557852f8584a79250d (reviewed shipping motor unchanged).
Authenticated Organizer TestFlightInternalOnly upload accepted; actual Uploaded to Apple/Today14:50/build1 confirmed. Missing vendor UnityRuntime.frameworkdSYM UUID392D7A7F-6A4F-3A7A-8788-4089FA96FF38 warning retained; no ownUnityFramework symbol issue. TestFlight processing/encryption declaration/group1tester still awaiting restored ASC browser session; do not claim ImTest yet.
Private/unpublished GitHub draft onemorefloor-build1-signing-input durable IPA backup verified; repo itself is public, draft never published. Final archive/private/tmp/OneMoreFloor-Release-Build1.xcarchive retained. ReproducibleDerived/export/decodedverification removed after source+final+proof backup. Central signed Android next.

## Central Android build1 delivery
Native Unity Android release built successfully7509/33uniquechecksPASS. Default local Unity debug JAR wrapper removed onlyMETA-INF/ANDROIDD.SF,RSA,MANIFEST.MF; all573non-signature payload entries byte-identical. Actual bundletool manifest com.kamilunavo.onemorefloor/version1.0/code1, ARM64IL2CPP/release-debuggablefalse, ownAdMob~7462449743. Unsigned35457886bytes SHA2568ea4e7bdfc2933df8b78258b860c829253ec297b4c54178aed9b96d2c78cd613.
Source206e00393cf5983cd56d24216118b5cfbfd2147a includes portable AndroidResolverDependencies.xml; allUnityinputs tracked/pushed and remoteHEAD verified before Library removal. Central pinned signing workflow37782182983success, unsigned draftasset621871026.
Final outputs/one-more-floor-build1-signed/OneMoreFloor-1.0-build1-central-signed/OneMoreFloor-1.0-build1.aab35523375bytes SHA2567388a775791bae8168e580e51797f2ea282959c035c9223139b3aae8eeea4ff8. Actual jarsigner verified; universalcertificateSHA1BCF2337D41E617C03BCAE698C09D1523654BD790/SHA2567985BD6B33711BACA7E6BA722C2B3870EBBC802F7DB4A7BC1206BDAE51C4D5D6. Signedmanifestidentical/all573payload entriesidentical/sourceprovenanceexact. No key downloaded to Mac/no permissions expanded/no Play publication.
Private/unpublished durable draft406820427 holds IPA+unsigned input+final signedAAB. Required TestFlight group assignment still pending browser reauthentication; keep Task3 partial. Native physical thermal/haptics/trueDuo inner/genuineSDK commerce/privacy gates explicit. Continue Repair Empire while user away.


## Lossless archive storage, 2026-10-08

The final Xcode archive is preserved as `outputs/archives/OneMoreFloor-Release-Build1.xcarchive.tar.gz` in the delivery workspace. All 61 regular files were compared byte-for-byte with the original before removing the uncompressed temporary archive. SHA-256: `542fa1f62a0df0b1ee3a5832729d92e2186c9ef377c2d930da82416d89ce15de`. The matching proof manifest is beside the archive. Durable owner-only backup: unpublished draft release asset `622090166`; the server digest matches. Restore with tar extraction before opening in Xcode. Original `/private/tmp` archive paths in earlier entries are historical. Unity source, signed binaries and QA evidence remain preserved.

## Internal TestFlight assignment, 2026-10-08

The restored owner session shows app6819872214, build1.0(1), build ID66f7a81d-80a6-421c-b57e-8a30b5f7ead6. Encryption questionnaire saved for the existing OS-only baseline. Internal group **Kamilunavo Intern** (cf3964c5-35e5-4eac-b68c-9bc7845d0128) has automatic distribution, one existing owner/admin tester and one build. Build status **Bereit zum Testen**; tester status **Eingeladen**, 8October2026. German movement/landing/checkpoint/rotation/save/audio/haptic test notes saved and verified. Local delivery receipt work/floor-testflight-assignment-20261008.json contains no private email. Actual invitation acceptance, physical installation, genuine SDK commerce, approved privacy URL and inaccessible Duo inner display remain open; no public release or App Review submission. Earlier session/processing blockers are historical.

## User correction and distinct game direction,2026-10-09

Physical user reports unintended camera changes during jump/movement and poor portrait playability; landscape is somewhat better. Prior runtime checks assert stored OrbitCamera.Yaw and control bounds but do not verify rendered rotation throughout an actual jump or that the next actionable deck stays in view. Source still uses LookRotation toward the moving focus from a damped camera position, the same rendering mechanism reproduced/fixed in Rising Steps; exact OneMoreFloor native reproduction not yet claimed. User selected **elevator, timing and risk** instead of the Rising-like free parkour loop. New fixed-camera/one-thumb/30-floor/checkpoint/risk design written at docs/superpowers/specs/2026-10-09-elevator-timing-risk-design.md and queued for written review. Old P00-P08 records describe the previous candidate; no claim the new game principle is implemented or accepted. Existing source/binaries/purchases/QA evidence preserved.

## Approved execution,2026-10-09
User: "Wieso hast du dann nichts ausgeführt? Leg los" explicitly approves the concrete elevator/timing/risk design and immediate implementation. Execute inline without another plan approval prompt. New user-fixed-camera requirement supersedes legacy orbit instructions. Plan: docs/superpowers/plans/2026-10-09-elevator-timing-risk.md. Dependencies: rules own persisted pending reward; course owns deterministic clock; motor delegates timing input; HUD and fixed camera observe course. Existing stores keep profile/paused/event contracts. Native evidence remains required, previous parkour PASS is not acceptance of this game.

## Elevator integration checkpoint,2026-10-09
Rules RED missing ElevatorRules/fields then actual Unity GREEN10489; baseline legacy7509 and commerce33 remain GREEN. Initial runtime fixture missing transfer APIs RED; integration compiles, Mac development build3 succeeds. Actual isolated rendered player work/floor-elevator-mac-qa3 PASS1933:87 guided transfers/three30floor towers, actual arrival against moving bays, full quaternion constancy during flight, intentional mistimed arrival -> checkpoint5 with only pending loss, modal/focus input guard, min48 timing control. Mac orientation request does not resize native desktop window: true iOS portrait/landscape still pending. First build screenshots exposed inherited circular button and oversized label; wide RoundedPanel/25point cap fixed. First framing assertion ran before LateUpdate; final checks sample a rendered frame after camera follow. Native continuous framing remains required. Desktop QA can run in background with development-only focus callback setup, not a claim of physical lifecycle acceptance. Source after this proof adds pending-label outline, rejects voluntary menu opening during transfer, routes banked timing action to Home and makes checkpoint Back count as Continue; native rerun will include these. New mechanical tracks, only current/next decks active, city envelope moved outside fixed camera.

## Fresh whole-change review and single correction pass
Sole fresh reviewer8aafbab..6a4ea3d: noCritical/noMinor,7Important. All addressed in this correction pass: (1) preserve legacy paid reward frontier; (2) exact idle floor restored, durable in-flight marker; (3) restored checkpoint banking decision and idempotent Continue; (4) terminal payout/completion saved together, interrupted terminal state repaired; (5) exact0.78s arrival timestamp despite overshoot then carry for remaining frame; (6) full source/target corners, runner head and arc fitted into actual HUD-free viewport; (7) camera immediately reframes on recovery/restart/landing, no stale-position interpolation. Concrete pre-fix Editor fixture RED: paid frontier remints; actual landscape source corner viewport(0.57,0.16,7.64) overlaps HUD. Missing restore/in-flight/fit APIs RED; expanded actual Unity10533GREEN, original7509+commerce33GREEN and actual camera regressionGREEN. Overshoot fixture realm2/floor15/seed260908/launch0.04 with0.76+0.33s: old end-frame evaluation misses, exact arrival remainsPerfect.
Additional actual MacQA4 under concurrent native export failed floor10 due buffered touch dispatch on the next frame. New direct pointer dispatch fixture RED -> immediate Pressed callbackGREEN; shipping timing action now launches at pointer time. MacQA5 actual rendered player PASS2988: three towers/87transfers plus missed/replayed route, additional landscape-request transfers, every-flight-frame full route framing and quaternion, in-flight pause/resume stale-touch rejection, immediate restart frame, checkpoint decision saved for reload. Desktop native window remains portrait despite orientation request; landscape projection separately checked by actual Editor camera fixture. Fresh Macprocess same isolated key RELOAD-PASS5: pending checkpoint decision restored, exact payout, duplicateBank no mint. Evidence work/floor-elevator-correction-mac-qa5 and -reload. Latest native export includes all corrections and presentation-first close guard; Xcode simulator compilation pending.
Ruling: a process killed during a launched transfer recovers to its checkpoint and loses only pending round coins on relaunch; ordinary OS pause freezes/resumes, and idle reload preserves exact floor — prevents abort-to-avoid-loss exploit without serializing an entire flight — cost if wrong: OS termination during a paused flight is treated as a miss; wallet/purchases never lost.
Review set aside native device visual/DEEN matrix, genuine SDK buys/restore/consent/reward, physical haptic/thermal/performance, signing/internal delivery and subjective difficulty/onboarding. Native/signing/internal delivery remain required work; genuine-provider/physical acceptance remain external gates. No second reviewer; failures retained.


## Native elevator matrix, 2026-10-09
Source805d8aafae3aeb2eae48ceee7f1b43eb228e8990 native IL2CPP/Metal/UIKit player retained with49byte-identical file checks. Compact SE3 DE landscape/portrait PASS8632; 16ProMax EN portrait/landscape PASS8561. Each completes all3towers/87guided transfers, intentional miss, bank/restart, actual orientation, every-flight-frame full source/target/arc framing and fixed quaternion, engine-driven pointer callbacks, background pause/resume and stale input rejection. Fresh native processes each RELOAD-PASS5 restore checkpoint decision/pending reward and exact-once bank. Native portraits and landscapes visually inspected; no HUD/deck/button collision. Evidence work/floor-elevator-native-{compact1,large1} and respective -reload. This proves simulator gameplay and native lifecycle callbacks, not physical touch/thermal or genuine provider commerce acceptance. Owned temporary clones removed only after screenshots, logs, provenance and preferences retained. New shipping iOS/Android1.0(2) next; old build1 remains historical.


## Elevator native internal delivery1.0(2),2026-10-09
Shipping sourcebccdb23e38ee1627c189633b799a683e47683876 clean/pushed/remote verified before build cache cleanup. Device Unity full validation10533/elevator review/original7509/commerce33 PASS, native ARCHIVE SUCCEEDED and EXPORT SUCCEEDED. IPA88571549bytes SHA2568098b8f767aaa1e04c42c7d47d98d49697e0a7484e9e161254c9d849cfab0892; strict deep codesign verified, AppleDistribution/teamTKG684N5GL/get-task-allowfalse, version1.0/build2/ownGAD~3774486179. Development QA types absent from shipping generated C++. Full Xcode archive all61files byte-verified as outputs/archives/OneMoreFloor-Elevator-Build2.xcarchive.tar.gz SHA256da23df77a305ed1dadb26e6c07812a2c92bf4905e0a400f137aa0fc4f36fc2dc.
Organizer TestFlight Internal Only upload completed with known vendor UnityRuntime.frameworkdSYM warning UUID392D7A7F-6A4F-3A7A-8788-4089FA96FF38; ownUnityFrameworkdSYM present. Restored authenticated App Store Connect: platform-provided encryption declaration saved, build413baaed-6d93-4d91-9043-18a4b6d38ad5 actual row Im Test/Kamilunavo Intern/1invitation. Detail confirms1internaltester and saved German test notes describing new principle, timing/Perfect/checkpoints/orientations/reload and killed-flight pending-loss ruling. Receipt work/floor-elevator-testflight-build2-status.json. Physical install not claimed.
Native Android1.0/code2, com.kamilunavo.onemorefloor, release-debuggablefalse, ARM64IL2CPP/6ELF16KB alignment/ownGAD~7462449743. Only local debug JAR signature wrapper removed; all573payloads byte-identical. Pinned central signing run37961712409 succeeded; exact source/provenance/manifest/payload comparison and jarsigner verified, existing universal SHA1BCF2337D41E617C03BCAE698C09D1523654BD790/SHA2567985BD6B33711BACA7E6BA722C2B3870EBBC802F7DB4A7BC1206BDAE51C4D5D6 match. Final outputs/one-more-floor-build2-signed/OneMoreFloor-1.0-build2.aab SHA25614c1171833a303f95439c4827b068b17f45385c51f2d084c27b96390d0d40e21. No key downloaded or access expanded.
Unpublished draft408117133 onemorefloor-build2-elevator-timing retains finalIPA/fullarchive/unsignedinput/central-signedAAB and evidence; all4remote server digests verified. Source backed up in canonical main. Reproducible Unity Library, Xcode derived/export and verified duplicate archives cleaned only after source, final binary and lossless archive proof; native QA player/screenshots/logs/preferences retained. No App Review submission, public store release or Play upload. Existing genuine sandbox purchases/restores, ad completion/consent-provider acceptance, approved privacy URL, physical haptics/thermal/performance/Duo and subjective difficulty remain external gates. These do not prevent internal build testing. Sole fresh reviewer7Important fixed/regressed in805; no second review or new unrecorded ruling.
