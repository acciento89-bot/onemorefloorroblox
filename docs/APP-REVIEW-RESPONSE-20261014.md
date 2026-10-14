# One More Floor: App Review response, 14 October 2026

Status: sent in the existing Resolution Center thread at 2026-10-14T21:16:08.298Z. App remains REJECTED and the submission UNRESOLVED_ISSUES while awaiting a substantive reply. The rejected binary was not resubmitted.

## Review issue and scope

Apple cited guidelines 4.3 and 4.2.6, requested nine complete answers, and did not identify a comparison app or template. The human corrected an earlier overemphasis on comparison with their own projects. That is not the assumed rejection cause. Other first-party projects are disclosed only to answer Apple's explicit portfolio/component questions.

## Evidence and delivery

- App 6819872214; native Unity version 1.0 (6).
- Submission a5e537c0-a31a-47e3-8e0c-727590b3f99f; thread 54d208ae-7dea-378a-bddd-0a58da6be30e.
- Sent message cffb5f97-f134-40e9-888c-b5fb633cbe91; exact 3,653-character body independently read back.
- A four-page PDF includes all nine full answers and three actual iOS Simulator frames from the submitted production source. It is labelled as QA profile data, not concept art or physical-device performance evidence.
- Before sending, the draft attachment was COMPLETE with a matching source checksum. Apple promoted it to attachment 8ec50ae5-6a45-3c22-9397-b7d208996da1. Its published download is byte-for-byte SHA-256 identical to the local PDF.
- Native source and gameplay were not modified. Original user-owned deletion of docs/concepts/PRIMARY-CONCEPT.png is preserved.
- Testing claims distinguish the one installed internal tester/owner feedback from automated native checks. No large external beta, authenticated purchase/restore, live ad playback, or physical-device frame rate is claimed.
- First-party reuse is disclosed: support/input/UI/animation/audio/commerce code, seven sound/ambience cues and licensed fonts. No claim that every byte or character-rig concept is exclusive.
- Full original Apple message, code-sharing audit, sanitized delivery receipts, PDF and native source images remain in /Volumes/SSK SSD/Kamilunavo/Builds/FloorRejection-20261014.

## Sent message body

Hello App Review,

We are replying to your nine questions for One More Floor 1.0 (6), not simply resubmitting it. The attached PDF provides the full answers and native build images. Your message does not identify a comparison app or template; please specify one if a concern remains.

1. Purpose: a native 3D timing game. One moving elevator carries a character beside fixed tower balconies. A motion-derived landing forecast helps time the transfer. At five-floor checkpoints, bank the round's coins or continue for a higher multiplier. A miss returns to the checkpoint and loses unbanked coins; previously banked currency remains. Three 30-floor towers and a daily route are included.

2. Audience: casual mobile players seeking short, one-thumb timing sessions with readable forecasts and recoverable progress. DE/EN, a replayable tutorial, reduced motion, contrast settings and large controls support this use.

3. Specific value: a consistent spatial elevator-to-balcony timing skill combined with checkpoint decisions. We do not claim that timing or reward banking is new. Our activity differs from the dungeon RPG sold under the same title and from mixed-microchallenge games. The PDF gives concrete market context without assuming those are your comparators.

4. Testing: small owner-led pre-release testing, with one installed tester in the internal TestFlight group. Applied feedback changed the game to one elevator/fixed balconies, rebuilt the landing meter, narrowed the carriage, made rectangular world cards, corrected parent Back navigation and compact checkpoint sheets. Automated native iOS Simulator checks separately covered all 87 transfers across three towers, payouts, miss/recovery, fresh-process save/banking and DE/EN phone/iPad layouts. We do not present these as a large human beta study.

5. Standalone: this is not a set of separately published themed copies. All tower themes are in one binary. Other Kamilunavo projects have different central activities: block placement (Perfect Drop), player-steered platforming (Rising Steps), and service-van/jobs/workshop management (Repair Empire). They are mentioned for completeness, not as an already published suite or an assumed basis for your rejection.

6. Consolidation: adding this to another game would embed a separate timing simulation, transfer/boarding state machine, forecast UI and banking progression, rather than add a map or skin. No other app/account is needed to play; progress and currency are independent.

7. Shared first-party components: yes - mobile UI/safe-area/touch utilities, input/animation helpers, mesh/resource management, audio playback and commerce/reward/ad reliability code are reused/adapted across our development projects. Seven first-party sound cues/ambience files are shared with Rising Steps, and licensed fonts are shared. The PDF identifies roles and distinguishes Floor's dedicated gameplay and artwork. We are not claiming every byte is exclusive.

