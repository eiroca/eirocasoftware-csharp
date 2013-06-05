namespace DifferenceEngine {
  using System;
  using System.Collections;

  public class DiffList_CharData : IDiffList {
    #region Fields
    private char[] _charList;
    #endregion Fields

    #region Constructors
    public DiffList_CharData(string charData) {
      _charList = charData.ToCharArray();
    }
    #endregion Constructors

    #region Methods
    public int Count() {
      return _charList.Length;
    }

    public IComparable GetByIndex(int index) {
      return _charList[index];
    }
    #endregion Methods
    
  }
  
}