namespace Domain.SpectraData;

public abstract class SpectraPlot : Data
{
	public bool IsVisible { get; protected set; }

	public abstract void ChangeVisibility(bool isVisible);
}