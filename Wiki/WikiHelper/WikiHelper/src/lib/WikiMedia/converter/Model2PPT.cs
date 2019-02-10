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

namespace WikiHelper.lib.WikiMedia.converter {
  using System;
  using System.IO;
  using System.Text;
  using System.Web;

  using DotNetWikiBot;
  using Microsoft.Office.Core;

  //Extract PPT with a summary taken form the document model
  public class ExtractSummary : SummaryBuilder {
    #region Fields
    object[] data;
    string extractorName;
    string[] names;
    #endregion Fields

    #region Constructors
    //Class to extract summary of document model
    public ExtractSummary(Model2PowerPoint converter, string template) {
      this.converter = converter;
      string[] lines = File.ReadAllLines(template, Encoding.UTF8);
      if (lines.Length < 2) {
        return;
      }
      extractorName = lines[0];
      names = new string[lines.Length];
      data = new object[lines.Length];
      char[] sep = { '\t' };
      for (int i = 1; i < lines.Length; i++) {
        string l = lines[i];
        string[] fld = l.Split(sep);
        string name = null;
        object datum = null;
        if (fld.Length >= 2) {
          try {
            datum = Int32.Parse(fld[0]);
          }
          catch {
            datum = fld[0];
          }
          name = fld[1];
        }
        else if (fld.Length >= 1) {
          datum = fld[0];
          name = fld[1];
        }
        names[i] = name;
        data[i] = datum;
      }
    }
    #endregion Constructors

    #region Methods
    //extract a summary form the document model and generate a slide into the presentation
    public override void Export(Presentation pres, Document doc, string title) {
      Header[] heads = new Header[data.Length];
      for (int i = 0; i < data.Length; i++) {
        if (data[i] != null) {
          if (data[i] is Int32) {
            Int32 id = (Int32)data[i];
            if (id < 0) {
              id += doc.headers.Count;
            }
            if ((id >= 0) && (id < doc.headers.Count)) {
              heads[i] = doc.headers[id];
            }
          }
          else {
            heads[i] = doc.FindHeader(data[i] as string);
          }
        }
      }
      ExportHeaders(pres, title, heads, names, false);
    }

    //Generate a slide with the given header element names
    public void ExportHeaders(Presentation pres, string title, Header[] heads, string[] names, bool firstSpecial) {
      Microsoft.Office.Interop.PowerPoint.Slide slide = pres.Add(Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText, title);
      Microsoft.Office.Interop.PowerPoint.TextRange textRange;
      Microsoft.Office.Interop.PowerPoint.TextRange tr1;
      textRange = slide.Shapes[2].TextFrame.TextRange;
      for (int i = 0; i < heads.Length; i++) {
        Header header = heads[i];
        if (!converter.IsEmptyHeader(header)) {
          tr1 = textRange.InsertAfter(names[i] + "\r");
          converter.AddHeader(textRange, header, firstSpecial, 0);
          tr1.Font.Bold = MsoTriState.msoTrue;
          tr1.ParagraphFormat.Bullet.Visible = MsoTriState.msoFalse;
        }
      }
    }

    public override string GetName() {
      return extractorName;
    }
    #endregion Methods

  }

  //Conver a model into a PowerPoint
  public class Model2PowerPoint {
    #region Fields
    public string[] SKIPS = { "", "-", "nessuno" };
    private string fBasePath;
    private string fTemplatePath;
    WikiMedia wiki;
    #endregion Fields

    #region Constructors
    public Model2PowerPoint(WikiMedia aWiki, string basePath, string templatePath) {
      this.wiki = aWiki;
      fBasePath = basePath;
      if (fBasePath.Length > 0) {
        if (fBasePath[fBasePath.Length - 1] != '\\') {
          fBasePath += "\\";
        }
      }
      fTemplatePath = templatePath;
    }
    #endregion Constructors

    #region Delegates
    public delegate void ExportProc(Presentation pres, Document doc, string title);
    #endregion Delegates

