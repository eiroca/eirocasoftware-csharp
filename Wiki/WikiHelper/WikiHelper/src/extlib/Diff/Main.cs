namespace DiffCalc {
  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.Drawing;
  using System.IO;
  using System.Windows.Forms;

  using DifferenceEngine;

  /// <summary>
  /// Summary description for Form1.
  /// </summary>
  public class MainForm : System.Windows.Forms.Form  {
    #region Fields
    private System.Windows.Forms.CheckBox chkBinary;
    private System.Windows.Forms.Button cmdClose;
    private System.Windows.Forms.Button cmdCompare;
    private System.Windows.Forms.Button cmdDestination;
    private System.Windows.Forms.Button cmdSource;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RadioButton rbFast;
    private System.Windows.Forms.RadioButton rbMedium;
    private System.Windows.Forms.RadioButton rbSlow;
    private System.Windows.Forms.TextBox txtDestination;
    private System.Windows.Forms.TextBox txtSource;
    private DiffEngineLevel _level;
    #endregion Fields

    #region Constructors
    public MainForm() {
      InitializeComponent();
      _level = DiffEngineLevel.FastImperfect;
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )  {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main2() {
      Application.Run(new MainForm());
    }

    private void BinaryDiff(string sFile, string dFile) {
      this.Cursor = Cursors.WaitCursor;
      DiffList_BinaryFile sLF = null;
      DiffList_BinaryFile dLF = null;
      try {
        sLF = new DiffList_BinaryFile(sFile);
        dLF = new DiffList_BinaryFile(dFile);
      }
      catch (Exception ex) {
        this.Cursor = Cursors.Default;
        MessageBox.Show(ex.Message,"File Error");
        return;
      }
      try {
        double time = 0;
        DiffEngine de = new DiffEngine();
        time = de.ProcessDiff(sLF,dLF,_level);
        ArrayList rep = de.DiffReport();
        BinaryResults dlg = new BinaryResults(rep,time);
        dlg.ShowDialog();
        dlg.Dispose();
      }
      catch (Exception ex) {
        this.Cursor = Cursors.Default;
        string tmp = string.Format("{0}{1}{1}***STACK***{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
        MessageBox.Show(tmp,"Compare Error");
        return;
      }
      this.Cursor = Cursors.Default;
    }

    private void cmdClose_Click(object sender, System.EventArgs e) {
      this.Close();
    }

    private void cmdCompare_Click(object sender, System.EventArgs e) {
      string sFile = txtSource.Text.Trim();
      string dFile = txtDestination.Text.Trim();
      if (!ValidFile(sFile)) {
        MessageBox.Show("Source file name is invalid.","Invalid File");
        txtSource.Focus();
        return;
      }
      if (!ValidFile(dFile)) {
        MessageBox.Show("Destination file name is invalid.","Invalid File");
        txtDestination.Focus();
        return;
      }
      if (rbFast.Checked) {
        _level = DiffEngineLevel.FastImperfect;
      }
      else {
        if (rbMedium.Checked) {
          _level = DiffEngineLevel.Medium;
        }
        else {
          _level = DiffEngineLevel.SlowPerfect;
        }
      }
      if (chkBinary.Checked) {
        BinaryDiff(sFile,dFile);
      }
      else {
        TextDiff(sFile,dFile);
      }
    }

    private void cmdDestination_Click(object sender, System.EventArgs e) {
      txtDestination.Text = GetFileName();
    }

    private void cmdSource_Click(object sender, System.EventArgs e) {
      txtSource.Text = GetFileName();
    }

    private string GetFileName() {
      string fname = string.Empty;
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.InitialDirectory = "c:\\" ;
      dlg.Filter = "All files (*.*)|*.*" ;
      dlg.FilterIndex = 1 ;
      dlg.RestoreDirectory = true ;
      if(dlg.ShowDialog() == DialogResult.OK) {
        fname = dlg.FileName;
      }
      return fname;
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.cmdCompare = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtSource = new System.Windows.Forms.TextBox();
      this.cmdSource = new System.Windows.Forms.Button();
      this.cmdDestination = new System.Windows.Forms.Button();
      this.txtDestination = new System.Windows.Forms.TextBox();
      this.cmdClose = new System.Windows.Forms.Button();
      this.chkBinary = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rbFast = new System.Windows.Forms.RadioButton();
      this.rbMedium = new System.Windows.Forms.RadioButton();
      this.rbSlow = new System.Windows.Forms.RadioButton();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      //
      // cmdCompare
      //
      this.cmdCompare.Location = new System.Drawing.Point(153, 165);
      this.cmdCompare.Name = "cmdCompare";
      this.cmdCompare.Size = new System.Drawing.Size(61, 20);
      this.cmdCompare.TabIndex = 8;
      this.cmdCompare.Text = "Compare";
      this.cmdCompare.Click += new System.EventHandler(this.cmdCompare_Click);
      //
      // label1
      //
      this.label1.Location = new System.Drawing.Point(6, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 15);
      this.label1.TabIndex = 0;
      this.label1.Text = "Source:";
      //
      // label2
      //
      this.label2.Location = new System.Drawing.Point(6, 55);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(85, 15);
      this.label2.TabIndex = 3;
      this.label2.Text = "Destination:";
      //
      // txtSource
      //
      this.txtSource.Location = new System.Drawing.Point(20, 26);
      this.txtSource.Name = "txtSource";
      this.txtSource.Size = new System.Drawing.Size(233, 20);
      this.txtSource.TabIndex = 1;
      this.txtSource.Text = "";
      //
      // cmdSource
      //
      this.cmdSource.Location = new System.Drawing.Point(253, 26);
      this.cmdSource.Name = "cmdSource";
      this.cmdSource.Size = new System.Drawing.Size(28, 20);
      this.cmdSource.TabIndex = 2;
      this.cmdSource.Text = "...";
      this.cmdSource.Click += new System.EventHandler(this.cmdSource_Click);
      //
      // cmdDestination
      //
      this.cmdDestination.Location = new System.Drawing.Point(253, 76);
      this.cmdDestination.Name = "cmdDestination";
      this.cmdDestination.Size = new System.Drawing.Size(28, 20);
      this.cmdDestination.TabIndex = 5;
      this.cmdDestination.Text = "...";
      this.cmdDestination.Click += new System.EventHandler(this.cmdDestination_Click);
      //
      // txtDestination
      //
      this.txtDestination.Location = new System.Drawing.Point(20, 76);
      this.txtDestination.Name = "txtDestination";
      this.txtDestination.Size = new System.Drawing.Size(233, 20);
      this.txtDestination.TabIndex = 4;
      this.txtDestination.Text = "";
      //
      // cmdClose
      //
      this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmdClose.Location = new System.Drawing.Point(220, 165);
      this.cmdClose.Name = "cmdClose";
      this.cmdClose.Size = new System.Drawing.Size(61, 20);
      this.cmdClose.TabIndex = 9;
      this.cmdClose.Text = "Close";
      this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
      //
      // chkBinary
      //
      this.chkBinary.Location = new System.Drawing.Point(20, 166);
      this.chkBinary.Name = "chkBinary";
      this.chkBinary.Size = new System.Drawing.Size(83, 16);
      this.chkBinary.TabIndex = 7;
      this.chkBinary.Text = "Binary Diff";
      //
      // groupBox1
      //
      this.groupBox1.Controls.Add(this.rbSlow);
      this.groupBox1.Controls.Add(this.rbMedium);
      this.groupBox1.Controls.Add(this.rbFast);
      this.groupBox1.Location = new System.Drawing.Point(7, 103);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(270, 51);
      this.groupBox1.TabIndex = 6;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Diff Level";
      //
      // rbFast
      //
      this.rbFast.Checked = true;
      this.rbFast.Location = new System.Drawing.Point(10, 18);
      this.rbFast.Name = "rbFast";
      this.rbFast.Size = new System.Drawing.Size(45, 24);
      this.rbFast.TabIndex = 0;
      this.rbFast.TabStop = true;
      this.rbFast.Text = "Fast";
      //
      // rbMedium
      //
      this.rbMedium.Location = new System.Drawing.Point(89, 18);
      this.rbMedium.Name = "rbMedium";
      this.rbMedium.Size = new System.Drawing.Size(69, 24);
      this.rbMedium.TabIndex = 1;
      this.rbMedium.Text = "Medium";
      //
      // rbSlow
      //
      this.rbSlow.Location = new System.Drawing.Point(180, 18);
      this.rbSlow.Name = "rbSlow";
      this.rbSlow.Size = new System.Drawing.Size(83, 24);
      this.rbSlow.TabIndex = 2;
      this.rbSlow.Text = "Slow/Best";
      //
      // Form1
      //
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.cmdClose;
      this.ClientSize = new System.Drawing.Size(286, 194);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.chkBinary);
      this.Controls.Add(this.cmdClose);
      this.Controls.Add(this.cmdDestination);
      this.Controls.Add(this.txtDestination);
      this.Controls.Add(this.txtSource);
      this.Controls.Add(this.cmdSource);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmdCompare);
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Compare Files";
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    private void TextDiff(string sFile, string dFile) {
      this.Cursor = Cursors.WaitCursor;
      DiffList_TextFile sLF = null;
      DiffList_TextFile dLF = null;
      try {
        sLF = new DiffList_TextFile(sFile);
        dLF = new DiffList_TextFile(dFile);
      }
      catch (Exception ex) {
        this.Cursor = Cursors.Default;
        MessageBox.Show(ex.Message,"File Error");
        return;
      }
      try {
        double time = 0;
        DiffEngine de = new DiffEngine();
        time = de.ProcessDiff(sLF,dLF,_level);
        ArrayList rep = de.DiffReport();
        Results dlg = new Results(sLF,dLF,rep,time);
        dlg.ShowDialog();
        dlg.Dispose();
      }
      catch (Exception ex) {
        this.Cursor = Cursors.Default;
        string tmp = string.Format("{0}{1}{1}***STACK***{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
        MessageBox.Show(tmp,"Compare Error");
        return;
      }
      this.Cursor = Cursors.Default;
    }

    private bool ValidFile(string fname) {
      if (fname != string.Empty) {
        if (File.Exists(fname)) {
          return true;
        }
      }
      return false;
    }
    #endregion Methods
    
  }
}