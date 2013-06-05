#region Header
/** GPL > 3
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
  using System.Collections.Specialized;
  using System.Text;

  #region Enumerations
  public enum Status {
    UNKNOWN = 0,
    REAL,
    ACTIVE,
    CHECKED,
    ENGAGED
  }
  #endregion Enumerations

  public interface WikiErrorCheck {
    #region Methods
    void Process(WikiDocument doc, WikiMedia.ExportNotify notify);
    #endregion Methods
  }

  public interface WikiFixUp {
    #region Methods
    bool Process(WikiDocument doc, TagObj obj);
    #endregion Methods
  }

  public interface WikiProcessor {
      #region Methods
      bool Process(WikiDocument doc);
      #endregion Methods
    }

  public class CheckContact : WikiErrorCheck {

    #region Methods
    public void Process(WikiDocument doc, WikiMedia.ExportNotify notify) {
      WikiHeader header;
      header = doc.FindHeader("Contact Information");
      if (header!=null) {
        foreach (string line in header.text) {
          if (line.EndsWith("http://")) {
            notify("E\t"+doc.title+"\tInvalid header: Contact Information\n");
            }
          }
        }
      }
    #endregion Methods
    
  }

  public class CheckInvalidParagraph : WikiErrorCheck {
    #region Fields
    private HashSet<string> blacklist = new HashSet<string>();
    #endregion Fields

    #region Constructors
    public CheckInvalidParagraph(string[] invalidNames) {
      for (int i=0; i<invalidNames.Length; i++) {
        blacklist.Add(invalidNames[i]);
      }
    }
    #endregion Constructors

    #region Methods
    public void Process(WikiDocument doc, WikiMedia.ExportNotify notify) {
      foreach (string name in blacklist) {
        WikiHeader header = doc.FindHeader(name);
        if (header!=null) {
          notify("E\t"+doc.title+"\tInvalid header: "+name+"\n");
        }
      }
    }
    #endregion Methods
    
  }

  public class CheckMissingParagraph : WikiErrorCheck {
    #region Fields
    private HashSet<string> requiredNames = new HashSet<string>();
    #endregion Fields

    #region Constructors
    public CheckMissingParagraph(string[] requiredNames) {
      for (int i=0; i<requiredNames.Length; i++) {
        this.requiredNames.Add(requiredNames[i]);
      }
    }
    #endregion Constructors

    #region Methods
    public void Process(WikiDocument doc, WikiMedia.ExportNotify notify) {
      foreach (string name in requiredNames) {
        WikiHeader header = doc.FindHeader(name);
        if (header==null) {
          notify("E\t"+doc.title+"\tMissing header: "+name+"\n");
        }
      }
    }
    #endregion Methods
    
  }

  public class FixUpContact : WikiFixUp {

    #region Methods
    public bool Process(WikiDocument doc, TagObj obj) {
      WikiHeader contact;
      contact = doc.FindHeader("Contact Information");
      if (contact!=null) {
        int i=0;
        while (i<contact.text.Count) {
          string line = contact.text[i];
          if (!String.IsNullOrEmpty(line)) {
            if (line.StartsWith(":")) {
              line = line.Trim().Substring(1);
              if (String.IsNullOrEmpty(line)) {
                contact.text.RemoveAt(i);
                continue;
              }
              if (line.StartsWith("Tel:")) {
                string tel = line.Substring("Tel:".Length).Trim();
                if (String.IsNullOrEmpty(tel)) {
                  contact.text.RemoveAt(i);
                  continue;
                }
              }
              else if (line.StartsWith("Fax:")) {
                string tel = line.Substring("Fax:".Length).Trim();
                if (String.IsNullOrEmpty(tel)) {
                  contact.text.RemoveAt(i);
                  continue;
                }
              }
              else if (line.StartsWith("Info:")) {
                string tel = line.Substring("Info:".Length).Trim();
                if (String.IsNullOrEmpty(tel)) {
                  contact.text.RemoveAt(i);
                  continue;
                }
              }
            }
            else if (line.EndsWith("http://")) {
              contact.text[i] = line.Substring(0, line.Length - "http://".Length);
            }
          }
          i++;
        }
      }
      return true;
    }
    #endregion Methods
    
  }

  public class FixUpMapping : WikiFixUp {

    #region Fields
    private string field;
    private Dictionary<string, string> mapping;
    #endregion Fields

    #region Constructors
    public FixUpMapping(string field, string[,] map) {
      this.mapping = new Dictionary<string, string>();
      for (int i=0; i<map.GetLength(0); i++) {
        this.mapping.Add(map[i, 0], map[i, 1]);
      }
      this.field = field;
    }
    #endregion Constructors

    #region Methods
    public bool Process(WikiDocument doc, TagObj obj) {
      string val = obj[field];
      if (!String.IsNullOrEmpty(val)) {
        string newVal = null;
        mapping.TryGetValue(val, out newVal);
        if (String.IsNullOrEmpty(newVal)) {
          newVal = val;
        }
        obj[field] = newVal;
      }
      return true;
    }
    #endregion Methods

  }

  public class FixUpObj : WikiFixUp {
    
    #region Fields
    public static string STATUS_STR = "status";
    #endregion Fields

    #region Methods
    public bool Process(WikiDocument doc, TagObj obj) {
      string str = obj[STATUS_STR];
      Status status;
      try {
        status = (Status)Enum.Parse(typeof(Status), str.ToString().ToUpper());
      }
      catch {
        status = Status.UNKNOWN;
      }
      obj[STATUS_STR] = status.ToString().ToUpper();
      return true;
    }
    #endregion Methods
    
  }

  public class FixUpSummary : WikiFixUp {

    #region Fields
    private static string PAGENAME = "{{PAGENAME}}";
    private static string SUMMARY = "summary";
    #endregion Fields

    #region Methods
    public bool Process(WikiDocument doc, TagObj obj) {
      WikiHeader summary;
      object summaryText = obj[SUMMARY];
      if (summaryText!=null) {
        obj[SUMMARY] = null;
        summary = new WikiHeader();
        summary.level=1;
        summary.name = PAGENAME;
        string sum = summaryText.ToString();
        if (String.IsNullOrEmpty(sum)) {
          sum = PAGENAME;
        }
        summary.text.Add(sum);
        doc.headers.Insert(0, summary);
      }
      WikiHeader head0 = doc.headers.Count>0 ? doc.headers[0] : null;
      if ((head0==null) || (!head0.name.Equals(PAGENAME))) {
        summary = new WikiHeader();
        summary.level=1;
        summary.name=PAGENAME;
        summary.text.Add(PAGENAME);
        doc.headers.Insert(0, summary);
      }
      return true;
    }
    #endregion Methods
    
  }

  public class ProcessorRemoveEmpty : WikiProcessor {
    #region Methods
    public bool Process(WikiDocument doc) {
      for (int i=doc.headers.Count-1; i>=0; i--) {
        WikiHeader header = doc.headers[i];
        if (header.text.Count==0) {
          int nextLev = header.level;
          if (i<doc.headers.Count-1) {
            nextLev = doc.headers[i+1].level;
          }
          if (header.level>=nextLev) {
            doc.headers.RemoveAt(i);
          }
        }
      }
      return true;
    }
    #endregion Methods
    
  }

  public class ProcessorStandardEmpty : WikiProcessor {

    #region Fields
    private static HashSet<string> strip = new HashSet<string>();
    #endregion Fields
  
    #region Constructors
    public ProcessorStandardEmpty(string[] emptyAlias) {
      for (int i=0; i<emptyAlias.Length; i++) {
        String alias = emptyAlias[i];
        if (!String.IsNullOrEmpty(alias)) {
          strip.Add(alias);
        }
      }
    }
    #endregion Constructors
  
    #region Methods
    public bool Process(WikiDocument doc) {
      ProcessHeader(doc.rowData);
      foreach (WikiHeader header in doc.headers) {
        ProcessHeader(header);
      }
      return true;
    }
  
    public void ProcessHeader(WikiContainer header) {
      List<string> newText = new List<string>();
      bool first = true;
      for (int i=0; i<header.text.Count; i++) {
        string row = header.text[i].TrimEnd();
        string trimmed = row.TrimStart();
        if (!String.IsNullOrEmpty(trimmed)) {
          if (strip.Contains(trimmed)) {
            row="";
            trimmed="";
          }
        }
        if (first && !String.IsNullOrEmpty(trimmed)) {
          first = false;
        }
        if (!first) {
          newText.Add(row);
        }
      }
      while ((newText.Count>0) && (String.IsNullOrEmpty(newText[newText.Count-1].Trim()))) {
        newText.RemoveAt(newText.Count-1);
      }
      header.text=newText;
    }
    #endregion Methods
    
  }

  public class ProcessorTemplateObj : WikiProcessor {

    #region Fields
    public WikiFixUp[] fixes;
    #endregion Fields

    #region Constructors
    public ProcessorTemplateObj(WikiFixUp[] fixes) {
      this.fixes = fixes;
    }
    #endregion Constructors

    #region Methods
    public bool Process(WikiDocument doc) {
      TagObj obj = new TagObj(doc);
      for (int i=0; i<fixes.Length; i++) {
        fixes[i].Process(doc, obj);
      }
      obj.Update();
      return true;
    }
    #endregion Methods

  }

  public class TagObj {
      #region Fields
      private static char SEP_NL = '\n';
      private static char[] SEP_VL = new char[] {'='};
      private static string TEMPLATEOBJ_BEGIN = "{{Template:Obj";
      private static string TEMPLATEOBJ_END = "}}";
      private static string TEMPLATEOBJ_SEP = "|";
      private int beginIndex = -1;
      private ListDictionary data;
      private WikiDocument doc;
      private int endIndex = -1;
      #endregion Fields

      #region Constructors
      public TagObj(WikiDocument doc) {
        this.doc = doc;
        ReadObj();
      }
      #endregion Constructors

      #region Indexers
      public string this[string name] {
        get {
          string val = (string)data[name];
          return val;
        }
        set {
          if (value==null) {
            data.Remove(name);
          }
          else {
            data[name] = value;
          }
        }
      }
      #endregion Indexers

      #region Methods
      public void ReadObj() {
        beginIndex = -1;
        endIndex = -1;
        data = new ListDictionary();
        int state = 0;
        string name = null;
        for (int i=0; i<doc.rowData.text.Count; i++) {
          string text = doc.rowData.text[i].Trim();
          switch (state) {
            case 0:
              if (text.Equals(TEMPLATEOBJ_BEGIN)) {
                state = 1;
                beginIndex = i;
                name = null;
              }
              break;
            case 1:
              if (text.Equals(TEMPLATEOBJ_END)) {
                TagClose(name);
                state = 2;
                endIndex = i;
              }
              else if (text.StartsWith(TEMPLATEOBJ_SEP)) {
                TagClose(name);
                name = TagOpen(text.Substring(1));
              }
              else {
                TagAdd(name, text);
              }
              break;
            case 2:
              name = null;
              break;
            }
          }
        }

      public override string ToString() {
        StringBuilder sb = new StringBuilder(256);
        sb.Append(TEMPLATEOBJ_BEGIN).Append(SEP_NL);
        foreach (string key in data.Keys) {
          string val = this[key];
          sb.Append(TEMPLATEOBJ_SEP).Append(key);
          if (!String.IsNullOrEmpty(val)) {
            sb.Append(SEP_VL).Append(val);
          }
          sb.Append(SEP_NL);
        }
        sb.Append(TEMPLATEOBJ_END).Append(SEP_NL);
        return sb.ToString();
      }

      public void Update() {
        if (beginIndex!=-1) {
          if (endIndex==-1) {
            endIndex = doc.rowData.text.Count-1;
          }
          doc.rowData.text.RemoveRange(beginIndex, endIndex - beginIndex + 1);
        }
        string str = ToString();
        if (beginIndex<0) {
          beginIndex = 0;
        }
        doc.rowData.text.Insert(beginIndex, str);
      }

      private void TagAdd(string name, string text) {
        if (name!=null) {
          string val = this[name];
          if (val!=null) {
            if (!String.IsNullOrEmpty(val)) {
              val = val + "\n";
            }
            val = val + text.Trim();
          }
          else {
            val = text.Trim();
          }
          data[name]=val;
        }
      }

      private void TagClose(string name) {
        if (name!=null) {
          object val = data[name];
          if (val!=null) {
            string newVal;
            newVal = val.ToString().Trim();
            if ("?".Equals(newVal)) {
              newVal="";
            }
            if (String.IsNullOrEmpty(newVal)) {
              data.Remove(name);
            }
          }
          else {
            data.Remove(name);
          }
        }
      }

      private string TagOpen(string text) {
        string name = null;
        string val = "";
        string[] flds = text.Split(SEP_VL);
        if (flds.Length>0) {
          name = flds[0].Trim();
        }
        if (flds.Length>1) {
          val = flds[1].Trim();
        }
        if (name!=null) {
          data.Add(name, val);
        }
        return name;
      }
      #endregion Methods
      
    }
  
}