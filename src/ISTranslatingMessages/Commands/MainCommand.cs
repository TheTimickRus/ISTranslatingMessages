using GTranslate.Translators;
using ISTranslatingMessages.Extensions;
using ISTranslatingMessages.Helpers;
using ISTranslatingMessages.Models;
using Spectre.Console;
using Spectre.Console.Cli;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantNullableFlowAttribute

namespace ISTranslatingMessages.Commands;

public class MainCommand: AsyncCommand<MainCommandSettings>
{
    #region Private Props

    private string _filename = "";
    private string _sLang = "";
    private string _tLang = "";
    private ITranslator _translator = new AggregateTranslator();

    #endregion

    #region Execute

    public override async Task<int> ExecuteAsync(CommandContext context, MainCommandSettings settings)
    {
        /* Отображаем шапку */
        AnsiConsoleLib.ShowHeader();
        
        /* Получаем параметры */
        var (filename, sLang, tLang, translator) = GetParams(settings);
        _filename = filename;
        _sLang = sLang;
        _tLang = tLang;
        _translator = translator;
        
        /* Переводим файл */
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Christmas)
            .AutoRefresh(true)
            .StartAsync("Please wait...", Translation);
        
        /* Завершение работы */
        AnsiConsole.WriteLine();
        AnsiConsoleLib.ShowRule(
            "The work of the program is completed! Press any button to exit",
            Justify.Center,
            Constants.Colors.SuccessColor
        );
        AnsiConsole.Console.Input.ReadKey(true);
        return 0;
    }

    #endregion

    #region Private Methods
    
    private async Task Translation(StatusContext arg)
    {
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        var color = Constants.Colors.SecondColor.ToHex();
        
        /* Читаем файл */
        arg.Status($"[bold #{color}]Reading file...[/]");
        await Task.Delay(1000);

        if (File.Exists(_filename) is false)
            throw new FileNotFoundException();
        
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
        var data = parser.ReadFile(_filename);
        
        /* Переводим секцию Messages */
        var mSection = data.Sections.GetSectionData("Messages");
        if (mSection is not null)
        {
            var preparedKeys = mSection.Keys
                .Where(key => key.KeyName.Contains($"{_sLang}."))
                .Select((x, i) => new { Data = x, Index = i })
                .ToList();

            foreach (var key in preparedKeys)
            {
                if (key.Data.Value.IsNotEmpty())
                {
                    var translationMessage = await _translator.TranslateAsync(key.Data.Value, _tLang, _sLang);
                    mSection.Keys.AddKey(
                        key.Data.KeyName.Replace($"{_sLang}.", $"{_tLang}."),
                        translationMessage.Translation
                    );
                }
                else
                {
                    mSection.Keys.AddKey(
                        key.Data.KeyName.Replace($"{_sLang}.", $"{_tLang}."),
                        ""
                    );
                }
                
                arg.Status($"[bold #{color}]Translating [[Messages]] section... ({key.Index.ToPercent(preparedKeys.Count)})[/]");
            }
        }
        
        /* Переводим секцию CustomMessages */
        var cmSection = data.Sections.GetSectionData("CustomMessages");
        if (cmSection is not null)
        {
            var preparedKeys = cmSection.Keys
                .Where(key => key.KeyName.Contains($"{_sLang}."))
                .Select((x, i) => new { Data = x, Index = i })
                .ToList();
            
            foreach (var key in preparedKeys)
            {
                if (key.Data.Value.IsNotEmpty())
                {
                    var translationMessage = await _translator.TranslateAsync(key.Data.Value, _tLang, _sLang);
                    cmSection.Keys.AddKey(
                        key.Data.KeyName.Replace($"{_sLang}.", $"{_tLang}."),
                        translationMessage.Translation
                    );
                }
                else
                {
                    cmSection.Keys.AddKey(
                        key.Data.KeyName.Replace($"{_sLang}.", $"{_tLang}."),
                        ""
                    );
                }
                
                arg.Status($"[bold #{color}]Translating [[CustomMessages]] section... ({key.Index.ToPercent(preparedKeys.Count)})[/]");
            }
        }

        /* Сохраняем файл */
        arg.Status($"[bold #{color}]Saving file...[/]");
        await Task.Delay(1000);
        
        parser.WriteFile(
            $"{Path.GetFileNameWithoutExtension(_filename)}_translated{Path.GetExtension(_filename)}", 
            data
        );
    }
    
    #endregion

    #region Private Static Methods

    private static (string filename, string sLang, string tLang, ITranslator translator) GetParams(MainCommandSettings settings)
    {
        string filename;
        string sourceLanguage;
        string targetLanguage;
        ITranslator translator;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        var color = Constants.Colors.MainColor.ToHex();
        
        switch (settings.IsCommandLine())
        {
            case true:
                filename = settings.Filename;
                sourceLanguage = settings.SourceLanguage;
                targetLanguage = settings.TargetLanguage;
                translator = GetTranslator(settings.Translator);
            
                AnsiConsole.MarkupLine(
                    $"[bold #{color}]" +
                
                    $"File = {filename}\n" +
                    $"SLang = {sourceLanguage}\n" +
                    $"TLang = {targetLanguage}\n" +
                    $"Translator = {translator.ToString()?.Replace("Name: ", "")}" +
                
                    "[/]"
                );
                break;
            
            case false:
                filename = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[bold #{color}]Name of the file to translate:[/]")
                        .PromptStyle(new Style(Constants.Colors.SecondColor))
                );
                sourceLanguage = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[bold #{color}]Source language (as in the file):[/]")
                        .PromptStyle(new Style(Constants.Colors.SecondColor))
                );
                targetLanguage = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[bold #{color}]Target language (as in the file):[/]")
                        .PromptStyle(new Style(Constants.Colors.SecondColor))
                );

                var translatorResult = Enum.Parse<Translator>(
                    AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title($"[bold #{color}]Select translator:[/]")
                            .AddChoices("GoogleV1", "GoogleV2", "Microsoft", "Yandex", "Bing")
                            .HighlightStyle(new Style(Constants.Colors.SecondColor))
                    )
                );
            
                translator = GetTranslator(translatorResult);
                break;
        }
        
        return (filename, sourceLanguage, targetLanguage, translator);
    }
    
    private static ITranslator GetTranslator(Translator translator)
    {
        return translator switch
        {
            Translator.GoogleV1 => new GoogleTranslator(),
            Translator.GoogleV2 => new GoogleTranslator2(),
            Translator.Microsoft => new MicrosoftTranslator(),
            Translator.Yandex => new YandexTranslator(),
            Translator.Bing => new BingTranslator(),
            _ => new AggregateTranslator()
        };
    }

    #endregion
}