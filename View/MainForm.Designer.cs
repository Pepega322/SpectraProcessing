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
        readMainMenu = new ToolStripMenuItem();
        readRootAsOneButton = new ToolStripMenuItem();
        readEachFolderAsSeriesToolStripMenuItem = new ToolStripMenuItem();
        tableLayoutPanelMiddle = new TableLayoutPanel();
        tableLayoutPanelLeft = new TableLayoutPanel();
        tableLayoutPanelLeftUp = new TableLayoutPanel();
        rootNavigation = new MenuStrip();
        rootBackButton = new ToolStripMenuItem();
        rootTreeView = new TreeView();
        imageListForRootFolder = new ImageList(components);
        dataTreeView = new TreeView();
        plotView = new ScottPlot.FormsPlot();
        miniToolStrip = new MenuStrip();
        tableLayoutPanelMain.SuspendLayout();
        mainMenuStrip.SuspendLayout();
        tableLayoutPanelMiddle.SuspendLayout();
        tableLayoutPanelLeft.SuspendLayout();
        tableLayoutPanelLeftUp.SuspendLayout();
        rootNavigation.SuspendLayout();
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
        tableLayoutPanelMain.Size = new Size(1894, 1009);
        tableLayoutPanelMain.TabIndex = 0;
        // 
        // mainMenuStrip
        // 
        mainMenuStrip.Dock = DockStyle.Fill;
        mainMenuStrip.ImageScalingSize = new Size(32, 32);
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { rootSelectButton, readMainMenu });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1894, 40);
        mainMenuStrip.TabIndex = 1;
        mainMenuStrip.Text = "menuStrip1";
        // 
        // rootSelectButton
        // 
        rootSelectButton.Name = "rootSelectButton";
        rootSelectButton.ShortcutKeys = Keys.Control | Keys.Tab;
        rootSelectButton.Size = new Size(168, 36);
        rootSelectButton.Text = "Select folder";
        // 
        // readMainMenu
        // 
        readMainMenu.DropDownItems.AddRange(new ToolStripItem[] { readRootAsOneButton, readEachFolderAsSeriesToolStripMenuItem });
        readMainMenu.Name = "readMainMenu";
        readMainMenu.Size = new Size(86, 36);
        readMainMenu.Text = "Read";
        // 
        // readRootAsOneButton
        // 
        readRootAsOneButton.Name = "readRootAsOneButton";
        readRootAsOneButton.ShortcutKeys = Keys.Control | Keys.R;
        readRootAsOneButton.Size = new Size(568, 44);
        readRootAsOneButton.Text = "Read folder as one series";
        // 
        // readEachFolderAsSeriesToolStripMenuItem
        // 
        readEachFolderAsSeriesToolStripMenuItem.Name = "readEachFolderAsSeriesToolStripMenuItem";
        readEachFolderAsSeriesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
        readEachFolderAsSeriesToolStripMenuItem.Size = new Size(568, 44);
        readEachFolderAsSeriesToolStripMenuItem.Text = "Read each folder as series";
        // 
        // tableLayoutPanelMiddle
        // 
        tableLayoutPanelMiddle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanelMiddle.ColumnCount = 2;
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        tableLayoutPanelMiddle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        tableLayoutPanelMiddle.Controls.Add(tableLayoutPanelLeft, 0, 0);
        tableLayoutPanelMiddle.Controls.Add(plotView, 1, 0);
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
        tableLayoutPanelLeft.Controls.Add(dataTreeView, 0, 1);
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
        // dataTreeView
        // 
        dataTreeView.Dock = DockStyle.Fill;
        dataTreeView.Location = new Point(3, 461);
        dataTreeView.Name = "dataTreeView";
        dataTreeView.Size = new Size(554, 453);
        dataTreeView.TabIndex = 4;
        // 
        // plotView
        // 
        plotView.Dock = DockStyle.Fill;
        plotView.Location = new Point(573, 6);
        plotView.Margin = new Padding(7, 6, 7, 6);
        plotView.Name = "plotView";
        plotView.Size = new Size(1308, 911);
        plotView.TabIndex = 3;
        // 
        // miniToolStrip
        // 
        miniToolStrip.AccessibleName = "New item selection";
        miniToolStrip.AccessibleRole = AccessibleRole.ComboBox;
        miniToolStrip.AutoSize = false;
        miniToolStrip.Dock = DockStyle.None;
        miniToolStrip.ImageScalingSize = new Size(32, 32);
        miniToolStrip.Location = new Point(89, 1);
        miniToolStrip.Name = "miniToolStrip";
        miniToolStrip.Size = new Size(364, 40);
        miniToolStrip.TabIndex = 0;
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
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanelMain;
    private MenuStrip mainMenuStrip;
    private ImageList imageListForRootFolder;
    private ToolStripMenuItem readMainMenu;
    private ToolStripMenuItem readRootAsOneButton;
    private MenuStrip miniToolStrip;
    private TableLayoutPanel tableLayoutPanelMiddle;
    private TableLayoutPanel tableLayoutPanelLeft;
    private TableLayoutPanel tableLayoutPanelLeftUp;
    private MenuStrip rootNavigation;
    private ToolStripMenuItem rootBackButton;
    private TreeView rootTreeView;
    private TreeView dataTreeView;
    private ScottPlot.FormsPlot plotView;
    private ToolStripMenuItem readEachFolderAsSeriesToolStripMenuItem;
    private ToolStripMenuItem rootSelectButton;
}
