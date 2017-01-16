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
	public class ScriptingHost
	{
		public object bridge;
		WebConsole _View;
		WebBrowserReadyState _ReadyState;
		int _Status;
		string _Type;
		string _Url;
		bool _Async;
		byte[] _Response;
		string _ResponseText;
		string _ContentType;

		public ScriptingHost(WebConsole view)
		{
			_View = view;
		}

		public WebConsole View { get { return _View; } }

		public Script Script { get { return _View.Script; } }
		public Document Document { get { return _View.Script.Document; } }

		public object CreateXMLHttpRequest()
		{
			return new ScriptingHost(_View);
		}

		public object new_array()
		{
			return new List<object>();
		}

		public void array_add(object array_, object item)
		{
			var array = (List<object>)array_;
			array.Add(item);
		}

		public object new_object()
		{
			return new Dictionary<string, object>();
		}

		public void object_add(object dic_, string name, object item)
		{
			var dic = (Dictionary<string, object>)dic_;
			dic.Add(name, item);
		}

		public void console_log(object text)
		{
			System.Diagnostics.Debug.WriteLine(text.ToString());
		}

		public void console_warn(object text)
		{
			System.Diagnostics.Debug.WriteLine(text.ToString());
		}

		public object Error { get; set; }

		public object new_error()
		{
			return ((dynamic)Error).call();
		}

		public void onerror(string msg, string src, int line, int column, Dictionary<string, object> excpetion)
		{
			var kvs = new string[excpetion.Count];
			int i = 0;
			foreach (var kv in excpetion) {
				kvs[i++] = kv.Key + ":\"" + kv.Value.ToString() + "\"";
			}

			System.Diagnostics.Debug.WriteLine(GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name
				+ " " + msg + " " + src + " " + line + " " + column + "\n" + String.Join(",\n", kvs));
		}

		private void SetReadyState(WebBrowserReadyState readyState)
		{
			_ReadyState = readyState;
			try {
				if (onreadystatechange != null)
					onreadystatechange.call(null, true);
			}
			catch (System.Runtime.InteropServices.COMException e) {
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}

		private void Load(string url)
		{
			if (url.StartsWith("./"))
				url = url.Substring(2);

			if (!_View.ResourceReader.GetResource(url, out _Response)) {
				_Status = 404;
				SetReadyState(WebBrowserReadyState.Complete);
				return;
			}

			switch (System.IO.Path.GetExtension(url)) {
			case "html": _ContentType = "text/html"; break;
			case "css": _ContentType = "text/css"; break;
			case "js": _ContentType = "text/javascript"; break;
			case "gif": _ContentType = "image/gif"; break;
			case "jpeg": _ContentType = "image/jpeg"; break;
			case "png": _ContentType = "image/png"; break;
			case "svg": _ContentType = "image/svg+xml"; break;
			case "json": _ContentType = "application/json"; break;
			default: _ContentType = "text/plane"; break;
			}
			_ResponseText = Encoding.UTF8.GetString(_Response);

			_Status = 200;
			SetReadyState(WebBrowserReadyState.Complete);
		}

		public int readyState { get { return (int)_ReadyState; } }
		public int status { get { return _Status; } }
		public string statusText { get { return (_Status == 200) ? "OK" : "Not Found"; } }
		public string responseText { get { return _ResponseText; } }
		public dynamic onreadystatechange { get; set; }

		public void open(string type, string url, bool async)
		{
			open(type, url, async, "", "");
		}

		public void open(string type, string url, bool async, string username, string password)
		{
			_Type = type;
			_Url = url;
			_Async = async;

			_Status = 404;
			SetReadyState(WebBrowserReadyState.Loading);
		}

		public void send(object body)
		{
			SetReadyState(WebBrowserReadyState.Loaded);

			if (_Async)
				_View.BeginInvoke(new MethodInvoker(() => { Load(_Url); }));
			else
				Load(_Url);
		}

		public string getAllResponseHeaders()
		{
			return
				"Content-Type: " + _ContentType + "\n" +
				"Last-Modified: Tue, 11 Nov 2014 00:00:00 GMT\n" +
				"Accept-Ranges: bytes\n" +
				"Server: Microsoft-IIS/8.0\n" +
				"Date: " + DateTime.Now.ToString("R");
		}

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

		public object InvokeHandler(object handler_, object arguments_)
		{
			var handler = handler_ as Delegate;
			var arguments = arguments_ as List<object>;
			return handler.DynamicInvoke(arguments.ToArray());
		}

		public Element ToElement(dynamic element)
		{
			return Element.Create(Script, element);
		}

		public string NullOrString(object text)
		{
			return ((text == null) || (text is DBNull)) ? "null" : text.ToString();
		}
	}

	public class Script
	{
		public static DBNull Undefined = DBNull.Value;
		public dynamic Bridge;
		public ScriptingHost ScriptHost;
		public Document Document { get; private set; }

		public Script(ScriptingHost scriptHost)
		{
			ScriptHost = scriptHost;
			Bridge = ScriptHost.bridge;
			if (Bridge != null) {
				Bridge.instance = ScriptHost;
				Document = new Document(this);
			}
		}

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

		public dynamic NewFunc(Delegate d)
		{
			System.Diagnostics.Debug.Assert(d != null);
			return Bridge.NewFunc(d);
		}

		public dynamic New(string name, object[] args)
		{
			var a = Script.NewArray();
			foreach (var i in args)
				Script.Push(a, i);
			return Bridge.New(name, a);
		}

		public static int ParseInt(string value, int radix = 10)
		{
			return (int)Microsoft.JScript.GlobalObject.parseInt(value, radix);
		}

		public static double ParseFloat(string value)
		{
			return Microsoft.JScript.GlobalObject.parseFloat(value);
		}

		public static bool IsNaN(object num)
		{
			return Microsoft.JScript.GlobalObject.isNaN(num);
		}

		public object Get(object scope, string name)
		{
			return Bridge.Get(scope, name);
		}

		public T Get<T>(object scope, string name)
		{
			var ret = Bridge.Get(scope, name);
			if ((ret == null) || (ret is DBNull))
				return default(T);
			return (T)ret;
		}

		public void Set(object scope, string name, object value)
		{
			Bridge.Set(scope, name, value);
		}

		public string Replace(string str, dynamic regex, string dst)
		{
			return Bridge.Replace(str, regex, dst);
		}

		public string Split(string str, dynamic regex)
		{
			return Bridge.Split(str, regex);
		}

		public object Match(string str, dynamic regex)
		{
			return Bridge.Match(str, regex);
		}

		public Element CreateElement(string tagname)
		{
			return Element.Create(this, Bridge.CreateElement(tagname));
		}

		public Element CreateTextNode(string text)
		{
			return Element.Create(this, Bridge.CreateTextNode(text));
		}

		public HTMLElement GetElementById(string id)
		{
			return Element.Create(this, Bridge.GetElementById(id));
		}

		public void PreventDefault(Event @event)
		{
			Bridge.PreventDefault(@event.instance);
		}

		public void StopPropagation(Event @event)
		{
			Bridge.StopPropagation(@event.instance);
		}

		public string Stringify(object value)
		{
			return Bridge.Stringify(value);
		}

		public object Parse(string text)
		{
			return Bridge.Parse(text);
		}

		public static string EncodeURI(string url)
		{
			return Microsoft.JScript.GlobalObject.encodeURI(url);
		}

		public static string DecodeURI(string url)
		{
			return Microsoft.JScript.GlobalObject.decodeURI(url);
		}

		public dynamic NewRegExp(string pattern, string flag)
		{
			return Bridge.NewRegExp(pattern, flag);
		}

		public static string RegExpEscape(string s)
		{
			return Microsoft.JScript.GlobalObject.escape(s);
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

		public void console_log(string text)
		{
			ScriptHost.console_log((object)text);
		}

		public void console_warn(string text)
		{
			ScriptHost.console_warn((object)text);
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
			var Script = regex.Script;
			return Script.Replace(str, regex.instance, dst);
		}

		public static string[] Split(this string str, Text.RegularExpressions.Regex regex)
		{
			var Script = regex.Script;
			var ret = Script.Split(str, regex.instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			int len = Script.Get(ret, "length");
			var result = new List<string>(len);
			for (int i = 0; i < len; i++) {
				result.Add(Script.Get<string>(ret, i.ToString()));
			}
			return result.ToArray();
		}

		public static string[] Match(this string str, Text.RegularExpressions.Regex regex)
		{
			var Script = regex.Script;
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

	public static class StringListExtention
	{
		public static string Join(this IEnumerable<string> list, string separator)
		{
			return String.Join(separator, list);
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
		public Script Script { get; private set; }

		public string parentId { get { return instance.parentId; } }
		public string inputName { get { return instance.inputName; } }
		public object coordinate { get { return instance.coordinate; } }

		public Location(Script script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public void construct(string parentId, string inputName, object coordinate)
		{
			instance = Script.New("Blockly.Location", new[] { parentId, inputName, coordinate });
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
		public Script Script { get; private set; }

		public Node(Script script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public string NodeName { get { return instance.nodeName; } }
		public string NodeValue { get { return instance.nodeValue; } }
		public NodeList ChildNodes { get { return new NodeList(Script, instance.childNodes); } }
	}

	public class NodeList
	{
		public dynamic instance;
		public Script Script { get; private set; }

		public NodeList(Script script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public Node this[int index] { get { return Element.Create(Script, Script.Get(instance, index.ToString())); } }

		public int Length { get { return instance.length; } }
	}

	public class Element : Node
	{
		public string InnerHTML { get { return instance.innerHTML; } set { instance.innerHTML = value; } }

		public string OuterHTML { get { return instance.outerHTML; } }

		public Element(Script script, object instance)
			: base(script, instance)
		{
		}

		public static Element Create(Script script, dynamic instance)
		{
			if ((instance == null) || (instance is DBNull))
				return null;
			return new Element(script, instance);
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

	public class Document
	{
		public Script Script { get; private set; }

		public Document(Script script)
		{
			Script = script;
		}

		public Element CreateElement(string tagname)
		{
			return Script.CreateElement(tagname);
		}

		public Element CreateTextNode(string text)
		{
			return Script.CreateTextNode(text);
		}

		public HTMLElement GetElementById(string id)
		{
			return Script.GetElementById(id);
		}
	}

	public class HTMLElement : Element
	{
		public HTMLElement(Script script, object instance)
			: base(script, instance)
		{
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
		public Script Script { get; private set; }

		public Event(Script script, object instance)
		{
			Script = script;
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
		public static string Stringify(object value)
		{
			return Codeplex.Data.DynamicJson.Serialize(value);
		}

		public static object Parse(string text)
		{
			return Codeplex.Data.DynamicJson.Parse(text);
		}
	}
}

namespace Bridge.Text.RegularExpressions
{
	public class Regex
	{
		public dynamic instance;
		public Script Script { get; private set; }

		public Regex(Script script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public Regex(Script script, string patern, string flag = "")
		{
			Script = script;
			instance = Script.NewRegExp(patern, flag);
		}

		public bool Global { get { return instance.global; } }
		public bool IgnoreCase { get { return instance.ignoreCase; } }
		public int LastIndex { get { return instance.lastIndex; } set { instance.lastIndex = value; } }
		public bool Multiline { get { return instance.multiline; } }
		public string Pattern { get { return instance.pattern; } }
		public string Source { get { return instance.source; } }

		public static string Escape(string s)
		{
			return Script.RegExpEscape(s);
		}

		public Matches Exec(string s)
		{
			return new Matches(Script, instance.exec(s));
		}

		public bool Test(string s)
		{
			return instance.test(s);
		}
	}

	public class Matches
	{
		public dynamic instance;
		public Script Script { get; private set; }

		public Matches(Script script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public int index { get { return instance.index; } }

		public string this[int i] { get { return Script.Get(instance, i.ToString()); } }

		public static implicit operator string(Matches m)
		{
			return m.instance;
		}
	}
}
