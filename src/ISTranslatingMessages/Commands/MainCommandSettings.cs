// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.ComponentModel;
using ISTranslatingMessages.Extensions;
using ISTranslatingMessages.Models;
using Spectre.Console.Cli;

namespace ISTranslatingMessages.Commands;

public class MainCommandSettings : CommandSettings
{
    #region Public Props

    [Description("Name of the file to translate")]
    [CommandOption("-f|--filename")]
    public string Filename { get; init; } = "";
        
    [Description("Source language (as in the file)")]
    [CommandOption("-s|--source")]
    public string SourceLanguage { get; init; } = "";

    [Description("Target language (as in the file)")]
    [CommandOption("-t|--target")]
    public string TargetLanguage { get; init; } = "";

    [Description("Translator")]
    [CommandOption("-p|--translator")] 
    public Translator Translator { get; init; } = Translator.Default;

    #endregion
    
    #region Public Func

    public bool IsCommandLine()
    {
        return Filename.IsNotEmpty() && SourceLanguage.IsNotEmpty() && TargetLanguage.IsNotEmpty();
    }

    #endregion
}