namespace SpectraProcessing.Application;

sealed partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        components = new System.ComponentModel.Container();
        tableLayoutPanel1 = new TableLayoutPanel();
        tableLayoutPanel2 = new TableLayoutPanel();
        tableLayoutPanel3 = new TableLayoutPanel();
        tableLayoutPanel4 = new TableLayoutPanel();
        menuStrip2 = new MenuStrip();
        resizeToolStripMenuItem = new ToolStripMenuItem();
        addOrRemovePeaksToolStripMenuItem = new ToolStripMenuItem();
        customPeaksToolStripMenuItem = new ToolStripMenuItem();
        clearPeaksToolStripMenuItem = new ToolStripMenuItem();
        baselineModeToolStripMenuItem = new ToolStripMenuItem();
        plotView = new ScottPlot.WinForms.FormsPlot();
        tableLayoutPanel5 = new TableLayoutPanel();
        dataStorageTreeView = new TreeView();
        plotStorageTreeView = new TreeView();
        menuStrip1 = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        readFolderToolStripMenuItem = new ToolStripMenuItem();
        readFolderRecursiveToolStripMenuItem = new ToolStripMenuItem();
        peaksToolStripMenuItem = new ToolStripMenuItem();
        exportPeaksToolStripMenuItem = new ToolStripMenuItem();
        importPeaksToolStripMenuItem = new ToolStripMenuItem();
        dataContextMenu = new ContextMenuStrip(components);
        dataContextMenuPlot = new ToolStripMenuItem();
        dataContextMenuSaveAsEsp = new ToolStripMenuItem();
        dataContextMenuDelete = new ToolStripMenuItem();
        dataContextMenuClear = new ToolStripMenuItem();
        dataSetContextMenu = new ContextMenuStrip(components);
        dataSetContextMenuPlot = new ToolStripMenuItem();
        dataSetContextMenuAddToPlot = new ToolStripMenuItem();
        dataSetContextMenuSaveAsEsp = new ToolStripMenuItem();
        dataSetContextMenuSaveAsEspCurrent = new ToolStripMenuItem();
        dataSetContextMenuSaveAsEspRecursive = new ToolStripMenuItem();
        dataSetContextMenuDelete = new ToolStripMenuItem();
        dataSetContextMenuClear = new ToolStripMenuItem();
        plotContextMenu = new ContextMenuStrip(components);
        plotContextMenuDelete = new ToolStripMenuItem();
        spectraProcessingToolStripMenuItem1 = new ToolStripMenuItem();
        plotContextMenuSmooth = new ToolStripMenuItem();
        plotContextMenuSubstractBaseline = new ToolStripMenuItem();
        peaksProcessingToolStripMenuItem1 = new ToolStripMenuItem();
        plotContextMenuImportPeaks = new ToolStripMenuItem();
        plotContextMenuExportPeaks = new ToolStripMenuItem();
        plotContextMenuFitPeaks = new ToolStripMenuItem();
        plotContextMenuClear = new ToolStripMenuItem();
        plotSetContextMenu = new ContextMenuStrip(components);
        plotSetContextMenuHighlight = new ToolStripMenuItem();
        plotSetContextMenuDelete = new ToolStripMenuItem();
        spectraProcessingToolStripMenuItem = new ToolStripMenuItem();
        plotSetContextMenuSmooth = new ToolStripMenuItem();
        plotSetContextMenuSubstractBaseline = new ToolStripMenuItem();
        plotSetContextMenuGetAverage = new ToolStripMenuItem();
        peaksProcessingToolStripMenuItem = new ToolStripMenuItem();
        plotSetContextMenuImportPeaks = new ToolStripMenuItem();
        plotSetContextMenuExportPeaks = new ToolStripMenuItem();
        plotSetContextMenuFitPeaks = new ToolStripMenuItem();
        plotSetContextMenuClear = new ToolStripMenuItem();
        numericUpDown1 = new NumericUpDown();
        mouseCoordinatesBox = new TextBox();
        tableLayoutPanel1.SuspendLayout();
        tableLayoutPanel2.SuspendLayout();
        tableLayoutPanel3.SuspendLayout();
        tableLayoutPanel4.SuspendLayout();
        menuStrip2.SuspendLayout();
        tableLayoutPanel5.SuspendLayout();
        menuStrip1.SuspendLayout();
        dataContextMenu.SuspendLayout();
        dataSetContextMenu.SuspendLayout();
        plotContextMenu.SuspendLayout();
        plotSetContextMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
        SuspendLayout();
        //
        // tableLayoutPanel1
        //
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
        tableLayoutPanel1.Controls.Add(menuStrip1, 0, 0);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 3;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 98F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 2F));
        tableLayoutPanel1.Size = new Size(1894, 1009);
        tableLayoutPanel1.TabIndex = 0;
        //
        // tableLayoutPanel2
        //
        tableLayoutPanel2.ColumnCount = 2;
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 0);
        tableLayoutPanel2.Controls.Add(tableLayoutPanel5, 1, 0);
        tableLayoutPanel2.Dock = DockStyle.Fill;
        tableLayoutPanel2.Location = new Point(3, 48);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 1;
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel2.Size = new Size(1888, 938);
        tableLayoutPanel2.TabIndex = 0;
        //
        // tableLayoutPanel3
        //
        tableLayoutPanel3.ColumnCount = 1;
        tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
        tableLayoutPanel3.Controls.Add(plotView, 0, 1);
        tableLayoutPanel3.Dock = DockStyle.Fill;
        tableLayoutPanel3.Location = new Point(3, 3);
        tableLayoutPanel3.Name = "tableLayoutPanel3";
        tableLayoutPanel3.RowCount = 2;
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel3.Size = new Size(1504, 932);
        tableLayoutPanel3.TabIndex = 0;
        //
        // tableLayoutPanel4
        //
        tableLayoutPanel4.ColumnCount = 3;
        tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
        tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
        tableLayoutPanel4.Controls.Add(mouseCoordinatesBox, 2, 0);
        tableLayoutPanel4.Controls.Add(menuStrip2, 0, 0);
        tableLayoutPanel4.Controls.Add(numericUpDown1, 1, 0);
        tableLayoutPanel4.Dock = DockStyle.Fill;
        tableLayoutPanel4.Location = new Point(3, 3);
        tableLayoutPanel4.Name = "tableLayoutPanel4";
        tableLayoutPanel4.RowCount = 1;
        tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel4.Size = new Size(1498, 44);
        tableLayoutPanel4.TabIndex = 0;
        //
        // menuStrip2
        //
        menuStrip2.Dock = DockStyle.Fill;
        menuStrip2.ImageScalingSize = new Size(32, 32);
        menuStrip2.Items.AddRange(new ToolStripItem[] { resizeToolStripMenuItem, addOrRemovePeaksToolStripMenuItem, customPeaksToolStripMenuItem, clearPeaksToolStripMenuItem, baselineModeToolStripMenuItem });
        menuStrip2.Location = new Point(0, 0);
        menuStrip2.Name = "menuStrip2";
        menuStrip2.Size = new Size(1123, 44);
        menuStrip2.TabIndex = 1;
        menuStrip2.Text = "menuStrip2";
        //
        // resizeToolStripMenuItem
        //
        resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
        resizeToolStripMenuItem.Size = new Size(100, 40);
        resizeToolStripMenuItem.Text = "Resize";
        //
        // addOrRemovePeaksToolStripMenuItem
        //
        addOrRemovePeaksToolStripMenuItem.CheckOnClick = true;
        addOrRemovePeaksToolStripMenuItem.Name = "addOrRemovePeaksToolStripMenuItem";
        addOrRemovePeaksToolStripMenuItem.Size = new Size(237, 40);
        addOrRemovePeaksToolStripMenuItem.Text = "Add|Remove peaks";
        //
        // customPeaksToolStripMenuItem
        //
        customPeaksToolStripMenuItem.CheckOnClick = true;
        customPeaksToolStripMenuItem.Name = "customPeaksToolStripMenuItem";
        customPeaksToolStripMenuItem.Size = new Size(182, 40);
        customPeaksToolStripMenuItem.Text = "Custom Peaks";
        //
        // clearPeaksToolStripMenuItem
        //
        clearPeaksToolStripMenuItem.Name = "clearPeaksToolStripMenuItem";
        clearPeaksToolStripMenuItem.Size = new Size(156, 40);
        clearPeaksToolStripMenuItem.Text = "Clear peaks";
        //
        // baselineModeToolStripMenuItem
        //
        baselineModeToolStripMenuItem.Name = "baselineModeToolStripMenuItem";
        baselineModeToolStripMenuItem.Size = new Size(191, 40);
        baselineModeToolStripMenuItem.Text = "Baseline mode";
        //
        // plotView
        //
        plotView.DisplayScale = 2F;
        plotView.Dock = DockStyle.Fill;
        plotView.Location = new Point(3, 53);
        plotView.Name = "plotView";
        plotView.Size = new Size(1498, 876);
        plotView.TabIndex = 1;
        //
        // tableLayoutPanel5
        //
        tableLayoutPanel5.ColumnCount = 1;
        tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel5.Controls.Add(dataStorageTreeView, 0, 1);
        tableLayoutPanel5.Controls.Add(plotStorageTreeView, 0, 0);
        tableLayoutPanel5.Dock = DockStyle.Fill;
        tableLayoutPanel5.Location = new Point(1513, 3);
        tableLayoutPanel5.Name = "tableLayoutPanel5";
        tableLayoutPanel5.RowCount = 2;
        tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
        tableLayoutPanel5.Size = new Size(372, 932);
        tableLayoutPanel5.TabIndex = 1;
        //
        // dataStorageTreeView
        //
        dataStorageTreeView.Dock = DockStyle.Fill;
        dataStorageTreeView.Location = new Point(3, 562);
        dataStorageTreeView.Name = "dataStorageTreeView";
        dataStorageTreeView.Size = new Size(366, 367);
        dataStorageTreeView.TabIndex = 0;
        //
        // plotStorageTreeView
        //
        plotStorageTreeView.CheckBoxes = true;
        plotStorageTreeView.Dock = DockStyle.Fill;
        plotStorageTreeView.Location = new Point(3, 3);
        plotStorageTreeView.Name = "plotStorageTreeView";
        plotStorageTreeView.Size = new Size(366, 553);
        plotStorageTreeView.TabIndex = 1;
        //
        // menuStrip1
        //
        menuStrip1.ImageScalingSize = new Size(32, 32);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, peaksToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1894, 40);
        menuStrip1.TabIndex = 1;
        menuStrip1.Text = "menuStrip1";
        //
        // fileToolStripMenuItem
        //
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { readFolderToolStripMenuItem, readFolderRecursiveToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(71, 36);
        fileToolStripMenuItem.Text = "File";
        //
        // readFolderToolStripMenuItem
        //
        readFolderToolStripMenuItem.Name = "readFolderToolStripMenuItem";
        readFolderToolStripMenuItem.Size = new Size(371, 44);
        readFolderToolStripMenuItem.Text = "Read folder";
        //
        // readFolderRecursiveToolStripMenuItem
        //
        readFolderRecursiveToolStripMenuItem.Name = "readFolderRecursiveToolStripMenuItem";
        readFolderRecursiveToolStripMenuItem.Size = new Size(371, 44);
        readFolderRecursiveToolStripMenuItem.Text = "Read folder recursive";
        //
        // peaksToolStripMenuItem
        //
        peaksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportPeaksToolStripMenuItem, importPeaksToolStripMenuItem });
        peaksToolStripMenuItem.Name = "peaksToolStripMenuItem";
        peaksToolStripMenuItem.Size = new Size(93, 36);
        peaksToolStripMenuItem.Text = "Peaks";
        //
        // exportPeaksToolStripMenuItem
        //
        exportPeaksToolStripMenuItem.Name = "exportPeaksToolStripMenuItem";
        exportPeaksToolStripMenuItem.Size = new Size(286, 44);
        exportPeaksToolStripMenuItem.Text = "Export peaks";
        //
        // importPeaksToolStripMenuItem
        //
        importPeaksToolStripMenuItem.Name = "importPeaksToolStripMenuItem";
        importPeaksToolStripMenuItem.Size = new Size(286, 44);
        importPeaksToolStripMenuItem.Text = "Import peaks";
        //
        // dataContextMenu
        //
        dataContextMenu.ImageScalingSize = new Size(32, 32);
        dataContextMenu.Items.AddRange(new ToolStripItem[] { dataContextMenuPlot, dataContextMenuSaveAsEsp, dataContextMenuDelete, dataContextMenuClear });
        dataContextMenu.Name = "dataContextMenu";
        dataContextMenu.Size = new Size(217, 156);
        //
        // dataContextMenuPlot
        //
        dataContextMenuPlot.Name = "dataContextMenuPlot";
        dataContextMenuPlot.Size = new Size(216, 38);
        dataContextMenuPlot.Text = "Plot";
        //
        // dataContextMenuSaveAsEsp
        //
        dataContextMenuSaveAsEsp.Name = "dataContextMenuSaveAsEsp";
        dataContextMenuSaveAsEsp.Size = new Size(216, 38);
        dataContextMenuSaveAsEsp.Text = "Save as .esp";
        //
        // dataContextMenuDelete
        //
        dataContextMenuDelete.Name = "dataContextMenuDelete";
        dataContextMenuDelete.Size = new Size(216, 38);
        dataContextMenuDelete.Text = "Delete";
        //
        // dataContextMenuClear
        //
        dataContextMenuClear.Name = "dataContextMenuClear";
        dataContextMenuClear.Size = new Size(216, 38);
        dataContextMenuClear.Text = "Clear";
        //
        // dataSetContextMenu
        //
        dataSetContextMenu.ImageScalingSize = new Size(32, 32);
        dataSetContextMenu.Items.AddRange(new ToolStripItem[] { dataSetContextMenuPlot, dataSetContextMenuAddToPlot, dataSetContextMenuSaveAsEsp, dataSetContextMenuDelete, dataSetContextMenuClear });
        dataSetContextMenu.Name = "dataSetContextMenu";
        dataSetContextMenu.Size = new Size(217, 194);
        //
        // dataSetContextMenuPlot
        //
        dataSetContextMenuPlot.Name = "dataSetContextMenuPlot";
        dataSetContextMenuPlot.Size = new Size(216, 38);
        dataSetContextMenuPlot.Text = "Plot";
        //
        // dataSetContextMenuAddToPlot
        //
        dataSetContextMenuAddToPlot.Name = "dataSetContextMenuAddToPlot";
        dataSetContextMenuAddToPlot.Size = new Size(216, 38);
        dataSetContextMenuAddToPlot.Text = "Add to plot";
        //
        // dataSetContextMenuSaveAsEsp
        //
        dataSetContextMenuSaveAsEsp.DropDownItems.AddRange(new ToolStripItem[] { dataSetContextMenuSaveAsEspCurrent, dataSetContextMenuSaveAsEspRecursive });
        dataSetContextMenuSaveAsEsp.Name = "dataSetContextMenuSaveAsEsp";
        dataSetContextMenuSaveAsEsp.Size = new Size(216, 38);
        dataSetContextMenuSaveAsEsp.Text = "Save as .esp";
        //
        // dataSetContextMenuSaveAsEspCurrent
        //
        dataSetContextMenuSaveAsEspCurrent.Name = "dataSetContextMenuSaveAsEspCurrent";
        dataSetContextMenuSaveAsEspCurrent.Size = new Size(283, 44);
        dataSetContextMenuSaveAsEspCurrent.Text = "Current set";
        //
        // dataSetContextMenuSaveAsEspRecursive
        //
        dataSetContextMenuSaveAsEspRecursive.Name = "dataSetContextMenuSaveAsEspRecursive";
        dataSetContextMenuSaveAsEspRecursive.Size = new Size(283, 44);
        dataSetContextMenuSaveAsEspRecursive.Text = "Set recursive";
        //
        // dataSetContextMenuDelete
        //
        dataSetContextMenuDelete.Name = "dataSetContextMenuDelete";
        dataSetContextMenuDelete.Size = new Size(216, 38);
        dataSetContextMenuDelete.Text = "Delete";
        //
        // dataSetContextMenuClear
        //
        dataSetContextMenuClear.Name = "dataSetContextMenuClear";
        dataSetContextMenuClear.Size = new Size(216, 38);
        dataSetContextMenuClear.Text = "Clear";
        //
        // plotContextMenu
        //
        plotContextMenu.ImageScalingSize = new Size(32, 32);
        plotContextMenu.Items.AddRange(new ToolStripItem[] { plotContextMenuDelete, spectraProcessingToolStripMenuItem1, peaksProcessingToolStripMenuItem1, plotContextMenuClear });
        plotContextMenu.Name = "plotContextMenu";
        plotContextMenu.Size = new Size(289, 156);
        //
        // plotContextMenuDelete
        //
        plotContextMenuDelete.Name = "plotContextMenuDelete";
        plotContextMenuDelete.Size = new Size(288, 38);
        plotContextMenuDelete.Text = "Delete";
        //
        // spectraProcessingToolStripMenuItem1
        //
        spectraProcessingToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { plotContextMenuSmooth, plotContextMenuSubstractBaseline });
        spectraProcessingToolStripMenuItem1.Name = "spectraProcessingToolStripMenuItem1";
        spectraProcessingToolStripMenuItem1.Size = new Size(288, 38);
        spectraProcessingToolStripMenuItem1.Text = "Spectra processing";
        //
        // plotContextMenuSmooth
        //
        plotContextMenuSmooth.Name = "plotContextMenuSmooth";
        plotContextMenuSmooth.Size = new Size(340, 44);
        plotContextMenuSmooth.Text = "Smooth ";
        //
        // plotContextMenuSubstractBaseline
        //
        plotContextMenuSubstractBaseline.Name = "plotContextMenuSubstractBaseline";
        plotContextMenuSubstractBaseline.Size = new Size(340, 44);
        plotContextMenuSubstractBaseline.Text = "Substract baseline";
        //
        // peaksProcessingToolStripMenuItem1
        //
        peaksProcessingToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { plotContextMenuImportPeaks, plotContextMenuExportPeaks, plotContextMenuFitPeaks });
        peaksProcessingToolStripMenuItem1.Name = "peaksProcessingToolStripMenuItem1";
        peaksProcessingToolStripMenuItem1.Size = new Size(288, 38);
        peaksProcessingToolStripMenuItem1.Text = "Peaks processing";
        //
        // plotContextMenuImportPeaks
        //
        plotContextMenuImportPeaks.Name = "plotContextMenuImportPeaks";
        plotContextMenuImportPeaks.Size = new Size(286, 44);
        plotContextMenuImportPeaks.Text = "Import peaks";
        //
        // plotContextMenuExportPeaks
        //
        plotContextMenuExportPeaks.Name = "plotContextMenuExportPeaks";
        plotContextMenuExportPeaks.Size = new Size(286, 44);
        plotContextMenuExportPeaks.Text = "Export peaks";
        //
        // plotContextMenuFitPeaks
        //
        plotContextMenuFitPeaks.Name = "plotContextMenuFitPeaks";
        plotContextMenuFitPeaks.Size = new Size(286, 44);
        plotContextMenuFitPeaks.Text = "Fit peaks";
        //
        // plotContextMenuClear
        //
        plotContextMenuClear.Name = "plotContextMenuClear";
        plotContextMenuClear.Size = new Size(288, 38);
        plotContextMenuClear.Text = "Clear";
        //
        // plotSetContextMenu
        //
        plotSetContextMenu.ImageScalingSize = new Size(32, 32);
        plotSetContextMenu.Items.AddRange(new ToolStripItem[] { plotSetContextMenuHighlight, plotSetContextMenuDelete, spectraProcessingToolStripMenuItem, peaksProcessingToolStripMenuItem, plotSetContextMenuClear });
        plotSetContextMenu.Name = "plotSetContextMenu";
        plotSetContextMenu.Size = new Size(289, 194);
        //
        // plotSetContextMenuHighlight
        //
        plotSetContextMenuHighlight.Name = "plotSetContextMenuHighlight";
        plotSetContextMenuHighlight.Size = new Size(288, 38);
        plotSetContextMenuHighlight.Text = "Highlight";
        //
        // plotSetContextMenuDelete
        //
        plotSetContextMenuDelete.Name = "plotSetContextMenuDelete";
        plotSetContextMenuDelete.Size = new Size(288, 38);
        plotSetContextMenuDelete.Text = "Delete";
        //
        // spectraProcessingToolStripMenuItem
        //
        spectraProcessingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { plotSetContextMenuSmooth, plotSetContextMenuSubstractBaseline, plotSetContextMenuGetAverage });
        spectraProcessingToolStripMenuItem.Name = "spectraProcessingToolStripMenuItem";
        spectraProcessingToolStripMenuItem.Size = new Size(288, 38);
        spectraProcessingToolStripMenuItem.Text = "Spectra processing";
        //
        // plotSetContextMenuSmooth
        //
        plotSetContextMenuSmooth.Name = "plotSetContextMenuSmooth";
        plotSetContextMenuSmooth.Size = new Size(358, 44);
        plotSetContextMenuSmooth.Text = "Smooth";
        //
        // plotSetContextMenuSubstactBaseline
        //
        plotSetContextMenuSubstractBaseline.Name = "plotSetContextMenuSubstractBaseline";
        plotSetContextMenuSubstractBaseline.Size = new Size(358, 44);
        plotSetContextMenuSubstractBaseline.Text = "Substract baseline";
        //
        // plotSetContextMenuGetAverage
        //
        plotSetContextMenuGetAverage.Name = "plotSetContextMenuGetAverage";
        plotSetContextMenuGetAverage.Size = new Size(358, 44);
        plotSetContextMenuGetAverage.Text = "Get average spectra";
        //
        // peaksProcessingToolStripMenuItem
        //
        peaksProcessingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { plotSetContextMenuImportPeaks, plotSetContextMenuExportPeaks, plotSetContextMenuFitPeaks });
        peaksProcessingToolStripMenuItem.Name = "peaksProcessingToolStripMenuItem";
        peaksProcessingToolStripMenuItem.Size = new Size(288, 38);
        peaksProcessingToolStripMenuItem.Text = "Peaks processing";
        //
        // plotSetContextMenuImportPeaks
        //
        plotSetContextMenuImportPeaks.Name = "plotSetContextMenuImportPeaks";
        plotSetContextMenuImportPeaks.Size = new Size(286, 44);
        plotSetContextMenuImportPeaks.Text = "Import peaks";
        //
        // plotSetContextMenuExportPeaks
        //
        plotSetContextMenuExportPeaks.Name = "plotSetContextMenuExportPeaks";
        plotSetContextMenuExportPeaks.Size = new Size(286, 44);
        plotSetContextMenuExportPeaks.Text = "Export peaks";
        //
        // plotSetContextMenuFitPeaks
        //
        plotSetContextMenuFitPeaks.Name = "plotSetContextMenuFitPeaks";
        plotSetContextMenuFitPeaks.Size = new Size(286, 44);
        plotSetContextMenuFitPeaks.Text = "Fit peaks";
        //
        // plotSetContextMenuClear
        //
        plotSetContextMenuClear.Name = "plotSetContextMenuClear";
        plotSetContextMenuClear.Size = new Size(288, 38);
        plotSetContextMenuClear.Text = "Clear";
        //
        // numericUpDown1
        //
        numericUpDown1.Location = new Point(1126, 3);
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(143, 39);
        numericUpDown1.TabIndex = 2;
        //
        // mouseCoordinatesBox
        //
        mouseCoordinatesBox.Dock = DockStyle.Fill;
        mouseCoordinatesBox.Location = new Point(1275, 3);
        mouseCoordinatesBox.Name = "mouseCoordinatesBox";
        mouseCoordinatesBox.Size = new Size(220, 39);
        mouseCoordinatesBox.TabIndex = 3;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1894, 1009);
        Controls.Add(tableLayoutPanel1);
        MainMenuStrip = menuStrip1;
        Name = "MainForm";
        Text = "MainForm";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        tableLayoutPanel2.ResumeLayout(false);
        tableLayoutPanel3.ResumeLayout(false);
        tableLayoutPanel4.ResumeLayout(false);
        tableLayoutPanel4.PerformLayout();
        menuStrip2.ResumeLayout(false);
        menuStrip2.PerformLayout();
        tableLayoutPanel5.ResumeLayout(false);
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        dataContextMenu.ResumeLayout(false);
        dataSetContextMenu.ResumeLayout(false);
        plotContextMenu.ResumeLayout(false);
        plotSetContextMenu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    #endregion

    private ScottPlot.WinForms.FormsPlot plotView;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem readFolderToolStripMenuItem;
    private ToolStripMenuItem readFolderRecursiveToolStripMenuItem;
    private TableLayoutPanel tableLayoutPanel5;
    private TreeView dataStorageTreeView;
    private ContextMenuStrip dataContextMenu;
    private ToolStripMenuItem dataContextMenuSaveAsEsp;
    private ToolStripMenuItem dataContextMenuDelete;
    private ToolStripMenuItem dataContextMenuPlot;
    private ContextMenuStrip dataSetContextMenu;
    private ToolStripMenuItem dataSetContextMenuSaveAsEsp;
    private ToolStripMenuItem dataSetContextMenuSaveAsEspCurrent;
    private ToolStripMenuItem dataSetContextMenuSaveAsEspRecursive;
    private ToolStripMenuItem dataSetContextMenuDelete;
    private ToolStripMenuItem dataSetContextMenuPlot;
    private ToolStripMenuItem dataSetContextMenuAddToPlot;
    private ToolStripMenuItem dataContextMenuClear;
    private ToolStripMenuItem dataSetContextMenuClear;
    private ContextMenuStrip plotContextMenu;
    private ToolStripMenuItem plotContextMenuDelete;
    private ContextMenuStrip plotSetContextMenu;
    private ToolStripMenuItem plotSetContextMenuHighlight;
    private ToolStripMenuItem plotSetContextMenuDelete;
    private ToolStripMenuItem plotContextMenuClear;
    private ToolStripMenuItem plotSetContextMenuClear;
    private TreeView plotStorageTreeView;
    private MenuStrip menuStrip2;
    private ToolStripMenuItem resizeToolStripMenuItem;
    private ToolStripMenuItem addOrRemovePeaksToolStripMenuItem;
    private ToolStripMenuItem customPeaksToolStripMenuItem;
    private ToolStripMenuItem clearPeaksToolStripMenuItem;
    private ToolStripMenuItem peaksToolStripMenuItem;
    private ToolStripMenuItem exportPeaksToolStripMenuItem;
    private ToolStripMenuItem importPeaksToolStripMenuItem;
    private ToolStripMenuItem peaksProcessingToolStripMenuItem;
    private ToolStripMenuItem plotSetContextMenuFitPeaks;
    private ToolStripMenuItem plotSetContextMenuExportPeaks;
    private ToolStripMenuItem spectraProcessingToolStripMenuItem;
    private ToolStripMenuItem plotSetContextMenuSmooth;
    private ToolStripMenuItem plotSetContextMenuSubstractBaseline;
    private ToolStripMenuItem plotSetContextMenuGetAverage;
    private ToolStripMenuItem spectraProcessingToolStripMenuItem1;
    private ToolStripMenuItem plotContextMenuSubstractBaseline;
    private ToolStripMenuItem peaksProcessingToolStripMenuItem1;
    private ToolStripMenuItem plotContextMenuSmooth;
    private ToolStripMenuItem plotContextMenuImportPeaks;
    private ToolStripMenuItem plotContextMenuExportPeaks;
    private ToolStripMenuItem plotContextMenuFitPeaks;
    private ToolStripMenuItem plotSetContextMenuImportPeaks;
    private ToolStripMenuItem baselineModeToolStripMenuItem;
    private TextBox mouseCoordinatesBox;
    private NumericUpDown numericUpDown1;
}
