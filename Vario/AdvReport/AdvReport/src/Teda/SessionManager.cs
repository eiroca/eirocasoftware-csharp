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
using System.Timers;

namespace Reporting {

  public class Hit : IComparable {
    public DateTime when;
    public string timeZone;
    public string reqMethod;
    public Uri refer;
    public Uri reqURI;
    public string reqProtocol;
    public int status;
    public int byteSent;

    public int CompareTo(Object obj) {
      Hit h = (Hit)obj;
      return when.CompareTo(h.when);
    }

  }

  public class Session {

    public bool isNew = true;
    public DateTime lastTouch = DateTime.Now;
    public DateTime start;
    public DateTime last;

    public int key; // Session affinity key - e.g. Hash(user)
    public string user;
    public string userAgent;
    public SortedList<long, Hit> hits = new SortedList<long, Hit>();

    public Session() {
    }

    public bool Update(HitEventItem hit) {
      if (isNew) {
        isNew = false;
        start = hit.when;
        last = hit.when;
        key = hit.key;
        user = hit.user;
        userAgent = hit.userAgent;
      }
      else {
        if (hit.when < start) {
          start = hit.when;
        }
        if (hit.when > last) {
          last = hit.when;
        }
      }
      lastTouch = DateTime.Now;
      Hit h = new Hit();
      h.when = hit.when;
      h.timeZone = hit.timeZone;
      h.reqMethod = hit.reqMethod;
      h.reqURI = hit.reqURI;
      h.reqProtocol = hit.reqProtocol;
      h.status = hit.status;
      h.byteSent = hit.byteSent;
      h.refer = hit.refer;
      long tick = hit.when.Ticks;
      while (hits.ContainsKey(tick)) {
        tick++;
      }
      hits.Add(tick, h);
      return true;
    }

  }

  public class SessionManager : IWorkingElement {

    Dictionary<string, Session> sessions = new Dictionary<string, Session>();

    private static ReaderWriterLock readWriteLock = new ReaderWriterLock();

    System.Timers.Timer clock;
    WorkerStats stats = new WorkerStats();
    MyQueue<Session> sessionQueue;

    DateTime limit;
    const int CHECK_DELAY = 5000;
    const int TIMEOUT = 60000;

    public SessionManager(MyQueue<Session> sessionQueue) {
      this.sessionQueue = sessionQueue;
      limit = DateTime.Now;
      clock = new System.Timers.Timer(CHECK_DELAY);
      clock.Elapsed += new ElapsedEventHandler(CleanUp);
    }

    public Session GetSession(string user) {
      Session result = null;
      bool cached;
      readWriteLock.AcquireReaderLock(TIMEOUT);
      try {
        if (sessions.ContainsKey(user)) {
          cached = true;
          result = sessions[user];
        }
        else {
          cached = false;
          result = new Session();
          sessions[user] = result;
        }
        // Uncomment for accurate stats (performance penalty)
        //lock (typeof(SessionManager)) {
        if (cached) {
          stats.OK++;
        }
        else {
          stats.KO++;
        }
        // }
      }
      finally {
        readWriteLock.ReleaseReaderLock();
      }
      return result;
    }

    protected void Flush(Session s) {
      sessionQueue.Insert(s);
    }

    public void FlushAll() {
      readWriteLock.AcquireWriterLock(TIMEOUT);
      List<Session> flushed = new List<Session>();
      try {
        foreach (Session s in sessions.Values) {
          flushed.Add(s);
        }
        sessions.Clear();
      }
      finally {
        readWriteLock.ReleaseWriterLock();
      }
      foreach (Session s in flushed) {
        Flush(s);
      }
    }

    protected void CleanUp(object source, ElapsedEventArgs e) {
      Console.WriteLine("Session cleanup");
      clock.Stop();
      List<Session> flushed = new List<Session>();
      readWriteLock.AcquireWriterLock(TIMEOUT);
      try {
        foreach (Session s in sessions.Values) {
          if (s.lastTouch < limit) {
            sessions.Remove(s.user);
            flushed.Add(s);
          }
        }
        limit = DateTime.Now;
      }
      finally {
        readWriteLock.ReleaseWriterLock();
        clock.Start();
      }
      foreach (Session s in flushed) {
        Console.WriteLine("Cleanup " + s.user);
        Flush(s);
      }
    }

    void IWorkingElement.Start() {
      clock.Start();
    }

    void IWorkingElement.Stop() {
      clock.Stop();
      FlushAll();
    }

    bool IWorkingElement.IsFinished() {
      return true;
    }

    string IWorkingElement.GetName() {
      return "Session Manager";
    }

    WorkerStats IWorkingElement.Stats() {
      return stats;
    }

  }

}
