using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlocklyMruby
{
	public static class Mruby
	{
		[DllImport("mruby.dll")]
		extern static int mruby_main(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] argv);
		[DllImport("mruby.dll")]
		extern static int mrbc_main(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] argv);

		public static int mruby(params string[] args_)
		{
			var args = new List<string>();
			args.Add("mruby");
			args.AddRange(args_);
			return mruby_main(args.Count, args.ToArray());
		}

		public static int mrbc(params string[] args_)
		{
			var args = new List<string>();
			args.Add("mrbc");
			args.AddRange(args_);
			return mrbc_main(args.Count, args.ToArray());
		}
	}
}
