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
using System.Collections;
using System.Collections.Generic;

namespace Foxoft.Ci {

  public class GenPas : SourceGenerator, ICiSymbolVisitor {
    //
    public string BlockOpenStr = "{";
    public string BlockCloseStr = "}";
    public string IdentStr = "  ";
    public string NewLineStr = "\r\n";
    //
    string Namespace;
    //
    PascalPreProcessing prePro = new PascalPreProcessing();

    public GenPas(string aNamespace) {
      this.Namespace = (aNamespace == null ? "cito" : aNamespace);
      BlockCloseStr = "end";
      BlockOpenStr = "begin";
    }

    protected override void Write(string s) {
      newText.Append(s);
    }

    protected virtual void WriteFormat(string format, params object[] args) {
      newText.AppendFormat(format, args);
    }

    protected override void Write(char c) {
      newText.Append(c);
    }

    protected override void Write(int i) {
      newText.Append(i);
    }

    protected override void WriteLowercase(string s) {
      foreach (char c in s) {
        newText.Append(char.ToLowerInvariant(c));
      }
    }

    protected override void WriteCamelCase(string s) {
      newText.Append(char.ToLowerInvariant(s[0]));
      newText.Append(s.Substring(1));
    }

    protected override void WriteUppercaseWithUnderscores(string s) {
      bool first = true;
      foreach (char c in s) {
        if (char.IsUpper(c) && !first) {
          newText.Append('_');
          newText.Append(c);
        }
        else {
          newText.Append(char.ToUpperInvariant(c));
        }
        first = false;
      }
    }

    protected override void WriteLowercaseWithUnderscores(string s) {
      bool first = true;
      foreach (char c in s) {
        if (char.IsUpper(c)) {
          if (!first) {
            newText.Append('_');
          }
          newText.Append(char.ToLowerInvariant(c));
        }
        else {
          newText.Append(c);
        }
        first = false;
      }
    }

    protected override void WriteLine(string s) {
      Write(s);
      WriteLine();
    }

    protected override void WriteLine(string format, params object[] args) {
      WriteFormat(format, args);
      WriteLine();
    }

    protected override void WriteLine() {
      string newTxt = newText.ToString().Trim();
      if (newText.Equals("")) {
        oldText.Append(NewLineStr);
        return;
      }
      string oldTxt = oldText.ToString();
      if (newTxt.StartsWith("else")) {
        for (int i=oldTxt.Length-1; i>=0; i--) {
          if ("\n\r\t ".IndexOf(oldTxt[i]) < 0) {
            if (oldTxt[i] == ';') {
              oldTxt = oldTxt.Remove(i, 1);
              break;
            }
          }
        }
      }
      else if ((this.Indent == 0) && (newTxt.StartsWith("end;"))) {
        //(pork)workaround
        for (int i=oldTxt.Length-1; i>=0; i--) {
          if ("\n\r\t ".IndexOf(oldTxt[i]) < 0) {
            if (i >= 5) {
              if (oldTxt.Substring(i - 4, 5).Equals("exit;")) {
                oldTxt = oldTxt.Remove(i - 5, 6).TrimEnd(new char[] { '\r', '\t', '\n', ' ' });
                break;
              }
            }
          }
        }
      }
      fullText.Append(oldTxt);
      oldText = new StringBuilder();
      for (int i = 0; i < this.Indent; i++) {
        oldText.Append(IdentStr);
      }
      oldText.Append(newTxt);
      oldText.Append(NewLineStr);
      newText = new StringBuilder();
    }

    public StringBuilder newText = null;
    public StringBuilder oldText = null;
    public StringBuilder fullText = null;

    new protected virtual void CreateFile(string filename) {
      newText = new StringBuilder();
      oldText = new StringBuilder();
      fullText = new StringBuilder();
      base.CreateFile(filename);
    }

    new protected virtual void CloseFile() {
      if (oldText.Length > 0) {
        fullText.Append(oldText);
      }
      if (newText.Length > 0) {
        fullText.Append(newText);
      }
      this.Writer.Write(fullText);
      base.CloseFile();
    }

    protected override void OpenBlock() {
      OpenBlock(true);
    }

    protected virtual void OpenBlock(bool explict) {
      if (explict) {
        WriteLine(BlockOpenStr);
      }
      this.Indent++;
    }

    protected override void CloseBlock() {
      CloseBlock(true);
    }

    protected virtual void CloseBlock(bool explict) {
      this.Indent--;
      if (explict) {
        Write(BlockCloseStr);
      }
    }

    protected override void Write(CiBinaryResourceExpr expr) {
      WriteName(expr.Resource);
    }

    protected override void WriteName(CiBinaryResource resource) {
      SymbolMapping symbol = SymbolMapping.Find(resource);
      Write(symbol != null ? symbol.NewName : resource.Name);
    }

    protected override void WriteName(CiConst konst) {
      SymbolMapping symbol = SymbolMapping.Find(konst);
      Write(symbol != null ? symbol.NewName : konst.Name);
    }

    protected virtual void WriteName(CiSymbol var) {
      SymbolMapping symbol = SymbolMapping.Find(var);
      Write(symbol != null ? symbol.NewName : var.Name);
    }

    public void WriteInitialization(CiProgram prog) {
      WriteLine();
      WriteLine("initialization");
      OpenBlock(false);
      HashSet<string> types = new HashSet<string>();
      foreach (CiType t in SuperType.GetTypeList()) {
        TypeMappingInfo info = SuperType.GetTypeInfo(t);
        if (info.NullInit != null) {
          if (!types.Contains(info.Name)) {
            types.Add(info.Name);
            Write(info.NullInit);
            WriteLine(";");
          }
        }
      }
      foreach (CiSymbol symbol in prog.Globals) {
        if (symbol is CiClass) {
          CiClass klass = (CiClass)symbol;
          foreach (CiConst konst in klass.ConstArrays) {
            WriteConstFull(konst);
          }
        }
      }
      foreach (CiSymbol symbol in prog.Globals) {
        if (symbol is CiClass) {
          CiClass klass = (CiClass)symbol;
          foreach (CiBinaryResource resource in klass.BinaryResources) {
            WriteName(resource);
            Write(":= __getBinaryResource('");
            Write(resource.Name);
            WriteLine("');");
          }
        }
      }
      CloseBlock(false);
    }

    public void WriteConstants(CiProgram prog) {
      bool first = true;
      foreach (CiSymbol symbol in prog.Globals) {
        if (symbol is CiClass) {
          CiClass klass = (CiClass)symbol;
          foreach (CiConst konst in klass.ConstArrays) {
            if (first) {
              WriteLine();
              WriteLine("var");
              OpenBlock(false);
              first = false;
            }
            WriteName(konst);
            Write(": ");
            WriteType(konst.Type);
            WriteLine(";");
          }
        }
      }
      if (!first) {
        CloseBlock(false);
      }
      first = true;
      foreach (CiSymbol symbol in prog.Globals) {
        if (symbol is CiClass) {
          CiClass klass = (CiClass)symbol;
          foreach (CiBinaryResource resource in klass.BinaryResources) {
            if (first) {
              WriteLine();
              WriteLine("var");
              OpenBlock(false);
              first = false;
            }
            WriteName(resource);
            WriteLine(": ArrayOf_byte;");
          }
        }
      }
      if (!first) {
        CloseBlock(false);
      }
    }

