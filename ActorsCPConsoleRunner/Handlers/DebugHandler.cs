using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    /// Отладочный класс
    /// </summary>
    [Verb("debug", HelpText = "Опции опладки")]
    public class DebugHandler : HandlerBase
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
                    new Example("Convert file to a trendy format", new DebugHandler { filename = "file.bin" })
                    };
                return list;
                }
            }

        #endregion Пример использования

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected override async Task<int> InternalRun()
            {
            Console.WriteLine($"InternalRun() - {this.ToString()}");
            return 0;
            }
        } // end class DebugOptions
    } // end namespace ActorsCPConsoleRunner.Handlers