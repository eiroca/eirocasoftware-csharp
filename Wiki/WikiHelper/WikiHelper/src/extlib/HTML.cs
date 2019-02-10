/// This source code may be used freely under the Limited GNU Public License(LGPL).
/// Written by Jeff Heaton (http://www.jeffheaton.com)
/// changed by Enrico Croce
namespace HTML {
  using System;
  using System.Collections;
  using System.Text;
  using System.Web;

  /// <summary>
  /// Attribute holds one attribute, as is normally stored in an
  /// HTML or XML file. This includes a name, value and delimiter.
  /// </summary>
  public class Attribute : ICloneable {

    #region Fields
    /// <summary>
    /// The delimiter for this attribute.
    /// </summary>
    private char fDelim;
    /// <summary>
    /// The name for this attribute.
    /// </summary>
    private string fName;
    /// <summary>
    /// The value of this attribute
    /// </summary>
    private string fValue;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Construct a new Attribute.  The name, delim, and value
    /// properties can be specified here.
    /// </summary>
    /// <param name="name">The name of this attribute.</param>
    /// <param name="value">The value of this attribute.</param>
    /// <param name="delim">The delimiter character for the value.
    /// </param>
    public Attribute(string name, string value, char delim) {
      fName = name;
      fValue = value;
      fDelim = delim;
    }

    /// <summary>
    /// The default constructor.  Construct a blank attribute.
    /// </summary>
    public Attribute() : this("", "", (char)0) {
    }

    /// <summary>
    /// Construct an attribute without a delimiter.
    /// </summary>
    /// <param name="name">The name of this attribute.</param>
    /// <param name="value">The value of this attribute.</param>
    public Attribute(String name, String value) : this(name, value, (char)0) {
    }
    #endregion Constructors

    #region Properties
    public char Delim {
      get {
        return fDelim;
      }
      set {
        fDelim = value;
      }
    }

    public string Name {
      get {
        return fName;
      }
      set {
        fName = value;
      }
    }

    public string Value {
      get {
        return fValue;
      }
      set {
        fValue = value;
      }
    }
    #endregion Properties

    #region Methods
    public virtual object Clone() {
      return new Attribute(fName, fValue, fDelim);
    }

