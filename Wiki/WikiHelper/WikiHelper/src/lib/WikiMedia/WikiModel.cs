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

namespace WikiHelper.lib.WikiMedia {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class Container : Element {
    #region Fields
    public List<Element> elements = new List<Element>();
    public Container parent;
    #endregion Fields

    #region Constructors
    public Container(Container parent) {
      this.parent = parent;
    }
    #endregion Constructors

    #region Methods
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      foreach(Element e in elements) {
        sb.Append(e.ToString());
      }
      return sb.ToString();
    }
    #endregion Methods
    
  }

  public class Document : Container {
  
    #region Fields
    public List<Header> headers = new List<Header>();
    #endregion Fields

    #region Constructors
    public Document() : base(null) {
    }
    #endregion Constructors

    #region Methods
    public Header FindHeader(int level, string name) {
      for (int i = 0; i < headers.Count-1; i++) {
        Header header = headers[i];
        if ((header.level==level) && (header.name.Equals(name))) {
          return header;
        }
      }
      return null;
    }

    public Header FindHeader(string name) {
      for (int i = 0; i < headers.Count-1; i++) {
        Header header = headers[i];
        if (header.name == null) {
          continue;
        }
        if (header.name.Equals(name)) {
          return header;
        }
      }
      return null;
    }
    #endregion Methods
    
  }

  public class Element {
    #region Constructors
    public Element() {
    }
    #endregion Constructors
  }

  public class Header : Container {
    #region Fields
    public int level;
    public List<Element> title = new List<Element>();
    #endregion Fields

    #region Constructors

    public Header(Container parent) : base(parent) {
    }
    #endregion Constructors

    #region Properties
    public string name {
      get {
        string res = null;
        if (title.Count>0) {
          res = title[0].ToString();
        }
        return res;
      }
    }
    #endregion Properties

    #region Methods
    public virtual bool isEmpty() {
      return (elements.Count==0);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(title.ToString()).Append('\n');
      sb.Append(base.ToString()).Append('\n');
      return sb.ToString();
    }
    #endregion Methods
    
  }

  public class HyperLink : Text {
    #region Fields
    public string URL;
    #endregion Fields

    #region Constructors
    public HyperLink() {
    }
    #endregion Constructors

    #region Methods
    public override string ToString() {
      return "["+base.ToString()+"]";
    }
    #endregion Methods
    
  }

  public class List : Container {
    
    #region Constructors
    public List(Container parent) : base(parent) {
    }
    #endregion Constructors
    
  }

  public class ListItem : Text {
    #region Fields
    public int level;
    public int mode;
    public int num;
    #endregion Fields

    #region Constructors
    public ListItem() {
      level = 1;
      num = 1;
      mode = 0;
    }
    #endregion Constructors

    #region Methods
    public override string ToString() {
      return "\t"+base.ToString();
    }
    #endregion Methods
    
  }

  public class NewLine : Element {
  
    #region Constructors
    public NewLine() {
    }
    #endregion Constructors

    #region Methods
    public override string ToString() {
      return "\n";
    }
    #endregion Methods
    
  }

  public class Paragraph : Container {
    #region Constructors
    public Paragraph(Container parent) : base(parent) {
    }
    #endregion Constructors

    #region Methods
    public override string ToString() {
      return base.ToString()+"\n";
    }
    #endregion Methods
    
  }

  public class Text : Element {
    
    #region Fields
    public const int FMT_BOLD = 2;
    public const int FMT_ITALIC = 1;
    public const int FMT_UNDERLINE = 4;
    public int format;
    public StringBuilder text;
    #endregion Fields

    #region Constructors
    public Text() {
      text = new StringBuilder();
    }

    public Text(string text) : this() {
      Append(text);
    }
    #endregion Constructors

    #region Properties
    public bool bold {
      get {
        return (format & FMT_BOLD) == FMT_BOLD;
      }
      set {
        if (value) {
          format = format | FMT_BOLD;
        }
        else {
          format = format & ~FMT_BOLD;
        }
      }
    }

    public bool italic {
      get {
        return (format & FMT_ITALIC) == FMT_ITALIC;
      }
      set {
        if (value) {
          format = format | FMT_ITALIC;
        }
        else {
          format = format & ~FMT_ITALIC;
        }
      }
    }

    public bool underline {
      get {
        return (format & FMT_UNDERLINE) == FMT_UNDERLINE;
      }
      set {
        if (value) {
          format = format | FMT_UNDERLINE;
        }
        else {
          format = format & ~FMT_UNDERLINE;
        }
      }
    }
    #endregion Properties

    #region Methods
    public Text Append(string text) {
      this.text.Append(text);
      return this;
    }

    public override string ToString() {
      return text.ToString();
    }
    #endregion Methods
    
  }
  
}