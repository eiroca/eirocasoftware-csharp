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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Reporting {
	
	/// <summary>
	/// Description of LogCollector.
	/// </summary>
	public sealed class LogCollector : Coordinator {
	
		public MyQueue<FileItem> inFiles = new MyQueue<FileItem>();
		public MyQueue<FileItem> outFiles = new MyQueue<FileItem>();
		public MyQueue<RowItem> rows = new MyQueue<RowItem>();

		const int NUM_READERS=2;
		const int NUM_PROCESSORS=4;
		
		public LogCollector(MyQueue<string> paths, MyQueue<EventItem> events) {
/*
 	    LogCollectorConfig config = LogCollectorConfig.GetSection(ConfigurationUserLevel.None);
			Console.WriteLine(config.ExampleAttribute);
			config.ExampleAttribute = "B";
			config.Class1.Add(new Class1Element());
	    config.Save();
*/
	    workers = new IWorkingElement[1+NUM_READERS+NUM_PROCESSORS];
	    int p = 0;
	    workers[p] = new DirectoryScanner("Scanner", paths, inFiles); 
	    p++;
		  for (int i = 0; i<NUM_READERS; i++) {
		    workers[p] = new FileReader("FR-"+i, inFiles, rows, outFiles);
		    p++;
		  }
		  for (int i = 0; i<NUM_PROCESSORS; i++) {
		    workers[p] = new RowProcessor("RP-"+i, rows, events);
		    p++;
		  }
		}

	}
	
}
