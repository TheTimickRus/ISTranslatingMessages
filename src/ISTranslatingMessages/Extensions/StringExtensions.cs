namespace ISTranslatingMessages.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty(this string @this)
    {
        return @this == "";
    }
    
    public static bool IsNotEmpty(this string @this)
    {
        return IsEmpty(@this) is false;
    }
}