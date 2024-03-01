using Model.DataFormats;
using Model.MathHelper;

namespace Model.Controllers;
public abstract class PeakBordersController {
    private List<PeakBorder> borders = [];
    public Point<float> Coordinates { get; private set; } = new(0f, 0f);
    public IEnumerable<PeakBorder> Borders => borders;

    public abstract Task<bool> AddBorder();

    protected abstract Task<float> GetXCoordinateByClick();

    protected abstract Task<float> GetXCoordinateByKeyDown();

    protected abstract void DrawOnPlot(PeakBorder border);

    protected abstract void WipeFromPlot(PeakBorder border);
    public async Task<bool> DeleteLast()
       => await Task.Run(RemoveLastBorder);

    public async Task<bool> Clear()
       => await Task.Run(ClearBorders);

    public void SetCoordinates(float x, float y) {
        Coordinates = new(x, y);
    }

    public bool RedrawBorders() {
        lock (borders) {
            if (borders.Count == 0) return false;
            foreach (var b in borders) {
                WipeFromPlot(b);
                DrawOnPlot(b);
            }
            return true;
        }
    }

    protected bool AddBorder(PeakBorder border) {
        lock (borders) {
            if (borders.Contains(border)) return false;
            DrawOnPlot(border);
            borders.Add(border);
            return true;
        }
    }

    protected bool RemoveBorder(PeakBorder border) {
        lock (borders) {
            if (borders.Remove(border)) {
                WipeFromPlot(border);
                return true;
            }
            return false;
        }
    }

    protected bool RemoveLastBorder() {
        lock (borders) {
            if (borders.Count == 0) return false;
            return RemoveBorder(borders.Last());
        }
    }

    protected bool ClearBorders() {
        lock (borders) {
            if (borders.Count == 0) return false;
            foreach (var b in borders)
                WipeFromPlot(b);
            borders.Clear();
            return true;
        }
    }
}
