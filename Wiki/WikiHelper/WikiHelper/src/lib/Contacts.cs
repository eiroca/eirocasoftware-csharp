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
  using System.IO;
  using System.Text;
  using System.Text.RegularExpressions;
  using DotNetWikiBot;
  using WikiHelper.lib.WikiMedia;

  /// <summary>
  /// Description of Class1.
  /// </summary>
  public class ContactsHelper {
    #region Fields
    public WikiMedia wiki;
    private const int IDX_COMPANY = 16;
    string[] defContatto = {
      /* 00 */ "Title",
      /* 01 */ "Cognome",
      /* 02 */ "Nome",
      /* 03 */ "Role",
      /* 04 */ "web",
      /* 05 */ "Address",
      /* 06 */ "mailto",
      /* 07 */ "Cell",
      /* 08 */ "Mob",
      /* 09 */ "Tel",
      /* 10 */ "Tel1",
      /* 11 */ "Tel2",
      /* 12 */ "Fax",
      /* 13 */ "MSN",
      /* 14 */ "skype",
      /* 15 */ "Tag"
    };
    int[] mapping;
    string[] outlook2contatto = {
      "Title", "Title",
      "First Name", "Nome",
      "Middle Name", "",
      "Last Name", "Cognome",
      "Suffix", "",
      "Company", "Company",
      "Department", "",
      "Job Title", "Role",
      "Business Street", "Address",
      "Business Street 2", "",
      "Business Street 3", "",
      "Business City", "",
      "Business State", "",
      "Business Postal Code", "",
      "Business Country", "",
      "Home Street", "",
      "Home Street 2", "",
      "Home Street 3", "",
      "Home City", "",
      "Home State", "",
      "Home Postal Code", "",
      "Home Country", "",
      "Other Street", "",
      "Other Street 2", "",
      "Other Street 3", "",
      "Other City", "",
      "Other State", "",
      "Other Postal Code", "",
      "Other Country", "",
      "Assistant's Phone", "",
      "Business Fax", "Fax",
      "Business Phone", "Tel1",
      "Business Phone 2", "Tel2",
      "Callback", "",
      "Car Phone", "",
      "Company Main Phone", "",
      "Home Fax", "",
      "Home Phone", "Tel",
      "Home Phone 2", "",
      "ISDN", "",
      "Mobile Phone", "Cell",
      "Other Fax", "",
      "Other Phone", "Mob",
      "Pager", "",
      "Primary Phone", "",
      "Radio Phone", "",
      "TTY/TDD Phone", "",
      "Telex", "",
      "Account", "",
      "Anniversary", "",
      "Assistant's Name", "",
      "Billing Information", "",
      "Birthday", "",
      "Business Address PO Box", "",
      "Categories", "",
      "Children", "",
      "Directory Server", "",
      "E-mail Address", "mailto",
      "E-mail Type", "",
      "E-mail Display Name", "",
      "E-mail 2 Address", "MSN",
      "E-mail 2 Type", "",
      "E-mail 2 Display Name", "",
      "E-mail 3 Address", "skype",
      "E-mail 3 Type", "",
      "E-mail 3 Display Name", "",
      "Gender", "",
      "Government ID Number", "",
      "Hobby", "",
      "Home Address PO Box", "",
      "Initials", "",
      "Internet Free Busy", "",
      "Keywords", "",
      "Language", "",
      "Location", "",
      "Manager's Name", "",
      "Mileage", "",
      "Notes", "",
      "Office Location", "",
      "Organizational ID Number", "",
      "Other Address PO Box", "",
      "Priority", "",
      "Private", "",
      "Profession", "",
      "Referred By", "",
      "Sensitivity", "",
      "Spouse", "",
      "User 1", "",
      "User 2", "",
      "User 3", "",
      "User 4", "",
      "Web Page", "web"
    };
    string outlookFormat;
    string outlookHeader;
    Regex[] rRules;
    #endregion Fields

    #region Constructors
    public ContactsHelper(WikiMedia aWiki) {
      this.wiki = aWiki;
      SetupContacts();
    }
    #endregion Constructors

    #region Methods
    public void Export(WikiMedia.ExportNotify notify, string category, string outPath) {
      FileStream outFile = new FileStream(outPath, FileMode.Create);
      StreamWriter writer = new StreamWriter(outFile);
      writer.WriteLine(outlookHeader);
      PageList pl = wiki.GetPages(category);
      foreach (Page page in pl) {
        page.Load();
        string text = page.text;
        // Decodifica Company
        int compBase = defContatto.Length;
        string[] cats = page.GetAllCategories(false);
        ArrayList comp = new ArrayList();
        for (int i = 0; i < cats.Length; i++) {
          if (!cats[i].Equals("Rubrica")) {
            comp.Add(cats[i]);
          }
        }
        if (comp.Count == 0) {
          comp.Add("");
        }
        string[] vals = new string[defContatto.Length + comp.Count];
        // Copia company
        for (int i = 0; i < comp.Count; i++) {
          vals[compBase + i] = comp[i].ToString();
        }
        // Estrai campi
        for (int i = 0; i < defContatto.Length; i++) {
          Match m = rRules[i].Match(text);
          if (m.Success) {
            vals[i] = m.Groups[1].Captures[0].ToString().Trim();
          }
          else {
            vals[i] = "";
          }
        }
        // FixUp
        if (String.IsNullOrEmpty(vals[7]) && !String.IsNullOrEmpty(vals[8])) {
          vals[7] = vals[8];
          vals[8] = "";
        }
        string[] tels = { "", "", "" };
        int telCnt = 0;
        if (!String.IsNullOrEmpty(vals[9])) {
          tels[telCnt] = vals[9];
          telCnt++;
        }
        if (!String.IsNullOrEmpty(vals[10])) {
          tels[telCnt] = vals[10];
          telCnt++;
        }
        if (!String.IsNullOrEmpty(vals[11])) {
          tels[telCnt] = vals[11];
          telCnt++;
        }
        if (telCnt == 1) {
          vals[9] = "";
          vals[10] = tels[0];
          vals[11] = "";
        }
        else if (telCnt == 2) {
          vals[9] = "";
          vals[10] = tels[0];
          vals[11] = tels[1];
        }
        // Export
        if (!String.IsNullOrEmpty(vals[1])) {
          notify(vals[1]);
          string row = String.Format(outlookFormat, vals);
          writer.WriteLine(row);
        }
      }
      writer.Close();
    }

    protected void SetupContacts() {
      rRules = new Regex[defContatto.Length];
      for (int i = 0; i < defContatto.Length; i++) {
        rRules[i] = new Regex(String.Format("[|]{0}=(.*)", defContatto[i]), RegexOptions.Compiled);
      }
      StringBuilder sbHeader = new StringBuilder();
      StringBuilder sbFormat = new StringBuilder();
      mapping = new int[outlook2contatto.Length / 2];
      for (int i = 0; i < outlook2contatto.Length; i += 2) {
        string oF = outlook2contatto[i];
        string rF = outlook2contatto[i + 1];
        if (i != 0) {
          sbHeader.Append(',');
          sbFormat.Append(',');
        }
        sbHeader.Append('"').Append(oF).Append('"');
        if (!String.IsNullOrEmpty(rF)) {
          int idx = -1;
          for (int j = 0; j < defContatto.Length; j++) {
            if (defContatto[j].Equals(rF)) {
              idx = j;
              break;
            }
          }
          if (rF.Equals("Company")) {
            idx = IDX_COMPANY;
          }
          if (idx >= 0) {
            sbFormat.Append("\"{").Append(idx).Append("}\"");
          }
        }
      }
      outlookHeader = sbHeader.ToString();
      outlookFormat = sbFormat.ToString();
    }
    #endregion Methods

  }

}