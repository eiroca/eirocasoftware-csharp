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
using System.Threading;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;

using MySql.Data.MySqlClient;

namespace Reporting {

  class AWR {
    
    public static DBHelper db = new DBHelper();
    
    public static MyQueue<string> paths = new MyQueue<string>();
    public static MyQueue<EventItem> events = new MyQueue<EventItem>();

    private static LogCollector logCollector = new LogCollector(paths, events);
    private static TEDA teda = new TEDA(events);
        
    public static void Main(string[] args) {
            
      Tokenizer tokenizer = new Tokenizer();
       
      URIItem uri = tokenizer.Tokenize(new Uri("http://www.eiroca.net/path/index.html?a=2#frag"));
              

      logCollector.Start();
      teda.Start();
      
      paths.Insert("C:\\temp");

      bool a;
      bool b;
      do {
        Thread.Sleep(1000);
        a = logCollector.IsFinished();
        b = teda.IsFinished();
      }
      while ((!a) | (!b));

      logCollector.Stop();
      teda.Stop();
      
      Console.WriteLine("Press any key to continue . . . ");
      Console.ReadKey(true);
    }
  }
  
}