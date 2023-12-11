namespace View;

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
        rootSelectButton = new ToolStripMenuItem();
        rootRead = new ToolStripMenuItem();
        rootReadThisButton = new ToolStripMenuItem();
        rootReadAllButton = new ToolStripMenuItem();
        tableLayoutPanelMiddle = new TableLayoutPanel();
        tableLayoutPanelLeft = new TableLayoutPanel();
        tableLayoutPanelLeftUp = new TableLayoutPanel();
        rootNavigation = new MenuStrip();
        rootBackButton = new ToolStripMenuItem();
        rootTreeView = new TreeView();
        imageListForRootFolder = new ImageList(components);
        tableLayoutPanelLeftDown = new TableLayoutPanel();
        dataTreeView = new TreeView();
        dataNavigation = new MenuStrip();
        dataUpdateButton = new ToolStripMenuItem();
        tableLayoutPanelRight = new TableLayoutPanel();
        plotNavigation = new MenuStrip();
        plotClearButton = new ToolStripMenuItem();
        tableLayoutPanelRightUp = new TableLayoutPanel();
        mouseCoordinatesBox = new TextBox();
        plotView = new ScottPlot.FormsPlot();
        dataNodeContextMenu = new ContextMenuStrip(components);
        dataNodeContextSave = new ToolStripMenuItem();
        dataNodeContextSaveThisButton = new ToolStripMenuItem();
        dataNodeContextSaveAllButton = new ToolStripMenuItem();
        dataNodeContextPlot = new ToolStripMenuItem();
        dataNodeContextPlotThisButton = new ToolStripMenuItem();
        dataNodeContextPlotAllButton = new ToolStripMenuItem();
        dataNodeContextAddToPlot = new ToolStripMenuItem();
        dataNodeContextAddToPlotThisButton = new ToolStripMenuItem();
        dataNodeContextAddToPlotAllButton = new ToolStripMenuItem();
        dataNodeContextDeleteButton = new ToolStripMenuItem();
        dataContextMenu = new ContextMenuStrip(components);
        dataContextSaveAsESPButton = new ToolStripMenuItem();
        dataContextAddToPlotButton = new ToolStripMenuItem();
        dataContextDeleteButton = new ToolStripMenuItem();
        tableLayoutPanelMain.SuspendLayout();
        mainMenuStrip.SuspendLayout();
        tableLayoutPanelMiddle.SuspendLayout();
        tableLayoutPanelLeft.SuspendLayout();
        tableLayoutPanelLeftUp.SuspendLayout();
        rootNavigation.SuspendLayout();
        tableLayoutPanelLeftDown.SuspendLayout();
        dataNavigation.SuspendLayout();
        tableLayoutPanelRight.SuspendLayout();
        plotNavigation.SuspendLayout();
        tableLayoutPanelRightUp.SuspendLayout();
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
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { rootSelectButton, rootRead });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1894, 40);
        mainMenuStrip.TabIndex = 1;
        mainMenuStrip.Text = "menuStrip1";
        // 
        // rootSelectButton
        // 
        rootSelectButton.Name = "rootSelectButton";
        rootSelectButton.Size = new Size(168, 36);
        rootSelectButton.Text = "Select folder";
        // 
        // rootRead
        // 
        rootRead.DropDownItems.AddRange(new ToolStripItem[] { rootReadThisButton, rootReadAllButton });
        rootRead.Name = "rootRead";
        rootRead.Size = new Size(86, 36);
        rootRead.Text = "Read";
        // 
        // rootReadThisButton
        // 
        rootReadThisButton.Name = "rootReadThisButton";
        rootReadThisButton.Size = new Size(425, 44);
        rootReadThisButton.Text = "Only this folder";
        // 
        // rootReadAllButton
        // 
        rootReadAllButton.Name = "rootReadAllButton";
        rootReadAllButton.Size = new Size(425, 44);
        rootReadAllButton.Text = "This folder and subfolders";
        // 
        // tableLayoutPanelMiddle
        // 
        tableLayoutPanelMiddle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanelMiddle.ColumnCount = 2;
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanelLeft, 0, 0);
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanelRight, 1, 0);
        tableLayoutPanelMiddle.Dock = DockStyle.Fill;
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
        tableLayoutPanelLeft.Size = new Size(560, 917);
        tableLayoutPanelLeft.TabIndex = 2;
        // 
        // tableLayoutPanelLeftUp
        // 
        tableLayoutPanelLeftUp.ColumnCount = 1;
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        tableLayoutPanelLeftUp.Controls.Add(rootNavigation, 0, 0);
        tableLayoutPanelLeftUp.Controls.Add(rootTreeView, 0, 1);
        tableLayoutPanelLeftUp.Dock = DockStyle.Fill;
        tableLayoutPanelLeftUp.Location = new Point(3, 3);
        tableLayoutPanelLeftUp.Name = "tableLayoutPanelLeftUp";
        tableLayoutPanelLeftUp.RowCount = 2;
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftUp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftUp.Size = new Size(554, 452);
        tableLayoutPanelLeftUp.TabIndex = 1;
        // 
        // rootNavigation
        // 
        rootNavigation.Dock = DockStyle.Fill;
        rootNavigation.ImageScalingSize = new Size(32, 32);
        rootNavigation.Items.AddRange(new ToolStripItem[] { rootBackButton });
        rootNavigation.Location = new Point(0, 0);
        rootNavigation.Name = "rootNavigation";
        rootNavigation.Size = new Size(554, 40);
        rootNavigation.TabIndex = 0;
        rootNavigation.Text = "menuStrip1";
        // 
        // rootBackButton
        // 
        rootBackButton.Name = "rootBackButton";
        rootBackButton.Size = new Size(83, 36);
        rootBackButton.Text = "Back";
        // 
        // rootTreeView
        // 
        rootTreeView.Dock = DockStyle.Fill;
        rootTreeView.ImageIndex = 1;
        rootTreeView.ImageList = imageListForRootFolder;
        rootTreeView.ImeMode = ImeMode.NoControl;
        rootTreeView.Location = new Point(3, 43);
        rootTreeView.Name = "rootTreeView";
        rootTreeView.SelectedImageIndex = 2;
        rootTreeView.Size = new Size(548, 406);
        rootTreeView.TabIndex = 1;
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
        tableLayoutPanelLeftDown.Controls.Add(dataTreeView, 0, 1);
        tableLayoutPanelLeftDown.Controls.Add(dataNavigation, 0, 0);
        tableLayoutPanelLeftDown.Dock = DockStyle.Fill;
        tableLayoutPanelLeftDown.Location = new Point(3, 461);
        tableLayoutPanelLeftDown.Name = "tableLayoutPanelLeftDown";
        tableLayoutPanelLeftDown.RowCount = 2;
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelLeftDown.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelLeftDown.Size = new Size(554, 453);
        tableLayoutPanelLeftDown.TabIndex = 2;
        // 
        // dataTreeView
        // 
        dataTreeView.Dock = DockStyle.Fill;
        dataTreeView.Location = new Point(3, 43);
        dataTreeView.Name = "dataTreeView";
        dataTreeView.Size = new Size(548, 407);
        dataTreeView.TabIndex = 5;
        // 
        // dataNavigation
        // 
        dataNavigation.ImageScalingSize = new Size(32, 32);
        dataNavigation.Items.AddRange(new ToolStripItem[] { dataUpdateButton });
        dataNavigation.Location = new Point(0, 0);
        dataNavigation.Name = "dataNavigation";
        dataNavigation.Size = new Size(554, 40);
        dataNavigation.TabIndex = 6;
        dataNavigation.Text = "menuStrip1";
        // 
        // dataUpdateButton
        // 
        dataUpdateButton.Name = "dataUpdateButton";
        dataUpdateButton.Size = new Size(111, 36);
        dataUpdateButton.Text = "Update";
        // 
        // tableLayoutPanelRight
        // 
        tableLayoutPanelRight.ColumnCount = 1;
        tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelRight.Controls.Add(plotNavigation, 0, 0);
        tableLayoutPanelRight.Controls.Add(tableLayoutPanelRightUp, 0, 2);
        tableLayoutPanelRight.Controls.Add(plotView, 0, 1);
        tableLayoutPanelRight.Dock = DockStyle.Fill;
        tableLayoutPanelRight.Location = new Point(569, 3);
        tableLayoutPanelRight.Name = "tableLayoutPanelRight";
        tableLayoutPanelRight.RowCount = 3;
        tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tableLayoutPanelRight.Size = new Size(1316, 917);
        tableLayoutPanelRight.TabIndex = 3;
        // 
        // plotNavigation
        // 
        plotNavigation.Dock = DockStyle.Fill;
        plotNavigation.ImageScalingSize = new Size(32, 32);
        plotNavigation.Items.AddRange(new ToolStripItem[] { plotClearButton });
        plotNavigation.Location = new Point(0, 0);
        plotNavigation.Name = "plotNavigation";
        plotNavigation.Size = new Size(1316, 40);
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
        tableLayoutPanelRightUp.Size = new Size(1310, 34);
        tableLayoutPanelRightUp.TabIndex = 6;
        // 
        // mouseCoordinatesBox
        // 
        mouseCoordinatesBox.Dock = DockStyle.Fill;
        mouseCoordinatesBox.Location = new Point(3, 3);
        mouseCoordinatesBox.Name = "mouseCoordinatesBox";
        mouseCoordinatesBox.Size = new Size(1042, 39);
        mouseCoordinatesBox.TabIndex = 7;
        // 
        // plotView
        // 
        plotView.Dock = DockStyle.Fill;
        plotView.Location = new Point(7, 46);
        plotView.Margin = new Padding(7, 6, 7, 6);
        plotView.Name = "plotView";
        plotView.Size = new Size(1302, 825);
        plotView.TabIndex = 4;
        // 
        // dataNodeContextMenu
        // 
        dataNodeContextMenu.ImageScalingSize = new Size(32, 32);
        dataNodeContextMenu.Items.AddRange(new ToolStripItem[] { dataNodeContextSave, dataNodeContextPlot, dataNodeContextAddToPlot, dataNodeContextDeleteButton });
        dataNodeContextMenu.Name = "dataNodeContext1";
        dataNodeContextMenu.Size = new Size(284, 156);
        // 
        // dataNodeContextSave
        // 
        dataNodeContextSave.DropDownItems.AddRange(new ToolStripItem[] { dataNodeContextSaveThisButton, dataNodeContextSaveAllButton });
        dataNodeContextSave.Name = "dataNodeContextSave";
        dataNodeContextSave.Size = new Size(283, 38);
        dataNodeContextSave.Text = "Save series as .esp";
        // 
        // dataNodeContextSaveThisButton
        // 
        dataNodeContextSaveThisButton.Name = "dataNodeContextSaveThisButton";
        dataNodeContextSaveThisButton.Size = new Size(409, 44);
        dataNodeContextSaveThisButton.Text = "Only this series";
        // 
        // dataNodeContextSaveAllButton
        // 
        dataNodeContextSaveAllButton.Name = "dataNodeContextSaveAllButton";
        dataNodeContextSaveAllButton.Size = new Size(409, 44);
        dataNodeContextSaveAllButton.Text = "This series and subseries";
        // 
        // dataNodeContextPlot
        // 
        dataNodeContextPlot.DropDownItems.AddRange(new ToolStripItem[] { dataNodeContextPlotThisButton, dataNodeContextPlotAllButton });
        dataNodeContextPlot.Name = "dataNodeContextPlot";
        dataNodeContextPlot.Size = new Size(283, 38);
        dataNodeContextPlot.Text = "Plot";
        // 
        // dataNodeContextPlotThisButton
        // 
        dataNodeContextPlotThisButton.Name = "dataNodeContextPlotThisButton";
        dataNodeContextPlotThisButton.Size = new Size(409, 44);
        dataNodeContextPlotThisButton.Text = "Only this series";
        // 
        // dataNodeContextPlotAllButton
        // 
        dataNodeContextPlotAllButton.Name = "dataNodeContextPlotAllButton";
        dataNodeContextPlotAllButton.Size = new Size(409, 44);
        dataNodeContextPlotAllButton.Text = "This series and subseries";
        // 
        // dataNodeContextAddToPlot
        // 
        dataNodeContextAddToPlot.DropDownItems.AddRange(new ToolStripItem[] { dataNodeContextAddToPlotThisButton, dataNodeContextAddToPlotAllButton });
        dataNodeContextAddToPlot.Name = "dataNodeContextAddToPlot";
        dataNodeContextAddToPlot.Size = new Size(283, 38);
        dataNodeContextAddToPlot.Text = "Add to plot";
        // 
        // dataNodeContextAddToPlotThisButton
        // 
        dataNodeContextAddToPlotThisButton.Name = "dataNodeContextAddToPlotThisButton";
        dataNodeContextAddToPlotThisButton.Size = new Size(409, 44);
        dataNodeContextAddToPlotThisButton.Text = "Only this series";
        // 
        // dataNodeContextAddToPlotAllButton
        // 
        dataNodeContextAddToPlotAllButton.Name = "dataNodeContextAddToPlotAllButton";
        dataNodeContextAddToPlotAllButton.Size = new Size(409, 44);
        dataNodeContextAddToPlotAllButton.Text = "This series and subseries";
        // 
        // dataNodeContextDeleteButton
        // 
        dataNodeContextDeleteButton.Name = "dataNodeContextDeleteButton";
        dataNodeContextDeleteButton.Size = new Size(283, 38);
        dataNodeContextDeleteButton.Text = "Delete series";
        // 
        // dataContextMenu
        // 
        dataContextMenu.ImageScalingSize = new Size(32, 32);
        dataContextMenu.Items.AddRange(new ToolStripItem[] { dataContextSaveAsESPButton, dataContextAddToPlotButton, dataContextDeleteButton });
        dataContextMenu.Name = "contextMenuStrip2";
        dataContextMenu.Size = new Size(217, 118);
        // 
        // dataContextSaveAsESPButton
        // 
        dataContextSaveAsESPButton.Name = "dataContextSaveAsESPButton";
        dataContextSaveAsESPButton.Size = new Size(216, 38);
        dataContextSaveAsESPButton.Text = "Save as .esp";
        // 
        // dataContextAddToPlotButton
        // 
        dataContextAddToPlotButton.Name = "dataContextAddToPlotButton";
        dataContextAddToPlotButton.Size = new Size(216, 38);
        dataContextAddToPlotButton.Text = "Add to plot";
        // 
        // dataContextDeleteButton
        // 
        dataContextDeleteButton.Name = "dataContextDeleteButton";
        dataContextDeleteButton.Size = new Size(216, 38);
        dataContextDeleteButton.Text = "Delete";
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
        tableLayoutPanelRight.ResumeLayout(false);
        tableLayoutPanelRight.PerformLayout();
        plotNavigation.ResumeLayout(false);
        plotNavigation.PerformLayout();
        tableLayoutPanelRightUp.ResumeLayout(false);
        tableLayoutPanelRightUp.PerformLayout();
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
    private TreeView rootTreeView;
    private ToolStripMenuItem rootSelectButton;
    private TreeView dataTreeView;
    private TableLayoutPanel tableLayoutPanelLeftDown;
    private MenuStrip dataNavigation;
    private TableLayoutPanel tableLayoutPanelRight;
    private ScottPlot.FormsPlot plotView;
    private TableLayoutPanel tableLayoutPanelRightUp;
    private TextBox mouseCoordinatesBox;
    private MenuStrip plotNavigation;
    private ToolStripMenuItem rootRead;
    private ToolStripMenuItem rootReadThisButton;
    private ToolStripMenuItem rootReadAllButton;
    private ToolStripMenuItem rootBackButton;
    private ToolStripMenuItem dataUpdateButton;
    private ContextMenuStrip dataNodeContextMenu;
    private ContextMenuStrip dataContextMenu;
    private ToolStripMenuItem dataNodeContextSave;
    private ToolStripMenuItem dataNodeContextSaveThisButton;
    private ToolStripMenuItem dataNodeContextSaveAllButton;
    private ToolStripMenuItem dataNodeContextPlot;
    private ToolStripMenuItem dataNodeContextPlotThisButton;
    private ToolStripMenuItem dataNodeContextPlotAllButton;
    private ToolStripMenuItem dataNodeContextAddToPlot;
    private ToolStripMenuItem dataNodeContextAddToPlotThisButton;
    private ToolStripMenuItem dataNodeContextAddToPlotAllButton;
    private ToolStripMenuItem dataNodeContextDeleteButton;
    private ToolStripMenuItem plotClearButton;
    private ToolStripMenuItem dataContextSaveAsESPButton;
    private ToolStripMenuItem dataContextAddToPlotButton;
    private ToolStripMenuItem dataContextDeleteButton;
}
