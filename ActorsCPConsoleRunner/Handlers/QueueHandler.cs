using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    ///
    /// </summary>
    [Verb("queue", HelpText = "Работа с очередями")]
    public class QueueHandler : HandlerBase
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
                    new Example("Convert file to a trendy format", new QueueHandler { filename = "file.bin" })
                    };
                return list;
                }
            }

        #endregion Пример использования

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected override Task<int> InternalRun()
            {
            Console.WriteLine($"InternalRun() - {this.ToString()}");
            return Task.FromResult(0);
            }
        } // end class QueueOptions
    } // end namespace ActorsCPConsoleRunner.Handlers