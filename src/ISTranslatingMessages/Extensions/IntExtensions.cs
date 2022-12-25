using System.Globalization;

namespace ISTranslatingMessages.Extensions;

public static class IntExtensions
{
    public static string ToPercent(this int @this, int total)
    {
        return $"{@this * 100 / total} %";
    }
}