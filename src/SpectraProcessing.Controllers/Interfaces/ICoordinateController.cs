using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface ICoordinateController
{
    event Action? OnChange;

    Point<float> Coordinates { get; set; }

    Task<Point<float>> GetCoordinateByClick();

    Task<Point<float>> GetCoordinateByKeyDown();
}
