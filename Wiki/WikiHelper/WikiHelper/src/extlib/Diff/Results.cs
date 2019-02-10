namespace DiffCalc {
  using System.Collections;
  using System.Drawing;
  using System.Windows.Forms;

  using DifferenceEngine;

  /// <summary>
  /// Summary description for Results.
  /// </summary>
  public class Results : System.Windows.Forms.Form {
    #region Fields
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ColumnHeader columnHeader3;
    private System.Windows.Forms.ColumnHeader columnHeader4;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.ListView lvDestination;
    private System.Windows.Forms.ListView lvSource;
    #endregion Fields

    #region Constructors
    public Results(IDiffList source, IDiffList destination, ArrayList DiffLines, double seconds) {
      InitializeComponent();
      this.Text = string.Format("Results: {0} secs.", seconds.ToString("#0.00"));
      ListViewItem lviS;
      ListViewItem lviD;
      int cnt = 1;
      int i;
      foreach (DiffResultSpan drs in DiffLines) {
        switch (drs.Status) {
          case DiffResultSpanStatus.DeleteSource:
            for (i = 0; i < drs.Length; i++) {
              lviS = new ListViewItem(cnt.ToString("00000"));
              lviD = new ListViewItem(cnt.ToString("00000"));
              lviS.BackColor = Color.Red;
              lviS.SubItems.Add((source.GetByIndex(drs.SourceIndex + i)).ToString());
              lviD.BackColor = Color.LightGray;
              lviD.SubItems.Add("");
              lvSource.Items.Add(lviS);
              lvDestination.Items.Add(lviD);
              cnt++;
            }
            break;
          case DiffResultSpanStatus.NoChange:
            for (i = 0; i < drs.Length; i++) {
              lviS = new ListViewItem(cnt.ToString("00000"));
              lviD = new ListViewItem(cnt.ToString("00000"));
              lviS.BackColor = Color.White;
              lviS.SubItems.Add((source.GetByIndex(drs.SourceIndex + i)).ToString());
              lviD.BackColor = Color.White;
              lviD.SubItems.Add((destination.GetByIndex(drs.DestIndex + i)).ToString());
              lvSource.Items.Add(lviS);
              lvDestination.Items.Add(lviD);
              cnt++;
            }
            break;
          case DiffResultSpanStatus.AddDestination:
            for (i = 0; i < drs.Length; i++) {
              lviS = new ListViewItem(cnt.ToString("00000"));
              lviD = new ListViewItem(cnt.ToString("00000"));
              lviS.BackColor = Color.LightGray;
              lviS.SubItems.Add("");
              lviD.BackColor = Color.LightGreen;
              lviD.SubItems.Add((destination.GetByIndex(drs.DestIndex + i)).ToString());
              lvSource.Items.Add(lviS);
              lvDestination.Items.Add(lviD);
              cnt++;
            }
            break;
          case DiffResultSpanStatus.Replace:
            for (i = 0; i < drs.Length; i++) {
              lviS = new ListViewItem(cnt.ToString("00000"));
              lviD = new ListViewItem(cnt.ToString("00000"));
              lviS.BackColor = Color.Red;
              lviS.SubItems.Add((source.GetByIndex(drs.SourceIndex + i)).ToString());
              lviD.BackColor = Color.LightGreen;
              lviD.SubItems.Add((destination.GetByIndex(drs.DestIndex + i)).ToString());
              lvSource.Items.Add(lviS);
              lvDestination.Items.Add(lviD);
              cnt++;
            }
            break;
        }
      }
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.lvSource = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.lvDestination = new System.Windows.Forms.ListView();
      this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      //
      // lvSource
      //
      this.lvSource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.columnHeader1, this.columnHeader2 });
      this.lvSource.FullRowSelect = true;
      this.lvSource.HideSelection = false;
      this.lvSource.Location = new System.Drawing.Point(28, 17);
      this.lvSource.MultiSelect = false;
      this.lvSource.Name = "lvSource";
      this.lvSource.Size = new System.Drawing.Size(114, 102);
      this.lvSource.TabIndex = 0;
      this.lvSource.View = System.Windows.Forms.View.Details;
      this.lvSource.Resize += new System.EventHandler(this.lvSource_Resize);
      this.lvSource.SelectedIndexChanged += new System.EventHandler(this.lvSource_SelectedIndexChanged);
      //
      // columnHeader1
      //
      this.columnHeader1.Text = "Line";
      this.columnHeader1.Width = 50;
      //
      // columnHeader2
      //
      this.columnHeader2.Text = "Text (Source)";
      this.columnHeader2.Width = 147;
      //
      // lvDestination
      //
      this.lvDestination.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.columnHeader3, this.columnHeader4 });
      this.lvDestination.FullRowSelect = true;
      this.lvDestination.HideSelection = false;
      this.lvDestination.Location = new System.Drawing.Point(176, 15);
      this.lvDestination.MultiSelect = false;
      this.lvDestination.Name = "lvDestination";
      this.lvDestination.Size = new System.Drawing.Size(123, 110);
      this.lvDestination.TabIndex = 2;
      this.lvDestination.View = System.Windows.Forms.View.Details;
      this.lvDestination.Resize += new System.EventHandler(this.lvDestination_Resize);
      this.lvDestination.SelectedIndexChanged += new System.EventHandler(this.lvDestination_SelectedIndexChanged);
      //
      // columnHeader3
      //
      this.columnHeader3.Text = "Line";
      this.columnHeader3.Width = 50;
      //
      // columnHeader4
      //
      this.columnHeader4.Text = "Text (Destination)";
      this.columnHeader4.Width = 198;
      //
      // Results
      //
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(533, 440);
      this.Controls.Add(this.lvDestination);
      this.Controls.Add(this.lvSource);
      this.Name = "Results";
      this.Text = "Results";
      this.Resize += new System.EventHandler(this.Results_Resize);
      this.Load += new System.EventHandler(this.Results_Load);
      this.ResumeLayout(false);
    }

    private void lvDestination_Resize(object sender, System.EventArgs e) {
      if (lvDestination.Width > 100) {
        lvDestination.Columns[1].Width = -2;
      }
    }

    private void lvDestination_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (lvDestination.SelectedItems.Count > 0) {
        ListViewItem lvi = lvSource.Items[lvDestination.SelectedItems[0].Index];
        lvi.Selected = true;
        lvi.EnsureVisible();
      }
    }

    private void lvSource_Resize(object sender, System.EventArgs e) {
      if (lvSource.Width > 100) {
        lvSource.Columns[1].Width = -2;
      }
    }

    private void lvSource_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (lvSource.SelectedItems.Count > 0) {
        ListViewItem lvi = lvDestination.Items[lvSource.SelectedItems[0].Index];
        lvi.Selected = true;
        lvi.EnsureVisible();
      }
    }

    private void Results_Load(object sender, System.EventArgs e) {
      Results_Resize(sender, e);
    }

    private void Results_Resize(object sender, System.EventArgs e) {
      int w = this.ClientRectangle.Width / 2;
      lvSource.Location = new Point(0, 0);
      lvSource.Width = w;
      lvSource.Height = this.ClientRectangle.Height;
      lvDestination.Location = new Point(w + 1, 0);
      lvDestination.Width = this.ClientRectangle.Width - (w + 1);
      lvDestination.Height = this.ClientRectangle.Height;
    }
    #endregion Methods
  }

}