    public void WriteEnums(CiProgram prog) {
      foreach (CiSymbol symbol in prog.Globals) {
        if (symbol is CiEnum) {
          symbol.Accept(this);
        }
      }
    }

    public void WriteClassInterface(CiClass klass) {
      WriteLine();
      Write(klass.Documentation);
      WriteName(klass);
      Write(" = ");
      Write("class(");
      if (klass.BaseClass != null) {
        WriteName(klass.BaseClass);
      }
      else {
        Write("TInterfacedObject");
      }
      WriteLine(")");
      OpenBlock(false);
      foreach (CiSymbol member in klass.Members) {
        if (!(member is CiMethod)) {
          member.Accept(this);
        }
      }
      WriteLine("public constructor Create;");
      foreach (CiSymbol member in klass.Members) {
        if ((member is CiMethod)) {
          WriteMethodIntf((CiMethod)member);
        }
      }
      CloseBlock(false);
      WriteLine("end;");
    }

    public void WriteClassImplementation(CiClass klass) {
      WriteLine();
      WriteMethodCreateImpl(klass);
      foreach (CiSymbol member in klass.Members) {
        if (member is CiMethod) {
          CiMethod method = (CiMethod)member;
          if (method.CallType != CiCallType.Abstract) {
            WriteMethodImpl(method);
          }
        }
      }
    }

    public void WriteClassesInterface(CiProgram prog) {
      var delegates = prog.Globals.Where(s => s is CiDelegate);
      if (delegates.Count() > 0) {
        WriteLine();
        foreach (CiDelegate del in delegates) {
          Write(del.Documentation);
          WriteSignature(null, del, true);
          WriteLine(";");
        }
      }
      foreach (CiClass klass in ClassOrder.GetList()) {
        WriteClassInterface(klass);
      }
    }

    public void WriteClassesImplementation() {
      foreach (CiClass klass in ClassOrder.GetList()) {
        WriteClassImplementation(klass);
      }
    }

    public void WriteInterfaceHeader() {
      WriteLine();
      WriteLine("interface");
      WriteLine();
      WriteLine("uses SysUtils, StrUtils, Classes, Math;");
    }

    public void WriteSuperTypes() {
      bool sep = false;
      foreach (CiClass klass in SuperType.GetClassList()) {
        if (!sep) {
          WriteLine();
          sep = true;
        }
        WriteName(klass);
        WriteLine(" = class;");
      }
      HashSet<string> types = new HashSet<string>();
      foreach (CiType t in SuperType.GetTypeList()) {
        TypeMappingInfo info = SuperType.GetTypeInfo(t);
        if (!info.Native) {
          if (sep) {
            sep = false;
            WriteLine();
          }
          if (!types.Contains(info.Name)) {
            types.Add(info.Name);
            Write(info.Name);
            Write(" = ");
            Write(info.Definition);
            WriteLine(";");
          }
        }
      }
    }

    public void WriteImplementationHeader() {
      WriteLine();
      WriteLine("implementation");
      WriteLine();
      bool getResProc = false;
      bool first = true;
      HashSet<string> types = new HashSet<string>();
      foreach (CiType t in SuperType.GetTypeList()) {
        TypeMappingInfo info = SuperType.GetTypeInfo(t);
        if (!info.Native) {
          if (first) {
            first = false;
          }
          if (!types.Contains(info.Name)) {
            types.Add(info.Name);
            Write("var ");
            Write(info.Null);
            Write(": ");
            Write(info.Name);
            WriteLine(";");
            WriteLine("procedure __CCLEAR(var x: " + info.Name + "); overload; var i: integer; begin for i:= low(x) to high(x) do x[i]:= " + info.ItemDefault + "; end;");
            WriteLine("procedure __CFILL (var x: " + info.Name + "; v: " + info.ItemType + "); overload; var i: integer; begin for i:= low(x) to high(x) do x[i]:= v; end;");
            WriteLine("procedure __CCOPY (const source: " + info.Name + "; sourceStart: integer; var dest: " + info.Name + "; destStart: integer; len: integer); overload; var i: integer; begin for i:= 0 to len do dest[i+destStart]:= source[i+sourceStart]; end;");
            if ((info.ItemType != null) && (info.ItemType.Equals("byte"))) {
              getResProc = true;
            }
          }
        }
      }
      if (getResProc) {
        WriteLine("function  __getBinaryResource(const aName: string): ArrayOf_byte; var myfile: TFileStream; begin myFile := TFileStream.Create(aName, fmOpenRead); SetLength(Result, myFile.Size); try myFile.seek(0, soFromBeginning); myFile.ReadBuffer(Result, myFile.Size); finally myFile.free; end; end;");
        WriteLine("function  __TOSTR (const x: ArrayOf_byte): string; var i: integer; begin Result:= ''; for i:= low(x) to high(x) do Result:= Result + chr(x[i]); end;");
      }
      WriteLine("function  __CDEC_Pre (var x: integer): integer; overload; inline; begin dec(x); Result:= x; end;");
      WriteLine("function  __CDEC_Post(var x: integer): integer; overload; inline; begin Result:= x; dec(x); end;");
      WriteLine("function  __CINC_Pre (var x: integer): integer; overload; inline; begin inc(x); Result:= x; end;");
      WriteLine("function  __CINC_Post(var x: integer): integer; overload; inline; begin Result:= x; inc(x); end;");
      WriteLine("function  __CDEC_Pre (var x: byte): byte; overload; inline; begin dec(x); Result:= x; end;");
      WriteLine("function  __CDEC_Post(var x: byte): byte; overload; inline; begin Result:= x; dec(x); end;");
      WriteLine("function  __CINC_Pre (var x: byte): byte; overload; inline; begin inc(x); Result:= x; end;");
      WriteLine("function  __CINC_Post(var x: byte): byte; overload; inline; begin Result:= x; inc(x); end;");
      WriteLine("function  __getMagic(const cond: array of boolean): integer; var i: integer; var o: integer; begin Result:= 0; for i:= low(cond) to high(cond) do begin if (cond[i]) then o:= 1 else o:= 0; Result:= Result shl 1 + o; end; end;");
    }

