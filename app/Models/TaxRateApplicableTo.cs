namespace Calculator.App.Models;

public enum TaxRateApplicableTo
{
    /// <summary>
    /// The tax is applicable to the excess income, i.e. the difference between their income and the bottom of the band
    /// </summary>
    ExcessIncome,
    /// <summary>
    /// The tax is applicable to the whole income, not just the excess within a specific bracket
    /// </summary>
    WholeIncome
}
