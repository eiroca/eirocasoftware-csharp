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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Reporting {
  
  public class DBHelper {

    Dictionary<string, OleDbConnection> connections = new Dictionary<string, OleDbConnection>();

    string connectionString = null;
    
    public DBHelper() {
      connectionString =
        "Provider=Microsoft.Jet.OLEDB.4.0;"+
        "Data Source=C:\\DB\\datamodel.mdb;";
    }
    
    public OleDbConnection GetConnection(string section) {
      OleDbConnection connection;
      lock (connections) {
        if (connections.ContainsKey(section)) {
          connection = connections[section];
        }
        else {
          connection = new OleDbConnection(connectionString);
          connections.Add(section, connection);
          connection.Open();
        }
      }
      return connection;
    }
    
    public void Close() {
      lock (connections) {
        foreach (DbConnection c in connections.Values) {
          c.Close();
        }
        connections.Clear();
      }
    }

  }
  
}
