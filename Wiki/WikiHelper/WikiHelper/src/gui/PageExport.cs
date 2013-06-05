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
  public partial class PageExport : Form, IExporter {
    #region Fields
    public Model2PowerPoint converter;
    public WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public PageExport(WikiMedia wiki, Model2PowerPoint converter) {
      InitializeComponent();
      this.wiki = wiki;
      this.converter = converter;
    }
    #endregion Constructors

    #region Methods
    public void Export(WikiMedia.ExportNotify notify) {
      string outDir = iOutDir.Text;
      converter.ExportPages(iPages.Lines, outDir, notify);
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