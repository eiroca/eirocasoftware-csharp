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
  using System.Collections;
  using System.Drawing;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  using DiffCalc;

  using DifferenceEngine;

  using DotNetWikiBot;

  /// <summary>
  /// Description of WikiMedia.
  /// </summary>
  public class WikiMedia {
    #region Fields
    public string defCategory;
    public string password;
    public Site site;
    public string username;
    public string WikiDomain;
    public string WikiURL;
    #endregion Fields

    #region Constructors
    public WikiMedia(string WikiURL, string WikiDomain, string aDefCategory) {
      this.WikiURL = WikiURL;
      this.WikiDomain = WikiDomain;
      // System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
      System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
        return true;
      };
      defCategory = aDefCategory;
    }
    #endregion Constructors

    #region Delegates
    public delegate void ExportNotify(string msg);
    #endregion Delegates

    #region Methods
    public void CreatePages(string template, string[] lines, WikiMedia.ExportNotify notify) {
      char[] sep = { '\t' };
      string[] val = null;
      for (int i = 0; i < lines.Length; i++) {
        string[] fld = lines[i].Split(sep);
        val = new string[fld.Length];
        string pageName = GetPageName(fld[0].Trim());
        string msg = pageName;
        if (!string.IsNullOrEmpty(pageName)) {
          int cnt = fld.Length;
          for (int j = 0; j < val.Length; j++) {
            string vl = null;
            if (j < fld.Length) {
              vl = fld[j];
            }
            if (vl == null) {
              vl = "";
            }
            val[j] = vl;
          }
          Page p = new Page(site, pageName);
          p.Load();
          if (!p.Exists()) {
            msg += " created";
            p.text = string.Format(template, val);
            p.Save();
          }
          else {
            msg += " skipped";
          }
          notify(msg);
        }
      }
    }

    public Page GetPage(string title) {
      return new Page(site, title);
    }

    public string GetPageName(string name) {
      if (name.IndexOf(':') == -1) {
        name = defCategory + ":" + name;
      }
      return name;
    }

    public PageList GetPages(string category) {
      PageList pl = new PageList(site);
      pl.FillAllFromCategoryEx(category);
      return pl;
    }

    public void LogIn() {
      if (username != null) {
        site = new Site(WikiURL, username, password, WikiDomain);
      }
      else {
        site = new Site(WikiURL, null, null);
      }
    }

    public void Process(string[] pages, string[,] replaces, WikiProcessor[] procs, WikiErrorCheck[] checks, bool preview, bool save, WikiMedia.ExportNotify notify) {
      Regex[] re = CalcRegex(replaces);
      foreach (string pageName in pages) {
        if (!String.IsNullOrEmpty(pageName)) {
          Page page = new Page(site, pageName);
          page.Load();
          if (!String.IsNullOrEmpty(page.text)) {
            string text = page.text.Trim();
            page.text = text;
            text = DoRegex(page.text, re, replaces);
            WikiDocument doc = new WikiDocument(pageName, text);
            foreach (WikiProcessor p in procs) {
              p.Process(doc);
            }
            foreach (WikiErrorCheck check in checks) {
              check.Process(doc, notify);
            }
            text = doc.ToString().Trim();
            if (!text.Equals(page.text)) {
              notify("W\t" + pageName + "\tchanged \n");
              if (preview) {
                char[] sep = { '\n' };
                IDiffList oldText = new DiffList_String(page.text, sep);
                IDiffList newText = new DiffList_String(text, sep);
                double time = 0;
                DiffEngine de = new DiffEngine();
                time = de.ProcessDiff(oldText, newText);
                ArrayList rep = de.DiffReport();
                Results dlg = new Results(oldText, newText, rep, time);
                dlg.Size = new Size(1000, 700);
                dlg.ShowInTaskbar = false;
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.ShowDialog();
                dlg.Dispose();
              }
              if (save) {
                try {
                  page.text = text;
                  page.Save();
                }
                catch (Exception e) {
                  notify("E\t" + pageName + "\t" + e.Message + "\n");
                }
              }
            }
            else {
              notify("N\t" + pageName + "\t\n");
            }
          }
          else {
            notify("I\t" + pageName + "\t\n");
          }
        }
      }
    }

    public void Replace(string category, string[,] replaces, WikiMedia.ExportNotify notify) {
      Regex[] re = CalcRegex(replaces);
      PageList pl = GetPages(category);
      foreach (Page page in pl) {
        page.Load();
        string text = DoRegex(page.text, re, replaces);
        if (!text.Equals(page.text)) {
          notify(page.title);
          page.text = text;
          page.Save();
        }
      }
    }

    private Regex[] CalcRegex(string[,] replaces) {
      int len = replaces.GetLength(0);
      Regex[] re = new Regex[len];
      for (int i = 0; i < len; i++) {
        re[i] = new Regex(replaces[i, 0], RegexOptions.Compiled | RegexOptions.Singleline);
      }
      return re;
    }

    private string DoRegex(string text, Regex[] re, string[,] replaces) {
      for (int i = 0; i < re.Length; i++) {
        text = re[i].Replace(text, replaces[i, 1]);
      }
      return text;
    }
    #endregion Methods

  }

}