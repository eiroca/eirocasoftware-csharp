namespace DotBits.Configuration {
  using System;
  using System.Collections;
  using System.ComponentModel;

  /// <summary>
  /// Modified class from the CustomClass originally created by Venu Madhav.
  /// Venu's DataTable and DataColumn objects have been removed and variables and
  /// class names have been renamed. The derived DynamicProperty class below now contains 
  /// the property data. This is a simpler implementation becuase the backing store for 
  /// the property data is the XML Config file - no need for another set of objects to hold 
  /// the data before it gets written back to the config file (although actually holding the
  /// property data in the PropertyDescriptor class is probably a little unorthodox).
  /// CustomClass implements ICustomTypeDescriptor and derives ExpandableObjectConverter.
  /// This class can be instantiated and properties to this class can
  /// be added dynamically using AddProperty(string propName, object propValue, string propDesc, 
  /// string propCat,  System.Type propType, bool isReadOnly, bool isExpandable). 
  /// </summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class CustomClass : Component, ICustomTypeDescriptor {
    #region Fields
    //Private members
    private PropertyDescriptorCollection propertyCollection;
    //Max length will be used help control the layout of the Windows Form and PropertyGrid
    //or the table of controls in an ASP.Net page. The Form or Webpage layout will be adjusted
    //dynamically depending on length of these values.
    private int _maxLength;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Constructor of CustomClass which initializes the new PropertyDescriptorCollection.
    /// </summary>
    public CustomClass() {
      propertyCollection = new PropertyDescriptorCollection(new PropertyDescriptor[]{});
    }
    #endregion Constructors

    #region Properties

    public int MaxLength
    {
      get { return _maxLength; }
              set { if (value > _maxLength) _maxLength = value; }
    }

    #endregion Properties

    #region Indexers
    //Indexer for this class - returns a DynamicProperty by index position.
    public DynamicProperty this[int index] {
      get { return (DynamicProperty)propertyCollection[index]; }
    }

    //Overloaded Indexer for this class - returns a DynamicProperty by name.
    public DynamicProperty this[string name] {
      get { return (DynamicProperty)propertyCollection[name]; }
    }
    #endregion Indexers

    #region Methods
    /// <summary>
    /// Adds a property into the CustomClass.
    /// </summary>
    /// <param name="propName">Name of the property that needs to be added.</param>
    /// <param name="propValue">Value of the property that needs to be added.</param>
    /// <param name="propDesc">Description of the property that needs to be added.</param>
    /// <param name="propCat">The category to display this property in.</param>
    /// <param name="isReadOnly">Sets the property value to readonly in the property grid.</param>
    /// <param name="isExpandable">Tells the property grid that this property is expandable.</param>
    /// <param name="propType">DataType of the property that needs to be added.</param>
    public void AddProperty(string propName, object propValue, string propDesc, string propCat, System.Type propType, bool isReadOnly, bool isExpandable) {
      DynamicProperty p = new DynamicProperty(propName, propValue, propDesc, propCat, propType, isReadOnly, isExpandable);
      propertyCollection.Add(p);
      //Set our layout helper value.
      this.MaxLength = propName.Length;
      this.MaxLength = propValue.ToString().Length;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public AttributeCollection GetAttributes() {
      return(TypeDescriptor.GetAttributes(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string GetClassName() {
      return(TypeDescriptor.GetClassName(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string GetComponentName() {
      return(TypeDescriptor.GetComponentName(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public TypeConverter GetConverter() {
      return(TypeDescriptor.GetConverter(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public EventDescriptor GetDefaultEvent() {
      return(TypeDescriptor.GetDefaultEvent(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public PropertyDescriptor GetDefaultProperty() {
      PropertyDescriptorCollection props = GetAllProperties();
      if (props.Count > 0) return(props[0]);
      else return(null);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="editorBaseType"></param>
    /// <returns></returns>
    public object GetEditor(Type editorBaseType) {
      return(TypeDescriptor.GetEditor(this, editorBaseType,true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public EventDescriptorCollection GetEvents(Attribute[] attributes) {
      return(TypeDescriptor.GetEvents(this, attributes, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public EventDescriptorCollection GetEvents() {
      return(TypeDescriptor.GetEvents(this, true));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
      return(GetAllProperties());
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public PropertyDescriptorCollection GetProperties() {
      return(GetAllProperties());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="pd"></param>
    /// <returns></returns>
    public object GetPropertyOwner(PropertyDescriptor pd) {
      return(this);
    }

    /// <summary>
    ///     Helper method to return the PropertyDescriptorCollection or our Dynamic Properties
    /// </summary>
    /// <param name="pd"></param>
    /// <returns></returns>
    private PropertyDescriptorCollection GetAllProperties() {
      return propertyCollection;
    }
    #endregion Methods

    #region Nested Types
    /// <summary>
    ///     This is the Property class this will be dynamically added to the class at runtime.
    ///     These classes are returned in the PropertyDescriptorCollection of the GetAllProperties
    ///     method of the custom class.
    /// </summary>
    /// <param name="pd"></param>
    /// <returns></returns>
    public class DynamicProperty : PropertyDescriptor {
      #region Fields
      private bool isExpandable;
      private bool isReadOnly;
      private string propCategory;
      private string propDescription;
      private string propName;
      private Type propType;
      private object propValue;
      #endregion Fields

      #region Constructors
      public DynamicProperty(string pName, object pValue, string pDesc, string pCat, Type pType, bool readOnly, bool expandable) : base(pName, new Attribute[]{}) {
        propName  = pName;
        propValue   = pValue;
        propDescription = pDesc;
        propCategory    = pCat;
        propType    = pType;
        isReadOnly  = readOnly;
        isExpandable    = expandable;
      }
      #endregion Constructors

      #region Properties
      public override string Category {
        get { return propCategory; }
      }

      public override System.Type ComponentType {
        get { return null; }
      }

      public override string Description {
        get { return propDescription; }
      }

      public override bool IsReadOnly {
        get { return isReadOnly; }
      }

      public override System.Type PropertyType {
        get { return propType; }
      }
      #endregion Properties

      #region Methods
      public override bool CanResetValue(object component) {
        return true;
      }

      public override object GetValue(object component) {
        return propValue;
      }

      public override void ResetValue(object component) {
        propValue = null;
      }

      public override void SetValue(object component, object value ) {
        propValue = value;
      }

      public override bool ShouldSerializeValue(object component) {
        return false;
      }
      #endregion Methods
    }

    #endregion Nested Types
    
  }
  
}