﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Tests.TestActors;
using ActorsCP.ViewPorts.ConsoleViewPort;

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

        [Option('c', "count", Required = true, HelpText = "Количество элементов для обработки в толпе")]
        public int nItemsCount
            {
            get;
            set;
            }

        [Option('l', "limit", Required = false, HelpText = "Ограничивать параллелизм")]
        public bool LimitParallelelism
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
        protected override async Task<int> InternalRun()
            {
            ActorTime actorTime = default;

            if (nItemsCount <= 0)
                {
                RaiseError("count должно быть > 0");
                }

            if (nProcessorCount == null)
                {
                nProcessorCount = Environment.ProcessorCount;
                }

            var viewPort = new ConsoleActorViewPort();
            // MessageChannelImplementation mci = MessageChannel as MessageChannelImplementation;

            RaiseWarning($"LimitParallelelism = {LimitParallelelism}");
            RaiseWarning($"nProcessorCount = {nProcessorCount}");

            var crowd = new ActorsCrowd();
            crowd.SetCleanupAfterTermination(true);

            crowd.BindViewPort(viewPort);

            // crowd.SetIMessageChannel(this);

            if (LimitParallelelism)
                {
                crowd.SetMaxDegreeOfParallelism(nProcessorCount);
                }
            else
                {
                crowd.SetMaxDegreeOfParallelism(null);
                }

            for (int i = 0; i < nItemsCount; i++)
                {
                string name = string.Format("Объект {0}", i + 1);
                var actor = new SimpleActor(name);
                crowd.Add(actor);
                }

            //viewPort.OutMessages = false;
            // mci.RaiseMessages = false;
            actorTime.SetStartDate();
            await crowd.RunAsync();
            actorTime.SetEndDate();
            var result = actorTime.GetTimeIntervalWithComment(nItemsCount);

            //viewPort.OutMessages = true;
            // mci.RaiseMessages = true;

            RaiseWarning($"nItemsCount = {nItemsCount}");
            RaiseWarning(result);
            return 0;
            }

        //
        } // end class CrowdOptions
    } // end namespace ActorsCPConsoleRunner.Handlers