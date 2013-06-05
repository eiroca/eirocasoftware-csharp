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

namespace Reporting {

  public class EventProcessor : Worker<EventItem> {
  
    SessionManager sessionManager;
    MyQueue<TrafficEventItem> trafficQueue;
    
    public EventProcessor(string aName, MyQueue<EventItem> inQueue, SessionManager sessionManager, MyQueue<TrafficEventItem> trafficQueue) : base(aName, inQueue) {
      this.trafficQueue = trafficQueue;
      this.sessionManager = sessionManager;
    }
  
  	override public bool Process(EventItem ev) {
      if (ev is HitEventItem) {
        HitEventItem hit = (HitEventItem)ev;
        Session session = sessionManager.GetSession(hit.user);
        return session.Update(hit);
        TrafficEventItem ti = new TrafficEventItem();
        ti.when = hit.when;
        ti.key = hit.key;
        ti.type = hit.type;
        ti.byteSent = hit.byteSent;
        trafficQueue.Insert(ti);
      }
      return false;
  	}

  }
  
}
