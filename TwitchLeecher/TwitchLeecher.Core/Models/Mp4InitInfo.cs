using System;
using System.Text;

namespace TwitchLeecher.Core.Models
{
    /// <summary>
    /// Reads the video resolution and codec from an fMP4 init segment (moov > trak > tkhd / stsd)
    /// </summary>
    public static class Mp4InitInfo
    {
        public static bool TryRead(byte[] data, out int width, out int height, out string codec)
        {
            width = 0;
            height = 0;
            codec = null;

            if (data == null)
            {
                return false;
            }

            int w = 0, h = 0;
            string c = null;

            Walk(data, 0, data.Length, ref w, ref h, ref c);

            width = w;
            height = h;
            codec = c;

            return width > 0 && height > 0;
        }

        private static void Walk(byte[] d, int start, int end, ref int width, ref int height, ref string codec)
        {
            int pos = start;

            while (pos + 8 <= end)
            {
                long size = ReadUInt32(d, pos);
                string type = Encoding.ASCII.GetString(d, pos + 4, 4);
                int header = 8;

                if (size == 1 && pos + 16 <= end)
                {
                    size = (long)ReadUInt32(d, pos + 8) << 32 | ReadUInt32(d, pos + 12);
                    header = 16;
                }
                else if (size == 0)
                {
                    size = end - pos;
                }

                if (size < header || pos + size > end)
                {
                    return;
                }

                int boxEnd = (int)(pos + size);

                switch (type)
                {
                    case "moov":
                    case "trak":
                    case "mdia":
                    case "minf":
                    case "stbl":
                        Walk(d, pos + header, boxEnd, ref width, ref height, ref codec);
                        break;

                    case "tkhd":
                        // Width and height are the last 8 bytes, 16.16 fixed point. Audio tracks have 0.
                        if (boxEnd - 8 >= pos + header)
                        {
                            int tw = (int)(ReadUInt32(d, boxEnd - 8) >> 16);
                            int th = (int)(ReadUInt32(d, boxEnd - 4) >> 16);

                            if (tw > 0 && th > 0 && width == 0)
                            {
                                width = tw;
                                height = th;
                            }
                        }
                        break;

                    case "stsd":
                        // full box header (4) + entry count (4), then the first sample entry
                        int entry = pos + header + 8;

                        if (entry + 8 <= boxEnd)
                        {
                            string fourcc = Encoding.ASCII.GetString(d, entry + 4, 4);

                            if (fourcc is "hvc1" or "hev1" or "avc1" or "avc3" or "av01")
                            {
                                codec ??= fourcc;
                            }
                        }
                        break;
                }

                pos = boxEnd;
            }
        }

        private static uint ReadUInt32(byte[] d, int pos)
        {
            return (uint)(d[pos] << 24 | d[pos + 1] << 16 | d[pos + 2] << 8 | d[pos + 3]);
        }
    }
}
