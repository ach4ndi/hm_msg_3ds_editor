using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using BokujoMessage4.Format;
using Newtonsoft.Json;

namespace BokujoMessage4.uForm
{
    public partial class uXBB : Form
    {
        internal xbb xbbFile;
        internal string path;

        


        public uXBB()
        {
            GC.Collect();
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void cb_xbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = BitConverter.ToString(xbbFile.getselectedUniq(cb_xbb.SelectedIndex).uniqueId).Replace("-","");
            label3.Text = xbbFile.getSelectedfile(cb_xbb.SelectedIndex).Length + " byte";

            DynamicFileByteProvider dynamicFileByteProvider = null;

            try
            {

                dynamicFileByteProvider = new DynamicFileByteProvider(new MemoryStream(xbbFile.getSelectedfile(cb_xbb.SelectedIndex)));
            }
            catch
            {
            }

            hexBox1.ByteProvider = dynamicFileByteProvider;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = path;

            if (diag.ShowDialog() == DialogResult.OK)
            {
                path = Path.GetFullPath(diag.FileName);
                //xbbFile = null;
                GC.Collect();

                FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
                BinaryReader _binaryread = new BinaryReader(_FileTemp);

                xbbFile.setSelectedfile(cb_xbb.SelectedIndex, _binaryread.ReadBytes((int)_FileTemp.Length));

                _binaryread.Close();
                _FileTemp.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void formclose_view(object sender, FormClosedEventArgs e)
        {
            xbbFile = null;
            GC.Collect();
            this.Dispose(true);
        }

        private void formclosing_view(object sender, FormClosingEventArgs e)
        {
            xbbFile = null;
            GC.Collect();
            this.Dispose(true);
        }

        private void ts_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = path;
            diag.Title = "Open XBB Files ...";
            diag.Filter = "XBB files (*.xbb, *.bin) | *.xbb; *.bin|All Files|*.*";

            if (diag.ShowDialog() != DialogResult.OK) return;
            
            path = Path.GetFullPath(diag.FileName);
            xbbFile = null;
            GC.Collect();

            FileStream _FileTemp = new FileStream(diag.FileName, FileMode.Open);
            BinaryReader _binaryread = new BinaryReader(_FileTemp);
            xbbFile = new xbb();
            xbbFile.Load(_binaryread.ReadBytes((int)_FileTemp.Length));
            _binaryread.Close();
            _FileTemp.Close();

            tlb_files.Text = xbbFile.getCount() + " files";

            cb_xbb.Items.Clear();

            for (int i = 0; i < xbbFile.getCount(); i++)
            {
                cb_xbb.Items.Add(xbbFile.getFileName(i));
            }

            cb_xbb.SelectedIndex = 0;

            ts_saveas.Enabled = true;
            ts_unpack.Enabled = true;
            uxTabControl1.Enabled = true;
            
        }

        private void ts_saveas_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save XBB Files ...";
            diag.Filter = "XBB files (*.xbb) | *.xbb";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                xbbFile.RecreateContent();

                File.WriteAllBytes(diag.FileName, xbbFile.Data);
            }
        }

        private void ts_unpack_Click(object sender, EventArgs e)
        {
            Ookii.Dialogs.VistaFolderBrowserDialog folderdialog = new Ookii.Dialogs.VistaFolderBrowserDialog();

            folderdialog.SelectedPath = Program.AConfig.FilePath[0];

            progressBar1.Value = 0;
            progressBar1.Maximum = (int)xbbFile.getCount();

            if (folderdialog.ShowDialog() == DialogResult.OK)
            {
                string pathe = folderdialog.SelectedPath + @"\" + Path.GetFileName(path).Replace(Path.GetExtension(path), "") + @"\";

                if (!Directory.Exists(pathe))
                {
                    Directory.CreateDirectory(pathe);
                }

                Thread nTh = new Thread(
                    new ThreadStart(() =>
                    {
                        for (int i = 0; i < xbbFile.getCount(); i++)
                        {
                            File.WriteAllBytes(pathe + xbbFile.getFileName(i).Replace("\0", "") + "", xbbFile.getSelectedfile(i));

                            progressBar1.BeginInvoke(new Action(() =>
                            {
                                progressBar1.Value += 1;
                            }));
                        }

                        GC.Collect(9, GCCollectionMode.Forced);
                    }));

                nTh.Start();

                File.WriteAllText(pathe + @"\" + "uniqueIDList" + ".json", JsonConvert.SerializeObject(xbbFile.getselectedUniq(), Formatting.Indented), Encoding.UTF8);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog targetFile = new SaveFileDialog();
            targetFile.Title = "Save path xbb files";
            OpenFileDialog uniqueFile = new OpenFileDialog();
            uniqueFile.Title = "Input UniqueID Files";
            OpenFileDialog inputFile = new OpenFileDialog();
            inputFile.Title = "Input files to pack";
            inputFile.Multiselect = true;

            //Ookii.Dialogs.VistaFolderBrowserDialog pathDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();

            if (targetFile.ShowDialog() != DialogResult.OK) return;
            if (inputFile.ShowDialog() != DialogResult.OK) return;
            if (uniqueFile.ShowDialog() != DialogResult.OK) return;

            string[] filelist = inputFile.FileNames;

            Array.Sort(filelist, new Utils.AlphanumComparatorFast());

            xbb.Pack(targetFile.FileName, filelist, JsonConvert.DeserializeObject<XBBUQ[]>(File.ReadAllText(uniqueFile.FileName)));
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog targetFile = new SaveFileDialog();
            targetFile.Title = "Save File Location";
            targetFile.FileName = xbbFile.getFileName(cb_xbb.SelectedIndex);

            if (targetFile.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(targetFile.FileName, xbbFile.getSelectedfile(cb_xbb.SelectedIndex));
            
        }
    }
}
