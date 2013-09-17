// MainWindow.cs - CiToViewer main window
//
// Copyright (C) 2013  Enrico Croce
//
// This file is part of CiTo, see http://cito.sourceforge.net
//
// CiTo is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CiTo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CiTo.  If not, see http://www.gnu.org/licenses/
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using CiToViewer;

public partial class MainWindow: Gtk.Window {	

  protected ProjectFiles Project = new ProjectFiles();

  public MainWindow(): base (Gtk.WindowType.Toplevel) {
    Build();
    tvSource.Buffer.Changed += OnSourceChange;
    lbMsg.Text = "";
    TranslateCode();
    PopulateCombo(cbSource, Project.GetSources());
    PopulateCombo(cbLanguage, Project.Generator.GetLanguages());
  }

  private void PopulateCombo(ComboBox cb, string[] items) {
    cb.Clear();
    ListStore store = new ListStore(typeof(string));
    foreach (var item in items) {
      store.AppendValues(item);
    }
    cb.Model = store;
    var cellRenderer = new CellRendererText();
    cb.PackStart(cellRenderer, true);
    cb.AddAttribute(cellRenderer, "text", 0);
    cb.Active = 0;
  }

  private void TranslateCode() {
    lbMsg.Text = "";
    tvTarget.Buffer.Text = "";
    try {
      Project.GenerateTarget(cbLanguage.ActiveText, iNameSpace.Text);
    }
    catch (Exception e) {
      lbMsg.Text = e.Message;
    }
    PopulateCombo(cbTarget, Project.GetTargets());
  }

  private void LoadSourceFiles(string[] Filenames) {
    Project.LoadFiles(Filenames);
    PopulateCombo(cbSource, Project.GetSources());
    TranslateCode();
  }

  protected void OnDeleteEvent(object sender, DeleteEventArgs a) {
    Application.Quit();
    a.RetVal = true;
  }

  protected void OnExit(object sender, EventArgs e) {
    Application.Quit();
  }

  protected void OnOpen(object sender, EventArgs e) {
    tvSource.Buffer.Text = "";
    FileChooserDialog chooser = new FileChooserDialog("Please select a Ci file to view ...", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
    chooser.SelectMultiple = true;
    if (chooser.Run() == (int)ResponseType.Accept) {
      lbMsg.Text = "File(s) loaded.";
      LoadSourceFiles(chooser.Filenames);
    } 
    chooser.Destroy();
  }

  protected void OnFontItem(object sender, EventArgs e) {
    lbMsg.Text = "";
    FontSelectionDialog fdia = new FontSelectionDialog("Select font name");
    fdia.Response += delegate (object o, ResponseArgs resp) {
      if (resp.ResponseId == ResponseType.Ok) {
        Pango.FontDescription fontdesc = Pango.FontDescription.FromString(fdia.FontName);
        tvSource.ModifyFont(fontdesc);
        tvTarget.ModifyFont(fontdesc);
      }
    };
    fdia.Run();
    fdia.Destroy();
  }

  protected ProjectFile GetFileInfo(string key, Dictionary<string, ProjectFile> db) {
    ProjectFile file = null;
    if (key != null) {
      db.TryGetValue(key, out file);
    }
    return file;
  }

  protected void cbSourceChanged(object sender, EventArgs e) {
    ProjectFile file = GetFileInfo(cbSource.ActiveText, Project.Source);
    if (file != null) {
      tvSource.Buffer.Text = file.Code;
    }
  }

  protected void cbTargetChanged(object sender, EventArgs e) {
    ProjectFile file = GetFileInfo(cbTarget.ActiveText, Project.Target);
    if (file != null) {
      tvTarget.Buffer.Text = file.Code;
    }
  }

  protected void OnSourceChange(object sender, EventArgs e) {
    if (AutoTranslateAction.Active) {
      CopySource();
      TranslateCode();
    }
  }

  protected void CopySource() {
    if (tvSource.Buffer.Modified) {
      ProjectFile file = GetFileInfo(cbSource.ActiveText, Project.Source);
      if (file != null) {
        tvSource.Buffer.Modified = false;
        file.Code = tvSource.Buffer.Text;
        file.Changed = true;
      }
    }
  }

  protected void OnTranslate(object sender, EventArgs e) {
    CopySource();
    TranslateCode();
  }

  protected void SaveCode(Dictionary<string, ProjectFile> db) {
    foreach (CiToViewer.ProjectFile file in db.Values) {
      if (file.Changed) {
        System.IO.File.WriteAllText(file.Path, file.Code);
        file.Changed = false;
      }
    }
  }

  protected void OnSaveSource(object sender, EventArgs e) {
    try {
      SaveCode(Project.Source);
      lbMsg.Text = "Source saved";
    }
    catch (Exception ex) {
      lbMsg.Text = ex.Message;
    }
  }

  protected void OnSaveTarget(object sender, EventArgs e) {
    try {
      SaveCode(Project.Target);
      lbMsg.Text = "Target saved";
    }
    catch (Exception ex) {
      lbMsg.Text = ex.Message;
    }
  }

  protected void OnLanguageChange(object sender, EventArgs e) {
    TranslateCode();
  }
}
