using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace BokujoMessage4
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();

            InitializeComponent();
            this.label1.Text = name.Name+" "+ name.Version +" (" +name.ProcessorArchitecture+")\n\n"+
                "Created by Andi (Leafie)\n\n" +
                "Tools for Translation Msg.xbb on\n" +
                "- Story of Seasons US/EU/JP\n" +
                "- Story of Seasons 2 JP\n" +
                "- ANB JP/US/EU\n\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
