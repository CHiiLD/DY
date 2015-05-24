using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.IO;
using System.Collections;

namespace DY.SAMPLE.LEAK_TESTER
{
    /// <summary>
    /// BrowseModelItemControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BrowseModelItemControl : UserControl
    {
        public BrowseModelItemControl()
        {
            this.InitializeComponent();
        }

        private void NSearch_Click(object sender, RoutedEventArgs e)
        {
            DateTime? datetime = NDatePicker.SelectedDate;
            if (datetime == null)
                return;
            List<ModelItem> list;
            if (ModelItemDirector.LoadByFile(out list, (DateTime)datetime))
            {
                var listview = NListControl.NView;
                listview.Items.Clear();
                foreach (var i in list)
                    listview.Items.Add(i);
            }
            else
            {
                MessageBox.Show(string.Format("{0} 날짜의 데이터를 찾을 수 없습니다.", datetime.Value.ToString("yy.MM.dd")), "알림", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        private void NSaveToCSV_Click(object sender, RoutedEventArgs e)
        {
            if(NListControl.NView.Items.Count == 0)
            {
                MessageBox.Show("저장하고자 할 데이터가 리스트 뷰에 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "DY.SAMPLE.LEAK_TESTER " + DateTime.Now.ToString("yy.MM.dd") + ".csv";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv documents (.csv)|*.csv";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                using (TextWriter textWriter = File.CreateText(dlg.FileName))
                {
                    var csvWriter = new CsvWriter(textWriter);
                    csvWriter.WriteRecords(NListControl.NView.Items);
                }
            }
        }
    }
}