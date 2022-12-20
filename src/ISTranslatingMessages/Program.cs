using System.Text;
using ISTranslatingMessages;
using ISTranslatingMessages.Commands;
using ISTranslatingMessages.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = Constants.Titles.VeryShortTitle;

var app = new CommandApp<MainCommand>();
app.Configure(conf =>
{
    conf.Settings.ApplicationName = $"{Constants.Titles.VeryShortTitle}.exe";
    conf.Settings.ApplicationVersion = Constants.Titles.VersionWithDate;
    conf.Settings.ExceptionHandler += ex => 
    {
        AnsiConsole.Clear();
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.ErrorColor);
        
        AnsiConsole.MarkupLine("\n> [bold red]A fatal error has occurred in the operation of the program![/]\n");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

        AnsiConsole.Console.Input.ReadKey(true);
        return -1;
    };
});

return await app.RunAsync(args);