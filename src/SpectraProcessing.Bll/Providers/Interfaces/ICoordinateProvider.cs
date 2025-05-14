using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface ICoordinateProvider
{
    Point<float> Coordinates { get; }

    public float Width { get; }

    public float Heigth { get; }

    void UpdateCoordinates(int x, int y);
}
