using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinAppBar
{
    public partial class Main : Form
    {
        //private Container components = null;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.BackColor = GetTaskbarColor();
            RegisterBar();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegisterBar();
        }

    }
}
