/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
namespace WikiHelper.gui {
  
  partial class ReplaceForm {
    /// <summary>
    /// Designer variable used to keep track of non-visual components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    
    /// <summary>
    /// Disposes resources used by the form.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
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
    private void InitializeComponent() {
    	this.bCancel = new System.Windows.Forms.Button();
    	this.bDoReplace = new System.Windows.Forms.Button();
    	this.lbFrom = new System.Windows.Forms.ListBox();
    	this.bAdd = new System.Windows.Forms.Button();
    	this.bDelete = new System.Windows.Forms.Button();
    	this.bReplace = new System.Windows.Forms.Button();
    	this.label1 = new System.Windows.Forms.Label();
    	this.label2 = new System.Windows.Forms.Label();
    	this.iFrom = new System.Windows.Forms.TextBox();
    	this.iTo = new System.Windows.Forms.TextBox();
    	this.lbTo = new System.Windows.Forms.ListBox();
    	this.label3 = new System.Windows.Forms.Label();
    	this.iCategory = new System.Windows.Forms.ComboBox();
    	this.SuspendLayout();
    	// 
    	// bCancel
    	// 
    	this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    	this.bCancel.Location = new System.Drawing.Point(107, 265);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 8;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// bDoReplace
    	// 
    	this.bDoReplace.DialogResult = System.Windows.Forms.DialogResult.OK;
    	this.bDoReplace.Location = new System.Drawing.Point(195, 265);
    	this.bDoReplace.Name = "bDoReplace";
    	this.bDoReplace.Size = new System.Drawing.Size(75, 23);
    	this.bDoReplace.TabIndex = 9;
    	this.bDoReplace.Text = "&Replace";
    	this.bDoReplace.UseVisualStyleBackColor = true;
    	// 
    	// lbFrom
    	// 
    	this.lbFrom.FormattingEnabled = true;
    	this.lbFrom.Location = new System.Drawing.Point(12, 91);
    	this.lbFrom.Name = "lbFrom";
    	this.lbFrom.Size = new System.Drawing.Size(126, 134);
    	this.lbFrom.TabIndex = 6;
    	this.lbFrom.SelectedIndexChanged += new System.EventHandler(this.LbFromSelectedIndexChanged);
    	// 
    	// bAdd
    	// 
    	this.bAdd.Location = new System.Drawing.Point(12, 60);
    	this.bAdd.Name = "bAdd";
    	this.bAdd.Size = new System.Drawing.Size(75, 23);
    	this.bAdd.TabIndex = 3;
    	this.bAdd.Text = "Add";
    	this.bAdd.UseVisualStyleBackColor = true;
    	this.bAdd.Click += new System.EventHandler(this.BAddClick);
    	// 
    	// bDelete
    	// 
    	this.bDelete.Location = new System.Drawing.Point(107, 60);
    	this.bDelete.Name = "bDelete";
    	this.bDelete.Size = new System.Drawing.Size(75, 23);
    	this.bDelete.TabIndex = 4;
    	this.bDelete.Text = "Delete";
    	this.bDelete.UseVisualStyleBackColor = true;
    	this.bDelete.Click += new System.EventHandler(this.BDeleteClick);
    	// 
    	// bReplace
    	// 
    	this.bReplace.Location = new System.Drawing.Point(195, 60);
    	this.bReplace.Name = "bReplace";
    	this.bReplace.Size = new System.Drawing.Size(75, 23);
    	this.bReplace.TabIndex = 5;
    	this.bReplace.Text = "Replace";
    	this.bReplace.UseVisualStyleBackColor = true;
    	this.bReplace.Click += new System.EventHandler(this.BReplaceClick);
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(12, 8);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(59, 20);
    	this.label1.TabIndex = 18;
    	this.label1.Text = "Replace:";
    	// 
    	// label2
    	// 
    	this.label2.Location = new System.Drawing.Point(12, 34);
    	this.label2.Name = "label2";
    	this.label2.Size = new System.Drawing.Size(100, 23);
    	this.label2.TabIndex = 19;
    	this.label2.Text = "Into:";
    	// 
    	// iFrom
    	// 
    	this.iFrom.Location = new System.Drawing.Point(68, 5);
    	this.iFrom.Name = "iFrom";
    	this.iFrom.Size = new System.Drawing.Size(202, 20);
    	this.iFrom.TabIndex = 1;
    	// 
    	// iTo
    	// 
    	this.iTo.Location = new System.Drawing.Point(68, 31);
    	this.iTo.Name = "iTo";
    	this.iTo.Size = new System.Drawing.Size(202, 20);
    	this.iTo.TabIndex = 2;
    	// 
    	// lbTo
    	// 
    	this.lbTo.FormattingEnabled = true;
    	this.lbTo.Location = new System.Drawing.Point(144, 91);
    	this.lbTo.Name = "lbTo";
    	this.lbTo.Size = new System.Drawing.Size(126, 134);
    	this.lbTo.TabIndex = 22;
    	this.lbTo.TabStop = false;
    	this.lbTo.SelectedIndexChanged += new System.EventHandler(this.LbToSelectedIndexChanged);
    	// 
    	// label3
    	// 
    	this.label3.Location = new System.Drawing.Point(10, 233);
    	this.label3.Name = "label3";
    	this.label3.Size = new System.Drawing.Size(58, 23);
    	this.label3.TabIndex = 23;
    	this.label3.Text = "Category:";
    	// 
    	// iCategory
    	// 
    	this.iCategory.FormattingEnabled = true;
    	this.iCategory.Location = new System.Drawing.Point(68, 231);
    	this.iCategory.Name = "iCategory";
    	this.iCategory.Size = new System.Drawing.Size(202, 21);
    	this.iCategory.TabIndex = 7;
    	// 
    	// ReplaceForm
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(284, 298);
    	this.Controls.Add(this.label3);
    	this.Controls.Add(this.iCategory);
    	this.Controls.Add(this.lbTo);
    	this.Controls.Add(this.iTo);
    	this.Controls.Add(this.iFrom);
    	this.Controls.Add(this.label2);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.bReplace);
    	this.Controls.Add(this.bDelete);
    	this.Controls.Add(this.bAdd);
    	this.Controls.Add(this.lbFrom);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bDoReplace);
    	this.Name = "ReplaceForm";
    	this.Text = "Replace";
    	this.ResumeLayout(false);
    	this.PerformLayout();
    }
    private System.Windows.Forms.Button bDoReplace;
    private System.Windows.Forms.ComboBox iCategory;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ListBox lbTo;
    private System.Windows.Forms.TextBox iTo;
    private System.Windows.Forms.TextBox iFrom;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button bReplace;
    private System.Windows.Forms.Button bDelete;
    private System.Windows.Forms.Button bAdd;
    private System.Windows.Forms.ListBox lbFrom;
    private System.Windows.Forms.Button bCancel;
    
  }
  
}
