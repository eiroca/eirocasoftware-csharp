/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
namespace WikiHelper.gui {
  
  partial class WikiLoginForm {
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
    	this.iUsername = new System.Windows.Forms.TextBox();
    	this.label1 = new System.Windows.Forms.Label();
    	this.label2 = new System.Windows.Forms.Label();
    	this.iPassword = new System.Windows.Forms.TextBox();
    	this.bnLogin = new System.Windows.Forms.Button();
    	this.SuspendLayout();
    	// 
    	// iUsername
    	// 
    	this.iUsername.Location = new System.Drawing.Point(84, 9);
    	this.iUsername.Name = "iUsername";
    	this.iUsername.Size = new System.Drawing.Size(100, 20);
    	this.iUsername.TabIndex = 0;
    	// 
    	// label1
    	// 
    	this.label1.Location = new System.Drawing.Point(12, 9);
    	this.label1.Name = "label1";
    	this.label1.Size = new System.Drawing.Size(66, 23);
    	this.label1.TabIndex = 1;
    	this.label1.Text = "Username";
    	// 
    	// label2
    	// 
    	this.label2.Location = new System.Drawing.Point(12, 35);
    	this.label2.Name = "label2";
    	this.label2.Size = new System.Drawing.Size(66, 23);
    	this.label2.TabIndex = 3;
    	this.label2.Text = "Password";
    	// 
    	// iPassword
    	// 
    	this.iPassword.Location = new System.Drawing.Point(84, 35);
    	this.iPassword.Name = "iPassword";
    	this.iPassword.PasswordChar = '*';
    	this.iPassword.Size = new System.Drawing.Size(100, 20);
    	this.iPassword.TabIndex = 1;
    	// 
    	// bnLogin
    	// 
    	this.bnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
    	this.bnLogin.Location = new System.Drawing.Point(109, 61);
    	this.bnLogin.Name = "bnLogin";
    	this.bnLogin.Size = new System.Drawing.Size(75, 23);
    	this.bnLogin.TabIndex = 2;
    	this.bnLogin.Text = "&Login";
    	this.bnLogin.UseVisualStyleBackColor = true;
    	this.bnLogin.Click += new System.EventHandler(this.BnLoginClick);
    	// 
    	// WikiLoginForm
    	// 
    	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    	this.ClientSize = new System.Drawing.Size(193, 91);
    	this.Controls.Add(this.bnLogin);
    	this.Controls.Add(this.label2);
    	this.Controls.Add(this.iPassword);
    	this.Controls.Add(this.label1);
    	this.Controls.Add(this.iUsername);
    	this.Name = "WikiLoginForm";
    	this.Text = "Wiki Login";
    	this.ResumeLayout(false);
    	this.PerformLayout();
    }
    
    private System.Windows.Forms.Button bnLogin;
    private System.Windows.Forms.TextBox iPassword;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox iUsername;
    
    void BnLoginClick(object sender, System.EventArgs e) {
      wiki.username = iUsername.Text;
      wiki.password = iPassword.Text;
    }
    
  }
  
}
