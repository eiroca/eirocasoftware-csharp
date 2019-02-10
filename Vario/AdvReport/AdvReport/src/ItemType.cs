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

  public enum LogFormat {
    CLF,
    COMBINED
  }

  public enum EventType {
    HIT
  }

  public class FileItem {
    public LogFormat type;
    public string path;
  }

  public class RowItem {
    public LogFormat type;
    public string line;
  }


  public class EventItem {
    public EventType type;
    public DateTime when;
    public int key; // Session affinity key - e.g. Hash(user)
  }

  public class HitEventItem : EventItem {
    public string user;
    public string timeZone;
    public string remoteHost;
    public string remoteLogName;
    public string authUser;
    public string reqMethod;
    public Uri reqURI;
    public string reqProtocol;
    public int status;
    public int byteSent;
    public Uri refer;
    public string userAgent;
  }

  public class TrafficEventItem : EventItem {
    public int byteSent;
  }

  public class URIItem {
    public int CodURL;
    public int scheme;
    public int host;
    public int port;
    public int path;
    public int query;
    public int fragment;
  }

}
