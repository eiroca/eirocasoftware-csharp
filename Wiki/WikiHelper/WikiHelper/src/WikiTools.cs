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
  using System.IO;
  using System.Text;
  using System.Windows.Forms;

  using WikiHelper.gui;
  using WikiHelper.lib.WikiMedia;
  using WikiHelper.lib.WikiMedia.converter;

  internal sealed class WikiTools {
    #region Fields
    public static AddressBookExport addressBookExport;
    public static CategoryExport categoryExport;
    public static ContactsHelper contatti;
    public static Model2PowerPoint converter;
    public static CreateForm createForm;
    public static SummaryBuilder[] extractors;
    public static MainForm mainForm;
    public static PageExport pageExport;
    public static PageProcess pageProcessForm;
    public static ReplaceForm replaceForm;
    public static WikiMedia wiki;
    public static WikiConf wikiConf;
    public static WikiLoginForm wikiLoginForm;
    #endregion Fields

    #region Methods
    [STAThread]
    private static void Main(string[] args) {
      // Configuration dirs
      string basePath;
      basePath = Directory.GetCurrentDirectory()+"\\";
      wikiConf = new WikiConf();
      wiki = new WikiMedia(wikiConf.wikiURL, wikiConf.wikiDomain, wikiConf.wikiDefCategory);
      contatti = new ContactsHelper(wiki);
      converter = new Model2PowerPoint(wiki, basePath, basePath + wikiConf.pptTemplate);
      string[] extNames = Directory.GetFiles(basePath+wikiConf.extractors);
      extractors = new SummaryBuilder[extNames.Length];
      for(int i=0; i<extNames.Length; i++) {
        extractors[i] = new ExtractSummary(converter, extNames[i]);
      }
      string[] lines;
      if (File.Exists(wikiConf.loginConf) == true) {
        lines = File.ReadAllLines(wikiConf.loginConf, Encoding.UTF8);
      }
      else {
        lines = new string[2] {null, null};
      }
      //GUI Init
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      wikiLoginForm = new WikiLoginForm(wiki, lines[0], lines[1]);
      mainForm = new MainForm(wiki, wikiConf);
      categoryExport = new CategoryExport(wiki, converter, extractors);
      categoryExport.SetCategories(wikiConf.categories);
      addressBookExport = new AddressBookExport(wiki, contatti);
      addressBookExport.SetCategories(wikiConf.categories);
      pageExport = new PageExport(wiki, converter);
      replaceForm = new ReplaceForm(wikiConf);
      createForm = new CreateForm(wikiConf);
      pageProcessForm = new PageProcess(wikiConf, wiki);
      //GO
      Application.Run(mainForm);
    }
    #endregion Methods
    
  }
  
}