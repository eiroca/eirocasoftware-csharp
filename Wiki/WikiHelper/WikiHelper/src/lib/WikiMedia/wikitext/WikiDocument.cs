#region Header
/**
 * (C) 2006-2010 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
#endregion Header

namespace WikiHelper.lib.WikiMedia {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class WikiContainer {
    #region Fields
    public List<string> text = new List<string>();
    bool _isEmpty = true;
    bool _wasNull = false;
    #endregion Fields

    #region Constructors
    public WikiContainer() {
    }
    #endregion Constructors

    #region Methods
    public virtual void Add(string row) {
      string tRow = row.Trim();
      if (String.IsNullOrEmpty(tRow)) {
        if (!_wasNull) {
          _wasNull = true;
        }
        else {
          text.Add("");
        }
      }
      else {
        if (!_isEmpty) {
          if (_wasNull) {
            text.Add("");
          }
        }
        text.Add(row);
        _isEmpty = false;
        _wasNull = false;
      }
    }

    public string GetText() {
      StringBuilder sb = new StringBuilder();
      foreach (string line in text) {
        sb.Append(line).Append('\n');
      }
      return sb.ToString();
    }

    public virtual bool isEmpty() {
      return _isEmpty;
    }
    #endregion Methods
  }

  public class WikiDocument {
    #region Fields
    public List<WikiHeader> headers = new List<WikiHeader>();
    public WikiContainer rowData = new WikiContainer();
    public Char[] SEP = new Char[] { '\n' };
    public string title;
    WikiHeader header = null;
    #endregion Fields

    #region Constructors
    public WikiDocument(string title, string text) {
      this.title = title;
      header = null;
      string[] rows = text.Split(SEP);
      for (int i = 0; i < rows.Length; i++) {
        ProcessRow(rows[i]);
      }
    }
    #endregion Constructors

    #region Methods

    public WikiHeader FindHeader(int level, string name) {
      if (name != null) {
        for (int i = 0; i < headers.Count - 1; i++) {
          WikiHeader header = headers[i];
          if ((header.level == level) && (name.Equals(header.name))) {
            return header;
          }
        }
      }
      return null;
    }

    public WikiHeader FindHeader(string name) {
      if (name != null) {
        for (int i = 0; i < headers.Count; i++) {
          WikiHeader header = headers[i];
          if (name.Equals(header.name)) {
            return header;
          }
        }
      }
      return null;
    }

    public void ProcessHeaderRow(string row) {
      WikiHeader newHeader = new WikiHeader();
      int lev = 0;
      int state = 0;
      StringBuilder name = new StringBuilder();
      for (int ci = 0; ci < row.Length; ci++) {
        if (row[ci] == '=') {
          if (state == 0) {
            lev++;
          }
          else {
            state = 2;
          }
        }
        else {
          if (state == 0) {
            state = 1;
          }
          if (state == 1) {
            name.Append(row[ci]);
          }
        }
      }
      newHeader.name = name.ToString().Trim();
      newHeader.level = lev;
      headers.Add(newHeader);
      header = newHeader;
    }

    public void ProcessRow(string row) {
      string trimmed = row.Trim();
      if (trimmed.StartsWith("=")) {
        ProcessHeaderRow(trimmed);
      }
      else {
        if (header != null) {
          header.Add(row);
        }
        else {
          rowData.Add(row);
        }
      }
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder(1024);
      for (int i = 0; i < rowData.text.Count; i++) {
        sb.Append(rowData.text[i].ToString()).Append('\n');
      }
      for (int i = 0; i < headers.Count; i++) {
        WikiHeader header = headers[i];
        sb.Append(header.ToString());
      }
      return sb.ToString();
    }

    #endregion Methods
  }

  public class WikiHeader : WikiContainer {
    #region Fields
    public int level;
    public string name;
    #endregion Fields

    #region Constructors
    public WikiHeader() {
    }
    #endregion Constructors

    #region Methods
    public string Format(bool head, bool raw, bool pack, string sep) {
      StringBuilder sb = new StringBuilder();
      if (head) {
        for (int i = 0; i < level; i++) {
          sb.Append('=');
        }
        sb.Append(' ').Append(name).Append(' ');
        for (int i = 0; i < level; i++) {
          sb.Append('=');
        }
        sb.Append(sep);
      }
      if (raw) {
        for (int i = 0; i < text.Count; i++) {
          string row = text[i].ToString();
          if (!(pack && String.IsNullOrEmpty(row))) {
            sb.Append(row).Append(sep);
          }
        }
      }
      return sb.ToString();
    }

    public override string ToString() {
      return Format(true, true, false, "\n");
    }
    #endregion Methods
  }

}