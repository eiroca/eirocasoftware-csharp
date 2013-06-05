/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
namespace WikiHelper.gui {
  
  partial class PageProcess {
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
    	this.bProcess = new System.Windows.Forms.Button();
    	this.label1 = new System.Windows.Forms.Label();
    	this.iPage = new System.Windows.Forms.TextBox();
    	this.bPreview = new System.Windows.Forms.Button();
    	this.iCategory = new System.Windows.Forms.TextBox();
    	this.bLoadCategory = new System.Windows.Forms.Button();
    	this.SuspendLayout();
    	// 
    	// bCancel
    	// 
    	this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    	this.bCancel.Location = new System.Drawing.Point(207, 272);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 4;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// bProcess
    	// 
    	this.bProcess.DialogResult = System.Windows.Forms.DialogResult.Yes;
    	this.bProcess.Location = new System.Drawing.Point(376, 272);
    	this.bProcess.Name = "bProcess";
    	this.bProcess.Size = new System.Drawing.Size(75, 23);
    	this.bProcess.TabIndex = 6;
    	this.bProcess.Text = "&Process";
    	this.bProcess.UseVisualStyleBackColor = true;
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(12, 46);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(100, 16);
    	this.label1.TabIndex = 2;
    	this.label1.Text = "Page(s)";
    	// 
    	// iPage
    	// 
    	this.iPage.Location = new System.Drawing.Point(12, 75);
    	this.iPage.Multiline = true;
    	this.iPage.Name = "iPage";
    	this.iPage.Size = new System.Drawing.Size(441, 191);
    	this.iPage.TabIndex = 3;
    	// 
    	// bPreview
    	// 
    	this.bPreview.DialogResult = System.Windows.Forms.DialogResult.No;
    	this.bPreview.Location = new System.Drawing.Point(295, 272);
    	this.bPreview.Name = "bPreview";
    	this.bPreview.Size = new System.Drawing.Size(75, 23);
    	this.bPreview.TabIndex = 5;
    	this.bPreview.Text = "Previe&w";
    	this.bPreview.UseVisualStyleBackColor = true;
    	// 
    	// iCategory
    	// 
    	this.iCategory.Location = new System.Drawing.Point(116, 10);
    	this.iCategory.Name = "iCategory";
    	this.iCategory.Size = new System.Drawing.Size(258, 20);
    	this.iCategory.TabIndex = 1;
    	// 
    	// bLoadCategory
    	// 
    	this.bLoadCategory.Location = new System.Drawing.Point(12, 10);
    	this.bLoadCategory.Name = "bLoadCategory";
    	this.bLoadCategory.Size = new System.Drawing.Size(98, 23);
    	this.bLoadCategory.TabIndex = 0;
    	this.bLoadCategory.Text = "Load Category";
    	this.bLoadCategory.UseVisualStyleBackColor = true;
    	this.bLoadCategory.Click += new System.EventHandler(this.BLoadCategoryClick);
    	// 
    	// PageProcess
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(465, 309);
    	this.Controls.Add(this.bLoadCategory);
    	this.Controls.Add(this.iCategory);
    	this.Controls.Add(this.bPreview);
    	this.Controls.Add(this.iPage);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bProcess);
    	this.Name = "PageProcess";
    	this.Text = "Process page(s)";
    	this.ResumeLayout(false);
    	this.PerformLayout();
    }
    private System.Windows.Forms.TextBox iCategory;
    private System.Windows.Forms.Button bLoadCategory;
    private System.Windows.Forms.Button bProcess;
    private System.Windows.Forms.Button bPreview;
    private System.Windows.Forms.TextBox iPage;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button bCancel;
    
    
  }
  
}
