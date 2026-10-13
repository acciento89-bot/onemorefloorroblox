# One More Floor — native iOS candidate validation

User "Go"/"Weiter" authorizes continuing this game toward internal TestFlight after the approved approximate Concept refinement. No other game, public App Review submission or public store release is included.

Baseline: main `e0c7e71`, reviewed visual source `60bdebddf7fa68dd10fa52a898ddb69f02e1f709d5607b5475165a59e71e91af`, Unity6000.6.4f1, Xcode27.1/27A9275. User-owned historical Concept deletion remains unstaged/unrestored.

## Initial native validation and correction

Unity simulator export1 succeeds with14,711strict checks/zero collected errors and native SDK metadata. Xcode ARM64 iOS Simulator build1 succeeds. A fresh owned SE3 iOS26.5 simulator produces actual iOS menu, character/world/style and orientation images. The managed iOS command-line arguments do not enable the existing harness; its supported environment switches enable it correctly. Thus the command-line-only GPU A/B probe is not claimed as an iOS test; original Mac pixel controls remain separately identified.

The real native Showcase fails `portraitde compact lower risk sheet`. The iPhone’s2×point density and CanvasScaler reference390×844 yield approximately1.744Canvas scale on750×1334pixels. The49point minimum buttons require approximately56.2designunits, so the old `target*2+210` sheet exceeds its320unit contract. The ordinary desktop target52 gave314units and did not reproduce this. A screenshot and strict FAIL are preserved under `Builds/FloorIOS-20261013/ShowcaseNativeFailed1` and `native-risk-red.png`.

The smallest correction caps the existing sheet at320portrait/230wide and retains its safe-area bound, real button sizes, content and derived viewport. The affected SE3 viewport retains approximately127designunits for approximately104units of summary/comparison/spacing. No test limit is increased or disabled. A focused read-only reviewer finds no important correctness issue; native rerun remains required.

Export2 succeeds with the strict14,711check native suite and zero collected errors. Final native rerun, independent save restart, signatures/archive and actual upload status will be recorded separately in the candidate receipt. No initial failed or pending receipt is represented as passed.

## Environment and device coverage

Xcode is selected per command rather than changing the user's global developer directory. AdMob native SDK and signing identities are available. Test ads remain enabled; actual purchase/restore/consent/revenue acceptance is not claimed. At discovery there is no physical iPhone, only simulated devices. The user has been asked to connect an unlocked trusted iPhone and sign in to the expired Safari App Store Connect session. Native simulator evidence cannot establish physical FPS, haptics, thermal behavior or human touch feel.

Only the owned temporary simulator `A7F7C758-8037-458C-AC35-DA560F685D95` may be terminated/shut down. User simulators, production preferences and existing archives remain intact. All final captures must be copied before cleanup.

## Corrected actual iOS results

Exact tested native2source `cc9256e1a180306ef24019c58cbd2fd89283896d897fe55ce8253b168fa3db5e`; ARM64 simulator bundle `f4fc49e205c32b8ca7005fe642f46173b5b92fb658dac612e9be26d105ac2b3c`. Corrected strict native export14,711checks/zero errors and Xcode build succeed. Fresh native Showcase2,427checks covers DE/EN, actual750×1334and1334×750screens, full lobby/world/style/live-star/parent Back/risk/bank pages and real tutorial transfers; zero collected runtime errors. Original failing compact-sheet assertion now passes, comparison remains visible and target sizes are unchanged.

Full native `ELEVATOR_QA_PASS checks=8763` performs87real guided transfers across all three towers, miss/checkpoint recovery, exactly-once banking, pause/resume, fixed camera/framing and rotation. A genuinely fresh process then returns `ELEVATOR_RELOAD_PASS checks=6`, restores the pending checkpoint onto its tower balcony, banks the pending amount once and rejects repeated payout. Logs contain no harness failure or exception in these corrected runs. Data/captures copied to `ShowcaseNative2`/`FullNative2` before the owned simulator is shut down.

Device release export5 succeeds with the same14,711check strict suite. Explicit invocation uses `-buildNumber5 -versionName1.0`; the generated device Info.plist contains version5, while the Unity source files remain byte-identical to the tested native2input manifest. Version arguments are build-only and were not persisted to ProjectSettings. The IL2CPP release variant is independently archived; no simulator binary is distributed as an iPhone build.

Actual unchanged-Concept/iOS comparisons: `/Volumes/SSK SSD/Kamilunavo/Artifacts/FloorIOSComparison-20261013/Final1`. Exact-source visual review remains passed under the user's subjective approximate target, with simpler model/material detail disclosed. No numeric90% similarity or physical-device performance is asserted. Release/archive/upload status is recorded in `QA/floor-ios-candidate5-20261013.json`.

## Signed candidate and actual distribution state

Xcode release archive and local App Store export both succeed. Real IPA118,481,166bytes/SHA256`3b0a26fed6f1d0c052b689ad1a5f28c3cb3fa0d646e1e6207d695bff78e8c363`; deep strict signature valid, Apple Distribution/teamTKG684N5GL, get-task-allowfalse, bundlecom.kamilunavo.onemorefloor/version1.0/build5, correct own AdMob application declaration. Export explicitly enables TestFlightInternalTestingOnly. IPA: `/Volumes/SSK SSD/Kamilunavo/Builds/FloorIOS-20261013/Distribution5/OneMoreFloor.ipa`; original `.xcarchive` is retained beside it.

**Not uploaded to TestFlight.** Final device discovery still observes zero physical iPhones; Safari App Store Connect remains at Apple Account/E-Mail sign-in. The user-facing async requests to connect/trust an unlocked iPhone and sign in remain unanswered. Preparing/exporting the concrete candidate is complete; the prior stated physical check and authenticated TestFlight assignment cannot be completed from this environment. No upload attempt, public App Review, fake delivery status or live provider transaction. Native simulator readiness is not promoted to physical-device readiness.
