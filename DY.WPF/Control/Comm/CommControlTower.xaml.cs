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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MahApps.Metro.Controls;
using DY.WPF.SYSTEM.COMM;
using NLog;
using System.Collections;

namespace DY.WPF
{
    /// <summary>
    /// Configuration.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommControlTower : UserControl
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public static double TABITEM_HEADER_FONTSIZE { get { return 18; } }
        public static double TABCONTROL_MARGIN { get { return 5; } }

        public CommControlTower()
        {
            var client_mnt = CommClientManagement.GetInstance().Clientele;
            client_mnt.CollectionChanged += OnCollectionChanged;
            InitializeComponent();
        }

        /// <summary>
        /// 탭 아이템 추가
        /// CommClient에서 코멘트가 없으면 요약(Summary)로 탭아이템 헤더가 결정된다
        /// </summary>
        /// <param name="newItems"></param>
        private void AddTabItem(IList newItems)
        {
            TabControl tab_control = NTabControl;
            ItemCollection item_src = tab_control.Items;

            var new_item = newItems;
            TabItem tab_item = new TabItem();
            ControlsHelper.SetHeaderFontSize(tab_item, TABITEM_HEADER_FONTSIZE);

            foreach (var i in new_item)
            {
                var client = i as CommClient;
                LOG.Trace("통신 컨트롤 타워 탭 아이템 추가: " + client.Key);
                switch (client.Target)
                {
                    case DYDevice.DATALOGIC_MATRIX200:
                    case DYDevice.HONEYWELL_VUQUEST3310G:
                        break;
                    case DYDevice.LSIS_XGT:
                        CommIOMonitoring commIO = new CommIOMonitoring() { Client = client };
                        commIO.Margin = new Thickness(TABCONTROL_MARGIN);
                        tab_item.Content = commIO;
                        tab_item.Header = String.IsNullOrEmpty(client.Comment) ? client.Summary : client.Comment;
                        item_src.Add(tab_item);
                        break;
                }
            }
        }

        /// <summary>
        /// 텝 아이템 제거
        /// </summary>
        /// <param name="oldItem"></param>
        private void RemoveTabItem(IList oldItem)
        {
            TabControl tab_control = NTabControl;
            ItemCollection item_src = tab_control.Items;

            var old_item = oldItem;
            foreach (var i in old_item)
            {
                var client = i as CommClient;
                foreach (var s in item_src)
                {
                    TabItem tab_item = s as TabItem;
                    ICommMonitoring comm_moni = tab_item.Content as ICommMonitoring;
                    if (comm_moni != null && client.Key == comm_moni.Client.Key)
                    {
                        LOG.Trace("통신 컨트롤 타워 탭 아이템 삭제: " + client.Key);
                        item_src.Remove(tab_item);
                        comm_moni.Client = null;
                        break;
                    }
                }
            }
        }

        /// <summary>   
        /// CommClientManagement Clientele 컬렉션 변경 이벤트
        /// 엘리먼트의 변동사항으로 TabControl의 TabItem변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TabControl tab_control = NTabControl;
            ItemCollection item_src = tab_control.Items;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddTabItem(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveTabItem(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    LOG.Trace("NotifyCollectionChangedAction.Replace");
                    break;
                case NotifyCollectionChangedAction.Move:
                    LOG.Trace("NotifyCollectionChangedAction.Move");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    LOG.Trace("NotifyCollectionChangedAction.Reset");
                    break;
            }
        }
    }
}
