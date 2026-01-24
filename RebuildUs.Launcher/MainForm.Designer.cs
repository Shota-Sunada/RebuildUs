namespace RebuildUs.Launcher;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnAction;
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
        this.btnAction = new System.Windows.Forms.Button();
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
        // btnAction
        //
        this.btnAction.Location = new System.Drawing.Point(12, 45);
        this.btnAction.Name = "btnAction";
        this.btnAction.Size = new System.Drawing.Size(120, 40);
        this.btnAction.TabIndex = 1;
        this.btnAction.Text = "Action";
        this.btnAction.UseVisualStyleBackColor = true;
        this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
        //
        // notifyIcon
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
        this.ClientSize = new System.Drawing.Size(384, 111);
        this.Controls.Add(this.btnAction);
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
