using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ActorsCP.dotNET.Helpers
    {
    /// <summary>
    /// Вспомогательный клас для работы с XML
    /// </summary>
    public static class XmlHelper
        {
        /// <summary>
        /// Функция для переформатирования тектса
        /// </summary>
        /// <param name="Text">Исходный текст</param>
        /// <returns>Форматированный текст</returns>
        public static string ReformatXml(string Text)
            {
            var doc = new XmlDocument();
            doc.LoadXml(Text);

            var xws = new XmlWriterSettings();
            xws.CheckCharacters = true;
            xws.Indent = true;
            xws.IndentChars = "    ";
            xws.NewLineHandling = NewLineHandling.Replace;

            //xws.Encoding= Encoding.UTF8;
            //xws.NewLineOnAttributes = true;
            xws.NewLineChars = "\r\n";

            //xws.CloseOutput = true;

            var sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, xws))
                {
                doc.Save(writer);
                }

            var Result = sb.ToString();
            return Result;
            }

        /// <summary>
        /// Удалить корневой элемент
        /// </summary>
        /// <param name="XmlText">Текст</param>
        /// <returns></returns>
        public static string RemoveRootElement(string XmlText)
            {
            var xdoc = new XmlDocument();
            xdoc.LoadXml(XmlText);

            var FirstChild = xdoc.DocumentElement.FirstChild;

            // new document
            var docNew = new XmlDocument();

            // we must use ImportNode, that's the key to your conversion
            var newNode = docNew.ImportNode(FirstChild, true);

            // now you can set it as the document element's first child this only works if it is an
            // element, otherwise you need to create a root node first
            docNew.AppendChild(newNode);
            var ssss = docNew.OuterXml;
            return ssss;
            }

        /// <summary>
        /// Настройки XML lWriter
        /// </summary>
        /// <returns></returns>
        public static XmlWriterSettings GetXmlWriterSettings()
            {
            var xws = new XmlWriterSettings();
            xws.CheckCharacters = true;
            xws.Indent = true;
            xws.IndentChars = "    ";
            xws.NewLineHandling = NewLineHandling.Replace;

            //xws.Encoding= Encoding.GetEncoding("Windows-1251");
            //xws.NewLineOnAttributes = true;
            xws.NewLineChars = Environment.NewLine;
            return xws;
            }

        /// <summary>
        /// Получить XML документ с удаленным корневым элементом
        /// </summary>
        /// <param name="element">Объект</param>
        /// <returns></returns>
        public static string GetXmlRepresentation<ElementType>(ElementType element)
            {
            var builder = new StringBuilder();
            var settings = GetXmlWriterSettings();
            XmlSerializer xmlSerializer = null;
            xmlSerializer = new XmlSerializer(typeof(ElementType));

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
                {
                xmlSerializer.Serialize(writer, element);
                }

            var XmlText = builder.ToString();
            return XmlText;
            }

        /// <summary>
        /// Получить XML документ с удаленным корневым элементом
        /// </summary>
        /// <param name="element">Объект</param>
        /// <returns></returns>
        public static string ExcludeRootElement<ElementType>(ElementType element)
            {
            var XmlText = GetXmlRepresentation<ElementType>(element);
            var Result = XmlHelper.RemoveRootElement(XmlText);
            return Result;
            }

        /// <summary>
        /// Создает XmlReader из строки содержащей текст XML
        /// </summary>
        /// <param name="xml">текст XML всего сообщения SOAP</param>
        /// <returns></returns>
        public static XmlReader XmlReaderFromString(string xml)
            {
            var stream = new MemoryStream();

            // NOTE: don't use using(var writer ...){...} because the end of the StreamWriter's using
            //       closes the Stream itself
            var writer = new System.IO.StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;
            return XmlReader.Create(stream);
            }

        /// <summary>
        /// Создает XmlReader из строки содержащей текст XML
        /// </summary>
        /// <param name="xml">текст XML всего сообщения SOAP</param>
        /// <returns></returns>
        public static XmlReader XmlReaderFromString2(string xml)
            {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlReader xr = new XmlNodeReader(doc);
            return xr;
            }
        }
    }