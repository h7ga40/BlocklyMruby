using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
			//AddScript(Resources.blocks_compressed);
			AddScript(Resources.ja);
			Script.SetBlocks(new ColourPickerBlock());
			Script.SetBlocks(new ColourRandomBlock());
			Script.SetBlocks(new ColourRGBBlock());
			Script.SetBlocks(new ColourBlendBlock());
			Script.SetBlocks(new ListsCreateEmptyBlock());
			Script.SetBlocks(new ListsCreateWithBlock());
			Script.SetBlocks(new ListsCreateWithContainerBlock());
			Script.SetBlocks(new ListsCreateWithItemBlock());
			Script.SetBlocks(new ListsRepeatBlock());
			Script.SetBlocks(new ListsLengthBlock());
			Script.SetBlocks(new ListsIsEmptyBlock());
			Script.SetBlocks(new ListsIndexOfBlock());
			Script.SetBlocks(new ListsGetIndexBlock());
			Script.SetBlocks(new ListsSetIndexBlock());
			Script.SetBlocks(new ListsGetSublistBlock());
			Script.SetBlocks(new ListsSortBlock());
			Script.SetBlocks(new ListsSplitBlock());
			Script.SetBlocks(new ControlsIfBlock());
			Script.SetBlocks(new ControlsIfIfBlock());
			Script.SetBlocks(new ControlsIfElseIfBlock());
			Script.SetBlocks(new ControlsIfElseBlock());
			Script.SetBlocks(new SwitchCaseNumberBlock());
			Script.SetBlocks(new SwitchCaseNumberContainerBlock());
			Script.SetBlocks(new SwitchCaseNumberConstBlock());
			Script.SetBlocks(new SwitchCaseNumberRangeBlock());
			Script.SetBlocks(new SwitchCaseNumberDefaultBlock());
			Script.SetBlocks(new SwitchCaseTextBlock());
			Script.SetBlocks(new SwitchCaseTextContainerBlock());
			Script.SetBlocks(new SwitchCaseTextConstBlock());
			Script.SetBlocks(new SwitchCaseTextRangeBlock());
			Script.SetBlocks(new SwitchCaseTextDefaultBlock());
			Script.SetBlocks(new LogicCompareBlock());
			Script.SetBlocks(new LogicOperationBlock());
			Script.SetBlocks(new LogicNegateBlock());
			Script.SetBlocks(new LogicBooleanBlock());
			Script.SetBlocks(new LogicNullBlock());
			Script.SetBlocks(new LogicTernaryBlock());
			Script.SetBlocks(new ControlsRepeatExtBlock());
			Script.SetBlocks(new ControlsRepeatBlock());
			Script.SetBlocks(new ControlsWhileUntilBlock());
			Script.SetBlocks(new ControlsForBlock());
			Script.SetBlocks(new ControlsForEachBlock());
			Script.SetBlocks(new ControlsFlowStatementsBlock());
			Script.SetBlocks(new MathNumberBlock());
			Script.SetBlocks(new MathArithmeticBlock());
			Script.SetBlocks(new MathSingleBlock());
			Script.SetBlocks(new MathTrigBlock());
			Script.SetBlocks(new MathConstantBlock());
			Script.SetBlocks(new MathNumberPropertyBlock());
			Script.SetBlocks(new MathChangeBlock());
			Script.SetBlocks(new MathRoundBlock());
			Script.SetBlocks(new MathOnListBlock());
			Script.SetBlocks(new MathModuloBlock());
			Script.SetBlocks(new MathConstrainBlock());
			Script.SetBlocks(new MathRandomIntBlock());
			Script.SetBlocks(new MathRandomFloatBlock());
			Script.SetBlocks(new ProceduresDefnoreturnBlock());
			Script.SetBlocks(new ProceduresDefreturnBlock());
			Script.SetBlocks(new ProceduresMutatorcontainerBlock());
			Script.SetBlocks(new ProceduresMutatorargBlock());
			Script.SetBlocks(new ProceduresCallnoreturnBlock());
			Script.SetBlocks(new ProceduresCallreturnBlock());
			Script.SetBlocks(new ProceduresIfreturnBlock());
			Script.SetBlocks(new TextBlock());
			Script.SetBlocks(new TextJoinBlock());
			Script.SetBlocks(new TextCreateJoinContainerBlock());
			Script.SetBlocks(new TextCreateJoinItemBlock());
			Script.SetBlocks(new TextAppendBlock());
			Script.SetBlocks(new TextLengthBlock());
			Script.SetBlocks(new TextIsEmptyBlock());
			Script.SetBlocks(new TextIndexOfBlock());
			Script.SetBlocks(new TextCharAtBlock());
			Script.SetBlocks(new TextGetSubstringBlock());
			Script.SetBlocks(new TextChangeCaseBlock());
			Script.SetBlocks(new TextTrimBlock());
			Script.SetBlocks(new TextPrintBlock());
			Script.SetBlocks(new TextPromptExtBlock());
			Script.SetBlocks(new TextPromptBlock());
			Script.SetBlocks(new VariablesGetBlock());
			Script.SetBlocks(new VariablesSetBlock());
			Script.SetGenerator(new Ruby());
			webBrowser1.Document.InvokeScript("start_blockly");

			Application.Idle += Application_Idle;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			var workspace = Blockly.getMainWorkspace();
			workspace.addChangeListener(Workspace_Changed);
		}

		private void Workspace_Changed(Blockly.Events.Abstract obj)
		{
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
	}

	class App
	{
		public class Terminal
		{
			StringBuilder log = new StringBuilder();

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

		public static Terminal Term = new Terminal();
	}
}
