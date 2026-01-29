# Chatter Chirper — Cities: Skylines Mod

Chatter Chirper is a Cities: Skylines mod that intercepts in‑game "chirps" and replaces their text on the fly. It demonstrates safe, targeted game logic modification via Harmony, with a small, testable codebase you can extend.

---

## Overview

- Purpose: Intercept `ChirpPanel.AddMessage(IChirperMessage)` and swap the message text without breaking sender metadata.
- Technique: Harmony prefix patch + lightweight proxy (`ChirperMessageProxy`) that implements `IChirperMessage`.
- Target frameworks: Mod `net35` (Unity/Mono), Tests `net472`.

---

## Requirements

- Windows with Cities: Skylines (Steam recommended).
- Visual Studio 2022 (or VS Code) with .NET Framework 3.5 and 4.7.2 Dev Packs.
- Harmony (Lib.Harmony) is included via NuGet/reference in the project.

---

## Setup

1) Configure game managed assemblies path

Edit `Directory.Build.props` and point `ManagedPath` to your game install (example below). This ensures references like `Assembly-CSharp.dll`, `ICities.dll`, and `ColossalManaged.dll` resolve correctly.

```xml
<PropertyGroup>
    <!-- Example path for Windows Steam -->
    <ManagedPath>C:\Program Files (x86)\Steam\steamapps\common\Cities_Skylines\Cities_Data\Managed</ManagedPath>
    <!-- If using a different library folder name, adjust accordingly. -->
    <!-- E.g., Cities_Data/Managed or Cities_Data/Mono/Managed depending on build/version. -->
    </PropertyGroup>
```

2) Build (Release)

Preferred: use the provided PowerShell script.

```powershell
./build_mod.ps1
```

Alternatively, build the mod project directly:

```powershell
dotnet build src/ChatterChirper.Mod/ChatterChirper.Mod.csproj -c Release
```

---

## Install (Deploy to Cities: Skylines)

Copy the built files to your local Mods folder. Typical output will be under `src/ChatterChirper.Mod/bin/Release/net35/`.

- Destination: `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ChatterChirper\`
- Files to include: `ChatterChirper.Mod.dll` (and Harmony if not bundled by the loader/environment)

Then launch the game and enable the mod in Content Manager → Mods.

Tip: The mod log (when available) can be found near: `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ChatterChirper\ChatterChirper.log`.

---

## How It Works

1) Dynamic patch registration

At startup, `Patcher` resolves `ChirpPanel` and `AddMessage` at runtime and attaches a Harmony prefix method. This is resilient to small assembly load/order shifts.

2) Safe message substitution

`ChirperMessageProxy` wraps the original `IChirperMessage` and overrides only the `text` while forwarding `senderID` and `senderName`. The game proceeds with the proxy, rendering your custom text with the correct avatar/name.

---

## Testing

Unit tests target .NET Framework 4.7.2.

```powershell
dotnet test src/ChatterChirper.Tests/ChatterChirper.Tests.csproj -c Debug
```

---

## Project Structure

- `src/ChatterChirper.Mod/` — Mod source (patcher, systems, utilities, resources)
- `src/ChatterChirper.Tests/` — Test project (net472)
- `specs/001-dynamic-chirper/` — Design notes, plan, quickstart, and schema for messages
- `build_mod.ps1` — Convenience script to build in Release

Notable files:

- `Patches/ChirperPanelPatch.cs` — Harmony prefix hook entry
- `Models/ChirperMessageProxy.cs` — Proxy implementing `IChirperMessage`
- `Systems/MessageLibrary.cs` — Message sourcing/selection helpers
- `Resources/messages.json` — Example message content

---

## Troubleshooting

- Missing references: Recheck `ManagedPath` in `Directory.Build.props`.
- Build failures targeting `net35`: Ensure the .NET Framework 3.5 Dev Pack is installed.
- Tests failing to restore `net472`: Install the .NET Framework 4.7.2 Developer Pack.
- Mod not appearing: Confirm files are in the correct Mods folder and the mod is enabled in Content Manager.

---

## Acknowledgements

Special thanks to my good buddy Copilot for the assist and code guidance throughout this project.
