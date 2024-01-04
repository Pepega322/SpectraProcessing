using Model.DataFormats.Base;
using Model.DataFormats.SpectraFormats;
using Model.DataFormats.Spectras.Base;
using ScottPlot;
using ScottPlot.Plottable;

namespace View.Controllers;
public class ScottPlotContainer
{
    private readonly FormsPlot _plotView;
    private readonly Dictionary<IReadOnlyList<double>, double[]> _xSPlots;
    private readonly SortedDictionary<Data, IPlottable> _plots;
    private readonly Plot _builder;
    private bool _isInLowQuality => _plots.Count > 10;

    private Color _onSelectColor;
    private IPlottable? _onSelect = null!;
    //private readonly Type _selectedType = typeof(IPlottable);

    public ScottPlotContainer(FormsPlot plotView)
    {
        _plotView = plotView;
        _plotView.Plot.Palette = Palette.Category20;
        _plotView.Plot.XLabel("Raman shift, cm-1");
        _plotView.Plot.YLabel("Intensity");
        _builder = new();
        _builder.RenderLock();
        _builder.Palette = Palette.Category20;
        _xSPlots = [];
        _plots = [];
    }

    public void ClearPlot()
    {
        _plots.Clear();
        _plotView.Plot.Clear();
        _builder.Clear();
        _plotView.Render(_isInLowQuality, true);
    }

    public async Task PlotData(ISet<Data> dataToBePlotted)
    {
        await Task.Run(() => ChangePlot(dataToBePlotted));
        foreach (var plt in _plots.Values)
            _plotView.Plot.Add(plt);
        _plotView.Render(_isInLowQuality, true);
        _plotView.Plot.AxisAuto();
    }

    public void ChangeVisibility(Data data, bool isVisible)
    {
        if (_plots.ContainsKey(data))
        {
            _plots[data].IsVisible = isVisible;
            _plotView.Render(_isInLowQuality, true);
        }
    }

    public async void SelectPlot(Data data)
    {
        await Task.Run(() => HighlighPlot(data));
        _plotView.Render(_isInLowQuality, true);
    }

    private void HighlighPlot(Data data)
    {
        if (!_plots.ContainsKey(data) || _plots[data] == _onSelect) return;
        if (_onSelect is not null)
        {
            ChangeColor(_onSelect, _onSelectColor, out Color t);
            ChangeHighlightion(_onSelect, false);
        }
        _onSelect = _plots[data];
        ChangeColor(_onSelect, Color.Black, out Color original);
        ChangeHighlightion(_onSelect, true);
        _onSelectColor = original;
        _plotView.Plot.Remove(_onSelect);
        _plotView.Plot.Add(_onSelect);
    }

    private void ChangeColor(IPlottable plot, Color color, out Color original)
    {
        if (plot is SignalPlot signal)
        {
            original = signal.Color;
            signal.Color = color;
        }
        else if (plot is SignalPlotXY xySignal)
        {
            original = xySignal.Color;
            xySignal.Color = color;
        }
        else throw new NotImplementedException();
    }

    private void ChangeHighlightion(IPlottable plot, bool isHighlight)
    {
        if (plot is SignalPlot signal)
            signal.IsHighlighted = isHighlight;
        else if (plot is SignalPlotXY xySignal)
            xySignal.IsHighlighted = isHighlight;
        else throw new NotImplementedException();
    }

    //private void HighlighPlotOnReflection(Data data)
    //{
    //    if (_onSelect != null)
    //    {
    //        var t = _onSelect.GetType();
    //        t.GetProperty("Color").SetValue(_onSelect, _onSelectColor);
    //        t.GetProperty("IsHighlighted").SetValue(_onSelect, false);
    //    }

    //    _onSelect = _plots[data];
    //    var type = _onSelect.GetType();
    //    var color = type.GetProperty("Color").GetValue(_onSelect);
    //    _onSelectColor = (Color)color;
    //    type.GetProperty("Color").SetValue(_onSelect, Color.Black);
    //    type.GetProperty("IsHighlighted").SetValue(_onSelect, true);
    //}

    private void ChangePlot(ISet<Data> dataToBePlotted)
    {
        var toAdd = dataToBePlotted
            .Where(d => !_plots.ContainsKey(d))
            .ToArray();
        var toRemove = _plots.Keys
            .Where(d => !dataToBePlotted.Contains(d))
            .ToArray();
        Parallel.ForEach(toAdd, AddToPlot);
        Parallel.ForEach(toRemove, RemoveFromPlot);
    }

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

        lock (_builder)
        {
            if (spectra is ASP asp)
                plot = _builder.AddSignal(yS, 1 / asp.Delta);
            else
                plot = _builder.AddSignalXY(_xSPlots[xS], yS);
        }

        lock (_plots)
            _plots.Add(spectra, plot);
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
