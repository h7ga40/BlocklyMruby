using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bridge;

namespace BlocklyMruby
{
	public class XTermView : WebConsole
	{
		public XTermView()
		{
			Open(new ResourceReader("BlocklyMrubyRes"), new TerminalHost(this));

			Application.Idle += Application_Idle;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			Navigate("xterm.html");
		}

		protected override void DocumentCompleted(Uri Url)
		{
			if (Url.Scheme == "about")
				return;

			base.DocumentCompleted(Url);

			InvokeScript("start_xterm");
		}

		public void OnTermData(string text)
		{
			InvokeScript("on_term_data", new object[] { text });
		}

		public event EventHandler<StdioEventArgs> Stdio;

		internal void WriteStdin(string text)
		{
			Stdio?.Invoke(this, new StdioEventArgs(StdioType.In, text));
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class TerminalHost : ScriptingHost
	{
		public dynamic term;
		StringBuilder log = new StringBuilder();

		public TerminalHost(XTermView view)
			: base(view)
		{
		}

		public void on_data(object data)
		{
			((XTermView)View).WriteStdin(((string)data).Replace("\r", "\n"));
		}

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
}