    public override void Write(CiProgram prog) {
      CreateFile(this.OutputFile);
      prePro.Parse(prog);
      // Prologue
      WriteLine("unit " + this.Namespace + ";");
      // Declaration
      WriteInterfaceHeader();
      if (!SymbolMapping.IsEmpty()) {
        WriteLine();
        WriteLine("type");
        OpenBlock(false);
        WriteEnums(prog);
        WriteSuperTypes();
        WriteClassesInterface(prog);
        CloseBlock(false);
      }
      WriteConstants(prog);
      // Implementation
      WriteImplementationHeader();
      WriteClassesImplementation();
      //Epilogue
      WriteInitialization(prog);
      WriteLine("end.");
      CloseFile();
    }

    void ICiSymbolVisitor.Visit(CiClass klass) {
      throw new InvalidOperationException("Try to visit a Class");
    }

    void WriteMethodIntf(CiMethod method) {
      Write(method.Documentation);
      foreach (CiParam param in method.Signature.Params) {
        if (param.Documentation != null) {
          Write("{ @param '");
          WriteName(param);
          Write("' ");
          Write(param.Documentation.Summary);
          WriteLine("}");
        }
      }
      Write(method.Visibility);
      Write(" ");
      if (method.CallType == CiCallType.Static) {
        Write("class ");
      }
      WriteSignature(null, method.Signature, false);
      switch (method.CallType) {
        case CiCallType.Abstract:
          Write("; virtual; abstract");
          break;
        case CiCallType.Virtual:
          Write("; virtual");
          break;
        case CiCallType.Override:
          Write("; override");
          break;
      }
      WriteLine(";");
    }

    void WriteMethodCreateImpl(CiClass klass) {
      Write("constructor ");
      WriteName(klass);
      WriteLine(".Create;");
      if (klass.Constructor != null) {
        WriteLabels(klass.Constructor);
        WriteVars(klass.Constructor);
      }
      OpenBlock();
      foreach (CiSymbol member in klass.Members) {
        if (member is CiConst) {
          var konst = (CiConst)member;
          if (konst.Type is CiArrayType) {
            WriteConstFull(konst);
          }
        }
      }
      WriteVarsInit(klass);
      if (klass.Constructor != null) {
        WriteVarsInit(klass.Constructor);
        if (klass.Constructor.Body is CiBlock) {
          WriteCode(((CiBlock)klass.Constructor.Body).Statements);
        }
        else {
          Write(klass.Constructor.Body);
        }
      }
      CloseBlock();
      WriteLine(";");
    }

    void WriteMethodImpl(CiMethod method) {
      MethodStack.Push(method);
      WriteLine();
      if (method.CallType == CiCallType.Static) {
        Write("class ");
      }
      WriteSignature(method.Class.Name, method.Signature, false);
      WriteLine(";");
      // Emit Variabiles
      WriteLabels(method);
      WriteVars(method);
      // Emit Code
      OpenBlock();
      WriteVarsInit(method);
      if (method.Body is CiBlock) {
        WriteCode(((CiBlock)method.Body).Statements);
      }
      else {
        Write(method.Body);
      }
      CloseBlock();
      WriteLine(";");
      MethodStack.Pop();
    }

    void WriteLabels(CiMethod method) {
      List<BreakExit> labels = BreakExit.GetLabels(method);
      if (labels != null) {
        foreach (BreakExit label in labels) {
          WriteLine("label " + label.Name + ";");
        }
      }
    }

    void ICiSymbolVisitor.Visit(CiMethod method) {
      throw new InvalidOperationException("Try to visit a method");
    }

    void WriteSignature(string prefix, CiDelegate del, bool typeDeclare) {
      if (typeDeclare) {
        WriteName(del);
        Write(" = ");
      }
      if (del.ReturnType == CiType.Void) {
        Write("procedure ");
      }
      else {
        Write("function ");
      }
      if (!typeDeclare) {
        if (prefix != null) {
          Write(prefix + ".");
        }
        WriteName(del);
      }
      Write('(');
      bool first = true;
      foreach (CiParam param in del.Params) {
        if (first) {
          first = false;
        }
        else {
          Write("; ");
        }
        if (param.Type is CiArrayType) {
          Write("var ");
        }
        else if (param.Type is CiStringType) {
          Write(""); // TODO should be var but constant propagration must be handled
        }
        WriteName(param);
        Write(": ");
        Write(param.Type);
      }
      Write(')');
      if (del.ReturnType != CiType.Void) {
        Write(": ");
        Write(del.ReturnType);
      }
      if (typeDeclare) {
        if (prefix != null) {
          Write(" of object");
        }
      }
    }

    public int GetSize(CiType type) {
      if (type is CiArrayStorageType) {
        CiArrayStorageType arr = (CiArrayStorageType)type;
        if (arr.LengthExpr == null) {
          return ((CiArrayStorageType)type).Length;
        }
      }
      return -1;
    }

    void Write(CiType type) {
      WriteType(type);
    }

    void WriteType(CiType type) {
      Write(SuperType.GetTypeInfo(type).Name);
    }

    protected virtual void WriteVars(CiSymbol symb) {
      SymbolMapping vars = SymbolMapping.Find(symb);
      if (vars != null) {
        foreach (SymbolMapping var in vars.childs) {
          if (var.Symbol == null) {
            continue;
          }
          if (var.Symbol is CiVar) {
            WriteVar((CiVar)var.Symbol, var.NewName, true);
          }
          if (var.Symbol is CiField) {
            WriteField((CiField)var.Symbol, var.NewName, true);
          }
        }
      }
    }

    protected virtual void WriteVarsInit(CiSymbol symb) {
      SymbolMapping vars = SymbolMapping.Find(symb);
      if (vars != null) {
        foreach (SymbolMapping var in vars.childs) {
          if (var.Symbol == null) {
            continue;
          }
          if (var.Symbol is CiVar) {
            CiVar v = (CiVar)var.Symbol;
            WriteInitNew(v, v.Type);
          }
          if (var.Symbol is CiField) {
            CiField f = (CiField)var.Symbol;
            WriteInitNew(f, f.Type);
            WriteInitVal(f);
          }
        }
      }
    }

    protected virtual void WriteCode(ICiStatement[] block) {
      foreach (ICiStatement stmt in block) {
        Write(stmt);
      }
    }

    public override void Visit(CiBlock block) {
      OpenBlock();
      WriteCode(block.Statements);
      CloseBlock();
    }

    public override void Visit(CiVar var) {
      WriteInitVal(var);
    }

    protected override void WriteChild(ICiStatement stmt) {
      if (stmt is CiBlock) {
        Write((CiBlock)stmt);
      }
      else {
        if ((stmt is CiReturn) && (((CiReturn)stmt).Value != null)) {
          OpenBlock();
          Write(stmt);
          CloseBlock();
        }
        else {
          Write(stmt);
        }
      }
    }

    public override void Visit(CiWhile stmt) {
      BreakExit.Push(stmt);
      Write("while (");
      Write(stmt.Cond);
      Write(") do ");
      WriteChild(stmt.Body);
      BreakExit.Pop();
    }

