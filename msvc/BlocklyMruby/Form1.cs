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
		TerminalHost _TerminalHost;
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

			webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_ScriptHost = new ScriptHost();
			webBrowser1.ObjectForScripting = _ScriptHost;
			webBrowser1.DocumentText = Resources.index_html;
			_TerminalHost = new TerminalHost(_Mruby);
			webBrowser2.ObjectForScripting = _TerminalHost;
			webBrowser2.DocumentText = Resources.xterm_html;//.Replace("/*%%term_css%%*/", Resources.xterm_css);
		}

		private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
		{
			string text = webBrowser1.DocumentTitle;
			if (text != "")
				Text = text;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
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

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddScript(webBrowser1, Resources.blockly_js);
			AddScript(webBrowser1, Script.runtime);
			webBrowser1.Document.InvokeScript("load_blockly");
			Script.SetDocument(webBrowser1.Document, _ScriptHost);
			App.Init();
			webBrowser1.Document.InvokeScript("start_blockly");
			App.Init2();
		}

		private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddStyle(webBrowser2, Resources.xterm_css);
			AddScript(webBrowser2, Resources.xterm_js);
			AddScript(webBrowser2, Resources.fit_js);
			webBrowser2.Document.InvokeScript("start_xterm");
			_Mruby.Stdio += Mruby_Stdio;
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
			var workspace = Blockly.getMainWorkspace();
			var code = Blockly.Ruby.workspaceToCode(workspace);

			var generator = new MrbParser(false);
			generator.mrb_parse_nstring("temporary.rb", MrbParser.UTF8StringToArray(code));
			var scope = generator.tree as scope_node;
			if (scope == null) {
				return;
			}

			var rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
			using (var fs = new StreamWriter(rubyfile, false, Encoding.UTF8)) {
				fs.Write(code);
			}

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			_Mruby.mrbc(new string[] { "-e", "-o", mrbfile, rubyfile }, (ret) => {
				if (ret != 0)
					return;

				_Mruby.mruby(new string[] { "-b", mrbfile }, (ret1) => {
					if (ret1 != 0)
						return;
				});
			});
		}

		private void debugBtn_Click(object sender, EventArgs e)
		{
			var workspace = Blockly.getMainWorkspace();
			var code = Blockly.Ruby.workspaceToCode(workspace);

			var generator = new MrbParser(false);
			generator.mrb_parse_nstring("temporary.rb", MrbParser.UTF8StringToArray(code));
			var scope = generator.tree as scope_node;
			if (scope == null) {
				return;
			}

			var rubyfile = Path.ChangeExtension(Path.GetTempFileName(), "rb");
			using (var fs = new StreamWriter(rubyfile, false, Encoding.UTF8)) {
				fs.Write(code);
			}

			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			_Mruby.mrbc(new string[] { "-e", "-g", "-o", mrbfile, rubyfile }, (ret) => {
				if (ret != 0)
					return;

				_Mruby.mrdb(new string[] { "-b", mrbfile }, (ret1) => {
					if (ret1 != 0)
						return;
				});
			});
		}

		private void ExportRubyBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog() != DialogResult.OK)
				return;

			var workspace = Blockly.getMainWorkspace();
			var code = Blockly.Ruby.workspaceToCode(workspace);

			var generator = new MrbParser(false);
			generator.mrb_parse_nstring(saveFileDialog2.FileName, MrbParser.UTF8StringToArray(code));
			var scope = generator.tree as scope_node;
			if (scope != null) {
				code = generator.to_ruby();
			}

			using (var fs = new StreamWriter(saveFileDialog2.FileName, false, Encoding.UTF8)) {
				fs.Write(code);
			}
		}

		private void Mruby_Stdio(object sender, StdioEventArgs e)
		{
			BeginInvoke(new MethodInvoker(() => {
				webBrowser2.Document.InvokeScript("on_term_data", new object[] { e.Text });
			}));
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class TerminalHost
	{
		Mruby _Mruby;
		public object term;

		public TerminalHost(Mruby mruby)
		{
			_Mruby = mruby;
		}

		public void on_data(object data)
		{
			_Mruby.WriteStdin(((string)data).Replace("\r", "\n"));
		}
	}
}
