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
using System.Data.OleDb;
using System.Threading;

using GlynnTucker.Cache;

namespace Reporting {

  public class TrascodingTokenizer {

    const string SELECT = "SELECT {1} FROM {0} WHERE {2} = @value";
    const string INSERT = "INSERT INTO {0}({2}) VALUES (@value)"; //; SELECT LAST_INSERT_ID() AS ID;

    const int TIMEOUT = 60000;

    String keyContext;
    String descContext;

    OleDbCommand findKey;
    OleDbCommand findDesc;
    OleDbCommand addKey;

    bool caseSensitive;
    OleDbConnection conn;
    ReaderWriterLock aLock;

    public TrascodingTokenizer(OleDbConnection conn, ReaderWriterLock aLock, String tableName, bool caseSensitive, String keyField, String descField) {
      this.conn = conn;
      this.aLock = aLock;
      this.caseSensitive = caseSensitive;
      keyContext = tableName + ".key";
      descContext = tableName + ".desc";
      Cache.AddContext(keyContext);
      Cache.AddContext(descContext);
      findKey = new OleDbCommand();
      findKey.Connection = conn;
      findKey.CommandText = String.Format(SELECT, new Object[] { tableName, keyField, descField });
      findKey.Parameters.Add("@value", OleDbType.VarChar);
      findDesc = new OleDbCommand();
      findDesc.Connection = conn;
      findDesc.CommandText = String.Format(SELECT, new Object[] { tableName, descField, keyField });
      findDesc.Parameters.Add("@key", OleDbType.Integer);
      addKey = new OleDbCommand();
      addKey.Connection = conn;
      addKey.CommandText = String.Format(INSERT, new Object[] { tableName, keyField, descField });
      addKey.Parameters.Add("@value", OleDbType.VarChar);
    }

    public object AddEntry(string desc) {
      object key;
      aLock.AcquireWriterLock(TIMEOUT);
      try {
        addKey.Parameters["@value"].Value = desc;
        key = addKey.ExecuteScalar();
        key = findKey.ExecuteScalar();
      }
      finally {
        aLock.ReleaseWriterLock();
      }
      return key;
    }

    public Int32 getKey(string value) {
      object key;
      string desc;
      if (caseSensitive) {
        desc = value;
      }
      else {
        desc = value.ToLower();
      }
      if (!Cache.TryGet(keyContext, desc, out key)) {
        aLock.AcquireReaderLock(TIMEOUT);
        try {
          findKey.Parameters["@value"].Value = desc;
          key = findKey.ExecuteScalar();
          if (key == null) {
            key = AddEntry(desc);
          }
          Cache.AddOrUpdate(keyContext, desc, key);
        }
        finally {
          aLock.ReleaseReaderLock();
        }
      }
      return (Int32)key;
    }

    public string getDesc(Int32 key) {
      object desc;
      if (!Cache.TryGet(descContext, key, out desc)) {
        findDesc.Parameters["@key"].Value = key;
        desc = findKey.ExecuteScalar();
      }
      return (string)desc;
    }

  }

  public class Tokenizer {

    private TrascodingTokenizer schemeTokenizer;
    private TrascodingTokenizer hostTokenizer;
    private TrascodingTokenizer pathTokenizer;
    private TrascodingTokenizer queryTokenizer;
    private TrascodingTokenizer fragmentTokenizer;


    private static ReaderWriterLock readWriteLock = new ReaderWriterLock();

    const string SECTION = "tokenizers";

    OleDbConnection conn;

    public Tokenizer() {
      DBHelper db = AWR.db;
      this.conn = db.GetConnection(SECTION);
      schemeTokenizer = new TrascodingTokenizer(conn, readWriteLock, "sysURIScheme", false, "CodScheme", "Scheme");
      hostTokenizer = new TrascodingTokenizer(conn, readWriteLock, "sysURIHost", false, "CodHost", "Host");
      pathTokenizer = new TrascodingTokenizer(conn, readWriteLock, "sysURIPath", false, "CodPath", "Path");
      queryTokenizer = new TrascodingTokenizer(conn, readWriteLock, "sysURIQueryString", false, "CodQueryString", "QueryString");
      fragmentTokenizer = new TrascodingTokenizer(conn, readWriteLock, "sysURIFragment", false, "CodFragment", "Fragment");
    }

    public Int32 GetCodScheme(string scheme) {
      return schemeTokenizer.getKey(scheme);
    }

    public Int32 GetCodHost(string host) {
      return hostTokenizer.getKey(host);
    }

    public Int32 GetCodPath(string path) {
      return pathTokenizer.getKey(path);
    }

    public Int32 GetCodQuery(string query) {
      return queryTokenizer.getKey(query);
    }

    public Int32 GetCodFragment(string fragment) {
      return fragmentTokenizer.getKey(fragment);
    }

    public URIItem Tokenize(Uri uri) {
      URIItem item = new URIItem();
      item.scheme = GetCodScheme(uri.Scheme);
      item.host = GetCodHost(uri.Host);
      item.port = uri.Port;
      item.path = GetCodPath(uri.AbsolutePath);
      item.path = GetCodQuery(uri.Query);
      item.fragment = GetCodFragment(uri.Fragment);
      return item;
    }

  }

}
