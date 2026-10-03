# One More Floor runtime evidence — 2026-10-03

Build: local Rojo build from canonical `main` after persistence/cosmetics/retention/security/accessibility work.

## Confirmed live

- Server initializes successfully in an unpublished local place.
- DataStore unavailability falls back to an explicit ephemeral profile instead of aborting module load.
- Client initializes and the Roblox avatar remains visible in the gameplay camera.
- Floor 1 Precision Jump room, HUD, objective and timer render correctly.
- Timeout and fall enter the failure state once and expose the Retry action.
- Style Garage opens from gameplay and renders coin balance, owned/equipped states and catalog pricing.
- Keyboard/controller-style selection navigation scrolls through the Style Garage correctly.
- Reduced Motion can be selected and toggled ON through focused UI.
- No One More Floor ScriptContext error was observed after the lazy DataStore fix.

## Still open

- A successful human-equivalent Precision Jump clear was not claimed: desktop key injection reliably drove forward movement, but Space did not reliably reach the Roblox play client. The failed automated attempts were valid Fall failures and produced no security rejection.
- Published/API-enabled persistence rejoin, all-family runtime completion, controller hardware, phone/tablet, long soak and paid receipt flows remain release gates.
