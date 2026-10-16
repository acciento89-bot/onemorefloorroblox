# One More Floor

## Current review status — 2026-10-16

Apple renewed the **4.3(a) Design – Spam** rejection of 1.0(7) at 20:04 Berlin. After reading all five prior messages and receiving explicit owner approval, a clarification requesting experienced reassessment, specific duplication details and clarification of the account warning was sent at **20:30 Berlin**. The complete 2,712-character body was read back exactly as one published message. **The app remains REJECTED; no new binary or resubmission was made.** The earlier continuation-of-review acknowledgment was not approval. [Delivery evidence](docs/QA/floor-renewed-spam-reply-20261016.json).

## Icon correction — 2026-10-15

Original accurate icon is integrated, all native icon sizes checked and signed iOS1.0(7) uploaded. Apple processing is VALID; the Apple-processed store icon matches the new artwork exactly. **Build1.0(7) and all three IAPs were resubmitted on15October at22:01Berlin; Apple initially confirmed WAITING_FOR_REVIEW. The renewed rejection of16October is recorded above.** Owner Safari authentication now works; previous notes/nine-answer conversation were preserved and the additional Icon2.3 correction was appended. [Resubmission readback](docs/QA/floor-icon-resubmitted7-20261015.json). [Current proof and limits](docs/QA/floor-icon-release7-20261015.json). Gameplay and monetization are unchanged.


Native iOS + Android elevator timing arcade game by Kamilunavo.

## Current gameplay
One thumb times a jump from a single vertical brass elevator onto a fixed tower balcony. The carriage rises beside thirty numbered stone floors; a successful jump visibly ends on the tower before the same carriage prepares the next floor. Prediction and arrival share one deterministic forecast. The camera rotation stays fixed; a visible arc and timing feedback help judge the landing window.

- Three 30-floor towers and a deterministic daily route.
- Checkpoints every five floors; a miss returns immediately to the last checkpoint.
- Perfect chains and bonus floors build pending round coins.
- At checkpoints: bank coins once, or risk another floor for a higher multiplier.
- A miss loses only pending round coins; wallet, bought items and checkpoint remain.
- DE/EN, safe-area layouts, wide timing action, sound/haptics/accessibility settings.
- Existing optional rewarded videos and permanent design purchases.
- Distinct lift lobby with tower preview, fixed Continue/Play, and a skippable/replayable timing practice that preserves the saved run and wallet.

## Native project
Unity6000.6.4f1 / C#. iOS and Android bundle ID: `com.kamilunavo.onemorefloor`.
The historical repository name does not describe the runtime. No Roblox runtime.

## Current change and validation
Latest iOS follow-through: [Native iOS candidate 5](docs/FLOOR-IOS-VALIDATION-20261013.md). Corrected native compact decision sheet; actual SE3 simulator Showcase, full87transfers and fresh-process save reload pass. Signed internal-only IPA/archive prepared; TestFlight not uploaded pending real iPhone check and App Store Connect sign-in.

Latest coordinated polish: [Menu, marble and softer character highlights](docs/FLOOR-POLISH-20261013.md). Fresh strict native Editor/build/player/GPU evidence passes, and actual Concept comparison accepts the revised subjective approximate-match target. No numeric90% similarity is claimed; material/model finish remains simpler. Physical iPhone and native release checks remain required before TestFlight.

Latest user proportion correction: [Rectangular cards and slender cabin](docs/FLOOR-PROPORTIONS-20261013.md). Actual2.1:1portrait cards,36%slimmer same-height carriage with matched rails/deck and fresh native player/GPU evidence. This is historical proportion1 evidence; the subsequent polish uses the revised approximation target.

Previous native menu assets: [World cards, lettering and coin](docs/FLOOR-MENU-ASSETS-20261013.md). Original menu paintings, live stars, derived serif logo/titles and original currency sprite are connected to the actual UI; final4 strict native menu/GPU checks pass. Overall Concept gate remains failed.

Previous native graphics pass: [Original reflections and composition](docs/FLOOR-REFLECTION-COMPOSITION-20261013.md). Character prominence, original-room reflections and continuous lobby foreground are improved; overall Concept approval remains blocked.
Current literal Concept correction: [One elevator to fixed tower balconies](docs/FLOOR-LIFT-TO-TOWER-20261012.md). Earlier economy and checkpoint design: [Elevator timing and risk](docs/superpowers/specs/2026-10-09-elevator-timing-risk-design.md).
Latest native Mac Unity Editor / consumed sculpted assets / closed marble lobby / opaque child menus: [Original assets and native Art Studio](docs/FLOOR-UNITY-ART-STUDIO-20261012.md). Graphical acceptance remains failed. Earlier URP/gold UI/character/texture batch and bounded evidence: [Concept lighting and UI rebuild](docs/FLOOR-CONCEPT-URP-UI-20261012.md). Earlier [environment/lobby revision](docs/FLOOR-ENVIRONMENT-REBUILD-20261012.md) is historical. Overall graphical acceptance remains failed; no TestFlight upload.
Implementation progress and actual native/internal delivery evidence: [V1 ledger](docs/ONE-MORE-FLOOR-V1-LEDGER.md).
Older parkour tests and build1 delivery entries describe the previous candidate and do not validate this redesign.

Editor validation: `FloorValidation.ValidateAll`. New player QA: `-qaElevator` (development builds only, isolated save). Menu/tutorial QA: `-qaElevatorShowcase` (development only, isolated save) captures DE/EN and actual portrait/landscape dimensions and plays timing/Perfect/risk/bank practice. Current evidence is a Mac player, not physical iPhone validation. `-qaConceptRender` additionally probes real light/bloom/focus pixels. Previous `-qaFloor` harness is retained only for the historical parkour build.
