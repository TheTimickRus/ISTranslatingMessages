using Spectre.Console;

// ReSharper disable MemberCanBePrivate.Global

namespace ISTranslatingMessages;

public static class Constants
{
    public static class Titles
    {
        /// <summary>
        /// *Версия программы* (v.1.0)
        /// </summary>
        public const string Version = "v.1.2";
        /// <summary>
        /// *Версия программы с датой* (v.1.0 (02.09.2022))
        /// </summary>
        public const string VersionWithDate = $"{Version} (25.12.2022)";
        /// <summary>
        /// *Название программы* (*Версия* (*дата*)) by *Разработчик*
        /// </summary>
        public const string FullTitle = $"ISTranslatingMessages ({VersionWithDate}) by Timick";
        /// <summary>
        /// *Название программы* by *Разработчик*
        /// </summary>
        public const string ShortTitle = "ISTranslatingMessages by Timick";
        /// <summary>
        /// *Название программы*
        /// </summary>
        public const string VeryShortTitle = "ISTranslating";
        /// <summary>
        /// Имя лог-файла
        /// </summary>
        public const string LogFileName = $"{VeryShortTitle}.log";
        /// <summary>
        /// Имя файла конфигурации
        /// </summary>
        public const string ConfigFileName = $"{VeryShortTitle}.config";
    }

    public static class Colors
    {
        /// <summary>
        /// Основной цвет
        /// </summary>
        public static readonly Color MainColor = new(119, 141, 169);
        /// <summary>
        /// Второй цвет
        /// </summary>
        public static readonly Color SecondColor = new(65, 90, 119);
        /// <summary>
        /// Цвет успеха
        /// </summary>
        public static readonly Color SuccessColor = new(76, 149, 108);
        /// <summary>
        /// Цвет ошибки
        /// </summary>
        public static readonly Color ErrorColor = new(178, 58, 72); 
    }
}