using System;
using System.Collections.Generic;
using System.Linq;
using ActorsCP.Actors;
using System.Windows.Forms;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Часть с таймером
    /// </summary>
    partial class FormsViewPortBase
        {
        /// <summary>
        /// Время выполнения
        /// </summary>
        private ActorTime _executionTime = default;

        /// <summary>
        /// Статистика выполнения
        /// </summary>
        private ExecutionStatistics _currentExecutionStatistics;

        /// <summary>
        /// Последняя сохраненная статистика выполнения
        /// </summary>
        private ExecutionStatistics _lastSavedExecutionStatistics;

        /// <summary>
        /// Таймер событий (для отрисовки всех событий излучаемых акторами)
        /// </summary>
        private Timer _stateTimer = new Timer();

        /// <summary>
        /// Показывает, какой список изображений использовать
        /// </summary>
        private bool TimerSwitch = false;

        /// <summary>
        /// Запустить таймер
        /// </summary>
        private void SetActorTimer()
            {
            _stateTimer.Enabled = true;
            _stateTimer.Interval = 330;
            _stateTimer.Tick += OnStateTimer;
            _stateTimer.Start();
            }

        /// <summary>
        /// Уничтожить таймер
        /// </summary>
        private void KillStateTimer()
            {
            if (_stateTimer != null)
                {
                _stateTimer.Tick -= OnStateTimer;
                _stateTimer.Stop();
                _stateTimer.Dispose();
                _stateTimer = null;
                }
            }

        /// <summary>
        /// Событие таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateTimer(object sender, EventArgs e)
            {
            if (_viewPort == null)
                {
                return;
                }
            TimerSwitch = !TimerSwitch;
            _currentExecutionStatistics = _viewPort.СurrentExecutionStatistics;
            SetTitle();
            SetStatistics(); // OnStateTimer
            }

        /// <summary>
        /// Получить дополнительный текст для статистики
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAdditionalStatisticsText()
            {
            return null;
            }

        /// <summary>
        /// Обновить статистику
        /// </summary>
        private void SetStatistics()
            {
            if (_currentExecutionStatistics != _lastSavedExecutionStatistics)
                {
                _lastSavedExecutionStatistics = _currentExecutionStatistics;
                string statistics = _lastSavedExecutionStatistics.GetStatistics();

                string additionalStatisticsText = GetAdditionalStatisticsText();
                if (!string.IsNullOrEmpty(additionalStatisticsText))
                    {
                    statistics = statistics + ", " + additionalStatisticsText;
                    }

                if (!StatisticsLabel.Text.Equals(statistics, StringComparison.OrdinalIgnoreCase))
                    {
                    StatisticsLabel.Text = statistics;
                    }
                }
            }
        }
    }