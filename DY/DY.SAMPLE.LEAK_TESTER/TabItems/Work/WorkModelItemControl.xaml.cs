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
        private SolidColorBrush PNG_COLOR = new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0x4A, 0x27));

        public Model SelectedModel { get; private set; }
        public SerialNumber SelectedSerialNumber { get; private set; }

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
                if ((string)NModel.Content != m.ModelName)
                    NModel.Content = m.ModelName;
                if ((string)NCustomer.Content != m.Customer)
                    NCustomer.Content = m.Customer;
                if ((string)NProdectInfo.Content != m.ProductInfo)
                    NProdectInfo.Content = m.ProductInfo;
                if ((string)NProductNum.Content != m.PartNumber)
                    NProductNum.Content = m.PartNumber;
                if ((string)NLabelID.Content != m.LabelID)
                    NLabelID.Content = m.LabelID;
            }
            if (m is ModelItem)
            {
                var i = m as ModelItem;
                NProductValue.Content = i.Leak;
                NSerialNum.Content = i.SerialNumber;
                NQrCode.Content = i.QRCode;
                var ret = NProductRet;
                switch (i.Result)
                {
                    case ModelItem.RESULT.OK: //#FF37BD5B
                        if (ret.Background != OK_COLOR)
                        {
                            ret.Background = OK_COLOR;
                            ret.Content = ModelItem.RESULT.OK;
                        }
                        break;
                    case ModelItem.RESULT.NG: //#FFE84A27
                        if (ret.Background != NG_COLOR)
                        {
                            ret.Background = NG_COLOR;
                            ret.Content = ModelItem.RESULT.NG;
                        }
                        break;
                    case ModelItem.RESULT.PNG: //#FFE84A27
                        if (ret.Background != PNG_COLOR)
                        {
                            ret.Background = PNG_COLOR;
                            ret.Content = ModelItem.RESULT.PNG;
                        }
                        break;
                }
            }
        }

        private void NModelNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = e.AddedItems[0] as string;
            Model model = null;
            SerialNumber serialN = null;
            switch (selectedItem)
            {
                case ModelSettingWindow.MODEL_ITEM_1:
                    model = ModelDirector.GetInstance().Item[0];
                    serialN = SerialNumberDirector.GetInstance().Item[0];
                    break;
                case ModelSettingWindow.MODEL_ITEM_2:
                    model = ModelDirector.GetInstance().Item[1];
                    serialN = SerialNumberDirector.GetInstance().Item[1];
                    break;
                case ModelSettingWindow.MODEL_ITEM_3:
                    model = ModelDirector.GetInstance().Item[2];
                    serialN = SerialNumberDirector.GetInstance().Item[2];
                    break;
            }
            SetModelItemInfo(model);
            SelectedModel = model;
            SelectedSerialNumber = serialN;
        }
    }
}