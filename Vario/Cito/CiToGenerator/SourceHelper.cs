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

  public delegate bool StatementAction(ICiStatement s);
  //
  public class PascalPreProcessing {
			
    static public bool Execute(ICiStatement[] stmt, StatementAction action) {
      if (stmt != null) {
        foreach (ICiStatement s in stmt) {
          if (Execute(s, action)) {
            return true;
          }
        }
      }
      return false;
    }

    static public bool Execute(ICiStatement stmt, StatementAction action) {
      if (stmt == null) {
        return false;
      }
      if (action(stmt)) {
        return true;
      }
      if (stmt is CiBlock) {
        if (Execute(((CiBlock)stmt).Statements, action)) {
          return true;
        }
      }
      else if (stmt is CiFor) {
          CiFor loop = (CiFor)stmt;
          if (Execute(loop.Init, action)) {
            return true;
          }
          if (Execute(loop.Body, action)) {
            return true;
          }
          if (Execute(loop.Advance, action)) {
            return true;
          }
        }
        else if (stmt is CiLoop) {
            CiLoop loop = (CiLoop)stmt;
            if (Execute(loop.Body, action)) {
              return true;
            }
          }
          else if (stmt is CiIf) {
              CiIf iiff = (CiIf)stmt;
              if (Execute(iiff.OnTrue, action)) {
                return true;
              }
              if (Execute(iiff.OnFalse, action)) {
                return true;
              }
            }
            else if (stmt is CiSwitch) {
                CiSwitch swith = (CiSwitch)stmt;
                foreach (CiCase cas in swith.Cases) {
                  if (Execute(cas.Body, action)) {
                    return true;
                  }
                }
                if (Execute(swith.DefaultBody, action)) {
                  return true;
                }
              }
      return false;
    }

    public void Parse(CiProgram program) {
      SymbolMapping.Reset();
      BreakExit.Reset();
      ClassOrder.Reset();
      NoIIFExpand.Reset();
      ExprType.Reset();
      SuperType.Reset();
      MethodStack.Reset();
      SymbolMapping root = new SymbolMapping(null);
      foreach (CiSymbol symbol in program.Globals) {
        if (symbol is CiEnum) {
          SymbolMapping.AddSymbol(root, symbol);
        }
      }
      foreach (CiSymbol symbol in program.Globals) {
        if (symbol is CiDelegate) {
          SymbolMapping.AddSymbol(root, symbol);
        }
      }
      foreach (CiSymbol symbol in program.Globals) {
        if (symbol is CiClass) {
          ClassOrder.AddClass((CiClass)symbol);
        }
      }
      foreach (CiClass klass in ClassOrder.GetList()) {
        SymbolMapping parent = (klass.BaseClass != null ? SymbolMapping.Find(klass.BaseClass) : root);
        SymbolMapping.AddSymbol(parent, klass);
      }
      foreach (CiClass klass in ClassOrder.GetList()) {
        Parse(klass);
      }
    }

    public void Parse(CiClass klass) {
      SymbolMapping parent = SymbolMapping.Find(klass);
      foreach (CiSymbol member in klass.Members) {
        if (member is CiField) {
          SymbolMapping.AddSymbol(parent, member);
          SuperType.AddType(((CiField)member).Type);
        }
      }
      foreach (CiConst konst in klass.ConstArrays) {
        SymbolMapping.AddSymbol(parent, konst);
        SuperType.AddType(konst.Type);
      }
      foreach (CiBinaryResource resource in klass.BinaryResources) {
        SymbolMapping.AddSymbol(parent, resource);
        SuperType.AddType(resource.Type);
      }
      if (klass.Constructor != null) {
        SymbolMapping.AddSymbol(parent, klass.Constructor);
      }
      foreach (CiSymbol member in klass.Members) {
        if (member is CiMethod) {
          SymbolMapping methodContext = SymbolMapping.AddSymbol(parent, member, false);
          CiMethod method = (CiMethod)member;
          if (method.Signature.Params.Length > 0) {
            SymbolMapping methodCall = SymbolMapping.AddSymbol(methodContext, null);
            foreach (CiParam p in method.Signature.Params) {
              SymbolMapping.AddSymbol(methodCall, p);
              SuperType.AddType(p.Type);
            }
          }
          SuperType.AddType(method.Signature.ReturnType);
        }
      }
      if (klass.Constructor != null) {
        Visit(klass.Constructor);
      }
      foreach (CiSymbol member in klass.Members) {
        if (member is CiMethod) {
          Visit((CiMethod)member);
        }
      }
    }

    public void Visit(CiMethod method) {
      Execute((method.Body != null ? method.Body : null), s => VisitStatement(method, s));
    }

    protected bool CheckCode(ICiStatement[] code) {
      CiBreak brk = (code != null) ? code[code.Length - 1] as CiBreak : null;
      return PascalPreProcessing.Execute(code, s => ((s is CiBreak) && (s != brk)));
    }

    protected bool VisitStatement(CiMethod method, ICiStatement stmt) {
      if (stmt is CiVar) {
        var v = (CiVar)stmt;
        SymbolMapping parent = SymbolMapping.Find(method);
        string vName = SymbolMapping.GetPascalName(v.Name);
        // Look if local CiTo var in already defined in Pascal procedure vars
        foreach (SymbolMapping item in parent.childs) {
          if (String.Compare(item.NewName, vName, true) == 0) {
            return false;
          }
        }
        SymbolMapping.AddSymbol(parent, v);
        SuperType.AddType(v.Type);
      }
      else if (stmt is CiSwitch) {
          CiSwitch swith = (CiSwitch)stmt;
          bool needExit = false;
          foreach (CiCase kase in swith.Cases) {
            needExit = CheckCode(kase.Body);
            if (needExit) {
              break;
            }
          }
          if (!needExit) {
            needExit = CheckCode(swith.DefaultBody);
          }
          if (needExit) {
            BreakExit.AddSwitch(method, swith);
          }
        }
      return false;
    }
  }

  public class SymbolMapping {
    //
    static private string[] PascalWords = new String[] {
      "absolute",
      "and",
      "array",
      "as",
      "asm",
      "begin",
      "case",
      "class",
      "const",
      "constructor",
      "destructor",
      "dispinterface",
      "div",
      "do",
      "downto",
      "else",
      "end",
      "except",
      "exports",
      "file",
      "finalization",
      "finally",
      "for",
      "function",
      "goto",
      "if",
      "implementation",
      "in",
      "inherited",
      "initialization",
      "inline",
      "inline",
      "interface",
      "is",
      "label",
      "library",
      "mod",
      "nil",
      "not",
      "object",
      "of",
      "on",
      "on",
      "operator",
      "or",
      "out",
      "packed",
      "packed",
      "procedure",
      "program",
      "property",
      "raise",
      "record",
      "reintroduce",
      "repeat",
      "resourcestring",
      "self",
      "set",
      "shl",
      "shr",
      "string",
      "then",
      "threadvar",
      "to",
      "try",
      "type",
      "unit",
      "until",
      "uses",
      "var",
      "while",
      "with",
      "xor",
      "result",
      "length"
    };
    static private int suffix = 0;
    static private Dictionary<CiSymbol, SymbolMapping> varMap = new  Dictionary<CiSymbol, SymbolMapping>();
    static public HashSet<string> ReservedWords = new HashSet<string>(PascalWords);
    //
    public CiSymbol Symbol = null;
    public string NewName = "?";
    public SymbolMapping Parent = null;
    public List<SymbolMapping> childs = new List<SymbolMapping>();

    static public void Reset() {
      suffix = 0;
      varMap.Clear();
    }

    static public bool IsEmpty() {
      return varMap.Count == 0;
    }

    static public SymbolMapping AddSymbol(SymbolMapping aParent, CiSymbol aSymbol) {
      return AddSymbol(aParent, aSymbol, true);
    }

    static public SymbolMapping AddSymbol(SymbolMapping aParent, CiSymbol aSymbol, bool inParentCheck) {
      SymbolMapping item = new SymbolMapping(aParent, aSymbol, inParentCheck);
      if (aSymbol != null) {
        varMap.Add(aSymbol, item);
      }
      return item;
    }

    static public SymbolMapping Find(CiSymbol symbol) {
      SymbolMapping result = null;
      varMap.TryGetValue(symbol, out result);
      return result;
    }

    static public string GetPascalName(string name) {
      StringBuilder tmpName = new StringBuilder(name.Length);
      foreach (char c in name) {
        tmpName.Append(CiLexer.IsLetter(c) ? c : '_');
      }
      string baseName = tmpName.ToString();
      if (ReservedWords.Contains(baseName.ToLower())) {
        baseName = "a" + baseName;
      }
      return baseName;
    }

    public SymbolMapping(CiSymbol aSymbol) {
      this.Symbol = aSymbol;
    }

    public SymbolMapping(SymbolMapping aParent, CiSymbol aSymbol, bool inParentCheck) {
      this.Symbol = aSymbol;
      this.Parent = aParent;
      if (aParent != null) {
        aParent.childs.Add(this);
      }
      this.NewName = this.GetUniqueName(inParentCheck);
    }

    public string GetUniqueName(bool inParentCheck) {
      if (this.Symbol == null) {
        return "?";
      }
      string baseName = GetPascalName(this.Symbol.Name);
      SymbolMapping context = this.Parent;
      while (context!=null) {
        foreach (SymbolMapping item in context.childs) {
          if (String.Compare(item.NewName, baseName, true) == 0) {
            //TODO Generate a real unique name
            suffix++;
            return baseName + "__" + suffix;
          }
        }
        if (inParentCheck) {
          context = context.Parent;
        }
        else {
          context = null;
        }
      }
      return baseName;
    }
  }

  public class ClassOrder {
    static private List<CiClass> order = new List<CiClass>();

    static public List<CiClass>GetList() {
      return order;
    }

    static public void AddClass(CiClass klass) {
      if (klass == null) {
        return;
      }
      if (order.Contains(klass)) {
        return;
      }
      AddClass(klass.BaseClass);
      order.Add(klass);
    }

    static public void Reset() {
      order.Clear();
    }
  }

  public struct TypeMappingInfo {
    public CiType Type;
    public string Name;
    public string Definition;
    public bool Native;
    public string Null;
    public string NullInit;
    public string Init;
    public int level;
    public string ItemType;
    public string ItemDefault;
  }

  public class SuperType {
    static private HashSet<CiClass> refClass = new HashSet<CiClass>();
    static private HashSet<CiType> refType = new HashSet<CiType>();
    static private Dictionary<CiType, TypeMappingInfo> TypeCache = new Dictionary<CiType, TypeMappingInfo>();

    static public HashSet<CiClass>GetClassList() {
      return refClass;
    }

    static public HashSet<CiType>GetTypeList() {
      return refType;
    }

    static public string GetTypeName(CiType type) {
      return GetTypeInfo(type).Name;
    }

    static public TypeMappingInfo GetTypeInfo(CiType type) {
      if (TypeCache.ContainsKey(type)) {
        return TypeCache[type];
      }
      TypeMappingInfo info = DecodeType(type);
      TypeCache.Add(type, info);
      return info;
    }

    static public TypeMappingInfo DecodeType(CiType type) {
      TypeMappingInfo info = new TypeMappingInfo();
      info.Type = type;
      info.Native = true;
      info.level = 0;
      StringBuilder name = new StringBuilder();
      StringBuilder def = new StringBuilder();
      StringBuilder nul = new StringBuilder();
      StringBuilder nulInit = new StringBuilder();
      StringBuilder init = new StringBuilder();
      CiType elem = type;
      if (type.ArrayLevel > 0) {
        nul.Append("EMPTY_");
        nulInit.Append("SetLength({3}");
        init.Append("SetLength([0]");
        for (int i = 0; i < type.ArrayLevel; i++) {
          def.Append("array of ");
          name.Append("ArrayOf_");
          nul.Append("ArrayOf_");
          nulInit.Append(", 0");
          init.Append(", [" + (i + 1) + "]");
        }
        info.Native = false;
        nulInit.Append(")");
        init.Append(")");
        info.level = type.ArrayLevel;
        elem = type.BaseType;
      }
      if (elem is CiStringType) {
        name.Append("string");
        def.Append("string");
        info.ItemDefault = "''";
        info.ItemType = "string";
        if (info.Native) {
          nul.Append("''");
        }
        else {
          nul.Append("string");
        }
      }
      else if (elem == CiBoolType.Value) {
          name.Append("boolean");
          def.Append("boolean");
          info.ItemDefault = "false";
          info.ItemType = "boolean";
          if (info.Native) {
            nul.Append("''");
          }
          else {
            nul.Append("boolean");
          }
        }
        else if (elem == CiByteType.Value) {
            name.Append("byte");
            def.Append("byte");
            info.ItemDefault = "0";
            info.ItemType = "byte";
            if (info.Native) {
              nul.Append("0");
            }
            else {
              nul.Append("byte");
            }
          }
          else if (elem == CiIntType.Value) {
              name.Append("integer");
              def.Append("integer");
              info.ItemDefault = "0";
              info.ItemType = "integer";
              if (info.Native) {
                nul.Append("0");
              }
              else {
                nul.Append("integer");
              }
            }
            else {
              name.Append(elem.Name);
              def.Append(elem.Name);
              info.ItemDefault = "nil";
              if (info.Native) {
                nul.Append("nil");
              }
              else {
                nul.Append(elem.Name);
              }
              info.ItemType = elem.Name;
            }
      info.Name = name.ToString();
      info.Definition = def.ToString();
      info.Null = nul.ToString();
      info.NullInit = (nulInit.Length > 0 ? String.Format(nulInit.ToString(), info.Name, info.Definition, info.ItemType, info.Null).Replace('[', '{').Replace(']', '}') : null);
      info.Init = (nulInit.Length > 0 ? String.Format(init.ToString(), info.Name, info.Definition, info.ItemType, info.Null) : null);
      if ((!info.Native) && (info.Null != null)) {
        if (!SymbolMapping.ReservedWords.Contains(info.Null)) {
          SymbolMapping.ReservedWords.Add(info.Null);
        }
      }
      return info;
    }

    static public void AddType(CiType type) {
      if (type == null) {
        return;
      }
      if (refType.Contains(type)) {
        return;
      }
      if (type is CiArrayType) {
        CiArrayType arr = (CiArrayType)type;
        if (arr.BaseType is CiClassType) {
          CiClass klass = ((CiClassType)arr.BaseType).Class;
          if (!refClass.Contains(klass)) {
            refClass.Add(klass);
          }
        }
      }
      if (type is CiClassType) {
        CiClass klass = ((CiClassType)type).Class;
        if (!refClass.Contains(klass)) {
          refClass.Add(klass);
        }
      }
      refType.Add(type);
    }

    static public void Reset() {
      refClass.Clear();
      refType.Clear();
      TypeCache.Clear();
    }
  }

  public class NoIIFExpand {

    static private Stack<int> instructions = new Stack<int>();

    static public void Reset() {
      instructions.Clear();
    }

    static public void Push(int step) {
      instructions.Push(step);
    }

    static public void Pop() {
      instructions.Pop();
    }

    static public bool In(int inst) {
      return instructions.Any(step => step == inst);
    }
  }

  public class MethodStack {

    static private Stack<CiMethod> methods = new Stack<CiMethod>();

    static public void Reset() {
      methods.Clear();
    }

    static public void Push(CiMethod call) {
      methods.Push(call);
    }

    static public void Pop() {
      methods.Pop();
    }

    static public CiMethod Peek() {
      if (methods.Count > 0) {
        return methods.Peek();
      }
      return null;
    }
  }

  public class ExprType {
    static private Dictionary<CiExpr, CiType> exprMap = new  Dictionary<CiExpr, CiType>();

    static public void Reset() {
      exprMap.Clear();
    }

    static public CiType Get(CiExpr expr) {
      CiType result;
      exprMap.TryGetValue(expr, out result);
      if (result == null) {
        result = Analyze(expr);
        exprMap.Add(expr, result);
      }
      return result;
    }

    static public CiType Analyze(CiExpr expr) {
      if (expr == null)
        return CiType.Null;
      else if (expr is CiUnaryExpr) {
          var e = (CiUnaryExpr)expr;
          CiType t = Analyze(e.Inner);
          exprMap.Add(e.Inner, t);
          return t;
        }
        else if (expr is CiPostfixExpr) {
            var e = (CiPostfixExpr)expr;
            CiType t = Analyze(e.Inner);
            exprMap.Add(e.Inner, t);
            return t;
          }
          else if (expr is CiBinaryExpr) {
              var e = (CiBinaryExpr)expr;
              CiType left = Analyze(e.Left);
              CiType right = Analyze(e.Right);
              CiType t = ((left == null) || (left == CiType.Null)) ? right : left;
              if (!exprMap.ContainsKey(e.Left)) {
                exprMap.Add(e.Left, t);
              }
              if (!exprMap.ContainsKey(e.Right)) {
                exprMap.Add(e.Right, t);
              }
              return t;
            }
            else {
              return expr.Type;
            }
    }
  }

  public class BreakExit {
    //
    static private Dictionary<ICiStatement, BreakExit> mapping = new  Dictionary<ICiStatement, BreakExit>();
    static private Dictionary<CiMethod, List<BreakExit>> methods = new Dictionary<CiMethod, List<BreakExit>>();
    static private Stack<BreakExit> exitPoints = new Stack<BreakExit>();
    //
    public string Name;

    public BreakExit(string aName) {
      this.Name = aName;
    }

    static public void AddSwitch(CiMethod method, CiSwitch aSymbol) {
      List<BreakExit> labels = null;
      methods.TryGetValue(method, out labels);
      if (labels == null) {
        labels = new List<BreakExit>();
        methods.Add(method, labels);
      }
      BreakExit label = new BreakExit("goto__" + labels.Count);
      labels.Add(label);
      mapping.Add(aSymbol, label);
    }

    static public List<BreakExit> GetLabels(CiMethod method) {
      List<BreakExit> labels = null;
      methods.TryGetValue(method, out labels);
      return labels;
    }

    static public BreakExit GetLabel(ICiStatement stmt) {
      BreakExit label = null;
      mapping.TryGetValue(stmt, out label);
      return label;
    }

    static public void Reset() {
      mapping.Clear();
      methods.Clear();
    }

    static public BreakExit Push(ICiStatement stmt) {
      BreakExit label = GetLabel(stmt);
      exitPoints.Push(label);
      return label;
    }

    static public void Pop() {
      exitPoints.Pop();
    }

    static public BreakExit Peek() {
      if (exitPoints.Count == 0) {
        return null;
      }
      return exitPoints.Peek();
    }
  }
}