namespace Calculator.App.Exceptions;

public class TaxConfigurationException : Exception
{
    public string ConfigurationSectionName { get; init; }

    public TaxConfigurationException(string message, string sectionName) : base(message)
    {
        ConfigurationSectionName = sectionName;
    }
}
