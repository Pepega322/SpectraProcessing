namespace Domain.Graphics;

public interface IPlotDrawer<in TPlot> where TPlot : Plot
{
	void Draw(TPlot plot);
	void Erase(TPlot plot);
	void SetHighlight(TPlot plot, bool isHighlighted);
	void SetVisibility(TPlot plot, bool isVisible);
	void Resize();
	void Clear();
}