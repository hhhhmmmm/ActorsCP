using System;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    ///
    /// </summary>
    [Verb("crowd", HelpText = "Работа с толпами")]
    public class CrowdHandler : HandlerBase
        {
        #region Опции командной строки

        [Option('c', "count", Required = false, HelpText = "Количество элементов для обработки в толпе")]
        public int ItemsCount
            {
            get;
            set;
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
                    new Example("Запуск толпы с 10 т. элементов", new CrowdHandler { ItemsCount = 10000 })
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

        //
        } // end class CrowdOptions
    } // end namespace ActorsCPConsoleRunner.Handlers