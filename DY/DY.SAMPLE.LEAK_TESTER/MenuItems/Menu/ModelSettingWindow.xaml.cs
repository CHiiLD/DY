using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;

namespace DY.SAMPLE.LEAK_TESTER
{
    /// <summary>
    /// ModelSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModelSettingWindow : Window
    {
        public const string MODEL_ITEM_1 = "1";
        public const string MODEL_ITEM_2 = "2";
        public const string MODEL_ITEM_3 = "3";

        private List<Model> _ModelItem = new List<Model>();
        private List<SerialNumber> _SerialItem = new List<SerialNumber>();

        public ModelSettingWindow()
        {
            InitializeComponent();

            NModelNum.Items.Add(MODEL_ITEM_1);
            NModelNum.Items.Add(MODEL_ITEM_2);
            NModelNum.Items.Add(MODEL_ITEM_3);

            foreach (var i in ModelDirector.GetInstance().Item)
                _ModelItem.Add(new Model(i));
            foreach (var i in SerialNumberDirector.GetInstance().Item)
                _SerialItem.Add(new SerialNumber(i));
        }

        private void WriteTextBox(Model model, SerialNumber serial)
        {
            NModel.Text = model.ModelName;
            NCustomer.Text = model.Customer;
            NProdectInfo.Text = model.ProductInfo;
            NProductNum.Text = model.PartNumber;
            NLabelID.Text = model.LabelID;
            NLHSerialNum.Text = serial.SerialNumberStart_L.ToString();
            NRHSerialNum.Text = serial.SerialNumberStart_R.ToString();
        }

        private void WriteModel(Model model, SerialNumber serial)
        {
            model.ModelName = NModel.Text;
            model.Customer = NCustomer.Text;
            model.ProductInfo = NProdectInfo.Text;
            model.PartNumber = NProductNum.Text;
            model.LabelID = NLabelID.Text;

            int temp;
            if (Int32.TryParse(NLHSerialNum.Text, out temp))
                serial.SerialNumberStart_L = temp;
            if (Int32.TryParse(NRHSerialNum.Text, out temp))
                serial.SerialNumberStart_R = temp;
        }

        private void WriteModel(string combobox_item)
        {
            switch (combobox_item)
            {
                case MODEL_ITEM_1:
                    WriteModel(_ModelItem[0], _SerialItem[0]);
                    break;
                case MODEL_ITEM_2:
                    WriteModel(_ModelItem[1], _SerialItem[1]);
                    break;
                case MODEL_ITEM_3:
                    WriteModel(_ModelItem[2], _SerialItem[2]);
                    break;
            }
        }

        private void NModelNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count != 0)
                WriteModel(e.RemovedItems[0] as string);

            string selectedItem = e.AddedItems[0] as string;
            switch (selectedItem)
            {
                case MODEL_ITEM_1:
                    WriteTextBox(_ModelItem[0], _SerialItem[0]);
                    break;
                case MODEL_ITEM_2:
                    WriteTextBox(_ModelItem[1], _SerialItem[1]);
                    break;
                case MODEL_ITEM_3:
                    WriteTextBox(_ModelItem[2], _SerialItem[2]);
                    break;
                default:
                    return;
            }
        }

        private void NOK_Click(object sender, RoutedEventArgs e)
        {
            WriteModel(NModelNum.SelectedValue as string);

            ModelDirector.GetInstance().Item.Clear();
            ModelDirector.GetInstance().Item.AddRange(_ModelItem);
            ModelDirector.GetInstance().SaveToFile();

            SerialNumberDirector.GetInstance().Item.Clear();
            SerialNumberDirector.GetInstance().Item.AddRange(_SerialItem);
            SerialNumberDirector.GetInstance().SaveToFile();
            this.Close();
        }

        private void NCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
