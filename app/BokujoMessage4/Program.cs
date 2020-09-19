using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BokujoMessage4.Format;
using FastColoredTextBoxNS;
using Newtonsoft.Json;

namespace BokujoMessage4
{
    static class Program
    {
        internal static MainForm mainf;
        internal static appconfig AConfig;

        internal static xbb mainXBB;
        internal static papa mainPP;
        internal static msg4u mainMSG;

        internal static AutocompleteMenu PopMenu;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!File.Exists("config.json") || new FileInfo("config.json").Length == 0)
            {
                AConfig = new appconfig();
                AConfig.DefaultSeriesSelection = 0;
                AConfig.GeneralFullPath = Application.StartupPath;
                AConfig.KeyWords = new[] {"<PAGE>", "<BLUE>", "</BLUE>", "</RED>", "<RED>"};

                AConfig.FilePath = new string[10];
                AConfig.FilePath[0] = AConfig.GeneralFullPath;
                AConfig.TextEditorCfg = new TextEditorConfig();
                AConfig.TextEditorCfg.prefLineLenght = 48;
                AConfig.SeedRNG = 8937;

                File.WriteAllText("config.json", JsonConvert.SerializeObject(AConfig, Formatting.Indented));
            }
            else
            {
                AConfig =
                    JsonConvert.DeserializeObject<appconfig>(
                        File.ReadAllText(Application.StartupPath + @"\config.json"));
            }

            mainf = new MainForm();

            if (File.Exists("bg.png"))
            {
                mainf.BackgroundImage = Image.FromFile("bg.png");
            }
            else
            {
                mainf.rnd = new Random(AConfig.SeedRNG);
                mainf.rnd.Next(0, 16);
                Utils.setRandomBackGroundPanel(mainf.rnd.Next(0, 16), mainf);
            }

            Application.Run(mainf);
        }
    }
}