    public bool IsAssignment(ICiStatement stmt, CiVar v4r) {
      CiVar v = null;
      if (stmt is CiPostfixExpr) {
        if (((CiPostfixExpr)stmt).Inner is CiVarAccess) {
          v = (CiVar)((CiVarAccess)(((CiPostfixExpr)stmt).Inner)).Var;
        }
      }
      if (stmt is CiVarAccess) {
        v = (CiVar)((CiVarAccess)stmt).Var;
      }
      if (stmt is CiAssign) {
        if (((CiAssign)stmt).Target is CiVarAccess) {
          v = (CiVar)((CiVarAccess)(((CiAssign)stmt).Target)).Var;
        }
      }
      return (v != null) ? (v == v4r) : false;
    }
    // Detect Pascal For loop
    public bool ValidPascalFor(CiFor stmt) {
      if (!(stmt.Init is CiVar))
        return false;
      // Single variable
      var loopVar = (CiVar)stmt.Init;
      if (loopVar.InitialValue is CiCondExpr)
        return false;
      // Step must be variable (de|in)cremented
      if ((stmt.Advance is CiPostfixExpr) && (stmt.Cond is CiBoolBinaryExpr)) {
        if (!IsAssignment(stmt.Advance, loopVar))
          return false;
        CiBoolBinaryExpr cond = (CiBoolBinaryExpr)stmt.Cond;
        // bounded by const or var
        if ((cond.Left is CiVarAccess) && ((cond.Right is CiConstExpr) || (cond.Right is CiVarAccess))) {
          if (((CiVarAccess)cond.Left).Var == loopVar) {
            // loop varibale cannot be changed inside the loop
            if (PascalPreProcessing.Execute(stmt.Body, s => IsAssignment(s, loopVar)))
              return false;
            return true;
          }
        }
      }
      return false;
    }

    public override void Visit(CiFor stmt) {
      BreakExit.Push(stmt);
      bool hasInit = (stmt.Init != null);
      bool hasNext = (stmt.Advance != null);
      bool hasCond = (stmt.Cond != null);
      if (hasInit && hasNext && hasCond) {
        if (ValidPascalFor(stmt)) {
          CiBoolBinaryExpr cond = (CiBoolBinaryExpr)stmt.Cond;
          CiPostfixExpr mode = (CiPostfixExpr)stmt.Advance;
          String dir = null;
          int lmt = 0;
          if (mode.Op == CiToken.Increment) {
            dir = " to ";
            if (cond.Op == CiToken.LessOrEqual) {
              lmt = 0;
            }
            else if (cond.Op == CiToken.Less) {
              lmt = -1;
            }
          }
          if (mode.Op == CiToken.Decrement) {
            dir = " downto ";
            if (cond.Op == CiToken.GreaterOrEqual) {
              lmt = 0;
            }
            else if (cond.Op == CiToken.Greater) {
              lmt = +1;
            }
          }
          if (dir != null) {
            CiVar var = (CiVar)stmt.Init;
            Write("for ");
            WriteInitVal(var);
            Write(dir);
            if ((cond.Right is CiConstExpr) && (((CiConstExpr)cond.Right).Value is Int32)) {
              Write((Int32)((CiConstExpr)cond.Right).Value + lmt);
            }
            else {
              Write(cond.Right);
              if (lmt != 0) {
                if (lmt > 0) {
                  Write("+" + lmt);
                }
                else {
                  Write(lmt);
                }
              }
            }
            Write(" do ");
            WriteChild(stmt.Body);
            return;
          }
        }
      }
      if (hasInit) {
        stmt.Init.Accept(this);
        WriteLine(";");
      }
      Write("while (");
      if (hasCond) {
        Write(stmt.Cond);
      }
      else {
        Write("true");
      }
      Write(") do ");
      if (hasNext) {
        OpenBlock();
        if (stmt.Body is CiBlock) {
          WriteCode(((CiBlock)stmt.Body).Statements);
        }
        else {
          Write(stmt.Body);
        }
        stmt.Advance.Accept(this);
        WriteLine(";");
        CloseBlock();
      }
      else {
        WriteChild(stmt.Body);
      }
      BreakExit.Pop();
    }

    protected override void Write(CiBinaryExpr expr) {
      Write("(");
      base.Write(expr);
      Write(")");
    }

    public override void Visit(CiIf stmt) {
      Write("if ");
      NoIIFExpand.Push(1);
      Write(stmt.Cond);
      NoIIFExpand.Pop();
      Write(" then ");
      WriteChild(stmt.OnTrue);
      if (stmt.OnFalse != null) {
        Write("else ");
        if (stmt.OnFalse is CiIf) {
          Write(" ");
          Write(stmt.OnFalse);
        }
        else {
          WriteChild(stmt.OnFalse);
        }
      }
    }

    protected override void Write(CiUnaryExpr expr) {
      switch (expr.Op) {
        case CiToken.Increment:
          Write("__CINC_Pre(");
          break;
        case CiToken.Decrement:
          Write("__CDEC_Pre(");
          break;
        case CiToken.Minus:
          Write("-(");
          break;
        case CiToken.Not:
          Write("not (");
          break;
        default:
          throw new ArgumentException(expr.Op.ToString());
					
      }
      WriteChild(expr, expr.Inner);
      Write(")");
    }

    protected override void Write(CiCondNotExpr expr) {
      Write("not (");
      WriteChild(expr, expr.Inner);
      Write(")");
    }

    protected override void Write(CiPostfixExpr expr) {
      switch (expr.Op) {
        case CiToken.Increment:
          Write("__CINC_Post(");
          break;
        case CiToken.Decrement:
          Write("__CDEC_Post(");
          break;
        default:
          throw new ArgumentException(expr.Op.ToString());
      }
      WriteChild(expr, expr.Inner);
      Write(")");
    }

    void ICiSymbolVisitor.Visit(CiEnum enu) {
      WriteLine();
      Write(enu.Documentation);
      WriteName(enu);
      WriteLine(" = (");
      OpenBlock(false);
      bool first = true;
      foreach (CiEnumValue value in enu.Values) {
        if (first) {
          first = false;
        }
        else {
          WriteLine(",");
        }
        Write(value.Documentation);
        WriteName(value);
      }
      WriteLine();
      CloseBlock(false);
      WriteLine(");");
    }

    void Write(CiVisibility visibility) {
      switch (visibility) {
        case CiVisibility.Dead:
          Write("private");
          break;
        case CiVisibility.Private:
          Write("private");
          break;
        case CiVisibility.Internal:
          Write("private");
          break;
        case CiVisibility.Public:
          Write("public");
          break;
      }
    }

    void ICiSymbolVisitor.Visit(CiField field) {
      WriteField(field, null, true);
    }

    protected virtual void WriteField(CiField field, string NewName, bool docs) {
      if (docs) {
        Write(field.Documentation);
      }
      Write(field.Visibility);
      Write(" ");
      WriteName(field);
      Write(": ");
      WriteType(field.Type);
      WriteLine(";");
    }

