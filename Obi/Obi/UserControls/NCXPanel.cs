using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class NCXPanel : UserControl
    {
        private NCX.NCX mNCX;

        public NCX.NCX NCX
        {
            set
            {
                mNCX = value;
            }
        }

        public NCXPanel()
        {
            InitializeComponent();
            mNCX = null;
        }
    }
}
