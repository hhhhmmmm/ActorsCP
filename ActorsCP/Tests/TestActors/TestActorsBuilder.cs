using System;
using System.Collections.Generic;
using System.Text;

using ActorsCP.Actors;
using ActorsCP.Helpers;
using ActorsCP.Tests.ViewPorts;

namespace ActorsCP.Tests.TestActors
    {
    /// <summary>
    /// Билдер тестовых классов
    /// </summary>
    public class TestActorsBuilder
        {
        private readonly IMessageChannel _messageChannel;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="messageChannel">Канал сообщений</param>
        public TestActorsBuilder(IMessageChannel messageChannel)
            {
            _messageChannel = messageChannel;
            }

        #endregion Конструкторы

        /// <summary>
        /// Нстроить объект
        /// </summary>
        /// <param name="actor"></param>
        private void ConfigureActor(ActorBase actor)
            {
            actor.SetIMessageChannel(_messageChannel);
            }

        #region Свойства

        /// <summary>
        /// Новый пустой объект
        /// </summary>
        public SimpleActor NewSimpleActor
            {
            get
                {
                var actor = new SimpleActor();
                ConfigureActor(actor);
                return actor;
                }
            }

        /// <summary>
        /// Выбрасывает исключения при разных условиях
        /// </summary>
        public ExceptionActor NewExceptionActor
            {
            get
                {
                var actor = new ExceptionActor();
                ConfigureActor(actor);
                return actor;
                }
            }

        /// <summary>
        /// Не выбрасывает исключения при разных условиях, но завершается с ошибкой
        /// </summary>
        public NoExceptionFailureActor NewNoExceptionFailureActor
            {
            get
                {
                var actor = new NoExceptionFailureActor();
                ConfigureActor(actor);
                return actor;
                }
            }

        #endregion Свойства

        /// <summary>
        /// Создать список тестовых акторов
        /// S - SimpleActor
        /// E - ExceptionActor
        /// F - NoExceptionFailureActor
        /// </summary>
        /// <param name="type">тип</param>
        /// <param name="nCount">Количество</param>
        /// <returns></returns>
        public List<ActorBase> CreateListOfActors(char type, int nCount)
            {
            var list = new List<ActorBase>();
            for (int i = 1; i <= nCount; i++)
                {
                ActorBase actor;
                switch (type)
                    {
                    case ' ':
                    case '\t':
                        {
                        continue;
                        }

                    case 'S':
                        {
                        actor = NewSimpleActor;
                        break;
                        }
                    case 'E':
                        {
                        var _actor = NewExceptionActor;
                        _actor.ExceptionOnRun = true;
                        actor = _actor;
                        break;
                        }
                    case 'F':
                        {
                        actor = NewNoExceptionFailureActor;
                        break;
                        }
                    default:
                        {
                        throw new Exception($"Непонятный тип актора - '{type}'");
                        }
                    }
                list.Add(actor);
                }
            return list;
            }

        /// <summary>
        /// Создать список тестовых акторов
        /// S - SimpleActor
        /// E - ExceptionActor
        /// F - NoExceptionFailureActor
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<ActorBase> CreateListOfActors(string cmd)
            {
            var list = new List<ActorBase>();

            var str = cmd.ToUpper().Trim();
            int i = 0;
            foreach (char type in str)
                {
                i++;
                ActorBase actor = null;
                switch (type)
                    {
                    case ' ':
                    case '\t':
                        {
                        continue;
                        }

                    case 'S':
                        {
                        actor = NewSimpleActor;
                        break;
                        }
                    case 'E':
                        {
                        var _actor = NewExceptionActor;
                        _actor.ExceptionOnRun = true;
                        actor = _actor;
                        break;
                        }
                    case 'F':
                        {
                        actor = NewNoExceptionFailureActor;
                        break;
                        }
                    default:
                        {
                        throw new Exception($"Непонятный тип актора - '{type}'");
                        }
                    }
                list.Add(actor);
                }

            return list;
            }

        /// <summary>
        /// Создать Вьюпорт
        /// </summary>
        /// <returns></returns>
        public TestViewPortBase CreateViewPort()
            {
            var v = new TestViewPortBase();
            return v;
            }
        } // end class TestActorsBuilder
    } // end namespace ActorsCP.TestActors