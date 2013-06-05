/*
 * Created by SharpDevelop.
 * User: ecroce
 * Date: 05/03/2009
 * Time: 15.38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
  
namespace AdvReport {

  public class Data  {
    
    private MySqlConnection connection=null;
    string connectionString=null;
    
    public Data(string host, string database, string userId, string password) {
      connectionString= 
        "Server="+host+";"+
        "Database="+database+";"+
        "Uid="+userId+";"+
        "Pwd="+password+";";
      connection=new MySqlConnection(connectionString);
      connection.Open();
    }
    
    public DataSet log_visit() {
      MySqlCommand command = new MySqlCommand();
      MySqlCommandBuilder builder = new MySqlCommandBuilder();
      builder.ConflictOption = ConflictOption.OverwriteChanges;
      command.Connection  = connection;
      command.CommandText = "SELECT * FROM piwik_log_visit";
      MySqlDataAdapter adapter = new MySqlDataAdapter(command);
      builder.DataAdapter = adapter;
      DataSet result = new DataSet();
      adapter.Fill(result, "log_visit");
      return result;
    }
  }

}
