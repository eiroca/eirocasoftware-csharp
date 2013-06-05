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

  partial class PageExport {
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
    	this.label1 = new System.Windows.Forms.Label();
    	this.bExport = new System.Windows.Forms.Button();
    	this.bCancel = new System.Windows.Forms.Button();
    	this.iPages = new System.Windows.Forms.TextBox();
    	this.iOutDir = new System.Windows.Forms.TextBox();
    	this.label2 = new System.Windows.Forms.Label();
    	this.SuspendLayout();
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(8, 9);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(108, 23);
    	this.label1.TabIndex = 0;
    	this.label1.Text = "Page(s) (1 per line):";
    	// 
    	// bExport
    	// 
    	this.bExport.DialogResult = System.Windows.Forms.DialogResult.OK;
    	this.bExport.Location = new System.Drawing.Point(95, 216);
    	this.bExport.Name = "bExport";
    	this.bExport.Size = new System.Drawing.Size(75, 23);
    	this.bExport.TabIndex = 11;
    	this.bExport.Text = "&Export";
    	this.bExport.UseVisualStyleBackColor = true;
    	// 
    	// bCancel
    	// 
    	this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    	this.bCancel.Location = new System.Drawing.Point(7, 216);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 10;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// iPages
    	// 
    	this.iPages.AcceptsReturn = true;
    	this.iPages.Location = new System.Drawing.Point(8, 35);
    	this.iPages.Multiline = true;
    	this.iPages.Name = "iPages";
    	this.iPages.Size = new System.Drawing.Size(162, 122);
    	this.iPages.TabIndex = 12;
    	// 
    	// iOutDir
    	// 
    	this.iOutDir.Location = new System.Drawing.Point(8, 185);
    	this.iOutDir.Name = "iOutDir";
    	this.iOutDir.Size = new System.Drawing.Size(159, 20);
    	this.iOutDir.TabIndex = 14;
    	// 
    	// label2
    	// 
    	this.label2.Location = new System.Drawing.Point(8, 166);
    	this.label2.Name = "label2";
    	this.label2.Size = new System.Drawing.Size(100, 16);
    	this.label2.TabIndex = 15;
    	this.label2.Text = "Output dir:";
    	// 
    	// PageExport
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(179, 249);
    	this.Controls.Add(this.label2);
    	this.Controls.Add(this.iOutDir);
    	this.Controls.Add(this.iPages);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bExport);
    	this.Controls.Add(this.label1);
    	this.Name = "PageExport";
    	this.Text = "Page Export";
    	this.ResumeLayout(false);
    	this.PerformLayout();
    }
    private System.Windows.Forms.TextBox iPages;
    private System.Windows.Forms.TextBox iOutDir;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button bExport;
    private System.Windows.Forms.Button bCancel;
    private System.Windows.Forms.Label label1;
        
  }
}
