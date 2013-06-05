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

  using DotNetWikiBot;

  using WikiHelper.lib.WikiMedia;

  public partial class PageProcess : Form {
    #region Fields
    WikiConf conf;
    WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public PageProcess(WikiConf conf, WikiMedia wiki) {
      InitializeComponent();
      this.conf = conf;
      this.wiki = wiki;
    }
    #endregion Constructors

    #region Methods
    public string[] getOpers() {
      return new string[0];
    }

    public string[] getPages() {
      return iPage.Lines;
    }

    void BLoadCategoryClick(object sender, System.EventArgs e) {
      string category = iCategory.Text;
      if (!String.IsNullOrEmpty(category)) {
        PageList pages = wiki.GetPages(category);
        if (pages.Count()>0) {
          string[] lines = new string[pages.Count()];
          for(int i=0; i<pages.Count(); i++) {
            lines[i] = pages[i].title;
          }
          iPage.Lines = lines;
        }
      }
    }
    #endregion Methods
    
  }
  
}