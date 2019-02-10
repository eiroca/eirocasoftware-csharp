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
using System.IO;

namespace Reporting {
  /// <summary>
  /// Description of Class1.
  /// </summary>
  public class DirectoryScanner : Worker<string> {

    MyQueue<FileItem> outQueue;

    public DirectoryScanner(string aName, MyQueue<string> inQueue, MyQueue<FileItem> outQueue) : base(aName, inQueue) {
      this.outQueue = outQueue;
    }

    public void Scan(string aDirectory) {
      string[] files = Directory.GetFiles(aDirectory);
      for (int i = 0; i < files.Length; i++) {
        string file = files[i];
        if (File.Exists(file)) {
          StreamReader aFile = new StreamReader(file);
          string line;
          do {
            line = aFile.ReadLine();
            int a = line.IndexOf(" 200 ");
          }
          while ((line != null) && (line.IndexOf(" 200 ") == -1));
          if (line != null) {
            FileItem fi = new FileItem();
            if (line.EndsWith("\"")) {
              fi.type = LogFormat.COMBINED;
            }
            else {
              fi.type = LogFormat.CLF;
            }
            fi.path = file;
            outQueue.Insert(fi);
          }
          aFile.Close();
        }
      }
    }

    override public bool Process(string path) {
      Scan(path);
      return true;
    }

  }

}
