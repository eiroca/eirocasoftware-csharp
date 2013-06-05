/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
namespace WikiHelper.gui {
  
  partial class CreateForm {
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
    	this.bExport = new System.Windows.Forms.Button();
    	this.lbTemplate = new System.Windows.Forms.ListBox();
    	this.lbPages = new System.Windows.Forms.ListBox();
    	this.label1 = new System.Windows.Forms.Label();
    	this.label2 = new System.Windows.Forms.Label();
    	this.SuspendLayout();
    	// 
    	// bCancel
    	// 
    	this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    	this.bCancel.Location = new System.Drawing.Point(288, 272);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 12;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// bExport
    	// 
    	this.bExport.DialogResult = System.Windows.Forms.DialogResult.OK;
    	this.bExport.Location = new System.Drawing.Point(376, 272);
    	this.bExport.Name = "bExport";
    	this.bExport.Size = new System.Drawing.Size(75, 23);
    	this.bExport.TabIndex = 13;
    	this.bExport.Text = "&Create";
    	this.bExport.UseVisualStyleBackColor = true;
    	// 
    	// lbTemplate
    	// 
    	this.lbTemplate.FormattingEnabled = true;
    	this.lbTemplate.Location = new System.Drawing.Point(12, 28);
    	this.lbTemplate.Name = "lbTemplate";
    	this.lbTemplate.Size = new System.Drawing.Size(439, 108);
    	this.lbTemplate.TabIndex = 14;
    	// 
    	// lbPages
    	// 
    	this.lbPages.FormattingEnabled = true;
    	this.lbPages.Location = new System.Drawing.Point(12, 158);
    	this.lbPages.Name = "lbPages";
    	this.lbPages.Size = new System.Drawing.Size(439, 108);
    	this.lbPages.TabIndex = 22;
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(12, 9);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(100, 16);
    	this.label1.TabIndex = 23;
    	this.label1.Text = "Template";
    	// 
    	// label2
    	// 
    	this.label2.Location = new System.Drawing.Point(12, 139);
    	this.label2.Name = "label2";
    	this.label2.Size = new System.Drawing.Size(100, 16);
    	this.label2.TabIndex = 24;
    	this.label2.Text = "New page(s)";
    	// 
    	// CreateForm
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(465, 309);
    	this.Controls.Add(this.label2);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.lbPages);
    	this.Controls.Add(this.lbTemplate);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bExport);
    	this.Name = "CreateForm";
    	this.Text = "Create page(s)";
    	this.ResumeLayout(false);
    }
    private System.Windows.Forms.ListBox lbTemplate;
    private System.Windows.Forms.ListBox lbPages;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button bExport;
    private System.Windows.Forms.Button bCancel;
    
  }
  
}
