# Testing & Verification

A portable protocol for proving a change works. Every stage has a **verifiable outcome**; this defines how to prove one — and how a human confirms an agent's claims without reading every diff.

## Protocol

For each chunk, before implementing:

1. **Claim** — the acceptance criterion, one line.
2. **Check** — a command that returns pass/fail, or a captured artifact when the behaviour isn't assertable.
3. **Falsification** — deliberately break the feature and show the Check *fails*, then restore and show it passes (**red → green**). A check that cannot fail proves nothing.
4. **Evidence** — something the human can look at without trusting a summary: console trail, screenshot, or clip.

The Falsification step is the anti-agent-gaming rule. The failure mode with an agent is a test written to fit whatever was just built; showing it can go red is the guard.

## Two kinds of check

- **Assertable behaviour → automated tests.** Pure logic (maze determinism, scoring math, save round-trip, escalation curves) and system/scene behaviour (host up, contact resolves, stage reaches `Won`). Fast, exact, re-runnable.
- **Visual / feel → captured evidence.** Screenshots (`capture_game_view`) or short clips, plus a tight checklist tied to the milestone. Do **not** try to assert "feels good" — that is the trap.

## Where tests live (example: Chromata)

- `Assets/Game/Tests/` — `Game.Tests.asmdef` (PlayMode: all platforms, `UNITY_INCLUDE_TESTS`, refs `Game` + `PurrNet.Runtime` + test runners).
- Add a `Game.Core.Tests` (EditMode) assembly when `Core` pure logic lands (M1+). EditMode only for logic; PlayMode for systems/scenes.
- Co-located smoke scenes stay in each system's folder (e.g. `Game/Net/NetTest.unity`); cross-system scenes in `Assets/Testing/`.

## How to run

- Window: **Window → General → Test Runner** (green/red).
- CLI:
  - `unity command list_tests --mode playmode`
  - `unity command run_tests --mode playmode --async_tests` → then poll `unity command test_status` until `completed: n/n passed`.
  - PlayMode **must** run async: entering play reloads the domain and drops the sync HTTP call. The `run_tests` immediate response (`0/0 passed`) is a stub — read `test_status` for the real result.
  - Pass `--project-path "<project>"` on every call.
