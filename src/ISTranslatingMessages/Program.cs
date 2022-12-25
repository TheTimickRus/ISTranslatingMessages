using System.Text;
using ISTranslatingMessages;
using ISTranslatingMessages.Commands;
using ISTranslatingMessages.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = Constants.Titles.VeryShortTitle;
Console.BackgroundColor = ConsoleColor.Black;

var app = new CommandApp<MainCommand>();
app.Configure(conf =>
{
    conf.Settings.ApplicationName = $"{Constants.Titles.VeryShortTitle}.exe";
    conf.Settings.ApplicationVersion = Constants.Titles.VersionWithDate;
    
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng" });
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng", "-p GoogleV1" });
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng", "-p GoogleV2" });
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng", "-p Microsoft" });
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng", "-p Yandex" });
    conf.AddExample(new [] { "-f messages.iss", "-s rus", "-t eng", "-p Bing" });
    conf.AddExample(new [] { "--filename messages.iss", "--source rus", "--target eng", "--translator Bing" });
    
    conf.Settings.ExceptionHandler += ex =>
    {
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        var color = Constants.Colors.ErrorColor.ToHex();
        
        AnsiConsole.Clear();
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.ErrorColor);
        
        AnsiConsole.MarkupLine($"\n> [bold #{color}]A fatal error has occurred in the operation of the program![/]\n");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

        AnsiConsole.Console.Input.ReadKey(true);
        return -1;
    };
});

return await app.RunAsync(args);