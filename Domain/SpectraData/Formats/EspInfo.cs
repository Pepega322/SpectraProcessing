namespace Domain.SpectraData.Formats;

public record EspInfo
{
	public string ExpCfg { get; init; }
	public string ProcCfg { get; init; }

	public EspInfo(string[] contents)
	{
		ExpCfg = contents[0];
		ProcCfg = contents[1];
	}
}