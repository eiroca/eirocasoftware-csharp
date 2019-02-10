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
/*
 * Created by SharpDevelop.
 * User: enrico
 * Date: 08/03/2009
 * Time: 17.25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Reporting {

  public class TrafficExtractor : Worker<TrafficEventItem> {

    MyQueue<TrafficEventItem> trafficQueue;

    public TrafficExtractor(string aName, MyQueue<TrafficEventItem> inQueue) : base(aName, inQueue) {
      this.trafficQueue = inQueue;
    }

    override public bool Process(TrafficEventItem row) {
      return true;
    }

  }

}
