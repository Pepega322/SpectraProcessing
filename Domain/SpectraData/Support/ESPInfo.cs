namespace Domain.SpectraData.Support;

public record ESPInfo
{
	public string ExpCfg { get; init; }
	public string ProcCfg { get; init; }

	public ESPInfo(string[] contents)
	{
		ExpCfg = contents[0];
		ProcCfg = contents[1];
	}
}