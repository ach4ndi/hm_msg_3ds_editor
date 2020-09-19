using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using unvell.ReoGrid.IO;

namespace BokujoMessage4.uForm
{
    public partial class uSheet : Form
    {
        public uSheet(string path)
        {
            InitializeComponent();

            this.reoGridControl1.AutoSize = true;
            this.reoGridControl1.Load(path, FileFormat.Excel2007);
            reoGridControl1.SheetTabVisible = false;
            this.Text = Path.GetFileName(path) ;
        }
    }
}
