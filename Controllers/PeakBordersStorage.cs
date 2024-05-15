using Domain.SpectraData;

namespace Controllers;

public class PeakBordersStorage
{
	private readonly List<PeakBorder> borders = [];
	public int Count => borders.Count;
	public IEnumerable<PeakBorder> Borders => borders;

	public bool AddThreadSafe(PeakBorder border)
	{
		lock (borders)
		{
			if (borders.Contains(border)) return false;
			borders.Add(border);
		}

		return true;
	}

	public bool RemoveThreadSafe(PeakBorder border)
	{
		lock (borders)
			return borders.Remove(border);
	}

	public void ClearThreadSafe()
	{
		lock (borders)
			borders.Clear();
	}
}