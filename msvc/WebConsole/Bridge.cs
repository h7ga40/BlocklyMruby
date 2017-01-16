using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bridge.Html5;

namespace Bridge
{
	/// <summary>
	/// WebBrowserのJavaScriptから呼ばれる関数の定義
	/// </summary>
	/// <remarks>
	/// JavaScriptから呼び出す際の引数の数や型で関数の選択がうまく行かないので、
	/// ここでリダイレクトするため、引数の型はobject型で定義する。
	/// </remarks>
	[ComVisible(true)]
	public class ScriptHost
	{
		public bool isDefined(object obj, string name)
		{
			var members = obj.GetType().GetMember(name);
			return members.Length != 0;
		}

		public dynamic GetElementInstance(object element_)
		{
			var element = (Element)element_;
			return element.instance;
		}

		public List<object> NewArray()
		{
			return new List<object>();
		}

		public void Push(object a, object i)
		{
			var l = a as List<object>;
			l.Add(i);
		}

		public object InvokeHandler(object handler_, object arguments_)
		{
			var handler = handler_ as Delegate;
			var arguments = arguments_ as List<object>;
			return handler.DynamicInvoke(arguments.ToArray());
		}

		public Element ToElement(dynamic element)
		{
			return Element.Create(element);
		}

		public void console_log(object text)
		{
			Console.WriteLine(text.ToString());
		}

		public void console_warn(object text)
		{
			Console.WriteLine(text.ToString());
		}
	}

	public class Script
	{
		public static DBNull Undefined = DBNull.Value;
		public static dynamic Bridge;
		public static ScriptHost ScriptHost;

		public static dynamic NewObject()
		{
			return new Microsoft.JScript.JSObject();
		}

		public static dynamic NewArray(params object[] items)
		{
			var a = new Microsoft.JScript.JSObject();
			foreach (var i in items)
				Microsoft.JScript.ArrayPrototype.push(a, i);
			return a;
		}

		public static void Push(object a, object first, params object[] items)
		{
			Microsoft.JScript.ArrayPrototype.push(a, first);
			foreach (var i in items)
				Microsoft.JScript.ArrayPrototype.push(a, i);
		}

		public static dynamic NewFunc(Delegate d)
		{
			System.Diagnostics.Debug.Assert(d != null);
			return Bridge.NewFunc(d);
		}

		public static dynamic New(string name, object[] args)
		{
			var a = Script.NewArray();
			foreach (var i in args)
				Script.Push(a, i);
			return Bridge.New(name, a);
		}

		public static int ParseInt(string value, int radix)
		{
			return (int)Bridge.ParseInt(value, radix);
		}

		public static double ParseFloat(string value)
		{
			return (double)Bridge.ParseFloat(value);
		}

		public static bool IsNaN(object num)
		{
			return (bool)Bridge.IsNaN(num);
		}

		public static object Get(object scope, string name)
		{
			return Bridge.Get(scope, name);
		}

		public static T Get<T>(object scope, string name)
		{
			var ret = Bridge.Get(scope, name);
			if ((ret == null) || (ret is DBNull))
				return default(T);
			return (T)ret;
		}

		public static void Set(object scope, string name, object value)
		{
			Bridge.Set(scope, name, value);
		}

		public static string Replace(string str, dynamic instance, string dst)
		{
			return Bridge.Replace(str, instance, dst);
		}

		public static object Match(string str, dynamic instance)
		{
			return Bridge.Match(str, instance);
		}

		public static Element CreateElement(string tagname)
		{
			return Element.Create(Bridge.CreateElement(tagname));
		}

		public static Element CreateTextNode(string v)
		{
			return Element.Create(Bridge.CreateTextNode(v));
		}

		public static void PreventDefault(Event @event)
		{
			Bridge.PreventDefault(@event.instance);
		}

		public static void StopPropagation(Event @event)
		{
			Bridge.StopPropagation(@event.instance);
		}

		public static string Stringify(object value)
		{
			return Bridge.Stringify(value);
		}

		public static object Parse(string text)
		{
			return Bridge.Parse(text);
		}

		public static string EncodeURI(string url)
		{
			return Bridge.EncodeURI(url);
		}

		public static string dncodeURI(string url)
		{
			return Bridge.DncodeURI(url);
		}

		public static dynamic NewRegExp(string pattern, string flag)
		{
			return Bridge.NewRegExp(pattern, flag);
		}

