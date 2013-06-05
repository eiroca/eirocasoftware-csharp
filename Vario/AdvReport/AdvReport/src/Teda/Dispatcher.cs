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
using System.Text.RegularExpressions;

namespace Reporting {
 
	public class Dispatcher : Worker<EventItem> {
		
    MyQueue<EventItem>[] outQueue;
    int numQueue;
    
    public Dispatcher(string aName, MyQueue<EventItem> inQueue, MyQueue<EventItem>[] outQueue) : base(aName, inQueue) {
      this.outQueue = outQueue;
      numQueue = outQueue.Length;
    }
  
  	override public bool Process(EventItem row) {
      int idx = Math.Abs(row.key) % numQueue;
      outQueue[idx].Insert(row);
      return true;
  	}

  }
	
}
