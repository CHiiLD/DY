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
using DY.WPF.SYSTEM;

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
            var client_mnt = CommClientDirector.GetInstance().Clientele;
            client_mnt.CollectionChanged += OnCollectionChanged;
            InitializeComponent();
        }

        /// <summary>
        /// 탭 아이템 추가
        /// CommClient에서 코멘트가 없으면 요약(Summary)로 탭아이템 헤더가 결정된다
        /// </summary>
        /// <param name="newItems">새로 추가된 CommClient 객체</param>
        private void AddTabItem(IList newItems)
        {
            TabControl tab_control = NTabControl;
            ItemCollection tabItemCollection = tab_control.Items;

            IList new_item = newItems;
            TabItem tab_item = new TabItem();
            ControlsHelper.SetHeaderFontSize(tab_item, TABITEM_HEADER_FONTSIZE); //헤더 폰트 사이즈 설정
            ICommControlTowerTabItem towerTabItem;
            foreach (var i in new_item)
            {
                var commClient = i as CommClient;
                LOG.Trace("통신 컨트롤 타워 탭 아이템 추가: " + commClient.Key);
                switch (commClient.Target)
                {
#if false
                    case DYDevice.DATALOGIC_MATRIX200:
#endif
                    case DyNetDevice.HONEYWELL_VUQUEST3310G:
                        break;
                    case DyNetDevice.LSIS_XGT:
                        towerTabItem = new CommIOMonitoring(new CommIOMonitoringXGT(commClient))
                        {
                            Margin = new Thickness(TABCONTROL_MARGIN)
                        };
                        tab_item.Content = towerTabItem;
                        tab_item.SetBinding(HeaderedContentControl.HeaderProperty, new Binding("Comment") { Source = commClient });
                        if (String.IsNullOrEmpty(commClient.Comment))
                            commClient.Comment = commClient.Summary;
                        tabItemCollection.Add(tab_item);
                        ShotDownDirector.GetInstance().AddIDisposable(towerTabItem);
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
            ICommControlTowerTabItem towerTabItem;
            var old_item = oldItem;
            foreach (var i in old_item)
            {
                var client = i as CommClient;
                foreach (var s in item_src)
                {
                    TabItem tab_item = s as TabItem;
                    towerTabItem = tab_item.Content as ICommControlTowerTabItem;
                    if (towerTabItem != null && client.Key == towerTabItem.CClient.Key)
                    {
                        LOG.Trace("통신 컨트롤 타워 탭 아이템 삭제: " + client.Key);
                        item_src.Remove(tab_item);
                        towerTabItem.CClient = null;
                        ShotDownDirector.GetInstance().RemoveIDisposable(towerTabItem);
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

        /// <summary>
        /// Tab Item 선택 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList selected_items = e.AddedItems;
            IList unselected_items = e.RemovedItems;

            foreach (var i in selected_items)
            {
                var tab_item = i as TabItem;
                if (tab_item == null)
                    break;
                var tab_item_context = tab_item.Content as ICommControlTowerTabItem;
                if (tab_item_context == null)
                    continue;
                if (tab_item_context.Selected != null)
                    tab_item_context.Selected(tab_item, EventArgs.Empty);
            }

            foreach (var i in unselected_items)
            {
                var tab_item = i as TabItem;
                if (tab_item == null)
                    break;
                var tab_item_context = tab_item.Content as ICommControlTowerTabItem;
                if (tab_item_context == null)
                    continue;
                if (tab_item_context.Unselected != null)
                    tab_item_context.Unselected(tab_item, EventArgs.Empty);
            }
        }
    }
}
