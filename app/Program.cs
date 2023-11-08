using Calculator.App;
using Calculator.App.Configuration;
using Calculator.App.Exceptions;
using Calculator.App.Models;
using Calculator.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<TaxConfig>().BindConfiguration(TaxConfig.JsonKey);
builder.Services.AddOptions<FormatConfig>().BindConfiguration(FormatConfig.JsonKey);

builder.Services.AddSingleton<ITaxCalculator, TaxCalculator>();
builder.Services.AddSingleton<IStringFormatter, StringFormatter>();

using IHost host = builder.Build();

await host.StartAsync();

using IServiceScope scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

var taxCalculator = provider.GetRequiredService<ITaxCalculator>();
var stringFormatter = provider.GetRequiredService<IStringFormatter>();

// Finished setting up dependency injection, overkill for an app this size
//----------------------------------------------------------------------//

var yearlySalaryPackage = AskForAndValidateYearlySalary();
var payFrequency = AskForAndValidatePayFrequency();

Console.WriteLine("\nCalculating salary details...\n");
await Task.Delay(500); // Simulate a bit of processing delay

var (yearlyTaxableIncome, yearlySuperContribution) = taxCalculator.SplitGrossIncomeIntoTaxableAndSuper(yearlySalaryPackage);

Console.WriteLine($"Gross package: {stringFormatter.ToCurrency(yearlySalaryPackage)}");
Console.WriteLine($"Supperannuation: {stringFormatter.ToCurrency(yearlySuperContribution)}");
Console.WriteLine();

Console.WriteLine($"Taxable income: {stringFormatter.ToCurrency(yearlyTaxableIncome)}");
Console.WriteLine();

var deductions = new List<TaxCalculationResult>();
try
{
    deductions = taxCalculator.CalculateDeductions(yearlyTaxableIncome);
}
catch (TaxConfigurationException ex)
{
    ShowErrorAndExit($"Tax configuration is invalid. Please review the {ex.ConfigurationSectionName} section of the configuration file");
}

// Dynamically display any deductions the user is liable for
if (deductions.Any())
{
    Console.WriteLine("Deductions:");
}
foreach (var deduction in deductions)
{
    Console.WriteLine($" * {deduction.Name}: {stringFormatter.ToCurrency(deduction.Value)}");
}
Console.WriteLine();

var netYearlyIncome = yearlySalaryPackage - yearlySuperContribution - deductions.Sum(d => d.Value);
Console.WriteLine($"Net income: {stringFormatter.ToCurrency(netYearlyIncome)}");

var payPacketForFrequency = taxCalculator.CalculatePayPacket(netYearlyIncome, payFrequency);
Console.WriteLine($"Pay packet: {stringFormatter.ToCurrency(payPacketForFrequency)}");
Console.WriteLine();

Console.Write("Press any key to end...");
Console.ReadKey(); // Wait for user input before finishing so that results can be read

static decimal AskForAndValidateYearlySalary()
{
    ConsoleWriter.WriteColoured("\nEnter your salary package amount: ", ConsoleColor.Cyan);
    var rawSalaryPackageInput = Console.ReadLine();

    var isYearlySalaryPackageValid = decimal.TryParse(rawSalaryPackageInput, out var yearlySalaryPackage);
    if (!isYearlySalaryPackageValid || yearlySalaryPackage <= 0)
    {
        ShowErrorAndExit("Please enter a positive number for salary package.");
    }

    return yearlySalaryPackage;
}

static PayFrequency AskForAndValidatePayFrequency()
{
    ConsoleWriter.WriteColoured("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ", ConsoleColor.Cyan);
    var payFrequencyInput = Console.ReadLine();

    // Ignore case since a case sensitive check would be annoying for users
    var isValidPayFrequency = Enum.TryParse<PayFrequency>(payFrequencyInput, ignoreCase: true, out var payFrequency);

    // Prevent both in-range and out-of-range enum int representations, otherwise "1" would be valid
    if (!isValidPayFrequency || int.TryParse(payFrequencyInput, out _) || !Enum.IsDefined(payFrequency))
    {
        ShowErrorAndExit("Please enter a valid pay frequency");
    }

    return payFrequency;
}

static void ShowErrorAndExit(string message)
{
    Console.WriteLine();
    ConsoleWriter.WriteColouredLine(message, ConsoleColor.Red);
    Console.WriteLine("Press any key to end...");
    Console.ReadKey();

    Environment.Exit(0);
}