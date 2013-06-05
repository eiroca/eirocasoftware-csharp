#region Header
/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
#endregion Header

namespace WikiHelper.gui {
  using System;
  using System.IO;
  using System.Text;
  using System.Windows.Forms;

  public partial class ReplaceForm : Form {
    #region Fields
    WikiConf conf;
    #endregion Fields

    #region Constructors
    public ReplaceForm(WikiConf conf) {
      InitializeComponent();
      this.conf = conf;
      SetCategories(conf.categories);
      LoadData();
      Update(-1);
    }
    #endregion Constructors

    #region Methods
    public string GetCategory() {
      string cat = iCategory.Text.Trim();
      return (String.IsNullOrEmpty(cat) ? null : cat);
    }

    public string[,] GetReplaces() {
      string[,] replaces = null;
      int cnt = lbTo.Items.Count;
      if (cnt>0) {
        replaces = new string[cnt, 2];
        for (int i=0; i<cnt; i++) {
          replaces[i, 0] = lbFrom.Items[i].ToString();
          replaces[i, 1] = lbTo.Items[i].ToString();
        }
      }
      return replaces;
    }

    public void SetCategories(string[] categories) {
      iCategory.Items.Clear();
      iCategory.Items.AddRange(categories);
    }

    void BAddClick(object sender, EventArgs e) {
      lbTo.Items.Add(iTo.Text);
      lbFrom.Items.Add(iFrom.Text);
      Update(lbTo.Items.Count-1);
    }

    void BDeleteClick(object sender, EventArgs e) {
      int index = lbTo.SelectedIndex;
      if (index>=0) {
        lbTo.Items.RemoveAt(index);
        lbFrom.Items.RemoveAt(index);
        if (index >= lbTo.Items.Count) {
          index = lbTo.Items.Count-1;
        }
        Update(index);
      }
    }

    void BReplaceClick(object sender, EventArgs e) {
      int index = lbTo.SelectedIndex;
      if (index>=0) {
        string from = iFrom.Text;
        string to = iTo.Text;
        lbTo.Items[index] = to;
        lbFrom.Items[index] = from;
      }
    }

    void LbFromSelectedIndexChanged(object sender, EventArgs e) {
      Update(lbFrom.SelectedIndex);
    }

    void LbToSelectedIndexChanged(object sender, EventArgs e) {
      Update(lbTo.SelectedIndex);
    }

    void LoadData() {
      string[,] data = WikiConf.LoadPair(conf.PathReplaces);
      lbFrom.Items.Clear();
      lbTo.Items.Clear();
      for (int i=0; i<data.GetLength(0); i++) {
        lbFrom.Items.Add(data[i, 0]);
        lbTo.Items.Add(data[i, 1]);
      }
    }

    void Update(int index) {
      lbFrom.SelectedIndex = index;
      lbTo.SelectedIndex = index;
      if (index>=0) {
        iFrom.Text = lbFrom.Items[index].ToString();
        iTo.Text = lbTo.Items[index].ToString();
        bDelete.Enabled = true;
        bReplace.Enabled = true;
      }
      else {
        bDelete.Enabled = false;
        bReplace.Enabled = false;
      }
    }
    #endregion Methods

  }
  
}