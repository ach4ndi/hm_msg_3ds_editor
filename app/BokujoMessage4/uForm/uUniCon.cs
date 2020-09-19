using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BokujoMessage4.Format;
using BokujoMessage4.Format.gformat;

namespace BokujoMessage4.uForm
{
    public partial class uUniCon : Form
    {
        private papa papaFile;
        private bool checkReady = false;

        public uUniCon()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();

            if (diag.ShowDialog() != DialogResult.OK) return;

            checkReady = false;
            papaFile = new papa(File.ReadAllBytes(diag.FileName));
            RefreshLViewList();
        }

        private void RefreshLViewList(int indexselected = 0)
        {
            StringBuilder build = new StringBuilder();
            list_pp.Items.Clear();
            //1132

            for (int i = 0; i < papaFile.Count; i++)
            {
                uniConfigSOS2JP sp = new uniConfigSOS2JP(papaFile.getSelectedData(i));
                build.Append(sp.getDataStr()).Append(Environment.NewLine);
                list_pp.Items.Add(i);
            }
            textBox2.Text = build.ToString();
            Clipboard.SetText(build.ToString());
            list_pp.SelectedIndex = indexselected;
        }

        private void list_pp_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkReady = false;
            uniConfigSOS2JP sp = new uniConfigSOS2JP(papaFile.getSelectedData(list_pp.SelectedIndex));
            textBox1.Text = sp.getDataStr();
            checkReady = true;
        }
        
    }
}
