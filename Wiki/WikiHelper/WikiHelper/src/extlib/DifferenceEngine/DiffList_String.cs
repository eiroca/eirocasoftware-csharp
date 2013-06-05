#region GPL >3
/**
 * (C) 2006-2010 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
#endregion Header

namespace DifferenceEngine  {
  using System;
  using System.Collections;

  public class DiffList_String : IDiffList {
    #region Fields
    private const int MaxLineLength = 1024;
    private ArrayList _lines;
    #endregion Fields

    #region Constructors
    public DiffList_String(string data, char[] sep) {
      _lines = new ArrayList();
      string[] lin = data.Split(sep);
      foreach (string line in lin) {
        _lines.Add(new TextLine(line));
      }
    }
    #endregion Constructors

    #region Methods
    public int Count() {
      return _lines.Count;
    }

    public IComparable GetByIndex(int index) {
      return (TextLine)_lines[index];
    }
    #endregion Methods
    
  }
  
}