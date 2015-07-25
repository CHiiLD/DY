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

using DY.WPF.SYSTEM.COMM;
using System.Collections.Generic;

namespace DY.WPF
{
    /// <summary>
    /// CommDeviceAddition.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommDeviceSelection : UserControl
    {
        public Dictionary<string, object> ExtraData
        {
            get
            {
                var ret = new Dictionary<string, object>();
                var children = NExtra.Children;
                foreach(var child in children)
                {
                    IGetValue v = child as IGetValue;
                    if(v != null)
                        ret.Add(v.UserData as string, v.GetValue());
                }
                return ret;
            }
        }

        public CommDeviceSelection()
        {
            this.InitializeComponent();
            NDevice.ItemSource = Enum.GetValues(typeof(DYDevice));
            NDevice.NComboBox.SelectionChanged += OnSelectionChanged_Device;
            NType.NComboBox.SelectionChanged += OnSelectionChanged_Type;
        }

        /// <summary>
        /// 통신 디바이스를 설정하였을 때, 통신 타입을 연다
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged_Device(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            DYDevice item = (DYDevice)cb.SelectedItem;
            List<DYDeviceCommType> source = new List<DYDeviceCommType>();
            NType.SelectedItem = null;
            NGrid.Children.Clear();
            NExtra.Children.Clear();
            if (ServiceableDevice.Service.ContainsKey(item))
            {
                DYDeviceCommType type = ServiceableDevice.Service[item];
                DYDeviceCommType[] types = (DYDeviceCommType[])Enum.GetValues(typeof(DYDeviceCommType));
                foreach (var t in types)
                    if ((t & type) != 0)
                        source.Add(t);
                NType.ItemSource = source;
            }
        }

        /// <summary>
        /// 통신 타입을 설정하였을 때, 통신 옵션을 연다
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged_Type(object sender, SelectionChangedEventArgs e)
        {
            NGrid.Children.Clear();
            NExtra.Children.Clear();
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            DYDeviceCommType item = (DYDeviceCommType)cb.SelectedItem;
            switch (item)
            {
                case DYDeviceCommType.ETHERNET:
                    NGrid.Children.Add(new CommEthernetConfig());
                    break;
                case DYDeviceCommType.SERIAL:
                    NGrid.Children.Add(new CommSerialConfig());
                    TextBoxWithBar localbox = new TextBoxWithBar();
                    localbox.UserData = CommClient.EXTRA_XGT_CNET_LOCALPORT;
                    localbox.Title = "Local Port";
                    localbox.Text = "00";
                    NExtra.Children.Add(localbox);
                    break;
            }
        }
    }
}