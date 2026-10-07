# AGENTS.md

Universal working rules for every project that vendors `stoogebag`. A project's own root `AGENTS.md` must require reading this file, then add only its project-specifics (design authority, scene/assembly map, tech stack, folder paths). When a project doc and this file disagree, the project doc wins for that project.

## Working with me

- Answer concisely. Avoid useless jargon. Do not bloviate. Elaborate only if prompted.
- Keep responses under ~100 words unless detail is asked for.
- Suggest freely, but do NOT do anything without first asking.
- Don't ask questions that don't change the answer.

## Guardrails

- **Ask first, always.** Acting without instruction is the failure mode; suggesting is fine.
- **Don't touch git unless instructed** — no commits, staging, branches, tags or submodule changes.
- Ask before adding a third-party dependency or package.
- Don't move or rename folders or files wholesale — it breaks `.meta` references.
- Don't build speculative systems, and don't gold-plate. Every stage must leave the project playable.

## Conventions

- Composition over inheritance. Small components, wired together.
- **One system per GameObject.** Each system lives on its own child GameObject bolted onto a parent; a parent is a thin shell that groups children by function. No GameObject accumulates unrelated components. Break this only when a specific case clearly justifies it.
- **Organise by system, never by asset type.** A system folder owns its own code, prefabs, ScriptableObjects, materials, VFX and audio. There is no central `Scripts/`, `Prefabs/` or `Art/`.
- Data-driven content: content is ScriptableObjects and prefabs, not hardcoded classes or duplicated subclasses.
- Observables are the contract between systems. No global event bus except where it is obviously a good idea.
- Don't over-abstract. No framework-for-framework's-sake. If a system only has one caller, it doesn't need an interface.
- No comments in code unless asked.

## Testing

- Co-locate a sample/test scene in each system's own folder, so it travels with the system when extracted.
- Cross-system and integration scenes live in a top-level `Assets/Testing/`.
- Give test scenes a logger (typically on a root `Logger` object) so a play-test leaves a readable trail in the console; include one and read it after a play-test.

## Verification

- **Unity is usually already running with the project open. Do not launch another editor.**
- **Multiple editors run in parallel; never rely on auto-detection.** CLI calls pick an instance from the current working directory; pass `--project-path "<project>"` on every call so a command cannot bind to another project. Never mutate another project.
- Drive Unity through the CLI: `unity list` shows commands, `unity command <name> <flags>` runs one. Use these instead of hand-writing scene or prefab YAML.
- **Author scene, prefab and asset edits through the CLI, not one-shot editor scripts.** Compose the built-ins (`create_gameobject`, `add_component`, `set_serialized_field`, `set_transform`, `set_parent`, `save_scene`, `add_scene_to_build`) and, where they don't cover it, `eval`/`eval_file`. For structural prefab edits load the prefab via `PrefabUtility.LoadPrefabContents`/`SaveAsPrefabAsset`.
- `unity command <name> --help` does not print parameters. A wrong flag errors and says what it rejects; `--target`, `--source` and `--path` are common, though many commands use bespoke names — check `CliArg` usage in `Library/PackageCache/com.unity.pipeline*/`.
- Check your own work — capture a screenshot or observe GameObject state. Never claim something works because it compiled.
- Pipeline save paths resolve relative to `Assets/`, even when given an absolute project path. Clean up whatever you write there.
- Unity CLI: `C:\Users\brian\AppData\Local\Unity\bin\unity.exe` (also `unity` on the user PATH in a restarted terminal).
- Headless editor, only if explicitly needed: use the project's Unity Hub Editor install.

## Unity traps

- `.meta` files belong to their asset. Never create, delete, or hand-edit one.
- Scenes and prefabs are merge-hostile YAML. Treat edits to them as risky.
- Git LFS covers scenes and prefabs. Add any new large binary type (models, audio, textures, video) to LFS before it lands, or it is in history forever.

## Portable docs

The rules above are the terse version; the rationale lives in `Documentation~/`:

- `architecture-and-design-patterns.md` — the *why* and *shape*.
- `standards.md` — naming/reactive/async conventions.
- `ui-window-system.md` — the UITools window system.
- `testing-and-verification.md` — the falsifiable-checkpoint protocol.
