param(
    [string]$Configuration = "Release"
)

Write-Host "Building ChatterChirper in configuration: $Configuration" -ForegroundColor Cyan

$projectPath = "src/ChatterChirper.Mod/ChatterChirper.Mod.csproj"

# Run build
& dotnet build $projectPath -c $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Build completed. If Directory.Build.props ManagedPath is set correctly, the mod is already copied to %LOCALAPPDATA%/Colossal Order/Cities_Skylines/Addons/Mods/ChatterChirper." -ForegroundColor Green
