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
  using System.Text;

  using HTML;

  /**
   * Convert a HTML pages created with WikiMedia and Modern Style into a
   * object model that could be processed by a coputer
   */
  public class HTML2Model {
    #region Fields
    public const string ATR_CLASS = "class";
    public const string ATR_HREF = "href";
    public const string ATR_ID = "id";
    public const string ATR_NAME = "name";
    public const string CAT_LINKS = "catlinks";

    //marker to the content in order to exclude header/footer in a safe and fast way
    public const string MARKER_CONTENTBEGIN = "<!-- BEGIN-CONTENT -->";
    public const string MARKER_CONTENTEND = "<!-- END-CONTENT -->";
    public const string TAG_A = "a";
    public const string TAG_B = "b";
    public const string TAG_BR = "br";
    public const string TAG_DD = "dd";
    public const string TAG_DIV = "div";
    public const string TAG_H = "h";
    public const string TAG_I = "i";
    public const string TAG_LI = "li";
    public const string TAG_P = "p";

    // TAG to be converted
    public const string TAG_REMARK = "!--";
    public const string TAG_SCRIPT = "script";
    public const string TAG_SPAN = "span";
    public const string TAG_TABLE = "table";
    public const string TAG_U = "u";
    public const string VAL_EDIT = "editsection";
    public const string VAL_HEADLINE = "mw-headline";
    public const string VAL_TOC = "toc";

    // style class that could be safly ingnored in the conversion
    public string[] IGNORECLASS = {
      "mw-topboxes",
      "printfooter",
      "contentSub",
      "jump-to-nav",
      "printfooter",
      "mw_clear"
    };

    private const int ST_NORMAL = 0;
    private const int ST_TITLE = 1;

    private Document doc = null;
    private Header header = null;
    private HTMLParser parser;
    private int state = 0;
    private Text txt;
    #endregion Fields

    #region Constructors
    private HTML2Model(string html) {
      doc = new Document();
      txt = new Text();
      state = 0;
      Parse(ExtractContent(html));
    }
    #endregion Constructors

    #region Methods
    //Convert an HTML page into a document Model
    public static Document Convert(string html) {
      HTML2Model conv = new HTML2Model(html);
      return conv.doc;
    }

    //Convert a Tag into a model elemtn
    public void AddElement(Tag tag) {
      string name = tag.Name.ToLower();
      //Convert <h> tag into Header element
      if (name.StartsWith(TAG_H) && (name.Length == 2)) {
        if (tag.Opening) {
          int level = 0;
          Int32.TryParse(name.Substring(1), out level);
          header = new Header(doc);
          header.level = level;
          doc.headers.Add(header);
        }
      }
      //Convert <p> tag into Text element and NewLine element
      else if (name.Equals(TAG_P)) {
        txt.text = new StringBuilder(txt.text.ToString().TrimEnd());
        Flush();
        if (tag.Opening) {
          txt.format = 0;
        }
        else {
          AddIt(new NewLine());
        }
      }
      //Convert <span> with the Title into Header title
      else if (name.Equals(TAG_SPAN)) {
        if (tag.Opening) {
          string val = GetValue(tag, ATR_CLASS);
          if ((val != null) && (val.Equals(VAL_HEADLINE))) {
            state = ST_TITLE;
            Flush();
          }
        }
        if (tag.Closing) {
          // TODO recursive span check
          if (state == ST_TITLE) {
            state = ST_NORMAL;
            string res = null;
            if ((txt != null) && (txt.text.Length > 0)) {
              res = txt.ToString().Trim();
              header.title.Add(new Text(res));
            }
            txt = new Text();
          }
        }
      }
      // Convert <dd> tag into ListItem element
      //TODO Handling of nested element
      else if (name.Equals(TAG_DD)) {
        Flush();
        if (tag.Opening) {
          txt = new ListItem();
        }
      }
      // Convert <li> tag into ListItem element
      //TODO Handling of nested element
      else if (name.Equals(TAG_LI)) {
        Flush();
        if (tag.Opening) {
          txt = new ListItem();
        }
      }
      // Convert <br> tag into NewLine element
      else if (name.Equals(TAG_BR)) {
        if (tag.Closing) {
          Flush();
        }
        AddIt(new NewLine());
      }
      // Convert <a> tag into HyperLink element
      else if (name.Equals(TAG_A)) {
        Flush();
        if (tag.Opening) {
          txt = new HyperLink();
          string link = GetValue(tag, ATR_HREF);
          ((HyperLink)txt).URL = link;
        }
      }
      // Convert <i> tag Text style variation
      else if (name.Equals(TAG_I)) {
        int fmt = txt.format;
        Flush();
        if (tag.Opening) {
          txt.format = fmt | Text.FMT_ITALIC;
        }
        else {
          txt.format = fmt & ~Text.FMT_ITALIC;
        }
      }
      // Convert <b> tag Text style variation
      else if (name.Equals(TAG_B)) {
        int fmt = txt.format;
        Flush();
        if (tag.Opening) {
          txt.format = fmt | Text.FMT_BOLD;
        }
        else {
          txt.format = fmt & ~Text.FMT_BOLD;
        }
      }
      // Convert <u> tag Text style variation
      else if (name.Equals(TAG_U)) {
        int fmt = txt.format;
        Flush();
        if (tag.Opening) {
          txt.format = fmt | Text.FMT_UNDERLINE;
        }
        else {
          txt.format = fmt & ~Text.FMT_UNDERLINE;
        }
      }
      else {
        if (tag.IsCollapsed) {
          ;
        }
        if (tag.Opening) {
          Flush();
        }
        if (tag.Closing) {
          ;
        }
      }
    }

    //Add an element into the current model
    public void AddIt(Element e) {
      if (header != null) {
        header.elements.Add(e);
      }
      else {
        doc.elements.Add(e);
      }
    }

    // Extract the content from the page, content is the text between
    // begin and end marker (MARKER_CONTENT) or the whole page if they are absent
    public string ExtractContent(string html) {
      int start = html.IndexOf(MARKER_CONTENTBEGIN);
      if (start < 0) {
        start = 0;
      }
      else {
        start += MARKER_CONTENTBEGIN.Length;
      }
      int end = html.IndexOf(MARKER_CONTENTEND);
      if (end < 0) {
        end = html.Length;
      }
      return html.Substring(start, end - start);
    }

    // Add current Text (if present) into the document model
    public void Flush() {
      if ((txt != null) && (txt.text.Length > 0)) {
        AddIt(txt);
      }
      txt = new Text();
    }

    //Get the value of an attribute of the tag or null if absent
    public string GetValue(Tag tag, string attributeName) {
      HTML.Attribute id = tag[attributeName];
      string val = (id != null ? id.Value : null);
      return val;
    }

    // Check if an HTML TAG could be ignored (e.g. scripts, remarks, ...)
    public Tag Ignorable(Tag tag) {
      Tag res = tag;
      string name = tag.Name.ToLower();
      // Remove comments
      if (name.StartsWith(TAG_REMARK)) {
        res = null;
      }
      // Remove Scripts
      else if (name.Equals(TAG_SCRIPT)) {
        SkipToEnd(TAG_SCRIPT);
        res = null;
      }
      // Remove Table of Contents
      else if (name.Equals(TAG_TABLE)) {
        string val = GetValue(tag, ATR_ID);
        if ((val != null) && (val.Equals(VAL_TOC))) {
          SkipToEnd(TAG_TABLE);
          res = null;
        }
      }
      // Remove unuseful div section
      else if (name.Equals(TAG_DIV)) {
        string val = GetValue(tag, ATR_CLASS);
        if (val == null) {
          val = GetValue(tag, ATR_ID);
        }
        if (val != null) {
          bool ignored = false;
          for (int i = 0; i < IGNORECLASS.Length; i++) {
            if (val.Equals(IGNORECLASS[i])) {
              SkipToEnd(TAG_DIV);
              ignored = true;
              break;
            }
          }
          if (val.Equals(CAT_LINKS)) {
            //TODO gestione Categorie
            SkipToEnd(TAG_DIV);
            ignored = true;
          }
          if (ignored) {
            res = null;
          }
        }
      }
      // Remove Name place anchor
      else if (name.Equals(TAG_A)) {
        string val = GetValue(tag, ATR_NAME);
        if ((val != null)) {
          SkipToEnd(TAG_A);
          res = null;
        }
      }
      // Remove edit action
      else if (name.Equals(TAG_SPAN)) {
        string val = GetValue(tag, ATR_CLASS);
        if ((val != null) && (val.Equals(VAL_EDIT))) {
          SkipToEnd(TAG_SPAN);
          res = null;
        }
      }
      return res;
    }

    // Parse the HTML page
    public void Parse(string html) {
      Tag tag;
      Object ch;
      parser = new HTMLParser();
      parser.Source = html;
      while (!parser.Eof()) {
        ch = parser.Next();
        if (ch is Tag) {
          tag = ch as Tag;
          tag = Ignorable(tag);
          if (tag != null) {
            AddElement(tag);
          }
        }
        else {
          string s = ch as string;
          if (s.Length > 0) {
            txt.Append(s);
          }
        }
      }
      Flush();
    }

    //Skip text until the close of a tag, handle also nested tags
    public void SkipToEnd(string tagName) {
      Object ch;
      Tag tag;
      tagName = tagName.ToLower();
      int level = 1;
      while (!parser.Eof()) {
        ch = parser.Next();
        if (ch is Tag) {
          tag = ch as Tag;
          if (tag.Name.ToLower().Equals(tagName)) {
            if (tag.Opening) {
              level++;
            }
            if (tag.Closing) {
              level--;
              if (level == 0) {
                break;
              }
            }
          }
        }
      }
    }
    #endregion Methods

  }

}