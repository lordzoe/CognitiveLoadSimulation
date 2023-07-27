using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Helper</c> This class is just a helper for some string comparison overwrite things, not to be worried about
/// </summary>
public static class Helper
{
    public static string GetUntilOrEmpty(this string text, string stopAt = "_")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, System.StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return string.Empty;
    }


}
