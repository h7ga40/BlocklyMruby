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
		ScriptHost _ScriptHost;
		const string FEATURE_BROWSER_EMULATION = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

		public Form1()
		{
			var appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();
			var regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);
			regkey.SetValue(appName + ".exe", 11001, Microsoft.Win32.RegistryValueKind.DWord);
			regkey.SetValue(appName + ".vshost.exe", 11001, Microsoft.Win32.RegistryValueKind.DWord);
			regkey.Close();

			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_ScriptHost = new ScriptHost();
			webBrowser1.DocumentText = Resources.index;
			webBrowser1.ObjectForScripting = _ScriptHost;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			//var appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();
			//var regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);
			//regkey.DeleteValue(appName + ".exe");
			//regkey.DeleteValue(appName + ".vshost.exe");
			//regkey.Close();
		}

		private void AddScript(string scriptSrc)
		{
			var head = webBrowser1.Document.GetElementsByTagName("head")[0];
			var script = webBrowser1.Document.CreateElement("script");
			script.SetAttribute("type", "text/javascript");
			script.InnerHtml = scriptSrc;
			head.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, script);
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			AddScript(Resources.blockly);
			AddScript(Script.runtime);
			webBrowser1.Document.InvokeScript("load_blockly");
			Script.SetDocument(webBrowser1.Document, _ScriptHost);
			App.Init();
			webBrowser1.Document.InvokeScript("start_blockly");
			App.Init2();
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
			if(scope == null) {
				return;
			}

			var rubyfile = Path.GetTempFileName();
			var mrbfile = Path.ChangeExtension(rubyfile, "mrb");
			using (var fs = new StreamWriter(rubyfile, false, Encoding.UTF8)) {
				fs.Write(code);
			}

			var ret = Mruby.mrbc("-o", mrbfile, rubyfile);
			if (ret != 0)
				return;

			ret = Mruby.mruby(mrbfile);
			if (ret != 0)
				return;
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
	}
}
