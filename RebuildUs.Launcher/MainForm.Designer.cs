namespace RebuildUs.Launcher;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblUrl;
    private System.Windows.Forms.TextBox txtUrl;
    private System.Windows.Forms.Button btnAction;
    private System.Windows.Forms.Button btnUninstall;
    private System.Windows.Forms.NotifyIcon notifyIcon;

    private System.Windows.Forms.ContextMenuStrip trayMenu;
    private System.Windows.Forms.ToolStripMenuItem menuShow;
    private System.Windows.Forms.ToolStripMenuItem menuExit;

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
        this.components = new System.ComponentModel.Container();
        this.lblStatus = new System.Windows.Forms.Label();
        this.lblVersion = new System.Windows.Forms.Label();
        this.lblUrl = new System.Windows.Forms.Label();
        this.txtUrl = new System.Windows.Forms.TextBox();
        this.btnAction = new System.Windows.Forms.Button();
        this.btnUninstall = new System.Windows.Forms.Button();
        this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
        this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.menuShow = new System.Windows.Forms.ToolStripMenuItem();
        this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
        this.trayMenu.SuspendLayout();
        this.SuspendLayout();
        //
        // lblStatus
        //
        this.lblStatus.AutoSize = true;
        this.lblStatus.Location = new System.Drawing.Point(12, 9);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(39, 15);
        this.lblStatus.TabIndex = 0;
        this.lblStatus.Text = "Status";
        //
        // lblVersion
        //
        this.lblVersion.AutoSize = true;
        this.lblVersion.Location = new System.Drawing.Point(12, 45);
        this.lblVersion.Name = "lblVersion";
        this.lblVersion.Size = new System.Drawing.Size(51, 15);
        this.lblVersion.TabIndex = 4;
        this.lblVersion.Text = "Version: -";
        //
        // lblUrl
        //
        this.lblUrl.AutoSize = true;
        this.lblUrl.Location = new System.Drawing.Point(12, 70);
        this.lblUrl.Name = "lblUrl";
        this.lblUrl.Size = new System.Drawing.Size(85, 15);
        this.lblUrl.TabIndex = 2;
        this.lblUrl.Text = "Mod ZIP URL:";
        //
        // txtUrl
        //
        this.txtUrl.Location = new System.Drawing.Point(12, 88);
        this.txtUrl.Name = "txtUrl";
        this.txtUrl.Size = new System.Drawing.Size(360, 23);
        this.txtUrl.TabIndex = 3;
        this.txtUrl.TextChanged += new System.EventHandler(this.txtUrl_TextChanged);
        //
        // btnAction
        //
        this.btnAction.Location = new System.Drawing.Point(12, 117);
        this.btnAction.Name = "btnAction";
        this.btnAction.Size = new System.Drawing.Size(120, 40);
        this.btnAction.TabIndex = 1;
        this.btnAction.Text = "Action";
        this.btnAction.UseVisualStyleBackColor = true;
        this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
        //        // btnUninstall
        //
        this.btnUninstall.Location = new System.Drawing.Point(140, 117);
        this.btnUninstall.Name = "btnUninstall";
        this.btnUninstall.Size = new System.Drawing.Size(120, 40);
        this.btnUninstall.TabIndex = 5;
        this.btnUninstall.Text = "Uninstall";
        this.btnUninstall.UseVisualStyleBackColor = true;
        this.btnUninstall.Visible = false;
        this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
        //         // notifyIcon
        //
        this.notifyIcon.ContextMenuStrip = this.trayMenu;
        this.notifyIcon.Text = "RebuildUs Launcher";
        this.notifyIcon.Visible = false;
        this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
        //
        // trayMenu
        //
        this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.menuShow,
        this.menuExit});
        this.trayMenu.Name = "trayMenu";
        this.trayMenu.Size = new System.Drawing.Size(104, 48);
        //
        // menuShow
        //
        this.menuShow.Name = "menuShow";
        this.menuShow.Size = new System.Drawing.Size(103, 22);
        this.menuShow.Text = "Show";
        this.menuShow.Click += new System.EventHandler(this.menuShow_Click);
        //
        // menuExit
        //
        this.menuExit.Name = "menuExit";
        this.menuExit.Size = new System.Drawing.Size(103, 22);
        this.menuExit.Text = "Exit";
        this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
        //
        // MainForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(384, 169);
        this.Controls.Add(this.btnAction);
        this.Controls.Add(this.lblVersion);
        this.Controls.Add(this.txtUrl);
        this.Controls.Add(this.lblUrl);
        this.Controls.Add(this.lblStatus);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "RebuildUs Launcher";
        this.trayMenu.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion
}
