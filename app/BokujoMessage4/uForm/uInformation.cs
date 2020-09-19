using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BokujoMessage4.Format;
using Newtonsoft.Json;
using OfficeOpenXml;
using Ookii.Dialogs;

namespace BokujoMessage4.uForm
{
    public partial class uInformation : Form
    {
        public uInformation()
        {
            InitializeComponent();
            label4.Text = label4.Text.Replace("<file>", Path.GetFileName(Program.AConfig.FilePath[0]));

            textBox1.Text = Program.AConfig.FilePath[0];
            label11.Text = "";

            comboBox5.Items.Clear();

            for (int i = 0; i < Program.mainXBB.getCount(); i++)
            {
                comboBox5.Items.Add(Program.mainXBB.getFileName(i));
            }

            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 2;
            comboBox5.SelectedIndex = 0;

            textBox13.TextAlign = HorizontalAlignment.Center;
            textBox14.TextAlign = HorizontalAlignment.Center;
            textBox15.TextAlign = HorizontalAlignment.Center;

            FileInfo fileinfo = new FileInfo(textBox1.Text);

            textBox2.Text = fileinfo.Name;
            textBox3.Text = fileinfo.Directory.ToString();
            textBox4.Text = fileinfo.Length +" byte";

            textBox5.Text = Program.mainXBB.getCount() + " file";

            comboBox1.SelectedIndex = 0;

            progressBar1.Value = 0;
            progressBar1.Maximum = (int) Program.mainXBB.getCount();

            GameSeries indexcombo = Utils.getSeries(Program.mainf.cb_method.SelectedIndex);

            Thread nTh = new Thread(
                    new ThreadStart(() =>
                    {
                        int counted = 0;
                        int countblank = 0;
                        int countmsgdata = 0;
                        int countspace = 0;
                        int countnewline = 0;
                        int countlineall = 0;

                        for (int i = 0; i < Program.mainXBB.getCount(); i++)
                        {
                            papa inputpapatemp = new papa(Program.mainXBB.getSelectedfile(i));

                            counted += (int)inputpapatemp.Count;
                            countblank++;

                            for (int j = 0; j < (inputpapatemp.Count - 1); j++)
                            {
                                msg4u msgtemp = new msg4u(inputpapatemp.getSelectedData(j), indexcombo);
                                string strtemp = msgtemp.GetTextSection();

                                countlineall += strtemp.Length - strtemp.Replace(Environment.NewLine, string.Empty).Length;
                                countmsgdata += strtemp.Length;
                                countspace += strtemp.Count(Char.IsWhiteSpace);
                                countnewline += strtemp.Count(Char.IsControl);
                            }

                            if (IsHandleCreated)
                            {
                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));
                            }
                            
                        }

                        this.BeginInvoke(new Action(() =>
                        {
                            progressBar1.Value = 0;
                            textBox6.Text = (counted - countblank) + " message, " + countblank + " blank";
                            textBox7.Text = countmsgdata + " character";

                            textBox13.Text = (countmsgdata - countspace - countnewline) + "";
                            textBox14.Text = countspace + "";
                            textBox15.Text = countnewline + "";

                            //textBox8.Text = (countmsgdata - countspace - countnewline) + " character without "+countspace+" space and "+countnewline+" controls";
                            textBox9.Text = countlineall+"";
                        }));

                        GC.Collect(9,GCCollectionMode.Forced);
                    }));

            nTh.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ookii.Dialogs.VistaFolderBrowserDialog folderdialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            VistaSaveFileDialog folderdialog2 = new VistaSaveFileDialog();
            string filename = "";

            GameSeries indexcombo = Utils.getSeries(Program.mainf.cb_method.SelectedIndex);

