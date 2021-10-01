using System;
using System.Text;
using System.Windows.Forms;

using ActorsCP.Helpers;

namespace ActorsCP.dotNET.ViewPorts.Tree
    {
    /// <summary>
    /// Контейнер картинок для дерева
    /// </summary>
    public class TreeViewImageList : SingletonT<TreeViewImageList>
        {
        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        public TreeViewImageList()
            {
            }

        #endregion Конструкторы

        #region Свойства

        /// <summary>
        /// Список картинок
        /// </summary>
        public ImageList ImageList
            {
            get;
            } = new ImageList();

        #endregion Свойства

        //
        } // end class TreeViewImageList
    } // end namespace ActorsCP.dotNET.ViewPorts.Tree