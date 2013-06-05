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
using System.Threading;

namespace Reporting {

	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class MyQueue<T> {
		
		private Queue<T> queue;
    public EventWaitHandle wh = new AutoResetEvent (false);

		
		public MyQueue() {
			queue = new Queue<T>(32);
		}

		public bool IsEmpty() {
			lock (queue) {
				return (queue.Count==0);
			}
		}
		
		public T Remove(){
			T result = default(T);
			lock (queue) {
				if (queue.Count>0) {
					return queue.Dequeue();
				}
			}
			return result;
		}
		
		public void Insert(T obj) {
			lock (queue) {
				queue.Enqueue(obj);
			}
    	wh.Set();
		}

	}
	
}
