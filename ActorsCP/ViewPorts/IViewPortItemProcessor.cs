using System;
using System.Collections.Generic;
using System.Text;

namespace ActorsCP.ViewPorts
    {
    /// <summary>
    /// Обработчик элемента ViewPortItem
    /// </summary>
    public interface IViewPortItemProcessor
        {
        /// <summary>
        /// Обработать элемент типа ViewPortItem
        /// </summary>
        /// <param name="item">элемент типа ViewPortItem</param>
        void ProcessViewPortItem(ViewPortItem item);
        } // end interface IViewPortItemProcessor
    } // end namespace ActorsCP.ViewPorts