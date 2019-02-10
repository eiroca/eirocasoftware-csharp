namespace DiffCalcer {
  using System;
  using System.Collections;
  using System.IO;

  using DifferenceEngine;

  public class LineFile : IDiffList {
    #region Fields
    private const int MaxLineLength = 1024;
    private ArrayList _lines;
    #endregion Fields

    #region Constructors
    public LineFile(string fileName) {
      _lines = new ArrayList();
      using (StreamReader sr = new StreamReader(fileName)) {
        String line;
        // Read and display lines from the file until the end of
        // the file is reached.
        while ((line = sr.ReadLine()) != null) {
          if (line.Length > MaxLineLength) {
            throw new InvalidOperationException(string.Format("File contains a line greater than {0} characters.", MaxLineLength.ToString()));
          }
          _lines.Add(new TextLine(line));
        }
      }
    }
    #endregion Constructors

    #region Methods
    public int Count() {
      return _lines.Count;
    }

    public IComparable GetByIndex(int index) {
      return (TextLine)_lines[index];
    }
    #endregion Methods
  }

  public class TextLine : IComparable {
    #region Fields
    public string Line;
    private int _hash;
    #endregion Fields

    #region Constructors
    public TextLine(string str) {
      Line = str.Replace("\t", "    ");
      _hash = str.GetHashCode();
    }
    #endregion Constructors

    #region Methods
    public int CompareTo(object obj) {
      return _hash.CompareTo(((TextLine)obj)._hash);
    }
    #endregion Methods
  }

}