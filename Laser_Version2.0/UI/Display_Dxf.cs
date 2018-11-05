using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MxDrawXLib;

namespace Laser_Build_1._0
{
    public partial class Display_Dxf : Form
    {
        public Display_Dxf()
        {
            InitializeComponent();
        }

        private void Display_Dxf_Load(object sender, EventArgs e)
        {
            this.axMxDrawX1.ShowRulerWindow = true;
        }
    }
}
