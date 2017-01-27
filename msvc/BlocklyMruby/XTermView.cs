using System;
using System.Collections.Generic;
using System.IO;
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
			((TerminalHost)ObjectForScripting).write(text);
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

		public TerminalHost(XTermView view)
			: base(view)
		{
			Out = new XTermWriter(this);
		}

		internal XTermWriter Out { get; private set; }

		public void on_data(object data)
		{
			((XTermView)View).WriteStdin(((string)data).Replace("\r", "\n"));
		}

		internal void flush()
		{
		}

		internal void write(string text)
		{
			term.write(text.Replace("\n", "\r\n"));
		}
	}

	class XTermWriter : TextWriter
	{
		private TerminalHost term;

		public XTermWriter(TerminalHost term)
		{
			this.term = term;
		}

		public override Encoding Encoding {
			get {
				return new UTF8Encoding(false);
			}
		}

		public override void WriteLine(string value)
		{
			term.write(value + NewLine);
		}
	}
}
