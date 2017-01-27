using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace BlocklyMruby
{
	static class App
	{
		public static TerminalHost Term;
		public static TextWriter Out { get { return Term.Out; } }

		static App()
		{
		}

		internal static void NewItem(Action<IClassWorkspace> callback)
		{
			var identifier = Collections.ClassWorkspaces.UniqueName("Class");
			var view = Views.MainMenuView.NewBlocklyView(identifier);
			view.DocumentLoaded += (s, e) => {
				var workspace = new ClassWorkspace(view);
				callback(workspace);
			};
		}

		internal static void RemoveItem(IClassWorkspace item)
		{
			Views.MainMenuView.RemoveWorkspace(item);
		}

		internal static void Write(string text)
		{
			Term?.write(text);
		}

		internal static void Write(string text, params object[] args)
		{
			Term?.write(String.Format(text, args));
		}

		internal static void WriteLine()
		{
			Term?.write("");
		}

		internal static void WriteLine(string text)
		{
			Term?.write(text + "\n");
		}

		internal static void WriteLine(string text, params object[] args)
		{
			Term?.write(String.Format(text, args) + "\n");
		}

		internal static string ReadLine()
		{
			throw new NotImplementedException();
		}

		internal static void Flush()
		{
			Term?.flush();
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
		internal static GlobalWorkspace GlobalWorkspace;
		[Name(false)]
		internal static Collection<IClassWorkspace> ClassWorkspaces;
	}

	public class Views
	{
		[Name(false)]
		internal static MainForm MainMenuView;
		[Name(false)]
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