    protected virtual void WriteVar(CiVar var, string NewName, bool docs) {
      if (docs) {
        Write(var.Documentation);
      }
      Write("var ");
      WriteName(var);
      Write(": ");
      WriteType(var.Type);
      WriteLine(";");
    }

    protected override void WriteNew(CiType type) {
      throw new InvalidOperationException("Unsupported call to WriteNew()");
    }

    public override void Visit(CiDelete stmt) {
      if (stmt.Expr is CiVarAccess) {
        CiVar var = ((CiVarAccess)stmt.Expr).Var;
        if (var.Type is CiClassStorageType) {
          Write("FreeAndNil(");
          WriteName(var);
          Write(")");
        }
        else if (var.Type is CiArrayStorageType) {
          TypeMappingInfo info = SuperType.GetTypeInfo(var.Type);
          WriteName(var);
          Write(":= ");
          Write(info.Null);
        }
        else if (var.Type is CiArrayPtrType) {
          TypeMappingInfo info = SuperType.GetTypeInfo(var.Type);
          WriteName(var);
          Write(":= ");
          Write(info.Null);
        }
      }
    }

    void WriteAssignNew(CiVar Target, CiType Type) {
      if (Type is CiClassStorageType) {
        CiClassStorageType classType = (CiClassStorageType)Type;
        WriteName(Target);
        Write(":= ");
        WriteName(classType.Class);
        WriteLine(".Create();");
      }
      else if (Type is CiArrayStorageType) {
        CiArrayStorageType arrayType = (CiArrayStorageType)Type;
        Write("SetLength(");
        WriteName(Target);
        Write(", ");
        if (arrayType.LengthExpr != null) {
          Write(arrayType.LengthExpr);
        }
        else {
          Write(arrayType.Length);
        }
        WriteLine(");");
      }
    }

    void WriteAssignNew(CiExpr Target, CiToken Op, CiType Type) {
      if (Op != CiToken.Assign) {
        throw new InvalidOperationException("Unsupported assigment");
      }
      if (Type is CiClassStorageType) {
        CiClassStorageType classType = (CiClassStorageType)Type;
        Write(Target);
        Write(":= ");
        WriteName(classType.Class);
        WriteLine(".Create();");
      }
      else if (Type is CiArrayStorageType) {
        CiArrayStorageType arrayType = (CiArrayStorageType)Type;
        Write("SetLength(");
        Write(Target);
        Write(", ");
        if (arrayType.LengthExpr != null) {
          Write(arrayType.LengthExpr);
        }
        else {
          Write(arrayType.Length);
        }
        WriteLine(");");
      }
      else if (Type is CiArrayPtrType) {
        CiArrayStorageType arrayType = (CiArrayStorageType)Type;
        Write("SetLength(");
        Write(Target);
        Write(", ");
        if (arrayType.LengthExpr != null) {
          Write(arrayType.LengthExpr);
        }
        else {
          Write(arrayType.Length);
        }
        WriteLine(");");
      }
    }

    void WriteInitNew(CiSymbol symbol, CiType Type) {
      if (Type is CiClassStorageType) {
        CiClassStorageType classType = (CiClassStorageType)Type;
        WriteName(symbol);
        Write(":= ");
        WriteName(classType.Class);
        WriteLine(".Create();");
      }
      else if (Type is CiArrayStorageType) {
        CiArrayStorageType arrayType = (CiArrayStorageType)Type;
        Write("SetLength(");
        WriteName(symbol);
        Write(", ");
        if (arrayType.LengthExpr != null) {
          Write(arrayType.LengthExpr);
        }
        else {
          Write(arrayType.Length);
        }
        WriteLine(");");
      }
    }

    void WriteInitVal(CiSymbol symbol) {
      if (symbol is CiVar) {
        CiVar var = (CiVar)symbol;
        if (var.InitialValue != null) {
          if (var.Type is CiArrayStorageType) {
            Write("__CFILL(");
            WriteName(var);
            Write(",");
            Write(var.InitialValue);
            Write(")");
          }
          else {
            WriteAssign(var, var.InitialValue);
          }
        }
      }
    }

    protected override void WriteConst(object value) {
      throw new InvalidOperationException("Ops... WriteConst must have a type");
    }

    protected virtual void WriteConst(CiType type, object value) {
      if (value is string) {
        Write('\'');
        foreach (char c in (string) value) {
          if ((int)c < 32) {
            Write("'+chr(" + (int)c + ")+'");
          }
          else if (c == '\'') {
            Write("''");
          }
          else {
            Write(c);
          }
        }
        Write('\'');
      }
      else if (value is Array) {
        Write("( ");
        WriteContent(type, (Array)value);
        Write(" )");
      }
      else if (value == null) {
        if ((type is CiStringType) || (type is CiStringPtrType) || (type is CiStringStorageType)) {
          Write("''");
        }
        else if ((type is CiArrayStorageType) || (type is CiArrayPtrType)) {
          Write("EMPTY_");
          Write(type);
        }
        else {
          Write("nil");
        }
      }
      else if (value is bool) {
        Write((bool)value ? "true" : "false");
      }
      else if (value is byte) {
        Write((byte)value);
      }
      else if (value is int) {
        Write((int)value);
      }
      else if (value is CiEnumValue) {
        CiEnumValue ev = (CiEnumValue)value;
        Write(ev.Type.Name);
        Write('.');
        Write(ev.Name);
      }
      else {
        throw new ArgumentException(value.ToString());
      }
    }

    virtual protected void WriteConstFull(CiConst konst) {
      object value = konst.Value;
      if (value is Array) {
        CiType elemType = null;
        if (konst.Type is CiArrayStorageType) {
          CiArrayStorageType type = (CiArrayStorageType)konst.Type;
          elemType = type.ElementType;
        }
        Array array = (Array)value;
        Write("SetLength(");
        WriteName(konst);
        Write(", ");
        Write(array.Length);
        WriteLine(");");
        for (int i = 0; i < array.Length; i++) {
          WriteName(konst);
          Write("[" + i + "]:= ");
          WriteConst(elemType, array.GetValue(i));
          WriteLine(";");
        }
      }
      else {
        WriteName(konst);
        Write(":= ");
        WriteConst(value);
        WriteLine(";");
				
      }
    }

    public override void Visit(CiThrow stmt) {
      Write("Raise Exception.Create(");
      Write(stmt.Message);
      WriteLine(");");
    }

    protected override void Write(CiCondExpr expr) {
      Write("IfThen(");
      WriteNonAssocChild(expr, expr.Cond);
      Write(", ");
      WriteCondChild(expr, expr.OnTrue);
      Write(", ");
      WriteCondChild(expr, expr.OnFalse);
      Write(")");
    }

    protected override void Write(CiVarAccess expr) {
      string name = expr.Var.Name;
      if (name.Equals("this")) {
        Write("self");
      }
      else {
        WriteName(expr.Var);
      }
    }

