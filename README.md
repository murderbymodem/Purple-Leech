# Purple Leech

A fast, easy to use VOD downloader for Twitch, with 1440p support and a channel monitor that records broadcasts while they are live, so you keep the original audio.

## What's new in Purple Leech

- **1440p downloads**: Twitch hides the 1440p (HEVC) source quality of enhanced broadcasts from its VOD playlists. Purple Leech finds it anyway and lists it as `Source (2560x1440, HEVC)`, next to the usual qualities.
- **Channel monitor**: add the channels you care about and Purple Leech checks every 15 seconds whether they are live. When one goes live, its VOD is recorded automatically.
- **Live capture before muting**: the VOD is downloaded while the stream is running, before Twitch's copyright check mutes parts of it and deletes the original audio. The download is finished seconds after the stream ends.
- **Crash-safe recordings**: live captures are written to disk as they happen. A recording cut off by a crash or power loss stays playable and is finished automatically on the next start.
- **Muted chunk fallback**: when the original version of a chunk is gone, Purple Leech downloads the muted version instead of leaving a gap in the video.
- **Codec aware quality list**: qualities show their codec when it is not H.264, e.g. `HEVC` or `AV1`. A new `1440p` option is available for the default download quality, and `Source` always picks the best available quality.
- **Keeps your PC awake** while a live capture is running (Windows).
- **Self-contained Windows build**: a single `PurpleLeech.exe`, no .NET installation needed.

## Features

- Very easy to use, no manual needed
- Downloads thousands of video chunks in parallel, using all of your bandwidth
- Browse past broadcasts, uploads and highlights within the application
- Search by channel, VOD URL or VOD ID
- Sub-only VOD downloads (with your own subscription)
- Audio-only downloads
- Download only a time range of a VOD
- Queue multiple downloads
- Default search parameters, download quality, download folder and filename template with wildcards
- Save favorite channels and search presets
- Downloads VODs that are partly unavailable due to missing chunks
- Free and open source, built on .NET 10

## Download (Windows)

Grab the latest `PurpleLeech-vX.Y.Z-win-x64.zip` from the [Releases](https://github.com/murderbymodem/Purple-Leech/releases) page, unzip it anywhere and run `PurpleLeech.exe`.

- Windows 10/11, 64 bit
- No .NET installation required
- Keep `ffmpeg.exe` in the same folder as `PurpleLeech.exe`

## Using the channel monitor

1. Log in with your Twitch account (needed to check which channels are live).
2. Open **Monitor** in the title bar and add channels by name or URL.
3. Leave Purple Leech running. When a channel goes live, a recording appears under **Downloads** and finishes on its own once the stream ends.

Good to know:

- Purple Leech has to stay open for the monitor to work.
- The recording always starts from the beginning of the VOD, even if Purple Leech notices the stream a little late.
- Channels that don't save their broadcasts as VODs can't be recorded this way.
- Recordings use your default download quality, folder and filename template from the preferences.
- While recording, the video is saved as `<name>.recording.mp4` in your download folder and written to disk continuously. If Purple Leech or your PC stops unexpectedly, that file plays up to the moment it stopped, and the next time Purple Leech starts it turns it into a normal video file automatically.
- When the stream ends, the recording is turned into the final video in seconds, without re-encoding, so the quality is exactly what Twitch delivered.

## How it is so fast

Most VOD downloaders let FFmpeg download the video, which rarely gets anywhere near the speed of your internet connection. Purple Leech doesn't use FFmpeg for downloading at all. It downloads the small video chunks of a VOD in parallel, then FFmpeg only merges them into a single video file.

## Building from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and PowerShell.

FFmpeg is not stored in the repository. Download the pinned, checksum-verified build once:

```bash
pwsh scripts/get-ffmpeg.ps1
```

Then build:

```bash
dotnet publish PurpleLeech/PurpleLeech/PurpleLeech.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

Releases are built automatically by GitHub Actions when a `v*` tag (e.g. `v4.0.0`) is pushed.

On Linux the app also builds and runs (`-r linux-x64`) and uses the `ffmpeg` found on your `PATH`, but Windows is the supported platform for now.

## Support & Issues

If you have a problem or a feature request, [open an issue](https://github.com/murderbymodem/Purple-Leech/issues). Please include:

- The version of Purple Leech you are using
- The VOD URL, channel or ID
- Your operating system (e.g. Windows 11 64 bit)
- A description of the problem
- For download problems, the download log (the log button on the download)

## Legal Disclaimer

1. **Purpose of the software.** Purple Leech is a technical tool for personal use only. It is intended for downloading video content for offline viewing, archiving or personal backups (e.g. creators saving their own broadcasts).

2. **User responsibility.** By using this software you agree to comply with all applicable copyright laws and the Twitch Terms of Service. The developer does not encourage, support or induce copyright infringement or unauthorized distribution of content. You are solely responsible for the legality of the content you download.

3. **Access to restricted content.** This tool does not bypass or circumvent any digital rights management (DRM) or payment barriers. It only accesses publicly available video fragments. Sub-only VODs require a valid user-authorized token and can only be downloaded if you already have access to them through your own active subscription.

4. **No affiliation.** Purple Leech is an independent open-source project and is not affiliated with, authorized, maintained, sponsored or endorsed by Twitch Interactive, Inc., Amazon.com, Inc., or any of their affiliates or subsidiaries. "Twitch" is a registered trademark of Twitch Interactive, Inc.

5. **Limitation of liability.** The software is provided "as is", without warranty of any kind. The developer shall not be liable for any claims, damages or other liabilities, including account suspensions or legal actions resulting from the use of this tool.

## Credits

Claudecoded by murderbymodem.

Purple Leech is based on TwitchLeecher-DX by Manuel "Brainy" Schneider, which is a fork of Twitch Leecher by Dominik "Franiac" Rebitzer.

## License

Purple Leech is licensed under the [MIT License](LICENSE), like the projects it is based on.

The Windows release also contains third-party software, most notably FFmpeg (GPL-3.0), which ships as a separate `ffmpeg.exe`. Their licenses are listed in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) and included in the `licenses` folder of every release.
