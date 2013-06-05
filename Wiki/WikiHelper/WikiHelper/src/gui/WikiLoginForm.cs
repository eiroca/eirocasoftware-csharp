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
  using System.Windows.Forms;

  using WikiHelper.lib.WikiMedia;

  public partial class WikiLoginForm : Form {
    #region Fields
    WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public WikiLoginForm(WikiMedia wiki, string username, string password) {
      //
      // The InitializeComponent() call is required for Windows Forms designer support.
      //
      InitializeComponent();
      this.wiki = wiki;
      if (username!=null) {
        iUsername.Text= username;
      }
      if (password!=null) {
        iPassword.Text= password;
      }
    }
    #endregion Constructors
    
  }
  
}