8. Third parties: Unity/URP/UGUI/TMP, Unity IAP, Google Mobile Ads/UMP and licensed OFL fonts provide standard services. No other developer's app source or purchased commercial app template was incorporated. Project-specific source/assets were authored under the owner's direction with coding/image-authoring assistance.

9. Content owner: this is Piotr Kaminski's own Kamilunavo product, submitted directly from the content owner's account. It is not a client/white-label app published on another content owner's behalf.

Please review the full attached answers and native evidence. If more differentiation is required, identify the specific overlap so we can address it precisely.

## Full PDF answers

This responds to the nine questions in the review message dated 14 October 2026 concerning guidelines 4.3 and 4.2.6. The game described below is the submitted native Unity build 1.0 (6). The review message did not identify a particular comparison app or commercial template. We are supplying product, authorship and testing evidence for that existing build, rather than resubmitting an unchanged binary without addressing the questions.

### 1. Function and purpose

One More Floor is a single-player 3D timing game. A character rides one vertically moving elevator carriage beside a tower with fixed balconies. The player observes the carriage's movement and the landing forecast, then taps one action to transfer the character onto the next balcony. The forecast and landing outcome use the same motion clock. A successful transfer ends with the character visibly standing on that fixed balcony; the same carriage then provides the next ascent.

There are three 30-floor towers and a seeded daily route. Later floors narrow the landing window, and tower motion rhythms differ. At five-floor checkpoints the player chooses between banking the current round's earned coins and continuing for a larger multiplier. A miss returns to the last checkpoint and loses unbanked round coins; previously banked currency and bought items remain. Banking is recorded once. The intended use is a short, readable timing-and-decision activity with recoverable progress. The application does not provide financial returns or real-money wagering.

### 2. Intended players

The intended players are mobile casual-arcade players who prefer one-thumb timing, a clearly indicated next action and short sessions that can be paused or ended at a checkpoint. They can learn one consistent transfer mechanic and then improve timing and risk decisions across the tower. German and English interfaces, a replayable tutorial, reduced-motion and contrast settings, and touch targets of at least 48 logical points support that use case. The product is a general-audience entertainment game, not a professional service or a client-specific application.

### 3. Specific value relative to existing games

We do not claim to have invented timing games or banking a run's rewards. The intended distinction is the complete interaction: transferring a visible character from a moving 3D vehicle to a stationary world-space landing, using a motion-derived landing forecast, then making checkpoint decisions without losing all prior ascent on an ordinary miss. This offers a consistent spatial timing skill with recoverable progress, rather than requiring continuous free movement or learning a succession of unrelated microgames.

For concrete market context, the App Store listing of the unrelated One More Floor by Joshua Myers (id6772283575) describes a dungeon RPG with combat, heroes and loot. The listing of +1 Floor by Carlo Pelliccioni (id6796015248) describes six kinds of short challenges. Our submitted game instead centres on the recurring elevator-to-balcony transfer and its timing/recovery model. These are examples we checked, not applications identified by your review message. We welcome the name of any specific comparison app or the overlapping functionality you consider insufficiently differentiated so that we can address the concern precisely.

### 4. Pre-release testing and feedback applied

Testing has been a small, owner-led pre-release iteration with direct play/screenshot feedback. The One More Floor internal TestFlight group currently contains one installed tester. We are not representing this as a large independent beta cohort, nor claiming a physical-device performance study from simulator runs.

Specific feedback incorporated before build 6 includes: (a) changing the interaction to one elevator carrying the player to fixed tower balconies; (b) rebuilding the landing meter around the actual transfer forecast; (c) making world cards rectangular and the carriage narrower while retaining its height; (d) returning child menus to their parent instead of forcing a gameplay resume; and (e) correcting a compact-phone checkpoint sheet that was too tall. These changes are present in the production source and its native captures.

Separately, automated native IL2CPP/Metal iOS Simulator validation covered all 87 transfers through the three 30-floor towers, all three tower payouts, deliberate misses, checkpoint recovery, pause and fixed-camera framing. A fresh-process reload checked checkpoint persistence and exactly-once banking. Native menu/tutorial checks covered German/English, iPhone SE3, a large iPhone and iPad layouts, portrait/landscape, world/style selection, Shop and parent Back navigation. These are engineering checks, not additional human beta testers.

### 5. Standalone product and other projects

One More Floor is a standalone product. It is not a location-, customer- or theme-specific edition of one repeatedly published game. Its three tower themes and cosmetic designs are already contained in the same binary. No other app, shared account or shared currency is required to play it.

