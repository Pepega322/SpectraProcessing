using Controllers.Interfaces;
using Domain.SpectraData.Processing;

namespace Application.Controllers;

public class WinformsCoordinateController(Control form) : ICoordinateController
{
	public Point<float> Coordinates { get; set; } = new(0f, 0f);

	public async Task<Point<float>> GetCoordinateByClick()
	{
		var task = new Task<Point<float>>(() => Coordinates);
		MouseEventHandler handler = (_, _) => task.Start();
		form.MouseDown += handler;
		var result = await task;
		form.MouseDown -= handler;
		return result;
	}

	public async Task<Point<float>> GetCoordinateByKeyDown()
	{
		var task = new Task<Point<float>>(() => Coordinates);
		KeyEventHandler handler = (_, e) =>
		{
			if (e.KeyData == Keys.Z) task.Start();
		};
		form.KeyDown += handler;
		var result = await task;
		form.KeyDown -= handler;
		return result;
	}
}