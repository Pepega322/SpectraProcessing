using Domain.SpectraData.Processing;

namespace Controllers.Interfaces;

public interface ICoordinateController
{
	Point<float> Coordinates { get; set; }
	Task<Point<float>> GetCoordinateByClick();
	Task<Point<float>> GetCoordinateByKeyDown();
}