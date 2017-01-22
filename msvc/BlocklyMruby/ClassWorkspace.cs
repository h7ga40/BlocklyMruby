using System;
using System.Text;
using Bridge.Html5;

namespace BlocklyMruby
{
	internal class ClassWorkspace : IClassWorkspace
	{
		private string identifier;
		private BlocklyView view;
		private Ruby rubyCode;

		public ClassWorkspace(BlocklyView view, string identifier)
		{
			this.view = view;
			this.identifier = identifier;
		}

		public string Identifier {
			get {
				return identifier;
			}
		}

		public Ruby RubyCode {
			get {
				return rubyCode;
			}
		}

		public BlocklyView View {
			get {
				return view;
			}
		}

		public Workspace Workspace {
			get {
				return view.Blockly.getMainWorkspace();
			}
		}

		public void Activate()
		{
		}

		public string GetImageUrl()
		{
			return "";
		}

		public void Inactivate()
		{
		}

		public bool IsPreset()
		{
			return false;
		}

		public void OpenModifyView(Action<bool> callback)
		{
			callback(false);
		}

		public void ReloadToolbox(HTMLElement toolbox)
		{
		}

		public string Template(string template)
		{
			template = template.Replace("%identifier%", identifier);
			template = template.Replace("%attribute%", "Class1");
			template = template.Replace("%img%", GetImageUrl());
			return template;
		}

		public string ToCode(string filename)
		{
			rubyCode = new Ruby(view.Blockly, filename);
			view.Blockly.Script.changed = false;

			var sb = new StringBuilder();
			sb.AppendLine("class " + identifier);
			var code = rubyCode.workspaceToCode(Workspace);
			sb.Append(code);
			sb.AppendLine("end");

			return sb.ToString();
		}
	}
}