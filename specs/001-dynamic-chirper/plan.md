# Implementation Plan: Context-aware Dynamic Chirper Messages

**Branch**: `001-dynamic-chirper` | **Date**: 2026-01-27 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/001-dynamic-chirper/spec.md`

## Summary

Implement a Cities: Skylines 1 Mod that intercepts the Chirper message feed using Harmony patches. The system will read game state (traffic, happiness, etc.) and replace generic messages with context-aware, weighted messages from an offline JSON library.

## Technical Context

**Language/Version**: C# (matching Cities: Skylines Unity/Mono version)
**Primary Dependencies**: 
- Harmony (Lib.Harmony) for method patching
- ICities (Colossal Order API)
- UnityEngine
**Storage**: JSON files for message library
**Testing**: NEEDS CLARIFICATION (Standard NUnit or manual playtesting?) -> Resolved to: Manual Playtesting + Unit Tests for non-game-dependent logic (TextSelector).
**Target Platform**: Windows/Mac/Linux (Cities: Skylines 1 Desktop)
**Project Type**: C# Class Library (.NET Framework 3.5/4.x compatible with game)
**Performance Goals**: < 1ms overhead per message trigger; zero GC allocation in hot paths.
**Constraints**: 
- Must run completely offline.
- Must not break if game updates (use stable APIs where possible).
- Resilience to missing message files.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **2.1 Spec Authority**: Spec is created and detailed.
- [x] **5.3 Patch Declaration**: Harmony usage explicitly declared in Tech Design.
- [x] **5.1/5.2 Separation**: Tech requirements separate from functional spec.
- [x] **7. Failure Modes**: Fallback to vanilla defined for error states.
- [x] **3.1 Scoping**: Scope limited to offline text replacement.

## Project Structure

### Documentation

```text
specs/001-dynamic-chirper/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (JSON Schemas)
│   └── message-library.schema.json
└── tasks.md             # Phase 2 output
```

### Source Code

```text
src/
├── ChatterChirper.Mod/
│   ├── ModInfo.cs                # ICities.IUserMod implementation
│   ├── ModConfig.cs              # Configuration definition
│   ├── ChirperExtension.cs       # IChirperExtension (if applicable) or LoadingExtension
│   ├── Patches/
│   │   └── ChirperPanelPatch.cs  # Harmony patch for AddMessage
│   ├── Systems/
│   │   ├── CityStateReader.cs    # Reads game managers
│   │   ├── TextSelector.cs       # Logic for picking messages
│   │   └── TextProvider.cs       # JSON loader and parser
│   └── Models/
│       ├── MessageDefinition.cs
│       └── CityContext.cs
└── ChatterChirper.Tests/       # Unit tests for TextSelector/TextProvider
    └── ...
```

**Structure Decision**: Standard C# Solution with a Mod project and a Test project. Separating validatable logic (Selector/Provider) from game-dependent logic (Patches/StateReader) allows for unit testing.
