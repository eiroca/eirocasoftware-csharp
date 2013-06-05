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
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  using DotNetWikiBot;

  using WikiHelper.lib.WikiMedia;

  /// <summary>
  /// Description of MainForm.
  /// </summary>
  public partial class MainForm : Form {
    #region Fields
    WikiConf conf;
    bool loggedIn = false;
    WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public MainForm(WikiMedia wiki, WikiConf conf) {
      this.wiki = wiki;
      this.conf = conf;
      InitializeComponent();
    }
    #endregion Constructors

    #region Delegates
    public delegate void Export(WikiMedia.ExportNotify notify);
    #endregion Delegates

    #region Methods

    public void Print(string msg) {
      if (!String.IsNullOrEmpty(msg)) {
        StringBuilder sb = new StringBuilder(meOut.Text.Length+200);
        sb.Append(msg);
        if (!msg.EndsWith("\n")) {
          sb.Append('\t');
        }
        sb.Append(meOut.Text);
        meOut.Text = sb.ToString();
        Application.DoEvents();
      }
    }

    void BuildPagesClick(object sender, EventArgs e) {
      if (!Login()) {
        return;
      }
      meOut.Clear();
      if (WikiTools.createForm.ShowDialog() == DialogResult.OK) {
        string template = File.ReadAllText(conf.newPage_Template, Encoding.UTF8);
        string[] lines = File.ReadAllLines(conf.newPage_List, Encoding.UTF8);
        wiki.CreatePages(template, lines, Print);
      }
    }

    void CloseClick(object sender, EventArgs e) {
      Close();
    }

    void ConfigurazioneClick(object sender, EventArgs e) {
      DotBits.Configuration.ConfigEditor c = new  DotBits.Configuration.ConfigEditor();
      c.ShowDialog(this);
    }

    void Do(Export action) {
      if (!Login()) {
        return;
      }
      meOut.Clear();
      action(Print);
    }

    void ExportAddressBookClick(object sender, System.EventArgs e) {
      ExportForm(WikiTools.addressBookExport);
    }

    void ExportCategoryClick(object sender, EventArgs e) {
      ExportForm(WikiTools.categoryExport);
    }

    void ExportForm(IExporter exporter) {
      if (!Login()) {
        return;
      }
      meOut.Clear();
      if (exporter.Setup()) {
        exporter.Export(Print);
      }
    }

    void ExportPageClick(object sender, EventArgs e) {
      ExportForm(WikiTools.pageExport);
    }

    bool Login() {
      if (conf.useLogin) {
        if (!loggedIn) {
          try {
            if (WikiTools.wikiConf.wikiDomain == null) {
              loggedIn = true;
              wiki.LogIn();
            }
            else {
              if (WikiTools.wikiLoginForm.ShowDialog() == DialogResult.OK) {
                string oldText = this.Text;
                this.Text = "WAITING ...";
                Application.DoEvents();
                wiki.LogIn();
                loggedIn = true;
                this.Text = oldText;
              }
            }
          }
          catch {
          }
        }
      }
      else {
        loggedIn = true;
        wiki.username=null;
        wiki.password=null;
        wiki.LogIn();
      }
      return loggedIn;
    }

    void ProcessPagesToolStripMenuItemClick(object sender, EventArgs e) {
      if (!Login()) {
        return;
      }
      meOut.Clear();
      DialogResult res = WikiTools.pageProcessForm.ShowDialog();
      if ((res == DialogResult.Yes)||(res == DialogResult.No)) {
        string[] pages = WikiTools.pageProcessForm.getPages();
        string[] opers = WikiTools.pageProcessForm.getOpers();

        string[,] mappingList = WikiConf.LoadPair(conf.PathMapping);
        string[,] replaces = WikiConf.LoadPair(conf.PathReplaces);
        
        string[] emptyAlias = WikiConf.LoadList(conf.PathAlias);
        
        string[] blacklist = WikiConf.LoadList(conf.PathBlackList);
        string[] whitelist = WikiConf.LoadList(conf.PathWhiteList);

        object[,] mapping = new object[mappingList.GetLength(0), 2];
        for (int i=0; i<mappingList.GetLength(0); i++) {
          mapping[i,0] = mappingList[i,0];
          mapping[i,1] = WikiConf.LoadPair(mappingList[i, 1]);
        };
        bool preview=false;
        bool save=false;
        if (res == DialogResult.Yes) {
          save = true;
        }
        else if (res == DialogResult.No) {
          preview = true;
        }
        WikiFixUp[] fixes = new WikiFixUp[3+mapping.GetLength(0)];
        fixes[0] = new FixUpObj();
        fixes[1] = new FixUpSummary();
        fixes[2] = new FixUpContact();
        for (int i=0; i<mappingList.GetLength(0); i++) {
          fixes[i+3] = new FixUpMapping((string)mapping[0, 0], (string[,])mapping[0, 1]);
        }
        WikiErrorCheck[] checks = new WikiErrorCheck[]{
          new CheckInvalidParagraph(blacklist),
          new CheckMissingParagraph(whitelist),
          new CheckContact()
        };
        WikiProcessor[] procs = new WikiProcessor[]{
          new ProcessorStandardEmpty(emptyAlias),
          new ProcessorRemoveEmpty(),
          new ProcessorTemplateObj(fixes),
          new ProcessorStandardEmpty(emptyAlias),
          new ProcessorRemoveEmpty()
        };
        wiki.Process(pages, replaces, procs, checks, preview, save, Print);
      }
    }

    void ReplacesClick(object sender, EventArgs e) {
      if (!Login()) {
        return;
      }
      meOut.Clear();
      if (WikiTools.replaceForm.ShowDialog() == DialogResult.OK) {
        string[,] replaces = WikiTools.replaceForm.GetReplaces();
        string category = WikiTools.replaceForm.GetCategory();
        if ((category!=null) && (replaces!=null)) {
          wiki.Replace(category, replaces, Print);
        }
      }
    }

    void TestToolStripMenuItemClick(object sender, EventArgs e) {
      Site import = new Site("http://www.startupcloud.it", null, null);
      Page main = new Page(import, "Startup_Cloud");
      PageList list = new PageList(import);
      main.Load();
      Regex rx1 = new Regex("\\[\\[(\\w*)]\\]\\s*(\\[\\[(.*)\\]\\])?\\s*(.*)", RegexOptions.Compiled|RegexOptions.Multiline);
      Regex rx2 = new Regex("<.*>(.*)<.*>", RegexOptions.Compiled);
      MatchCollection matches = rx1.Matches(main.text);
      foreach (Match match in matches) {
        GroupCollection groups = match.Groups;
        string name = groups[1].Value;
        string val = groups[4].Value;
        Match r = rx2.Match(val);
        if (r.Success) {
          val = r.Groups[1].Value;
        }
        Page p = new Page(import, name);
        list.Add(p);
        p.Load();
        meOut.Text += name + " "+ val +"\n";
        meOut.Text += p.text;
        break;
      }
    }
    #endregion Methods
    
  }
  
}