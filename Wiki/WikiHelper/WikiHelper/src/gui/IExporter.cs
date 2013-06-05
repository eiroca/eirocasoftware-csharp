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

  public interface IExporter {
    #region Methods
    void Export(WikiMedia.ExportNotify notify);
    bool Setup();
    #endregion Methods
    
  }
  
}