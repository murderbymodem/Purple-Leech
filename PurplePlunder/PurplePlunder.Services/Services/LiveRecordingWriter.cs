using System;
using System.Collections.Generic;
using System.IO;
using PurplePlunder.Core.Models;
using PurplePlunder.Shared.IO;

namespace PurplePlunder.Services.Services
{
    /// <summary>
    /// Appends the chunks of a live capture to a single "*.recording.mp4" (or .ts) file while the stream is
    /// running. Twitch chunks are fragmented MP4 or MPEG-TS, so the file stays playable up to the last
    /// complete chunk even if the application or the PC stops before the capture is finished.
    /// </summary>
    internal sealed class LiveRecordingWriter : IDisposable
    {
        public const string RecordingMarker = ".recording";

        private readonly FileStream _stream;
        private bool _initWritten;
        private int _nextIndex;

        private LiveRecordingWriter(string path)
        {
            Path = path;
            _stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        }

        public string Path { get; }

        public int AppendedChunks { get; private set; }

        /// <summary>
        /// Creates a new recording file next to where the final video will be saved
        /// </summary>
        public static LiveRecordingWriter Create(string folder, string filename, bool fragmentedMp4)
        {
            Directory.CreateDirectory(folder);

            string name = System.IO.Path.GetFileNameWithoutExtension(filename);
            string ext = fragmentedMp4 ? ".mp4" : ".ts";
            string path = System.IO.Path.Combine(folder, name + RecordingMarker + ext);

            // Never append to a leftover recording, e.g. when a failed capture is retried
            for (int i = 2; File.Exists(path); i++)
            {
                path = System.IO.Path.Combine(folder, $"{name} ({i}){RecordingMarker}{ext}");
            }

            return new LiveRecordingWriter(path);
        }

        /// <summary>
        /// Appends every chunk that is ready, in playlist order, and forces the data to disk
        /// </summary>
        public void AppendReady(VodPlaylist playlist, ISet<string> handledParts, Action<string> log, List<string> warnings)
        {
            if (!_initWritten && playlist.InitSegment != null)
            {
                if (!File.Exists(playlist.InitSegment.LocalFile))
                {
                    // Chunks cannot be decoded without the init segment, wait for it
                    return;
                }

                AppendFile(playlist.InitSegment.LocalFile);
                _initWritten = true;
            }

            bool appended = false;

            while (_nextIndex < playlist.Count && handledParts.Contains(playlist[_nextIndex].LocalFile))
            {
                VodPlaylistPart part = playlist[_nextIndex];

                if (File.Exists(part.LocalFile))
                {
                    AppendFile(part.LocalFile);
                    FileSystem.DeleteFile(part.LocalFile);
                    AppendedChunks++;
                    appended = true;
                }
                else
                {
                    warnings.Add("Video contains missing frames");
                    log(Environment.NewLine + "Chunk " + _nextIndex + " is missing from the recording");
                }

                _nextIndex++;
            }

            if (appended)
            {
                // Write through the OS cache so a crash or power loss keeps everything appended so far
                _stream.Flush(true);
            }
        }

        private void AppendFile(string file)
        {
            using (FileStream input = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                input.CopyTo(_stream);
            }
        }

        public void Dispose()
        {
            _stream.Flush(true);
            _stream.Dispose();
        }
    }
}
