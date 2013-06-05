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
using System.Collections.Generic;

namespace Reporting {

  public struct WorkerStats {
    public int OK;
    public int KO;
  }
  
	public interface IWorkingElement {
    void Start();
    void Stop();
    bool IsFinished();

    string GetName();
    
    WorkerStats Stats();
    
  }

  public abstract class Coordinator : IWorkingElement {
		
    protected IWorkingElement[] workers;

		virtual public void Start() {
		  foreach (IWorkingElement w in workers) {
		    w.Start();
		  }
		}

		virtual public void Stop() {
		  foreach (IWorkingElement w in workers) {
		    w.Stop();
		    WorkerStats stats = w.Stats();
		    Console.WriteLine(w.GetName()+"="+stats.OK +" "+stats.KO);
		  }
		}
		
		virtual public bool IsFinished() {
		  bool _finished = true;  
		  for (int i = 0; i<workers.Length & _finished; i++) {
		    _finished &= workers[i].IsFinished();
		  }
			return _finished;
		}

		virtual public WorkerStats Stats() {
      return new WorkerStats();
		}
    
    virtual public string GetName() {
      return "Coordinator";
    }

  }

  /// <summary>
	/// Description of Interface1.
	/// </summary>
	public abstract class Worker<T> : IWorkingElement {
		
	  private string FName;
	  
		protected volatile bool _stopped = true;
		protected volatile bool _working = false;

		public WorkerStats stats = new WorkerStats();
		
		public string Name {
		  get {
		    return FName;
		  }
		}
		protected MyQueue<T> inQueue;

		public Worker(string aName, MyQueue<T> aQueue) {
			FName = aName;
			inQueue = aQueue;
		}
	
		virtual public void Start() {
			lock (this) {
				if (_stopped) {
					_stopped = false;
					new Thread(Loop).Start();
				}
			}
		}

		virtual public bool Process(T obj) {
      Console.WriteLine(Name+" working "+obj);
      return true;
		}

		virtual public void Loop() {
			while (!_stopped) {
				T obj = inQueue.Remove();
   			if (obj != null) {
				  _working = true;
				  try {
				    if (Process(obj)) {
				      stats.OK++;
				    }
				    else {
				      stats.KO++;
				    }
				  }
				  finally {
  				  _working = false;
				  }
  			}
   			else {
     			if (!_stopped) {
     				inQueue.wh.WaitOne(5000, false);
     			}
  			}
			}
		}

		virtual public void Stop() {
			_stopped = true;
		}

		virtual public bool IsFinished() {
		  return !_working;
		}

		virtual public WorkerStats Stats() {
		  return stats;
		}
		
    virtual public string GetName() {
      return FName;
    }

	}
	
}
