namespace DiffCalc {
  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.Drawing;
  using System.Windows.Forms;

  using DifferenceEngine;

  /// <summary>
  /// Summary description for BinaryResults.
  /// </summary>
  public class BinaryResults : System.Windows.Forms.Form {
    #region Fields
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ColumnHeader columnHeader3;
    private System.Windows.Forms.ColumnHeader columnHeader4;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.ListView listView1;
    #endregion Fields

    #region Constructors
    public BinaryResults(ArrayList al,double secs) {
      InitializeComponent();
      this.Text = string.Format("Binary Results: {0} secs.",secs.ToString("#0.00"));
      ListViewItem lvi = null;
      foreach (DiffResultSpan drs in al) {
        lvi = new ListViewItem(drs.Status.ToString());
        lvi.SubItems.Add(drs.DestIndex == -1 ? "---" : drs.DestIndex.ToString());
        lvi.SubItems.Add(drs.SourceIndex == -1 ? "---" : drs.SourceIndex.ToString());
        lvi.SubItems.Add(drs.Length.ToString());
        //lvi.BackColor = shade ? Color.AliceBlue : Color.White;
        switch (drs.Status) {
          case DiffResultSpanStatus.NoChange:
            lvi.ForeColor = Color.Black;
            lvi.BackColor = Color.White;
            break;
          case DiffResultSpanStatus.DeleteSource:
            lvi.ForeColor = Color.White;
            lvi.BackColor = Color.LightCoral;
            break;
          case DiffResultSpanStatus.AddDestination:
            lvi.ForeColor = Color.White;
            lvi.BackColor = Color.LightGreen;
            break;
          case DiffResultSpanStatus.Replace:
            lvi.ForeColor = Color.White;
            lvi.BackColor = Color.LightSkyBlue;
            break;
        }
        listView1.Items.Add(lvi);
      }
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)  {
      if (disposing) {
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    private void BinaryResults_Load(object sender, System.EventArgs e) {
      listView1_Resize(this,e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.listView1 = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      //
      // listView1
      //
      this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.columnHeader1, this.columnHeader2, this.columnHeader3, this.columnHeader4});
      this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView1.FullRowSelect = true;
      this.listView1.GridLines = true;
      this.listView1.Location = new System.Drawing.Point(0, 0);
      this.listView1.MultiSelect = false;
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(368, 308);
      this.listView1.TabIndex = 0;
      this.listView1.View = System.Windows.Forms.View.Details;
      this.listView1.Resize += new System.EventHandler(this.listView1_Resize);
      //
      // columnHeader1
      //
      this.columnHeader1.Text = "Result";
      //
      // columnHeader2
      //
      this.columnHeader2.Text = "Dest Index";
      this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      //
      // columnHeader3
      //
      this.columnHeader3.Text = "Source Index";
      this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      //
      // columnHeader4
      //
      this.columnHeader4.Text = "Length";
      this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      //
      // BinaryResults
      //
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(368, 308);
      this.Controls.Add(this.listView1);
      this.Name = "BinaryResults";
      this.Text = "BinaryResults";
      this.Load += new System.EventHandler(this.BinaryResults_Load);
      this.ResumeLayout(false);
    }

    private void listView1_Resize(object sender, System.EventArgs e) {
      int w = Math.Max((listView1.Width - 20)/4,50);
      foreach (ColumnHeader ch in listView1.Columns) {
        ch.Width = w;
      }
    }
    #endregion Methods

  }
  
}