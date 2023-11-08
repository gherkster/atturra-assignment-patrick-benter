using Calculator.App.Configuration;
using Microsoft.Extensions.Options;

namespace Calculator.App.Services;

/// <summary>
/// Service to format currency in a consistent format based on an initial configuration
/// </summary>
public class StringFormatter : IStringFormatter
{
    private readonly FormatConfig _config;

    public StringFormatter(IOptions<FormatConfig> config)
    {
        _config = config.Value;
    }

    public string ToCurrency(decimal value)
    {
        return value.ToString($"C{_config.DisplayedDecimalPlaces}");
    }
}

public interface IStringFormatter
{
    string ToCurrency(decimal value);
}
