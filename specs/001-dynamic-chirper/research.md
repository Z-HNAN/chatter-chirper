# Research: Context-aware Dynamic Chirper Messages

**Feature**: `001-dynamic-chirper`
**Date**: 2026-01-27

## Unknowns & Clarifications

### 1. Patch Target Confirmation
**Task**: Confirm which method to patch for Chirper messages.
**Finding**: The `ChirperPanel.AddMessage` and `ChirperManager.AddMessage` are the standard entry points. `ChirperPanel` is UI-centric, while `ChirperManager` is the backend system.
**Decision**: Patch `ChirperPanel.AddMessage` (Prefix) as requested in the design to intercept messages just before display, allowing for the most compatible "visual" replacement without altering the underlying simulation history if not desired, or `ChirperManager` to alter it globally. The user requested "Replace generic messages", implying visual replacement.
**Choice**: `ChirperPanel.AddMessage` (Prefix).

### 2. Game API for Metrics
**Task**: Verify API access for required metrics.
**Finding**:
- Traffic: `TrafficManager.instance.m_averageTrafficFlow` (0-100 int or float).
- Happiness: `DistrictManager.instance.m_districts` iteration or global average.
- Unemployment: `DistrictManager` / `ImmaterialResourceManager`.
**Decision**: Use singleton instances of Managers (`TrafficManager`, `EconomyManager`, etc.) which are accessible in the simulation thread.

### 3. Testing Strategy
**Task**: Define how to test without launching the game constantly.
**Finding**: Game dependencies (`ICities`, `UnityEngine`) are hard to mock.
**Decision**: Isolate the "Selection Logic" and "JSON Parsing" into pure C# classes (`TextSelector`, `TextProvider`) that do not reference Unity/CSL types directly, or use adapters. This allows Unit Testing with NUnit.

## best Practices

### Serialization
- Use `UnityEngine.JsonUtility` (fast, built-in) or `Newtonsoft.Json` (if bundled/available).
- **Decision**: Use `SimpleJSON` or a lightweight strictly-typed parser to avoid dependency hell if Newtonsoft versions conflict with other mods. Or standard `.NET` `System.Runtime.Serialization.Json` if available in the target framework profile. Given CSL's old mono, `UnityEngine.JsonUtility` is safest but limited. User suggested "JSON" - will assume a simple robust parser.

### Performance
- **Constraint**: <1ms.
- **Practice**: Load all JSONs at startup (LoadingExtension). Do not read files at runtime.
- **Practice**: Pre-calculate weights or use efficient lookup structures.

## Summary of Decisions

| Decision | Choice | Rationale |
| :--- | :--- | :--- |
| **Patch Framework** | Harmony | Standard for CSL Mods, stable, high compatibility. |
| **Logic Isolation** | Adapter Pattern | Decouple `CityStateReader` (Game API) from `TextSelector` (Pure Logic) to enable Unit Testing. |
| **Config Format** | XML/JSON | Standard mod settings. |
| **Update Loop** | Event-Driven | Only calculate when `AddMessage` is called, do not run in `Update()`. |
