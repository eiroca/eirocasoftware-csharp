/* GPL > 3.0
Copyright (C) 1996-2008 eIrOcA Enrico Croce & Simona Burzio

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

using MySql.Data.MySqlClient;

namespace Reporting.Exort {
  
  public class Piwik  {
    
    private MySqlConnection connection=null;

    private MySqlCommand findAction;
    private MySqlCommand addAction;
    private MySqlCommand findVisit;
    private MySqlCommand addVisit;
    private MySqlCommand findHit;
    private MySqlCommand addHit;

    string connectionString=null;
    
    public Piwik(string host, string database, string userId, string password) {
      connectionString= 
        "Server="+host+";"+
        "Database="+database+";"+
        "allow user variables=true;"+
        "Uid="+userId+";"+
        "Pwd="+password+";";

      // Action
      connection=new MySqlConnection(connectionString);
      findAction = new MySqlCommand();
      findAction.Connection = connection;
      findAction.CommandText = "SELECT idaction FROM piwik_log_action WHERE name = @action AND type = @type";
      findAction.Parameters.Add("@action", MySqlDbType.String).Value = "";
      findAction.Parameters.Add("@type", MySqlDbType.Int32).Value = 1;
      addAction = new MySqlCommand();
      addAction.Connection = connection;
      addAction.CommandText = "INSERT INTO piwik_log_action(name, type) VALUES (@action, @type); SELECT LAST_INSERT_ID() AS ID;";
      addAction.Parameters.Add("@action", MySqlDbType.String).Value = "";
      addAction.Parameters.Add("@type", MySqlDbType.Int32).Value = 1;
      
      // Hit
      findHit = new MySqlCommand();
      findHit.Connection = connection;
      findHit.CommandText = "SELECT idlink_va FROM piwik_log_link_visit_action WHERE idvisit = @idvisit AND idaction = @idaction AND idaction_ref = @idaction_ref";
      findHit.Parameters.Add("@idvisit", MySqlDbType.Int32).Value = 0;
      findHit.Parameters.Add("@idaction", MySqlDbType.Int32).Value = 0;
      findHit.Parameters.Add("@idaction_ref", MySqlDbType.Int32).Value = 0;
      addHit = new MySqlCommand();
      addHit.Connection = connection;
      addHit.CommandText = "INSERT INTO piwik_log_link_visit_action(idvisit, idaction, idaction_ref, time_spent_ref_action) VALUES (@idvisit, @idaction, @idaction_ref, @time_spent_ref_action); SELECT LAST_INSERT_ID() AS ID;";
      addHit.Parameters.Add("@idvisit", MySqlDbType.Int32).Value = 0;
      addHit.Parameters.Add("@idaction", MySqlDbType.Int32).Value = 0;
      addHit.Parameters.Add("@idaction_ref", MySqlDbType.Int32).Value = 0;
      addHit.Parameters.Add("@time_spent_ref_action", MySqlDbType.UInt32).Value = 0;

      // Visit
      findVisit = new MySqlCommand();
      findVisit.Connection = connection;
      findVisit.CommandText = "SELECT idvisit FROM piwik_log_visit WHERE (config_md5config = @user)";
      findVisit.Parameters.Add("@user", MySqlDbType.String).Value = "";
      addVisit = new MySqlCommand();
      addVisit.Connection = connection;
      addVisit.CommandText = "INSERT INTO" +
        " piwik_log_visit(idsite, visitor_localtime, visitor_idcookie, visitor_returning, visit_first_action_time, visit_last_action_time, visit_server_date, visit_entry_idaction, visit_exit_idaction, visit_total_actions, visit_total_time, visit_goal_converted, referer_type, referer_name, referer_url, referer_keyword, config_md5config, config_os, config_browser_name, config_browser_version, config_resolution, config_pdf, config_flash, config_java, config_director, config_quicktime, config_realplayer, config_windowsmedia, config_cookie, location_ip, location_browser_lang, location_country) " +
        "VALUES          (@idsite, @visitor_localtime, @visitor_idcookie, @visitor_returning, @visit_first_action_time, @visit_last_action_time, @visit_server_date, @visit_entry_idaction, @visit_exit_idaction, @visit_total_actions, @visit_total_time, @visit_goal_converted, @referer_type, @referer_name, @referer_url, @referer_keyword, @config_md5config, @config_os, @config_browser_name, @config_browser_version, @config_resolution, @config_pdf, @config_flash, @config_java, @config_director, @config_quicktime, @config_realplayer, @config_windowsmedia, @config_cookie, @location_ip, @location_browser_lang, @location_country); SELECT LAST_INSERT_ID() AS ID;";
      addVisit.Parameters.Add("@idsite", MySqlDbType.Int32).Value = 1;
      addVisit.Parameters.Add("@visitor_localtime", MySqlDbType.Time).Value = DateTime.Now - DateTime.Today;
      addVisit.Parameters.Add("@visitor_idcookie", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@visitor_returning", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@visit_first_action_time", MySqlDbType.DateTime).Value = DateTime.Now;
      addVisit.Parameters.Add("@visit_last_action_time", MySqlDbType.DateTime).Value = DateTime.Now;
      addVisit.Parameters.Add("@visit_server_date", MySqlDbType.DateTime).Value = DateTime.Today;
      addVisit.Parameters.Add("@visit_entry_idaction", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@visit_exit_idaction", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@visit_total_actions", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@visit_total_time", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@visit_goal_converted", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@referer_type", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@referer_name", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@referer_url", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@referer_keyword", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_md5config", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_os", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_browser_name", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_browser_version", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_resolution", MySqlDbType.String).Value = "";
      addVisit.Parameters.Add("@config_pdf", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_flash", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_java", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_director", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_quicktime", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_realplayer", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_windowsmedia", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@config_cookie", MySqlDbType.Int32).Value = 0;
      addVisit.Parameters.Add("@location_ip", MySqlDbType.UInt32).Value = 0;
      addVisit.Parameters.Add("@location_browser_lang", MySqlDbType.String).Value = "en";
      addVisit.Parameters.Add("@location_country", MySqlDbType.String).Value = "it";
    }
    
    public void Open() {
      connection.Open();
    }
    
    public void Close() {
      connection.Close();
    }

    public int AddHit(int idVisit, int idAction, Int32 idRefer, uint time) {
      int result = 0;
      findHit.Parameters["@idvisit"].Value = idVisit;
      findHit.Parameters["@idaction"].Value = idAction;
      findHit.Parameters["@idaction_ref"].Value = idRefer;
      Object res = findHit.ExecuteScalar();
      if (res!=null) {
        result = Int32.Parse(res.ToString());
      }
      else {
        addHit.Parameters["@idvisit"].Value = idVisit;
        addHit.Parameters["@idaction"].Value = idAction;
        addHit.Parameters["@idaction_ref"].Value = idRefer;
        addHit.Parameters["@time_spent_ref_action"].Value = time;
        res = addHit.ExecuteScalar();
        result = Int32.Parse(res.ToString());
      }
      return result;
    }

    public int FindAction(string action) {
      int result = 0;
      findAction.Parameters["@action"].Value = action;
      findAction.Parameters["@type"].Value = 1;
      Object res = findAction.ExecuteScalar();
      if (res!=null) {
        result = Int32.Parse(res.ToString());
      }
      else {
        addAction.Parameters["@action"].Value = action;
        addAction.Parameters["@type"].Value = 1;
        res = addAction.ExecuteScalar();
        result = Int32.Parse(res.ToString());
      }
      return result;
    }
    
    public int FindVisit(Session s, Hit actStr, Hit actEnd) {
      int result = 0;
      findVisit.Parameters["@user"].Value = s.user;
      Object res = findVisit.ExecuteScalar();
      if (res!=null) {
        result = Int32.Parse(res.ToString());
      }
      else {
        addVisit.Parameters["@idsite"].Value = 1;
        addVisit.Parameters["@visitor_localtime"].Value = actStr.when.TimeOfDay;
        addVisit.Parameters["@visitor_idcookie"].Value = s.user;
        addVisit.Parameters["@visitor_returning"].Value = 0;
        addVisit.Parameters["@visit_first_action_time"].Value = actStr.when;
        addVisit.Parameters["@visit_last_action_time"].Value = actEnd.when;
        addVisit.Parameters["@visit_server_date"].Value = actStr.when.Date;
        addVisit.Parameters["@visit_entry_idaction"].Value = FindAction(actStr.reqURI.ToString());
        addVisit.Parameters["@visit_exit_idaction"].Value = FindAction(actEnd.reqURI.ToString());
        addVisit.Parameters["@visit_total_actions"].Value = s.hits.Count;
        addVisit.Parameters["@visit_total_time"].Value = 10;
        addVisit.Parameters["@visit_goal_converted"].Value = 0;
        addVisit.Parameters["@referer_type"].Value = 0;
        addVisit.Parameters["@referer_name"].Value = "";
        if (actStr.refer!=null) {
          addVisit.Parameters["@referer_url"].Value = actStr.refer.ToString();
        }
        else {
          addVisit.Parameters["@referer_url"].Value = "";
        }
        addVisit.Parameters["@referer_keyword"].Value = "";
        addVisit.Parameters["@config_md5config"].Value = s.user;
        addVisit.Parameters["@config_os"].Value = "";
        addVisit.Parameters["@config_browser_name"].Value = s.userAgent;
        addVisit.Parameters["@config_browser_version"].Value = "";
        addVisit.Parameters["@config_resolution"].Value = "";
        addVisit.Parameters["@config_pdf"].Value = 0;
        addVisit.Parameters["@config_flash"].Value = 0;
        addVisit.Parameters["@config_java"].Value = 0;
        addVisit.Parameters["@config_director"].Value = 0;
        addVisit.Parameters["@config_quicktime"].Value = 0;
        addVisit.Parameters["@config_realplayer"].Value = 0;
        addVisit.Parameters["@config_windowsmedia"].Value = 0;
        addVisit.Parameters["@config_cookie"].Value = 0;
        addVisit.Parameters["@location_ip"].Value = 1;
        addVisit.Parameters["@location_browser_lang"].Value = "en";
        addVisit.Parameters["@location_country"].Value = "it";
        res = addVisit.ExecuteScalar();
        result = Int32.Parse(res.ToString());
      }
      return result;
    }
    
    public void flush(Session s) {
      bool first = true;
      Hit idActStr = new Hit();
      Hit idActEnd = new Hit();
      if (s.hits.Count>0) {
        foreach(Hit hit in s.hits.Values) {
          if (first) {
            idActStr = hit;
            first = false;
          }
          idActEnd = hit;
        }
        int idVisit = FindVisit(s, idActStr, idActEnd);
        int idAction;
        Int32 idRefer;
        foreach(Hit hit in s.hits.Values) {
          idAction = FindAction(hit.reqURI.ToString());
          if (hit.refer == null) {
            idRefer = 0;
          }
          else {
            idRefer = FindAction(hit.refer.ToString());
          }
          AddHit(idVisit, idAction, idRefer, 1);
        }
      }
    }
    
  }
  
}
