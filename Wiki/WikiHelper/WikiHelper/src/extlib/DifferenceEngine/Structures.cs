#region Header

//#define USE_HASH_TABLE

#endregion Header

namespace DifferenceEngine {
  using System;

  #region Enumerations
  public enum DiffResultSpanStatus {
    NoChange, Replace, DeleteSource, AddDestination
  }

  internal enum DiffStatus {
    Matched = 1,
    NoMatch = -1,
    Unknown = -2
  }
  #endregion Enumerations

  public interface IDiffList {
    #region Methods
    int Count();
    IComparable GetByIndex(int index);
    #endregion Methods
  }

  public class DiffResultSpan : IComparable {
    #region Fields
    private const int BAD_INDEX = -1;
    private int _destIndex;
    private int _length;
    private int _sourceIndex;
    private DiffResultSpanStatus _status;
    #endregion Fields

    #region Constructors
    protected DiffResultSpan(DiffResultSpanStatus status, int destIndex, int sourceIndex, int length) {
      _status = status;
      _destIndex = destIndex;
      _sourceIndex = sourceIndex;
      _length = length;
    }
    #endregion Constructors

    #region Properties
    public int DestIndex {
      get {
        return _destIndex;
      }
    }

    public int Length {
      get {
        return _length;
      }
    }

    public int SourceIndex {
      get {
        return _sourceIndex;
      }
    }

    public DiffResultSpanStatus Status {
      get {
        return _status;
      }
    }
    #endregion Properties

    #region Methods
    public static DiffResultSpan CreateAddDestination(int destIndex, int length) {
      return new DiffResultSpan(DiffResultSpanStatus.AddDestination, destIndex, BAD_INDEX, length);
    }

    public static DiffResultSpan CreateDeleteSource(int sourceIndex, int length) {
      return new DiffResultSpan(DiffResultSpanStatus.DeleteSource, BAD_INDEX, sourceIndex, length);
    }

    public static DiffResultSpan CreateNoChange(int destIndex, int sourceIndex, int length) {
      return new DiffResultSpan(DiffResultSpanStatus.NoChange, destIndex, sourceIndex, length);
    }

    public static DiffResultSpan CreateReplace(int destIndex, int sourceIndex, int length) {
      return new DiffResultSpan(DiffResultSpanStatus.Replace, destIndex, sourceIndex, length);
    }

    public void AddLength(int i) {
      _length += i;
    }

    public int CompareTo(object obj) {
      return _destIndex.CompareTo(((DiffResultSpan)obj)._destIndex);
    }

    public override string ToString() {
      return string.Format("{0} (Dest: {1},Source: {2}) {3}", _status.ToString(), _destIndex.ToString(), _sourceIndex.ToString(), _length.ToString());
    }
    #endregion Methods
  }

  internal class DiffState {
    #region Fields
    private const int BAD_INDEX = -1;
    private int _length;
    private int _startIndex;
    #endregion Fields

    #region Constructors
    public DiffState() {
      SetToUnkown();
    }
    #endregion Constructors

    #region Properties
    public int EndIndex {
      get {
        return ((_startIndex + _length) - 1);
      }
    }

    public int Length {
      get {
        int len;
        if (_length > 0) {
          len = _length;
        }
        else {
          if (_length == 0) {
            len = 1;
          }
          else {
            len = 0;
          }
        }
        return len;
      }
    }

    public int StartIndex {
      get {
        return _startIndex;
      }
    }

    public DiffStatus Status {
      get {
        DiffStatus stat;
        if (_length > 0) {
          stat = DiffStatus.Matched;
        }
        else {
          switch (_length) {
            case -1:
              stat = DiffStatus.NoMatch;
              break;
            default:
              System.Diagnostics.Debug.Assert(_length == -2, "Invalid status: _length < -2");
              stat = DiffStatus.Unknown;
              break;
          }
        }
        return stat;
      }
    }
    #endregion Properties

    #region Methods

    public bool HasValidLength(int newStart, int newEnd, int maxPossibleDestLength) {
      if (_length > 0) {
        //have unlocked match
        if ((maxPossibleDestLength < _length) || ((_startIndex < newStart) || (EndIndex > newEnd))) {
          SetToUnkown();
        }
      }
      return (_length != (int)DiffStatus.Unknown);
    }

    public void SetMatch(int start, int length) {
      System.Diagnostics.Debug.Assert(length > 0, "Length must be greater than zero");
      System.Diagnostics.Debug.Assert(start >= 0, "Start must be greater than or equal to zero");
      _startIndex = start;
      _length = length;
    }

    public void SetNoMatch() {
      _startIndex = BAD_INDEX;
      _length = (int)DiffStatus.NoMatch;
    }

    protected void SetToUnkown() {
      _startIndex = BAD_INDEX;
      _length = (int)DiffStatus.Unknown;
    }
    #endregion Methods

  }

  internal class DiffStateList {
#if USE_HASH_TABLE
    private Hashtable _table;
#else
    private DiffState[] _array;
#endif

    #region Constructors
    public DiffStateList(int destCount) {
#if USE_HASH_TABLE
      _table = new Hashtable(Math.Max(9,destCount/10));
#else
      _array = new DiffState[destCount];
#endif
    }
    #endregion Constructors

    #region Methods

    public DiffState GetByIndex(int index) {
#if USE_HASH_TABLE
      DiffState retval = (DiffState)_table[index];
      if (retval == null) {
        retval = new DiffState();
        _table.Add(index,retval);
      }
#else
      DiffState retval = _array[index];
      if (retval == null) {
        retval = new DiffState();
        _array[index] = retval;
      }
#endif
      return retval;
    }

    #endregion Methods
  }

}