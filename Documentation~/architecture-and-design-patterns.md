# Architecture & Design Patterns

**v1 — 2026-09-29.** Extracted from Boomer/GayRobots at Stage 7.

A portable statement of how I build Unity projects. Written to travel — the intent is that a new project starts from these patterns rather than re-deciding them.

Examples are drawn from real code where a pattern isn't self-evident. Read this alongside a project's own `AGENTS.md` (process/guardrails) and `ARCHITECTURE.md` (system map); this document is the *why* and the *shape*, not the per-project inventory.

## Revision log

| Version | Date | Change |
|---|---|---|
| v1 | 2026-09-29 | First pass. Patterns, conventions, anti-patterns, judgement calls. |
| v2 | 2026-10-02 | §16: added "Verification is a contract (falsifiable checkpoints)" — claim/check/falsification/evidence, red→green discipline, assert vs capture, agent-trust rationale. |

---

## 1. Composition over inheritance

Behaviour is built by combining small components on GameObjects, never by building class hierarchies.

- A parent GameObject is a **thin shell** that groups children by function. Each child owns one responsibility.
- **One system per GameObject.** If a component doesn't belong to the concept the GameObject represents, it goes on a child.
- Break this only when a specific case clearly justifies it, and say why.

A concrete case: the player root owns the character motor, because the motor *is* the body — it moves that transform, so its controller callback has to live on the root. Everything else (input, look, camera, health, death) is a child. That's the justified exception, not the default.

Sub-objects that only exist as transform anchors are fine (`Model`, `Muzzle`), but they stay empty — no logic.

The failure mode to avoid is a "manager" object that accumulates responsibilities because it was convenient. When you feel one forming, split it.

## 2. Data-driven content

Content is ScriptableObject assets and prefabs, not hardcoded classes or duplicated subclasses.

**A ScriptableObject is earned when the data is shared between multiple instances, or when a designer should tune it without touching code.** That is the test.

- Shared/tunable numbers → a ScriptableObject (`WeaponConfig`, `PlayerMovementConfig`).
- Per-instance state that happens to be serialized → plain `[SerializeField]` fields on the component. `Health`'s max and `DamageType` are fields/enum precisely *because* they aren't shared content. Making them SOs would be data-driven for its own sake.

Corollary: an asset can drift from its type. Orphaned serialized fields left over from an earlier design (a `ReloadDuration` on a config whose transitions are animation-driven) are harmless to Unity but misleading to humans. When a field leaves the type, clean the assets.

## 3. Strategy as a ScriptableObject

When a behaviour varies independently of its data, put the behaviour in an abstract ScriptableObject and let the asset choose the concrete one.

The pattern is three parts:

1. A **driver** MonoBehaviour that owns state and orchestrates.
2. A **config** SO holding the numbers.
3. An **abstract SO strategy** with concrete subclass instances as assets.

In Boomer, `Weapon` (driver) + `WeaponConfig` (data) + `FireBehaviour` (strategy, with `HitscanFireBehaviour` and `ProjectileFireBehaviour` as assets). The driver calls `config.FireBehaviour?.Fire(this, config)`. Swapping hitscan for projectile is an inspector assignment, not a subclass, and a new weapon is a new `WeaponConfig` asset in a folder with its clips.

This is the flagship instance of composition-over-inheritance: the variation lives in data, so adding a variant is content work, not code work.

Use it when you can name two materially different concrete cases. Don't build the strategy slot speculatively for one implementation.

## 4. Observables are the contract; no global event bus

Systems communicate through UniRx streams they expose themselves.

- **No global event bus, no service locator, no `static event`.** Decoupling comes from a system holding a reference to the specific producer and subscribing to that producer's streams.
- Events are `Subject<T>`. Ongoing state is `ReactiveProperty<T>`. A payload-free signal is `Subject<Unit>`.
- Subscribers dispose with `.AddTo(this)`; owning types dispose their own streams in `OnDestroy`.
- Expose state outward as `IReadOnlyReactiveProperty<T>` where a consumer shouldn't write it.

Naming: **`On*` for events, a plain noun for state.** `Health.OnDamaged` / `Health.Current`; `Weapon.State` / `Weapon.OnFired`. This is worth being strict about — the convention is only useful if it's uniform.

The payoff: the weapon switcher sets `WeaponInventory.Current` and does nothing else. Each weapon subscribes, and decides for itself whether to draw or holster. The switcher has no animation knowledge, and adding a weapon type doesn't touch it.

Do not push values into another system's `ReactiveProperty`. State changes belong to its owner. Read-only exposure is how you make that structural rather than a matter of discipline.

## 5. Animation-end driven transitions

For anything animated, drive state transitions from the animation's own end event, not from a timer.

Timers desynchronise from the animation and can't be interrupted cleanly. An end callback is inherently cancellation-safe: **an interrupted play never fires it.** So a superseded draw never lands, and switching mid-animation reverses cleanly without special-casing.

