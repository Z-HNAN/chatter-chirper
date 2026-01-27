# Quickstart: Chatter Chirper Development

## Prerequisites

1. **Game Installation**: Cities: Skylines 1 (Steam Version recommended).
2. **IDE**: Visual Studio 2022 or JetBrains Rider.
3. **Framework**: .NET Framework 3.5 (standard CSL) or 4.x (if using updated modding API).
4. **Modding Tool**: `CitiesSkylines-Modding-Tool` (optional, for scaffolding).

## Setup

1. **Clone Repository**:
   ```bash
   git clone <repo-url>
   cd chatter-chirper
   ```

2. **References**:
   You must reference the following DLLs from your game installation (`Cities_Data/Managed`):
   - `Assembly-CSharp.dll`
   - `ICities.dll`
   - `UnityEngine.dll`
   - `ColossalManaged.dll`

3. **Nuget Packages**:
   - `Lib.Harmony` (Ensure correct version for CSL).

## Building

1. Open `ChatterChirper.sln`.
2. Select `Release` configuration.
3. Build logic:
   - The Post-Build event should copy the `ChatterChirper.dll` to:
     `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ChatterChirper\`

## Running

1. Launch Cities: Skylines.
2. Go to **Content Manager > Mods**.
3. Enable "Chatter Chirper".
4. Load a city.
5. Watch the Chirper feed at the top of the screen.

## Troubleshooting

- **Log File**: Check `Cities_Data/output_log.txt` (Windows) or `Player.log` (Mac/Linux).
- **Harmony Errors**: Ensure no other mod is patching the exact same method with an incompatible transpiler (we use Prefix, so it should be safe).
- **Missing Messages**: Ensure `messages.json` is copied to the Mod directory next to the DLL.
