namespace DY.SAMPLE.LEAK_TESTER
{
    public class Model
    {
        public string ModelName { get; set; }
        public string Customer { get; set; }
        public string ProductInfo { get; set; }
        public string PartNumber { get; set; }
        public string LabelID { get; set; }
        public string LHSerialStartNo { get; set; }
        public string RHSerialStartNo { get; set; }

        public Model(Model model)
        {
            ModelName = model.ModelName;
            Customer = model.Customer;
            ProductInfo = model.ProductInfo;
            PartNumber = model.PartNumber;
            LabelID = model.LabelID;
            LHSerialStartNo = model.LHSerialStartNo;
            RHSerialStartNo = model.RHSerialStartNo;
        }

        public Model()
        {
        }
    }
}