		public static string[] GetFieldNames(object comObj)
		{
			List<string> result = new List<string>();

			IDispatch dispatch = comObj as IDispatch;
			if (dispatch == null)
				return result.ToArray();

			System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo = null;
			try {
				if (dispatch.GetTypeInfo(0, 0, out typeInfo) != 0)
					return result.ToArray();

				IntPtr _typeAttr;
				typeInfo.GetTypeAttr(out _typeAttr);
				System.Runtime.InteropServices.ComTypes.TYPEATTR typeAttr = (System.Runtime.InteropServices.ComTypes.TYPEATTR)Marshal.PtrToStructure(_typeAttr, typeof(System.Runtime.InteropServices.ComTypes.TYPEATTR));
				typeInfo.ReleaseTypeAttr(_typeAttr);

				for (int i = 0; i < typeAttr.cVars; i++) {
					IntPtr _varDesc;
					typeInfo.GetVarDesc(i, out _varDesc);
					System.Runtime.InteropServices.ComTypes.VARDESC varDesc = (System.Runtime.InteropServices.ComTypes.VARDESC)Marshal.PtrToStructure(_varDesc, typeof(System.Runtime.InteropServices.ComTypes.VARDESC));
					typeInfo.ReleaseVarDesc(_varDesc);

					int j;
					string[] names = new string[5];
					typeInfo.GetNames(varDesc.memid, names, names.Length, out j);
					Array.Resize(ref names, j);

					foreach (string name in names) {
						result.Add(name);
					}
				}
			}
			finally {
				if (typeInfo != null) Marshal.ReleaseComObject(typeInfo);
			}

			return result.ToArray();
		}

		public static string[] GetFuncNames(object comObj)
		{
			List<string> result = new List<string>();

			IDispatch dispatch = comObj as IDispatch;
			if (dispatch == null)
				return result.ToArray();

			System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo = null;
			try {
				if (dispatch.GetTypeInfo(0, 0, out typeInfo) != 0)
					return result.ToArray();

				IntPtr _typeAttr;
				typeInfo.GetTypeAttr(out _typeAttr);
				System.Runtime.InteropServices.ComTypes.TYPEATTR typeAttr = (System.Runtime.InteropServices.ComTypes.TYPEATTR)Marshal.PtrToStructure(_typeAttr, typeof(System.Runtime.InteropServices.ComTypes.TYPEATTR));
				typeInfo.ReleaseTypeAttr(_typeAttr);

				for (int i = 0; i < typeAttr.cFuncs; i++) {
					IntPtr _funcDesc;
					typeInfo.GetFuncDesc(i, out _funcDesc);
					System.Runtime.InteropServices.ComTypes.FUNCDESC funcDesc = (System.Runtime.InteropServices.ComTypes.FUNCDESC)Marshal.PtrToStructure(_funcDesc, typeof(System.Runtime.InteropServices.ComTypes.FUNCDESC));
					typeInfo.ReleaseFuncDesc(_funcDesc);

					int j;
					string[] names = new string[5];
					typeInfo.GetNames(funcDesc.memid, names, names.Length, out j);
					Array.Resize(ref names, j);

					foreach (string name in names) {
						result.Add(name);
					}
				}
			}
			finally {
				if (typeInfo != null) Marshal.ReleaseComObject(typeInfo);
			}

			return result.ToArray();
		}

		public static bool HasMember(object obj, string name)
		{
			var a = GetFieldNames(obj);
			return a.Contains(name);
		}

		private string NullOrString(object text)
		{
			return ((text == null) || (text is DBNull)) ? "null" : text.ToString();
		}

		public static void console_log(string text)
		{
			ScriptHost.console_log(text);
		}

		public static void console_warn(string text)
		{
			ScriptHost.console_warn(text);
		}
	}

	public class Any<T1, T2>
	{
		object value;

		public Any(object value)
		{
			if (value is T1 || value is T2)
				this.value = value;
			else
				throw new ArgumentException();
		}

		public object Value { get { return value; } }

		public T As<T>() { if (value is T) return (T)value; else return default(T); }

		public bool Is<T>() { return value is T; }

		public static implicit operator Any<T1, T2>(T1 value)
		{
			return new Any<T1, T2>(value);
		}

		public static implicit operator Any<T1, T2>(T2 value)
		{
			return new Any<T1, T2>(value);
		}

		public static explicit operator T1(Any<T1, T2> any)
		{
			return any == null ? default(T1) : (T1)any.value;
		}

		public static explicit operator T2(Any<T1, T2> any)
		{
			return any == null ? default(T2) : (T2)any.value;
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}

	public class Any<T1, T2, T3>
	{
		object value;

		public Any(object value)
		{
			if (value is T1 || value is T2 || value is T3)
				this.value = value;
			else
				throw new ArgumentException();
		}

		public object Value { get { return value; } }

		public T As<T>() { if (value is T) return (T)value; else return default(T); }

		public bool Is<T>() { return value is T; }

		public static implicit operator Any<T1, T2, T3>(T1 value)
		{
			return new Any<T1, T2, T3>(value);
		}

		public static implicit operator Any<T1, T2, T3>(T2 value)
		{
			return new Any<T1, T2, T3>(value);
		}

		public static implicit operator Any<T1, T2, T3>(T3 value)
		{
			return new Any<T1, T2, T3>(value);
		}

		public static explicit operator T1(Any<T1, T2, T3> any)
		{
			return (T1)any.value;
		}

		public static explicit operator T2(Any<T1, T2, T3> any)
		{
			return (T2)any.value;
		}

		public static explicit operator T3(Any<T1, T2, T3> any)
		{
			return (T3)any.value;
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}

	public static class StringExtention
	{
		public static string ToLowerCase(this string str)
		{
			return str.ToLower();
		}

