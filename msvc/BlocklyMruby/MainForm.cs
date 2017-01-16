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
		private Ruby _RubyCode;
		private List<string> _BlockIds;

		public MainForm()
		{
			_Mruby = new Mruby();

			InitializeComponent();

			SetRunningMode(RunningMode.None);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			App.Term = (TerminalHost)xTermView1.ObjectForScripting;
			xTermView1.Stdio += Xterm_Stdio;
			_Mruby.Stdio += Mruby_Stdio;
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

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var workspace = blocklyView1.Blockly.getMainWorkspace();
			Bridge.Html5.Element xml = null;
			using (var fs = new StreamReader(openFileDialog1.FileName, Encoding.UTF8)) {
				var code = fs.ReadToEnd();
				xml = blocklyView1.Blockly.Xml.textToDom(code);
			}

			if (xml != null)
				blocklyView1.Blockly.Xml.domToWorkspace(xml, workspace);
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var workspace = blocklyView1.Blockly.getMainWorkspace();
			var xml = blocklyView1.Blockly.Xml.workspaceToDom(workspace);
			var code = xml.OuterHTML;
			using (var fs = new StreamWriter(saveFileDialog1.FileName, false, Encoding.UTF8)) {
				fs.Write(code);
			}
		}

		private void RunBtn_Click(object sender, EventArgs e)
		{
			string rubyfile;
			if (App.changed || _RubyCode == null) {
				rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
				_RubyCode = new Ruby(blocklyView1.Blockly, rubyfile);
				var workspace = blocklyView1.Blockly.getMainWorkspace();
				var code = _RubyCode.workspaceToCode(workspace);

				using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
					fs.Write(code);
				}
			}
			else {
				rubyfile = _RubyCode.filename;
			}

			SetRunningMode(RunningMode.Compile);

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			var run = _Mruby.mrbc(new string[] { "-e", "-o", mrbfile, rubyfile }, (exitCode) => {
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

			var run1 = _Mruby.mruby(new string[] { "-b", mrbfile }, (exitCode1) => {
				BeginInvoke(new MethodInvoker(() => {
					if (exitCode1 != 0) {
						SetRunningMode(RunningMode.None);
						return;
					}

					SetRunningMode(RunningMode.None);
				}));
			});

			if (!run1)
				SetRunningMode(RunningMode.None);
		}

		private void debugBtn_Click(object sender, EventArgs e)
		{
			string rubyfile;
			if (App.changed || _RubyCode == null) {
				rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
				_RubyCode = new Ruby(blocklyView1.Blockly, rubyfile);
				var workspace = blocklyView1.Blockly.getMainWorkspace();
				var code = _RubyCode.workspaceToCode(workspace);

				using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
					fs.Write(code);
				}
			}
			else {
				rubyfile = _RubyCode.filename;
			}

			SetRunningMode(RunningMode.Compile);

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			var run = _Mruby.mrbc(new string[] { "-e", "-g", "-o", mrbfile, rubyfile }, (exitCode) => {
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
			var run1 = _Mruby.mrdb(new string[] { "-b", mrbfile }, (exitCode1) => {
				BeginInvoke(new MethodInvoker(() => {
					if (exitCode1 != 0) {
						SetRunningMode(RunningMode.None);
						return;
					}

					SetRunningMode(RunningMode.None);
				}));
			});

			if (!run1)
				SetRunningMode(RunningMode.None);
		}

		private void ExportRubyBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog() != DialogResult.OK)
				return;

			string code;
			if (App.changed || _RubyCode == null) {
				_RubyCode = new Ruby(blocklyView1.Blockly, saveFileDialog2.FileName);
				var workspace = blocklyView1.Blockly.getMainWorkspace();
				code = _RubyCode.workspaceToCode(workspace);
			}
			else {
				using (var fs = new StreamReader(_RubyCode.filename, new UTF8Encoding(false))) {
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

		public void ConsoleHookInCompiling(object sender, StdioEventArgs e)
		{
			var match = new Regex(@"^(.+)\(([0-9]+),([0-9]+)\): (.+)\r?$", RegexOptions.Multiline).Match(e.Text);
			if (match.Success) {
				_BlockIds = new List<string>();
				string filename = match.Groups[1].Value;
				int lineno = Int32.Parse(match.Groups[2].Value);
				int column = Int32.Parse(match.Groups[3].Value);
				string message = match.Groups[4].Value;
				do {
					_BlockIds.AddRange(_RubyCode.GetBlockId(filename, lineno, column));
					match = match.NextMatch();
				} while (match.Success);
				if (_BlockIds.Count > 0) {
					var workspace = blocklyView1.Blockly.getMainWorkspace();
					var block = workspace.getBlockById(_BlockIds[0]);
					block.setWarningText(message);
				}
			}
		}

		public void ConsoleHookInRunning(object sender, StdioEventArgs e)
		{

		}

		public void ConsoleHookInDebuging(object sender, StdioEventArgs e)
		{
			if (e.Text.StartsWith("(-:0)")) {
				var workspace = blocklyView1.Blockly.getMainWorkspace();
				workspace.highlightBlock(null);
				SetRunningMode(RunningMode.DebugEnd);
				return;
			}
			var match = new Regex(@"^\((.+):([0-9]+)\)", RegexOptions.Multiline).Match(e.Text);
			if (match.Success) {
				_BlockIds = new List<string>();
				string filename = match.Groups[1].Value;
				int lineno = Int32.Parse(match.Groups[2].Value);
				do {
					_BlockIds.AddRange(_RubyCode.GetBlockId(filename, lineno));
					match = match.NextMatch();
				} while (match.Success);
				var workspace = blocklyView1.Blockly.getMainWorkspace();
				workspace.highlightBlock(null);
				foreach (var blockid in _BlockIds) {
					workspace.highlightBlock(blockid, true);
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

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == RubyTabPage) {
				string code;
				if (App.changed) {
					_RubyCode = new Ruby(blocklyView1.Blockly, "editor.rb");
					var workspace = blocklyView1.Blockly.getMainWorkspace();
					code = _RubyCode.workspaceToCode(workspace);
				}
				else {
					using (var fs = new StreamReader(_RubyCode.filename, new UTF8Encoding(false))) {
						code = fs.ReadToEnd();
					}
				}
				aceView1.SetText(code);
			}
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
