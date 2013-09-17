//
// Copyright (C) 2011-2013  Piotr Fusik
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: AssemblyTitle("CiPad")]
[assembly: AssemblyDescription("Ci Editor")]

namespace Foxoft.Ci
{

	public class CiPad : Form
	{
		string[] SearchDirs = new string[0];
		readonly CiPadGroup CiGroup;
		
		readonly CiPadGroup C89Group;
		readonly CiPadGroup C99Group;
		readonly CiPadGroup CsGroup;
		readonly CiPadGroup JavaGroup;

		readonly CiPadGroup PasGroup;
		readonly CiPadGroup DGroup;
		readonly CiPadGroup Perl1Group;
		readonly CiPadGroup Perl2Group;

		readonly CiPadGroup PHPGroup;
		readonly CiPadGroup Js1Group;
		readonly CiPadGroup Js2Group;
		readonly CiPadGroup AsGroup;

		TextBox Messages;

		void FocusCi()
		{
			TextBox ciBox = (TextBox) this.CiGroup.TabPages.First().Controls[0];
			ciBox.Select(0, 0); // don't want all text initially selected
			ciBox.Select(); // focus
		}

		void Menu_Open(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog { DefaultExt = "ci", Filter = "Æ Source Code (*.ci)|*.ci", Multiselect = true };
			if (dlg.ShowDialog() == DialogResult.OK) {
				// Directory for BinaryResources. Let's assume all sources and resources are in the same directory.
				this.SearchDirs = new string[1] { Path.GetDirectoryName(dlg.FileNames[0]) };

				this.CiGroup.Clear();
				foreach (string filename in dlg.FileNames) {
					string content = File.ReadAllText(filename).Replace("\r", "").Replace("\n", "\r\n");
					this.CiGroup.Set(Path.GetFileName(filename), content, false);
				}
				FocusCi();
			}
		}

		void Menu_Font(object sender, EventArgs e)
		{
			FontDialog dlg = new FontDialog { Font = this.Font, ShowEffects = false };
			if (dlg.ShowDialog() == DialogResult.OK)
				this.Font = dlg.Font;
		}

		void InitializeComponent()
		{
			this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			this.ClientSize = new Size(760, 500);
			this.Text = "CiPad";
			this.Messages = new TextBox();
			this.Messages.Multiline = true;
			this.Messages.ReadOnly = true;
			this.Messages.ScrollBars = ScrollBars.Both;
			this.Messages.WordWrap = false;
			this.Controls.Add(this.Messages);
			this.Menu = new MainMenu(new MenuItem[] {
			                         	new MenuItem("&Open", Menu_Open),
			                         	new MenuItem("&Font", Menu_Font)
			                         });
		}

		public CiPad()
		{
			this.SuspendLayout();
			InitializeComponent();
			this.CiGroup = new CiPadGroup(this);
			this.C89Group = new CiPadGroup(this);
			this.C99Group = new CiPadGroup(this);
			this.CsGroup = new CiPadGroup(this);
			this.JavaGroup = new CiPadGroup(this);
			this.PasGroup = new CiPadGroup(this);
			this.DGroup = new CiPadGroup(this);
			this.Perl1Group = new CiPadGroup(this);
			this.Perl2Group = new CiPadGroup(this);
			this.PHPGroup = new CiPadGroup(this);
			this.Js1Group = new CiPadGroup(this);
			this.Js2Group = new CiPadGroup(this);
			this.AsGroup = new CiPadGroup(this);
			this.SearchDirs = new string[1] { "."};
			string content;
			content = @"public class HelloCi {

	public const int VersionMajor = 0;
	public const int VersionMinor = 3;
	public const string Version = VersionMajor + ""."" + VersionMinor;

	/// Returns `true` if and only if `x` is a power of 2 (1, 2, 4, 8, 16, ...).
	public static bool IsPowerOfTwo(int x) {
		return (x & x - 1) == 0 && x > 0;
	}

}
";
			this.CiGroup.Set("hello.ci", content.Replace("\r", "").Replace("\n", "\r\n"), false);
			Translate();
			FocusCi();
			this.ResumeLayout();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			int cx = ClientRectangle.Width;
			int cy = ClientRectangle.Height;
			int cw = cx / 5;
			int ch = cy / 3;
			this.CiGroup.SetBounds (0, 0, cw, ch * 2);
			this.Messages.SetBounds(0, ch * 2, cw, ch);
			
			
			this.C89Group.SetBounds  (cw * 1, 0, cw, ch);
			this.C99Group.SetBounds  (cw * 2, 0, cw, ch);
			this.CsGroup.SetBounds   (cw * 3, 0, cw, ch);
			this.JavaGroup.SetBounds (cw * 4, 0, cw, ch);

			this.PasGroup.SetBounds  (cw * 1, ch * 1, cw, ch);
			this.DGroup.SetBounds    (cw * 2, ch * 1, cw, ch);
			this.Perl1Group.SetBounds(cw * 3, ch * 1, cw, ch);
			this.Perl2Group.SetBounds(cw * 4, ch * 1, cw, ch);

			this.PHPGroup.SetBounds  (cw * 1, ch * 2, cw, ch);
			this.Js1Group.SetBounds  (cw * 2, ch * 2, cw, ch);
			this.Js2Group.SetBounds  (cw * 3, ch * 2, cw, ch);
			this.AsGroup.SetBounds   (cw * 4, ch * 2, cw, ch);
		}

