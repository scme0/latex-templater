using System.Dynamic;
using System.Text;
using YamlDotNet.Serialization;

namespace LatexTemplater;

public static class Templater
{
    public static async Task<string> Execute(CommandLineArguments arguments, CancellationToken cancellationToken)
    {
        var dataContents = await File.ReadAllTextAsync(arguments.dataFile, cancellationToken);
        
        var dataObject = new Deserializer().Deserialize<ExpandoObject>(dataContents);
        
        var templatedTex = await TemplateTexFile(arguments.latexFile, dataObject, cancellationToken);

        if (arguments.outputFile != null)
        {
            await File.WriteAllTextAsync(arguments.outputFile, templatedTex, cancellationToken);
        }
        else
        {
            Console.WriteLine(templatedTex);
        }
        
        return templatedTex;
    }

    private static async Task<string> TemplateTexFile(string latexFile, object data, CancellationToken cancellationToken)
    {
        var tex = await File.ReadAllTextAsync(latexFile, cancellationToken);
        var texLocation = Directory.GetParent(latexFile)?.FullName;
        return await TemplateTexContent(tex, texLocation, data, cancellationToken);
    }

    private static async Task<string> TemplateTexContent(string tex, string? texLocation, object data,
        CancellationToken cancellationToken)
    {
        var startIndex = 0;
        var replacements = new Stack<Replacement>();
        while (tex.IndexOf("<<", startIndex, StringComparison.Ordinal) is var openingVar and > -1 &&
               tex.IndexOf(">>", startIndex, StringComparison.Ordinal) is var closingVar && closingVar > openingVar)
        {
            startIndex = closingVar + 2;
            var variable = tex.Substring(openingVar + 2, closingVar - openingVar - 2).Trim();
            string resultString;
            if (variable.Contains('|'))
            {
                var variableParts = variable.Split('|');
                var varValue = GetVariableValue(data, variableParts[0].Trim());
                if (varValue is List<object> list)
                {
                    var innerTexFile = Path.Join(texLocation, variableParts[1].Trim());
                    var innerTex = await File.ReadAllTextAsync(innerTexFile, cancellationToken);
                    texLocation = Directory.GetParent(innerTexFile)?.FullName;
                    var sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine(await TemplateTexContent(innerTex, texLocation, item, cancellationToken));
                    }
                    resultString = sb.ToString();
                }
                else
                {
                    resultString = await TemplateTexFile(Path.Join(texLocation, variableParts[1].Trim()), varValue, cancellationToken);
                }

            }
            else
            {
                var varValue = GetVariableValue(data, variable);
                if (varValue == null) throw new Exception($"Variable {variable} not found");
                resultString = varValue.ToString()!;
            }
            replacements.Push(new Replacement(openingVar, closingVar - openingVar + 2, resultString));
        }

        return replacements.Aggregate(tex, (current, replacement) => current.Remove(replacement.StartIdx, replacement.Length).Insert(replacement.StartIdx, replacement.Value)).Replace("\n\n", "\n");
    }

    private static object GetVariableValue(object data, string variable)
    {
        if (variable == ".") return data;
        return data switch
        {
            IDictionary<string, object> dictionary => GetVariableValue2(
                dictionary.ToDictionary(object (x) => x.Key, y => y.Value), variable),
            IDictionary<object, object> dictionary2 => GetVariableValue2(dictionary2, variable),
            _ => throw new Exception($"Variable {variable} not found in data file: {data}")
        };
    }

    private static object GetVariableValue2(IDictionary<object, object> data, string variable)
    {
        var variableParts = variable.Split('.');
        var currentDictionary = data;
        object? result = null;
        for (var i = 0; i < variableParts.Length; i++)
        {
            var variablePart = variableParts[i];
            if (variablePart == string.Empty) continue;
            if (i == variableParts.Length - 1)
            {
                result = currentDictionary[variablePart];
                break;
            }

            if (currentDictionary[variablePart] is IDictionary<object, object> dictionary)
            {
                currentDictionary = dictionary;
            }
            else
            {
                throw new Exception($"Variable {variablePart} not found in data file: {variable}");
            }
        }
        
        if (result == null) throw new Exception($"Variable {variable} not found in data file: {data}");

        return result;
    }
}

internal record Replacement(int StartIdx, int Length, string Value);