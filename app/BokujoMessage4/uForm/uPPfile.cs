using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using BokujoMessage4.Format;
using Newtonsoft.Json;

namespace BokujoMessage4.uForm
{
    public partial class uPPfile : Form
    {
        internal papa ppFile;
        internal string path;

        public uPPfile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void cb_xbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = ppFile.getSelectedData(cb_xbb.SelectedIndex).Length + " byte";

            DynamicFileByteProvider dynamicFileByteProvider = null;

            try
            {

                dynamicFileByteProvider = new DynamicFileByteProvider(new MemoryStream(ppFile.getSelectedData(cb_xbb.SelectedIndex)));
            }
            catch
            {
            }

            hexBox1.ByteProvider = dynamicFileByteProvider;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = path;

            if (diag.ShowDialog() == DialogResult.OK)
            {
                path = Path.GetFullPath(diag.FileName);
                GC.Collect();

                FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
                BinaryReader _binaryread = new BinaryReader(_FileTemp);

                ppFile.setSelectedData(cb_xbb.SelectedIndex, _binaryread.ReadBytes((int)_FileTemp.Length));

                _binaryread.Close();
                _FileTemp.Close();
            }
        }

        private void ts_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = path;
            diag.Title = "Open BIN/PP Files ...";
            diag.Filter = "PP files (*.pp, *.bin) | *.pp; *.bin|All Files|*.*";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                path = Path.GetFullPath(diag.FileName);
                ppFile = null;
                GC.Collect();

                FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
                BinaryReader _binaryread = new BinaryReader(_FileTemp);
                ppFile = new papa(_binaryread.ReadBytes((int)_FileTemp.Length));
                _binaryread.Close();
                _FileTemp.Close();

                toolStripLabel1.Text = ppFile.Count + " files";

                cb_xbb.Items.Clear();

                for (int i = 0; i < ppFile.Count; i++)
                {
                    cb_xbb.Items.Add(i);
                }

                cb_xbb.SelectedIndex = 0;

                ts_saveas.Enabled = true;
                ts_unpack.Enabled = true;
                uxTabControl1.Enabled = true;
            }
        }

        private void ts_saveas_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save PP Files ...";
            diag.Filter = "PP files (*.pp) | *.pp";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                ppFile.RecreateContent();

                File.WriteAllBytes(diag.FileName, ppFile.Data);
            }
        }

        private void ts_unpack_Click(object sender, EventArgs e)
        {
            Ookii.Dialogs.VistaFolderBrowserDialog folderdialog = new Ookii.Dialogs.VistaFolderBrowserDialog();

            folderdialog.SelectedPath = Program.AConfig.FilePath[0];

            progressBar1.Value = 0;
            progressBar1.Maximum = (int)ppFile.Count;

            if (folderdialog.ShowDialog() == DialogResult.OK)
            {
                string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(path).Replace(Path.GetExtension(path),"") + @"\";

                if (!Directory.Exists(pathe))
                {
                    Directory.CreateDirectory(pathe);
                }

                Thread nTh = new Thread(
                    new ThreadStart(() =>
                    {
                        for (int i = 0; i < ppFile.Count; i++)
                        {
                            File.WriteAllBytes(pathe + i + ".bin", ppFile.getSelectedData(i));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value += 1;
                            }));
                        }

                        GC.Collect(9, GCCollectionMode.Forced);
                    }));

                nTh.Start();
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog targetFile = new SaveFileDialog();
            targetFile.Title = "Save PP File Location";
            OpenFileDialog inputFile = new OpenFileDialog();
            inputFile.Title = "Input File To pack";
            inputFile.Multiselect = true;

            if (targetFile.ShowDialog() != DialogResult.OK) return;
            if (inputFile.ShowDialog() != DialogResult.OK) return;

            string[] filelist = inputFile.FileNames;

            Array.Sort(filelist, new Utils.AlphanumComparatorFast());

            papa.Pack(targetFile.FileName, inputFile.FileNames);
        }
    }
}
