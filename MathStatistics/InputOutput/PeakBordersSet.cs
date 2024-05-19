using Domain.InputOutput;
using MathStatistics.SpectraProcessing;

namespace MathStatistics.InputOutput;

public record PeakBordersSet(ICollection<PeakBorders> Borders) : IWriteableData
{
	public string? Name { get; set; } = string.Empty;
	public string? Extension => "borders";

	public IEnumerable<string> ToContents()
	{
		yield return "XStart;XEnd";
		foreach (var border in Borders)
			yield return $"{border.XStart};{border.XEnd}";
	}
}