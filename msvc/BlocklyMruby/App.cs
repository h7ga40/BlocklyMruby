using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace BlocklyMruby
{
	static class App
	{
		public static MainForm MainForm;
		public static TerminalHost Term;

		static App()
		{
		}

		internal static void NewItem(Action<IClassWorkspace> callback)
		{
			var identifier = Collections.ClassWorkspaces.UniqueName("Class");
			var view = MainForm.NewBlocklyView(identifier);
			view.DocumentLoaded += (s, e) => {
				var workspace = new ClassWorkspace(view, identifier);
				callback(workspace);
			};
		}

		internal static void RemoveItem(IClassWorkspace item)
		{
			MainForm.RemoveEObjectWorkspace(item);
		}
	}

	internal interface IClassWorkspace : IModel
	{
		Workspace Workspace { get; }
		BlocklyView View { get; }
		Ruby RubyCode { get; }
		string GetImageUrl();
		bool IsPreset();
		string ToCode(string filename);
		void Activate();
		void Inactivate();
		void ReloadToolbox(HTMLElement toolbox);
		void OpenModifyView(Action<bool> callback);
		string Template(string template);
	}

	public class Collections
	{
		[Name(false)]
		internal static Collection<IClassWorkspace> ClassWorkspaces = new Collection<IClassWorkspace>();
	}

	public class Views
	{
		internal static ClassSelectorView ClassSelectorView;
	}

	public class AppScript
	{
		ResourceReader Reader;

		public AppScript()
		{
			Reader = new ResourceReader("BlocklyMrubyRes");
		}

		internal void Ajax(AjaxOptions ajaxOptions)
		{
			byte[] res;
			if (Reader.GetResource(ajaxOptions.Url, out res)) {
				ajaxOptions.Success(Encoding.UTF8.GetString(res), "", null);
			}
			else {
				ajaxOptions.Error(null, "", "");
			}
		}

		internal object ParseJSON(string data)
		{
			return JSON.Parse(data);
		}
	}
}
