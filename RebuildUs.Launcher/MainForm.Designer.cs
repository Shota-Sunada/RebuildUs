namespace RebuildUs.Launcher;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer Components = null;

    private System.Windows.Forms.Label LblStatus;
    private System.Windows.Forms.Label LblVersion;
    private System.Windows.Forms.Label LblUrl;
    private System.Windows.Forms.ComboBox CmbVersions;
    private System.Windows.Forms.Button BtnAction;
    private System.Windows.Forms.Button BtnUninstall;
    private System.Windows.Forms.NotifyIcon NotifyIcon;

    private System.Windows.Forms.ContextMenuStrip TrayMenu;
    private System.Windows.Forms.ToolStripMenuItem MenuShow;
    private System.Windows.Forms.ToolStripMenuItem MenuExit;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (Components != null))
        {
            Components.Dispose();
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
        LblStatus = new Label();
        LblVersion = new Label();
        LblUrl = new Label();
        CmbVersions = new ComboBox();
        BtnAction = new Button();
        BtnUninstall = new Button();
        NotifyIcon = new NotifyIcon(components);
        TrayMenu = new ContextMenuStrip(components);
        MenuShow = new ToolStripMenuItem();
        MenuExit = new ToolStripMenuItem();
        OpenDirectoryButton = new Button();
        TrayMenu.SuspendLayout();
        SuspendLayout();
        // 
        // LblStatus
        // 
        LblStatus.AutoSize = true;
        LblStatus.Location = new Point(12, 9);
        LblStatus.Name = "LblStatus";
        LblStatus.Size = new Size(39, 15);
        LblStatus.TabIndex = 0;
        LblStatus.Text = "Status";
        // 
        // LblVersion
        // 
        LblVersion.AutoSize = true;
        LblVersion.Location = new Point(12, 45);
        LblVersion.Name = "LblVersion";
        LblVersion.Size = new Size(56, 15);
        LblVersion.TabIndex = 4;
        LblVersion.Text = "Version: -";
        // 
        // LblUrl
        // 
        LblUrl.AutoSize = true;
        LblUrl.Location = new Point(12, 70);
        LblUrl.Name = "LblUrl";
        LblUrl.Size = new Size(48, 15);
        LblUrl.TabIndex = 2;
        LblUrl.Text = "Version:";
        // 
        // CmbVersions
        // 
        CmbVersions.DropDownStyle = ComboBoxStyle.DropDownList;
        CmbVersions.Location = new Point(12, 88);
        CmbVersions.Name = "CmbVersions";
        CmbVersions.Size = new Size(360, 23);
        CmbVersions.TabIndex = 3;
        CmbVersions.SelectedIndexChanged += CmbVersions_SelectedIndexChanged;
        // 
        // BtnAction
        // 
        BtnAction.Location = new Point(12, 117);
        BtnAction.Name = "BtnAction";
        BtnAction.Size = new Size(120, 40);
        BtnAction.TabIndex = 1;
        BtnAction.Text = "Action";
        BtnAction.UseVisualStyleBackColor = true;
        BtnAction.Click += BtnAction_Click;
        // 
        // BtnUninstall
        // 
        BtnUninstall.Location = new Point(140, 117);
        BtnUninstall.Name = "BtnUninstall";
        BtnUninstall.Size = new Size(120, 40);
        BtnUninstall.TabIndex = 5;
        BtnUninstall.Text = "Uninstall";
        BtnUninstall.UseVisualStyleBackColor = true;
        BtnUninstall.Visible = false;
        BtnUninstall.Click += BtnUninstall_Click;
        // 
        // NotifyIcon
        // 
        NotifyIcon.ContextMenuStrip = TrayMenu;
        NotifyIcon.Text = "RebuildUs Launcher";
        NotifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        // 
        // TrayMenu
        // 
        TrayMenu.Items.AddRange(new ToolStripItem[] { MenuShow, MenuExit });
        TrayMenu.Name = "trayMenu";
        TrayMenu.Size = new Size(104, 48);
        // 
        // MenuShow
        // 
        MenuShow.Name = "MenuShow";
        MenuShow.Size = new Size(103, 22);
        MenuShow.Text = "Show";
        MenuShow.Click += MenuShow_Click;
        // 
        // MenuExit
        // 
        MenuExit.Name = "MenuExit";
        MenuExit.Size = new Size(103, 22);
        MenuExit.Text = "Exit";
        MenuExit.Click += MenuExit_Click;
        // 
        // OpenDirectoryButton
        // 
        OpenDirectoryButton.Location = new Point(138, 117);
        OpenDirectoryButton.Name = "OpenDirectoryButton";
        OpenDirectoryButton.Size = new Size(120, 40);
        OpenDirectoryButton.TabIndex = 5;
        OpenDirectoryButton.Text = "Mod導入先を開く";
        OpenDirectoryButton.UseVisualStyleBackColor = true;
        OpenDirectoryButton.Click += OpenDirectoryButton_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(384, 169);
        Controls.Add(OpenDirectoryButton);
        Controls.Add(BtnAction);
        Controls.Add(LblVersion);
        Controls.Add(CmbVersions);
        Controls.Add(LblUrl);
        Controls.Add(LblStatus);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "RebuildUs Launcher";
        TrayMenu.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion

    private System.ComponentModel.IContainer components;
    private Button OpenDirectoryButton;
}