		public static string Replace(this string str, Text.RegularExpressions.Regex regex, string dst)
		{
			return Script.Replace(str, regex.instance, dst);
		}

		public static string[] Match(this string str, Text.RegularExpressions.Regex regex)
		{
			var ret = Script.Match(str, regex.instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			int len = Script.Get(ret, "length");
			var result = new List<string>(len);
			for (int i = 0; i < len; i++) {
				result.Add(Script.Get<string>(ret, i.ToString()));
			}
			return result.ToArray();
		}

		public static string Join(this string[] str, string sep)
		{
			return String.Join(sep, str);
		}

		public static string CharAt(this string str, int index)
		{
			return str[index].ToString();
		}

		public static string[] Split(this string str, string sep)
		{
			return str.Split(new string[] { sep }, StringSplitOptions.None);
		}

		public static string Substr(this string str, int start)
		{
			return str.Substring(start);
		}

		public static int LocaleCompare(this string str, string b)
		{
			return str.CompareTo(b);
		}
	}

	public class ExternalAttribute : Attribute
	{

	}

	public class FieldPropertyAttribute : Attribute
	{

	}

	public class NameAttribute : Attribute
	{
		public NameAttribute(bool charcase)
		{
		}

		public NameAttribute(string name)
		{

		}
	}

	public class Location
	{
		public dynamic instance;

		public string parentId { get { return instance.parentId; } }
		public string inputName { get { return instance.inputName; } }
		public object coordinate { get { return instance.coordinate; } }

		public Location(object instance)
		{
			this.instance = instance;
		}

		public void construct(string parentId, string inputName, object coordinate)
		{
			instance = Script.New("Location", new[] { parentId, inputName, coordinate });
			instance.parentId = parentId;
			instance.inputName = inputName;
			instance.coordinate = coordinate;
		}
	}

	/// <summary> 
	/// Exposes objects, methods and properties to programming tools and other 
	/// applications that support Automation. 
	/// </summary> 
	[ComImport()]
	[Guid("00020400-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IDispatch
	{
		[PreserveSig]
		int GetTypeInfoCount(out int Count);

		[PreserveSig]
		int GetTypeInfo(
			[MarshalAs(UnmanagedType.U4)] int iTInfo,
			[MarshalAs(UnmanagedType.U4)] int lcid,
			out System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);

		[PreserveSig]
		int GetIDsOfNames(
			ref Guid riid,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
			string[] rgsNames,
			int cNames,
			int lcid,
			[MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);

		[PreserveSig]
		int Invoke(
			int dispIdMember,
			ref Guid riid,
			uint lcid,
			ushort wFlags,
			ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
			out object pVarResult,
			ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
			IntPtr[] pArgErr);
	}
}

namespace Bridge.Html5
{
	public class Node
	{
		public dynamic instance;

		public Node(object instance)
		{
			this.instance = instance;
		}

		public string NodeName { get { return instance.nodeName; } }
		public NodeList ChildNodes { get { return new NodeList(instance.childNodes); } }
	}

	public class NodeList
	{
		public dynamic instance;

		public NodeList(object instance)
		{
			this.instance = instance;
		}

		public Node this[int index] { get { return Element.Create(Script.Get(instance, index.ToString())); } }
	}

	public class Element : Node
	{
		public string InnerHTML { get { return instance.innerHTML; } }

		public string OuterHTML { get { return instance.outerHTML; } }

		public Element(object instance)
			: base(instance)
		{
		}

		public static Element Create(dynamic instance)
		{
			if ((instance == null) || (instance is DBNull))
				return null;
			return new Element(instance);
		}

		public string GetAttribute(string name)
		{
			var ret = instance.getAttribute(name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return (string)ret;
		}

		public void SetAttribute(string name, string value)
		{
			instance.setAttribute(name, value);
		}

		public void AppendChild(Element parameter)
		{
			instance.appendChild(parameter.instance);
		}
	}

	public static class Document
	{
		public static Element CreateElement(string tagname)
		{
			return Script.CreateElement(tagname);
		}

		public static Element CreateTextNode(string v)
		{
			return Script.CreateTextNode(v);
		}
	}

	public class SVGMatrix
	{
		public dynamic instance;

		public SVGMatrix(dynamic instance)
		{
			this.instance = instance;
		}
	}

	public class Event
	{
		public dynamic instance;

		public Event(object instance)
		{
			this.instance = instance;
		}

		public void PreventDefault()
		{
			Script.PreventDefault(this);
		}

		public void StopPropagation()
		{
			Script.StopPropagation(this);
		}
	}

	public class JSON
	{
		public static string Stringify(string[] value)
		{
			//return Script.Stringify(value);
			return Codeplex.Data.DynamicJson.Serialize(value);
		}

		public static object Parse(string text)
		{
			//return Script.Parse(text);
			return Codeplex.Data.DynamicJson.Parse(text);
		}
	}
}

namespace Bridge.Text.RegularExpressions
{
	public class Regex
	{
		public dynamic instance;

		public Regex(object instance)
		{
			this.instance = instance;
		}

		public Regex(string patern, string flag = "")
		{
			instance = Script.NewRegExp(patern, flag);
		}
	}
}
