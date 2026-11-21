# Downloads the FFmpeg build that Purple Plunder ships with and verifies its checksum.
# Puts ffmpeg.exe and its license/build information into PurplePlunder/Libraries.
#
# Usage (from the repository root):  pwsh scripts/get-ffmpeg.ps1

$ErrorActionPreference = "Stop"

# gyan.dev "essentials" release build, from gyan's permanent GitHub release archive
$Version = "9.1"
$Sha256 = "b9bbbb3acece000cbeec994f856649354e52c773807841e78908374b24d658fd"
$Url = "https://github.com/GyanD/codexffmpeg/releases/download/$Version/ffmpeg-$Version-essentials_build.zip"

$Root = Split-Path -Parent $PSScriptRoot
$Libraries = Join-Path $Root "PurplePlunder/Libraries"
$Temp = Join-Path ([System.IO.Path]::GetTempPath()) "purpleplunder-ffmpeg-$Version"
$Zip = Join-Path $Temp "ffmpeg.zip"

New-Item -ItemType Directory -Force -Path $Temp, $Libraries | Out-Null

Write-Host "Downloading FFmpeg $Version from $Url"
Invoke-WebRequest -Uri $Url -OutFile $Zip

$actual = (Get-FileHash -Algorithm SHA256 -Path $Zip).Hash.ToLowerInvariant()
if ($actual -ne $Sha256) {
    throw "Checksum mismatch for FFmpeg $Version. Expected $Sha256, got $actual"
}

Expand-Archive -Path $Zip -DestinationPath $Temp -Force
$Extracted = Join-Path $Temp "ffmpeg-$Version-essentials_build"

Copy-Item (Join-Path $Extracted "bin/ffmpeg.exe") (Join-Path $Libraries "ffmpeg.exe") -Force
Copy-Item (Join-Path $Extracted "LICENSE") (Join-Path $Libraries "ffmpeg-LICENSE.txt") -Force
Copy-Item (Join-Path $Extracted "README.txt") (Join-Path $Libraries "ffmpeg-README.txt") -Force

Remove-Item -Recurse -Force $Temp

Write-Host "FFmpeg $Version is ready in $Libraries"
