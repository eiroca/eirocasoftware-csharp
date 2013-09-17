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

namespace Foxoft.Ci {

	public class GenPHP : SourceGenerator, ICiSymbolVisitor {
		
		string Namespace;

		public GenPHP(string namespace_) {
			this.Namespace = namespace_;
		}

		void WriteDoc(string text) {
			foreach (char c in text) {
				switch (c) {
						case '&': Write("&amp;"); break;
						case '<': Write("&lt;"); break;
						case '>': Write("&gt;"); break;
						case '\n': break;
						default: Write(c); break;
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
							case "true": Write("<see langword=\"true\" />"); break;
							case "false": Write("<see langword=\"false\" />"); break;
							case "null": Write("<see langword=\"null\" />"); break;
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
				WriteLine("<list type=\"bullet\">");
				foreach (CiDocPara item in list.Items) {
					Write("<item>");
					Write(item);
					WriteLine("</item>");
				}
				Write("</list>");
				WriteLine();
				return;
			}
			Write((CiDocPara) block);
		}

		protected override void Write(CiCodeDoc doc) {
			if (doc == null)
				return;
			Write("/**");
			Write("<summary>");
			Write(doc.Summary);
			Write("</summary>");
			WriteLine();
			if (doc.Details.Length > 0) {
				Write("<remarks>");
				foreach (CiDocBlock block in doc.Details)
					Write(block);
				WriteLine("</remarks>");
			}
			WriteLine("*/");
		}

		void Write(CiVisibility visibility) {
			switch (visibility) {
				case CiVisibility.Dead:
				case CiVisibility.Private:
					break;
				case CiVisibility.Internal:
					break;
				case CiVisibility.Public:
					Write("public ");
					break;
			}
		}

		void WriteBaseType(CiType type) {
			if (type is CiStringType)
				Write("string");
			else
				Write(type.Name);
		}

		void Write(CiType type) {
			WriteBaseType(type.BaseType);
			for (int i = 0; i < type.ArrayLevel; i++)
				Write("[]");
			Write(' ');
		}

		protected override CiPriority GetPriority(CiExpr expr) {
			if (expr is CiPropertyAccess) {
				CiProperty prop = ((CiPropertyAccess) expr).Property;
				if (prop == CiLibrary.SByteProperty || prop == CiLibrary.LowByteProperty)
					return CiPriority.Prefix;
			}
			else if (expr is CiCoercion) {
				CiCoercion c = (CiCoercion) expr;
				if (c.ResultType == CiByteType.Value && c.Inner.Type == CiIntType.Value)
					return CiPriority.Prefix;
			}
			return base.GetPriority(expr);
		}

		protected override void Write(CiPropertyAccess expr) {
			if (expr.Property == CiLibrary.SByteProperty) {
				Write("");
				WriteChild(expr, expr.Obj);
			}
			else if (expr.Property == CiLibrary.LowByteProperty) {
				Write("");
				WriteChild(expr, expr.Obj);
			}
			else if (expr.Property == CiLibrary.StringLengthProperty) {
				Write("strlen(");
				WriteChild(expr, expr.Obj);
				Write(")");
			}
			else {
				throw new ArgumentException(expr.Property.Name);
			}
		}

		protected override void Write(CiMethodCall expr) {
			if (expr.Method == CiLibrary.MulDivMethod) {
				WriteChild(CiPriority.Prefix, expr.Obj);
				Write(" * ");
				WriteChild(CiPriority.Multiplicative, expr.Arguments[0]);
				Write(" / ");
				WriteNonAssocChild(CiPriority.Multiplicative, expr.Arguments[1]);
			}
			else if (expr.Method == CiLibrary.CharAtMethod) {
				Write("ord(");
				Write(expr.Obj);
				Write("[");
				Write(expr.Arguments[0]);
				Write("])");
			}
			else if (expr.Method == CiLibrary.SubstringMethod) {
				Write("substr(");
				Write(expr.Obj);
				Write(", ");
				Write(expr.Arguments[0]);
				Write(", ");
				Write(expr.Arguments[1]);
				Write(')');
			}
			else if (expr.Method == CiLibrary.ArrayCopyToMethod) {
				Write("Ci::Copy(");
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
				Write("Ci::GetString(");
				Write(expr.Obj);
				Write(", ");
				Write(expr.Arguments[0]);
				Write(", ");
				Write(expr.Arguments[1]);
				Write(')');
			}
			else if (expr.Method == CiLibrary.ArrayStorageClearMethod) {
				Write("Ci::Clear(");
				Write(expr.Obj);
				Write(", 0, ");
				Write(((CiArrayStorageType) expr.Obj.Type).Length);
				Write(')');
			}
			else {
				if (expr.Method != null) {
					if (expr.Obj != null) {
						Write(expr.Obj);
					}
					else {
						Write(expr.Method.Class.Name);
					}
					if (expr.Method.CallType == CiCallType.Static) {
						Write("::");
					}
					else {
						Write("->");
					}
					WriteName(expr.Method);
				}
				else {
					WriteDelegateCall(expr.Obj);
				}
				WriteArguments(expr);
				
			}
		}

		void WriteCondChild(CiCondExpr condExpr, CiExpr expr) {
			if (condExpr.ResultType == CiByteType.Value && expr is CiConstExpr) {
				Write("");
			}
			WriteChild(condExpr, expr);
		}

		protected override void Write(CiCondExpr expr) {
			WriteNonAssocChild(expr, expr.Cond);
			Write(" ? ");
			WriteCondChild(expr, expr.OnTrue);
			Write(" : ");
			WriteCondChild(expr, expr.OnFalse);
		}

		protected override void WriteNew(CiType type) {
			CiClassStorageType classType = type as CiClassStorageType;
			if (classType != null) {
				Write("new ");
				Write(classType.Class.Name);
				Write("()");
			}
			else {
				// CiArrayStorageType arrayType = (CiArrayStorageType) type;
				Write("array(");
				Write(")");
			}
		}

		protected override void Write(CiCoercion expr) {
			if (expr.ResultType == CiByteType.Value && expr.Inner.Type == CiIntType.Value) {
				Write("");
				WriteChild(expr, (CiExpr) expr.Inner); // TODO: Assign
			}
			else {
				base.Write(expr);
			}
		}

		protected override void WriteFallthrough(CiExpr expr) {
			Write("//$FALL-THROUGH$ go to ");
			if (expr!=null) {
				Write(expr);
			}
			else {
				Write("default");
			}
			WriteLine(":");
		}

		public override void Visit(CiThrow stmt) {
			Write("throw new Exception(");
			Write(stmt.Message);
			WriteLine(");");
		}

		void ICiSymbolVisitor.Visit(CiDelegate del) {
			Write(del.Documentation);
			Write("// delegate ");
			WriteSignature(del);
			WriteLine(";");
		}

		public override void Write(CiProgram prog) {
			CreateFile(this.OutputFile);
			if (this.Namespace != null) {
				Write("namespace ");
				Write(this.Namespace);
				WriteLine(";");
			}
			foreach (CiSymbol symbol in prog.Globals) {
				symbol.Accept(this);
			}
			WriteLine(@"
class Ci {
  static function Clear(&$arr, $v, $len) {for($i=0;$i<$len;$i++) {$arr[$i]=$v;}}
  static function Copy (&$src, $src_strt, &$dest, $dst_strt, $dst_len) {for($i=0;$i<$dst_len;$i++) {$dst[$dst_strt+$i]=$src[$src_strt+$i];}}
}
");
			WriteLine("?>");
			CloseFile();
		}
		
		protected override void WriteBanner() {
			WriteLine("<?php");
			WriteLine("// Generated automatically with \"cito\". Do not edit.");
		}

		protected override void OpenClass(bool isAbstract, CiClass klass, string extendsClause) {
			if (isAbstract) {
				Write("abstract ");
			}
			Write("class ");
			Write(klass.Name);
			if (klass.BaseClass != null) {
				Write(extendsClause);
				Write(klass.BaseClass.Name);
			}
			Write(" ");
			OpenBlock();
		}

		protected virtual void WriteCode(ICiStatement[] block) {
			foreach (ICiStatement stmt in block) {
				Write(stmt);
			}
		}
		
		void ICiSymbolVisitor.Visit(CiClass klass) {
			WriteLine();
			Write(klass.Documentation);
			OpenClass(klass.IsAbstract, klass, " extends ");
			foreach (CiConst konst in klass.ConstArrays) {
				Write("var $");
				Write(konst.Name);
				Write(" = ");
				WriteConst(konst.Value);
				WriteLine(";");
			}
			foreach (CiBinaryResource resource in klass.BinaryResources) {
				Write("var $");
				WriteName(resource);
				Write(" = ");
				WriteConst(resource.Content);
				WriteLine(";");
			}
			foreach (CiSymbol member in klass.Members) {
				if (member is CiField) {
					member.Accept(this);
				}
			}
			Write("function __constructor()");
			OpenBlock();
			foreach (CiSymbol member in klass.Members) {
				if ((member is CiField)) {
					CiField f = (CiField)member;
					if ((f.Type is CiClassStorageType) || (f.Type is CiArrayStorageType)) {
						Write("$this->");
						Write(f.Name);
						WriteInit(f.Type);
						WriteLine(";");
					}
				}
			}
			if (klass.Constructor != null) {
				if (klass.Constructor.Body is CiBlock) {
					WriteCode(((CiBlock)klass.Constructor.Body).Statements);
				}
				else  {
					Write(klass.Constructor.Body);
				}
			}
			CloseBlock();
			foreach (CiSymbol member in klass.Members) {
				if (!(member is CiField)) {
					member.Accept(this);
				}
			}
			CloseBlock();
		}

		public void WriteVarName(CiType Type, string Name) {
			Write("$");
			Write(Name);
		}
		
		void WriteSignature(CiDelegate del) {
			Write("function ");
			Write(del.Name);
			Write('(');
			bool first = true;
			foreach (CiParam param in del.Params) {
				if (first) {
					first = false;
				}
				else {
					Write(", ");
				}
				if ((param.Type is CiClassType) || (param.Type is CiArrayType)) {
					Write("&");
				}
				WriteVarName(param.Type, param.Name);
			}
			Write(')');
		}

		void ICiSymbolVisitor.Visit(CiMethod method) {
			WriteLine();
			Write(method.Documentation);
			foreach (CiParam param in method.Signature.Params) {
				if (param.Documentation != null) {
					Write("/** <param name=\"");
					Write(param.Name);
					Write("\">");
					Write(param.Documentation.Summary);
					WriteLine("</param> */");
				}
			}
			Write(method.Visibility);
			switch (method.CallType) {
					case CiCallType.Static: Write("static "); break;
					case CiCallType.Normal: break;
					case CiCallType.Abstract: Write("abstract "); break;
					case CiCallType.Virtual: Write(""); break;
					case CiCallType.Override: Write(""); break;
			}
			WriteSignature(method.Signature);
			if (method.CallType == CiCallType.Abstract) {
				WriteLine(";");
			}
			else {
				WriteLine();
				Write(method.Body);
			}
		}

		public override void Visit(CiVar stmt) {
			bool sep = false;
			if ((stmt.Type is CiClassStorageType) || (stmt.Type is CiArrayStorageType)){
				WriteVarName(stmt.Type, stmt.Name);
				sep = true;
				WriteInit(stmt.Type);
			}
			if (stmt.InitialValue!=null) {
				if (sep==true) {
					WriteLine(";");
				}
				WriteVarName(stmt.Type, stmt.Name);
				Write(" = ");
				Write(stmt.InitialValue);
			}
		}

		protected override void Write(CiVarAccess expr) {
			Write("$");
			Write(expr.Var.Name);
		}

		void ICiSymbolVisitor.Visit(CiConst konst) {
			Write(konst.Documentation);
			Write("const ");
			Write(konst.Name);
			Write(" = ");
			WriteConst(konst.Value);
			WriteLine(";");
		}

		void ICiSymbolVisitor.Visit(CiEnum enu) {
			WriteLine();
			Write(enu.Documentation);
			int i = 0;
			foreach (CiEnumValue value in enu.Values) {
				Write(value.Documentation);
				Write("define('");
				Write(enu.Name);
				Write("_");
				Write(value.Name);
				Write("', ");
				Write(i);
				WriteLine(");");
				i++;
			}
		}

		protected override void Write(CiFieldAccess expr) {
			WriteChild(expr, expr.Obj);
			Write("->");
			Write(expr.Field.Name);
		}

		void ICiSymbolVisitor.Visit(CiField field) {
			Write(field.Documentation);
			Write("var $");
			Write(field.Name);
			WriteLine(";");
		}

		bool WriteInit(CiType type) {
			if (type is CiClassStorageType || type is CiArrayStorageType) {
				Write(" = ");
				WriteNew(type);
				return true;
			}
			return false;
		}

		protected override void WriteName(CiConst konst) {
			Write("$this->");
			Write(konst.Name);
		}


		protected override void WriteConst(object value) {
			if (value is bool) {
				Write((bool) value ? "true" : "false");
			}
			else if (value is byte) {
				Write((byte) value);
			}
			else if (value is int) {
				Write((int) value);
			}
			else if (value is string) {
				Write('"');
				foreach (char c in (string) value) {
					switch (c) {
							case '\t': Write("\\t"); break;
							case '\r': Write("\\r"); break;
							case '\n': Write("\\n"); break;
							case '\\': Write("\\\\"); break;
							case '\"': Write("\\\""); break;
							default: Write(c); break;
					}
				}
				Write('"');
			}
			else if (value is CiEnumValue) {
				CiEnumValue ev = (CiEnumValue) value;
				Write(ev.Type.Name);
				Write("_");
				Write(ev.Name);
			}
			else if (value is Array) {
				Write("array( ");
				WriteContent((Array) value);
				Write(" )");
			}
			else if (value == null)
				Write("null");
			else
				throw new ArgumentException(value.ToString());
		}

	}

}
