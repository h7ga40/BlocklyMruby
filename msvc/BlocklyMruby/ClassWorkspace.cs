using System;
using System.Text;
using Bridge;
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
			var _RubyCode = new Ruby(view.Blockly, filename);
			_RubyCode.init(Workspace);
			var result = _RubyCode.defineClass(identifier, Workspace);
			((BlocklyScript)view.Script).changed = false;
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