State handler must re-check it's still valid before committing (e.g. `OnHolsterEnd` confirms the weapon is still not current before disabling itself).

Watch the host framework's replay semantics. Animancer's `Play` resumes from current time, so a replayed one-shot needs an explicit `state.Time = 0` after play — otherwise a redrawn weapon never reaches its end and sticks mid-state. Know your animation layer's edges; these bite in exactly this pattern.

## 6. State machines: only where state is real

Use an explicit enum state machine for a discrete process with meaningful transitions and gating — a weapon has `Holstered/Holstering/Drawing/Ready/Reloading` and refuses to fire unless `Ready`.

Do **not** use one for a description. Movement state (Idle/Walk/Run/Jump/Fall/Land/Crouch) is *derived* every tick from the motor's current situation; storing it as a machine would just be a cache that can go stale.

The rule: **a state machine when transitions are commands you act on; a derived value when the state is a description of what's already true.**

Gate transitions with a single property (`CanFire => State == Ready`) rather than scattering checks at call sites.

## 7. Seams: isolate the thing you know will change

When a system has one piece you expect to replace, keep it as a single call site behind a clean boundary, and say in the docs that it's a seam.

`PlayerDeath` subscribes to `Health.OnDied` and reloads the scene. That's the only place reload happens, deliberately, so the checkpoint/death-loop and level-loading stages replace one call and nothing else in the pipeline moves.

A seam should be obvious in the code and named in the docs. The point is that the eventual replacement is a local change.

## 8. Ask for values, not components

When a system needs input from another, take the value, not the component. This keeps the consumer ignorant of the producer's identity and lets a variant exist without the dependency.

The motor doesn't reference the look component; it accepts a `ReactiveProperty<float>` via `RegisterLook` and reads it. No provider registered? It falls back to deriving the basis from its own transform. That's what lets a body with no view reuse the motor unchanged — an enemy uses the same motor, no look component, no code change.

Registration should be idempotent and null-tolerant (last registration wins), because play-mode reloads can leave stale registrations behind.

## 9. Layering: dependencies point one way

Three layers, strictly ordered:

1. **Library** (third-party / personal utility set) — bottom. Knows nothing above it.
2. **Template/framework** — the reusable foundation. May use the library. Must never reference game code.
3. **Game** — the top. May use both. Nothing references it.

Enforce with namespaces (`Template.*`, `Game.*`), physical folder separation, and — once the assembly layout is settled — asmdefs. Enforce the *rule* with review until then, because a stray reference in the wrong direction is invisible without it.

Where a project deliberately runs with no namespace layer, asmdef folder boundaries carry the rule instead — follow the project's own `ARCHITECTURE.md`.

The template must stay game-agnostic. If a template system needs to know about a specific game's content, the seam is wrong: the game should provide a config/prefab/strategy asset instead.

A template asset should not accumulate game or library components. A prefab in the template carrying a component from the game layer is a layering violation even if it compiles.

## 10. Concrete first, then generalise

The most important process pattern.

1. Build the simplest thing that works, concretely.
2. Use it.
3. Notice what actually needs to be reusable.
4. Generalise it.
5. Validate the abstraction against a *second and third materially different* concrete case.
6. Move on.

Never design the framework up front. A weapon is built concretely first; only once it works is it generalised; then it's validated with three genuinely different weapons, and the exit condition is that adding the next one is configuration, not infrastructure. The same ladder applies to enemies.

**The abstraction is only proven by a different case.** One implementation is not evidence that a strategy slot is right.

Two invariants: the project is **always playable**, and **every stage leaves the test level better than it found it.** No long stretch of unintegrated framework work.

## 11. The test level is a living document

There is always a playable test scene, and it grows with the systems. It is the integration proof, not a scratch space.

Keep it self-contained: assets the scene needs (materials, etc.) are copied into the project's own folders so the scene references nothing under an imported package. Clone and own what you depend on; a package update shouldn't break your test level.

## 12. Structure by system, never by asset type

A system folder owns *everything* about that system: code, prefabs, ScriptableObjects, materials, VFX, audio, editor scripts. There is no central `Scripts/`, `Prefabs/` or `Art/`.

- Files sit flat inside a system until it hurts, then group by **feature**, not by type. Per-object content gets its own subfolder (`Weapons/Pistol/…`).
- `Editor/` is the exception — Unity treats that folder name as editor-only, so editor scripts must live in one.
- Shared infrastructure with no system home gets its own place (a `Core/`). Shared art gets a `Shared/`.

The test: can you find everything about a system without leaving one folder? If not, something's in the wrong place.

## 13. Earn your abstractions

- No framework-for-framework's-sake.
- **An interface is earned at the second implementation.** One implementer means no interface.
- A system with one caller doesn't need an interface — it needs to be a class.
- Prefer a concrete class plus an abstract *SO strategy* over subclassing when the variation is content-shaped.

`IDamageable` exists because there are two implementers (the health component and the hit-zone collider) and three consumers. Everything else stays concrete by design. Resist the reflex to interface-ify single-implementation types — it's cost with no buyer.

