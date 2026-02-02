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
        this.Components = new System.ComponentModel.Container();
        this.LblStatus = new System.Windows.Forms.Label();
        this.LblVersion = new System.Windows.Forms.Label();
        this.LblUrl = new System.Windows.Forms.Label();
        this.CmbVersions = new System.Windows.Forms.ComboBox();
        this.BtnAction = new System.Windows.Forms.Button();
        this.BtnUninstall = new System.Windows.Forms.Button();
        this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.Components);
        this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.Components);
        this.MenuShow = new System.Windows.Forms.ToolStripMenuItem();
        this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
        this.TrayMenu.SuspendLayout();
        this.SuspendLayout();
        //
        // lblStatus
        //
        this.LblStatus.AutoSize = true;
        this.LblStatus.Location = new System.Drawing.Point(12, 9);
        this.LblStatus.Name = "lblStatus";
        this.LblStatus.Size = new System.Drawing.Size(39, 15);
        this.LblStatus.TabIndex = 0;
        this.LblStatus.Text = "Status";
        //
        // lblVersion
        //
        this.LblVersion.AutoSize = true;
        this.LblVersion.Location = new System.Drawing.Point(12, 45);
        this.LblVersion.Name = "lblVersion";
        this.LblVersion.Size = new System.Drawing.Size(51, 15);
        this.LblVersion.TabIndex = 4;
        this.LblVersion.Text = "Version: -";
        //
        // lblUrl
        //
        this.LblUrl.AutoSize = true;
        this.LblUrl.Location = new System.Drawing.Point(12, 70);
        this.LblUrl.Name = "lblUrl";
        this.LblUrl.Size = new System.Drawing.Size(48, 15);
        this.LblUrl.TabIndex = 2;
        this.LblUrl.Text = "Version:";
        //
        // cmbVersions
        //
        this.CmbVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.CmbVersions.Location = new System.Drawing.Point(12, 88);
        this.CmbVersions.Name = "cmbVersions";
        this.CmbVersions.Size = new System.Drawing.Size(360, 23);
        this.CmbVersions.TabIndex = 3;
        this.CmbVersions.SelectedIndexChanged += new System.EventHandler(this.CmbVersions_SelectedIndexChanged);
        //
        // btnAction
        //
        this.BtnAction.Location = new System.Drawing.Point(12, 117);
        this.BtnAction.Name = "btnAction";
        this.BtnAction.Size = new System.Drawing.Size(120, 40);
        this.BtnAction.TabIndex = 1;
        this.BtnAction.Text = "Action";
        this.BtnAction.UseVisualStyleBackColor = true;
        this.BtnAction.Click += new System.EventHandler(this.BtnAction_Click);
        //        // btnUninstall
        //
        this.BtnUninstall.Location = new System.Drawing.Point(140, 117);
        this.BtnUninstall.Name = "btnUninstall";
        this.BtnUninstall.Size = new System.Drawing.Size(120, 40);
        this.BtnUninstall.TabIndex = 5;
        this.BtnUninstall.Text = "Uninstall";
        this.BtnUninstall.UseVisualStyleBackColor = true;
        this.BtnUninstall.Visible = false;
        this.BtnUninstall.Click += new System.EventHandler(this.BtnUninstall_Click);
        //         // notifyIcon
        //
        this.NotifyIcon.ContextMenuStrip = this.TrayMenu;
        this.NotifyIcon.Text = "RebuildUs Launcher";
        this.NotifyIcon.Visible = false;
        this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
        //
        // trayMenu
        //
        this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.MenuShow,
        this.MenuExit});
        this.TrayMenu.Name = "trayMenu";
        this.TrayMenu.Size = new System.Drawing.Size(104, 48);
        //
        // menuShow
        //
        this.MenuShow.Name = "menuShow";
        this.MenuShow.Size = new System.Drawing.Size(103, 22);
        this.MenuShow.Text = "Show";
        this.MenuShow.Click += new System.EventHandler(this.MenuShow_Click);
        //
        // menuExit
        //
        this.MenuExit.Name = "menuExit";
        this.MenuExit.Size = new System.Drawing.Size(103, 22);
        this.MenuExit.Text = "Exit";
        this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
        //
        // MainForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(384, 169);
        this.Controls.Add(this.BtnAction);
        this.Controls.Add(this.LblVersion);
        this.Controls.Add(this.CmbVersions);
        this.Controls.Add(this.LblUrl);
        this.Controls.Add(this.LblStatus);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "RebuildUs Launcher";
        this.TrayMenu.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion
}
