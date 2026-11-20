# Twitch Leech
If you are looking for an extremely fast and easy to use Twitch VOD downloader, this is your tool!

## Download (Windows)
Grab the latest `TwitchLeech-vX.Y.Z-win-x64.zip` from the [Releases](https://github.com/murderbymodem/Twitch-Leech/releases) page, unzip it anywhere and run `TwitchLeech.exe`.

- Windows 10/11, 64 bit
- No .NET installation required (the executable is self-contained)
- Keep `ffmpeg.exe` in the same folder as `TwitchLeech.exe`

## Building from source
Requires the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

```bash
dotnet publish TwitchLeecher/TwitchLeecher/TwitchLeecher.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

Releases are built automatically by GitHub Actions when a `v*` tag (e.g. `v4.0.0`) is pushed.

# Legal Disclaimer
1. Purpose of the Software
This software (Twitch Leech) is provided as a technical tool for personal use only. It is intended to facilitate the download of video content from Twitch for offline viewing, archiving, or personal backup purposes (e.g., for creators to save their own broadcasts).

2. User Responsibility
By using this software, you agree to comply with all applicable copyright laws and the Twitch Terms of Service (ToS). The developer of this software does not encourage, support, or induce any form of copyright infringement or unauthorized distribution of content. You are solely responsible for the legality of the content you download.

3. Access to Restricted Content
This tool does not bypass or circumvent any digital rights management (DRM) or payment barriers.

Public Content: The tool only accesses publicly available video fragments.

Subscription-Only Content: Access to subscriber-only VODs requires a valid user-authorized token. This software only facilitates the download if the user already possesses the legal right to access said content through their own active subscription.

4. No Affiliation
This project is an independent open-source "fork" and is not affiliated with, authorized, maintained, sponsored, or endorsed by Twitch Interactive, Inc., Amazon.com, Inc., or any of their affiliates or subsidiaries. "Twitch" is a registered trademark of Twitch Interactive, Inc.

5. Limitation of Liability
The software is provided "as is", without warranty of any kind. The developer shall not be liable for any claims, damages, or other liabilities, including account suspensions or legal actions resulting from the use of this tool.

## What is the difference compared to other VOD downloaders?
Nearly all of the well known VOD downloaders execute the download process via FFMPEG's integrated download capabilities. However, this is extremely slow. The download speed rarely exceeds 1.5Mbit even if the internet connection is 100 times faster. Twitch Leech does not use FFMPEG for download tasks at all. It downloads thousands of small video chunks in parallel while using all of the available bandwidth of your internet connection. As soon as all video chunks are downloaded, FFMPEG is only used to merge those chunks together in order to create a single video file again.

## Features
- Very easy to use, no manual needed
- Intuitive and stylish GUI
- Up to 20 times faster download speed compared to direct download with FFMPEG
- Browse your past broadcasts, uploads and highlights within the application
- Search channels, VOD urls and VOD IDs
- Sub-Only video download support
- Audio-Only download support
- Time Selection for VOD downloads
- Queue multiple downloads
- Specify default search parameters
- Specify default download quality
- Specify default download folders
- Specify a filename template with wildcards for your downloads
- Free and Open Source
- Actively maintained
- Modern .NET 10 framework
- Save favorite search parameter as preset
- 1440p (HEVC/AV1) VOD download support
- Download VODs that might be unavailable on Twitch due to missing chunk data
  
## Support & Issues
If you have any problem or wish a feature for Twitch Leech, feel free to [open an issue](https://github.com/murderbymodem/Twitch-Leech/issues). Please include:

- The version of Twitch Leech you are using
- As much information about the VOD as possible (Url, Channel, ID)
- Your operating system (e.g. Windows 11 64 Bit)
- A detailed description of the problem
- For download problems, the download log created by Twitch Leech

## Credits
Twitch Leech is based on Twitch Leecher by Dominik "Franiac" Rebitzer and its fork by Manuel "Brainy" Schneider.

## License
[MIT License](LICENSE)
