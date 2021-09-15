using System;
using System.Text;
using System.Threading.Tasks;

using ActorsCP.Helpers;

using CommandLine;

namespace ActorsCPConsoleRunner.Handlers
    {
    /// <summary>
    /// Базовый класс опций
    /// </summary>
    public class HandlerBase : IMessageChannel
        {
        #region Переменные

        /// <summary>
        /// Канал сообщений
        /// </summary>
        private IMessageChannel m_IMessageChannel;

        #endregion Переменные

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parameter"></param>
        public HandlerBase() // (object parameter) : base(parameter)
            {
            }

        #endregion Конструкторы

        #region Свойства

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose
            {
            get; set;
            }

        protected IMessageChannel MessageChannel
            {
            get
                {
                return m_IMessageChannel;
                }
            }

        #endregion Свойства

        #region Методы

        /// <summary>
        /// Установить указатель на канал сообщений
        /// </summary>
        /// <param name="imc">Канал сообщений</param>
        public void SetIMessageChannel(IMessageChannel imc)
            {
            m_IMessageChannel = imc;
            }

        /// <summary>
        /// Метод запуска
        /// </summary>
        public int Run()
            {
            var task = InternalRun();
            return task.Result;
            }

        #endregion Методы

        /// <summary>
        /// Метод запуска
        /// </summary>
        protected virtual Task<int> InternalRun()
            {
            Console.WriteLine($"InternalRun() - {this.ToString()}");
            return Task.FromResult(0);
            }

        #region Реализация интерфейса IMessageChannel

        /// <summary>
        /// Вывести сообщение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseMessage(string MessageText)
            {
            m_IMessageChannel?.RaiseMessage(MessageText);
            }

        /// <summary>
        /// Вывести предупреждение
        /// </summary>
        /// <param name="MessageText">Текст сообщения</param>
        public void RaiseWarning(string MessageText)
            {
            m_IMessageChannel?.RaiseWarning(MessageText);
            }

        /// <summary>
        /// Вывести сообщение об ошибке
        /// </summary>
        /// <param name="ErrorText">Текст сообщения об ошибке</param>
        public void RaiseError(string ErrorText)
            {
            m_IMessageChannel?.RaiseError(ErrorText);
            }

        public void RaiseDebug(string debugText)
            {
            m_IMessageChannel?.RaiseDebug(debugText);
            }

        public void RaiseException(Exception exception)
            {
            m_IMessageChannel?.RaiseException(exception);
            }

        #endregion Реализация интерфейса IMessageChannel

        //
        } // end class OptionsBase
    } // end namespace ActorsCPConsoleRunner.Handlers