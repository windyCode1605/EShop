namespace CR.Utils.Helpers;

public static class SlugHelper
{
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        
        return text.ToLower()
                   .Replace(" ", "-")
                   .Replace(".", "")
                   .Replace(",", "");
    }
}
