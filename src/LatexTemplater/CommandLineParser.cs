namespace LatexTemplater;

public static class CommandLineParser
{
    private const string UsageString = "Usage: latemp <latexfile> <datafile> [-o <outputfile>]";

    public static CommandLineArguments? Parse(string[] commandLineArgs)
    {
        var outputFileNext = false;
        string? latexFile = null, dataFile = null, outputFile = null;
        foreach (var arg in commandLineArgs)
        {
            if (outputFileNext)
            {
                outputFileNext = false;
                outputFile = arg;
                continue;
            }
            switch (arg)
            {
                case "-h":
                case "--help":
                    Console.WriteLine(UsageString);
                    return null;
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
                    if (latexFile == null)
                    {
                        latexFile = arg;
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
        if (latexFile == null || dataFile == null) 
            throw new Exception($"Requires at least two arguments: {UsageString}");
        
        return new CommandLineArguments(latexFile, dataFile, outputFile);
    }
}

public record CommandLineArguments(string latexFile, string dataFile, string? outputFile = null);