    #region Methods
    //Convert the first "not empty" header element in a document model into a TextRange
    //composed by an hyperlink to the page and a summary composed up to "size" charactes.
    public void AddFirstHeader(Microsoft.Office.Interop.PowerPoint.TextRange parent, Document doc, Page page, string title, int size) {
      Microsoft.Office.Interop.PowerPoint.TextRange tr1;
      tr1 = parent.InsertAfter("\r");
      tr1.IndentLevel = 1;
      Microsoft.Office.Interop.PowerPoint.Hyperlink link = tr1.ActionSettings[Microsoft.Office.Interop.PowerPoint.PpMouseActivation.ppMouseClick].Hyperlink;
      link.Address = wiki.site.site + wiki.site.indexPath + "index.php?title=" + HttpUtility.UrlEncode(page.title);
      link.TextToDisplay = title;
      tr1.InsertAfter(". ");
      int len = tr1.Text.Length;
      for (int i = 0; i < doc.headers.Count; i++) {
        Header header = doc.headers[i];
        if (!IsEmptyHeader(header)) {
          tr1.InsertAfter(Header2Text(header, 0, size - title.Length, true));
          break;
        }
      }
      tr1.InsertAfter("\r");
    }

    //Convert an header element into PowerPoint TextRage
    public void AddHeader(Microsoft.Office.Interop.PowerPoint.TextRange parent, Header header, bool firstSpecial, int maxElems) {
      Microsoft.Office.Interop.PowerPoint.TextRange tr = null;
      if ((maxElems < 1) || (maxElems > header.elements.Count)) {
        maxElems = header.elements.Count;
      }
      bool trim = true;
      for (int i = 0; i < maxElems; i++) {
        Element e = header.elements[i];
        if (e is NewLine) {
          trim = true;
          tr = parent.InsertAfter("\r");
          tr = null;
        }
        else {
          string row = e.ToString();
          if (trim) {
            row = row.TrimStart();
          }
          trim = false;
          int level = 1;
          if (!String.IsNullOrEmpty(row)) {
            if (e is ListItem) {
              level = (e as ListItem).level + 1;
              tr = parent.InsertAfter(row.Trim() + "\r");
              tr.IndentLevel = level;
              trim = true;
            }
            else if (e is HyperLink) {
              HyperLink link = e as HyperLink;
              if (tr == null) {
                tr = parent;
              }
              tr = tr.InsertAfter(" ");
              Microsoft.Office.Interop.PowerPoint.Hyperlink l = tr.ActionSettings[Microsoft.Office.Interop.PowerPoint.PpMouseActivation.ppMouseClick].Hyperlink;
              string url = link.URL;
              if (url != null) {
                if ((!url.StartsWith("http:")) || (!url.StartsWith("mailto:"))) {
                  url = wiki.site.site + url;
                }
                l.Address = url;
              }
              l.TextToDisplay = link.text.ToString();
            }
            else {
              tr = parent.InsertAfter(row);
              if (e is Text) {
                CopyFormatting(e as Text, tr);
              }
            }
          }
        }
      }
      if (firstSpecial) {
        if (header.elements.Count == 1) {
          tr.ParagraphFormat.Bullet.Visible = MsoTriState.msoFalse;
          tr.Text = '\t' + tr.Text;
        }
      }
    }

    public string BuildPath(string relPath) {
      string path = fBasePath + relPath + "\\";
      try {
        Directory.CreateDirectory(path);
      }
      catch {
        //
      }
      return path;
    }

    //Try to preserve Text element formatting into the PowerPoint TextRange
    public void CopyFormatting(Text t, Microsoft.Office.Interop.PowerPoint.TextRange tr) {
      if (t.bold) {
        tr.Font.Bold = MsoTriState.msoTrue;
      }
      else {
        tr.Font.Bold = MsoTriState.msoFalse;
      }
      if (t.italic) {
        tr.Font.Italic = MsoTriState.msoTrue;
      }
      else {
        tr.Font.Italic = MsoTriState.msoFalse;
      }
      if (t.underline) {
        tr.Font.Underline = MsoTriState.msoTrue;
      }
      else {
        tr.Font.Underline = MsoTriState.msoFalse;
      }
    }

