using Calculator.App.Configuration;
using Calculator.App.Exceptions;
using Calculator.App.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Calculator.App.Services;

public class TaxCalculator : ITaxCalculator
{
    private readonly TaxConfig _taxConfig;

    public TaxCalculator(IOptions<TaxConfig> config)
    {
        _taxConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
    }

    public (decimal TaxableIncome, decimal SuperContribution) SplitGrossIncomeIntoTaxableAndSuper(decimal grossYearlyIncome)
    {
        var yearlyTaxableIncome = grossYearlyIncome / (1 + _taxConfig.SuperPercentage);
        var yearlySuperContribution = yearlyTaxableIncome * _taxConfig.SuperPercentage;

        return (yearlyTaxableIncome, yearlySuperContribution);
    }

    /// <summary>
    /// Find all taxes that the user is liable for based on their taxable income and the configured tax brackets
    /// </summary>
    /// <exception cref="TaxConfigurationException"></exception>
    public List<TaxCalculationResult> CalculateDeductions(decimal rawTaxableIncome)
    {
        var results = new List<TaxCalculationResult>();

        foreach (var deduction in _taxConfig.Deductions)
        {
            // Taxable income is rounded down when calculating deductions
            var taxableIncome = Math.Floor(rawTaxableIncome);

            var taxBracket = deduction.TaxBrackets
                .OrderBy(b => b.BracketEnd) // Ensure items are in increasing order so that the lowest matching bracket will be selected
                .FirstOrDefault(b => taxableIncome <= b.BracketEnd);

            if (taxBracket is null)
            {
                throw new TaxConfigurationException("Tax bracket not found", nameof(_taxConfig.Deductions));
            }

            var affectedIncomeAmount = taxBracket.ApplicableTo == TaxRateApplicableTo.WholeIncome
                ? taxableIncome
                : taxableIncome - taxBracket.BracketStart;

            var deductionAmount = taxBracket.FlatAddition + affectedIncomeAmount * taxBracket.Rate;

            if (deduction.Rounding == RoundingStrategy.UpToNearestDollar)
            {
                deductionAmount = Math.Ceiling(deductionAmount);
            }

            results.Add(new TaxCalculationResult()
            {
                Name = deduction.Name,
                Value = deductionAmount
            });
        }

        return results;
    }

    public decimal CalculatePayPacket(decimal netSalary, PayFrequency payFrequency)
    {
        return payFrequency switch
        {
            PayFrequency.W => Math.Round(netSalary / 365 * 7, 2, MidpointRounding.AwayFromZero),
            PayFrequency.F => Math.Round(netSalary / 365 * 14, 2, MidpointRounding.AwayFromZero),
            PayFrequency.M => Math.Round(netSalary / 12, 2, MidpointRounding.AwayFromZero),
            _ => throw new InvalidEnumArgumentException($"Pay frequency {payFrequency} is not a supported option"),
        };
    }
}

public interface ITaxCalculator
{
    (decimal TaxableIncome, decimal SuperContribution) SplitGrossIncomeIntoTaxableAndSuper(decimal grossYearlyIncome);
    List<TaxCalculationResult> CalculateDeductions(decimal rawTaxableIncome);
    decimal CalculatePayPacket(decimal netSalary, PayFrequency payFrequency);
}
