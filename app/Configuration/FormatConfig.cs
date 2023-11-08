namespace Calculator.App.Configuration;

public class FormatConfig
{
    public const string JsonKey = "Format";

    /// <summary>
    /// Number of decimal places to use when displaying amounts to a user
    /// </summary>
    public int DisplayedDecimalPlaces { get; init; } = 2;
}
