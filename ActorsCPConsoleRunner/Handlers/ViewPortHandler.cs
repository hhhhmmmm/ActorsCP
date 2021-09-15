using System;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    ///
    /// </summary>
    [Verb("viewport", HelpText = "Работа с вьюпортами")]
    public class ViewPortHandler : HandlerBase
        {
        #region Опции командной строки

        [Option('c', "Class", Required = false, HelpText = "Класс отладки: EndUserDatabaseData")]
        public string Class
            {
            get; set;
            }

        [Option("filename", Required = false, HelpText = "Input filename.")]
        public string filename
            {
            get; set;
            }

        #endregion Опции командной строки

        #region Пример использования

        [Usage]
        public static IEnumerable<Example> Examples
            {
            get
                {
                var list = new List<Example>()
                    {
                    new Example("Convert file to a trendy format", new ViewPortHandler { filename = "file.bin" })
                    };
                return list;
                }
            }

        #endregion Пример использования

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected override int InternalRun()
            {
            Console.WriteLine($"InternalRun() - {this.ToString()}");
            return 0;
            }
        } // end class ViewPortOptions
    } // end namespace ActorsCPConsoleRunner.Handlers