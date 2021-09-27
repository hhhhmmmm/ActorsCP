﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

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

            MainForm mf = new MainForm();
            mf.Icon = MainIcon;

            Application.Run(mf);
            }
        }
    }