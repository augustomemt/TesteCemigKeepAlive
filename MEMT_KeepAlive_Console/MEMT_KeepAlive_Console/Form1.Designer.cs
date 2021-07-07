
namespace MEMT_KeepAlive_Console
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.CmdStart = new System.Windows.Forms.Button();
      this.CmdStop = new System.Windows.Forms.Button();
      this.TmProcess = new System.Windows.Forms.Timer(this.components);
      this.TmTrees = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // CmdStart
      // 
      this.CmdStart.Location = new System.Drawing.Point(599, 50);
      this.CmdStart.Name = "CmdStart";
      this.CmdStart.Size = new System.Drawing.Size(134, 64);
      this.CmdStart.TabIndex = 0;
      this.CmdStart.Text = "Start";
      this.CmdStart.UseVisualStyleBackColor = true;
      this.CmdStart.Click += new System.EventHandler(this.CmdStart_Click);
      // 
      // CmdStop
      // 
      this.CmdStop.Location = new System.Drawing.Point(599, 120);
      this.CmdStop.Name = "CmdStop";
      this.CmdStop.Size = new System.Drawing.Size(134, 64);
      this.CmdStop.TabIndex = 1;
      this.CmdStop.Text = "Stop";
      this.CmdStop.UseVisualStyleBackColor = true;
      this.CmdStop.Click += new System.EventHandler(this.CmdStop_Click);
      // 
      // TmProcess
      // 
      this.TmProcess.Tick += new System.EventHandler(this.TmProcess_Tick);
      // 
      // TmTrees
      // 
      this.TmTrees.Tick += new System.EventHandler(this.TmTrees_Tick);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.CmdStop);
      this.Controls.Add(this.CmdStart);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button CmdStart;
    private System.Windows.Forms.Button CmdStop;
    private System.Windows.Forms.Timer TmProcess;
    private System.Windows.Forms.Timer TmTrees;
  }
}

