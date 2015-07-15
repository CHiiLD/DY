using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace DY.WPF
{
    public class CommDataGridItem
    {
        public Path Image { get; set; }
        public CommDevice Target { get; set; }
        public CommType Type { get; set; }
        public string Option { get; set; }
        public string Note { get; set; }
        public ToggleButton OnOffButton { get; set; }
    }
}