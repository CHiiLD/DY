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

namespace DY.WPF
{
    /// <summary>
    /// CommDeviceAddition.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommDeviceSelection : UserControl
    {
        public EventHandler<SelectionChangedEventArgs> CommTypeComboBoxChanged { get; set; }

        public Dictionary<string, object> ExtraData
        {
            get
            {
                var ret = new Dictionary<string, object>();
                var children = NExtra.Children;
                foreach (var child in children)
                {
                    IGetContext v = child as IGetContext;
                    if (v != null)
                        ret.Add(v.UserData as string, v.GetContext());
                }
                return ret;
            }
        }

        public CommDeviceSelection()
        {
            this.InitializeComponent();
            NDevice.ItemSource = Enum.GetValues(typeof(DyNetDevice));
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
            DyNetDevice item = (DyNetDevice)cb.SelectedItem;
            List<DyNetCommType> source = new List<DyNetCommType>();
            NType.SelectedItem = null;
            NGrid.Children.Clear();
            NExtra.Children.Clear();
            if (ServiceableDevice.Service.ContainsKey(item))
            {
                DyNetCommType type = ServiceableDevice.Service[item];
                DyNetCommType[] types = (DyNetCommType[])Enum.GetValues(typeof(DyNetCommType));
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
            DyNetCommType item = (DyNetCommType)cb.SelectedItem;
            switch (item)
            {
                case DyNetCommType.ETHERNET:
                    NGrid.Children.Add(new CommEthernetConfig());
                    break;
                case DyNetCommType.SERIAL:
                    NGrid.Children.Add(new CommSerialConfig());
                    break;
            }
            if (CommTypeComboBoxChanged != null)
                CommTypeComboBoxChanged(sender, e);
        }
    }
}