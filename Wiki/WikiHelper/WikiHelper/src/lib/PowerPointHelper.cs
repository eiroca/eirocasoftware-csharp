#region Header
/**
 * (C) 2006-2009 eIrOcA (eNrIcO Croce & sImOnA Burzio)
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 */
#endregion Header

namespace WikiHelper {
  using System;

  using Microsoft.Office.Core;

  using PowerPoint;

  public class Presentation {
    
    #region Fields
    public const string SEP = "\r";
    static PowerPoint.Application powerpoint;
    static PowerPoint.Presentations presentationSet;
    bool bAssistantOn;
    PowerPoint.Presentation presentation;
    PowerPoint.Slides slides;
    #endregion Fields

    #region Constructors
    public Presentation(string strTemplate) {
      Create(strTemplate);
    }

    ~Presentation() {
      Close();
    }
    #endregion Constructors

    #region Methods
    public PowerPoint.Slide Add(string title) {
      return Add(PowerPoint.PpSlideLayout.ppLayoutTitleOnly, title);
    }

    public PowerPoint.Slide Add(PowerPoint.PpSlideLayout layout, string title) {
      PowerPoint.Slide slide;
      PowerPoint.TextRange textRange;
      slide = slides.Add(slides.Count+1, layout);
      textRange = slide.Shapes[1].TextFrame.TextRange;
      textRange.Text = title;
      return slide;
    }

    public void Close() {
      if (presentation !=null) {
        presentation.Close();
        slides = null;
        presentation = null;
      }
    }

    public void ClosePowerPoint() {
      if (powerpoint !=null) {
        // Reenable Office Assisant, if it was on:
        if (bAssistantOn) {
          powerpoint.Assistant.On = true;
          powerpoint.Assistant.Visible = false;
        }
        powerpoint.Quit();
        presentationSet = null;
        powerpoint = null;
      }
    }

    public void Create(string strTemplate) {
      if (powerpoint==null) {
        OpenPowerPoint();
      }
      //Create a new presentation based on a template.
      presentation = presentationSet.Open(strTemplate, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoTrue);
      slides = presentation.Slides;
    }

    public void OpenPowerPoint() {
      powerpoint = new PowerPoint.Application();
      powerpoint.Visible = MsoTriState.msoTrue;
      //Prevent Office Assistant from displaying alert messages:
      bAssistantOn = powerpoint.Assistant.On;
      powerpoint.Assistant.On = false;
      presentationSet = powerpoint.Presentations;
    }

    public void Save(string path) {
      presentation.SaveAs(path, PpSaveAsFileType.ppSaveAsPresentation, Microsoft.Office.Core.MsoTriState.msoFalse);
    }
    #endregion Methods
    
  }
  
}