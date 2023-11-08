# Summary

Atturra .NET Developer Interview Assignment - Patrick Benter

Uses .NET 7

# Main components

### app/

Contains the calculator app project

- **Program.cs** - Sets up dependency injection and handles user inputs
- appsettings.json - Includes settings mainly for tax deductions with associated brackets and calculation behaviours, which can be added/modified/removed as tax laws are updated
- ConsoleWriter.cs - Adds some methods to write colours to the console more cleanly
- Configuration/ - Contains the classes which are bound to the appsettings.json file on startup to inject type safe configuration using the [IOptions pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0)
- Exceptions/ - Contains the custom `TaxConfigurationException` used by the calculator in response to in invalid tax bracket configuration
- Models/ - The main data types which are not stored in a structured way
- Services/ - Contains the tax calculator logic and a string formatter to format currency in a consistent format

### tests/

Contains the unit tests written against the calculator app project. These are fairly minimal, and it'd be worth adding more of these for a proper implementation.

# Decisions / Assumptions

- `decimal` was chosen as the underlying data type for money because of the precision issues with `float`/`double`. I experimented with a `Money` domain model to model money in a more appropriate way, but I found this quickly got in the way more than it helped with all the operator overloading and multiplication with other types.

- I made all the injected services singletons since they're all thread safe, but for this app there's no difference since the lifecycle doesn't change.

- I didn't include a top level exception handler which would guarantee that no stack traces get shown to the user. I assumed that this wasn't a requirement, and it makes the code easier to follow with less nesting.

- I assumed the tax bracket range start and end amounts would stay as integers (i.e. 0 - 18200), I think this is a safe assumption and any changes would be minimal anyway if it were to change.

# Extensibility

All of the tax rules are defined in the `appsettings.json` file. It allows
* Taxes to be introduced, changed or removed (i.e. Medicare)
* A rounding strategy to be specified, i.e. whether values should be rounded up to the nearest dollar or left as is.
* Modifying each bracket within the tax, their associated rate and any flat dollar additions and which part of the salary the bracket applies to

With these options, changes to the income tax rules would only need a change to the configuration file and would not require a code change, unless they involved new types of tax calculation which are not currently catered for.

Adding the ability to calculate future and past years could be achieved by adding a financial year value to the tax bracket config section along with the other relevant fields for that financial year. Apart from that, it would likely require some minor changes to the code to ask the user for a date and use that as part of the configuration filtering.

# Future improvements
- Error Handling/Logging - should it include file logs, should it contact someone somehow if it fails, should it provide generic error codes etc
- Could it get the current figures from some government api
- Percentages stored in the `appsettings.json` could be stored as whole number percentages (e.g. 32.5) instead of decimals (e.g. 0.325), which might be easier for users to understand and configure
- User input validation could be improved to provide better coverage and separate it out into its own class, using something like [Fluent Validation](https://fluentvalidation.net/)
