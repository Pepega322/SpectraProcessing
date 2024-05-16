﻿namespace Domain.SpectraData.Formats;

public sealed class EspSpectra : Spectra
{
	public readonly EspInfo Info;

	internal EspSpectra(string name, SpectraPoints points, EspInfo info)
		: base(name, points)
	{
		Info = info;
		Format = SpectraFormat.Esp;
	}

	public override Spectra Copy() => new EspSpectra(Name, Points, Info);
}