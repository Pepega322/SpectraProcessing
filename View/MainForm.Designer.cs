namespace Model;

partial class MainForm
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        tableLayoutPanelMain = new TableLayoutPanel();
        mainMenuStrip = new MenuStrip();
        buttonRootSelect = new ToolStripMenuItem();
        rootRead = new ToolStripMenuItem();
        buttonRootReadThis = new ToolStripMenuItem();
        buttonRootReadAll = new ToolStripMenuItem();
        tableLayoutPanelMiddle = new TableLayoutPanel();
        tableLayoutPanelLeft = new TableLayoutPanel();
        tableLayoutPanelLeftUp = new TableLayoutPanel();
        rootNavigation = new MenuStrip();
        buttonRootBack = new ToolStripMenuItem();
        treeViewRoot = new TreeView();
        imageListForRootFolder = new ImageList(components);
        tableLayoutPanelLeftDown = new TableLayoutPanel();
        treeViewData = new TreeView();
        dataNavigation = new MenuStrip();
        buttonDataUpdate = new ToolStripMenuItem();
        tableLayoutPanelMiddleMiddle = new TableLayoutPanel();
        plotNavigation = new MenuStrip();
        plotClearButton = new ToolStripMenuItem();
        tableLayoutPanelRightUp = new TableLayoutPanel();
        mouseCoordinatesBox = new TextBox();
        plotView = new ScottPlot.FormsPlot();
        tableLayoutPanel1 = new TableLayoutPanel();
        menuStrip1 = new MenuStrip();
        treeView1 = new TreeView();
        dataNodeContextMenu = new ContextMenuStrip(components);
        dataNodeContextSave = new ToolStripMenuItem();
        buttonContextNodeSaveThis = new ToolStripMenuItem();
        buttonContextNodeSaveAll = new ToolStripMenuItem();
        buttonContextNodePlot = new ToolStripMenuItem();
        buttonContextNodeAddToPlot = new ToolStripMenuItem();
        buttonContextNodeDelete = new ToolStripMenuItem();
        dataContextMenu = new ContextMenuStrip(components);
        buttonContextDataSave = new ToolStripMenuItem();
        buttonContextDataPlot = new ToolStripMenuItem();
        buttonContextDataDelete = new ToolStripMenuItem();
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
        dataNodeContextMenu.SuspendLayout();
        dataContextMenu.SuspendLayout();
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
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { buttonRootSelect, rootRead });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1894, 40);
        mainMenuStrip.TabIndex = 1;
        mainMenuStrip.Text = "menuStrip1";
        // 
        // buttonRootSelect
        // 
        buttonRootSelect.Name = "buttonRootSelect";
        buttonRootSelect.Size = new Size(168, 36);
        buttonRootSelect.Text = "Select folder";
        // 
        // rootRead
        // 
        rootRead.DropDownItems.AddRange(new ToolStripItem[] { buttonRootReadThis, buttonRootReadAll });
        rootRead.Name = "rootRead";
        rootRead.Size = new Size(86, 36);
        rootRead.Text = "Read";
        // 
        // buttonRootReadThis
        // 
        buttonRootReadThis.Name = "buttonRootReadThis";
        buttonRootReadThis.Size = new Size(425, 44);
        buttonRootReadThis.Text = "Only this folder";
        // 
        // buttonRootReadAll
        // 
        buttonRootReadAll.Name = "buttonRootReadAll";
        buttonRootReadAll.Size = new Size(425, 44);
        buttonRootReadAll.Text = "This folder and subfolders";
        // 
        // tableLayoutPanelMiddle
        // 
        tableLayoutPanelMiddle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanelMiddle.ColumnCount = 3;
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
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
        tableLayoutPanelLeft.Size = new Size(466, 917);
        tableLayoutPanelLeft.TabIndex = 2;
        // 
        // tableLayoutPanelLeftUp
        // 
        tableLayoutPanelLeftUp.ColumnCount = 1;
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanelLeftUp.Controls.Add(rootNavigation, 0, 0);
        tableLayoutPanelLeftUp.Controls.Add(treeViewRoot, 0, 1);
        tableLayoutPanelLeftUp.Dock = DockStyle.Fill;
        tableLayoutPanelLeftUp.Location = new Point(3, 3);
        tableLayoutPanelLeftUp.Name = "tableLayoutPanelLeftUp";
        tableLayoutPanelLeftUp.RowCount = 2;
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.Size = new Size(460, 452);
        tableLayoutPanelLeftUp.TabIndex = 1;
        // 
        // rootNavigation
        // 
        rootNavigation.Dock = DockStyle.Fill;
        rootNavigation.ImageScalingSize = new Size(32, 32);
        rootNavigation.Items.AddRange(new ToolStripItem[] { buttonRootBack });
        rootNavigation.Location = new Point(0, 0);
        rootNavigation.Name = "rootNavigation";
        rootNavigation.Size = new Size(460, 40);
        rootNavigation.TabIndex = 0;
        rootNavigation.Text = "menuStrip1";
        // 
        // buttonRootBack
        // 
        buttonRootBack.Name = "buttonRootBack";
        buttonRootBack.Size = new Size(83, 36);
        buttonRootBack.Text = "Back";
        // 
        // treeViewRoot
        // 
        treeViewRoot.Dock = DockStyle.Fill;
        treeViewRoot.ImageIndex = 1;
        treeViewRoot.ImageList = imageListForRootFolder;
        treeViewRoot.ImeMode = ImeMode.NoControl;
        treeViewRoot.Location = new Point(3, 43);
        treeViewRoot.Name = "treeViewRoot";
        treeViewRoot.SelectedImageIndex = 2;
        treeViewRoot.Size = new Size(454, 406);
        treeViewRoot.TabIndex = 1;
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
        tableLayoutPanelLeftDown.Controls.Add(treeViewData, 0, 1);
        tableLayoutPanelLeftDown.Controls.Add(dataNavigation, 0, 0);
        tableLayoutPanelLeftDown.Dock = DockStyle.Fill;
        tableLayoutPanelLeftDown.Location = new Point(3, 461);
        tableLayoutPanelLeftDown.Name = "tableLayoutPanelLeftDown";
        tableLayoutPanelLeftDown.RowCount = 2;
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftDown.Size = new Size(460, 453);
        tableLayoutPanelLeftDown.TabIndex = 2;
        // 
        // treeViewData
        // 
        treeViewData.Dock = DockStyle.Fill;
        treeViewData.Location = new Point(3, 43);
        treeViewData.Name = "treeViewData";
        treeViewData.Size = new Size(454, 407);
        treeViewData.TabIndex = 5;
        // 
        // dataNavigation
        // 
        dataNavigation.ImageScalingSize = new Size(32, 32);
        dataNavigation.Items.AddRange(new ToolStripItem[] { buttonDataUpdate });
        dataNavigation.Location = new Point(0, 0);
        dataNavigation.Name = "dataNavigation";
        dataNavigation.Size = new Size(460, 40);
        dataNavigation.TabIndex = 6;
        dataNavigation.Text = "menuStrip1";
        // 
        // buttonDataUpdate
        // 
        buttonDataUpdate.Name = "buttonDataUpdate";
        buttonDataUpdate.Size = new Size(111, 36);
        buttonDataUpdate.Text = "Update";
        // 
        // tableLayoutPanelMiddleMiddle
        // 
        tableLayoutPanelMiddleMiddle.ColumnCount = 1;
        tableLayoutPanelMiddleMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddleMiddle.Controls.Add(plotNavigation, 0, 0);
        tableLayoutPanelMiddleMiddle.Controls.Add(tableLayoutPanelRightUp, 0, 2);
        tableLayoutPanelMiddleMiddle.Controls.Add(plotView, 0, 1);
        tableLayoutPanelMiddleMiddle.Dock = DockStyle.Fill;
        tableLayoutPanelMiddleMiddle.Location = new Point(475, 3);
        tableLayoutPanelMiddleMiddle.Name = "tableLayoutPanelMiddleMiddle";
        tableLayoutPanelMiddleMiddle.RowCount = 3;
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelMiddleMiddle.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanelMiddleMiddle.Size = new Size(938, 917);
        tableLayoutPanelMiddleMiddle.TabIndex = 3;
        // 
        // plotNavigation
        // 
        plotNavigation.Dock = DockStyle.Fill;
        plotNavigation.ImageScalingSize = new Size(32, 32);
        plotNavigation.Items.AddRange(new ToolStripItem[] { plotClearButton });
        plotNavigation.Location = new Point(0, 0);
        plotNavigation.Name = "plotNavigation";
        plotNavigation.Size = new Size(938, 40);
        plotNavigation.TabIndex = 8;
        plotNavigation.Text = "menuStrip1";
        // 
        // plotClearButton
        // 
        plotClearButton.Name = "plotClearButton";
        plotClearButton.Size = new Size(88, 36);
        plotClearButton.Text = "Clear";
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
        tableLayoutPanelRightUp.Size = new Size(932, 34);
        tableLayoutPanelRightUp.TabIndex = 6;
        // 
        // mouseCoordinatesBox
        // 
        mouseCoordinatesBox.Dock = DockStyle.Fill;
        mouseCoordinatesBox.Location = new Point(3, 3);
        mouseCoordinatesBox.Name = "mouseCoordinatesBox";
        mouseCoordinatesBox.Size = new Size(739, 39);
        mouseCoordinatesBox.TabIndex = 7;
        // 
        // plotView
        // 
        plotView.Dock = DockStyle.Fill;
        plotView.Location = new Point(7, 46);
        plotView.Margin = new Padding(7, 6, 7, 6);
        plotView.Name = "plotView";
        plotView.Size = new Size(924, 825);
        plotView.TabIndex = 4;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Controls.Add(menuStrip1, 0, 0);
        tableLayoutPanel1.Controls.Add(treeView1, 0, 1);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(1419, 3);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(466, 917);
        tableLayoutPanel1.TabIndex = 4;
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(32, 32);
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(466, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // treeView1
        // 
        treeView1.Dock = DockStyle.Fill;
        treeView1.Location = new Point(3, 43);
        treeView1.Name = "treeView1";
        treeView1.Size = new Size(460, 871);
        treeView1.TabIndex = 1;
        // 
        // dataNodeContextMenu
        // 
        dataNodeContextMenu.ImageScalingSize = new Size(32, 32);
        dataNodeContextMenu.Items.AddRange(new ToolStripItem[] { dataNodeContextSave, buttonContextNodePlot, buttonContextNodeAddToPlot, buttonContextNodeDelete });
        dataNodeContextMenu.Name = "dataNodeContext1";
        dataNodeContextMenu.Size = new Size(284, 156);
        // 
        // dataNodeContextSave
        // 
        dataNodeContextSave.DropDownItems.AddRange(new ToolStripItem[] { buttonContextNodeSaveThis, buttonContextNodeSaveAll });
        dataNodeContextSave.Name = "dataNodeContextSave";
        dataNodeContextSave.Size = new Size(283, 38);
        dataNodeContextSave.Text = "Save series as .esp";
        // 
        // buttonContextNodeSaveThis
        // 
        buttonContextNodeSaveThis.Name = "buttonContextNodeSaveThis";
        buttonContextNodeSaveThis.Size = new Size(409, 44);
        buttonContextNodeSaveThis.Text = "Only this series";
        // 
        // buttonContextNodeSaveAll
        // 
        buttonContextNodeSaveAll.Name = "buttonContextNodeSaveAll";
        buttonContextNodeSaveAll.Size = new Size(409, 44);
        buttonContextNodeSaveAll.Text = "This series and subseries";
        // 
        // buttonContextNodePlot
        // 
        buttonContextNodePlot.Name = "buttonContextNodePlot";
        buttonContextNodePlot.Size = new Size(283, 38);
        buttonContextNodePlot.Text = "Plot series";
        // 
        // buttonContextNodeAddToPlot
        // 
        buttonContextNodeAddToPlot.Name = "buttonContextNodeAddToPlot";
        buttonContextNodeAddToPlot.Size = new Size(283, 38);
        buttonContextNodeAddToPlot.Text = "Add series to plot";
        // 
        // buttonContextNodeDelete
        // 
        buttonContextNodeDelete.Name = "buttonContextNodeDelete";
        buttonContextNodeDelete.Size = new Size(283, 38);
        buttonContextNodeDelete.Text = "Delete series";
        // 
        // dataContextMenu
        // 
        dataContextMenu.ImageScalingSize = new Size(32, 32);
        dataContextMenu.Items.AddRange(new ToolStripItem[] { buttonContextDataSave, buttonContextDataPlot, buttonContextDataDelete });
        dataContextMenu.Name = "contextMenuStrip2";
        dataContextMenu.Size = new Size(217, 118);
        // 
        // buttonContextDataSave
        // 
        buttonContextDataSave.Name = "buttonContextDataSave";
        buttonContextDataSave.Size = new Size(216, 38);
        buttonContextDataSave.Text = "Save as .esp";
        // 
        // buttonContextDataPlot
        // 
        buttonContextDataPlot.Name = "buttonContextDataPlot";
        buttonContextDataPlot.Size = new Size(216, 38);
        buttonContextDataPlot.Text = "Plot";
        // 
        // buttonContextDataDelete
        // 
        buttonContextDataDelete.Name = "buttonContextDataDelete";
        buttonContextDataDelete.Size = new Size(216, 38);
        buttonContextDataDelete.Text = "Delete";
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
        dataNodeContextMenu.ResumeLayout(false);
        dataContextMenu.ResumeLayout(false);
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
    private TreeView treeViewRoot;
    private ToolStripMenuItem buttonRootSelect;
    private TreeView treeViewData;
    private TableLayoutPanel tableLayoutPanelLeftDown;
    private MenuStrip dataNavigation;
    private TableLayoutPanel tableLayoutPanelMiddleMiddle;
    private ScottPlot.FormsPlot plotView;
    private TableLayoutPanel tableLayoutPanelRightUp;
    private TextBox mouseCoordinatesBox;
    private MenuStrip plotNavigation;
    private ToolStripMenuItem rootRead;
    private ToolStripMenuItem buttonRootReadThis;
    private ToolStripMenuItem buttonRootReadAll;
    private ToolStripMenuItem buttonRootBack;
    private ToolStripMenuItem buttonDataUpdate;
    private ContextMenuStrip dataNodeContextMenu;
    private ContextMenuStrip dataContextMenu;
    private ToolStripMenuItem dataNodeContextSave;
    private ToolStripMenuItem buttonContextNodeSaveThis;
    private ToolStripMenuItem buttonContextNodeSaveAll;
    private ToolStripMenuItem buttonContextNodePlot;
    private ToolStripMenuItem buttonContextNodeAddToPlot;
    private ToolStripMenuItem buttonContextNodeDelete;
    private ToolStripMenuItem plotClearButton;
    private ToolStripMenuItem buttonContextDataSave;
    private ToolStripMenuItem buttonContextDataPlot;
    private ToolStripMenuItem buttonContextDataDelete;
    private TableLayoutPanel tableLayoutPanel1;
    private MenuStrip menuStrip1;
    private TreeView treeView1;
}
