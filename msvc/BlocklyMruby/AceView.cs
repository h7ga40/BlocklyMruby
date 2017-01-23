using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bridge;

namespace BlocklyMruby
{
	public class AceView : WebConsole
	{
		public AceView()
		{
			Open(new ResourceReader("BlocklyMrubyRes"), new RubyCodeHost(this));

			Application.Idle += Application_Idle;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			Navigate("ace.html");
		}

		protected override void DocumentCompleted(Uri Url)
		{
			if (Url.Scheme == "about")
				return;

			base.DocumentCompleted(Url);

			InvokeScript("start_ace");
			InvokeScript("focus_editor");
		}

		public void SetText(string code)
		{
			InvokeScript("set_text", new object[] { code });
		}

		public void MoveCursorBy(int x, int y)
		{
			InvokeScript("move_cursor_to", new object[] { x, y });
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class RubyCodeHost : ScriptingHost
	{
		Mruby _Mruby;
		public dynamic editor;

		public RubyCodeHost(AceView view)
			: base(view)
		{
		}

		internal void SetMruby(Mruby mruby)
		{
			_Mruby = mruby;
		}

		public void on_change()
		{

		}
	}
}
