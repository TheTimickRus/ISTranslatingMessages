using System.ComponentModel;
using GTranslate.Translators;
using ISTranslatingMessages.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantNullableFlowAttribute

namespace ISTranslatingMessages.Commands;

public class MainCommand: AsyncCommand<MainCommand.MainCommandSettings>
{
    public class MainCommandSettings : CommandSettings
    {
        [Description("Name of the file to translate")]
        [CommandOption("-f")]
        public string? File { get; init; }
        
        [Description("Source language (as in the file)")]
        [CommandOption("-s")]
        public string? SourceLanguage { get; init; }
        
        [Description("Target language (as in the file)")]
        [CommandOption("-t")]
        public string? TargetLanguage { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, MainCommandSettings settings)
    {
        AnsiConsoleLib.ShowHeader();

        string filename;
        string sourceLanguage;
        string targetLanguage;
        ITranslator translator = new AggregateTranslator();
        
        if (settings.File is null or "" || settings.SourceLanguage is null or "" || settings.TargetLanguage is null or "")
        {
            filename = AnsiConsole.Ask<string>($"[bold {Constants.Colors.MainColor}]Name of the file to translate:[/]");
            sourceLanguage = AnsiConsole.Ask<string>($"[bold {Constants.Colors.MainColor}]Source language (as in the file):[/]");
            targetLanguage = AnsiConsole.Ask<string>($"[bold {Constants.Colors.MainColor}]Target language (as in the file):[/]");
            
            var translatorStr = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold {Constants.Colors.MainColor}]Select translator[/]")
                    .AddChoices("Google v1", "Google v2", "Microsoft", "Yandex", "Bing")
                    .HighlightStyle(new Style(Constants.Colors.MainColor))
            );

            translator = translatorStr switch
            {
                "Google v1" => new GoogleTranslator(),
                "Google v2" => new GoogleTranslator2(),
                "Microsoft" => new MicrosoftTranslator(),
                "Yandex" => new YandexTranslator(),
                "Bing" => new BingTranslator(),
                _ => translator
            };
        }
        else
        {
            filename = settings.File;
            sourceLanguage = settings.SourceLanguage;
            targetLanguage = settings.TargetLanguage;
            
            AnsiConsole.MarkupLine($"[{Constants.Colors.MainColor}]File = {filename}\nSLang = {sourceLanguage}\nTLang = {targetLanguage}[/]");
        }
        
        AnsiConsole.WriteLine();
        AnsiConsoleLib.ShowRule("File translation, please wait...", Justify.Right, Constants.Colors.MainColor);
        
        // Читаем INI файл
        var parser = new IniFileParser.IniFileParser
        {
            Parser =
            {
                Configuration =
                {
                    SkipInvalidLines = true,
                    AllowDuplicateKeys = true,
                    AllowDuplicateSections = true
                }
            }
        };
        var data = parser.ReadFile(filename);
        
        var mSection = data.Sections.GetSectionData("Messages");
        if (mSection is not null) {
            foreach (var key in mSection.Keys.ToList().Where(key => key.KeyName.Contains($"{sourceLanguage}.")))
            {
                var translationMessage = await translator.TranslateAsync(key.Value, targetLanguage, sourceLanguage);

                mSection.Keys.AddKey(
                    key.KeyName.Replace($"{sourceLanguage}.", $"{targetLanguage}."),
                    translationMessage.Translation
                );
                
                AnsiConsole.MarkupLine($"[{Constants.Colors.MainColor}]{key.KeyName} = {key.Value.Replace("[", "[[").Replace("]", "]]")}[/]");
                AnsiConsole.MarkupLine($"[{Constants.Colors.SecondColor}]{key.KeyName.Replace($"{sourceLanguage}.", $"{targetLanguage}.")} = {translationMessage.Translation.Replace("[", "[[").Replace("]", "]]")}[/]\n");
            }
        }

        var cmSection = data.Sections.GetSectionData("CustomMessages");
        if (cmSection is not null)
        {
            foreach (var key in cmSection.Keys.ToList().Where(key => key.KeyName.Contains($"{sourceLanguage}.")))
            {
                var translationMessage = await translator.TranslateAsync(key.Value, targetLanguage, sourceLanguage);

                cmSection.Keys.AddKey(
                    key.KeyName.Replace($"{sourceLanguage}.", $"{targetLanguage}."),
                    translationMessage.Translation
                );

                AnsiConsole.MarkupLine($"[{Constants.Colors.MainColor}]{key.KeyName} = {key.Value.Replace("[", "[[").Replace("]", "]]")}[/]");
                AnsiConsole.MarkupLine($"[{Constants.Colors.SecondColor}]{key.KeyName.Replace($"{sourceLanguage}.", $"{targetLanguage}.")} = {translationMessage.Translation.Replace("[", "[[").Replace("]", "]]")}[/]\n");
            }
        }

        /* Пишем файл */
        parser.WriteFile($"{Path.GetFileNameWithoutExtension(filename)}_Translated{Path.GetExtension(filename)}", data);
        
        /* Завершение работы */
        AnsiConsoleLib.ShowRule(
            "The work of the program is completed! Press any button to exit",
            Justify.Center,
            Constants.Colors.SuccessColor
        );
        AnsiConsole.Console.Input.ReadKey(true);
        return 0;
    }
}