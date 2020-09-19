using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Be.Windows.Forms;
using BokujoMessage4.Format;
using BokujoMessage4.uForm;
using BokujoMessage4.uForm.mFrm;
using FastColoredTextBoxNS;
using Newtonsoft.Json;

namespace BokujoMessage4
{
    public partial class MainForm : Form
    {
#if DEBUG
        public string TitleProgram = "BokujoMessage4 Editor - Debug";
        internal Random rnd;
#else
        private string TitleProgram = "BokujoMessage4 Editor "+ Assembly.GetExecutingAssembly().GetName().Version;
        internal Random rnd;
#endif

        //public string path;

        internal int lastxbbselect = 0;
        internal int lastppselect = 0;

        public MainForm()
        {
            InitializeComponent();
            LoadConfig();

            Program.PopMenu = new AutocompleteMenu(ip_mes);
            Utils.BuildAutocompleteMenu(Program.PopMenu);

            this.Text = TitleProgram;
            this.Icon = Icon.FromHandle(Properties.Resources.icon_a_08.GetHicon());
            toolStripComboBox1.SelectedIndex = 0;
        }

        public void LoadConfig()
        {
            ip_mes.PreferredLineWidth = Program.AConfig.TextEditorCfg.prefLineLenght;
            cb_method.SelectedIndex = Program.AConfig.DefaultSeriesSelection;
        }


        #region OpenButton and Runs
        private void openMsgxbbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = Program.AConfig.FilePath[0];
            diag.Title = "Open XBB Files ...";
            diag.Filter = "XBB files (*.xbb, *.bin) | *.xbb; *.bin|All Files|*.*";

            if (diag.ShowDialog() != DialogResult.OK) return;
            LoadXBBMain(diag.FileName,diag.SafeFileName);

            Program.AConfig.FilePath[0] = diag.FileName;
        }

        internal void LoadXBBMain(string paths, string spath)
        {
            Program.AConfig.FilePath[0] = Path.GetFullPath(paths);
            Program.mainXBB = null;
            Program.mainPP = null;
            Program.mainMSG = null;
            GC.Collect();

            Load2XBBPrograms(paths, spath);
            LoadEnviromentAfterReadXBB(spath);
        }

        internal void Load2XBBPrograms(string path, string spath)
        {
            FileStream _FileTemp = new FileStream(path, FileMode.Open);
            BinaryReader _binaryread = new BinaryReader(_FileTemp);
            Program.mainXBB = new xbb();
            Program.mainXBB.Load(_binaryread.ReadBytes((int)_FileTemp.Length));
            _binaryread.Close();
            _FileTemp.Close();
        }

        internal void LoadEnviromentAfterReadXBB(string spath)
        {
            this.cb_method.Visible = true;
            this.cb_xbb.Visible = true;
            this.split_main0.Visible = true;
            this.BackgroundImage = null;

            saveMsgxbbToolStripMenuItem.Enabled = true;
            propertiesToolStripMenuItem.Enabled = true;

            propertiesToolStripMenuItem.Visible = true;
            toolStripSeparator10.Visible = true;
            toolStripSeparator1.Visible = true;
            RefreshComboBoxList(cb_xbb, (int)Program.mainXBB.getCount());

            this.Text = TitleProgram + " [" + spath + "]";

            lb_01.Text = cb_xbb.SelectedIndex + "/" + Program.mainXBB.getCount() + " |";
        }
        #endregion


        private void RefreshComboBoxList(ComboBox box, int Count)
        {
            box.Items.Clear();

            for (int i = 0; i < Count; i++)
            {
                box.Items.Add(Program.mainXBB.getFileName(i));
            }

            box.SelectedIndex = 0;
        }

        private void cb_xbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.mainPP = new papa();
            Program.mainPP.LoadData(Program.mainXBB.getSelectedfile(cb_xbb.SelectedIndex));
            RefreshLViewList();
            GC.Collect();
            lastxbbselect = cb_xbb.SelectedIndex;

