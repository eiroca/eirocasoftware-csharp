/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
using System;

namespace WikiHelper.gui {

  partial class AddressBookExport {
    /// <summary>
    /// Designer variable used to keep track of non-visual components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    
    /// <summary>
    /// Disposes resources used by the form.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }
    
    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent()
    {
    	this.iCategory = new System.Windows.Forms.ComboBox();
    	this.label1 = new System.Windows.Forms.Label();
    	this.bExport = new System.Windows.Forms.Button();
    	this.bCancel = new System.Windows.Forms.Button();
    	this.groupBox4 = new System.Windows.Forms.GroupBox();
    	this.iContactsName = new System.Windows.Forms.TextBox();
    	this.cbContacts = new System.Windows.Forms.CheckBox();
    	this.groupBox4.SuspendLayout();
    	this.SuspendLayout();
    	// 
    	// iCategory
    	// 
    	this.iCategory.FormattingEnabled = true;
    	this.iCategory.Location = new System.Drawing.Point(66, 7);
    	this.iCategory.Name = "iCategory";
    	this.iCategory.Size = new System.Drawing.Size(121, 21);
    	this.iCategory.TabIndex = 1;
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(8, 9);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(58, 23);
    	this.label1.TabIndex = 0;
    	this.label1.Text = "Category:";
    	// 
    	// bExport
    	// 
    	this.bExport.DialogResult = System.Windows.Forms.DialogResult.OK;
    	this.bExport.Location = new System.Drawing.Point(148, 93);
    	this.bExport.Name = "bExport";
    	this.bExport.Size = new System.Drawing.Size(75, 23);
    	this.bExport.TabIndex = 11;
    	this.bExport.Text = "&Export";
    	this.bExport.UseVisualStyleBackColor = true;
    	this.bExport.Click += new System.EventHandler(this.BExportClick);
    	// 
    	// bCancel
    	// 
    	this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    	this.bCancel.Location = new System.Drawing.Point(60, 93);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 10;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// groupBox4
    	// 
    	this.groupBox4.Controls.Add(this.iContactsName);
    	this.groupBox4.Controls.Add(this.cbContacts);
    	this.groupBox4.Location = new System.Drawing.Point(8, 35);
    	this.groupBox4.Name = "groupBox4";
    	this.groupBox4.Size = new System.Drawing.Size(215, 52);
    	this.groupBox4.TabIndex = 5;
    	this.groupBox4.TabStop = false;
    	this.groupBox4.Text = "Export Contacts";
    	// 
    	// iContactsName
    	// 
    	this.iContactsName.Location = new System.Drawing.Point(84, 21);
    	this.iContactsName.Name = "iContactsName";
    	this.iContactsName.Size = new System.Drawing.Size(121, 20);
    	this.iContactsName.TabIndex = 4;
    	this.iContactsName.Text = "contacts.csv";
    	// 
    	// cbContacts
    	// 
    	this.cbContacts.Location = new System.Drawing.Point(11, 19);
    	this.cbContacts.Name = "cbContacts";
    	this.cbContacts.Size = new System.Drawing.Size(67, 24);
    	this.cbContacts.TabIndex = 3;
    	this.cbContacts.Text = "export to";
    	this.cbContacts.UseVisualStyleBackColor = true;
    	// 
    	// AddressBookExport
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(237, 125);
    	this.Controls.Add(this.groupBox4);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bExport);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.iCategory);
    	this.Name = "AddressBookExport";
    	this.Text = "Address Book Export";
    	this.groupBox4.ResumeLayout(false);
    	this.groupBox4.PerformLayout();
    	this.ResumeLayout(false);
    }
    private System.Windows.Forms.TextBox iContactsName;
    private System.Windows.Forms.CheckBox cbContacts;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Button bExport;
    private System.Windows.Forms.Button bCancel;
    private System.Windows.Forms.ComboBox iCategory;
    private System.Windows.Forms.Label label1;
    
    void BExportClick(object sender, System.EventArgs e) {
      if  (!iCategory.Items.Contains(iCategory.Text)) {
        iCategory.Items.Add(iCategory.Text);
      }
    }
    
  }
}
