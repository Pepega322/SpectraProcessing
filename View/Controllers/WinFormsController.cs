using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using ScottPlot;
using ScottPlot.WinForms;

namespace View.Controllers;
public class WinFormsController {
    private RootController dirController;
    private DataController dataController;
    private ScottPlotController plotController;

    public IPlottable? ContextPlot { get; set; }
    private SortedSet<Data> dataToBePloted;

    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;
    public event Action? OnRootChanged;

    public WinFormsController(FormsPlot plot) {
        dataController = new WindowsDataController(new WindowsWriter());
        //var pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //dirController= new WorkingDirectory(pathToDesktop);
        //dirController= new WorkingDirectory("D:\\Study\\Chemfuck\\Lab\\MixturesData");
        dirController = new WindowsDirectoryController("d:\\Study\\Chemfuck\\Lab\\MixturesData\\single-components\\sugar-our\\");
        plotController = new ScottPlotController(plot);
        dataToBePloted = [];
    }

    #region PlotMethods

    public async Task ContextSetPlotAsync() {
        PrivateClearPlot();
        await ContextSetAddToPlotAsync();
    }

    public async Task ContextSetAddToPlotAsync() {
        foreach (var data in dataController.ContextSet.Data)
            dataToBePloted.Add(data);
        await UpdatePlotAsync();
    }

    public async Task ContextDataPlotAsync() {
        PrivateClearPlot();
        dataToBePloted.Add(dataController.ContextData);
        await UpdatePlotAsync();
    }

    public async Task ContextDataAddToPlotAsync(object dataItem) {
        if (dataItem is Data data) {
            dataToBePloted.Add(data);
            await UpdatePlotAsync();
        }
    }

    public void PlotClear() {
        PrivateClearPlot();
        OnPlotChanged?.Invoke();
    }

    public void PlotChangeVisibility(object plotItem, bool isVisible) {
        if (plotItem is Data data)
            plotController.ChangeVisibility(data, isVisible);
    }

    public void PlotSelect(object plotItem) {
        if (plotItem is Data data)
            plotController.SelectPlot(data);
    }

    private void PrivateClearPlot() {
        dataToBePloted.Clear();
        plotController.Clear();
    }

    private async Task UpdatePlotAsync() {
        await plotController.PlotSet(dataToBePloted);
        OnPlotChanged?.Invoke();
    }

    public IEnumerable<TreeNode> PlotGetTree() => plotController.GetPlotNodes();

    #endregion

    #region DirectoryControllerMethods

    public async Task RootReadWithSubdirsAsync() {
        var set = await dirController.ReadRoot(true);
        if (dataController.AddSet(set.Name, set))
            OnDataChanged?.Invoke();
    }

    public async Task RootReadAsync() {
        var set = await dirController.ReadRoot();
        if (dataController.AddSet(set.Name, set))
            OnDataChanged?.Invoke();
    }

    public void RootSelect() {
        var selectedPath = dirController.SelectPathInDialog();
        if (selectedPath != null && dirController.ChangeRoot(selectedPath))
            OnRootChanged?.Invoke();
    }

    public void RootStepBack() {
        if (dirController.StepBack())
            OnRootChanged?.Invoke();
    }

    public void RootRefresh() {
        OnRootChanged?.Invoke();
    }

    public async void RootDoubleClick(object rootItem) {
        if (rootItem is FileInfo file) {
            var data = await dirController.ReadData(file.FullName);
            if (dataController.AddDataToDefaultSet(data))
                OnDataChanged?.Invoke();
        }

        if (rootItem is DirectoryInfo newRoot) {
            if (dirController.ChangeRoot(newRoot.FullName))
                OnRootChanged?.Invoke();
        }
    }
    public IEnumerable<TreeNode> RootGetTree() => dirController.GetTree();

    #endregion

    #region DataControllerMethods

    public async Task ContextSetAndSubsetsSaveAsESPAsync() {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        var outputPath = Path.Combine(path, $"{dataController.ContextSet.Name} (converted all)");
        await dataController.WriteSetAsAsync(outputPath, ".esp", true);
    }

    public async Task ContextSetSaveAsESPAsync() {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        var outputPath = Path.Combine(path, $"{dataController.ContextSet.Name} (converted only this)");
        await dataController.WriteSetAsAsync(outputPath, ".esp", false);
    }

    public async Task ContextDataSaveAsESPAsync() {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        await dataController.WriteDataAsAsync(path, ".esp");
    }

    public void ContextSetDelete() {
        if (dataController.DeleteSet())
            OnDataChanged?.Invoke();
    }

    public void ContextDataDelete() {
        if (dataController.DeleteData())
            OnDataChanged?.Invoke();
    }

    public void DataClear() {
        dataController.Clear();
        OnDataChanged?.Invoke();
    }

    public bool ContextSetChange(TreeSet set) => dataController.ChangeContextSet(set);

    public bool ContextDataChange(Data data) => dataController.ChangeContextData(data);

    public IEnumerable<TreeNode> DataGetTree() => dataController.GetTree();

    #endregion
}
