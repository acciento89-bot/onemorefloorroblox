# One More Floor build and release policy

## Canonical source
- GitHub repository: `acciento89-bot/onemorefloorroblox`
- Canonical branch: `main`
- Do not create parallel QA/main variants or multiple competing source branches for release.
- Every coherent verified change is committed and pushed before moving to the next release task.

## Canonical Roblox place policy
- One canonical development/publish path is used for V1.
- Universe, development Place and production Place IDs are intentionally not guessed; they remain external configuration blockers until the real Roblox experience is created/confirmed.
- Publishing must use a build produced from the exact accepted `main` commit.
- Do not create a replacement Roblox experience when an existing canonical experience is available.

## Local verification
```sh
stylua --check src tests scripts
selene src tests scripts
rojo build default.project.json -o /tmp/onemorefloorroblox-qa/build.rbxlx
lune run scripts/run-tests.luau
lune run scripts/release-readiness.luau
```

## Artifact hygiene
- Temporary runtime/build output: `/tmp/onemorefloorroblox-qa`
- No screenshots, builds or temporary QA folders on the user's Desktop.
- Only curated release evidence is committed under `docs/evidence/`.

## Publish gate
A public toggle is permitted only after the canonical published build has completed the full runtime/device journey with no known P0/P1 defect. External IDs, purchases and persistence must be tested against the real experience rather than mocked IDs.
