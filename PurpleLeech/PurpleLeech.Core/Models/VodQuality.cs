using System;

namespace PurpleLeech.Core.Models
{
    public class VodQuality : IComparable<VodQuality>
    {
        #region Constants

        private const string SOURCE_ID = "chunked";
        private const string AUDIO_ONLY_ID = "audio_only";

        #endregion Constants

        #region Constructors

        public VodQuality(string id, string name, string resolution = null, string codecs = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Initialize(id, name, resolution, codecs);
        }

        #endregion Constructors

        #region Properties

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Resolution { get; private set; }

        public int VerticalResolution { get; private set; }

        public string Codecs { get; private set; }

        public string VideoCodec { get; private set; }

        public string DisplayString { get; private set; }

        public bool IsSource { get; private set; }

        public bool IsAudioOnly { get; private set; }

        #endregion Properties

        #region Methods

        private void Initialize(string id, string name, string resolution, string codecs)
        {
            Id = id;
            Name = name;
            Resolution = resolution;
            Codecs = codecs;
            VideoCodec = GetVideoCodec(codecs);

            string displayString = Name;

            if (!string.IsNullOrWhiteSpace(resolution))
            {
                displayString += $" ({resolution}";

                if (VideoCodec != null && VideoCodec != "H.264")
                {
                    displayString += $", {VideoCodec}";
                }

                displayString += ")";
            }

            DisplayString = displayString;

            VerticalResolution = GetVerticalResolution(resolution);

            IsSource = id.Equals(SOURCE_ID, StringComparison.Ordinal);
            IsAudioOnly = id.Equals(AUDIO_ONLY_ID, StringComparison.Ordinal);
        }

        private int GetVerticalResolution(string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution))
            {
                return 0;
            }

            int start = resolution.IndexOf("x") + 1;

            return int.TryParse(resolution.Substring(start, resolution.Length - start), out int res) ? res : 0;
        }

        private static string GetVideoCodec(string codecs)
        {
            if (string.IsNullOrWhiteSpace(codecs))
            {
                return null;
            }

            foreach (string codec in codecs.Split(','))
            {
                string c = codec.Trim().ToLowerInvariant();

                if (c.StartsWith("avc1") || c.StartsWith("avc3"))
                {
                    return "H.264";
                }

                if (c.StartsWith("hvc1") || c.StartsWith("hev1"))
                {
                    return "HEVC";
                }

                if (c.StartsWith("av01"))
                {
                    return "AV1";
                }
            }

            return null;
        }

        public int CompareTo(VodQuality other)
        {
            if (other == null)
            {
                return -1;
            }

            if (VerticalResolution > other.VerticalResolution)
            {
                return -1;
            }
            else if (VerticalResolution < other.VerticalResolution)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VodQuality other))
            {
                return false;
            }

            return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return DisplayString;
        }

        #endregion Methods
    }
}