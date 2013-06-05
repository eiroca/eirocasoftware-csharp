namespace DifferenceEngine {
  using System;
  using System.Collections;
  using System.IO;

  public class DiffList_BinaryFile : IDiffList {
    #region Fields
    private byte[] _byteList;
    #endregion Fields

    #region Constructors
    public DiffList_BinaryFile(string fileName) {
      FileStream fs = null;
      BinaryReader br = null;
      try {
        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        int len = (int)fs.Length;
        br = new BinaryReader(fs);
        _byteList = br.ReadBytes(len);
      }
      catch (Exception ex) {
        throw ex;
      }
      finally {
        if (br != null) br.Close();
        if (fs != null) fs.Close();
      }
    }
    #endregion Constructors

    #region Methods
    public int Count() {
      return _byteList.Length;
    }

    public IComparable GetByIndex(int index) {
      return _byteList[index];
    }
    #endregion Methods
    
  }
  
}