using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            ImageList.TransparentColor = System.Drawing.Color.Transparent;
            LoadList();
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

        #region Приватные методы

        /// <summary>
        /// Заполнить список картинками
        /// </summary>
        private void LoadList()
            {
            #region Пустой битмап в начале

            var nullBitmap = new Bitmap(16, 16);
            ImageList.Images.Add(nullBitmap);

            #endregion Пустой битмап в начале

            var images = new[]
                {
                    new { Index = TreeViewImageIndex.ActionNode, FileName ="Action.ActionNode.png" },
                    new { Index = TreeViewImageIndex.ActionDebug, FileName ="Action.ActionDebug.png" },
                    new { Index = TreeViewImageIndex.ActionSystemNeutral, FileName ="Action.ActionNeutral.png" },
                    new { Index = TreeViewImageIndex.ActionNeutral, FileName ="Action.ActionOrdinal.png" },
                    new { Index = TreeViewImageIndex.ActionWarning, FileName ="Action.ActionWarning.png" },
                    new { Index = TreeViewImageIndex.ActionError, FileName ="Action.ActionError.png" },
                    new { Index = TreeViewImageIndex.ActionException, FileName ="Action.ActionException.png" },

                    new { Index = TreeViewImageIndex.Actor_Pending, FileName ="Actor.Actor_Pending.png" },
                    new { Index = TreeViewImageIndex.Actor_Started, FileName ="Actor.Actor_Started.png" },
                    new { Index = TreeViewImageIndex.Actor_Running, FileName ="Actor.Actor_Running.png" },
                    new { Index = TreeViewImageIndex.Actor_Stopped, FileName ="Actor.Actor_Stopped.png" },
                    new { Index = TreeViewImageIndex.Actor_Terminated_OK, FileName ="Actor.Actor_Terminated_OK.png" },
                    new { Index = TreeViewImageIndex.Actor_Terminated_Failure, FileName ="Actor.Actor_Terminated_Failure.png" },
                    new { Index = TreeViewImageIndex.Actor_Cancel, FileName ="Actor.Actor_Cancel.png" }
                };

            var sortedList = images.OrderBy(x => x.Index);

            foreach (var imageDesc in sortedList)
                {
                var image = GetImageByName(imageDesc.FileName);
                AddImage(image);
                }
            }

        /// <summary>
        /// Загрузить картинку из каталога Images
        /// </summary>
        /// <param name="imageName">Имя файла, например ActionError.png</param>
        /// <returns></returns>
        private static Image GetImageByName(string imageName)
            {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetName().Name + ".Images.";
            var fullResourceName = resourceName + imageName;
            var st = assembly.GetManifestResourceStream(fullResourceName);
            if (st == null)
                {
                throw new ArgumentNullException($"Ошибка загрузки изображения '{imageName}'");
                }
            var bitmapToLoad = new Bitmap(st);
            return bitmapToLoad;
            }

        /// <summary>
        /// Добавить картику в конец
        /// </summary>
        private void AddImage(Image image)
            {
            if (image == null)
                {
                throw new ArgumentNullException($"{nameof(image)} не может быть null");
                }
            ImageList.Images.Add(image);
            }

        #endregion Приватные методы

        //
        } // end class TreeViewImageList
    } // end namespace ActorsCP.dotNET.ViewPorts.Tree