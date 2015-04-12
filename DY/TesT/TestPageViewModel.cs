using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace TesT
{
    // bind this view model to your page or window (DataContext)
    // class which represent a data point in the chart
    public class TestClass
    {
        public string Category { get; set; }

        public int Number { get; set; }
    }

    public class TestPageViewModel
    {
        public ObservableCollection<TestClass> Errors { get; private set; }

        public TestPageViewModel()
        {
            Errors = new ObservableCollection<TestClass>();
            Errors.Add(new TestClass() { Category = "Globalization", Number = 75 });
            Errors.Add(new TestClass() { Category = "Features", Number = 2 });
            Errors.Add(new TestClass() { Category = "ContentTypes", Number = 12 });
            Errors.Add(new TestClass() { Category = "Correctness", Number = 83 });
            Errors.Add(new TestClass() { Category = "Best Practices", Number = 29 });
        }

        private object selectedItem = null;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                // selected item has changed
                selectedItem = value;
            }
        }
    }
}
