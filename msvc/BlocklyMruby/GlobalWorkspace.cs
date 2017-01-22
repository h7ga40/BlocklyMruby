using System;
using System.Text;
using Bridge.Html5;

namespace BlocklyMruby
{
	internal class GlobalWorkspace : IClassWorkspace
	{
		private string identifier;
		private BlocklyView view;
		private Ruby rubyCode;

		public GlobalWorkspace(BlocklyView view, string identifier)
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
			return true;
		}

		public void OpenModifyView(Action<bool> callback)
		{
			callback(true);
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

			return rubyCode.workspaceToCode(Workspace);
		}
	}
}