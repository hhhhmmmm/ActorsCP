using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActorsCP.Actors;
using ActorsCP.Tests.TestActors;
using ActorsCP.ViewPorts.ConsoleViewPort;
using CommandLine;
using CommandLine.Text;

namespace ActorsCPConsoleRunner.Handlers
    {
    public class SetHandler : HandlerBase
        {
        [Option('n', "number", Required = true, HelpText = "Количество элементов для обработки")]
        public int nItemsCount
            {
            get;
            set;
            }

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected override async Task<int> InternalRun()
            {
            if (nItemsCount <= 0)
                {
                RaiseError("count должно быть > 0");
                }

            ActorTime actorTime = default;

            actorTime.SetStartDate();
            var bres = await InternalRunForSet();
            actorTime.SetEndDate();
            DefaultViewPort.NoOutMessages = false;
            RaiseError($"nItemsCount = {nItemsCount}");
            var str = actorTime.GetTimeIntervalWithComment(nItemsCount);
            RaiseError(str);
            var stat = DefaultViewPort.СurrentExecutionStatistics.TextStatistics;
            var m = ConsoleViewPortStatics.CreateWarningMessage(stat);
            ConsoleViewPortStatics.WriteToConsole(m);
            DefaultViewPort.ValidateStatistics();
            return bres;
            }

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected virtual Task<int> InternalRunForSet()
            {
            return Task.FromResult(0);
            }
        }
    }