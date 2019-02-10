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

namespace WikiHelper {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Text;

  public class WikiConf {
    #region Fields
    private System.Collections.Specialized.NameValueCollection settings;
    #endregion Fields

    #region Constructors
    public WikiConf() {
      settings = ConfigurationManager.AppSettings;
    }
    #endregion Constructors

    #region Properties
    public string[] categories {
      get {
        string cat = Get("categories");
        string[] cats = (String.IsNullOrEmpty(cat) ? new string[0] : cat.Split(','));
        for (int i = 0; i < cats.Length; i++) {
          cats[i] = cats[i].Trim();
        }
        return cats;
      }
    }

    public string BasePath {
      get {
        string basePath = Get("BasePath", ".\\");
        if (!basePath.EndsWith("\\")) {
          basePath = basePath + "\\";
          settings["BasePath"] = basePath;
        }
        return basePath;
      }
    }

    public string PathMapping {
      get {
        return GetPath("PathMapping", "mapping.txt");
      }
    }

    public string PathReplaces {
      get {
        return GetPath("PathReplaces", "data\\replaces.txt");
      }
    }

    public string PathAlias {
      get {
        return GetPath("PathAlias", "alias.txt");
      }
    }

    public string PathBlackList {
      get {
        return GetPath("PathBlackList", "blacklist.txt");
      }
    }

    public string PathWhiteList {
      get {
        return GetPath("PathWhiteList", "whitelist.txt");
      }
    }

    public string extractors {
      get {
        return GetPath("extractors", "extractors");
      }
    }

    public string loginConf {
      get {
        return GetPath("loginConf", "login.dat");
      }
    }

    public string newPage_List {
      get {
        return GetPath("newPage_List", "data\\newpages.txt");
      }
    }

    public string newPage_Template {
      get {
        return GetPath("newPage_Template", "data\\template.txt");
      }
    }

    public string pptTemplate {
      get {
        return GetPath("PPT_Template", "template.pot");
      }
    }

    public bool useLogin {
      get {
        string useLoginStr = Get("useLogin");
        return (String.IsNullOrEmpty(useLoginStr) ? true : (!useLoginStr.Equals("0") ? true : false));
      }
    }

    public string wikiDefCategory {
      get {
        return Get("wikiDefCategory");
      }
    }

    public string wikiDomain {
      get {
        return Get("wikiDomain");
      }
    }

    public string wikiURL {
      get {
        return Get("wikiURL");
      }
    }
    #endregion Properties

    #region Methods

    public string GetPath(string prop, string def) {
      string path = Get(prop, def);
      return (path != null ? BasePath + path : null);
    }

    public static string[] LoadList(string filename) {
      string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
      ArrayList validLines = new ArrayList();
      for (int i = 0; i < lines.Length; i++) {
        if (!String.IsNullOrEmpty(lines[i])) {
          validLines.Add(lines[i]);
        }
      }
      string[] res = new string[validLines.Count];
      for (int i = 0; i < validLines.Count; i++) {
        res[i] = (string)validLines[i];
      }
      return res;
    }

    public static string[,] LoadPair(string filename) {
      char[] sep = { '\t' };
      string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
      string[] val;
      ArrayList validLines = new ArrayList();
      for (int i = 0; i < lines.Length; i++) {
        string l = lines[i];
        val = l.Split(sep);
        if (val.Length == 2) {
          validLines.Add(val);
        }
      }
      string[,] res = new string[validLines.Count, 2];
      for (int i = 0; i < validLines.Count; i++) {
        val = (string[])validLines[i];
        res[i, 0] = val[0];
        res[i, 1] = val[1];
      }
      return res;
    }

    public static void StorePair(string filename, string[,] data) {
      if (data != null) {
        string[] lines = new string[data.GetLength(0)];
        for (int i = 0; i < data.GetLength(0); i++) {
          lines[i] = data[i, 0] + "\t" + data[i, 1];
        }
        File.WriteAllLines(filename, lines);
      }
    }

    public static void StorePair(string filename, Dictionary<string, string> data) {
      if (data != null) {
        string[] lines = new string[data.Count];
        int i = 0;
        foreach (string key in data.Keys) {
          lines[i] = key + "\t" + data[key];
          i++;
        }
        File.WriteAllLines(filename, lines);
      }
    }

    private string Get(string key) {
      string val = settings[key];
      if (String.IsNullOrEmpty(val)) {
        val = null;
      }
      return val;
    }

    private string Get(string key, string def) {
      string val = settings[key];
      if (String.IsNullOrEmpty(val)) {
        val = def;
      }
      return val;
    }
    #endregion Methods

  }

}