		void Translate()
		{
			try {
				CiParser parser = new CiParser();
				foreach (TabPage page in this.CiGroup.TabPages) {
					parser.Parse(page.Text, new StringReader(page.Controls[0].Text));
				}
				CiProgram program = parser.Program;
				CiResolver resolver = new CiResolver();
				resolver.SearchDirs = this.SearchDirs;
				resolver.Resolve(program);
				this.C89Group.Load (program, new GenC89 { OutputFile = "hello.c" });
				this.C99Group.Load (program, new GenC   { OutputFile = "hello99.c" });
				this.CsGroup.Load  (program, new GenCs(null) { OutputFile = "hello.cs" });
				this.JavaGroup.Load(program, new GenJava(null) { OutputFile = "." });

				this.PasGroup.Load  (program, new GenPas("Hello") {OutputFile = "hello.pas"});
				this.DGroup.Load    (program, new GenD { OutputFile = "hello.d" });
				this.Perl1Group.Load(program, new GenPerl58(null) { OutputFile = "hello.pm" });
				this.Perl2Group.Load(program, new GenPerl510(null) { OutputFile = "hello-5.10.pm" });

				this.PHPGroup.Load(program, new GenPHP("Hello") {OutputFile = "hello.php"});
				this.Js1Group.Load(program, new GenJs() { OutputFile = "hello.js" });
				this.Js2Group.Load(program, new GenJsWithTypedArrays() { OutputFile = "hello-Typed-Arrays.js" });
				this.AsGroup.Load (program, new GenAs(null) { OutputFile = "." });

				this.Messages.BackColor = SystemColors.Window;
				this.Messages.Text = "OK";
			}
			catch (Exception ex)
			{
				this.Messages.BackColor = Color.LightCoral;
				this.Messages.Text = ex.Message;
			}
		}

		internal void CiText_TextChanged(object sender, EventArgs e)
		{
			Translate();
			// When editing class name, TabControls for Java and AS get new TabPages and receive focus.
			// Restore focus, so we can continue typing.
			this.ActiveControl = (Control) sender;
		}

		[STAThread] // without it ShowDialog() hangs
		public static void Main(string[] args)
		{
			Application.Run(new CiPad());
		}
	}

	class CiPadWriter : StringWriter
	{
		CiPadGroup Parent;
		string Name;
		public CiPadWriter(CiPadGroup parent, string name)
		{
			this.Parent = parent;
			this.Name = name;
		}
		public override void Close()
		{
			base.Close();
			this.Parent.Set(this.Name, base.ToString(), true);
		}
	}

	class CiPadGroup
	{
		readonly CiPad Form;
		readonly TabControl TabControl = new TabControl();
		HashSet<TabPage> TabsToRemove;

		public CiPadGroup(CiPad form)
		{
			this.Form = form;
			form.Controls.Add(this.TabControl);
		}

		public void SetBounds(int x, int y, int w, int h)
		{
			this.TabControl.SetBounds(x, y, w, h);
		}

		public IEnumerable<TabPage> TabPages
		{
			get
			{
				return this.TabControl.TabPages.Cast<TabPage>();
			}
		}

		public void Clear()
		{
			this.TabControl.SelectedIndex = -1; // WORKAROUND Mono 2.10.9 BUG: ArgumentOutOfRangeException from TabPages.Clear()
			this.TabControl.TabPages.Clear();
		}

		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr wnd, uint msg, IntPtr wParam, int[] lParam);

		static void SetNarrowTabs(TextBox text)
		{
			const int EM_SETTABSTOPS = 0xcb;
			SendMessage(text.Handle, EM_SETTABSTOPS, new IntPtr(1), new int[] { 12 });
		}

		public void Set(string name, string content, bool readOnly)
		{
			TabPage page = this.TabControl.TabPages[name];
			if (page == null) {
				page = new TabPage();
				page.Name = name;
				page.Text = name;
				TextBox text = new TextBox();
				if (!readOnly) {
					text.AcceptsReturn = true;
					text.AcceptsTab = true;
					text.TextChanged += this.Form.CiText_TextChanged;
				}
				text.Dock = DockStyle.Fill;
				text.Multiline = true;
				text.MaxLength = 1000000;
				text.ReadOnly = readOnly;
				text.ScrollBars = ScrollBars.Both;
				text.TabStop = false;
				text.WordWrap = false;
				if (Type.GetType("Mono.Runtime") == null)
					SetNarrowTabs(text);
				page.Controls.Add(text);
				this.TabControl.TabPages.Add(page);
			}
			else if (this.TabsToRemove != null)
				this.TabsToRemove.Remove(page);
			page.Controls[0].Text = content;
		}

		TextWriter CreatePadWriter(string filename)
		{
			return new CiPadWriter(this, filename);
		}

		public void Load(CiProgram program, params SourceGenerator[] gens)
		{
			this.TabsToRemove = new HashSet<TabPage>(this.TabPages);
			foreach (SourceGenerator gen in gens) {
				gen.Write(program);
			}
			foreach (SourceGenerator gen in gens) {
				gen.CreateTextWriter = this.CreatePadWriter;
				gen.Write(program);
			}
			foreach (TabPage page in this.TabsToRemove) {
				if (page == this.TabControl.SelectedTab)
					this.TabControl.SelectedIndex = -1; // WORKAROUND Mono 2.10.9 BUG: Java/AS translations disappear when changing class name
				this.TabControl.TabPages.Remove(page);
			}
			if (this.TabControl.SelectedIndex == -1)
				this.TabControl.SelectedIndex = 0; // WORKAROUND continued
		}
	}

}
