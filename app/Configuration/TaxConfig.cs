using Calculator.App.Models;

namespace Calculator.App.Configuration;

/// <summary>
/// Configuration pulled from appsettings.json
/// </summary>
public class TaxConfig
{
    public const string JsonKey = "Tax";

    /// <summary>
    /// The super contribution percentage, usually 9.5% (stored as 0.095)
    /// </summary>
    public required decimal SuperPercentage { get; init; }
    public List<DeductionConfig> Deductions { get; init; } = new();
}

public class DeductionConfig
{
    /// <summary>
    /// The name of the tax bracket, displayed to the user
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Any rounding rules which apply to this tax
    /// </summary>
    public RoundingStrategy? Rounding { get; init; }

    public List<TaxBracketConfig> TaxBrackets { get; init; } = new();
}

public class TaxBracketConfig
{
    /// <summary>
    /// The start of the bracket, e.g. 18200
    /// </summary>
    public required int BracketStart { get; init; }

    /// <summary>
    /// The end of the bracket, e.g. 37000
    /// </summary>
    public int BracketEnd { get; init; } = int.MaxValue;

    /// <summary>
    /// The tax rate which should be applied for this bracket
    /// </summary>
    public required decimal Rate { get; init; }

    /// <summary>
    /// Any flat dollar amount which should be added as part of the calculation
    /// </summary>
    public decimal FlatAddition { get; init; }

    /// <summary>
    /// Whether the tax band rate applies to the whole income, or just the excess income within the bracket
    /// </summary>
    public TaxRateApplicableTo ApplicableTo { get; init; } = TaxRateApplicableTo.ExcessIncome;
}