    protected override void Write(CiFieldAccess expr) {
      WriteChild(expr, expr.Obj);
      Write('.');
      WriteName(expr.Field);
    }

    public override void Visit(CiDoWhile stmt) {
      BreakExit.Push(stmt);
      WriteLine("repeat");
      if ((stmt.Body != null) && (stmt.Body is CiBlock)) {
        OpenBlock(false);
        WriteCode(((CiBlock)stmt.Body).Statements);
        CloseBlock(false);
      }
      else {
        WriteChild(stmt.Body);
      }
      Write("until not(");
      Write(stmt.Cond);
      WriteLine(");");
      BreakExit.Pop();
    }

    protected bool WriteCaseInternal(CiSwitch swich, CiCase kase, ICiStatement[] body, string prefix) {
      if (body == null) {
        return false;
      }
      bool hasStmt = body.Any(s => !(s is CiBreak));
      if (!hasStmt) {
        return false;
      }
      if (!String.IsNullOrEmpty(prefix)) {
        Write(prefix);
      }
      OpenBlock();
      CiBreak breakFound = null;
      foreach (ICiStatement bodyStmt in body) {
        if (breakFound != null) {
          Write(breakFound);
          breakFound = null;
        }
        if (!(bodyStmt is CiBreak)) {
          Write(bodyStmt);
        }
        else {
          breakFound = (CiBreak)bodyStmt;
        }
      }
      if ((kase != null) && (kase.Fallthrough)) {
        if (kase.FallthroughTo == null) {
          WriteCaseInternal(swich, null, swich.DefaultBody, null);
        }
        else {
          if (kase.FallthroughTo is CiConstExpr) {
            string e = ((CiConstExpr)kase.FallthroughTo).Value.ToString();
            bool stop = false;
            foreach (var kkase in swich.Cases) {
              foreach (var val in kkase.Values) {
                if (val.ToString().Equals(e)) {
                  WriteLine("// include case " + val);
                  WriteCaseInternal(swich, kkase, kkase.Body, null);
                  WriteLine(";");
                  stop = true;
                  break;
                }
                if (stop) {
                  break;
                }
              }
            }
          }
          else {
            throw new InvalidOperationException("Unsupported  Fallthrough");
          }
        }
      }
      CloseBlock();
      return true;
    }

    public override void Visit(CiSwitch swich) {
      BreakExit label = BreakExit.Push(swich);
      Write("case (");
      Write(swich.Value);
      WriteLine(") of");
      OpenBlock(false);
      foreach (CiCase kase in swich.Cases) {
        bool first = true;
        foreach (object value in kase.Values) {
          if (!first) {
            Write(", ");
          }
          else {
            first = false;
          }
          WriteConst(null, value);
        }
        Write(": ");
        WriteCaseInternal(swich, kase, kase.Body, null);
        WriteLine(";");
      }
      if (WriteCaseInternal(swich, null, swich.DefaultBody, "else ")) {
        WriteLine(";");
      }
      CloseBlock(false);
      WriteLine("end;");
      BreakExit.Pop();
      if (label != null) {
        WriteLine(label.Name + ": ;");
      }
    }

    protected override void WriteOp(CiBinaryExpr expr) {
      switch (expr.Op) {
        case CiToken.Plus:
          Write(" + ");
          break;
        case CiToken.Minus:
          Write(" - ");
          return;
        case CiToken.Asterisk:
          Write(" * ");
          break;
        case CiToken.Slash:
          WriteDiv(ExprType.Get(expr));
          break;
        case CiToken.Mod:
          Write(" mod ");
          break;
        case CiToken.ShiftLeft:
          Write(" shl ");
          break;
        case CiToken.ShiftRight:
          Write(" shr ");
          break;
        case CiToken.Less:
          Write(" < ");
          break;
        case CiToken.LessOrEqual:
          Write(" <= ");
          break;
        case CiToken.Greater:
          Write(" > ");
          break;
        case CiToken.GreaterOrEqual:
          Write(" >= ");
          break;
        case CiToken.Equal:
          Write(" = ");
          break;
        case CiToken.NotEqual:
          Write(" <> ");
          break;
        case CiToken.And:
          Write(" and ");
          break;
        case CiToken.Or:
          Write(" or ");
          break;
        case CiToken.Xor:
          Write(" xor ");
          break;
        case CiToken.CondAnd:
          Write(" and ");
          break;
        case CiToken.CondOr:
          Write(" or ");
          break;
        default:
          throw new ArgumentException(expr.Op.ToString());
      }
    }

    protected override void Write(CiArrayAccess expr) {
      WriteChild(expr, expr.Array);
      Write('[');
      Write(expr.Index);
      Write(']');
    }

    protected virtual void WriteArguments(CiMethodCall expr, bool[] conds) {
      Write('(');
      int cond = 0;
      for (int i=0; i<expr.Arguments.Length; i++) {
        CiExpr arg = expr.Arguments[i];
        CiParam prm = expr.Signature.Params[i];
        if (i > 0) {
          Write(", ");
        }
        if ((arg is CiCondExpr) && (conds != null)) {
          if (conds[cond]) {
            WriteExpr(prm.Type, ((CiCondExpr)arg).OnTrue);
          }
          else {
            WriteExpr(prm.Type, ((CiCondExpr)arg).OnFalse);
          }
          cond++;
        }
        else {
          WriteExpr(prm.Type, arg);
        }
      }
      Write(')');
    }

    protected virtual void WriteMethodCall2(CiMethodCall expr, bool[] cond) {
      if (expr.Method != null) {
        if (expr.Obj != null) {
          Write(expr.Obj);
        }
        else {
          WriteName(expr.Method.Class);
        }
        Write('.');
        WriteName(expr.Method);
      }
      else {
        WriteDelegateCall(expr.Obj);
      }
      WriteArguments(expr, cond);
    }

    protected virtual void BuildCond(int cond, CiMethodCall expr, int level) {
      int mask = 1;
      bool[] leaf = new bool[level];
      for (int i=0; i<level; i++, mask *= 2) {
        leaf[i] = ((cond & mask) == mask);
      }
      WriteMethodCall2(expr, leaf);
    }

    protected virtual void WriteMethodCall(CiMethodCall expr) {
      bool processed = false;
      if (expr.Arguments.Length > 0) {
        if (!NoIIFExpand.In(1)) {
          List<CiExpr> iifs = expr.Arguments.Where(arg => arg is CiCondExpr).ToList();
          int level = iifs.Count;
          if (level > 0) {
            if (level > 1) {
              Write("case (__getMagic([");
              for (int i=0; i<level; i++) {
                if (i > 0) {
                  Write(", ");
                }
                Write(((CiCondExpr)iifs[i]).Cond);
              }
              WriteLine("])) of");
              OpenBlock(false);
              for (int i=0; i<(1<<level); i++) {
                Write(i + ": ");
                BuildCond(i, expr, level);
                WriteLine(";");
              }
              CloseBlock(false);
              Write("end");
              processed = true;
            }
            else {
              Write("if ");
              Write(((CiCondExpr)iifs[0]).Cond);
              Write(" then ");
              BuildCond(1, expr, 1);
              Write(" else ");
              BuildCond(0, expr, 1);
              processed = true;
            }
          }
        }
      }
      if (!processed) {
        WriteMethodCall2(expr, null);
      }
    }

