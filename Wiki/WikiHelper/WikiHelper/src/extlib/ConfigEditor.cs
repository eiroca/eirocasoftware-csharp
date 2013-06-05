namespace DotBits.Configuration {
  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.Drawing;
  using System.Windows.Forms;
  using System.Xml;

  /// <summary>
  /// The application will be compiled both as a .DLL and a .EXE. The DLL can be referenced
  /// by other Windows Forms applications and used to instantiate the ConfigEditor form. 
  /// If ConfigEditor is called from the DLL - static void Main(string[] args) will not
  /// be called - in which case the constructor will determine the configuration
  /// filename.
  /// 
  /// If the .EXE is called - optional args can be supplied to specify the Xml configuration
  /// file to use.
  /// 
  /// Configuration Loading and Saving methods have been kept clean from the interface
  /// event handlers. Compiled as a .DLL these methods could be called from an administrative
  /// ASP.Net Web Form which would could add text controls dynamically to the page to build a
  /// Web Form that contains all the key value pairs for all the properties in the custom
  /// class - allowing the configuration of the Web application to be set or changed remotely.
  /// </summary>    
  public class ConfigEditor : System.Windows.Forms.Form {
    #region Fields
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnLoad;
    //Controls
    private System.Windows.Forms.Button btnSave;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.Windows.Forms.TextBox txtConfigurationFile;
    //private data
    private string _configFilename = "";
    private bool _isDirty;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// If called from static void Main(string[] args) during a .EXE startup, 
    /// then the the value of this.ConfigFilename may be overwritten by an optional 
    /// argument passed to Main.
    /// </summary>
    public ConfigEditor() {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();
      //Set the configuration file value based on a couple of rules.
      //If Bin is the last directory in the Executable path, then it's in a
      //Web/Bin directory and we'll load Web.config from the directory above.
      //If not, then we'll look for a configuration file in the executable
      //directory *.Config (if for some reason there is more than one - which there
      //shoudn't be - we'll take the first one).
      //If we're being called from Main(string[] args) and a parameter was
      //passed then this value will be replaced when Main(string[] args) completes.
      string dir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
      if (dir.LastIndexOf("\\bin") == dir.Length - 4) {
        //\Bin is at the end of the directory path and the application has been called
        //from a Web project folder.
        this.ConfigFilename = dir.Remove(dir.Length - 3, 3) + "Web.Config";
      }
      else {
        string[] configFiles = System.IO.Directory.GetFiles(dir, "*.Config");
        if (configFiles.Length > 0) {
          this.ConfigFilename = configFiles[0];
        }
        else {
          this.ConfigFilename = "";
        }
      }
    }
    #endregion Constructors

    #region Properties

    //public properties
    public virtual string ConfigFilename {
      get  { return _configFilename; }
      set  { _configFilename = value; this.txtConfigurationFile.Text = _configFilename; }
    }

    private bool IsDirty {
      get { return _isDirty;}
      set { _isDirty = value;}
    }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Load the Xml Configuration document and populate our CustomClass with a dynamic property
    /// for each of the supported configuration sections. We're only supporting three sections here.
    /// The default appSettings, plus our standard ApplicationConfiguration
    /// and CommonConfiguration section handlers. These handlers are derived from IConfigurationSectionHandler.
    /// They have extended support for the Description attribute in addition to the Key, Value 
    /// pair attributes in the Xml configuration file.
    /// This could easily be extended to include support for any section under the configuration
    /// section that has the <add key="value" value="value"/> structure (assuming you haven't written a
    /// completely new Xml structure for your custom configuration section).
    /// </summary>
    public CustomClass LoadConfiguration(string configurationFile) {
      CustomClass customClass = new CustomClass();
      try {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(configurationFile);
        XmlNode configuration = xmlDoc.SelectSingleNode("configuration");
        //Build the node list
        XmlNodeList sectionList = configuration.ChildNodes;
        for(int y = 0; y <  sectionList.Count; y++) {
          XmlNodeList settingsList = xmlDoc.SelectNodes("configuration/" + sectionList[y].Name + "/add");
          if (settingsList.Count != 0 && settingsList != null) {
            //Add a property to customClass for each node found.
            for(int i = 0; i <  settingsList.Count; i++) {
              XmlAttribute atrribKey = settingsList[i].Attributes["key"];
              XmlAttribute attribValue = settingsList[i].Attributes["value"];
              XmlAttribute attribDescription = settingsList[i].Attributes["description"];
              if(atrribKey != null && attribValue != null) {
                //If there's no description for the key - assign the name to the description.
                //The description attribute is displayed below the name in the property grid.
                if (attribDescription == null) {
                  attribDescription = atrribKey;
                }
                //We'll at least test to see if it's a boolean property and set the type here
                //to force the property grid to display a dropdown list of True or False.
                Type propType;
                if (attribValue.Value.ToLower() == "true" || attribValue.Value.ToLower() == "false") {
                  propType = typeof(System.Boolean);
                }
                else {
                  propType = typeof(System.String);
                }
                //Now add the property
                customClass.AddProperty(atrribKey.Value.ToString(), attribValue.Value.ToString(),
                attribDescription.Value.ToString(), sectionList[y].Name, propType, false, false);
              }
            }
          }
        }
        xmlDoc = null;
      }
      catch(Exception ex) {
        throw ex;
      }
      return customClass;
    }

    /// <summary>
    /// We're only supporting three sections here at the moment.
    /// The default appSettings, plus our standard ApplicationConfiguration
    /// and CommonConfiguration section handlers. These handlers have extended support for
    /// the Description attribute in addition to the Key, Value pair attributes 
    /// in the Xml configuration file.
    /// </summary>
    public void SaveConfiguration(string configurationFile, CustomClass customClass) {
      try {
        //Reload the configuration file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(configurationFile);
        //Save a backup version
        xmlDoc.Save(configurationFile + "_bak");
        //Populate our property collection.
        PropertyDescriptorCollection props = customClass.GetProperties();
        //Repolulate the three supported sections
        RepopulateXmlSection("ApplicationConfiguration", xmlDoc, props);
        RepopulateXmlSection("CommonConfiguration", xmlDoc, props);
        RepopulateXmlSection("appSettings", xmlDoc, props);
        xmlDoc.Save(configurationFile);
      }
      catch(Exception ex) {
        throw ex;
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null)  {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// Check the state of IsDirty before closing the form and prompt the
    /// user to save their changes if IsDirty == true.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, System.EventArgs e) {
      if (this.IsDirty == true) {
        DialogResult result = MessageBox.Show("Save Application Configuration changes before closing?", "Application Configuration", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (result == DialogResult.Yes) {
          try {
            SaveConfiguration(this.ConfigFilename, (CustomClass)this.propertyGrid1.SelectedObject);
            MessageBox.Show("Application Configuration changes have been saved.", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
          }
          catch(Exception ex) {
            MessageBox.Show("Failed to btnSave configuration. Reason(" + ex.Message + ")", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
        else if (result == DialogResult.No) {
          this.Close();
        }
      }
      else {
        this.Close();
      }
    }

    /// <summary>
    /// Load a selected configuration file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, System.EventArgs e) {
      openFileDialog1.Filter = "Configuration Files (*.config)| *.config";
      DialogResult result = openFileDialog1.ShowDialog();
      if (result == DialogResult.OK) {
        this.ConfigFilename = openFileDialog1.FileName;
        this.propertyGrid1.SelectedObject = LoadConfiguration(this.ConfigFilename);
        LayoutForm((CustomClass)this.propertyGrid1.SelectedObject);
        this.propertyGrid1.Focus();
      }
    }

    /// <summary>
    /// Save the state of the property grid back to the Xml configuration file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, System.EventArgs e) {
      try {
        SaveConfiguration(this.ConfigFilename, (CustomClass)this.propertyGrid1.SelectedObject);
        MessageBox.Show("Application Configuration changes have been saved.", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.IsDirty = false;
        this.propertyGrid1.Focus();
      }
      catch(Exception ex) {
        MessageBox.Show("Failed to btnSave configuration. Reason(" + ex.Message + ")", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    /// <summary>
    /// On FormLoad event handler which will call the LoadConfiguration helper method.
    /// /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConfigEditor_Load(object sender, System.EventArgs e) {
      try {
        if (System.IO.File.Exists(this.ConfigFilename)) {
          this.propertyGrid1.SelectedObject = LoadConfiguration(this.ConfigFilename);
          LayoutForm((CustomClass)this.propertyGrid1.SelectedObject);
        }
      }
      catch (Exception ex) {
        MessageBox.Show("Failed to load the configuration file. Reason(" + ex.Message + ")", "Application Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      this.propertyGrid1.Focus();
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigEditor));
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.txtConfigurationFile = new System.Windows.Forms.TextBox();
      this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
      this.btnLoad = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.lblTitle = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      //
      // btnSave
      //
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(461, 385);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(120, 24);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "&Save Settings";
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      //
      // btnClose
      //
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.btnClose.Location = new System.Drawing.Point(589, 385);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(64, 24);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "&Done";
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      //
      // txtConfigurationFile
      //
      this.txtConfigurationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
      this.txtConfigurationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtConfigurationFile.Location = new System.Drawing.Point(7, 353);
      this.txtConfigurationFile.Name = "txtConfigurationFile";
      this.txtConfigurationFile.ReadOnly = true;
      this.txtConfigurationFile.Size = new System.Drawing.Size(614, 20);
      this.txtConfigurationFile.TabIndex = 16;
      //
      // propertyGrid1
      //
      this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
      this.propertyGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
      this.propertyGrid1.Location = new System.Drawing.Point(7, 49);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
      this.propertyGrid1.Size = new System.Drawing.Size(646, 296);
      this.propertyGrid1.TabIndex = 0;
      this.propertyGrid1.ToolbarVisible = false;
      this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
      //
      // btnLoad
      //
      this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoad.Location = new System.Drawing.Point(629, 353);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(24, 20);
      this.btnLoad.TabIndex = 17;
      this.btnLoad.Text = "...";
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      //
      // groupBox1
      //
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.BackColor = System.Drawing.Color.White;
      this.groupBox1.Controls.Add(this.pictureBox1);
      this.groupBox1.Controls.Add(this.lblTitle);
      this.groupBox1.Location = new System.Drawing.Point(-16, -24);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(692, 66);
      this.groupBox1.TabIndex = 18;
      this.groupBox1.TabStop = false;
      //
      // pictureBox1
      //
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(636, 28);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(48, 31);
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      //
      // lblTitle
      //
      this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.Location = new System.Drawing.Point(32, 29);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(272, 29);
      this.lblTitle.TabIndex = 0;
      this.lblTitle.Text = "Application Settings";
      //
      // ConfigEditor
      //
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(662, 416);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.propertyGrid1);
      this.Controls.Add(this.txtConfigurationFile);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnSave);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimumSize = new System.Drawing.Size(670, 450);
      this.Name = "ConfigEditor";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Application Configuration";
      this.Load += new System.EventHandler(this.ConfigEditor_Load);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    /// <summary>
    /// This method will look at the Maximum value of either a property name or it's
    /// value (which ever is greater) and adjust the width of the form accordingly.
    /// The Property Grid is set to Anchor Left, Top and Right so it will expand along
    /// with the form.
    /// </summary>
    /// <param name="customClass"></param>
    private void LayoutForm(CustomClass customClass) {
      //MaxLength is the number of characters. At our current
      //font size we need to allow for an acceptable multiplier in pixels.
      if (customClass.MaxLength > 95) {
        this.Width = 665;
      }
      else {
        this.Width = customClass.MaxLength  * 7;
      }
    }

    /// <summary>
    /// Property value changed - set the IsDirty flag to true.
    /// </summary>
    private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
      this.IsDirty = true;
    }

    /// <summary>
    /// Repopulates the Xml section in the Xml Document by finding the matching key name
    /// values in the property collection and Xml node list.
    /// </summary>
    private void RepopulateXmlSection(string sectionName, XmlDocument xmlDoc, PropertyDescriptorCollection props) {
      XmlNodeList nodes = xmlDoc.SelectNodes("configuration/" + sectionName + "/add");
      for(int i = 0; i <  nodes.Count; i++) {
        //Find the property in the property collection with the same name as the current node in the Xml document
        CustomClass.DynamicProperty property = (CustomClass.DynamicProperty)props[nodes[i].Attributes["key"].Value];
        if (property != null) {
          //Set the node value to the property value (which will have been set in the Property grid.
          nodes[i].Attributes["value"].Value = property.GetValue(null).ToString();
          //Check to see if we have a value for our extended custom xml attribute - the description attribute.
          //The default description is the property name when no descripyion attribute is present.
          //If they're not the same - then a value was passed when the property was created.
          if (property.Description != property.Name) {
            //double check here that there is in fact a description attribute
            if (nodes[i].Attributes["description"] != null) {
              nodes[i].Attributes["description"].Value = property.Description;
            }
          }
        }
      }
    }
    #endregion Methods
    
  }
  
}