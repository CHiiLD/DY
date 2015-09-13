using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    public interface ICOxyRectangleRange
    {
        double RectMaximunX { get; set; }
        double RectMaximunY { get; set; }
        double RectMinimunX { get; set; }
        double RectMinimunY { get; set; }
    }
}