            lb_01.Text = cb_xbb.SelectedIndex + "/" + Program.mainXBB.getCount() + " |";
            lb_02.Text = (list_pp.SelectedIndex + 1) + "/" + (Program.mainPP.Count - 1) + "/" + Program.mainPP.Count;
        }

        private void RefreshLViewList(int indexselected = 0)
        {
            list_pp.Items.Clear();

            for (int i = 0; i < Program.mainPP.Count - 1; i++)
            {
                msg4u mtemp = new msg4u(Program.mainPP.getSelectedData(i), Utils.getSeries(cb_method.SelectedIndex));

                list_pp.Items.Add(mtemp.GetVariableSection());
            }

            list_pp.SelectedIndex = indexselected;
        }

        private void list_pp_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();

            Program.mainMSG = new msg4u(Program.mainPP.getSelectedData(list_pp.SelectedIndex), Utils.getSeries(cb_method.SelectedIndex));
            lb_02.Text = (list_pp.SelectedIndex+1) + "/" + (Program.mainPP.Count-1)+"/"+ Program.mainPP.Count;

            DynamicFileByteProvider dynamicFileByteProvider = null;

            try
            {
                dynamicFileByteProvider = new DynamicFileByteProvider(new MemoryStream(Program.mainMSG.GetMSGData()));
            }
            catch
            {
            }

            ip_hexview.ByteProvider = dynamicFileByteProvider;

            ip_var.Text = Program.mainMSG.GetVariableSection();
            ip_mes.Text = Program.mainMSG.GetTextSection();
            ip_viewer.Text = Program.mainMSG.GetTextSection();
            lb_txcount_0.Text = Program.mainMSG.GetTextSection().Length +" character.";
            GC.Collect();
            lastppselect = list_pp.SelectedIndex;
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            resaveMSGtoPPArc();
        }

        private void resaveMSGtoPPArc()
        {
            Program.mainMSG.Create(Utils.getSeries(cb_method.SelectedIndex), ip_var.Text, ip_mes.Text, true);
            Program.mainPP.setSelectedData(list_pp.SelectedIndex, Program.mainMSG.GetMSGData());
            Program.mainPP.RecreateContent();
            Program.mainXBB.setSelectedfile(cb_xbb.SelectedIndex, Program.mainPP.Data);
        }

        internal void resaveMSGtoPPArc(int o,string a, string b, int c, int d)
        {
            Program.mainMSG.Create(Utils.getSeries(o), a, b, true);
            Program.mainPP.setSelectedData(c, Program.mainMSG.GetMSGData());
            Program.mainPP.RecreateContent();
            Program.mainXBB.setSelectedfile(d, Program.mainPP.Data);
        }

        private void saveMsgxbbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save XBB Files ...";
            diag.Filter = "XBB files (*.xbb) | *.xbb";
            diag.InitialDirectory = Program.AConfig.FilePath[1];

            if (diag.ShowDialog() == DialogResult.OK)
            {
                Program.mainXBB.RecreateContent();

                File.WriteAllBytes(diag.FileName, Program.mainXBB.Data);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.StartPosition = FormStartPosition.CenterParent;
            about.Icon = this.Icon;
            about.ShowDialog();
            /*
            MessageBox.Show(
                "BokujoMonogatari Message4 "+ Assembly.GetExecutingAssembly().GetName().Version + " \n\n" +
                "Created by Andi (Leafie)\n\n" +
                "Tools for Translation Msg.xbb on\n" +
                "- Story of Seasons US/EU\n" +
                "- Story of Seasons JP\n" +
                "- Story of Seasons 2 JP\n" +
                "- ANB JP/US/EU\n\n" +
                "Not Tested On:\n" +
                "- Story of Seasons KOR\n\n"+
                "Credit :\n" +
                "- walpaper : http://wallpapercave.com/black-elegant-wallpaper \n" +
                "- fugue icon : http://p.yusukekamiyamane.com/ \n" +
                "- see more on readme.txt file", "About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);*/
        }

        private string checknum(int input)
        {
            if (input == 0)
            {
                return "";
                
            }
            else if (input < 0)
            {
                return " (" + input + ")";
            }
            else
            {
                return " (" + "+" + input+")";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = Translate(ip_mes.Text, "auto", toolStripComboBox1.Text);
        }

        #region Inserting MSG Code
        private void insertString(string input)
        {
            var selectionIndex = ip_mes.SelectionStart;
            ip_mes.Text = ip_mes.Text.Insert(selectionIndex, input);
            ip_mes.SelectionStart = selectionIndex + input.Length;
        }

        private void insertTagString(string input)
        {
            var selectionIndex = ip_mes.SelectionStart;
            string stringselected = ip_mes.SelectedText;
            ip_mes.Text = ip_mes.Text.Insert(selectionIndex, "<" + input + ">");
            ip_mes.Text = ip_mes.Text.Insert(selectionIndex + stringselected.Length + (input.Length + 2), "</" + input + ">");
        }

        private void mYNAMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<MYNAME>");
        }

        private void bLUEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("BLUE");
        }

        private void rEDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("RED");
        }

        private void bLACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("BLACK");
        }

        private void gREENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("GREEN");
        }

        private void wHITEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("WHITE");
        }

        private void oRANGEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("ORANGE");
        }

        private void yELLOWGREENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("Y_GREEN");
        }

        private void lIGHTBLUEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("L_BLUE");
        }

        private void pURPLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("PURPLE");
        }

        private void pINKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTagString("PINK");
        }

        private void objectNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<MOJI0>");
            
        }

        private void objectName2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<MOJI1>");
        }

        private void objectName3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<MOJI2>");
        }

        private void newPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("\n<PAGE>\n");
        }

        private void charaName1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<CHARANAME0>");
        }

        private void charaName2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<CHARANAME1>");
        }

        private void charaName3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertString("<CHARANAME2>");
        }
