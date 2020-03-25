using SharpUpdate;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private SharpUpdater updater;
        public Form1()
        {
            InitializeComponent();
            label1.Text = ProductName + "\n" + ProductVersion;
            updater = new SharpUpdater(Assembly.GetExecutingAssembly(), this, new Uri("https://github.com/m3rk1989/CoD-World-At-War-R2R-Generator/tree/master"));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            updater.DoUpdate();
        }
    }
}
