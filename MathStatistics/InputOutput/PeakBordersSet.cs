using Domain.InputOutput;
using MathStatistics.SpectraProcessing;

namespace MathStatistics.InputOutput;

public record PeakBordersSet(ICollection<PeakBorders> Borders) : IWriteableData
{
	public string? Name { get; init; } = string.Empty;
	public string Extension => "borders";

	public IEnumerable<string> ToContents()
	{
		yield return "XStart;XEnd";
		foreach (var border in Borders
			         .OrderBy(b => b.XStart)
			         .ThenBy(b => b.XEnd))
			yield return $"{border.XStart};{border.XEnd}";
	}
}