using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Executors;
using ActorsCP.Logger;
using ActorsCP.Tests.TestActors;

using CommandLine;
using CommandLine.Text;
using ActorsCP;
using ActorsCP.Options;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    ///
    /// </summary>
    [Verb("queue", HelpText = "Работа с очередями")]
    public sealed class QueueHandler : SetHandler
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
        protected override async Task<int> InternalRunForSet()
            {
            DefaultViewPort.NoOutMessages = false;

            DefaultViewPort.NoOutMessages = NoOutMessagesDefault;

            GlobalSettings.AddOrUpdateActorDebugOption(ActorDebugKeywords.ViewPort_DebugStateChangedEvent, true);
            DefaultViewPort.Reconfigure();

            var queue = new ActorsQueue();
            queue.SetCleanupAfterTermination(true);

            // var loggerOptions = new ActorLoggerOptions();
            // loggerOptions.SetLogLevel(ActorLogLevel.Error);

            var globalLoggerOptions = DefaultErrorLoggerOptions.ErrorLoggerOptions;

            for (int i = 0; i < nItemsCount; i++)
                {
                var name = string.Format("ПРОСТОЙ-ОБЪЕКТ {0}", i + 1);
                var actor = new WaitActor(name);
                actor.SetLoggerOptions(globalLoggerOptions);
                actor.Interval = 100;
                queue.Add(actor);
                }

            using (var executor = new ActorExecutor(queue, DefaultViewPort))
                {
                await executor.RunAsync();
                } // end using

            return 0;
            }
        } // end class QueueOptions
    } // end namespace ActorsCPConsoleRunner.Handlers