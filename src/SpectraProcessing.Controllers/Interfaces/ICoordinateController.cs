using ScottPlot;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface ICoordinateController
{
    event Action? OnChange;

    Point<int> Location { get; set; }

    Pixel Pixel { get; }

    Point<float> Coordinates { get; }

    Task<Point<float>> GetCoordinateByClick();

    Task<Point<float>> GetCoordinateByKeyDown();
}
