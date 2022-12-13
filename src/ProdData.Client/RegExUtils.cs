using System.Text.RegularExpressions;

namespace ProdData.Client;

public static partial class RegExUtils
{
    private const string ppuPattern = @"\((\d+,\d+)\s+€\/Liter\)";
    private const string bottlesPattern = @"\d+";
    public const string okay = @"okay";
    
    /// <summary>
    /// ToPpu100 uses a regex pattern (see ppuPattern constant), in order to parse
    /// the numerical part from ppuText which is in the form (x,xx €/Liter).
    /// The ',' character is replaced by '', which is basically a multiplication by 100
    /// This allows us to not be concerned with additional localization complexity.  
    /// If no string matches 
    /// </summary>
    /// <param name="ppuText"></param>
    /// <returns></returns>
    public static (int,string) ToPpu100(this string ppuText)
    {
        if (ppuText == null) 
            return (int.MinValue, "no ppuText found");
        var regex = PpuRegex();
        var match = regex.Match(ppuText);
        if (!match.Success) 
            return (int.MinValue, $"{ppuText} did not match pattern {ppuPattern}");
        var value = match.Groups[1].Value;  // this will be "2,00"
        value = value.Replace(",", "");
        return (Convert.ToInt32(value), okay);
    }


    /// <summary>
    /// ToNbrOfBottles uses a regex pattern (see )
    /// </summary>
    /// <param name="shortText"></param>
    /// <returns></returns>
    public static (int, string) ToNbrOfBottles(this string shortText)
    {
        if (string.IsNullOrEmpty(shortText))
            return (int.MinValue, "no shortText found");
        var regex = new Regex(bottlesPattern);
        var match = regex.Match(shortText);
        if (!match.Success) 
            return (int.MinValue, $"{shortText} did not match pattern {bottlesPattern}");
        var value = match.Value;
        value = value.Replace(",", "");
        return (Convert.ToInt32(value), okay);
    }
    
    
    
    
    

    //[GeneratedRegex("\\((\\d+,\\d+)\\s+€\\/Liter\\)")]
    [GeneratedRegex(ppuPattern)]
    private static partial Regex PpuRegex();
}