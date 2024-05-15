namespace Domain.SpectraData;

public abstract class SpectraPlot
{
	public bool IsVisible { get; protected set; }

	public abstract void ChangeVisibility(bool isVisible);
}