    protected override void Write(CiMethodCall expr) {
      if (expr.Method == CiLibrary.MulDivMethod) {
        Write("(int64(");
        WriteChild(CiPriority.Prefix, expr.Obj);
        Write(") * int64(");
        WriteChild(CiPriority.Multiplicative, expr.Arguments[0]);
        Write(") div ");
        WriteNonAssocChild(CiPriority.Multiplicative, expr.Arguments[1]);
        Write(")");
      }
      else if (expr.Method == CiLibrary.CharAtMethod) {
        Write("ord(");
        Write(expr.Obj);
        Write("[");
        Write(expr.Arguments[0]);
        Write("+1])");
      }
      else if (expr.Method == CiLibrary.SubstringMethod) {
        Write("MidStr(");
        Write(expr.Obj);
        Write(", ");
        Write(expr.Arguments[0]);
        Write("+1, ");
        Write(expr.Arguments[1]);
        Write(")");
      }
      else if (expr.Method == CiLibrary.ArrayCopyToMethod) {
        Write("__CCOPY(");
        Write(expr.Obj);
        Write(", ");
        Write(expr.Arguments[0]);
        Write(", ");
        Write(expr.Arguments[1]);
        Write(", ");
        Write(expr.Arguments[2]);
        Write(", ");
        Write(expr.Arguments[3]);
        Write(')');
      }
      else if (expr.Method == CiLibrary.ArrayToStringMethod) {
        Write("__TOSTR(");
        Write(expr.Obj);
//				Write(expr.Arguments[0]);
//				Write(expr.Arguments[1]);
        Write(")");
      }
      else if (expr.Method == CiLibrary.ArrayStorageClearMethod) {
        Write("__CCLEAR(");
        Write(expr.Obj);
        Write(")");
      }
      else {
        WriteMethodCall(expr);
      }
    }

    public void WriteAssign(CiVar Target, CiExpr Source) {
      if (!NoIIFExpand.In(1) && (Source is CiCondExpr)) {
        CiCondExpr expr = (CiCondExpr)Source;
        Write("if ");
        WriteNonAssocChild(expr, expr.Cond);
        Write(" then ");
        WriteAssign(Target);
        WriteCondChild(expr, expr.OnTrue);
        Write(" else ");
        WriteAssign(Target);
        WriteCondChild(expr, expr.OnFalse);
      }
      else if (Source is CiNewExpr) {
        WriteAssignNew(Target, ((CiNewExpr)Source).NewType);
      }
      else {
        NoIIFExpand.Push(1);
        WriteAssign(Target);
        WriteInline(Source);
        NoIIFExpand.Pop();
      }
			
    }

    public void WriteAssign(CiExpr Target, CiToken Op, CiMaybeAssign Source) {
      if (Source is CiCondExpr) {
        CiCondExpr expr = (CiCondExpr)Source;
        Write("if ");
        WriteNonAssocChild(expr, expr.Cond);
        Write(" then ");
        WriteAssign(Target, Op);
        WriteCondChild(expr, expr.OnTrue);
        Write(" else ");
        WriteAssign(Target, Op);
        WriteCondChild(expr, expr.OnFalse);
      }
      else if (Source is CiNewExpr) {
        WriteAssignNew(Target, Op, ((CiNewExpr)Source).NewType);
      }
      else {
        NoIIFExpand.Push(1);
        WriteAssign(Target, Op);
        WriteInline(Source);
        NoIIFExpand.Pop();
      }
    }

    public void WriteAssign(CiVar Target) {
      WriteName(Target);
      Write(":= ");
    }

    public void WriteDiv(CiType type) {
      if ((type is CiIntType) || (type is CiByteType)) {
        Write(" div ");
      }
      else {
        Write(" / ");
      }
    }

    public void WriteAssign(CiExpr Target, CiToken Op) {
      Write(Target);
      Write(":= ");
      if (Op != CiToken.Assign) {
        Write(Target);
        switch (Op) {
          case CiToken.AddAssign:
            Write(" + ");
            break;
          case CiToken.SubAssign:
            Write(" - ");
            break;
          case CiToken.MulAssign:
            Write(" * ");
            break;
          case CiToken.DivAssign:
            WriteDiv(ExprType.Get(Target));
            break;
          case CiToken.ModAssign:
            Write(" mod ");
            break;
          case CiToken.ShiftLeftAssign:
            Write(" shl ");
            break;
          case CiToken.ShiftRightAssign:
            Write(" shr ");
            break;
          case CiToken.AndAssign:
            Write(" and ");
            break;
          case CiToken.OrAssign:
            Write(" or ");
            break;
          case CiToken.XorAssign:
            Write(" xor ");
            break;
          default:
            throw new ArgumentException(Op.ToString());
        }
      }
    }

    public override void Visit(CiAssign assign) {
      if (assign.Source is CiAssign) {
        CiAssign prev = (CiAssign)assign.Source;
        Visit(prev);
        WriteLine(";");
        WriteAssign(assign.Target, assign.Op, prev.Target);
      }
      else {
        WriteAssign(assign.Target, assign.Op, assign.Source);
      }
    }

    public override void Visit(CiReturn stmt) {
      if (stmt.Value == null) {
        Write("exit");
      }
      else {
        NoIIFExpand.Push(1);
        if (stmt.Value is CiCondExpr) {
          CiCondExpr expr = (CiCondExpr)stmt.Value;
          Write("if ");
          WriteNonAssocChild(expr, expr.Cond);
          Write(" then ");
          Write("Result:= ");
          WriteCondChild(expr, expr.OnTrue);
          Write(" else ");
          Write("Result:= ");
          WriteCondChild(expr, expr.OnFalse);
          WriteLine(";");
        }
        else if (stmt.Value is CiNewExpr) {
          CiVar result = new CiVar();
          result.Name = "Result";
          WriteInitNew(result, ((CiNewExpr)stmt.Value).NewType);
          WriteLine(";");
        }
        else {
          Write("Result:= ");
          if (stmt.Value is CiConstExpr) {
            CiMethod call = MethodStack.Peek();
            WriteConst((call != null ? call.Signature.ReturnType : null), ((CiConstExpr)stmt.Value).Value);
          }
          else {
            Write(stmt.Value);
          }
          WriteLine(";");
        }
        Write("exit");
        NoIIFExpand.Pop();
      }
    }