## 14. Damage (and similar pipelines): one entry point

Cross-cutting effects share a single data contract and a single entry point.

- One struct carrying source, amount, type, hit location, hit zone, direction.
- One interface with one method.
- Every producer calls that method; every consumer implements it. Special cases live *behind* the interface, not inside callers.

In Boomer, hit zones are an implementation of the damage interface sitting in front of health. Weapons and hazards didn't change when hit zones were added, because the interface absorbed it. That's the test of a good pipeline: adding a case requires no edits at the producers.

Pass the struct by `in` to avoid copying, and put convenience derivations on it (`IsHeadshot => HitZone == Head`) rather than at call sites.

## 15. Timing is part of the architecture

Decide and document which systems run at frame rate and which at fixed rate, and don't let anything move a transform behind the physics/character-controller's back.

Typical split: input, look, and camera-height interpolation at frame rate; the character motor and physics at fixed rate. Values written at frame rate are read at fixed rate, so they're current whenever sampled.

One writer per transform. If two systems can move the same object, you have a bug waiting.

## 16. Authoring and verification

- **Author scenes, prefabs and assets through tooled operations, not by hand-writing YAML**, and not through one-shot editor scripts that are deleted after use.
- **Verify your own work** — screenshot it, run it, read the console. Never claim something works because it compiled.
- Keep authoring helpers reusable and non-destructive.
- Document the verification *limits* (e.g. simulated input not reaching the real input stack) rather than pretending around them.

### Verification is a contract (falsifiable checkpoints)

"Compiling is not evidence" stated positively: for each stage, define *what check will prove it* before building it, and make that check something a human can run without trusting the implementer. This matters most when the implementation is done by an agent — the check is how a human confirms a claim without reading every diff.

Four parts, in order:

1. **Claim** — restate the acceptance criterion in one line.
2. **Check** — a command that yields pass/fail, or a defined artifact (screenshot/clip) when the behaviour isn't assertable.
3. **Falsification** — deliberately break the feature and show the check go **red**, then restore and show it **green**. **A check that cannot fail is not a check**; this is the guard against a test written to fit whatever was just built.
4. **Evidence** — something the human can inspect independently (console trail, screenshot, clip, result summary).

Rules of thumb:

- **Assert what is assertable** — state, counts, reachability, determinism, win/lose outcomes. **Capture what isn't** — feel and visuals. Trying to unit-test "feels good" is the trap; a clip the human glances at is the honest check.
- **Define the check from the acceptance criterion, not from the implementation.** If it's derived from the code, it only proves the code agrees with itself.
- **The human must be able to run it.** A check only the author can run is a claim, not a checkpoint.
- **EditMode for pure logic; PlayMode for systems and scenes.** Keep the split physical (separate test assemblies), not by discipline.

## 17. Practical conventions worth keeping uniform

- **Naming**: `PascalCase` public members, `_camelCase` private fields, `[SerializeField] private camelCase` (no underscore on serialized fields), `m_` only inside imported packages.
- **No comments in code.** Names carry the meaning. Explain in docs, not in the file.
- **Guard clauses first.** Return early on invalid state; don't nest.
- **`On*` for events, noun for state.** Non-negotiable, or the convention stops being useful.
- **`Subject<Unit>`** for payload-free events rather than `Action` or a bespoke args type.
- **Frame-rate-independent smoothing**: `1 - exp(-sharpness * dt)`, not a raw `lerp` factor.
- **Lazy-init state** so disabled objects are still valid — a disabled weapon must have usable ammo, because a pickup can grant ammo before it's ever drawn.
- **Self-authoring components**: `[RequireComponent]` for hard dependencies, and fix impossible states in `OnValidate` (a trigger volume forces `isTrigger`).

## 18. Anti-patterns

- A global event bus / static event / service locator. Wire references explicitly.
- Pushing values into another system's observable instead of exposing a method or read-only stream.
- Interfaces, abstract bases or strategy slots with a single implementation.
- A "manager" GameObject accumulating unrelated concerns.
- Timers driving animation-coupled state.
- Building framework before you have two real cases.
- Template assets accumulating game/layer components.
- Assuming "it compiled" means "it works".

## 19. Where this is a judgement call

Not everything is a rule, and pretending otherwise is its own failure.

- **Naming drift happens.** A convention with two exceptions is more honest than a rule everyone quietly breaks; if you can't enforce it, fix the code or fix the rule.
- **Read-only enforcement is all-or-nothing.** If you expose most state as writable properties, "observables are the contract" becomes a vibe rather than a guarantee. Decide per-project and apply it consistently.
- **Some coupling is fine in debug tooling.** A debug observer that knows every gameplay type is acceptable *because* it's debug-only and lives in a debug folder. Keep it there.
- **The test level wants to be self-contained, but shared infra (an encounter system, a level-script host) may reasonably live in the template.** Prefer duplicating a small thing into a test scene over a cross-layer dependency.
