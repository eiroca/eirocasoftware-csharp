/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
namespace WikiHelper.gui {

  partial class MainForm {
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
    	this.meOut = new System.Windows.Forms.RichTextBox();
    	this.menuStrip1 = new System.Windows.Forms.MenuStrip();
    	this.mnFile = new System.Windows.Forms.ToolStripMenuItem();
    	this.configurazioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
    	this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.mnWikimedia = new System.Windows.Forms.ToolStripMenuItem();
    	this.miBuildPages = new System.Windows.Forms.ToolStripMenuItem();
    	this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
    	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
    	this.miExportPage = new System.Windows.Forms.ToolStripMenuItem();
    	this.exportCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.miAddressBook = new System.Windows.Forms.ToolStripMenuItem();
    	this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.processPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    	this.menuStrip1.SuspendLayout();
    	this.SuspendLayout();
    	// 
    	// meOut
    	// 
    	this.meOut.Dock = System.Windows.Forms.DockStyle.Fill;
    	this.meOut.Location = new System.Drawing.Point(0, 24);
    	this.meOut.Name = "meOut";
    	this.meOut.Size = new System.Drawing.Size(747, 360);
    	this.meOut.TabIndex = 2;
    	this.meOut.Text = "";
    	// 
    	// menuStrip1
    	// 
    	this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
    	    	    	this.mnFile,
    	    	    	this.mnWikimedia,
    	    	    	this.experimentalToolStripMenuItem});
    	this.menuStrip1.Location = new System.Drawing.Point(0, 0);
    	this.menuStrip1.Name = "menuStrip1";
    	this.menuStrip1.Size = new System.Drawing.Size(747, 24);
    	this.menuStrip1.TabIndex = 3;
    	this.menuStrip1.Text = "menuStrip1";
    	// 
    	// mnFile
    	// 
    	this.mnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
    	    	    	this.configurazioneToolStripMenuItem,
    	    	    	this.toolStripMenuItem1,
    	    	    	this.closeToolStripMenuItem});
    	this.mnFile.Name = "mnFile";
    	this.mnFile.Size = new System.Drawing.Size(35, 20);
    	this.mnFile.Text = "&File";
    	// 
    	// configurazioneToolStripMenuItem
    	// 
    	this.configurazioneToolStripMenuItem.Name = "configurazioneToolStripMenuItem";
    	this.configurazioneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
    	this.configurazioneToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
    	this.configurazioneToolStripMenuItem.Text = "&Configuration";
    	this.configurazioneToolStripMenuItem.Click += new System.EventHandler(this.ConfigurazioneClick);
    	// 
    	// toolStripMenuItem1
    	// 
    	this.toolStripMenuItem1.Name = "toolStripMenuItem1";
    	this.toolStripMenuItem1.Size = new System.Drawing.Size(182, 6);
    	// 
    	// closeToolStripMenuItem
    	// 
    	this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
    	this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
    	this.closeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
    	this.closeToolStripMenuItem.Text = "&Exit";
    	this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseClick);
    	// 
    	// mnWikimedia
    	// 
    	this.mnWikimedia.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
    	    	    	this.miBuildPages,
    	    	    	this.toolStripMenuItem2,
    	    	    	this.toolStripSeparator1,
    	    	    	this.miExportPage,
    	    	    	this.exportCategoryToolStripMenuItem,
    	    	    	this.miAddressBook});
    	this.mnWikimedia.Name = "mnWikimedia";
    	this.mnWikimedia.Size = new System.Drawing.Size(66, 20);
    	this.mnWikimedia.Text = "Wiki&Media";
    	// 
    	// miBuildPages
    	// 
    	this.miBuildPages.Name = "miBuildPages";
    	this.miBuildPages.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
    	this.miBuildPages.Size = new System.Drawing.Size(241, 22);
    	this.miBuildPages.Text = "Build Pages";
    	this.miBuildPages.Click += new System.EventHandler(this.BuildPagesClick);
    	// 
    	// toolStripMenuItem2
    	// 
    	this.toolStripMenuItem2.Name = "toolStripMenuItem2";
    	this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
    	this.toolStripMenuItem2.Size = new System.Drawing.Size(241, 22);
    	this.toolStripMenuItem2.Text = "Replace";
    	this.toolStripMenuItem2.Click += new System.EventHandler(this.ReplacesClick);
    	// 
    	// toolStripSeparator1
    	// 
    	this.toolStripSeparator1.Name = "toolStripSeparator1";
    	this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
    	// 
    	// miExportPage
    	// 
    	this.miExportPage.Name = "miExportPage";
    	this.miExportPage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
    	this.miExportPage.Size = new System.Drawing.Size(241, 22);
    	this.miExportPage.Text = "Export page(s)";
    	this.miExportPage.Click += new System.EventHandler(this.ExportPageClick);
    	// 
    	// exportCategoryToolStripMenuItem
    	// 
    	this.exportCategoryToolStripMenuItem.Name = "exportCategoryToolStripMenuItem";
    	this.exportCategoryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
    	    	    	| System.Windows.Forms.Keys.P)));
    	this.exportCategoryToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
    	this.exportCategoryToolStripMenuItem.Text = "Export Category...";
    	this.exportCategoryToolStripMenuItem.Click += new System.EventHandler(this.ExportCategoryClick);
    	// 
    	// miAddressBook
    	// 
    	this.miAddressBook.Name = "miAddressBook";
    	this.miAddressBook.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
    	this.miAddressBook.Size = new System.Drawing.Size(241, 22);
    	this.miAddressBook.Text = "Address Book Export";
    	this.miAddressBook.Click += new System.EventHandler(this.ExportAddressBookClick);
    	// 
    	// experimentalToolStripMenuItem
    	// 
    	this.experimentalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
    	    	    	this.processPagesToolStripMenuItem,
    	    	    	this.testToolStripMenuItem});
    	this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
    	this.experimentalToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
    	this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
    	this.experimentalToolStripMenuItem.Text = "Experimental";
    	// 
    	// processPagesToolStripMenuItem
    	// 
    	this.processPagesToolStripMenuItem.Name = "processPagesToolStripMenuItem";
    	this.processPagesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
    	this.processPagesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
    	this.processPagesToolStripMenuItem.Text = "&Process page(s)";
    	this.processPagesToolStripMenuItem.Click += new System.EventHandler(this.ProcessPagesToolStripMenuItemClick);
    	// 
    	// testToolStripMenuItem
    	// 
    	this.testToolStripMenuItem.Name = "testToolStripMenuItem";
    	this.testToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
    	this.testToolStripMenuItem.Text = "Test";
    	this.testToolStripMenuItem.Click += new System.EventHandler(this.TestToolStripMenuItemClick);
    	// 
    	// MainForm
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(747, 384);
    	this.Controls.Add(this.meOut);
    	this.Controls.Add(this.menuStrip1);
    	this.MainMenuStrip = this.menuStrip1;
    	this.Name = "MainForm";
    	this.Text = "WikiHelper";
    	this.menuStrip1.ResumeLayout(false);
    	this.menuStrip1.PerformLayout();
    	this.ResumeLayout(false);
    	this.PerformLayout();
    }
    private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem processPagesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem experimentalToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem miBuildPages;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem mnFile;
    private System.Windows.Forms.ToolStripMenuItem mnWikimedia;
    private System.Windows.Forms.ToolStripMenuItem configurazioneToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem miExportPage;
    private System.Windows.Forms.ToolStripMenuItem miAddressBook;
    private System.Windows.Forms.ToolStripMenuItem exportCategoryToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.RichTextBox meOut;
       
  }
  
}
