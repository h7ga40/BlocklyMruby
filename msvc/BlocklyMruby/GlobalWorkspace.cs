using System;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	internal class GlobalWorkspace : IClassWorkspace
	{
		private BlocklyView view;
		private Ruby rubyCode;

		public GlobalWorkspace(BlocklyView view)
		{
			this.view = view;
		}

		public string Identifier {
			get {
				return view.Identifier;
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
				return view.Workspace;
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
			template = template.Replace("%identifier%", Identifier);
			template = template.Replace("%attribute%", "Global");
			template = template.Replace("%img%", GetImageUrl());
			return template;
		}

		public string ToCode(string filename)
		{
			rubyCode = new Ruby(view.Blockly, filename);
			rubyCode.init(Workspace);
			var result = rubyCode.defineGlobal(Identifier, Workspace);
			view.Changed = false;
			return result;
		}
	}

	public partial class Ruby
	{
		internal string defineGlobal(string identifier, Workspace workspace)
		{
			var nodes = workspaceToNodes(workspace);
			return finish(nodes);
		}
	}
}
