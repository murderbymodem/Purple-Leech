using System;

namespace PurplePlunder.Shared.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }
    }
}