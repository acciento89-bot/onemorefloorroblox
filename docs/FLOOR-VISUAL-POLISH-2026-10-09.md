# One More Floor visual polish handoff — 2026-10-09

Source-only revision under the user's immediate execution authorization. Current delivered TestFlight 1.0(3) is unchanged. No Unity import/build, native build, version change, commit, push, upload or publication was performed by the implementation owner.

## Confirmed cause

`FloorHud.Row` created a button with cyan, allowing `UiFactory.Button` to choose dark text from its input luminance, then replaced only the background with dark steel. Risk Continue and bank-result Home retained the dark text. The supplied screenshots reproduce the resulting black-on-dark labels. Numerical pre-fix reproduction: 1.125:1 contrast against a 4.5:1 requirement.

Final surface and foreground now move together through `UiFactory.StyleButton`; all normal, highlighted, selected, pressed and disabled state tints preserve contrast. Rows use their final surface at creation. Home Play and checkpoint Bank explicitly restyle both surface and label. The live timing action also updates its text when changing between navy, cyan and gold.

## Presentation changes

- Scenic hero and three tower selector crops reuse the existing owned panorama. Cool/dusk crops and cyan/gold lift accents establish One More Floor's navy/neon identity.
- The lift elevation has a backplate, paired guide channels, structural bracing, ribbed deck fronts, lit bays, control displays and a transfer trail.
- Risk and bank results use concise image-backed amount/checkpoint or wallet summaries and height-capped cards. Landscape uses shorter summary/row heights so both decisions fit above the fixed footer.
- Darker brushed deck steel, service inlays/vents, front hazard chevrons, powered undercarriages, rollers and a control display add industrial form while retaining the single unchanged landing collider.
- Stationary track guides, drive housings, joints and end vents accompany the moving decks. A batched rear skyline adds city depth behind the entire route.
- Softer warm key light, cyan shadowless rim and cooler ambient/fog preserve gold route/cyan bay separation. Existing fixed camera, timing, profiles, commerce, tutorial ownership and pooled art disposal remain the owning systems.

## Files

`Assets/Scripts/UI/UiFactory.cs`, `FloorHud.cs`, `LiftTowerGraphic.cs`, new `ScenicCityGraphic.cs` and its meta; `Assets/Scripts/Visuals/PlatformArt.cs`, `CityArt.cs`; `Assets/Scripts/Gameplay/FloorCourse.cs`; `Assets/Scripts/Core/GameBootstrap.cs`; `Assets/Editor/FloorMenuValidation.cs`; `Assets/Scripts/QA/ElevatorRuntimeQa.cs`.

## Evidence already run

- Pre-fix contrast reproduction failed as intended at 1.125:1.
- Lightweight C# probe runs the exact production `StyleButton` and `Contrast` methods using standalone color/UI containers: 71 checks pass, minimum authored state contrast 5.015497:1. Includes bright-to-dark and dark-to-gold restyling, high contrast settings and the old black-on-dark negative control. This does not simulate Unity rendering.
- Roslyn syntax parse of all ten changed/new C# files: zero syntax errors.
- `git diff --check`: clean.
- Unity compilation, shader/render acceptance, editor fixtures, runtime screenshots and performance remain for the root's serial validation. Syntax/color probes are not engine/native acceptance.

## Engine validation entry points

`FloorValidation.ValidateAll` includes `FloorMenuValidation.Validate` with DE/EN fixtures at 375×627, 393×759, 430×839, 734×343, 809×372 and the reserved 280×580 pane. New checks cover final label colors across every button state in risk, bank result, settings and completion, >=48 logical points, visible decision buttons, image presence, exactly one unchanged landing collider and authored industrial detail.

Development player: `-qaElevatorShowcase -qaExit -qaOutput <owned-output-directory>`, or `ELEVATOR_QA_SHOWCASE=1` with existing isolated `ELEVATOR_QA_ID` and `ELEVATOR_QA_OUTPUT`. The showcase now performs real ordinary four-transfer checkpoint banking in both DE and EN in actual portrait and landscape, capturing `risk-{pose}-{language}`, `banked-{pose}-{language}` and `settings-{pose}-{language}`. It checks state contrast and that risk/result choices remain inside the clipped viewport. Existing actual tutorial risk/bank route remains covered. Run the full `-qaElevator` route afterward for framing/recovery and all 87 transfers.

## Visual inspection

Inspect lobby DE/EN with all three tower crops, portrait and landscape, and the narrow reserved pane. At floor5 check the amount summary, visible white Continue label and gold Bank button; bank and inspect gold Replay plus white Towers & Designs. Check both available and disabled option/shop/daily controls and high contrast. Inspect real floor2/7/16 gameplay for authored deck surface, bright but bounded bay edges, warm/cyan runner lighting and skyline depth. The next deck and arc must remain unobstructed throughout transfers and after orientation changes. Monitor scene/resource disposal after tutorial replay, skip and restarting all three towers; no generated texture/sprite is allocated per menu open.
