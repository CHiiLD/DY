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

namespace DY.WPF.CTRL.COMM
{
    /// <summary>
    /// CommConfigDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommConfigDataGrid : UserControl
    {
        public CommConfigDataGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 새로운 통신 설정을 추가한다
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NCM_AddNewComm_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}