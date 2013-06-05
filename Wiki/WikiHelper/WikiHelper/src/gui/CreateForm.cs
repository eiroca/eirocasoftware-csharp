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

  public partial class CreateForm : Form {
    #region Fields
    WikiConf conf;
    #endregion Fields

    #region Constructors
    public CreateForm(WikiConf conf) {
      InitializeComponent();
      this.conf = conf;
      LoadData();
    }
    #endregion Constructors

    #region Methods
    void LoadData() {
      string[] template = File.ReadAllLines(conf.newPage_Template, Encoding.UTF8);
      string[] lines = File.ReadAllLines(conf.newPage_List, Encoding.UTF8);
      lbTemplate.Items.AddRange(template);
      lbPages.Items.AddRange(lines);
    }
    #endregion Methods
    
  }
  
}