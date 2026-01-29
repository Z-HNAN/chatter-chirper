# Chatter Chirper - Cities: Skylines Mod (MVP)

Chatter Chirper is a mod for Cities: Skylines. This is a Minimal Viable Product (MVP) demonstrating how to intercept "chirps" and replace their content.

This repository serves as a reference implementation (seed project) for creating mods that intercept and modify game logic using **Harmony**.

---

## 1. Installation & Setup (Seed Project Guide)

If you are using this project as a template or "seed" to start your own modding journey, follow these steps to get your environment ready.

### Prerequisites
- **Visual Studio 2022** (or VS Code with .NET Framework 3.5 Check/Dev Pack).
- **Cities: Skylines** installed (Steam version recommended).
- **Harmony** (Lib.Harmony) - Included via NuGet or game reference.

### Step 1: Configure Game Paths
The project needs to reference the game's DLLs (`assembly-csharp.dll`, `ICities.dll`, `ColossalManaged.dll`, etc.).
We use `Directory.Build.props` to define the path to these files so you don't have to edit the `.csproj` directly.

1. Open `Directory.Build.props` in the root folder.
2. Edit the `ManagedPath` property to match your local installation:
   ```xml
   <PropertyGroup>
     <!-- Example Path for Windows Steam -->
     <ManagedPath>C:\Program Files (x86)\Steam\steamapps\common\Cities_Skylines\Cities_Data\Managed</ManagedPath>
   </PropertyGroup>
   ```

### Step 2: Build the Mod
You can build the project using the provided PowerShell script or standard dotnet commands.

**Using PowerShell:**
```powershell
./build_mod.ps1
```
This script builds the solution in `Release` mode.

**Using Command Line:**
```bash
dotnet build src/ChatterChirper.Mod/ChatterChirper.Mod.csproj -c Release
```

### Step 3: Deployment
The `.csproj` file usually contains a post-build event (or you can manually copy) to move the compiled DLLs to the game's local mod directory:
- **Location**: `%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\ChatterChirper\`
- **Files**: `ChatterChirper.Mod.dll`, `0Harmony.dll`

Once copied, launch Cities: Skylines and enable the mod in Content Manager.

---

## 2. Technical Guide: Intercepting & Modifying Chirps

The core functionality of this mod relies on **intercepting** the message before it is displayed on the screen and **swapping** its content.

### The Hook (Harmony Patch)
We use the [Harmony](https://harmony.pardeike.net/) library to patch the game's UI method that receives new messages.

**Target Method:** `ChirpPanel.AddMessage(IChirperMessage message)`  
**Patch Type:** `Prefix`

Because `IChirperMessage` is an interface, we can intercept the call and inspect the incoming message object.

### Implementation Details

**1. The Patcher (Dynamic)**
Instead of using standard Harmony annotations, we use dynamic patching in `Patcher.cs` to locate the `ChirpPanel` class and `AddMessage` method at runtime. This adds robustness against game updates or assembly reference issues.

```csharp
// Logic in Patcher.PatchAll()
var chirpPanelType = AccessTools.TypeByName("ChirpPanel");
var method = AccessTools.Method(chirpPanelType, "AddMessage");
harmony.Patch(method, new HarmonyMethod(typeof(ChirperPanelPatch).GetMethod("Prefix")));
```

**2. The Patch Logic (Prefix)**
Located in `src/ChatterChirper.Mod/Patches/ChirperPanelPatch.cs`:

```csharp
public static class ChirperPanelPatch
{
    // The "ref" keyword is crucial here!
    public static void Prefix(ref IChirperMessage message)
    {
        // 1. Log the original message
        // 2. Wrap it with our Proxy to override the text
        // 3. Game receives the proxy and displays our fixed text
        message = new ChirperMessageProxy(message, "Hello World!");
    }
}
```

**2. Swapping the Message**
The game's built-in `CitizenMessage` class might be immutable or hard to modify directly via reflection. Instead of hacking the object's private fields, we use the **Proxy Pattern**.

We defined a wrapper class `ChirperMessageProxy` that implements `IChirperMessage`:

```csharp
public class ChirperMessageProxy : IChirperMessage
{
    private readonly IChirperMessage _original;
    private readonly string _newText;

    public ChirperMessageProxy(IChirperMessage original, string newText)
    {
        _original = original;
        _newText = newText;
    }

    // Return our custom text instead of the original
    public string text => _newText;
    
    // Pass everything else through to the original
    public uint senderID => _original.senderID;
    public string senderName => _original.senderName;
}
```

**3. The Result**
In the `Prefix` method, we replace the `ref message` argument with our proxy:

```csharp
// Inside Prefix method
string newText = "This is a custom message!";
message = new ChirperMessageProxy(message, newText);
```

When the game continues to execute `AddMessage`, it uses our `ChirperMessageProxy` instead of the original object, displaying our custom text on the screen while keeping the correct sender name and icon.
