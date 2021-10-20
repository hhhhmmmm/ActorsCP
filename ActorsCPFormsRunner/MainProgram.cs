using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ActorsCP.Logger;
using ActorsCP.Options;

namespace ActorsCPFormsRunner
    {
    internal static class MainProgram
        {
        public static Icon MainIcon
            {
            get;
            set;
            }

        public static Icon LoadIcon(string IconName)
            {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = "ActorsCPFormsRunner." + IconName;

            // var names = assembly.GetManifestResourceNames();
            var st = assembly.GetManifestResourceStream(fullResourceName);
            if (st == null)
                {
                return null;
                }
            var iconToLoad = new Icon(st);

            return iconToLoad;
            }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
            {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainIcon = LoadIcon("MainIcon.ico");
            ActorsCP.dotNET.Globals.ApplicationIcon = MainIcon;

            MainForm mf = new MainForm();
            mf.Icon = MainIcon;

            var logger = ActorLoggerImplementation.GetInstance();
            logger.InitLog("ActorsCPFormsRunner");
            logger.SetLogLevel(ActorLogLevel.Debug);
            ActorLoggerImplementation.ConfigureNLogGlobally(logger);

            GlobalActorLogger.SetGlobalLoggerInstance(logger);

            logger.LogInfo("Настройка логгера завершена");

            var debugOptions = GlobalActorDebugOptions.GetInstance();
            debugOptions.AddOrUpdate(ActorDebugKeywords.QueueBufferT_Debug, true);

            Application.Run(mf);
            }
        }
    }