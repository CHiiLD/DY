namespace DY.WPF
{
    /// <summary>
    /// COxyGraph XY축의 최대, 최소값을 설정한다.
    /// </summary>
    public interface ICOxyPlotRange
    {
        double PlotMaximumX { get; set; }
        double PlotMaximumY { get; set; }

        double PlotMinimumX { get; set; }
        double PlotMinimumY { get; set; }
    }
}
