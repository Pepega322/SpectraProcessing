using Model.DataFormats.Base;
using Model.DataFormats.SpectraFormats;
using Model.DataFormats.Spectras.Base;
using ScottPlot.Plottable;

namespace View.Controllers;
public class ScottPlotContainer
{
    private readonly ScottPlot.FormsPlot _plotView;
    private readonly Dictionary<IReadOnlyList<double>, double[]> _xSPlots;
    private readonly SortedDictionary<Data, IPlottable> _plots;

    private Color _currentColor;
    private IPlottable? _currentPlot;

    public ScottPlotContainer(ScottPlot.FormsPlot plot)
    {
        _plotView = plot;
        _xSPlots = [];
        _plots = [];
    }

    public void ClearPlot()
    {
        _plots.Clear();
        _plotView.Plot.Clear();
        _plotView.Refresh();
    }

    public void UpdatePlot(ISet<Data> dataToBePlotted)
    {
        var toAdd = dataToBePlotted
            .Where(d => !_plots.ContainsKey(d))
            .ToArray();
        var toRemove = _plots.Keys
            .Where(d => !dataToBePlotted.Contains(d))
            .ToArray();
        _plotView.Plot.RenderLock();
        Parallel.ForEach(toAdd, AddToPlot);
        Parallel.ForEach(toRemove, RemoveFromPlot);
        _plotView.Plot.AxisAuto();
        _plotView.Plot.RenderUnlock();
        _plotView.Refresh();
    }

    public void ChangeVisibility(Data data, bool isVisible)
    {
        if (_plots.ContainsKey(data))
        {
            _plots[data].IsVisible = isVisible;
            _plotView.Refresh();
        }
    }

    //public void HighlightPlot(Data data)
    //{
    //    if (_plots.ContainsKey(data))
    //        Task.Run(() => ChangeCurrent(data));
    //    _plotView.Refresh();
    //}

    //private void ChangeCurrent(Data data)
    //{
    //    if (_currentPlot != null)
    //    {
    //        var t = _currentPlot.GetType();
    //        t.GetProperty("Color").SetValue(_currentPlot, _currentColor);
    //        t.GetProperty("IsHighlighted").SetValue(_currentPlot, false);
    //    }

    //    _currentPlot = _plots[data];
    //    var type = _currentPlot.GetType();
    //    var color = type.GetProperty("Color").GetValue(_currentPlot);
    //    _currentColor = (Color)color;
    //    type.GetProperty("Color").SetValue(_currentPlot, Color.Black);
    //    type.GetProperty("IsHighlighted").SetValue(_currentPlot, true);
    //}

    private void AddToPlot(Data data)
    {
        if (_plots.ContainsKey(data)) return;
        if (data is Spectra spectra)
            AddSpectraToPlot(spectra);
    }

    private void AddSpectraToPlot(Spectra spectra)
    {
        IPlottable plot;
        var (xS, yS) = spectra.GetPoints();
        lock (_xSPlots)
        {
            if (!_xSPlots.ContainsKey(xS))
                _xSPlots.Add(xS, xS.ToArray());
        }

        lock (_plotView)
        {
            if (spectra is ASP asp)
                plot = _plotView.Plot.AddSignal(yS, 1 / asp.Delta);
            else
                plot = _plotView.Plot.AddSignalXY(_xSPlots[xS], yS);
        }
        lock (_plots) _plots.Add(spectra, plot);
    }

    private void RemoveFromPlot(Data data)
    {
        lock (_plots)
        {
            if (_plots.ContainsKey(data))
            {
                lock (_plotView) _plotView.Plot.Remove(_plots[data]);
                _plots.Remove(data);
            }
        }
    }

    public IEnumerable<TreeNode> GetPlotNodes()
    {
        foreach (var pair in _plots)
        {
            yield return new TreeNode
            {
                Text = pair.Key.Name,
                Tag = pair.Key,
                Checked = pair.Value.IsVisible,
            };
        }
    }
}
