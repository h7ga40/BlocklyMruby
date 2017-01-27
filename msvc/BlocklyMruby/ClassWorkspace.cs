using System;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	internal class ClassWorkspace : IClassWorkspace
	{
		private BlocklyView view;
		private Ruby rubyCode;

		public ClassWorkspace(BlocklyView view)
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
			return false;
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
			template = template.Replace("%attribute%", "Class");
			template = template.Replace("%img%", GetImageUrl());
			return template;
		}

		public string ToCode(string filename)
		{
			rubyCode = new Ruby(view.Blockly, filename);
			rubyCode.init(Workspace);
			var result = rubyCode.defineClass(Identifier, Workspace);
			view.Changed = false;
			return result;
		}
	}

	public partial class Ruby
	{
		internal string defineClass(string identifier, Workspace workspace)
		{
			var nodes = new JsArray<node>();
			global = false;
			var lv = local_switch();
			{
				var body = new begin_node(this, workspaceToNodes(workspace));
				var cls = new class_node(this, intern(identifier), null, body);
				nodes.Add(cls);
			}
			local_resume(lv);
			global = true;

			return finish(nodes);
		}
	}
}
