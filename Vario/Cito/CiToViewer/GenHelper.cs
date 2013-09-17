// GenHelper.cs - Generator Helper
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
using Foxoft.Ci;

namespace CiToViewer {

  public class GenInfo {
    public string ID;
    public string Language;
    public string Extension;
    public bool SplitFile;
    public SourceGenerator Generator;

    public GenInfo(string ID, string language, string extension, bool splitFile, SourceGenerator generator) {
      this.ID = ID;
      this.Language = language;
      this.Extension = extension;
      this.SplitFile = splitFile;
      this.Generator = generator;
    }
  }

  public class GenHelper {

    public Dictionary<string, GenInfo> Generators = new Dictionary<string, GenInfo>();

    public GenHelper() {
      string NameSpace = "cito";
      Add(new GenInfo("10", "Object Pascal", "pas", false, new GenPas(NameSpace)));
      Add(new GenInfo("20", "PHP", "php", false, new GenPHP(NameSpace)));
      Add(new GenInfo("30", "Java", "java", true, new GenJava(NameSpace)));
      Add(new GenInfo("40", "C89", "c", false, new GenC89()));
      Add(new GenInfo("41", "C99", "c99", false, new GenC()));
      Add(new GenInfo("50", "D", "d", false, new GenD()));
      Add(new GenInfo("60", "C#", "cs", false, new GenCs(NameSpace)));
      Add(new GenInfo("70", "Perl 5.8", "pm", false, new GenPerl58(NameSpace)));
      Add(new GenInfo("71", "Perl 5.10", "pm510", false, new GenPerl510(NameSpace)));
      Add(new GenInfo("80", "JavaScript", "js", false, new GenJs()));
      Add(new GenInfo("81", "JavaScript (Typed Arrays)", "js-ta", false, new GenJsWithTypedArrays()));
      Add(new GenInfo("90", "Action Script", "as", true, new GenAs(NameSpace)));
    }

    protected void Add(GenInfo generator) {
      Generators.Add(generator.Language, generator);
    }

    public GenInfo GetGenerator(string Language) {
      GenInfo result = null;
      if (Language != null) {
        Generators.TryGetValue(Language, out result);
      }
      return result;
    }

    public string[] GetLanguages() {
      return Generators.Select(x => x.Value).OrderBy(x => x.ID).Select(x => x.Language).ToArray();
    }
  }
}
