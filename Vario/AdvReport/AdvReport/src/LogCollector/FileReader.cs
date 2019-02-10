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
using System.IO;

namespace Reporting {

  /// <summary>
  /// Description of Class1.
  /// </summary>
  public class FileReader : Worker<FileItem> {

    MyQueue<RowItem> rowQueue;
    MyQueue<FileItem> processedQueue;

    public FileReader(string aName, MyQueue<FileItem> inQueue, MyQueue<RowItem> rowQueue, MyQueue<FileItem> processedQueue) : base(aName, inQueue) {
      this.rowQueue = rowQueue;
      this.processedQueue = processedQueue;
    }

    override public bool Process(FileItem fileItem) {
      Console.WriteLine(fileItem.path);
      bool ok = false;
      StreamReader file = null;
      try {
        try {
          file = new StreamReader(fileItem.path);
          string line;
          while ((line = file.ReadLine()) != null) {
            RowItem ri = new RowItem();
            ri.type = fileItem.type;
            ri.line = line;
            rowQueue.Insert(ri);
          }
          ok = true;
        }
        finally {
          if (file != null) {
            file.Close();
          }
        }
      }
      catch {
      }
      if (ok) {
        processedQueue.Insert(fileItem);
      }
      return ok;
    }
  }

}
