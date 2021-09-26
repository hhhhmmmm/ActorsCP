using System;
using System.Collections.Generic;
using System.Linq;
using ActorsCP.Actors;
using System.Windows.Forms;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Часть с таймером
    /// </summary>
    partial class FormsViewPortBase
        {
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
            TimerSwitch = !TimerSwitch;
            SetTitle();
            }
        }
    }