    //Export all the pages in a Wiki category into a index presentation, each page
    //will contain up to expPagining summaries. A summary will be composed up to
    //descSize characters taken from the first Header of the Page
    public void ExportIndex(string expCat, string outFileName, int expPaging, int descSize, WikiMedia.ExportNotify expNotify) {
      Microsoft.Office.Interop.PowerPoint.Slide slide = null;
      Microsoft.Office.Interop.PowerPoint.TextRange textRange = null;
      Presentation pres = new Presentation(fTemplatePath);
      PageList pl = wiki.GetPages(expCat);
      int cnt = 0;
      foreach (Page page in pl) {
        string title = GetTitle(page);
        if (title == null) {
          continue;
        }
        cnt++;
        if ((cnt % expPaging) == 1) {
          slide = pres.Add(Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText, GetTitleName(expCat));
          textRange = slide.Shapes[2].TextFrame.TextRange;
        }
        if (expNotify != null) {
          expNotify(page.title);
        }
        page.LoadHTML();
        Document doc = HTML2Model.Convert(page.text);
        AddFirstHeader(textRange, doc, page, title, descSize);
      }
      pres.Save(fBasePath + outFileName);
      pres.Close();
    }

    //Export pages present in a Wiki Category
    public void ExportPages(string expCat, string expDir, WikiMedia.ExportNotify expNotify) {
      PageList pl = wiki.GetPages(expCat);
      string[] pages = new string[pl.Count()];
      for (int i = 0; i < pl.Count(); i++) {
        Page page = pl[i];
        pages[i] = GetTitle(page);
      }
      ExportPages(pages, expDir, expNotify);
    }

    //Export a set of Wiki pages into several Power Point slides (saved into export directory)
    public void ExportPages(string[] pages, string expDir, WikiMedia.ExportNotify expNotify) {
      Page page;
      Document doc;
      Presentation pres;
      string outPath = BuildPath(expDir);
      foreach (string title in pages) {
        if (String.IsNullOrEmpty(title)) {
          continue;
        }
        if (expNotify != null) {
          expNotify(title);
        }
        page = wiki.GetPage(title);
        page.LoadHTML();
        if (!String.IsNullOrEmpty(page.text)) {
          doc = HTML2Model.Convert(page.text);
          pres = new Presentation(fTemplatePath);
          Page2Slide(pres, doc, title, true);
          StringBuilder fileName = new StringBuilder(title.Length);
          for (int i = 0; i < title.Length; i++) {
            char c = title[i];
            if ("\\:/".IndexOf(c) == -1) {
              fileName.Append(c);
            }
          }
          pres.Save(outPath + fileName.ToString() + ".ppt");
          pres.Close();
        }
      }
    }

    //Export a summary Presentation with all the pages present in a category
    //The summar is composed by an index (optional) and a slide for each page
    //the slide content will be detemined by the expDetail delegate
    public void ExportSummary(string expCat, string outFileName, string expIdxName, int expPaging, ExportProc expDetail, WikiMedia.ExportNotify expNotify) {
      Presentation pres = new Presentation(fTemplatePath);
      PageList pl = wiki.GetPages(expCat);
      if (expIdxName != null) {
        Index2Slide(pres, pl, expIdxName, expPaging);
      }
      foreach (Page page in pl) {
        string title = GetTitle(page);
        if (title == null) {
          continue;
        }
        if (expNotify != null) {
          expNotify(page.title);
        }
        page.LoadHTML();
        Document doc = HTML2Model.Convert(page.text);
        expDetail(pres, doc, title);
      }
      pres.Save(fBasePath + outFileName);
      pres.Close();
    }

    public string GetTitle(Page page) {
      string title = page.title.Trim();
      if (title.StartsWith("Category:")) {
        title = null;
      }
      else {
        int pos = title.IndexOf(':');
        if (pos >= 0) {
          title = title.Substring(pos + 1);
        }
      }
      return title;
    }

    public string GetTitleName(string fullTitle) {
      int p = fullTitle.IndexOf(':');
      if (p != -1) {
        return fullTitle.Substring(p + 1);
      }
      return fullTitle;
    }