            switch (comboBox1.SelectedIndex)
            {
                case 6:
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(Program.AConfig.FilePath[0]).Replace(Path.GetExtension(Program.AConfig.FilePath[0]), "") + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainXBB.getCount();

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainXBB.getCount(); i++)
                            {
                                filename = Program.mainXBB.getFileName(i).Replace("\0", "");
                                papa tempapa = new papa(Program.mainXBB.getSelectedfile(i));

                                StringBuilder buildstr = new StringBuilder();

                                for (int j = 0; j < tempapa.Count - 1; j++)
                                {
                                    msg4u newMsg = new msg4u(tempapa.getSelectedData(j), indexcombo);

                                    buildstr.AppendLine("============================================");
                                    buildstr.AppendLine(newMsg.GetVariableSection()).Append("\n");
                                    buildstr.AppendLine("--------------------------------------------");
                                    buildstr.AppendLine(newMsg.GetTextSection()).Append("\n");
                                }

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + (i + 1) + " of " + Program.mainXBB.getCount();
                                }));

                                File.WriteAllText(pathe + @"\" + filename + ".txt", buildstr.ToString(), Encoding.UTF8);
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 0:
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;

                        string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(Program.AConfig.FilePath[0]).Replace(Path.GetExtension(Program.AConfig.FilePath[0]), "") + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainXBB.getCount();
                        
                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainXBB.getCount(); i++)
                            {
                                File.WriteAllBytes(pathe + Program.mainXBB.getFileName(i).Replace("\0", "") + ".pp", Program.mainXBB.getSelectedfile(i));

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Text = "Progress : " + i + " of " + Program.mainXBB.getCount();
                                }));
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 1:
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(Program.AConfig.FilePath[0]).Replace(Path.GetExtension(Program.AConfig.FilePath[0]), "") + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainXBB.getCount();

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainXBB.getCount(); i++)
                            {
                                filename = Program.mainXBB.getFileName(i).Replace("\0", "");
                                papa tempapa = new papa(Program.mainXBB.getSelectedfile(i));

                                ppmsg4 newtempjson = new ppmsg4();
                                newtempjson.Date = DateTime.Now;
                                newtempjson.Author = "BokujoMessage4 JSON generated files";
                                newtempjson.Content = new pmsg4[tempapa.Count - 1];

                                for (int j = 0; j < tempapa.Count - 1; j++)
                                {
                                    msg4u newMsg = new msg4u(tempapa.getSelectedData(j), indexcombo);

                                    newtempjson.Content[j].VardId = newMsg.GetVariableSection();
                                    newtempjson.Content[j].Text = newMsg.GetTextSection();
                                }

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + i + " of " + Program.mainXBB.getCount();
                                }));

                                File.WriteAllText(pathe + @"\" + filename + ".json", JsonConvert.SerializeObject(newtempjson, Formatting.Indented), Encoding.UTF8);
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 2:
                    filename = Program.mainf.cb_xbb.Text;
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = folderdialog.SelectedPath + @"\" + filename + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainPP.Count;

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainPP.Count - 1; i++)
                            {
                                msg4u newMsg = new msg4u(Program.mainPP.getSelectedData(i), indexcombo);

                                File.WriteAllText(pathe + newMsg.GetVariableSection() + ".txt", newMsg.GetTextSection(), Encoding.UTF8);

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + i + " of " + Program.mainPP.Count;
                                }));
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 3:
                    filename = Program.mainf.cb_xbb.Text;
                    folderdialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = folderdialog.SelectedPath + @"\" + filename + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainPP.Count;

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainPP.Count - 1; i++)
                            {
                                msg4u newMsg = new msg4u(Program.mainPP.getSelectedData(i), indexcombo);

                                File.WriteAllBytes(pathe + newMsg.GetVariableSection() + ".msg4", newMsg.GetMSGData());

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + i + " of " + Program.mainPP.Count;
                                }));
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 5:
                    folderdialog.SelectedPath = Program.AConfig.FilePath[0];

                    if (folderdialog.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(Program.AConfig.FilePath[0]).Replace(Path.GetExtension(Program.AConfig.FilePath[0]), "") + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainXBB.getCount();

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainXBB.getCount(); i++)
                            {
                                filename = Program.mainXBB.getFileName(i).Replace("\0", "");
                                papa tempapa = new papa(Program.mainXBB.getSelectedfile(i));

                                FileInfo newFile = new FileInfo(pathe + @"\" + filename + ".xlsx");
                                ExcelPackage pck = new ExcelPackage(newFile);

                                if (File.Exists(newFile.FullName))
                                {
                                    File.Delete(newFile.FullName);
                                }

                                var ws = pck.Workbook.Worksheets.Add(filename);

                                for (int j = 0; j < tempapa.Count - 1; j++)
                                {
                                    msg4u newMsg = new msg4u(tempapa.getSelectedData(j), indexcombo);

                                    ws.Cells["A" + (j+1)].Value = newMsg.GetVariableSection();
                                    ws.Cells["B" + (j+1)].Value = newMsg.GetTextSection();
                                }

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + (i+1) + " of " + Program.mainXBB.getCount();
                                }));

                                pck.Save();
                            }

                            label11.BeginInvoke(new Action(() =>
                            {
                                label11.Text = "";
                            }));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value = 0;
                            }));
                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();
                    }
                    break;
                case 4:
                    filename = Program.mainf.cb_xbb.Text;
                    folderdialog2.FileName = filename + ".json";
                    folderdialog2.Filter = "JSON files (*.json) | *.json";
                    folderdialog2.InitialDirectory = Program.AConfig.FilePath[0];

                    if (folderdialog2.ShowDialog() == DialogResult.OK)
                    {
                        uxTabControl1.Enabled = false;
                        string pathe = Path.GetDirectoryName(folderdialog2.InitialDirectory) + @"\";

                        if (!Directory.Exists(pathe))
                        {
                            Directory.CreateDirectory(pathe);
                        }

                        ppmsg4 newtempjson = new ppmsg4();
                        newtempjson.Date = DateTime.Now;
                        newtempjson.Author = "BokujoMessage4 JSON generated files";
                        newtempjson.Content = new pmsg4[Program.mainPP.Count - 1];

                        progressBar1.Value = 0;
                        progressBar1.Maximum = (int)Program.mainPP.Count;

                        Thread nTh = new Thread(
                        new ThreadStart(() =>
                        {
                            for (int i = 0; i < Program.mainPP.Count - 1; i++)
                            {
                                msg4u newMsg = new msg4u(Program.mainPP.getSelectedData(i), indexcombo);

                                newtempjson.Content[i].VardId = newMsg.GetVariableSection();
                                newtempjson.Content[i].Text = newMsg.GetTextSection();

                                progressBar1.BeginInvoke(new Action(() =>
                                {
                                    progressBar1.Value += 1;
                                }));

                                label11.BeginInvoke(new Action(() =>
                                {
                                    label11.Text = "Progress : " + i + " of " + Program.mainPP.Count;
                                }));
                            }

                            uxTabControl1.BeginInvoke(new Action(() =>
                            {
                                uxTabControl1.Enabled = true;
                            }));
                            

                            GC.Collect(9, GCCollectionMode.Forced);
                        }));

                        nTh.Start();

                        File.WriteAllText(folderdialog2.FileName, JsonConvert.SerializeObject(newtempjson, Formatting.Indented) , Encoding.UTF8);
                    }

                    break;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var papaFile = new papa();
            papaFile.LoadData(Program.mainXBB.getSelectedfile(comboBox5.SelectedIndex));

            textBox11.Text = (papaFile.Count-1)+"";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Open XLSX Files ...";
            diag.Filter = "Excell 2007 files (*.xlsx) | *.xlsx";

            if (diag.ShowDialog() != DialogResult.OK) return;

            var package = new ExcelPackage(new FileInfo(diag.FileName));

            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            textBox10.Text = diag.FileName + "";
            textBox12.Text = workSheet.Dimension.Rows + "";

            package.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox10.Text.Length != 0)
            {
                uSheet opdiag = new uSheet(textBox10.Text);
                opdiag.Icon = this.Icon;
                opdiag.StartPosition = FormStartPosition.CenterParent;

                opdiag.ShowDialog(this);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var package = new ExcelPackage(new FileInfo(textBox10.Text));

            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            int posVar = comboBox2.SelectedIndex;
            int posMsg = comboBox4.SelectedIndex;
            int posCMBMode = Program.mainf.cb_method.SelectedIndex;
            int posPP = comboBox5.SelectedIndex;

            Thread nTh = new Thread(
            new ThreadStart(() =>
            {
                for (int i = 0; i < workSheet.Dimension.Rows; i++)
                {
                    Program.mainf.resaveMSGtoPPArc(posCMBMode,workSheet.Cells[i + 1, posVar + 1].Text,
                        workSheet.Cells[i + 1, posMsg + 1].Text, i, posPP);
                }
            }));

            nTh.Start();

            GC.Collect(9, GCCollectionMode.Forced);
        }
    }
}
