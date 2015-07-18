using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using DY.WPF.WINDOW;
using DY.WPF.SYSTEM.COMM;
using System.Collections.Concurrent;
using System.Collections;

namespace DY.WPF
{
	/// <summary>
	/// CommDataGrid.xaml에 대한 상호 작용 논리
	/// </summary>

	public partial class CommDataGrid : UserControl
	{
        public IEnumerable ClientCommItems
        {
            get
            {
                return ClientCommManagement.GetInstance().Clientele;
            }
        }

		public CommDataGrid()
		{
			this.InitializeComponent();
		}

        private void NMI_AddCommDevice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new CommDeviceSetWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }
	}
}
