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
  using System.Net;
  using System.Web;

  using DotNetWikiBot;

  /// <summary>
  /// Description of WikiBot.
  /// </summary>
  public class WikiBot : Page {
    
    #region Constructors
    public WikiBot() {
    }
    #endregion Constructors

    #region Methods
    public void LoadHTML() {
      if (string.IsNullOrEmpty(title)) throw new WikiBotException(Bot.Msg("No title specified for page to load."));
        Uri res = new Uri(site.site + site.indexPath + "index.php?title=" +HttpUtility.UrlEncode(title) + (string.IsNullOrEmpty(lastRevisionID) ? "" : "&oldid=" + lastRevisionID) + "&redirect=no&dontcountme=s");
        try {
          text = site.GetPageHTM(res.ToString());
        }
        catch (WebException e) {
          string message = Bot.isRunningOnMono ? e.InnerException.Message : e.Message;
          if (message.Contains(": (404) ")) {		// Not Found
            Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), title);
            text = "";
            return;
          }
          else throw;
        }
        Console.WriteLine(Bot.Msg("Page \"{0}\" loaded successfully."), title);
    }
    #endregion Methods
    
  }
  
}