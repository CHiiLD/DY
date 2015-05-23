using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DY.SAMPLE.LEAK_TESTER
{
    /// <summary>
    /// ModelSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModelSettingWindow : Window
    {
        private const string MODEL_ITEM_1 = "1";
        private const string MODEL_ITEM_2 = "2";
        private const string MODEL_ITEM_3 = "3";

        private List<Model> _ModelItem = new List<Model>();
        private bool _Changed = false;

        public ModelSettingWindow()
        {
            InitializeComponent();

            NModelNum.Items.Add(MODEL_ITEM_1);
            NModelNum.Items.Add(MODEL_ITEM_2);
            NModelNum.Items.Add(MODEL_ITEM_3);

            for (int i = 0; i < ModelDirector.GetInstance().Item.Count; i++)
                _ModelItem.Add(new Model(ModelDirector.GetInstance().Item[i]));
        }

        private void WriteTextBox(Model model)
        {
            NModel.Text = model.ModelName;
            NCustomer.Text = model.Customer;
            NProdectInfo.Text = model.ProductInfo;
            NProductNum.Text = model.PartNumber;
            NLabelID.Text = model.LabelID;
            NLHSerialNum.Text = model.LHSerialStartNo;
            NRHSerialNum.Text = model.RHSerialStartNo;
        }

        private void WriteModel(Model model)
        {
            model.ModelName = NModel.Text;
            model.Customer = NCustomer.Text;
            model.ProductInfo = NProdectInfo.Text;
            model.PartNumber = NProductNum.Text;
            model.LabelID = NLabelID.Text;
            model.LHSerialStartNo = NLHSerialNum.Text;
            model.RHSerialStartNo = NRHSerialNum.Text;
        }

        private void NModelNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_Changed)
                _Changed = true;

            if (e.RemovedItems.Count != 0)
            {
                string old = e.RemovedItems[0] as string;
                switch (old)
                {
                    case MODEL_ITEM_1:
                        WriteModel(_ModelItem[0]);
                        break;
                    case MODEL_ITEM_2:
                        WriteModel(_ModelItem[1]);
                        break;
                    case MODEL_ITEM_3:
                        WriteModel(_ModelItem[2]);
                        break;
                }
            }

            string selectedItem = e.AddedItems[0] as string;
            switch (selectedItem)
            {
                case MODEL_ITEM_1:
                    WriteTextBox(_ModelItem[0]);
                    break;
                case MODEL_ITEM_2:
                    WriteTextBox(_ModelItem[1]);
                    break;
                case MODEL_ITEM_3:
                    WriteTextBox(_ModelItem[2]);
                    break;
                default:
                    return;
            }
        }

        private void NOK_Click(object sender, RoutedEventArgs e)
        {
            if (_Changed)
            {
                ModelDirector.GetInstance().Item.Clear();
                ModelDirector.GetInstance().Item.AddRange(_ModelItem);
                ModelDirector.GetInstance().SaveToFile();
            }
            this.Close();
        }

        private void NCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
