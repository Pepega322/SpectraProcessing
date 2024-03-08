﻿using Domain.SpectraData.Support;

namespace Domain.SpectraData;
public abstract class ESP : Spectra {
    protected const int FirstPointIndex = 2;

    public ESPInfo Info;

    protected ESP(string name, SpectraPoints points, ESPInfo info)
        : base(name, points) {
        Info = info;
        Format = SpectraFormat.ESP;
    }
}