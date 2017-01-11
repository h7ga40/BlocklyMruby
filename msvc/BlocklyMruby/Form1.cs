using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Bridge;
using BlocklyMruby.Properties;

namespace BlocklyMruby
{
	public partial class Form1 : Form
	{
		Mruby _Mruby;
		ScriptHost _ScriptHost;
		EditorHost _EditorHost;
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
			var generator = new Ruby();
			var workspace = Blockly.getMainWorkspace();
			var code = generator.workspaceToCode(workspace);

			var rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
			using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
				fs.Write(code);
			}

			SetRunning(true);

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			var run = _Mruby.mrbc(new string[] { "-e", "-o", mrbfile, rubyfile }, (ret) => {
				if (ret != 0) {
					BeginInvoke(new Action<bool>(SetRunning), false);
					return;
				}

				var run1 = _Mruby.mruby(new string[] { "-b", mrbfile }, (ret1) => {
					if (ret != 0) {
						BeginInvoke(new Action<bool>(SetRunning), false);
						return;
					}

					BeginInvoke(new Action<bool>(SetRunning), false);
				});

				if (!run1)
					BeginInvoke(new Action<bool>(SetRunning), false);
			});

			if (!run)
				SetRunning(false);
		}

		private void debugBtn_Click(object sender, EventArgs e)
		{
			var generator = new Ruby();
			var workspace = Blockly.getMainWorkspace();
			var code = generator.workspaceToCode(workspace);

			var rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
			using (var fs = new StreamWriter(rubyfile, false, new UTF8Encoding(false))) {
				fs.Write(code);
			}

			SetRunning(true);

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			var run = _Mruby.mrbc(new string[] { "-e", "-g", "-o", mrbfile, rubyfile }, (ret) => {
				if (ret != 0) {
					BeginInvoke(new Action<bool>(SetRunning), false);
					return;
				}

				var run1 = _Mruby.mrdb(new string[] { "-b", mrbfile }, (ret1) => {
					if (ret != 0) {
						BeginInvoke(new Action<bool>(SetRunning), false);
						return;
					}

					BeginInvoke(new Action<bool>(SetRunning), false);
				});

				if (!run1)
					BeginInvoke(new Action<bool>(SetRunning), false);
			});

			if (!run)
				SetRunning(false);
		}

		private void SetRunning(bool running)
		{
			RunBtn.Enabled = !running;
			DebugBtn.Enabled = !running;
		}

		private void ExportRubyBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog() != DialogResult.OK)
				return;

			var generator = new Ruby();
			var workspace = Blockly.getMainWorkspace();
			var code = generator.workspaceToCode(workspace);

			var parser = new MrbParser(false);
			parser.mrb_parse_nstring(saveFileDialog2.FileName, MrbParser.UTF8StringToArray(code));
			var scope = parser.tree as scope_node;
			if (scope != null) {
				code = parser.to_ruby();
			}

			using (var fs = new StreamWriter(saveFileDialog2.FileName, false, new UTF8Encoding(false))) {
				fs.Write(code);
			}
		}

		System.Threading.AutoResetEvent _Event = new System.Threading.AutoResetEvent(false);

		private void Mruby_Stdio(object sender, StdioEventArgs e)
		{
			BeginInvoke(new MethodInvoker(() => {
				ConsoleWb.Document.InvokeScript("on_term_data", new object[] { e.Text });
				_Event.Set();
			}));
			_Event.WaitOne();
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == RubyTabPage) {
				var generator = new Ruby();
				var workspace = Blockly.getMainWorkspace();
				var code = generator.workspaceToCode(workspace);
				RubyEditorWb.Document.InvokeScript("set_text", new object[] { code });
			}
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
