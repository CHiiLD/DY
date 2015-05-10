using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DY.SAMPLE.PLC
{
    /// <summary>
    /// Interaction logic for ComSettingWindow.xaml
    /// </summary>
    public partial class SerialSettingWindow : Window
    {
        private COM _com;

        public SerialSettingWindow()
        {
            InitializeComponent();
        }

        public void AddStackPanelItem(string title, EventHandler handler)
        {

        }

        public void SetCOM(COM com)
        {
            _com = com;

            switch (_com)
            {
                case COM.SERIAL:
                    {

                    }
                    break;
                default:
                    break;
            }
        }
    }
}
