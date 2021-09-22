using System;

//#if DEBUG
//#define DEBUG_TPL_ERRORREPORTER
//#endif // DEBUG_TPL_ERRORREPORTER

using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Базовый класс вьюпорта со всеми интерфейсами
    /// и буферизованным выводом
    /// </summary>
    public class BufferedActorViewPortBase : ActorViewPortBase
        {
        #region Приватные мемберы

        /// <summary>
        /// Буфер сообщений
        /// </summary>
        private BufferBlock<ActorEventArgs> _tplDataFlowDataBufferBlock;

        /// <summary>
        /// Актор работы с ошибками
        /// </summary>
        private ActionBlock<ActorEventArgs> _tplDataFlowDataActionBlock;

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public BufferedActorViewPortBase()
            {
            InitTplDataFlow();
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Tpl завершен
        /// </summary>
        public bool TplDataFlowTerminated
            {
            get;
            private set;
            }

        #endregion Свойства

        #region Инициализация/Завершение

        /// <summary>
        /// Инициализация TplDataFlow
        /// </summary>
        public void InitTplDataFlow()
            {
            if (_tplDataFlowDataBufferBlock != null)
                {
                return;
                }

            _tplDataFlowDataBufferBlock = new BufferBlock<ActorEventArgs>();
            _tplDataFlowDataActionBlock = new ActionBlock<ActorEventArgs>(TplDataFlowProcessAction);
            _tplDataFlowDataBufferBlock.LinkTo(_tplDataFlowDataActionBlock);
            _tplDataFlowDataBufferBlock.Completion.ContinueWith(TplDataFlow_DataBufferBlock_Completion);
            _tplDataFlowDataActionBlock.Completion.ContinueWith(TplDataFlow_ActionBlock_Completion);

#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("InitTplDataFlow()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        /// <summary>
        /// Завершить TplDataFlow
        /// </summary>
        private void TerminateTplDataFlow()
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("TerminateTplDataFlow() - начало");
#endif // DEBUG_TPL_ERRORREPORTER

            _tplDataFlowDataBufferBlock?.Complete();
            if (_tplDataFlowDataActionBlock != null)
                {
                _tplDataFlowDataActionBlock.Complete();
                var task = _tplDataFlowDataActionBlock?.Completion;
                if (task != null && task.Status != TaskStatus.Faulted)
                    {
                    task.Wait();
                    }
                }

            if (_tplDataFlowDataBufferBlock != null)
                {
                var task = _tplDataFlowDataBufferBlock?.Completion;
                if (task != null && task.Status != TaskStatus.Faulted)
                    {
                    task.Wait();
                    }
                }
            _tplDataFlowDataBufferBlock = null;
            _tplDataFlowDataActionBlock = null;

            TplDataFlowTerminated = true;
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TerminateTplDataFlow() - конец");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        #endregion Инициализация/Завершение

        #region Завершители

        /// <summary>
        /// Завершение работы буфера
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_DataBufferBlock_Completion(Task task)
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TplDataFlow_DataBufferBlock_Completion()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        /// <summary>
        /// Завершение работы актора
        /// </summary>
        /// <param name="task">Задача завершения</param>
        private void TplDataFlow_ActionBlock_Completion(Task task)
            {
#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine($"TplDataFlow_ActionBlock_Completion()");
#endif // DEBUG_TPL_ERRORREPORTER
            }

        #endregion Завершители

        /// <summary>
        /// Добавить данные в очередь на обработку
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task TplDataFlowAddDataAsync(ActorEventArgs data)
            {
            if (data == null)
                {
                throw new ArgumentNullException(nameof(data), "data не может быть null");
                }
            await _tplDataFlowDataBufferBlock.SendAsync(data);
            }

        /// <summary>
        /// Обработать сообщение
        /// </summary>
        /// <param name="data">Данные</param>
        private void TplDataFlowProcessAction(ActorEventArgs data)
            {
            if (data == null)
                {
                return;
                }

#if DEBUG_TPL_ERRORREPORTER
            Debug.WriteLine("TplDataFlowProcessAction()");
#endif // DEBUG_TPL_ERRORREPORTER
            }
        }
    }