    //Convert an header element in a text of a maximum of "size" characters. If original
    //text is bigger than "size" only the text up to the last "." is taken, if is still
    //to much the text will be truncated and the last word (if possible) and "..." appended.
    public string Header2Text(Header header, int maxElems, int stopAt, bool strongLimit) {
      if (stopAt <= 0) {
        stopAt = Int32.MaxValue;
      }
      StringBuilder buf = new StringBuilder();
      bool trim = true;
      if ((maxElems < 1) || (maxElems > header.elements.Count)) {
        maxElems = header.elements.Count;
      }
      //Merge all the header elements into a single string
      for (int i = 0; (i < maxElems) && (buf.Length <= stopAt); i++) {
        Element e = header.elements[i];
        if (e is NewLine) {
          buf.Append(' ');
          trim = true;
          continue;
        }
        else if (e is HyperLink) {
          string row = (e as HyperLink).text.ToString();
          if (trim) {
            row = row.TrimStart();
          }
          trim = false;
          buf.Append(row);
          continue;
        }
        else {
          string row = e.ToString();
          if (trim) {
            row = row.TrimStart();
          }
          trim = false;
          if (!String.IsNullOrEmpty(row)) {
            buf.Append(row);
          }
        }
      }
      //Checks length
      if ((buf.Length > stopAt) && (strongLimit)) {
        // Up to last "."
        string res = buf.ToString();
        int p = res.LastIndexOf('.');
        while ((p >= 0) && (p > stopAt)) {
          p = res.LastIndexOf('.', p - 1);
        }
        if (p == -1) { //
          // Still too long, up to last " " and add "...";
          if (stopAt > 3) {
            p = res.LastIndexOf(' ', stopAt - 3);
            if (p == -1) {
              p = stopAt - 4;
            }
            res = res.Substring(0, p + 1) + "...";
          }
          else {
            res = res.Substring(0, stopAt);
          }
        }
        else {
          res = res.Substring(0, p + 1);
        }
        return res;
      }
      return buf.ToString();
    }

    //Create slides with the index of all the pages in a wiki category
    //Pages will be subdivided into two columns and paged in pageSize elements per page
    public void Index2Slide(Presentation pres, PageList pl, string indexName, int pageSize) {
      Microsoft.Office.Interop.PowerPoint.Slide idexSlide;
      Microsoft.Office.Interop.PowerPoint.TextRange index1 = null;
      Microsoft.Office.Interop.PowerPoint.TextRange index2 = null;
      Microsoft.Office.Interop.PowerPoint.TextRange index = null;
      int cnt = 0;
      bool first = true;
      foreach (Page page in pl) {
        string title = GetTitle(page);
        if (title == null) {
          continue;
        }
        if ((cnt % pageSize) == 0) {
          idexSlide = pres.Add(Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTwoColumnText, indexName);
          index1 = idexSlide.Shapes[2].TextFrame.TextRange;
          index2 = idexSlide.Shapes[3].TextFrame.TextRange;
          first = true;
        }
        if ((cnt % 2) == 0) {
          index = index1;
        }
        else {
          index = index2;
        }
        if (!first) {
          index.Text += Presentation.SEP;
        }
        index.Text += title;
        if ((cnt % 2) != 0) {
          first = false;
        }
        cnt++;
      }
    }

    //Check if a header element is "empty", that means with out sub elements or
    //only with a text that should be ignored (presents the SKIPS list)
    public bool IsEmptyHeader(Header header) {
      if ((header == null) || (header.elements.Count == 0)) {
        return true;
      }
      StringBuilder buf = new StringBuilder();
      for (int i = 0; i < header.elements.Count; i++) {
        buf.Append(header.elements[i].ToString());
      }
      bool empty = (buf.Length == 0);
      if (!empty) {
        string row = buf.ToString().Trim();
        for (int i = 0; i < SKIPS.Length; i++) {
          if (row.Equals(SKIPS[i])) {
            empty = true;
            break;
          }
        }
      }
      return empty;
    }

    // Converts a wiki page into a set of slide (one for each header)
    // Exclude header that have only a sentence presente in SKIPS
    public void Page2Slide(Presentation pres, Document doc, string title, bool skipEmpty) {
      Microsoft.Office.Interop.PowerPoint.Slide slide;
      Microsoft.Office.Interop.PowerPoint.TextRange tr;
      slide = pres.Add(Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitle, title);
      foreach (Header header in doc.headers) {
        if (skipEmpty) {
          if (IsEmptyHeader(header)) {
            continue;
          }
        }
        slide = pres.Add(Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText, header.name);
        tr = slide.Shapes[2].TextFrame.TextRange;
        AddHeader(tr, header, true, 0);
      }
    }
    #endregion Methods

  }

  // Convert Document Model into a PowerPoint
  //Abstract class for summary builder
  public abstract class SummaryBuilder {
    #region Fields
    protected Model2PowerPoint converter;
    #endregion Fields

    #region Methods
    public abstract void Export(Presentation pres, Document doc, string title);

    public virtual string GetName() {
      return this.GetType().ToString();
    }
    #endregion Methods

  }

}