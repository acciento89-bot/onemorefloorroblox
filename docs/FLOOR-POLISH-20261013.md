# One More Floor — final coordinated polish, October 13

The user now requests a little refinement toward approximately 90% visual similarity; literal pixel identity is explicitly no longer required. This is a subjective art-direction target, not a computable similarity percentage. The unchanged approved October 10 board remains the reference. Its SHA256 is `49ec586059178ceb804c92723fc65e24aa85a5cd736837fe497609a8a5501d31`.

## Changes

The live serif wordmark curves over its actual visible glyph span. Portrait title/card spacing brings the scenic selection column farther left, preserves landscape rectangles, and adapts to compact phones. Larger semantic destination icons sit on taller primary ivory buttons; parent Back, live stars, shared original coin, Shop access and action ownership are unchanged.

Marked marble surfaces use quiet grain, soft veining, restrained highlights and a reflection from the existing original-room cubemap. Unmarked masonry receives no reflection contribution. No extra reflection camera, runtime cubemap, texture resource or light is created. Hair-only specular/point fill is reduced to soften the bright crown; clothing and skin retain their existing finish.

The slim same-height cabin, live gameplay character scale, imported artist meshes, fixed camera rotation, collider, walk-out and deterministic timing/transfer/rewards remain unchanged. One More Floor only; other games remain queued.

## Native development and meaningful failures

The original project was refreshed and built using Unity6000.6.4f1 on the Mac, through the existing native Art Studio command bridge. No second Editor was run concurrently.

The new marble fixture first failed on the old shipping palette missing its shared reflection binding; `Builds/FloorPolish-20261013/marble-red.txt` preserves the result. An attempted 12° portrait menu framing failed the actual imported floor mesh ray test at the bottom of a430×932viewport. The foreground geometry ends before the new ray intersects it. The original17° framing was restored; the failure is preserved in `pitch12-floor-ray-red.txt`. No threshold was weakened or actual floor replaced with a surrogate.

Final strict manual validation passes14,711checks with zero collected errors. Build validation reaches the cumulative29,423counter, also with zero collected errors, and native Mac build finishes successfully. The historical initial exception stays in the Editor log; these success counts describe the corrected runs. The wordmark still preallocates its Graphic and defers placement outside the TMP draw callback.

## Exact-source evidence

Native source SHA256: `60bdebddf7fa68dd10fa52a898ddb69f02e1f709d5607b5475165a59e71e91af`.

Native Mac bundle SHA256: `f14badbf44823eba632bea9104738c4d7160b35a09e741aa060eafa71fb59b9d`.

Bundle: `/Volumes/SSK SSD/Kamilunavo/Builds/FloorPolish-20261013/OneMoreFloor-PolishQa1.app`.

Fresh native player receipts, genuine unpainted screenshots and the visual judgement are recorded in `QA/floor-polish-20261013.json`. Original menu image-generation/font provenance remains in `FLOOR-MENU-ASSETS-20261013.md`; no replacement concept or painted gameplay image is used here.

Mac-player validation does not establish physical iPhone FPS, touch, audio, signing, native ad-provider or StoreKit acceptance. Earlier full87-transfer/reload evidence belongs to distinct reflection1 source and is not attributed to this candidate. This render-only candidate independently exercises real transfers, menus and variants through its fresh Showcase suite. No TestFlight upload or provider transaction is performed in this polish.

## Actual rendered result and limits

Fresh Showcase passes2,437checks with no captured runtime errors, covering actual transfers across three worlds, tutorial/perfect/risk/bank, styles0/1/3/7, live ratings0/1/3, DE/EN, parent Back/Shop and real430×932/932×430screens. Native GPU probes pass mask isolation, point lighting, bloom and opaque-depth focus, plus brass/glass reflection controls. New actual marked marble reflection delta is0.02745098; unmarked facade leakage is false.

Actual lobby, gameplay, risk and world sheets are in `/Volumes/SSK SSD/Kamilunavo/Artifacts/FloorPolishComparison-20261013/Final1`, with immutable pixel-source hashes in `sources.json`. Only crop/scale/layout/captions are applied, never painted gameplay.

The exact-source visual report passes the revised subjective approximation target. No numerical90% score is asserted. An independent read-only reviewer found no blocker in actual lobby/gameplay samples; the main review also inspected wide lobby/gameplay/settings, portrait risk, ratings and world/style evidence. The actor’s face/shading, fairly uniform yellow brass, angular foliage and carving remain simpler than the illustration. This is a bounded finishing pass, not renewed full art reconstruction.

Before any TestFlight upload, genuine physical iPhone/platform/provider release checks remain required. The passing Mac visual report alone is insufficient.
