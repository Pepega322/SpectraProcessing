using Controllers;
using Domain.MathHelp;

namespace View.Controllers;

public class WinformsCoordinateController(UserControl form) : CoordrinateController {
    UserControl form = form;

    public override async Task<Point<float>> GetCoordinateByClick() {
        var task = new Task<Point<float>>(() => Coordinates);
        MouseEventHandler handler = (sender, e) => {
            task.Start();
        };
        form.MouseDown += handler;
        var result = await task;
        form.MouseDown -= handler;
        return result;
    }

    public override async Task<Point<float>> GetCoordinateByKeyDown() {
        var task = new Task<Point<float>>(() => Coordinates);
        KeyEventHandler handler = (sender, e) => {
            if (e.KeyData == Keys.Z) task.Start();
        };
        form.KeyDown += handler;
        var result = await task;
        form.KeyDown -= handler;
        return result;
    }
}
