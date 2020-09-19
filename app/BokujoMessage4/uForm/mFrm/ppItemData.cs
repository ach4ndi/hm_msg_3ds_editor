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

namespace BokujoMessage4.uForm.mFrm
{
    public partial class ppItemData : Form
    {
        private papa papaFile;
        private Format.gformat.ppItemData ppdatay;
        private bool checkReady = false;

        public ppItemData()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;

            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkReady)
            {
                papaFile.setSelectedData(list_pp.SelectedIndex,
                    ppdatay.GetData(getSeries(comboBox1.SelectedIndex),
                    textBox1.Text, textBox2.Text, textBox3.Text,uint.Parse(textBox4.Text)
                    , uint.Parse(textBox5.Text), uint.Parse(textBox6.Text), uint.Parse(textBox7.Text)
                    , uint.Parse(textBox8.Text), uint.Parse(textBox9.Text), uint.Parse(textBox10.Text)
                    , uint.Parse(textBox11.Text), uint.Parse(textBox12.Text), uint.Parse(textBox13.Text)));
                papaFile.RecreateContent();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();

            if (diag.ShowDialog() != DialogResult.OK) return;

            checkReady = false;
            papaFile = new papa(File.ReadAllBytes(diag.FileName));
            RefreshLViewList();
        }

        internal GameSeries getSeries(int i)
        {
            /*
             * SOS 2 JP
            SOS 1 JP
            SOS 1 US
            SOS 1 EU
            ANB JP
            ANB US
            ANB EU
            SOS 1 KOR
            */
            switch (i)
            {
                case 0:
                    return GameSeries.SOS2JP;
                case 1:
                    return GameSeries.SOSJP;
                case 2:
                    return GameSeries.SOSUS;
                case 3:
                    return GameSeries.SOSEU;
                case 4:
                    return GameSeries.ANBJP;
                case 5:
                    return GameSeries.ANBUS;
                case 6:
                    return GameSeries.ANBEU;
                case 7:
                    return GameSeries.SOSKOR;
            }
            return GameSeries.SOSUS;
        }

        private void RefreshLViewList(int indexselected = 0)
        {
            StringBuilder build = new StringBuilder();
            list_pp.Items.Clear();
            //1132
            for (int i = 0; i < 1132; i++)
            {
                Format.gformat.ppItemData mtemp = new Format.gformat.ppItemData(papaFile.getSelectedData(i), getSeries(comboBox1.SelectedIndex));

                build.Append(mtemp.InfoVariableConfig).Append(",");
                build.Append(mtemp.TextVar).Append(",");
                build.Append(mtemp.ModelVar).Append(",");
                build.Append(mtemp.unkw[0]);

                list_pp.Items.Add(mtemp.InfoVariableConfig.Replace("INFO_",""));
                build.Append(Environment.NewLine);
                Console.WriteLine(i);
            }
            Clipboard.SetText(build.ToString());
            list_pp.SelectedIndex = indexselected;
        }

        private void list_pp_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkReady = false;

            ppdatay = new Format.gformat.ppItemData(papaFile.getSelectedData(list_pp.SelectedIndex), getSeries(comboBox1.SelectedIndex));

            textBox1.Text = ppdatay.TextVar;
            textBox2.Text = ppdatay.ModelVar;
            textBox3.Text = ppdatay.InfoVariableConfig;
            textBox4.Text = ppdatay.unkw[0] + "";
            textBox5.Text = ppdatay.unkw[1] + "";
            textBox6.Text = ppdatay.unkw[2] + "";
            textBox7.Text = ppdatay.unkw[3] + "";
            textBox8.Text = ppdatay.unkw[4] + "";
            textBox9.Text = ppdatay.unkw[5] + "";
            textBox10.Text = ppdatay.unkw[6] + "";
            textBox11.Text = ppdatay.unkw[7] + "";
            textBox12.Text = ppdatay.unkw[8] + "";
            textBox13.Text = ppdatay.unkw[9] + "";

            checkReady = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save PP Files ...";
            diag.Filter = "files (*.*) | *.*";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                papaFile.RecreateContent();

                File.WriteAllBytes(diag.FileName, papaFile.Data);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();

            if (diag.ShowDialog() != DialogResult.OK) return;

            string[] arraytext = File.ReadAllText(diag.FileName)
                .Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            for (int i = 0; i < arraytext.Length; i++)
            {
                Format.gformat.ppItemData ppdat = new Format.gformat.ppItemData(papaFile.getSelectedData(i), getSeries(comboBox1.SelectedIndex));

                byte[] inputd = ppdat.GetData(getSeries(comboBox1.SelectedIndex), arraytext[i], ppdat.ModelVar,
                    ppdat.InfoVariableConfig, ppdat.unkw[0], ppdat.unkw[1], ppdat.unkw[2], ppdat.unkw[3],
                    ppdat.unkw[4], ppdat.unkw[5], ppdat.unkw[6], ppdat.unkw[7], ppdat.unkw[8], ppdat.unkw[9]);

                papaFile.setSelectedData(i, inputd);
                papaFile.RecreateContent();
            }

            RefreshLViewList();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
