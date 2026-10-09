# One More Floor

Native iOS + Android elevator timing arcade game by Kamilunavo.

## Current gameplay
One thumb controls a guided transfer between industrial elevator decks above a neon city. The player chooses the launch time; the moving landing bay must line up at arrival. The camera rotation stays fixed. A visible arc and timing feedback help judge the window.

- Three 30-floor towers and a deterministic daily route.
- Checkpoints every five floors; a miss returns immediately to the last checkpoint.
- Perfect chains and bonus floors build pending round coins.
- At checkpoints: bank coins once, or risk another floor for a higher multiplier.
- A miss loses only pending round coins; wallet, bought items and checkpoint remain.
- DE/EN, safe-area layouts, wide timing action, sound/haptics/accessibility settings.
- Existing optional rewarded videos and permanent design purchases.

## Native project
Unity6000.6.4f1 / C#. iOS and Android bundle ID: `com.kamilunavo.onemorefloor`.
The historical repository name does not describe the runtime. No Roblox runtime.

## Current change and validation
Approved design: [Elevator timing and risk](docs/superpowers/specs/2026-10-09-elevator-timing-risk-design.md).
Implementation progress and actual native/internal delivery evidence: [V1 ledger](docs/ONE-MORE-FLOOR-V1-LEDGER.md).
Older parkour tests and build1 delivery entries describe the previous candidate and do not validate this redesign.

Editor validation: `FloorValidation.ValidateAll`. New player QA: `-qaElevator` (development builds only, isolated save). Previous `-qaFloor` harness is retained only for the historical parkour build.
