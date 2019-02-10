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

namespace Reporting {

  public class TEDA : Coordinator {

    const int NUM_PROCESSORS = 4;

    MyQueue<EventItem>[] processorQueues;
    MyQueue<EventItem> eventsQueue;

    MyQueue<Session> sessionQueue;
    MyQueue<TrafficEventItem> trafficQueue;
    SessionManager sm;

    public TEDA(MyQueue<EventItem> events) {
      eventsQueue = events;
      processorQueues = new MyQueue<EventItem>[NUM_PROCESSORS];
      sessionQueue = new MyQueue<Session>();
      trafficQueue = new MyQueue<TrafficEventItem>();

      int p = 0;
      workers = new IWorkingElement[1 + 1 + 1 + 1 + NUM_PROCESSORS];
      sm = new SessionManager(sessionQueue);
      workers[p] = sm;
      p++;
      workers[p] = new Dispatcher("DS", events, processorQueues);
      p++;
      workers[p] = new TrafficExtractor("TE", trafficQueue);
      p++;
      workers[p] = new SessionFixup("SF", sessionQueue);
      p++;
      for (int i = 0; i < NUM_PROCESSORS; i++) {
        processorQueues[i] = new MyQueue<EventItem>();
        workers[p] = new EventProcessor("EP-" + i, processorQueues[i], sm, trafficQueue);
        p++;
      }
    }

    override public bool IsFinished() {
      return eventsQueue.IsEmpty() && sessionQueue.IsEmpty() && base.IsFinished();
    }

  }

}
