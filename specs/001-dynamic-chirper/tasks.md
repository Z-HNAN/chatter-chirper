---
description: "Task list for Context-aware Dynamic Chirper Messages"
---

# Tasks: Context-aware Dynamic Chirper Messages

**Input**: Design documents from `/specs/001-dynamic-chirper/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/
**Tests**: Unit tests included for non-game logic (TextSelector/Provider). Integration tests are manual playtests.
**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Mod Project**: `src/ChatterChirper.Mod/`
- **Test Project**: `src/ChatterChirper.Tests/`
- **Def files**: `src/ChatterChirper.Mod/Models/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create Visual Studio Solution `ChatterChirper.sln` with Mod and Test projects
- [x] T002 Configure `.csproj` files with references to Cities: Skylines DLLs (`ICities`, `UnityEngine`, `Assembly-CSharp`)
- [x] T003 [P] Add NuGet dependency `Lib.Harmony` to `src/ChatterChirper.Mod/`
- [x] T004 Define Post-Build event to copy DLL and assets to Local AppData Mod folder

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented
**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T005 Create `IUserMod` implementation in `src/ChatterChirper.Mod/ModInfo.cs` (Mod metadata)
- [x] T006 Implement `Patcher` class to manage Harmony Patch/Unpatch in `src/ChatterChirper.Mod/Patcher.cs`
- [x] T007 Create `ModLogger` wrapper for `Debug.Log` in `src/ChatterChirper.Mod/Utils/ModLogger.cs`
- [x] T008 [P] Setup basic NUnit test project structure in `src/ChatterChirper.Tests/`
- [x] T009 Define constants and file paths in `src/ChatterChirper.Mod/Constants.cs`

**Checkpoint**: Mod loads in Content Manager, Harmony initializes without error.

---

## Phase 3: User Story 1 - Context-Aware Message Feed (Priority: P1) 🎯 MVP

**Goal**: Transform Chirper from random text to a system that reacts to specific city states like traffic or pollution.

**Independent Test**: Load game, trigger dummy high-traffic condition in `CityContext`, verify `TextSelector` returns traffic message in Unit Test. Then verify in-game.

### Tests for User Story 1 ⚠️

- [x] T010 [P] [US1] Unit test `TextProvider.Load` with sample JSON in `src/ChatterChirper.Tests/TextProviderTests.cs`
- [x] T011 [P] [US1] Unit test `TextSelector.SelectMessage` matching high severity condition in `src/ChatterChirper.Tests/TextSelectorTests.cs`

### Implementation for User Story 1

- [x] T012 [P] [US1] Create `MessageDefinition` and `Condition` models in `src/ChatterChirper.Mod/Models/MessageDefinition.cs`
- [x] T013 [P] [US1] Create `CityContext` model in `src/ChatterChirper.Mod/Models/CityContext.cs`
- [x] T014 [US1] Implement `TextProvider` to load/parse JSON library in `src/ChatterChirper.Mod/Systems/TextProvider.cs`
- [x] T015 [US1] Implement `CityStateReader` to read `TrafficManager`/`DistrictManager` in `src/ChatterChirper.Mod/Systems/CityStateReader.cs`
- [x] T016 [US1] Implement `TextSelector` core logic (filter & first match) in `src/ChatterChirper.Mod/Systems/TextSelector.cs`
- [x] T017 [US1] Implement Harmony Patch `ChirperPanelPatch` for `AddMessage` in `src/ChatterChirper.Mod/Patches/ChirperPanelPatch.cs`
- [x] T018 [US1] Create default `messages.json` library file with basic categories in `src/ChatterChirper.Mod/Resources/messages.json`

**Checkpoint**: In-game Chirper messages are replaced by JSON content based on hard-coded or real metrics.

---

## Phase 4: User Story 2 - Mod Configuration & Customization (Priority: P2)

**Goal**: Allow users to toggle the mod and adjust toxicity/intensity.

**Independent Test**: Change setting in Options Panel, verify `ModConfig` values change, verify `TextSelector` respects "Disabled" or "Toxicity" params.

### Implementation for User Story 2

- [x] T019 [P] [US2] Create `ModConfig` serializable class in `src/ChatterChirper.Mod/ModConfig.cs`
- [x] T020 [US2] Implement `OnSettingsUI` in `ModInfo.cs` using `UIHelper` for options panel
- [x] T021 [US2] Integrate config "Enabled" check into `ChirperPanelPatch.cs`
- [x] T022 [US2] Update `TextSelector.cs` to use `Config.Toxicity` multiplier
- [x] T023 [US2] Add save/load logic for config to `src/ChatterChirper.Mod/Systems/ConfigManager.cs`

**Checkpoint**: Users can disable the mod or change toxicity from the internal menu.

---

## Phase 5: User Story 3 - Message Variety & Repetition Control (Priority: P3)

**Goal**: Prevent text repetition and ensure variety via cooldowns and weighted random.

**Independent Test**: Call `SelectMessage` 100 times in test, verify distribution matches weights and no immediate repeats.

### Tests for User Story 3 ⚠️

- [ ] T024 [P] [US3] Unit test for Cooldown logic (immediate repeat prevention) in `src/ChatterChirper.Tests/TextSelectorTests.cs`
- [ ] T025 [P] [US3] Unit test for Weighted Random distribution in `src/ChatterChirper.Tests/TextSelectorTests.cs`

### Implementation for User Story 3

- [ ] T026 [US3] Add `LastSeen` dictionary to `TextSelector` state (runtime only)
- [ ] T027 [US3] Implement Cooldown check logic in `src/ChatterChirper.Mod/Systems/TextSelector.cs`
- [ ] T028 [US3] Implement Weighted Random selection algorithm in `src/ChatterChirper.Mod/Systems/TextSelector.cs`
- [ ] T029 [US3] Add more variety to `messages.json` resource file

**Checkpoint**: Feed feels natural with no immediate repeats.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T030 Add support for multiple JSON files (merge logic) in `TextProvider`
- [ ] T031 Error handling: Graceful fallback if JSON is corrupt
- [ ] T032 Performance check: Ensure `CityStateReader` doesn't lag on large cities
- [ ] T033 Documentation: Add `README.md` for Steam Workshop
- [ ] T034 [P] Code cleanup: Remove debug logs

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup. Blocks all US.
- **US1 (Phase 3)**: Depends on Foundational.
- **US2 (Phase 4)**: Depends on Foundational. Best done after US1 logic exists to hook into.
- **US3 (Phase 5)**: Depends on US1 (needs Selector).

### Parallel Opportunities

- **T012/T013** (Models) can be built in parallel.
- **T015** (Reader) and **T014** (Provider) are independent until integration in Selector.
- **T019** (Config model) is independent.

## Implementation Strategy

### MVP First (User Story 1 Only)

1. **Setup & Foundation**: Get the mod loading and patching safely (even if the patch does nothing).
2. **US1 Core**: Hardcode one message. Confirm patch replaces text.
3. **US1 Data**: Connect `CityStateReader` and JSON loading.
4. **Validation**: Test "Traffic" scenario.

### Incremental Delivery

1. **v0.1**: Framework + Patch (Foundation).
2. **v0.2**: MVP (US1) - Context aware messages.
3. **v0.3**: Config (US2) - Settings panel.
4. **v0.4**: Polish (US3) - Better math/randomness + Content Pack.
