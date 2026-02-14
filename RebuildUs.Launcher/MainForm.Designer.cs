namespace RebuildUs.Launcher;

sealed partial class MainForm
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        Components = new System.ComponentModel.Container();
        LblStatus = new System.Windows.Forms.Label();
        LblVersion = new System.Windows.Forms.Label();
        LblUrl = new System.Windows.Forms.Label();
        CmbVersions = new System.Windows.Forms.ComboBox();
        BtnAction = new System.Windows.Forms.Button();
        BtnUninstall = new System.Windows.Forms.Button();
        NotifyIcon = new System.Windows.Forms.NotifyIcon(Components);
        TrayMenu = new System.Windows.Forms.ContextMenuStrip(Components);
        MenuShow = new System.Windows.Forms.ToolStripMenuItem();
        MenuExit = new System.Windows.Forms.ToolStripMenuItem();
        TrayMenu.SuspendLayout();
        SuspendLayout();
        // 
        // LblStatus
        // 
        LblStatus.AutoSize = true;
        LblStatus.Location = new System.Drawing.Point(12, 9);
        LblStatus.Name = "LblStatus";
        LblStatus.Size = new System.Drawing.Size(39, 15);
        LblStatus.TabIndex = 0;
        LblStatus.Text = "Status";
        // 
        // LblVersion
        // 
        LblVersion.AutoSize = true;
        LblVersion.Location = new System.Drawing.Point(12, 45);
        LblVersion.Name = "LblVersion";
        LblVersion.Size = new System.Drawing.Size(56, 15);
        LblVersion.TabIndex = 4;
        LblVersion.Text = "Version: -";
        // 
        // LblUrl
        // 
        LblUrl.AutoSize = true;
        LblUrl.Location = new System.Drawing.Point(12, 70);
        LblUrl.Name = "LblUrl";
        LblUrl.Size = new System.Drawing.Size(48, 15);
        LblUrl.TabIndex = 2;
        LblUrl.Text = "Version:";
        // 
        // CmbVersions
        // 
        CmbVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        CmbVersions.Location = new System.Drawing.Point(12, 88);
        CmbVersions.Name = "CmbVersions";
        CmbVersions.Size = new System.Drawing.Size(360, 23);
        CmbVersions.TabIndex = 3;
        CmbVersions.SelectedIndexChanged += CmbVersions_SelectedIndexChanged;
        // 
        // BtnAction
        // 
        BtnAction.Location = new System.Drawing.Point(12, 117);
        BtnAction.Name = "BtnAction";
        BtnAction.Size = new System.Drawing.Size(120, 40);
        BtnAction.TabIndex = 1;
        BtnAction.Text = "Action";
        BtnAction.UseVisualStyleBackColor = true;
        BtnAction.Click += BtnAction_Click;
        // 
        // BtnUninstall
        // 
        BtnUninstall.Location = new System.Drawing.Point(140, 117);
        BtnUninstall.Name = "BtnUninstall";
        BtnUninstall.Size = new System.Drawing.Size(120, 40);
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
        TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuShow, MenuExit });
        TrayMenu.Name = "trayMenu";
        TrayMenu.Size = new System.Drawing.Size(104, 48);
        // 
        // MenuShow
        // 
        MenuShow.Name = "MenuShow";
        MenuShow.Size = new System.Drawing.Size(103, 22);
        MenuShow.Text = "Show";
        MenuShow.Click += MenuShow_Click;
        // 
        // MenuExit
        // 
        MenuExit.Name = "MenuExit";
        MenuExit.Size = new System.Drawing.Size(103, 22);
        MenuExit.Text = "Exit";
        MenuExit.Click += MenuExit_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(384, 169);
        Controls.Add(BtnAction);
        Controls.Add(LblVersion);
        Controls.Add(CmbVersions);
        Controls.Add(LblUrl);
        Controls.Add(LblStatus);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "RebuildUs Launcher";
        TrayMenu.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