    /// <summary>
    /// Convert attribute to a string
    /// </summary>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      if (Name != null) {
        sb.Append(Name);
        if (!String.IsNullOrEmpty(Value)) {
          sb.Append('=');
          if (Delim != 0) {
            sb.Append(Delim);
          }
          sb.Append(Value);
          if (Delim != 0) {
            sb.Append(Delim);
          }
        }
      }
      return sb.ToString();
    }
    #endregion Methods

  }

  /// <summary>
  /// Base class for parsing tag based files, such as HTML,
  /// HTTP headers, or XML.
  ///
  /// </summary>
  public class HTMLParser {
    #region Fields
    public bool skipMultipleSpace = true;
    /// <summary>
    /// The source text that is being parsed.
    /// </summary>
    private string fSource;
    /// <summary>
    /// The current position inside of the text that is being parsed.
    /// </summary>
    private int idx;
    #endregion Fields

    #region Properties
    public string Source {
      get {
        return fSource;
      }
      set {
        fSource = value;
      }
    }
    #endregion Properties

    #region Methods
    public static bool IsAttrNameEnd(char ch) {
      return ("\t\n\r =>/".IndexOf(ch) != -1);
    }

    public static bool IsAttrValueEnd(char ch) {
      return ("\t\n\r >/".IndexOf(ch) != -1);
    }

    public static bool IsDelim(char ch) {
      return ("'\"".IndexOf(ch) != -1);
    }

    public static bool IsTagEnd(char ch) {
      return ("/>".IndexOf(ch) != -1);
    }

    public static bool IsTagNameEnd(char ch) {
      return ("\t\n\r >".IndexOf(ch) != -1);
    }

    /// <summary>
    /// Determine if the specified character is whitespace or not.
    /// </summary>
    /// <param name="ch">A character to check</param>
    /// <returns>true if the character is whitespace</returns>
    public static bool IsWhiteSpace(char ch) {
      return ("\t\n\r ".IndexOf(ch) != -1);
    }

    /// <summary>
    /// Move the index forward by one.
    /// </summary>
    public void Advance() {
      idx++;
    }

    /// <summary>
    /// Obtain the next character and advance the index by one.
    /// </summary>
    /// <returns>The next character</returns>
    public char AdvanceCurrentChar() {
      return fSource[idx++];
    }

    /// <summary>
    /// Advance the index until past any whitespace.
    /// </summary>
    public void EatWhiteSpace() {
      while (!Eof()) {
        if (!IsWhiteSpace(GetCurrentChar())) {
          return;
        }
        Advance();
      }
    }

    /// <summary>
    /// Determine if the end of the source text has been reached.
    /// </summary>
    /// <returns>True if the end of the source text has been
    /// reached.</returns>
    public bool Eof() {
      return (idx >= fSource.Length);
    }

    /// <summary>
    /// Get a few characters ahead of the current character.
    /// </summary>
    /// <param name="peek">How many characters to peek ahead for.</param>
    /// <returns>The character that was retrieved.</returns>
    public char GetCurrentChar(int peek) {
      return ((idx + peek) < fSource.Length ? fSource[idx + peek] : (char)0);
    }

    public char GetCurrentChar() {
      return (idx < fSource.Length ? fSource[idx] : (char)0);
    }

    public bool IsTagStart() {
      // Is it a tag?
      char ch = GetCurrentChar();
      if (ch == '<') {
        ch = char.ToUpper(GetCurrentChar(1));
        if ((ch >= 'A') && (ch <= 'Z') || (ch == '!') || (ch == '/')) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Returns next parsed object, could be a Tag or string object (text up to next tag or end of file)
    /// </summary>
    public Object Next() {
      if (Eof()) {
        return null;
      }
      // Is it a tag?
      if (IsTagStart()) {
        return ParseTag();
      }
      // Process Text
      char ch = GetCurrentChar();
      StringBuilder text = new StringBuilder();
      while ((!Eof()) && (!IsTagStart())) {
        if (ch == '&') {
          text.Append(ParseEscape());
        }
        else {
          if (skipMultipleSpace) {
            AddElement(text, ch);
          }
          else {
            text.Append(ch);
          }
          Advance();
        }
        ch = GetCurrentChar();
      }
      return text.ToString();
    }

    /// <summary>
    /// Parse the attribute name.
    /// </summary>
    public void ParseAttributeName(Attribute attr) {
      EatWhiteSpace();
      StringBuilder name = new StringBuilder();
      // get attribute name
      while (!Eof()) {
        char ch = GetCurrentChar();
        if (IsAttrNameEnd(ch)) {
          break;
        }
        name.Append(ch);
        Advance();
      }
      EatWhiteSpace();
      attr.Name = name.ToString();
    }

    /// <summary>
    /// Parse the attribute value
    /// </summary>
    public void ParseAttributeValue(Attribute attr) {
      if (GetCurrentChar() == '=') {
        char ch;
        StringBuilder val = new StringBuilder();
        Advance();
        EatWhiteSpace();
        ch = GetCurrentChar();
        if (IsDelim(ch)) {
          char delim = ch;
          attr.Delim = delim;
          Advance();
          ch = GetCurrentChar();
          while (ch != delim) {
            val.Append(ch);
            Advance();
            ch = GetCurrentChar();
          }
          Advance();
        }
        else {
          while (!Eof() && !IsAttrValueEnd(ch)) {
            val.Append(ch);
            Advance();
            ch = GetCurrentChar();
          }
        }
        attr.Value = val.ToString();
        EatWhiteSpace();
      }
    }

    /// <summary>
    /// Parse an escaped sequence &amp;something;
    /// </summary>
    public string ParseEscape() {
      StringBuilder escaped = new StringBuilder();
      escaped.Append(AdvanceCurrentChar()); // Skip &
      while (!Eof()) {
        if (GetCurrentChar() == ';') {
          break;
        }
        escaped.Append(AdvanceCurrentChar());
      }
      escaped.Append(AdvanceCurrentChar()); // Skip ;
      String esc = escaped.ToString();
      string s = HttpUtility.HtmlDecode(esc);
      if (!String.IsNullOrEmpty(s)) {
        return s;
      }
      return esc;
    }

    /// <summary>
    /// Parse a TAg
    /// </summary>
    protected Tag ParseTag() {
      Advance(); // Skip <
      StringBuilder name = new StringBuilder();
      Tag tag = new Tag();
      char ch;
      // Is it a comment?
      if ((GetCurrentChar() == '!') && (GetCurrentChar(1) == '-') && (GetCurrentChar(2) == '-')) {
        while (!Eof()) {
          if ((GetCurrentChar() == '-') && (GetCurrentChar(1) == '-') && (GetCurrentChar(2) == '>')) {
            break;
          }
          if (GetCurrentChar() != '\r') {
            name.Append(GetCurrentChar());
          }
          Advance();
        }
        name.Append("--");
        Advance();
        Advance();
        Advance();
      }
      else {
        // Find the tag name
        bool first = true;
        char lastCh = (char)0;
        while (!Eof()) {
          ch = GetCurrentChar();
          if (first) {
            if (ch == '/') {
              tag.Closing = true;
            }
            else {
              tag.Opening = true;
            }
          }
          if (IsTagNameEnd(ch)) {
            if (!first && (lastCh == '/')) {
              tag.Closing = true;
            }
            break;
          }
          name.Append(GetCurrentChar());
          Advance();
          lastCh = ch;
          first = false;
        }
        int len = name.Length;
        if (len > 0) {
          if (name[0] == '/') {
            name.Remove(0, 1);
            len--;
          }
          if (len > 0) {
            if (name[len - 1] == '/') {
              name.Remove(len - 1, 1);
            }
          }
        }
        EatWhiteSpace();
        // Get the attributes
        ch = GetCurrentChar();
        while (!IsTagEnd(ch)) {
          Attribute attr = new Attribute();
          ParseAttributeName(attr);
          if (IsTagEnd(ch)) {
            tag.Add(attr);
            break;
          }
          // Get the value(if any)
          ParseAttributeValue(attr);
          tag.Add(attr);
          ch = GetCurrentChar();
        }
        if (ch == '/') {
          //TODO check standard <tag attribute / missedgtsometing, / close the tag
          tag.Closing = true;
          Advance();
          EatWhiteSpace();
          if (GetCurrentChar() == '>') {
            Advance();
          }
        }
        else {
          Advance();
        }
      }
      tag.Name = name.ToString();
      return tag;
    }

    /// <summary>
    /// Append a characted to the buffer doing space compression
    /// </summary>
    private void AddElement(StringBuilder buffer, char ch) {
      if (HTML.HTMLParser.IsWhiteSpace(ch)) {
        char nxt = GetCurrentChar(1);
        if (!HTML.HTMLParser.IsWhiteSpace(nxt)) {
          if ((ch == ' ') || (nxt != '<')) {
            buffer.Append(' ');
          }
        }
      }
      else {
        buffer.Append(ch);
      }
    }
    #endregion Methods

  }

  /// <summary>
  /// The AttributeList class is used to store list of
  /// Attribute classes.
  /// This source code may be used freely under the
  /// Limited GNU Public License(LGPL).
  ///
  /// Written by Jeff Heaton (http://www.jeffheaton.com)
  /// </summary>
  ///
  public class Tag : Attribute {

    #region Fields
    /// <summary>
    /// An internally used Vector.  This vector contains
    /// the entire list of attributes.
    /// </summary>
    private ArrayList fAttributes;
    /// <summary>
    /// Is closing Tag e.g. &lt;/tag&gt;
    /// </summary>
    private bool fClose = false;
    /// <summary>
    /// Is opening Tag e.g. &lt;tag&gt;
    /// </summary>
    private bool fOpen = false;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Create a new, empty, attribute list.
    /// </summary>
    public Tag() : base("", "", (char)0) {
      fAttributes = new ArrayList();
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// A list of the attributes in this AttributeList
    /// </summary>
    public ArrayList Attributes {
      get {
        return fAttributes;
      }
    }

    public bool Closing {
      get {
        return fClose;
      }
      set {
        fClose = value;
      }
    }

    /// <summary>
    /// How many attributes are in this AttributeList?
    /// </summary>
    public int Count {
      get {
        return fAttributes.Count;
      }
    }

    /// <summary>
    /// Is collapsed Tag e.g. &lt;tag/&gt;
    /// </summary>
    public bool IsCollapsed {
      get {
        return fOpen && fClose;
      }
    }

    public bool Opening {
      get {
        return fOpen;
      }
      set {
        fOpen = value;
      }
    }
    #endregion Properties

    #region Indexers
    /// <summary>
    /// Access the individual attributes
    /// </summary>
    public Attribute this[int index] {
      get {
        return (index < fAttributes.Count ? (Attribute)fAttributes[index] : null);
      }
    }

    /// <summary>
    /// Access the individual attributes by name.
    /// </summary>
    public Attribute this[string index] {
      get {
        index = index.ToLower();
        int i = 0;
        while (this[i] != null) {
          if (this[i].Name.ToLower().Equals(index)) {
            return this[i];
          }
          i++;
        }
        return null;
      }
    }
    #endregion Indexers

    #region Methods
    /// <summary>
    /// Add the specified attribute to the list of attributes.
    /// </summary>
    /// <param name="a">An attribute to add to this AttributeList.</param>
    public void Add(Attribute a) {
      fAttributes.Add(a);
    }

    /// <summary>
    /// Clear all attributes from this AttributeList and return it to a empty state.
    /// </summary>
    public void Clear() {
      fAttributes.Clear();
    }

    /// <summary>
    /// Make an exact copy of this object using the cloneable
    /// interface.
    /// </summary>
    /// <returns>A new object that is a clone of the specified
    /// object.</returns>
    public override Object Clone() {
      Tag rtn = new Tag();
      rtn.Name = Name;
      rtn.Value = Value;
      rtn.Delim = Delim;
      for (int i = 0; i < fAttributes.Count; i++) {
        rtn.Add((Attribute)this[i].Clone());
      }
      return rtn;
    }

    /// <summary>
    /// Returns true of this AttributeList is empty, with no attributes.
    /// </summary>
    /// <returns>True if this AttributeList is empty, false otherwise.</returns>
    public bool IsEmpty() {
      return (fAttributes.Count <= 0);
    }

    /// <summary>
    /// If there is already an attribute with the specified name,
    /// it will have its value changed to match the specified
    /// value. If there is no Attribute with the specified name,
    /// one will be created. This method is case-insensitive.
    /// </summary>
    /// <param name="name">The name of the Attribute to edit or create. Case-insensitive.</param>
    /// <param name="value">The value to be held in this attribute.</param>
    public void Set(string name, string value) {
      if (name == null) {
        return;
      }
      if (value == null) {
        value = "";
      }
      Attribute a = this[name];
      if (a == null) {
        a = new Attribute(name, value);
        Add(a);
      }
      else {
        a.Value = value;
      }
    }

    /// <summary>
    /// Convert Tag to a string
    /// </summary>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("<");
      if (!fOpen && fClose) {
        sb.Append("/");
      }
      sb.Append(base.ToString());
      if (fAttributes.Count > 0) {
        for (int i = 0; i < fAttributes.Count; i++) {
          sb.Append(' ');
          sb.Append(fAttributes[i].ToString());
        }
      }
      if (fOpen && fClose) {
        sb.Append("/");
      }
      sb.Append(">");
      return sb.ToString();
    }
    #endregion Methods

  }

}