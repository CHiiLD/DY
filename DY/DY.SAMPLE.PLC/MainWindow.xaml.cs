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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DY.SAMPLE.LOGIC;
using DY.NET.LSIS.XGT;
using DY.NET;

namespace DY.SAMPLE.PLC
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private SwitchLoopLogic _SwitchLoopLogic;

        public MainWindow()
        {
            InitializeComponent();
            NPLC_CB.Items.Add(PLC.LSIS_XGT);
        }

        private void NPLC_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            PLC plc = (PLC)cb.SelectedItem;
            switch (plc)
            {
                case 
                PLC.LSIS_XGT: NComCB.Items.Add(COM.SERIAL);
                    break;
                default:
                    break;
            }
        }

        private void NComCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            COM com = (COM)cb.SelectedItem;
            switch (com)
            {
                case COM.SERIAL:
               
                    break;
                default:
                    break;
            }
        }

        public void ReadyProgram(DYSerialPort port)
        {
            
        }
    }
}
