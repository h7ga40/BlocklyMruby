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
	public partial class Form1 : Form
	{
		Mruby _Mruby;
		ScriptHost _ScriptHost;
		EditorHost _EditorHost;
		Ruby _RubyCode;
		const string FEATURE_BROWSER_EMULATION = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

		public Form1()
		{
			_Mruby = new Mruby();

			var appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();
			var regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);
			regkey.SetValue(appName + ".exe", 11001, Microsoft.Win32.RegistryValueKind.DWord);
			regkey.SetValue(appName + ".vshost.exe", 11001, Microsoft.Win32.RegistryValueKind.DWord);
			regkey.Close();

			InitializeComponent();

			SetRunningMode(RunningMode.None);

			BlocklyWb.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_ScriptHost = new ScriptHost();
			BlocklyWb.ObjectForScripting = _ScriptHost;
			BlocklyWb.DocumentText = Resources.index_html;
			App.Term = new TerminalHost(_Mruby);
			ConsoleWb.ObjectForScripting = App.Term;
			ConsoleWb.DocumentText = Resources.xterm_html;
			_EditorHost = new EditorHost(_Mruby);
			RubyEditorWb.ObjectForScripting = _EditorHost;
			RubyEditorWb.DocumentText = Resources.ace_html;
		}

		private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
		{
			string text = BlocklyWb.DocumentTitle;
			if (text != "")
				Text = text;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_Mruby.IsRunning) {
				MessageBox.Show("実行中です。停止してください。");
				e.Cancel = true;
				return;
			}

			_Mruby.Stdio -= Mruby_Stdio;
			//var appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();
			//var regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);
			//regkey.DeleteValue(appName + ".exe");
			//regkey.DeleteValue(appName + ".vshost.exe");
			//regkey.Close();
		}

		private void AddScript(WebBrowser wb, string scriptSrc)
		{
			var head = wb.Document.GetElementsByTagName("head")[0];
			var script = wb.Document.CreateElement("script");
			script.SetAttribute("type", "text/javascript");
			script.InnerHtml = scriptSrc;
			head.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, script);
		}

		private void AddStyle(WebBrowser wb, string styleSrc)
		{
			var head = wb.Document.GetElementsByTagName("head")[0];
			var style = wb.Document.CreateElement("style");
			style.InnerHtml = styleSrc;
			head.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, style);
		}

		private void BlocklyPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddScript(BlocklyWb, Resources.blockly_js);
			AddScript(BlocklyWb, Script.runtime);
			BlocklyWb.Document.InvokeScript("load_blockly");
			Script.SetDocument(BlocklyWb.Document, _ScriptHost);
			App.Init();
			BlocklyWb.Document.InvokeScript("start_blockly");
			App.Init2();
		}

		private void ConsolePage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddStyle(ConsoleWb, Resources.xterm_css);
			AddScript(ConsoleWb, Resources.xterm_js);
			AddScript(ConsoleWb, Resources.fit_js);
			ConsoleWb.Document.InvokeScript("start_xterm");
			_Mruby.Stdio += Mruby_Stdio;
		}

		private void RubyEditorPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddScript(RubyEditorWb, Resources.ace_js);
			AddScript(RubyEditorWb, Resources.theme_twilight_js);
			AddScript(RubyEditorWb, Resources.mode_ruby_js);
			RubyEditorWb.Document.InvokeScript("start_ace");
			RubyEditorWb.Document.InvokeScript("focus_editor");
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var workspace = Blockly.getMainWorkspace();
			Bridge.Html5.Element xml = null;
			using (var fs = new StreamReader(openFileDialog1.FileName, Encoding.UTF8)) {
				var code = fs.ReadToEnd();
				xml = Blockly.Xml.textToDom(code);
			}

			if (xml != null)
				Blockly.Xml.domToWorkspace(xml, workspace);
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() != DialogResult.OK)
				return;

			var workspace = Blockly.getMainWorkspace();
			var xml = Blockly.Xml.workspaceToDom(workspace);
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
				_RubyCode = new Ruby(rubyfile);
				var workspace = Blockly.getMainWorkspace();
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
				_RubyCode = new Ruby(rubyfile);
				var workspace = Blockly.getMainWorkspace();
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
				_RubyCode = new Ruby(saveFileDialog2.FileName);
				var workspace = Blockly.getMainWorkspace();
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
			DebugEnd,
		}

		private void SetRunningMode(RunningMode mode)
		{
			RunBtn.Enabled = mode == RunningMode.None;
			DebugBtn.Enabled = mode == RunningMode.None;
			DebugRunBtn.Enabled = mode == RunningMode.Debug;
			StepBtn.Enabled = mode == RunningMode.Debug;
			ContinueBtn.Enabled = mode == RunningMode.Debug;
			BreakBtn.Enabled = mode == RunningMode.Debug;
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
			case RunningMode.DebugEnd:
				_ConsoleHook = ConsoleHookInDebuging;
				break;
			}
		}

		public void ConsoleHookInCompiling(object sender, StdioEventArgs e)
		{

		}

		public void ConsoleHookInRunning(object sender, StdioEventArgs e)
		{

		}

		public void ConsoleHookInDebuging(object sender, StdioEventArgs e)
		{
			_BlockIds = new List<string>();
			if (e.Text.StartsWith("(-:0)")) {
				var workspace = Blockly.getMainWorkspace();
				workspace.highlightBlock(null);
				SetRunningMode(RunningMode.DebugEnd);
				return;
			}
			var match = new Regex(@"^\((.+):([0-9]+)\)", RegexOptions.Multiline).Match(e.Text);
			if (match.Success) {
				string filename = match.Groups[1].Value;
				int lineno = Script.ParseInt(match.Groups[2].Value, 10);
				do {
					_BlockIds.AddRange(_RubyCode.GetBlockId(filename, lineno));
					match = match.NextMatch();
				} while (match.Success);
				var workspace = Blockly.getMainWorkspace();
				workspace.highlightBlock(null);
				foreach (var blockid in _BlockIds) {
					workspace.highlightBlock(blockid, true);
				}
				return;
			}
		}

		System.Threading.AutoResetEvent _Event = new System.Threading.AutoResetEvent(false);
		private List<string> _BlockIds;
		EventHandler<StdioEventArgs> _ConsoleHook;

		private void Mruby_Stdio(object sender, StdioEventArgs e)
		{
			BeginInvoke(new MethodInvoker(() => {
				ConsoleWb.Document.InvokeScript("on_term_data", new object[] { e.Text });

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
					_RubyCode = new Ruby("editor.rb");
					var workspace = Blockly.getMainWorkspace();
					code = _RubyCode.workspaceToCode(workspace);
				}
				else {
					using (var fs = new StreamReader(_RubyCode.filename, new UTF8Encoding(false))) {
						code = fs.ReadToEnd();
					}
				}
				RubyEditorWb.Document.InvokeScript("set_text", new object[] { code });
			}
		}

		private void StepBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("s\n");
		}

		private void ContinueBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("c\n");
		}

		private void DebugRunBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("r\n");
		}

		private void BreakBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("b\n");
		}

		private void QuitBtn_Click(object sender, EventArgs e)
		{
			_Mruby.WriteStdin("q\n");
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class EditorHost
	{
		Mruby _Mruby;
		public dynamic editor;

		public EditorHost(Mruby mruby)
		{
			_Mruby = mruby;
		}

		public void on_change()
		{

		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class TerminalHost
	{
		Mruby _Mruby;
		public object term;
		StringBuilder log = new StringBuilder();

		public TerminalHost(Mruby mruby)
		{
			_Mruby = mruby;
		}

		public void on_data(object data)
		{
			_Mruby.WriteStdin(((string)data).Replace("\r", "\n"));
		}

		internal void flush()
		{
			System.Console.Write(log.ToString());
			if (log.Length != 0)
				log.Clear();
		}

		internal void write(string text)
		{
			log.Append(text);
		}
	}
}
