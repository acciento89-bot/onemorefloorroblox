# One More Floor Approved Implementation Concept

Portrait 9:16.

Top HUD: Pause | Stage 12/30 + progress | Score | Coins.
World: short readable neon platform chain above a sunset city.
Checkpoint: gold ring and compact dialog with Continue and Retry.
Bottom: joystick left, jump right.

Critical behavior:
- Continue closes the panel and keeps the latest checkpoint.
- Retry returns to the latest checkpoint immediately.
- Falling outside a checkpoint dialog also returns to the latest checkpoint immediately.
- Full run restart is a separate explicit action, never an automatic fall result.
