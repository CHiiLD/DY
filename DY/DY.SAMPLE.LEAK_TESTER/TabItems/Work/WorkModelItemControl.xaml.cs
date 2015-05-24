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

namespace DY.SAMPLE.LEAK_TESTER
{
    public partial class WorkModelItemControl : UserControl
    {
        private SolidColorBrush OK_COLOR = new SolidColorBrush(Color.FromArgb(0xFF, 0x37, 0xBD, 0x5B));
        private SolidColorBrush NG_COLOR = new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0x4A, 0x27));

        public WorkModelItemControl()
        {
            this.InitializeComponent();

            NModelNum.Items.Add(ModelSettingWindow.MODEL_ITEM_1);
            NModelNum.Items.Add(ModelSettingWindow.MODEL_ITEM_2);
            NModelNum.Items.Add(ModelSettingWindow.MODEL_ITEM_3);
        }

        public void SetModelItemInfo(Model m)
        {
            if (m != null)
            {
                NModel.Content = m.ModelName;
                NCustomer.Content = m.Customer;
                NProdectInfo.Content = m.ProductInfo;
                NProductNum.Content = m.PartNumber;
                NLabelID.Content = m.LabelID;
            }
            if (m is ModelItem)
            {
                var i = m as ModelItem;
                NProductValue.Content = i.Leak;
                NSerialNum.Content = i.SerialNumber;
                NQrCode.Content = i.QRCode;
                var ret = NProductRet;
                switch(i.Result)
                {
                    case ModelItem.RESULT.OK: //#FF37BD5B
                        ret.Background = OK_COLOR;
                        break;
                    case ModelItem.RESULT.NG: //#FFE84A27
                        ret.Background = NG_COLOR;
                        break;
                }
            }
        }

        private void NModelNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = e.AddedItems[0] as string;
            Model model = null;
            switch (selectedItem)
            {
                case ModelSettingWindow.MODEL_ITEM_1:
                    model = ModelDirector.GetInstance().Item[0];
                    break;
                case ModelSettingWindow.MODEL_ITEM_2:
                    model = ModelDirector.GetInstance().Item[1];
                    break;
                case ModelSettingWindow.MODEL_ITEM_3:
                    model = ModelDirector.GetInstance().Item[2];
                    break;
            }
            SetModelItemInfo(model);
        }
    }
}