For completeness in answering the portfolio questions, separately developed Kamilunavo game projects include Perfect Drop (moving-block placement, overlap and remaining stack area), Rising Steps (player-steered 3D running/jumping across realm courses), and Repair Empire (service-van driving, customer jobs, equipment and workshop progression). These are development/review projects, not a claim that a suite of similar games is already publicly released. Their central inputs, world state and progression differ from this game's elevator timing and bank-or-continue loop. The rejection is not being interpreted as an identified comparison between our own projects.

### 6. Why this is a separate application

A game compilation could technically contain different games, but One More Floor would require embedding a separate timing simulation, transfer/boarding state machine, fixed-balcony architecture, forecast interface and checkpoint banking economy. It is a complete activity rather than an extra map, skin or premium feature of block stacking, freely controlled platforming or a service-business game. Those other products do not expose this elevator game as their core interaction. We chose an independent install and progression for the specific audience described in answer 2. All One More Floor tower themes are consolidated within this application, and its in-app purchases are optional cosmetics or ad removal.

### 7. Developer-owned code and assets reused across projects

Yes. We reuse our own mobile-game support code across the development projects mentioned above, and disclose this beyond the standard third-party frameworks. The shared or closely adapted components are: UI construction, safe-area fitting, display/touch metrics and reserved-region handling; basic input, character motion/marker and humanoid animation helpers; procedural-mesh/palette/resource-lifetime utilities; background-music playback; ad-load generation and store-reconnection guards; and product entitlement, reward-limit, transaction-idempotency and StoreKit adapter patterns. Their roles are rendering, platform adaptation, input support, audio and commerce reliability. The currently active Floor gameplay is its dedicated elevator transfer mode.

A byte-level resource audit also found seven reused first-party cues/ambience files with Rising Steps: Jump, Landing, Perfect, Fall, Portal, Menu and SkyAmbient. FloorCity is a separate score. Barlow and Cinzel font files are licensed third-party resources shared across projects, with licence notices retained. The audit did not find byte-identical authored PNG/JPG artwork between One More Floor and these three projects. This is a limited resource-audit statement, not a claim that every design technique or character-rig concept is exclusive.

The game-specific production path is implemented by FloorCourse, TowerLiftMotion and ElevatorRules: one moving carriage, fixed target balconies, motion-derived prediction, transfer/boarding, landing classification, reward frontier, banking and checkpoint recovery. Its carriage/tower assets, architectural lobby and scenic selection cards form the Floor presentation. Shared support code and a publisher art style do not make those other projects use this game's core interaction.

### 8. Third-party app code, frameworks and content

We have not incorporated another developer's app codebase or a purchased commercial game/app template into this project. Documented standard dependencies are Unity 6000.6 with URP/UGUI and TextMeshPro resources, Unity IAP 5.4.4 for native purchases, and Google Mobile Ads 11.5.0 with UMP for consent/advertising. Licensed fonts include Barlow, Cinzel and an OFL-derived Libre Baskerville face renamed Floor Concept Serif; the project retains their notices. These dependencies supply engine, rendering, text, payments and advertising functions, not the elevator game or its content.

Project-specific source, authored 3D assets and generated original artwork were developed under the product owner's direction. Coding and image-authoring assistance was used during production. This is not a white-label customer app published by a template vendor. The original functional contribution is the complete native timing/transfer, recovery and banking game described in answers 1 and 7.

### 9. Content provider and submitting account

This is Piotr Kaminski's own Kamilunavo game, submitted directly under the content owner's Apple developer account. The concept, product decisions, branding and development are directed for that owned product. It was not commissioned by an external client, partner, restaurant, venue or other content owner whose app we are publishing on their behalf. Authoring tools and SDK vendors are development suppliers, not separate clients or the provider of this game's branding/content. This is why the application is submitted from this developer account.

Please reconsider build 1.0 (6) in light of these answers and the native evidence. If a specific existing app, template or component remains the concern, please identify it and explain the substantive overlap so that we can provide a targeted clarification or make an appropriate product change. We have not simply resubmitted the rejected binary.

## References

- [Apple App Review Guidelines - 4.3 and 4.2.6](https://developer.apple.com/app-store/review/guidelines/)
- [Unrelated One More Floor - Joshua Myers](https://apps.apple.com/ca/app/one-more-floor/id6772283575)
- [+1 Floor - Carlo Pelliccioni](https://apps.apple.com/fr/app/1-floor/id6796015248)
