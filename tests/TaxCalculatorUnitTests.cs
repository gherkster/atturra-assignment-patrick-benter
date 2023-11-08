using Calculator.App.Configuration;
using Calculator.App.Services;
using Microsoft.Extensions.Options;

namespace Calculator.Tests;

[TestFixture]
public class TaxCalculatorUnitTests
{
    [Test]
    public void SplitGrossIntoTaxableAndSuper_ValidGross_ShouldReturnCorrectSplit()
    {
        var taxConfig = Options.Create(new TaxConfig()
        {
            SuperPercentage = 0.095M
        });

        var calculator = new TaxCalculator(taxConfig);
        var result = calculator.SplitGrossIncomeIntoTaxableAndSuper(65_000);

        Assert.That(Math.Round(result.TaxableIncome, 2), Is.EqualTo(59_360.73M));
        Assert.That(Math.Round(result.SuperContribution, 2), Is.EqualTo(5_639.27M));
    }

    [Test]
    public void Deductions_MedicareLowIncome_ShouldNotPayTax()
    {
        var taxConfig = Options.Create(new TaxConfig()
        {
            SuperPercentage = 0.095M,
            Deductions = new List<DeductionConfig>()
            {
                new DeductionConfig()
                {
                    Name = "Medicare",
                    Rounding = App.Models.RoundingStrategy.UpToNearestDollar,
                    TaxBrackets = new List<TaxBracketConfig>()
                    {
                        new TaxBracketConfig()
                        {
                            BracketStart = 0,
                            BracketEnd = 21_335,
                            Rate = 0
                        }
                    }
                }
            }
        });

        var calculator = new TaxCalculator(taxConfig);
        var result = calculator.CalculateDeductions(10_000);

        Assert.That(result.Select(r => r.Value), Is.All.EqualTo(0));
    }

    [Test]
    public void Deductions_MedicareHighIncome_ShouldPayTaxOnWholeIncome()
    {
        var taxConfig = Options.Create(new TaxConfig()
        {
            SuperPercentage = 0.095M,
            Deductions = new List<DeductionConfig>()
            {
                new DeductionConfig()
                {
                    Name = "Medicare",
                    Rounding = App.Models.RoundingStrategy.UpToNearestDollar,
                    TaxBrackets = new List<TaxBracketConfig>()
                    {
                        new TaxBracketConfig()
                        {
                            BracketStart = 26_669,
                            Rate = 0.02M,
                            ApplicableTo = App.Models.TaxRateApplicableTo.WholeIncome
                        }
                    }
                }
            }
        });

        var calculator = new TaxCalculator(taxConfig);
        var result = calculator.CalculateDeductions(200_000);

        Assert.That(result.Select(r => r.Value), Is.All.EqualTo(4_000));
    }

    [Test]
    public void PayPacket_SelectMonthly_ShouldReturnMonthlySalary()
    {
        var taxConfig = Options.Create(new TaxConfig()
        {
            SuperPercentage = 0.095M
        });

        var calculator = new TaxCalculator(taxConfig);
        var result = calculator.CalculatePayPacket(60_000, App.Models.PayFrequency.M);

        Assert.That(result, Is.EqualTo(5000));
    }
}
