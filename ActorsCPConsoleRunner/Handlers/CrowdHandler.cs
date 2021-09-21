using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActorsCP;
using ActorsCP.Actors;
using ActorsCP.Executors;
using ActorsCP.Logger;
using ActorsCP.Options;
using ActorsCP.Tests.TestActors;

using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    ///
    /// </summary>
    [Verb("crowd", HelpText = "Работа с толпами")]
    public sealed class CrowdHandler : SetHandler
        {
        #region Опции командной строки

        [Option('w', "withoutlimits", Required = false, HelpText = "Не ограничивать параллелизм, без лимитов на кол-во процесоров")]
        public bool Withoutlimits
            {
            get;
            set;
            }

        [Option('p', "ProcessorCount", Required = false, Default = null, HelpText = "Количество процессоров")]
        public int? nProcessorCount
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
                    new Example("Запуск толпы с 10 т. элементов", new CrowdHandler { nItemsCount = 10000 })
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

            GlobalSettings.AddOrUpdateActorDebugOption(ActorDebugKeywords.ViewPort_DebugStateChangedEvent, true);
            DefaultViewPort.Reconfigure();

            // ActorTime actorTime = default;

            if (nProcessorCount == null)
                {
                nProcessorCount = Environment.ProcessorCount;
                }

            RaiseError($"Withoutlimits = {Withoutlimits}");
            RaiseError($"nProcessorCount = {nProcessorCount}");

            DefaultViewPort.NoOutMessages = NoOutMessagesDefault;

            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(true);
            if (Withoutlimits)
                {
                crowd.SetMaxDegreeOfParallelism(null);
                }
            else
                {
                crowd.SetMaxDegreeOfParallelism(nProcessorCount);
                }

            // var loggerOptions = new ActorLoggerOptions();
            // loggerOptions.SetLogLevel(ActorLogLevel.Error);

            var globalLoggerOptions = DefaultErrorLoggerOptions.ErrorLoggerOptions;

            for (int i = 0; i < nItemsCount; i++)
                {
                var name = string.Format(" ПРОСТОЙ-ОБЪЕКТ{0}", i + 1);
                //var actor = new SimpleActor(name);
                var actor = new WaitActor(name);
                actor.SetLoggerOptions(globalLoggerOptions);
                actor.Interval = 10;
                crowd.Add(actor);
                }

            using (var executor = new ActorExecutor(crowd, DefaultViewPort))
                {
                // crowd.BindViewPort(DefaultViewPort);
                // crowd.SetIMessageChannel(DefaultViewPort);
                await executor.RunAsync();
                } // end using

            //var crowd2 = new ActorsCrowd();
            //for (int i = 0; i < nItemsCount; i++)
            //    {
            //    var name = string.Format(" ВЛ-ОБЪЕКТ-{0}", i + 1);
            //    var actor = new SimpleActor(name);
            //    crowd2.Add(actor);
            //    }

            //crowd.Add(crowd2);

            //viewPort.OutMessages = false;
            // mci.RaiseMessages = false;
            //actorTime.SetStartDate();
            // await crowd.RunAsync();
            //actorTime.SetEndDate();

            //viewPort.OutMessages = true;
            // mci.RaiseMessages = true;

            //var result = actorTime.GetTimeIntervalWithComment(nItemsCount);
            //RaiseWarning(result);

            return 0;
            }

        //
        } // end class CrowdOptions
    } // end namespace ActorsCPConsoleRunner.Handlers