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
 
  /// <summary>
  ///Parsing log apache da rifare in un modo decente
  /// </summary>
  public class RowProcessor : Worker<RowItem> {
    
    MyQueue<EventItem>outQueue;

/*
%...a:          Remote IP-address
%...A:          Local IP-address
%...B:          Bytes sent, excluding HTTP headers.
%...b:          Bytes sent, excluding HTTP headers. In CLF format
        i.e. a '-' rather than a 0 when no bytes are sent.
%...c:          Connection status when response was completed.
                'X' = connection aborted before the response completed.
                '+' = connection may be kept alive after the response is sent.
                '-' = connection will be closed after the response is sent.
%...{FOOBAR}e:  The contents of the environment variable FOOBAR
%...f:          Filename
%...h:          Remote host
%...H       The request protocol
%...{Foobar}i:  The contents of Foobar: header line(s) in the request
                sent to the server.
%...l:          Remote logname (from identd, if supplied)
%...m       The request method
%...{Foobar}n:  The contents of note "Foobar" from another module.
%...{Foobar}o:  The contents of Foobar: header line(s) in the reply.
%...p:          The canonical Port of the server serving the request
%...P:          The process ID of the child that serviced the request.
%...q       The query string (prepended with a ? if a query string exists,
        otherwise an empty string)
%...r:          First line of request
%...s:          Status.  For requests that got internally redirected, this is
                the status of the *original* request --- %...>s for the last.
%...t:          Time, in common log format time format (standard english format)
%...{format}t:  The time, in the form given by format, which should
                be in strftime(3) format. (potentially localized)
%...T:          The time taken to serve the request, in seconds.
%...u:          Remote user (from auth; may be bogus if return status (%s) is 401)
%...U:          The URL path requested, not including any query string.
%...v:          The canonical ServerName of the server serving the request.
%...V:          The server name according to the UseCanonicalName setting.
*/
    //CLF Format
    //LogFormat "%h %l %u %t \"%r\" %>s %b" common
    Regex fmt_CLF      = new Regex("^([^ ]+) ([^ ]+) ([^ ]+) \\[([^ ]+) ([^ ]+)\\] \"([^ \"]+) ?([^ ]+)? ?([^\"]*)?\" ([^ ]+) ([^ ]+)", RegexOptions.Compiled);
    
    //Combined Format
    //LogFormat "%h %l %u %t \"%r\" %>s %b \"%{Referer}i\" \"%{User-Agent}i\"" combined
    Regex fmt_combined = new Regex("^([^ ]+) ([^ ]+) ([^ ]+) \\[([^ ]+) ([^ ]+)\\] \"([^ \"]+) ?([^ ]+)? ?([^\"]*)?\" ([^ ]+) ([^ ]+) \"([^\"]*)\" \"([^\"]*)\"", RegexOptions.Compiled);

    Uri server = new Uri("http://smartlab");
      
    public RowProcessor(string aName, MyQueue<RowItem> inQueue, MyQueue<EventItem> outQueue) : base(aName, inQueue) {
      this.outQueue = outQueue;
    }

  private string NormalizeField(string s) {
    if (s.CompareTo("-")==0) { return null; }
    else return s;
  }

  private int NormalizeIntField(string s) {
    if (s.CompareTo("-")==0) { return 0; }
    else return Int32.Parse(s);
  }
  
  private bool ParseLogTimeStamp(string timeStamp, out DateTime result) {
    // 12/Dec/2008:17:35:28
    // 01234567890123456789
    try {
      int yy, mm, dd, hh, mi, ss;
      string month = timeStamp.Substring(3,3).ToLower();
      if (month.CompareTo("jan")==0) { mm = 1; }
      else if (month.CompareTo("feb")==0) { mm = 2; }
      else if (month.CompareTo("mar")==0) { mm = 3; }
      else if (month.CompareTo("apr")==0) { mm = 4; }
      else if (month.CompareTo("may")==0) { mm = 5; }
      else if (month.CompareTo("jun")==0) { mm = 6; }
      else if (month.CompareTo("jul")==0) { mm = 7; }
      else if (month.CompareTo("aug")==0) { mm = 8; }
      else if (month.CompareTo("sep")==0) { mm = 9; }
      else if (month.CompareTo("oct")==0) { mm = 10; }
      else if (month.CompareTo("nov")==0) { mm = 11; }
      else if (month.CompareTo("dec")==0) { mm = 12; }
      else {mm = 0; }
      dd  = Int32.Parse(timeStamp.Substring(0,2));
      yy = Int32.Parse(timeStamp.Substring(7,4));
      hh = Int32.Parse(timeStamp.Substring(12,2));
      mi = Int32.Parse(timeStamp.Substring(15,2));
      ss = Int32.Parse(timeStamp.Substring(18,2));
      result = new DateTime(yy, mm, dd, hh, mi, ss);
      return true;      
    }
    catch {
      result = default(DateTime);
      return false;
    }
  }

    private bool ProcessWebLog(Regex fmt, RowItem row) {
      HitEventItem ev = null;
      bool ok = false;
      Match m = fmt.Match(row.line);
      if (m.Groups.Count >= 11) {
        string[] fld = new string[m.Groups.Count];
        int count = 0;
        foreach (Group g in m.Groups) {
          foreach (Capture c in g.Captures) {
            fld[count] = c.Value;
            count++;
          }
        }
        try  {
          ev = new HitEventItem();
          ev.type = EventType.HIT;
          if (ParseLogTimeStamp(fld[4], out ev.when)) {
            ev.remoteHost = NormalizeField(fld[1]);
            ev.authUser = NormalizeField(fld[2]);
            ev.remoteLogName = NormalizeField(fld[3]);
            ev.timeZone = NormalizeField(fld[5]);
            ev.reqMethod = NormalizeField(fld[6]);
            ev.reqProtocol = NormalizeField(fld[8]);
            ev.status = NormalizeIntField(fld[9]);
            ev.byteSent = NormalizeIntField(fld[10]);
            ev.reqURI = new Uri(server, fld[7]);
            if (row.type == LogFormat.COMBINED) {
              string refer = NormalizeField(fld[11]);
              if (refer!=null) {
                ev.refer = new Uri(refer);
              }
              ev.userAgent = NormalizeField(fld[12]);
            }
            if (ev.authUser!=null) {
              ev.user="A-"+ev.authUser;
            }
            else {
              if (ev.userAgent!=null) {
                ev.user="C-"+ev.remoteHost+"."+ev.userAgent;
              }
              else {
                ev.user="I-"+ev.remoteHost;
              }
            }
            ev.key = ev.user.GetHashCode();
            ok = true;
          }
        }
        catch (Exception e) {
          Console.WriteLine("ERR="+e.Message);
          ok = false; 
        }
      }
      if (ok) {
        outQueue.Insert(ev);
      }
      else {
        Console.WriteLine("ERR "+row.line);
      }
      return ok;
    }
    
    override public bool Process(RowItem row) {
      bool ok = false;
      switch (row.type) {
        case LogFormat.CLF: 
          ok = ProcessWebLog(fmt_CLF, row);
          break;
        case LogFormat.COMBINED: 
          ok = ProcessWebLog(fmt_combined, row);
          break;
      }
      return ok;
    }

  }
  
}
