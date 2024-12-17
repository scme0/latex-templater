namespace Tmpltr;

public static class CommandLineParser
{
    private const string UsageString = "Usage: latemp <template-file> <data-file> [-o <output-file>] [-d '<start><space><end>']";

    public static CommandLineArguments? Parse(string[] commandLineArgs)
    {
        bool outputFileNext = false, delimiterNext = false;
        string? templateFile = null, dataFile = null, outputFile = null, delimiter = null;
        foreach (var arg in commandLineArgs)
        {
            if (outputFileNext)
            {
                outputFileNext = false;
                outputFile = arg;
                continue;
            }
            if (delimiterNext)
            {
                delimiterNext = false;
                delimiter = arg;
                continue;
            }
            switch (arg)
            {
                case "-h":
                case "--help":
                    Console.WriteLine(UsageString);
                    return null;
                case "-d":
                    if (delimiter != null)
                        throw new Exception("Delimiter already set.");
                    delimiterNext = true;
                    continue;
                case "-o":
                    if (outputFile != null)
                        throw new Exception("Output file already set.");
                    outputFileNext = true;
                    continue;
                case var _ when arg.StartsWith("-o="):
                    if (outputFile != null)
                        throw new Exception("Output file already set.");
                    outputFile = arg;
                    continue;
                default:
                {
                    if (templateFile == null)
                    {
                        templateFile = arg;
                    } else if (dataFile == null)
                    {
                        dataFile = arg;
                    }
                    else
                    {
                        throw new Exception($"Too many arguments: {UsageString}");
                    }

                    break;
                }
            }
        }
        if (templateFile == null || dataFile == null) 
            throw new Exception($"Requires at least two arguments: {UsageString}");

        var delimiters = delimiter?.Split(' ');
        if (delimiters is not null and not { Length: 2 }) 
            throw new Exception($"delimiters are not formatted correctly ('{delimiter}'): {UsageString}"); 
        
        return new CommandLineArguments(templateFile, dataFile, delimiters ?? ["<<",">>"], outputFile);
    }
}

public record CommandLineArguments(string templateFile, string dataFile, string[] delimiters, string? outputFile = null);