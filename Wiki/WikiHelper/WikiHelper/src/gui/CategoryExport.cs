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
  using System.Drawing;
  using System.Windows.Forms;

  using WikiHelper.lib.WikiMedia;
  using WikiHelper.lib.WikiMedia.converter;

  /// <summary>
  /// Description of CategoryExport.
  /// </summary>
  public partial class CategoryExport : Form, IExporter {
  
    #region Fields
    public Model2PowerPoint converter;
    public WikiHelper.lib.WikiMedia.converter.SummaryBuilder[] extractors;
    public WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public CategoryExport(WikiMedia wiki, Model2PowerPoint converter, SummaryBuilder[] extractors) {
      InitializeComponent();
      this.wiki = wiki;
      this.converter = converter;
      this.extractors = extractors;
      iExtractor.Items.Clear();
      foreach (SummaryBuilder extractor in extractors) {
        iExtractor.Items.Add(extractor.GetName());
      }
      iExtractor.SelectedIndex = 0;
    }
    #endregion Constructors

    #region Methods
    public void Export(WikiMedia.ExportNotify notify) {
      string expCategory = "Category:"+iCategory.Text.Trim();
      if (cbIndex.Checked) {
        NotifyMessage(notify, "Index of "+expCategory+"\n");
        int size = 350;
        int paging = 5;
        Int32.TryParse(iIndexSize.Text, out size);
        Int32.TryParse(iIndexPaging.Text, out paging);
        converter.ExportIndex(expCategory, iIndexName.Text, paging, size, notify);
      }
      if (cbSummary.Checked) {
        NotifyMessage(notify, "Summary of "+expCategory+"\n");
        int paging = 24;
        Int32.TryParse(iSummaryPaging.Text, out paging);
        converter.ExportSummary(expCategory, iSummaryName.Text, iSummaryIndex.Text, paging, extractors[iExtractor.SelectedIndex].Export, notify);
      }
      if (cbPages.Checked) {
        NotifyMessage(notify, "Pages of "+expCategory+"\n");
        converter.ExportPages  (expCategory, iPagesDir.Text, notify);
      }
    }

    public void SetCategories(string[] categories) {
      iCategory.Items.Clear();
      iCategory.Items.AddRange(categories);
    }

    public bool Setup() {
      return (ShowDialog() == DialogResult.OK);
    }

    void NotifyMessage(WikiMedia.ExportNotify notify, string msg) {
      if (notify!=null) {
        notify(msg);
      }
    }
    #endregion Methods
    
  }
  
}