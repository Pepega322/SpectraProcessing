namespace View;

partial class MainForm {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        tableLayoutPanelMain = new TableLayoutPanel();
        mainMenuStrip = new MenuStrip();
        rootButtonSelect = new ToolStripMenuItem();
        rootRead = new ToolStripMenuItem();
        rootButtonRead = new ToolStripMenuItem();
        rootButtonReadWithSubdirs = new ToolStripMenuItem();
        tableLayoutPanelMiddle = new TableLayoutPanel();
        tableLayoutPanelLeft = new TableLayoutPanel();
        tableLayoutPanelLeftUp = new TableLayoutPanel();
        rootNavigation = new MenuStrip();
        rootButtonStepBack = new ToolStripMenuItem();
        rootButtonRefresh = new ToolStripMenuItem();
        rootTree = new TreeView();
        imageListForRootFolder = new ImageList(components);
        tableLayoutPanelLeftDown = new TableLayoutPanel();
        dataTree = new TreeView();
        dataNavigation = new MenuStrip();
        dataButtonClear = new ToolStripMenuItem();
        tableLayoutPanelMiddleMiddle = new TableLayoutPanel();
        plotNavigation = new MenuStrip();
        plotButtonAddPeak = new ToolStripMenuItem();
        plotButtonDeleteLastPeak = new ToolStripMenuItem();
        plotButtonClearPeaks = new ToolStripMenuItem();
        tableLayoutPanelRightUp = new TableLayoutPanel();
        mouseCoordinatesBox = new TextBox();
        plotView = new ScottPlot.WinForms.FormsPlot();
        tableLayoutPanel1 = new TableLayoutPanel();
        menuStrip1 = new MenuStrip();
        plotButtonClear = new ToolStripMenuItem();
        plotButtonResize = new ToolStripMenuItem();
        plotTree = new TreeView();
        dataSetMenu = new ContextMenuStrip(components);
        dataNodeContextSave = new ToolStripMenuItem();
        dataContextDataSetSaveAs = new ToolStripMenuItem();
        dataContextDataSetAndSubsetsSaveAs = new ToolStripMenuItem();
        dataContextDataSetPlot = new ToolStripMenuItem();
        dataContextDataSetAddToPlot = new ToolStripMenuItem();
        dataContextDataSetDelete = new ToolStripMenuItem();
        dataNodeContextSubstractBaseline = new ToolStripMenuItem();
        dataContextDataSetSubstactBaseline = new ToolStripMenuItem();
        dataContextDataSetAndSubsetsSubstractBaseline = new ToolStripMenuItem();
        dataMenu = new ContextMenuStrip(components);
        dataContextDataSave = new ToolStripMenuItem();
        dataContextDataPlot = new ToolStripMenuItem();
        dataContextDataDelete = new ToolStripMenuItem();
        dataContextDataSubstractBaseline = new ToolStripMenuItem();
        plotSetMenu = new ContextMenuStrip(components);
        plotContextPlotSetHighlight = new ToolStripMenuItem();
        plotContextPlotSetDelete = new ToolStripMenuItem();
        plotContextPlotSetPeaksProcess = new ToolStripMenuItem();
        plotMenu = new ContextMenuStrip(components);
        plotContextPlotDelete = new ToolStripMenuItem();
        plotContextPlotPeaksProcess = new ToolStripMenuItem();
        tableLayoutPanelMain.SuspendLayout();
        mainMenuStrip.SuspendLayout();
        tableLayoutPanelMiddle.SuspendLayout();
        tableLayoutPanelLeft.SuspendLayout();
        tableLayoutPanelLeftUp.SuspendLayout();
        rootNavigation.SuspendLayout();
        tableLayoutPanelLeftDown.SuspendLayout();
        dataNavigation.SuspendLayout();
        tableLayoutPanelMiddleMiddle.SuspendLayout();
        plotNavigation.SuspendLayout();
        tableLayoutPanelRightUp.SuspendLayout();
        tableLayoutPanel1.SuspendLayout();
        menuStrip1.SuspendLayout();
        dataSetMenu.SuspendLayout();
        dataMenu.SuspendLayout();
        plotSetMenu.SuspendLayout();
        plotMenu.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanelMain
        // 
        tableLayoutPanelMain.ColumnCount = 1;
        tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelMain.Controls.Add(mainMenuStrip, 0, 0);
        tableLayoutPanelMain.Controls.Add(tableLayoutPanelMiddle, 0, 1);
        tableLayoutPanelMain.Dock = DockStyle.Fill;
        tableLayoutPanelMain.Location = new Point(0, 0);
        tableLayoutPanelMain.Name = "tableLayoutPanelMain";
        tableLayoutPanelMain.RowCount = 3;
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanelMain.Size = new Size(1894, 1009);
        tableLayoutPanelMain.TabIndex = 0;
        // 
        // mainMenuStrip
        // 
        mainMenuStrip.Dock = DockStyle.Fill;
        mainMenuStrip.ImageScalingSize = new Size(32, 32);
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { rootButtonSelect, rootRead });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1894, 40);
        mainMenuStrip.TabIndex = 1;
        mainMenuStrip.Text = "menuStrip1";
        // 
        // rootButtonSelect
        // 
        rootButtonSelect.Name = "rootButtonSelect";
        rootButtonSelect.ShortcutKeys = Keys.Control | Keys.Tab;
        rootButtonSelect.Size = new Size(168, 36);
        rootButtonSelect.Text = "Select folder";
        // 
        // rootRead
        // 
        rootRead.DropDownItems.AddRange(new ToolStripItem[] { rootButtonRead, rootButtonReadWithSubdirs });
        rootRead.Name = "rootRead";
        rootRead.Size = new Size(86, 36);
        rootRead.Text = "Read";
        // 
        // rootButtonRead
        // 
        rootButtonRead.Name = "rootButtonRead";
        rootButtonRead.ShortcutKeys = Keys.Control | Keys.R;
        rootButtonRead.Size = new Size(571, 44);
        rootButtonRead.Text = "Only this folder";
        rootButtonRead.TextImageRelation = TextImageRelation.TextAboveImage;
        // 
        // rootButtonReadWithSubdirs
        // 
        rootButtonReadWithSubdirs.Name = "rootButtonReadWithSubdirs";
        rootButtonReadWithSubdirs.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
        rootButtonReadWithSubdirs.Size = new Size(571, 44);
        rootButtonReadWithSubdirs.Text = "This folder and subfolders";
        // 
        // tableLayoutPanelMiddle
        // 
        tableLayoutPanelMiddle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanelMiddle.ColumnCount = 3;
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500F));
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanelLeft, 0, 0);
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanelMiddleMiddle, 1, 0);
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanel1, 2, 0);
        tableLayoutPanelMiddle.Dock = DockStyle.Fill;
        tableLayoutPanelMiddle.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
        tableLayoutPanelMiddle.Location = new Point(3, 43);
        tableLayoutPanelMiddle.Name = "tableLayoutPanelMiddle";
        tableLayoutPanelMiddle.RowCount = 1;
        tableLayoutPanelMiddle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddle.Size = new Size(1888, 923);
        tableLayoutPanelMiddle.TabIndex = 2;
        // 
        // tableLayoutPanelLeft
        // 
        tableLayoutPanelLeft.ColumnCount = 1;
        tableLayoutPanelLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanelLeft.Controls.Add(tableLayoutPanelLeftUp, 0, 0);
        tableLayoutPanelLeft.Controls.Add(tableLayoutPanelLeftDown, 0, 1);
        tableLayoutPanelLeft.Dock = DockStyle.Fill;
        tableLayoutPanelLeft.Location = new Point(3, 3);
        tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
        tableLayoutPanelLeft.RowCount = 2;
        tableLayoutPanelLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanelLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanelLeft.Size = new Size(344, 917);
        tableLayoutPanelLeft.TabIndex = 2;
        // 
        // tableLayoutPanelLeftUp
        // 
        tableLayoutPanelLeftUp.ColumnCount = 1;
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanelLeftUp.Controls.Add(rootNavigation, 0, 0);
        tableLayoutPanelLeftUp.Controls.Add(rootTree, 0, 1);
        tableLayoutPanelLeftUp.Dock = DockStyle.Fill;
        tableLayoutPanelLeftUp.Location = new Point(3, 3);
        tableLayoutPanelLeftUp.Name = "tableLayoutPanelLeftUp";
        tableLayoutPanelLeftUp.RowCount = 2;
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.Size = new Size(338, 452);
        tableLayoutPanelLeftUp.TabIndex = 1;
        // 
        // rootNavigation
        // 
        rootNavigation.Dock = DockStyle.Fill;
        rootNavigation.ImageScalingSize = new Size(32, 32);
        rootNavigation.Items.AddRange(new ToolStripItem[] { rootButtonStepBack, rootButtonRefresh });
        rootNavigation.Location = new Point(0, 0);
        rootNavigation.Name = "rootNavigation";
        rootNavigation.Size = new Size(338, 40);
        rootNavigation.TabIndex = 0;
        rootNavigation.Text = "menuStrip1";
        // 
        // rootButtonStepBack
        // 
        rootButtonStepBack.Name = "rootButtonStepBack";
        rootButtonStepBack.Size = new Size(83, 36);
        rootButtonStepBack.Text = "Back";
        // 
        // rootButtonRefresh
        // 
        rootButtonRefresh.Name = "rootButtonRefresh";
        rootButtonRefresh.Size = new Size(113, 36);
        rootButtonRefresh.Text = "Refresh";
        // 
        // rootTree
        // 
        rootTree.Dock = DockStyle.Fill;
        rootTree.ImageIndex = 1;
        rootTree.ImageList = imageListForRootFolder;
        rootTree.ImeMode = ImeMode.NoControl;
        rootTree.Location = new Point(3, 43);
        rootTree.Name = "rootTree";
        rootTree.SelectedImageIndex = 2;
        rootTree.Size = new Size(332, 406);
        rootTree.TabIndex = 1;
        // 
        // imageListForRootFolder
        // 
        imageListForRootFolder.ColorDepth = ColorDepth.Depth32Bit;
        imageListForRootFolder.ImageStream = (ImageListStreamer)resources.GetObject("imageListForRootFolder.ImageStream");
        imageListForRootFolder.TransparentColor = Color.Transparent;
        imageListForRootFolder.Images.SetKeyName(0, "folderIcon.png");
        imageListForRootFolder.Images.SetKeyName(1, "fileIcon.png");
        imageListForRootFolder.Images.SetKeyName(2, "emptyIcon.png");
        // 
        // tableLayoutPanelLeftDown
        // 
        tableLayoutPanelLeftDown.ColumnCount = 1;
        tableLayoutPanelLeftDown.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftDown.Controls.Add(dataTree, 0, 1);
        tableLayoutPanelLeftDown.Controls.Add(dataNavigation, 0, 0);
        tableLayoutPanelLeftDown.Dock = DockStyle.Fill;
        tableLayoutPanelLeftDown.Location = new Point(3, 461);
        tableLayoutPanelLeftDown.Name = "tableLayoutPanelLeftDown";
        tableLayoutPanelLeftDown.RowCount = 2;
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftDown.Size = new Size(338, 453);
        tableLayoutPanelLeftDown.TabIndex = 2;
        // 
        // dataTree
        // 
        dataTree.Dock = DockStyle.Fill;
        dataTree.Location = new Point(3, 43);
        dataTree.Name = "dataTree";
        dataTree.Size = new Size(332, 407);
        dataTree.TabIndex = 5;
        // 
        // dataNavigation
        // 
        dataNavigation.ImageScalingSize = new Size(32, 32);
        dataNavigation.Items.AddRange(new ToolStripItem[] { dataButtonClear });
        dataNavigation.Location = new Point(0, 0);
        dataNavigation.Name = "dataNavigation";
        dataNavigation.Size = new Size(338, 40);
        dataNavigation.TabIndex = 6;
        dataNavigation.Text = "menuStrip1";
        // 
        // dataButtonClear
        // 
        dataButtonClear.Name = "dataButtonClear";
        dataButtonClear.Size = new Size(88, 36);
        dataButtonClear.Text = "Clear";
        // 
        // tableLayoutPanelMiddleMiddle
        // 
        tableLayoutPanelMiddleMiddle.ColumnCount = 1;
        tableLayoutPanelMiddleMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddleMiddle.Controls.Add(plotNavigation, 0, 0);
        tableLayoutPanelMiddleMiddle.Controls.Add(tableLayoutPanelRightUp, 0, 2);
        tableLayoutPanelMiddleMiddle.Controls.Add(plotView, 0, 1);
        tableLayoutPanelMiddleMiddle.Dock = DockStyle.Fill;
        tableLayoutPanelMiddleMiddle.Location = new Point(353, 3);
        tableLayoutPanelMiddleMiddle.Name = "tableLayoutPanelMiddleMiddle";
        tableLayoutPanelMiddleMiddle.RowCount = 3;
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanelMiddleMiddle.Size = new Size(1032, 917);
        tableLayoutPanelMiddleMiddle.TabIndex = 3;
        // 
        // plotNavigation
        // 
        plotNavigation.Dock = DockStyle.Fill;
        plotNavigation.ImageScalingSize = new Size(32, 32);
        plotNavigation.Items.AddRange(new ToolStripItem[] { plotButtonAddPeak, plotButtonDeleteLastPeak, plotButtonClearPeaks });
        plotNavigation.Location = new Point(0, 0);
        plotNavigation.Name = "plotNavigation";
        plotNavigation.Size = new Size(1032, 40);
        plotNavigation.TabIndex = 8;
        plotNavigation.Text = "menuStrip1";
        // 
        // plotButtonAddPeak
        // 
        plotButtonAddPeak.ForeColor = SystemColors.ControlText;
        plotButtonAddPeak.Name = "plotButtonAddPeak";
        plotButtonAddPeak.ShortcutKeyDisplayString = "Ctrl+S";
        plotButtonAddPeak.ShortcutKeys = Keys.Control | Keys.S;
        plotButtonAddPeak.Size = new Size(135, 36);
        plotButtonAddPeak.Text = "Add peak";
        // 
        // plotButtonDeleteLastPeak
        // 
        plotButtonDeleteLastPeak.Name = "plotButtonDeleteLastPeak";
        plotButtonDeleteLastPeak.ShortcutKeyDisplayString = "Ctrl+Z";
        plotButtonDeleteLastPeak.ShortcutKeys = Keys.Control | Keys.Z;
        plotButtonDeleteLastPeak.Size = new Size(147, 36);
        plotButtonDeleteLastPeak.Text = "Delete last";
        // 
        // plotButtonClearPeaks
        // 
        plotButtonClearPeaks.Name = "plotButtonClearPeaks";
        plotButtonClearPeaks.Size = new Size(156, 36);
        plotButtonClearPeaks.Text = "Clear peaks";
        // 
        // tableLayoutPanelRightUp
        // 
        tableLayoutPanelRightUp.ColumnCount = 2;
        tableLayoutPanelRightUp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
        tableLayoutPanelRightUp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        tableLayoutPanelRightUp.Controls.Add(mouseCoordinatesBox, 0, 0);
        tableLayoutPanelRightUp.Dock = DockStyle.Fill;
        tableLayoutPanelRightUp.Location = new Point(3, 880);
        tableLayoutPanelRightUp.Name = "tableLayoutPanelRightUp";
        tableLayoutPanelRightUp.RowCount = 1;
        tableLayoutPanelRightUp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelRightUp.Size = new Size(1026, 34);
        tableLayoutPanelRightUp.TabIndex = 6;
        // 
        // mouseCoordinatesBox
        // 
        mouseCoordinatesBox.Dock = DockStyle.Fill;
        mouseCoordinatesBox.Location = new Point(3, 3);
        mouseCoordinatesBox.Name = "mouseCoordinatesBox";
        mouseCoordinatesBox.Size = new Size(814, 39);
        mouseCoordinatesBox.TabIndex = 7;
        // 
        // plotView
        // 
        plotView.DisplayScale = 2F;
        plotView.Dock = DockStyle.Fill;
        plotView.Location = new Point(3, 43);
        plotView.Name = "plotView";
        plotView.Size = new Size(1026, 831);
        plotView.TabIndex = 9;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Controls.Add(menuStrip1, 0, 0);
        tableLayoutPanel1.Controls.Add(plotTree, 0, 1);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(1391, 3);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(494, 917);
        tableLayoutPanel1.TabIndex = 4;
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(32, 32);
        menuStrip1.Items.AddRange(new ToolStripItem[] { plotButtonClear, plotButtonResize });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(494, 40);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // plotButtonClear
        // 
        plotButtonClear.Name = "plotButtonClear";
        plotButtonClear.Size = new Size(88, 36);
        plotButtonClear.Text = "Clear";
        // 
        // plotButtonResize
        // 
        plotButtonResize.Name = "plotButtonResize";
        plotButtonResize.Size = new Size(100, 36);
        plotButtonResize.Text = "Resize";
        // 
        // plotTree
        // 
        plotTree.CheckBoxes = true;
        plotTree.Dock = DockStyle.Fill;
        plotTree.Location = new Point(3, 43);
        plotTree.Name = "plotTree";
        plotTree.Size = new Size(488, 871);
        plotTree.TabIndex = 1;
        // 
        // dataSetMenu
        // 
        dataSetMenu.ImageScalingSize = new Size(32, 32);
        dataSetMenu.Items.AddRange(new ToolStripItem[] { dataNodeContextSave, dataContextDataSetPlot, dataContextDataSetAddToPlot, dataContextDataSetDelete, dataNodeContextSubstractBaseline });
        dataSetMenu.Name = "dataNodeContext1";
        dataSetMenu.Size = new Size(386, 194);
        dataSetMenu.Tag = "";
        // 
        // dataNodeContextSave
        // 
        dataNodeContextSave.DropDownItems.AddRange(new ToolStripItem[] { dataContextDataSetSaveAs, dataContextDataSetAndSubsetsSaveAs });
        dataNodeContextSave.Name = "dataNodeContextSave";
        dataNodeContextSave.Size = new Size(385, 38);
        dataNodeContextSave.Text = "Save series as .esp";
        // 
        // dataContextDataSetSaveAs
        // 
        dataContextDataSetSaveAs.Name = "dataContextDataSetSaveAs";
        dataContextDataSetSaveAs.Size = new Size(409, 44);
        dataContextDataSetSaveAs.Text = "Only this series";
        // 
        // dataContextDataSetAndSubsetsSaveAs
        // 
        dataContextDataSetAndSubsetsSaveAs.Name = "dataContextDataSetAndSubsetsSaveAs";
        dataContextDataSetAndSubsetsSaveAs.Size = new Size(409, 44);
        dataContextDataSetAndSubsetsSaveAs.Text = "This series and subseries";
        // 
        // dataContextDataSetPlot
        // 
        dataContextDataSetPlot.Name = "dataContextDataSetPlot";
        dataContextDataSetPlot.Size = new Size(385, 38);
        dataContextDataSetPlot.Text = "Plot series";
        // 
        // dataContextDataSetAddToPlot
        // 
        dataContextDataSetAddToPlot.Name = "dataContextDataSetAddToPlot";
        dataContextDataSetAddToPlot.Size = new Size(385, 38);
        dataContextDataSetAddToPlot.Text = "Add series to plot";
        // 
        // dataContextDataSetDelete
        // 
        dataContextDataSetDelete.Name = "dataContextDataSetDelete";
        dataContextDataSetDelete.Size = new Size(385, 38);
        dataContextDataSetDelete.Text = "Delete series";
        // 
        // dataNodeContextSubstractBaseline
        // 
        dataNodeContextSubstractBaseline.DropDownItems.AddRange(new ToolStripItem[] { dataContextDataSetSubstactBaseline, dataContextDataSetAndSubsetsSubstractBaseline });
        dataNodeContextSubstractBaseline.Name = "dataNodeContextSubstractBaseline";
        dataNodeContextSubstractBaseline.Size = new Size(385, 38);
        dataNodeContextSubstractBaseline.Text = "Substract baseline for series";
        // 
        // dataContextDataSetSubstactBaseline
        // 
        dataContextDataSetSubstactBaseline.Name = "dataContextDataSetSubstactBaseline";
        dataContextDataSetSubstactBaseline.Size = new Size(409, 44);
        dataContextDataSetSubstactBaseline.Text = "Only this series";
        // 
        // dataContextDataSetAndSubsetsSubstractBaseline
        // 
        dataContextDataSetAndSubsetsSubstractBaseline.Name = "dataContextDataSetAndSubsetsSubstractBaseline";
        dataContextDataSetAndSubsetsSubstractBaseline.Size = new Size(409, 44);
        dataContextDataSetAndSubsetsSubstractBaseline.Text = "This series and subseries";
        // 
        // dataMenu
        // 
        dataMenu.ImageScalingSize = new Size(32, 32);
        dataMenu.Items.AddRange(new ToolStripItem[] { dataContextDataSave, dataContextDataPlot, dataContextDataDelete, dataContextDataSubstractBaseline });
        dataMenu.Name = "contextMenuStrip2";
        dataMenu.Size = new Size(282, 156);
        // 
        // dataContextDataSave
        // 
        dataContextDataSave.Name = "dataContextDataSave";
        dataContextDataSave.Size = new Size(281, 38);
        dataContextDataSave.Text = "Save as .esp";
        // 
        // dataContextDataPlot
        // 
        dataContextDataPlot.Name = "dataContextDataPlot";
        dataContextDataPlot.Size = new Size(281, 38);
        dataContextDataPlot.Text = "Plot";
        // 
        // dataContextDataDelete
        // 
        dataContextDataDelete.Name = "dataContextDataDelete";
        dataContextDataDelete.Size = new Size(281, 38);
        dataContextDataDelete.Text = "Delete";
        // 
        // dataContextDataSubstractBaseline
        // 
        dataContextDataSubstractBaseline.Name = "dataContextDataSubstractBaseline";
        dataContextDataSubstractBaseline.Size = new Size(281, 38);
        dataContextDataSubstractBaseline.Text = "Substract baseline";
        // 
        // plotSetMenu
        // 
        plotSetMenu.ImageScalingSize = new Size(32, 32);
        plotSetMenu.Items.AddRange(new ToolStripItem[] { plotContextPlotSetHighlight, plotContextPlotSetDelete, plotContextPlotSetPeaksProcess });
        plotSetMenu.Name = "plotSetMenu";
        plotSetMenu.Size = new Size(340, 118);
        // 
        // plotContextPlotSetHighlight
        // 
        plotContextPlotSetHighlight.Name = "plotContextPlotSetHighlight";
        plotContextPlotSetHighlight.Size = new Size(339, 38);
        plotContextPlotSetHighlight.Text = "Highlight On\\Off";
        // 
        // plotContextPlotSetDelete
        // 
        plotContextPlotSetDelete.Name = "plotContextPlotSetDelete";
        plotContextPlotSetDelete.Size = new Size(339, 38);
        plotContextPlotSetDelete.Text = "Delete series";
        // 
        // plotContextPlotSetPeaksProcess
        // 
        plotContextPlotSetPeaksProcess.Name = "plotContextPlotSetPeaksProcess";
        plotContextPlotSetPeaksProcess.Size = new Size(339, 38);
        plotContextPlotSetPeaksProcess.Text = "Process peaks for series";
        // 
        // plotMenu
        // 
        plotMenu.ImageScalingSize = new Size(32, 32);
        plotMenu.Items.AddRange(new ToolStripItem[] { plotContextPlotDelete, plotContextPlotPeaksProcess });
        plotMenu.Name = "plotMenu";
        plotMenu.Size = new Size(236, 80);
        // 
        // plotContextPlotDelete
        // 
        plotContextPlotDelete.Name = "plotContextPlotDelete";
        plotContextPlotDelete.Size = new Size(235, 38);
        plotContextPlotDelete.Text = "Delete";
        // 
        // plotContextPlotPeaksProcess
        // 
        plotContextPlotPeaksProcess.Name = "plotContextPlotPeaksProcess";
        plotContextPlotPeaksProcess.Size = new Size(235, 38);
        plotContextPlotPeaksProcess.Text = "Process peaks";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Window;
        ClientSize = new Size(1894, 1009);
        Controls.Add(tableLayoutPanelMain);
        MainMenuStrip = mainMenuStrip;
        Name = "MainForm";
        Text = "Spectra Processing";
        tableLayoutPanelMain.ResumeLayout(false);
        tableLayoutPanelMain.PerformLayout();
        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        tableLayoutPanelMiddle.ResumeLayout(false);
        tableLayoutPanelLeft.ResumeLayout(false);
        tableLayoutPanelLeftUp.ResumeLayout(false);
        tableLayoutPanelLeftUp.PerformLayout();
        rootNavigation.ResumeLayout(false);
        rootNavigation.PerformLayout();
        tableLayoutPanelLeftDown.ResumeLayout(false);
        tableLayoutPanelLeftDown.PerformLayout();
        dataNavigation.ResumeLayout(false);
        dataNavigation.PerformLayout();
        tableLayoutPanelMiddleMiddle.ResumeLayout(false);
        tableLayoutPanelMiddleMiddle.PerformLayout();
        plotNavigation.ResumeLayout(false);
        plotNavigation.PerformLayout();
        tableLayoutPanelRightUp.ResumeLayout(false);
        tableLayoutPanelRightUp.PerformLayout();
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        dataSetMenu.ResumeLayout(false);
        dataMenu.ResumeLayout(false);
        plotSetMenu.ResumeLayout(false);
        plotMenu.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanelMain;
    private MenuStrip mainMenuStrip;
    private ImageList imageListForRootFolder;
    private TableLayoutPanel tableLayoutPanelMiddle;
    private TableLayoutPanel tableLayoutPanelLeft;
    private TableLayoutPanel tableLayoutPanelLeftUp;
    private MenuStrip rootNavigation;
    private TreeView rootTree;
    private ToolStripMenuItem rootButtonSelect;
    private TreeView dataTree;
    private TableLayoutPanel tableLayoutPanelLeftDown;
    private MenuStrip dataNavigation;
    private TableLayoutPanel tableLayoutPanelMiddleMiddle;
    private TableLayoutPanel tableLayoutPanelRightUp;
    private TextBox mouseCoordinatesBox;
    private MenuStrip plotNavigation;
    private ToolStripMenuItem rootRead;
    private ToolStripMenuItem rootButtonRead;
    private ToolStripMenuItem rootButtonReadWithSubdirs;
    private ToolStripMenuItem rootButtonStepBack;
    private ContextMenuStrip dataSetMenu;
    private ContextMenuStrip dataMenu;
    private ToolStripMenuItem dataNodeContextSave;
    private ToolStripMenuItem dataContextDataSetSaveAs;
    private ToolStripMenuItem dataContextDataSetAndSubsetsSaveAs;
    private ToolStripMenuItem dataContextDataSetPlot;
    private ToolStripMenuItem dataContextDataSetAddToPlot;
    private ToolStripMenuItem dataContextDataSetDelete;
    private ToolStripMenuItem dataContextDataSave;
    private ToolStripMenuItem dataContextDataPlot;
    private ToolStripMenuItem dataContextDataDelete;
    private TableLayoutPanel tableLayoutPanel1;
    private MenuStrip menuStrip1;
    private TreeView plotTree;
    private ToolStripMenuItem plotButtonClear;
    private ScottPlot.WinForms.FormsPlot plotView;
    private ToolStripMenuItem dataNodeContextSubstractBaseline;
    private ToolStripMenuItem dataContextDataSubstractBaseline;
    private ToolStripMenuItem dataButtonClear;
    private ToolStripMenuItem rootButtonRefresh;
    private ContextMenuStrip plotSetMenu;
    private ContextMenuStrip plotMenu;
    private ToolStripMenuItem plotContextPlotDelete;
    private ToolStripMenuItem plotContextPlotSetDelete;
    private ToolStripMenuItem plotContextPlotSetHighlight;
    private ToolStripMenuItem dataContextDataSetSubstactBaseline;
    private ToolStripMenuItem dataContextDataSetAndSubsetsSubstractBaseline;
    private ToolStripMenuItem plotContextPlotSetPeaksProcess;
    private ToolStripMenuItem plotContextPlotPeaksProcess;
    private ToolStripMenuItem plotButtonAddPeak;
    private ToolStripMenuItem plotButtonDeleteLastPeak;
    private ToolStripMenuItem plotButtonClearPeaks;
    private ToolStripMenuItem plotButtonResize;
}