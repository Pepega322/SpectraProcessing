using Domain.MathHelp;

namespace Controllers;
public abstract class CoordrinateController {
    public Point<float> Coordinates { get; private set; } = new(0f, 0f);

    public abstract Task<Point<float>> GetCoordinateByClick();

    public abstract Task<Point<float>> GetCoordinateByKeyDown();

    public void SetCoordinates(float x, float y) {
        Coordinates = new(x, y);
    }
}
