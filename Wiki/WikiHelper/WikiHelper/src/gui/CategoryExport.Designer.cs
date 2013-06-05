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

  partial class CategoryExport {
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
    	this.groupBox1 = new System.Windows.Forms.GroupBox();
    	this.label3 = new System.Windows.Forms.Label();
    	this.label2 = new System.Windows.Forms.Label();
    	this.iIndexSize = new System.Windows.Forms.MaskedTextBox();
    	this.iIndexPaging = new System.Windows.Forms.MaskedTextBox();
    	this.iIndexName = new System.Windows.Forms.TextBox();
    	this.cbIndex = new System.Windows.Forms.CheckBox();
    	this.groupBox2 = new System.Windows.Forms.GroupBox();
    	this.label6 = new System.Windows.Forms.Label();
    	this.iExtractor = new System.Windows.Forms.ComboBox();
    	this.iSummaryIndex = new System.Windows.Forms.TextBox();
    	this.label4 = new System.Windows.Forms.Label();
    	this.label5 = new System.Windows.Forms.Label();
    	this.iSummaryPaging = new System.Windows.Forms.MaskedTextBox();
    	this.iSummaryName = new System.Windows.Forms.TextBox();
    	this.cbSummary = new System.Windows.Forms.CheckBox();
    	this.groupBox3 = new System.Windows.Forms.GroupBox();
    	this.iPagesDir = new System.Windows.Forms.TextBox();
    	this.cbPages = new System.Windows.Forms.CheckBox();
    	this.groupBox1.SuspendLayout();
    	this.groupBox2.SuspendLayout();
    	this.groupBox3.SuspendLayout();
    	this.SuspendLayout();
    	// 
    	// iCategory
    	// 
    	this.iCategory.FormattingEnabled = true;
    	this.iCategory.Location = new System.Drawing.Point(66, 7);
    	this.iCategory.Name = "iCategory";
    	this.iCategory.Size = new System.Drawing.Size(121, 21);
    	this.iCategory.TabIndex = 1;
    	this.iCategory.SelectedIndexChanged += new System.EventHandler(this.UpdateFileName);
    	this.iCategory.TextUpdate += new System.EventHandler(this.UpdateFileName);
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
    	this.bExport.Location = new System.Drawing.Point(410, 236);
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
    	this.bCancel.Location = new System.Drawing.Point(322, 236);
    	this.bCancel.Name = "bCancel";
    	this.bCancel.Size = new System.Drawing.Size(75, 23);
    	this.bCancel.TabIndex = 10;
    	this.bCancel.Text = "Cancel";
    	this.bCancel.UseVisualStyleBackColor = true;
    	// 
    	// groupBox1
    	// 
    	this.groupBox1.Controls.Add(this.label3);
    	this.groupBox1.Controls.Add(this.label2);
    	this.groupBox1.Controls.Add(this.iIndexSize);
    	this.groupBox1.Controls.Add(this.iIndexPaging);
    	this.groupBox1.Controls.Add(this.iIndexName);
    	this.groupBox1.Controls.Add(this.cbIndex);
    	this.groupBox1.Location = new System.Drawing.Point(8, 34);
    	this.groupBox1.Name = "groupBox1";
    	this.groupBox1.Size = new System.Drawing.Size(477, 52);
    	this.groupBox1.TabIndex = 2;
    	this.groupBox1.TabStop = false;
    	this.groupBox1.Text = "Export Index";
    	// 
    	// label3
    	// 
    	this.label3.Location = new System.Drawing.Point(350, 24);
    	this.label3.Name = "label3";
    	this.label3.Size = new System.Drawing.Size(73, 19);
    	this.label3.TabIndex = 7;
    	this.label3.Text = "summary size";
    	// 
    	// label2
    	// 
    	this.label2.Location = new System.Drawing.Point(211, 24);
    	this.label2.Name = "label2";
    	this.label2.Size = new System.Drawing.Size(82, 19);
    	this.label2.TabIndex = 5;
    	this.label2.Text = "Items per page";
    	// 
    	// iIndexSize
    	// 
    	this.iIndexSize.Location = new System.Drawing.Point(429, 21);
    	this.iIndexSize.Mask = "0000";
    	this.iIndexSize.Name = "iIndexSize";
    	this.iIndexSize.Size = new System.Drawing.Size(36, 20);
    	this.iIndexSize.TabIndex = 8;
    	this.iIndexSize.Text = "250";
    	this.iIndexSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
    	// 
    	// iIndexPaging
    	// 
    	this.iIndexPaging.Location = new System.Drawing.Point(299, 21);
    	this.iIndexPaging.Mask = "99";
    	this.iIndexPaging.Name = "iIndexPaging";
    	this.iIndexPaging.Size = new System.Drawing.Size(36, 20);
    	this.iIndexPaging.TabIndex = 6;
    	this.iIndexPaging.Text = "5";
    	this.iIndexPaging.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
    	// 
    	// iIndexName
    	// 
    	this.iIndexName.Location = new System.Drawing.Point(84, 21);
    	this.iIndexName.Name = "iIndexName";
    	this.iIndexName.Size = new System.Drawing.Size(121, 20);
    	this.iIndexName.TabIndex = 4;
    	// 
    	// cbIndex
    	// 
    	this.cbIndex.Location = new System.Drawing.Point(11, 19);
    	this.cbIndex.Name = "cbIndex";
    	this.cbIndex.Size = new System.Drawing.Size(67, 24);
    	this.cbIndex.TabIndex = 3;
    	this.cbIndex.Text = "export to";
    	this.cbIndex.UseVisualStyleBackColor = true;
    	// 
    	// groupBox2
    	// 
    	this.groupBox2.Controls.Add(this.label6);
    	this.groupBox2.Controls.Add(this.iExtractor);
    	this.groupBox2.Controls.Add(this.iSummaryIndex);
    	this.groupBox2.Controls.Add(this.label4);
    	this.groupBox2.Controls.Add(this.label5);
    	this.groupBox2.Controls.Add(this.iSummaryPaging);
    	this.groupBox2.Controls.Add(this.iSummaryName);
    	this.groupBox2.Controls.Add(this.cbSummary);
    	this.groupBox2.Location = new System.Drawing.Point(8, 92);
    	this.groupBox2.Name = "groupBox2";
    	this.groupBox2.Size = new System.Drawing.Size(477, 80);
    	this.groupBox2.TabIndex = 3;
    	this.groupBox2.TabStop = false;
    	this.groupBox2.Text = "Export Summary";
    	// 
    	// label6
    	// 
    	this.label6.Location = new System.Drawing.Point(212, 22);
    	this.label6.Name = "label6";
    	this.label6.Size = new System.Drawing.Size(58, 23);
    	this.label6.TabIndex = 14;
    	this.label6.Text = "Extractor:";
    	// 
    	// iExtractor
    	// 
    	this.iExtractor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
    	this.iExtractor.FormattingEnabled = true;
    	this.iExtractor.Location = new System.Drawing.Point(270, 16);
    	this.iExtractor.Name = "iExtractor";
    	this.iExtractor.Size = new System.Drawing.Size(195, 21);
    	this.iExtractor.TabIndex = 15;
    	// 
    	// iSummaryIndex
    	// 
    	this.iSummaryIndex.Location = new System.Drawing.Point(80, 48);
    	this.iSummaryIndex.Name = "iSummaryIndex";
    	this.iSummaryIndex.Size = new System.Drawing.Size(190, 20);
    	this.iSummaryIndex.TabIndex = 9;
    	// 
    	// label4
    	// 
    	this.label4.Location = new System.Drawing.Point(276, 51);
    	this.label4.Name = "label4";
    	this.label4.Size = new System.Drawing.Size(100, 19);
    	this.label4.TabIndex = 7;
    	this.label4.Text = "Items/index page";
    	// 
    	// label5
    	// 
    	this.label5.Location = new System.Drawing.Point(11, 51);
    	this.label5.Name = "label5";
    	this.label5.Size = new System.Drawing.Size(63, 19);
    	this.label5.TabIndex = 5;
    	this.label5.Text = "Index Title";
    	// 
    	// iSummaryPaging
    	// 
    	this.iSummaryPaging.Location = new System.Drawing.Point(382, 48);
    	this.iSummaryPaging.Mask = "00";
    	this.iSummaryPaging.Name = "iSummaryPaging";
    	this.iSummaryPaging.Size = new System.Drawing.Size(26, 20);
    	this.iSummaryPaging.TabIndex = 8;
    	this.iSummaryPaging.Text = "24";
    	this.iSummaryPaging.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
    	// 
    	// iSummaryName
    	// 
    	this.iSummaryName.Location = new System.Drawing.Point(84, 19);
    	this.iSummaryName.Name = "iSummaryName";
    	this.iSummaryName.Size = new System.Drawing.Size(121, 20);
    	this.iSummaryName.TabIndex = 4;
    	// 
    	// cbSummary
    	// 
    	this.cbSummary.Location = new System.Drawing.Point(11, 17);
    	this.cbSummary.Name = "cbSummary";
    	this.cbSummary.Size = new System.Drawing.Size(67, 24);
    	this.cbSummary.TabIndex = 3;
    	this.cbSummary.Text = "export to";
    	this.cbSummary.UseVisualStyleBackColor = true;
    	// 
    	// groupBox3
    	// 
    	this.groupBox3.Controls.Add(this.iPagesDir);
    	this.groupBox3.Controls.Add(this.cbPages);
    	this.groupBox3.Location = new System.Drawing.Point(8, 178);
    	this.groupBox3.Name = "groupBox3";
    	this.groupBox3.Size = new System.Drawing.Size(477, 52);
    	this.groupBox3.TabIndex = 4;
    	this.groupBox3.TabStop = false;
    	this.groupBox3.Text = "Export Pages";
    	// 
    	// iPagesDir
    	// 
    	this.iPagesDir.Location = new System.Drawing.Point(84, 21);
    	this.iPagesDir.Name = "iPagesDir";
    	this.iPagesDir.Size = new System.Drawing.Size(121, 20);
    	this.iPagesDir.TabIndex = 4;
    	// 
    	// cbPages
    	// 
    	this.cbPages.Location = new System.Drawing.Point(11, 19);
    	this.cbPages.Name = "cbPages";
    	this.cbPages.Size = new System.Drawing.Size(67, 24);
    	this.cbPages.TabIndex = 3;
    	this.cbPages.Text = "export to";
    	this.cbPages.UseVisualStyleBackColor = true;
    	// 
    	// CategoryExport
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(494, 271);
    	this.Controls.Add(this.groupBox3);
    	this.Controls.Add(this.groupBox2);
    	this.Controls.Add(this.groupBox1);
    	this.Controls.Add(this.bCancel);
    	this.Controls.Add(this.bExport);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.iCategory);
    	this.Name = "CategoryExport";
    	this.Text = "Category Export";
    	this.groupBox1.ResumeLayout(false);
    	this.groupBox1.PerformLayout();
    	this.groupBox2.ResumeLayout(false);
    	this.groupBox2.PerformLayout();
    	this.groupBox3.ResumeLayout(false);
    	this.groupBox3.PerformLayout();
    	this.ResumeLayout(false);
    }
    private System.Windows.Forms.CheckBox cbPages;
    private System.Windows.Forms.TextBox iPagesDir;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.MaskedTextBox iSummaryPaging;
    private System.Windows.Forms.ComboBox iExtractor;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox iSummaryIndex;
    private System.Windows.Forms.CheckBox cbSummary;
    private System.Windows.Forms.TextBox iSummaryName;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.MaskedTextBox iIndexPaging;
    private System.Windows.Forms.MaskedTextBox iIndexSize;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox cbIndex;
    private System.Windows.Forms.TextBox iIndexName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button bExport;
    private System.Windows.Forms.Button bCancel;
    private System.Windows.Forms.ComboBox iCategory;
    private System.Windows.Forms.Label label1;
    
    void BExportClick(object sender, System.EventArgs e) {
      if  (!iCategory.Items.Contains(iCategory.Text)) {
        iCategory.Items.Add(iCategory.Text);
      }
    }
    
    void UpdateFileName(object sender, EventArgs e) {
      string baseName = iCategory.Text.Trim();
      if (baseName.Length>1) {
      iIndexName.Text = baseName+"_idx.ppt";
      iSummaryName.Text = baseName+".ppt";
      iSummaryIndex.Text = baseName.Substring(0,1).ToUpper() + baseName.Substring(1).ToLower();
      iPagesDir.Text = baseName;
      }
    }
    
  }
}