    protected override void Write(ICiStatement stmt) {
      if (stmt == null) {
        return;
      }
      stmt.Accept(this);
      if (newText.Length > 0) {
        WriteLine(";");
      }
    }

    public override void Visit(CiBreak stmt) {
      BreakExit label = BreakExit.Peek();
      if (label != null) {
        WriteLine("goto " + label.Name + ";");
      }
      else {
        WriteLine("break;");
      }
    }

    protected virtual void WriteExpr(CiType type, CiExpr expr) {
      if (expr is CiConstExpr) {
        WriteConst(type, ((CiConstExpr)expr).Value);
      }
      else if (expr is CiConstAccess) {
        WriteName(((CiConstAccess)expr).Const);
      }
      else if (expr is CiVarAccess) {
        Write((CiVarAccess)expr);
      }
      else if (expr is CiFieldAccess) {
        Write((CiFieldAccess)expr);
      }
      else if (expr is CiPropertyAccess) {
        Write((CiPropertyAccess)expr);
      }
      else if (expr is CiArrayAccess) {
        Write((CiArrayAccess)expr);
      }
      else if (expr is CiMethodCall) {
        Write((CiMethodCall)expr);
      }
      else if (expr is CiUnaryExpr) {
        Write((CiUnaryExpr)expr);
      }
      else if (expr is CiCondNotExpr) {
        Write((CiCondNotExpr)expr);
      }
      else if (expr is CiPostfixExpr) {
        Write((CiPostfixExpr)expr);
      }
      else if (expr is CiBinaryExpr) {
        Write((CiBinaryExpr)expr);
      }
      else if (expr is CiCondExpr) {
        Write((CiCondExpr)expr);
      }
      else if (expr is CiBinaryResourceExpr) {
        Write((CiBinaryResourceExpr)expr);
      }
      else if (expr is CiNewExpr) {
        WriteNew(((CiNewExpr)expr).NewType);
      }
      else if (expr is CiCoercion) {
        Write((CiCoercion)expr);
      }
      else {
        throw new ArgumentException(expr.ToString());
      }
    }

    protected override void Write(CiExpr expr) {
      WriteExpr(ExprType.Get(expr), expr);
    }

    protected virtual void WriteContent(CiType type, Array array) {
      for (int i = 0; i < array.Length; i++) {
        if (i > 0) {
          if (i % 16 == 0) {
            WriteLine(",");
            Write('\t');
          }
          else {
            Write(", ");
          }
        }
        WriteConst(type, array.GetValue(i));
      }
    }

    public void Visit(CiDelegate del) {
      throw new InvalidOperationException("Unsupported Visit(CiDelegate)");
    }
    // Unchecked
    void WriteDoc(string text) {
      foreach (char c in text) {
        switch (c) {
          case '{':
            Write("[");
            break;
          case '}':
            Write("]");
            break;
          case '\r':
            break;
          case '\n':
            break;
          default:
            Write(c);
            break;
        }
      }
    }

    void Write(CiDocPara para) {
      foreach (CiDocInline inline in para.Children) {
        CiDocText text = inline as CiDocText;
        if (text != null) {
          WriteDoc(text.Text);
          continue;
        }
        CiDocCode code = inline as CiDocCode;
        if (code != null) {
          switch (code.Text) {
            case "true":
              Write("<see langword=\"true\" />");
              break;
            case "false":
              Write("<see langword=\"false\" />");
              break;
            case "null":
              Write("<see langword=\"null\" />");
              break;
            default:
              Write("<c>");
              WriteDoc(code.Text);
              Write("</c>");
              break;
          }
          continue;
        }
        throw new ArgumentException(inline.GetType().Name);
      }
    }

    void Write(CiDocBlock block) {
      CiDocList list = block as CiDocList;
      if (list != null) {
        WriteLine();
        WriteLine("/// <list type=\"bullet\">");
        foreach (CiDocPara item in list.Items) {
          Write("/// <item>");
          Write(item);
          WriteLine("</item>");
        }
        Write("/// </list>");
        WriteLine();
        Write("/// ");
        return;
      }
      Write((CiDocPara)block);
    }

    protected override void Write(CiCodeDoc doc) {
      if (doc == null) {
        return;
      }
      Write("{ ");
      Write(doc.Summary);
      WriteLine(" }");
      if (doc.Details.Length > 0) {
        Write("{ <remarks>");
        foreach (CiDocBlock block in doc.Details) {
          Write(block);
        }
        WriteLine("</remarks> }");
      }
    }

    void ICiSymbolVisitor.Visit(CiConst konst) {
      Write(konst.Documentation);
      Write("public ");
      if (!(konst.Type is CiArrayType)) {
        Write("const ");
      }
      WriteName(konst);
      Write(": ");
      Write(konst.Type);
      if (!(konst.Type is CiArrayType)) {
        Write(" = ");
        WriteConst(konst.Type, konst.Value);
      }
      WriteLine(";");
    }

    protected override CiPriority GetPriority(CiExpr expr) {
      if (expr is CiPropertyAccess) {
        CiProperty prop = ((CiPropertyAccess)expr).Property;
        if (prop == CiLibrary.SByteProperty || prop == CiLibrary.LowByteProperty)
          return CiPriority.Prefix;
      }
      else if (expr is CiCoercion) {
        CiCoercion c = (CiCoercion)expr;
        if (c.ResultType == CiByteType.Value && c.Inner.Type == CiIntType.Value)
          return CiPriority.Prefix;
      }
      return base.GetPriority(expr);
    }

    protected override void Write(CiPropertyAccess expr) {
      if (expr.Property == CiLibrary.SByteProperty) {
        Write("shortint(");
        WriteChild(expr, expr.Obj);
        Write(")");
      }
      else if (expr.Property == CiLibrary.LowByteProperty) {
        Write("byte(");
        WriteChild(expr, expr.Obj);
        Write(")");
      }
      else if (expr.Property == CiLibrary.StringLengthProperty) {
        Write("Length(");
        WriteChild(expr, expr.Obj);
        Write(")");
      }
      else {
        throw new ArgumentException(expr.Property.Name);
      }
    }

    void WriteCondChild(CiCondExpr condExpr, CiExpr expr) {
      if (condExpr.ResultType == CiByteType.Value && expr is CiConstExpr) {
        Write("byte(");
        WriteChild(condExpr, expr);
        Write(")");
      }
      else {
        WriteChild(condExpr, expr);
      }
    }

    protected override void Write(CiCoercion expr) {
      if (expr.ResultType == CiByteType.Value && expr.Inner.Type == CiIntType.Value) {
        Write("byte(");
        WriteChild(expr, (CiExpr)expr.Inner); // TODO: Assign
        Write(")");
      }
      else {
        base.Write(expr);
      }
    }

    protected override void WriteFallthrough(CiExpr expr) {
      throw new InvalidOperationException("WriteFallthrough should be nevel called");
    }
  }
}