#endregion

        public static string Translate(string text, string from, string to)
        {
            string page = null;
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                wc.Encoding = Encoding.UTF8;

                string url = string.Format(@"http://translate.google.com/m?hl=en&sl={0}&tl={1}&ie=UTF-8&prev=_m&q={2}",
                                            from, to, Uri.EscapeUriString(text));

                page = wc.DownloadString(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            page = page.Remove(0, page.IndexOf("<div dir=\"ltr\" class=\"t0\">")).Replace("<div dir=\"ltr\" class=\"t0\">", "");
            int last = page.IndexOf("</div>");
            page = page.Remove(last, page.Length - last);

            return page.Replace("&lt;","<").Replace("&gt;", ">");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Program.mainPP.removeSelectedfile(list_pp.SelectedIndex);
            Program.mainXBB.setSelectedfile(cb_xbb.SelectedIndex, Program.mainPP.Data);
            RefreshLViewList();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            msg4u newMsg = new msg4u();
            newMsg.Create(Utils.getSeries(cb_method.SelectedIndex),"VarNew_"+rnd.Next(824,9000),"", true);

            Program.mainPP.addSelectedfile(newMsg.GetMSGData());
            Program.mainXBB.setSelectedfile(cb_xbb.SelectedIndex, Program.mainPP.Data);
            RefreshLViewList((int) Program.mainPP.Count - 2);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Open Msg Files ...";
            diag.Filter = "bokujomsg4 file (*.msg4) | *.msg4";
            diag.InitialDirectory = Program.AConfig.FilePath[2];

            if (diag.ShowDialog() == DialogResult.OK)
            {
                GC.Collect();

                FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
                BinaryReader _binaryread = new BinaryReader(_FileTemp);

                Program.mainPP.addSelectedfile(_binaryread.ReadBytes((int)_FileTemp.Length));

                _binaryread.Close();
                _FileTemp.Close();

                Program.mainXBB.setSelectedfile(cb_xbb.SelectedIndex, Program.mainPP.Data);
                RefreshLViewList((int)Program.mainPP.Count - 2);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.FileName = list_pp.SelectedItem.ToString()+".msg4";
            diag.Title = "Save msg Files ...";
            diag.Filter = "bokujomsg4 file (*.msg4) | *.msg4";
            diag.InitialDirectory = Program.AConfig.FilePath[3];

            if (diag.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(diag.FileName, Program.mainPP.getSelectedData(list_pp.SelectedIndex));
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Open Msg Files ...";
            diag.Filter = "bokujomsg4 file (*.msg4) | *.msg4";
            diag.InitialDirectory = Program.AConfig.FilePath[3];

            if (diag.ShowDialog() == DialogResult.OK)
            {
                GC.Collect();

                FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
                BinaryReader _binaryread = new BinaryReader(_FileTemp);

                Program.mainPP.setSelectedData(list_pp.SelectedIndex,_binaryread.ReadBytes((int)_FileTemp.Length));
                Program.mainPP.RecreateContent();
                _binaryread.Close();
                _FileTemp.Close();

                Program.mainXBB.setSelectedfile(cb_xbb.SelectedIndex, Program.mainPP.Data);
                RefreshLViewList(list_pp.SelectedIndex);
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uInformation info = new uInformation();
            info.StartPosition = FormStartPosition.CenterParent;
            info.Icon = this.Icon;
            info.ShowDialog();
        }

        private void swapTextVarTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string a = ip_var.Text;
            string b = ip_mes.Text;

            ip_var.Text = b;
            ip_mes.Text = a;

            resaveMSGtoPPArc();
        }

        private void ip_mes_tx_changed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (Program.mainMSG != null)
            {
                lb_txcount_0.Text = ip_mes.Text.Length + checknum(ip_mes.Text.Length - Program.mainMSG.GetTextSection().Length) + " character";


                msg4u mainMSG0 = new msg4u();

                if (cb_method.SelectedIndex == 2)
                {
                    mainMSG0.Create(Utils.getSeries(cb_method.SelectedIndex), ip_var.Text, ip_mes.Text, true);


                    textBox1.Text = BitConverter.ToInt32(mainMSG0.GetEUSection(), 0) + "";
                    textBox2.Text = Program.mainMSG.isHaveMessage + "";
                }
                else
                {
                    mainMSG0.Create(Utils.getSeries(cb_method.SelectedIndex), ip_var.Text, ip_mes.Text, true);
                    textBox1.Text = "";
                    textBox2.Text = Program.mainMSG.isHaveMessage + "";
                }

                DynamicFileByteProvider dynamicFileByteProvider = null;

                try
                {

                    dynamicFileByteProvider = new DynamicFileByteProvider(new MemoryStream(mainMSG0.GetMSGData()));
                }
                catch
                {
                }

                ip_hexview.ByteProvider = dynamicFileByteProvider;

                lb_txpos_0.Text = "Position: " + ip_mes.SelectionStart + " | " + ip_mes.LinesCount + " Line | "+ mainMSG0.GetMSGData().Length+"/"+Program.mainMSG.GetMSGData().Length +" byte" ;
            }
        }

        private void ip_mes_selection(object sender, EventArgs e)
        {
            if (Program.mainMSG != null)
            {
                lb_txpos_0.Text = "Position: " + ip_mes.SelectionStart + " | " + ip_mes.LinesCount + " Line | " +
                                  Program.mainMSG.GetMSGData().Length + " byte";
            }
            else
            {
                lb_txpos_0.Text = "Position: " + ip_mes.SelectionStart ;
            }
        }

        private void xBBUnpackReplacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uXBB vXBB = new uXBB();
            vXBB.StartPosition = FormStartPosition.CenterParent;
            vXBB.Icon = this.Icon;
            vXBB.ShowDialog(this);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ip_mes.PreferredLineWidth = (int) numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 48;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 36;
        }

        private void pPUnpackPackReplacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uPPfile vXBB = new uPPfile();
            vXBB.StartPosition = FormStartPosition.CenterParent;
            vXBB.Icon = this.Icon;
            vXBB.ShowDialog(this);
        }

        private void configuratorEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ppItemData vXBB = new ppItemData();
            vXBB.StartPosition = FormStartPosition.CenterParent;
            vXBB.Icon = this.Icon;
            vXBB.ShowDialog(this);
        }

        private void configPPViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uUniCon vXBB = new uUniCon();
            vXBB.StartPosition = FormStartPosition.CenterParent;
            vXBB.Icon = this.Icon;
            vXBB.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //text to search
            var text = textBox3.Text;
            //search ranges
            var list = ip_mes.GetRanges(Regex.Escape(text)).Select(r => new Utils.SearchResult(r)).ToList();
            //set list to listBox
            listBox1.DataSource = list;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                var range = (listBox1.SelectedItem as Utils.SearchResult).Range;
                ip_mes.Selection = range;
                ip_mes.DoSelectionVisible();
            }
        }

        private void FormMain_Close(object sender, FormClosedEventArgs e)
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Program.AConfig, Formatting.Indented));
        }
    }
}
