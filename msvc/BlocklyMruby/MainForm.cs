using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Bridge;
using BlocklyMruby.Properties;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BlocklyMruby
{
	public partial class MainForm : Form
	{
		private Mruby _Mruby;
		private JsArray<Tuple<string, string>> _BlockIds;

		public MainForm()
		{
			_Mruby = new Mruby();

			InitializeComponent();

			SetRunningMode(RunningMode.None);

			App.MainForm = this;
			Views.ClassSelectorView = classSelectorView1;

			blocklyView1.DocumentLoaded += BlocklyView1_DocumentLoaded;
			classSelectorView1.DocumentLoaded += ClassSelectorView1_DocumentLoaded;
			classSelectorView1.Selected += ClassSelectorView1_Selected;
			classSelectorView1.Removed += ClassSelectorView1_Removed;
			classSelectorView1.MarkClicked += ClassSelectorView1_MarkClicked;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			App.Term = (TerminalHost)xTermView1.ObjectForScripting;
			xTermView1.Stdio += Xterm_Stdio;
			_Mruby.Stdio += Mruby_Stdio;
			tabControl1.SelectedTab = BlockTabPage;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_Mruby.IsRunning) {
				MessageBox.Show("実行中です。停止してください。");
				e.Cancel = true;
				return;
			}

			xTermView1.Stdio -= Xterm_Stdio;
			_Mruby.Stdio -= Mruby_Stdio;
		}

		private void BlocklyView1_DocumentLoaded(object sender, EventArgs e)
		{
			var workspace = new GlobalWorkspace(blocklyView1, "Global");
			Collections.ClassWorkspaces.Add(workspace);
			blocklyView1.ReloadToolbox(workspace);
		}

		private void ClassSelectorView1_DocumentLoaded(object sender, EventArgs e)
		{
			classSelectorView1.SetCollection(Collections.ClassWorkspaces);
		}

		bool _TabSelecting;

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_TabSelecting)
				return;

			var tab = tabControl1.SelectedTab;
			if (tab == RubyTabPage) {
				UpdateCode();
			}
			else if ((tab != null) && (tab.Controls.Count > 0)) {
				var view = tab.Controls[0] as BlocklyView;
				foreach (var item in Collections.ClassWorkspaces) {
					if (view != item.View)
						continue;

					classSelectorView1.SelectClassWorkspace(item);
					break;
				}
			}
		}

		private void ClassSelectorView1_Selected(object sender, EventArgs e)
		{
			var item = classSelectorView1.Current;
			if (item == null) {
				UpdateCode();

				_TabSelecting = true;
				tabControl1.SelectedTab = RubyTabPage;
				_TabSelecting = false;
			}
			else {
				var view = item.View;
				_TabSelecting = true;
				tabControl1.SelectedTab = (TabPage)view.Parent;
				_TabSelecting = false;
			}
		}

		private void ClassSelectorView1_Removed(object sender, ItemRemovedEventArgs e)
		{
			var view = e.Item.View;
			tabControl1.TabPages.Remove((TabPage)view.Parent);
			view.Dispose();
		}

		private void ClassSelectorView1_MarkClicked(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == RubyTabPage) {
				var item = classSelectorView1.Current;
				if (item != null) {
					var view = item.View;
					_TabSelecting = true;
					tabControl1.SelectedTab = (TabPage)view.Parent;
					_TabSelecting = false;
				}
			}
			else {
				UpdateCode();

				_TabSelecting = true;
				tabControl1.SelectedTab = RubyTabPage;
				_TabSelecting = false;
			}
		}

		private void UpdateCode()
		{
			string code = "";
			var item = classSelectorView1.Current;
			if (item != null) {
				var view = item.View;
				if (view.Blockly.Script.changed || item.RubyCode == null) {
					var rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
					var workspace = view.Blockly.getMainWorkspace();
					code = item.ToCode(rubyfile);

					using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
						fs.Write(code);
					}
				}
				else {
					using (var fs = new StreamReader(item.RubyCode.filename, Encoding.UTF8)) {
						code = fs.ReadToEnd();
					}
				}
			}
			aceView1.SetText(code);
		}

		internal BlocklyView NewBlocklyView(string identifier)
		{
			var view = new BlocklyView();
			TabPage Tab = new TabPage(identifier);
			Tab.Controls.Add(view);
			view.Dock = DockStyle.Fill;
			tabControl1.TabPages.Add(Tab);
			return view;
		}

		internal void RemoveEObjectWorkspace(IClassWorkspace item)
		{
			var view = item.View;
			tabControl1.TabPages.Remove((TabPage)view.Parent);
			view.Dispose();
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			var item = classSelectorView1.Current;
			if (item == null)
				return;

			if (openFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var view = item.View;
			var workspace = view.Blockly.getMainWorkspace();
			Bridge.Html5.Element xml = null;
			using (var fs = new StreamReader(openFileDialog1.FileName, Encoding.UTF8)) {
				var code = fs.ReadToEnd();
				xml = view.Blockly.Xml.textToDom(code);
			}

			if (xml != null)
				view.Blockly.Xml.domToWorkspace(xml, workspace);
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			var item = classSelectorView1.Current;
			if (item == null)
				return;

			if (saveFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var view = item.View;
			var workspace = view.Blockly.getMainWorkspace();
			var xml = view.Blockly.Xml.workspaceToDom(workspace);
			var code = xml.OuterHTML;
			using (var fs = new StreamWriter(saveFileDialog1.FileName, false, Encoding.UTF8)) {
				fs.Write(code);
			}
		}

		private static void GetCompileArgs(JsArray<string> rubyfiles, out string mrbfile)
		{
			var i = rubyfiles.Count;
			rubyfiles.Add("placeholder.mrb");

			var list = new JsArray<IClassWorkspace>();
			foreach (var item in Collections.ClassWorkspaces) {
				list.Add(item);
			}

			foreach (var item in list) {
				string rubyfile;
				var view = item.View;
				if (view.Blockly.Script.changed || item.RubyCode == null) {
					rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
					var workspace = view.Blockly.getMainWorkspace();
					var code = item.ToCode(rubyfile);

					using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
						fs.Write(code);
					}
				}
				else {
					rubyfile = item.RubyCode.filename;
				}
				rubyfiles.Push(rubyfile);
			}
			mrbfile = Path.ChangeExtension(rubyfiles[i + 1], "mrb");
			rubyfiles[i] = mrbfile;
		}

		private void RunBtn_Click(object sender, EventArgs e)
		{
			if (Collections.ClassWorkspaces.Length == 0)
				return;

			var rubyfiles = new JsArray<string>() { "-e", "-o" };
			string mrbfile;
			GetCompileArgs(rubyfiles, out mrbfile);

			SetRunningMode(RunningMode.Compile);

			var run = _Mruby.mrbc(rubyfiles.ToArray(), (exitCode) => {
				BeginInvoke(new Action<string, int>(CompileDoneInRunMode), mrbfile, exitCode);
			});

			if (!run)
				SetRunningMode(RunningMode.None);
		}

		void CompileDoneInRunMode(string mrbfile, int exitCode)
		{
			if (exitCode != 0) {
				SetRunningMode(RunningMode.None);
				return;
			}

			SetRunningMode(RunningMode.Run);

			var run = _Mruby.mruby(new string[] { "-b", mrbfile }, (exitCode1) => {
				BeginInvoke(new MethodInvoker(() => {
					if (exitCode1 != 0) {
						SetRunningMode(RunningMode.None);
						return;
					}

					SetRunningMode(RunningMode.None);
				}));
			});

			if (!run)
				SetRunningMode(RunningMode.None);
		}

		private void debugBtn_Click(object sender, EventArgs e)
		{
			if (Collections.ClassWorkspaces.Length == 0)
				return;

			var rubyfiles = new JsArray<string>() { "-e", "-g", "-o" };
			string mrbfile;
			GetCompileArgs(rubyfiles, out mrbfile);

			SetRunningMode(RunningMode.Compile);

			var run = _Mruby.mrbc(rubyfiles.ToArray(), (exitCode) => {
				BeginInvoke(new Action<string, int>(CompileDoneInDebugMode), mrbfile, exitCode);
			});

			if (!run)
				SetRunningMode(RunningMode.None);
		}

		private void CompileDoneInDebugMode(string mrbfile, int exitCode)
		{
			if (exitCode != 0) {
				SetRunningMode(RunningMode.None);
				return;
			}

			SetRunningMode(RunningMode.Debug);
			var run = _Mruby.mrdb(new string[] { "-b", mrbfile }, (exitCode1) => {
				BeginInvoke(new MethodInvoker(() => {
					if (exitCode1 != 0) {
						SetRunningMode(RunningMode.None);
						return;
					}

					SetRunningMode(RunningMode.None);
				}));
			});

			if (!run)
				SetRunningMode(RunningMode.None);
		}

		private void ExportRubyBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog() != DialogResult.OK)
				return;

			var item = classSelectorView1.Current;
			if (item == null)
				return;

			var view = item.View;
			string code;
			if (view.Blockly.Script.changed || item.RubyCode == null) {
				var workspace = view.Blockly.getMainWorkspace();
				code = item.ToCode(saveFileDialog2.FileName);
			}
			else {
				using (var fs = new StreamReader(item.RubyCode.filename, Encoding.UTF8)) {
					code = fs.ReadToEnd();
				}
			}

			using (var fs = new StreamWriter(saveFileDialog2.FileName, false, new UTF8Encoding(false))) {
				fs.Write(code);
			}
		}

		enum RunningMode
		{
			None,
			Compile,
			Run,
			Debug,
			DebugRunning,
			DebugEnd,
		}

		private void SetRunningMode(RunningMode mode)
		{
			RunBtn.Enabled = mode == RunningMode.None;
			DebugBtn.Enabled = mode == RunningMode.None;
			DebugRunBtn.Enabled = mode == RunningMode.Debug;
			StepBtn.Enabled = mode == RunningMode.Debug;
			ContinueBtn.Enabled = mode == RunningMode.Debug;
			BreakBtn.Enabled = mode == RunningMode.DebugRunning;
			QuitBtn.Enabled = mode == RunningMode.DebugEnd;

			switch (mode) {
			case RunningMode.None:
				_ConsoleHook = null;
				break;
			case RunningMode.Compile:
				_ConsoleHook = ConsoleHookInCompiling;
				break;
			case RunningMode.Run:
				_ConsoleHook = ConsoleHookInRunning;
				break;
			case RunningMode.Debug:
			case RunningMode.DebugRunning:
			case RunningMode.DebugEnd:
				_ConsoleHook = ConsoleHookInDebuging;
				break;
			}
		}

		private IClassWorkspace GetWorkspace(string filename)
		{
			foreach (var item in Collections.ClassWorkspaces) {
				if (item.RubyCode != null && item.RubyCode.filename == filename)
					return item;
			}
			return null;
		}

		private IClassWorkspace GetWorkspaceById(string workspace_id)
		{
			foreach (var item in Collections.ClassWorkspaces) {
				var workspace = item.Workspace;
				if (workspace.id == workspace_id)
					return item;
			}
			return null;
		}

		public void ConsoleHookInCompiling(object sender, StdioEventArgs e)
		{
			var match = new Regex(@"^(.+)\(([0-9]+),([0-9]+)\): (.+)\r?$", RegexOptions.Multiline).Match(e.Text);
			if (match.Success) {
				_BlockIds = new JsArray<Tuple<string, string>>();
				string filename = match.Groups[1].Value;
				int lineno = Int32.Parse(match.Groups[2].Value);
				int column = Int32.Parse(match.Groups[3].Value);
				string message = match.Groups[4].Value;

				var item = GetWorkspace(filename);
				if (item != null) {
					var rubyCode = item.RubyCode;
					do {
						_BlockIds.AddRange(rubyCode.GetBlockId(filename, lineno, column));
						match = match.NextMatch();
					} while (match.Success);

					if (_BlockIds.Length > 0) {
						classSelectorView1.SelectClassWorkspace(item);
						var workspace = item.Workspace;
						var block = workspace.getBlockById(_BlockIds[0].Item2);
						block.setWarningText(message);
					}
				}
			}
		}

		public void ConsoleHookInRunning(object sender, StdioEventArgs e)
		{

		}

		public void ConsoleHookInDebuging(object sender, StdioEventArgs e)
		{
			if (e.Text.StartsWith("(-:0)")) {
				var item = classSelectorView1.Current;
				if (item == null)
					return;

				var workspace = (WorkspaceSvg)item.Workspace;
				workspace.highlightBlock(null);
				SetRunningMode(RunningMode.DebugEnd);
				return;
			}
			var match = new Regex(@"^\((.+):([0-9]+)\)", RegexOptions.Multiline).Match(e.Text);
			if (match.Success) {
				_BlockIds = new JsArray<Tuple<string, string>>();
				string filename = match.Groups[1].Value;
				int lineno = Int32.Parse(match.Groups[2].Value);

				var item = GetWorkspace(filename);
				if (item != null) {
					var rubyCode = item.RubyCode;
					do {
						_BlockIds.AddRange(rubyCode.GetBlockId(filename, lineno));
						match = match.NextMatch();
					} while (match.Success);

					if (_BlockIds.Length > 0) {
						classSelectorView1.SelectClassWorkspace(item);
						var workspace = (WorkspaceSvg)item.Workspace;
						workspace.highlightBlock(null);
						foreach (var blockid in _BlockIds) {
							workspace.highlightBlock(blockid.Item2, true);
						}
					}
					else
						item = null;
				}
				if (item == null) {
					string code;
					using (var fs = new StreamReader(filename, Encoding.UTF8)) {
						code = fs.ReadToEnd();
					}
					aceView1.SetText(code);
				}

				SetRunningMode(RunningMode.Debug);
				return;
			}
		}

		private void Xterm_Stdio(object sender, StdioEventArgs e)
		{
			_Mruby.WriteStdin(e.Text);
		}

		System.Threading.AutoResetEvent _Event = new System.Threading.AutoResetEvent(false);
		EventHandler<StdioEventArgs> _ConsoleHook;

		private void Mruby_Stdio(object sender, StdioEventArgs e)
		{
			BeginInvoke(new MethodInvoker(() => {
				xTermView1.OnTermData(e.Text);

				_ConsoleHook?.Invoke(this, e);

				_Event.Set();
			}));
			_Event.WaitOne();
		}

		private void StepBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("s\n");
			SetRunningMode(RunningMode.DebugRunning);
		}

		private void ContinueBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("c\n");
			SetRunningMode(RunningMode.DebugRunning);
		}

		private void DebugRunBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("r\n");
			SetRunningMode(RunningMode.DebugRunning);
		}

		private void BreakBtn_Click(object sender, EventArgs e)
		{
			_Mruby.break_program();
		}

		private void QuitBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("q\n");
		}
	}
}
