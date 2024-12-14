using FluentAssertions;

namespace LatexTemplater.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task BasicTest()
    {
        var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            Directory.CreateDirectory(temporaryDirectory);
            var tex = 
                """
                hello
                << my.cool.variable >>
                goodbye
                """;
            var yaml =
                """
                my:
                    cool:
                        variable:
                            "This is a string that will be inserted into the text file."
                """;
            var texFile = Path.Combine(temporaryDirectory, "tex.tex");
            var yamlFile = Path.Combine(temporaryDirectory, "yaml.yaml");
            await File.WriteAllTextAsync(texFile, tex);
            await File.WriteAllTextAsync(yamlFile, yaml);
        
            var result = await Templater.Execute(new CommandLineArguments(texFile, yamlFile), CancellationToken.None);
            
            result.Should().Be("hello\nThis is a string that will be inserted into the text file.\ngoodbye");
        }
        finally
        {
            Directory.Delete(temporaryDirectory, true);
        }
    }
    [Test]
    public async Task TwoFileTest()
    {
        var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            Directory.CreateDirectory(temporaryDirectory);
            var tex = 
                """
                hello
                << more.things.here >>
                goodbye
                """;
            var texBase =
                """
                I want to insert another tex into this file
                << my.cool.inserted.stuff | tex.tex >>
                Other stuff afterwards
                """;
            var yaml =
                """
                my:
                    cool:
                        inserted:
                            stuff:
                                more:
                                    things:
                                        here:
                                            "This is a string that will be inserted into the text file."
                """;
            var texFile = Path.Combine(temporaryDirectory, "tex.tex");
            var texBaseFile = Path.Combine(temporaryDirectory, "texBase.tex");
            var yamlFile = Path.Combine(temporaryDirectory, "yaml.yaml");
            await File.WriteAllTextAsync(texFile, tex);
            await File.WriteAllTextAsync(texBaseFile, texBase);
            await File.WriteAllTextAsync(yamlFile, yaml);
        
            var result = await Templater.Execute(new CommandLineArguments(texBaseFile, yamlFile), CancellationToken.None);
            
            result.Should().Be("I want to insert another tex into this file\nhello\nThis is a string that will be inserted into the text file.\ngoodbye\nOther stuff afterwards");
        }
        finally
        {
            Directory.Delete(temporaryDirectory, true);
        }
    }
}