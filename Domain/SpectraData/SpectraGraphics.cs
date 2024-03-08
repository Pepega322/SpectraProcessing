namespace Domain.SpectraData;
public abstract class SpectraGraphics {
    protected HashSet<SpectraPlot> plots = [];
    public int PlotCount => plots.Count;

    public abstract void DrawThreadSafe(SpectraPlot plot);
    public abstract void EraseThreadSafe(SpectraPlot plot);
    public abstract void ChangeHighlightion(SpectraPlot plot, bool isHighlighted);
    public abstract void ResizeThreadSafe();
    public abstract void ClearThreadSafe();
}