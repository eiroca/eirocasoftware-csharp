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

namespace HTML {
  using System;
  using System.IO;
  using System.Net;
  using System.Text;

  /// <summary>
  /// FindLinks is a class that will test the HTML parser.
  /// This short example will prompt for a URL and then
  /// scan that URL for links.
  /// This source code may be used freely under the
  /// Limited GNU Public License(LGPL).
  ///
  /// Written by Jeff Heaton (http://www.jeffheaton.com)
  /// </summary>
  class FindLinks {
    #region Methods
    public static string GetPage(string url) {
      WebResponse response = null;
      Stream stream = null;
      StreamReader reader = null;
      try {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        response = request.GetResponse();
        stream = response.GetResponseStream();
        if (!response.ContentType.ToLower().StartsWith("text/")) {
          return null;
        }

        StringBuilder buffer = new StringBuilder();
        string line;
        reader = new StreamReader(stream);
        while ((line = reader.ReadLine()) != null) {
          buffer.AppendLine(line);
        }
        return buffer.ToString();
      }
      catch (WebException e) {
        System.Console.WriteLine("Can't download:" + e);
        return null;
      }
      catch (IOException e) {
        System.Console.WriteLine("Can't download:" + e);
        return null;
      }
      finally {
        if (reader != null) {
          reader.Close();
        }

        if (stream != null) {
          stream.Close();
        }

        if (response != null) {
          response.Close();
        }
      }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Run(string[] args) {
      System.Console.Write("Enter a URL address:");
      string url = System.Console.ReadLine();
      System.Console.WriteLine("Scanning hyperlinks at: " + url);
      string page = GetPage(url);
      if (page == null) {
        System.Console.WriteLine("Can't process that type of file, please specify an HTML file URL.");
        return;
      }
      HTMLParser parser = new HTMLParser();
      parser.Source = page;
      while (!parser.Eof()) {
        Object ch = parser.Next();
        if (ch is Tag) {
          Tag tag = ch as Tag;
          if (tag["href"] != null) {
            System.Console.WriteLine("Found link: " + tag["href"].Value);
          }
        }
      }
    }
    #endregion Methods

  }

}