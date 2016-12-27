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
using BlocklyMruby;

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
		public dynamic blockly;
		public dynamic ProceduresGetDefinition;
		public dynamic ProceduresMutateCallers;
		public dynamic XmlWorkspaceToDom;
		public dynamic XmlBlockToDomWithXY;
		public dynamic XmlBlockToDom;
		public dynamic XmlDomToText;
		public dynamic XmlDomToPrettyText;
		public dynamic XmlTextToDom;
		public dynamic XmlDomToWorkspace;
		public dynamic XmlDomToBlock;
		public dynamic XmlDeleteNext;
		public dynamic EventsGetDescendantIds_;
		public dynamic EventsEnable;
		public dynamic EventsDisable;
		public dynamic EventsSetGroup;
		public dynamic ContextMenuCallbackFactory;
		public dynamic WorkspaceGetById;
		public dynamic NamesEquals;
		public dynamic MutatorReconnect;

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

		public dynamic GetBlockInstance(object block_)
		{
			var block = (Block)block_;
			return block.instance;
		}

		public object NewBlockInstance(object instance)
		{
			var block = Script.CreateBlock(instance);
			System.Diagnostics.Debug.Assert(block != null);
			((dynamic)block).init();
			return block;
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

		public Blockly.Workspace ToWorkspace(dynamic workspace)
		{
			return Blockly.WorkspaceSvg.Create(workspace);
		}

		public Block ToBlock(dynamic block)
		{
			var instance = Script.Get(block, "instance");
			if (instance != null)
				return (Block)instance;
			return Script.CreateBlock(block);
		}

		public Blockly.Events.Abstract ToEvent(dynamic ev)
		{
			return Script.CreateEvent(ev);
		}

		public object mutationToDom(object block_, object opt_paramIds_)
		{
			var block = (Block)block_;
			bool opt_paramIds = opt_paramIds_ == null ? false : (bool)opt_paramIds_;
			var ret = (Element)((dynamic)block).mutationToDom(opt_paramIds);
			if (ret != null)
				return GetElementInstance(ret);
			return null;
		}

		public void domToMutation(object block_, object xmlElement)
		{
			var block = (Block)block_;
			((dynamic)block).domToMutation(ToElement(xmlElement));
		}

		public object decompose(object block_, object workspace)
		{
			var block = (Block)block_;
			var ret = (Block)((dynamic)block).decompose(ToWorkspace(workspace));
			if (ret != null) {
				return GetBlockInstance(ret);
			}
			return null;
		}

		public void compose(object block_, object containerBlock)
		{
			dynamic block = (Block)block_;
			block.compose(ToBlock(containerBlock));
		}

		public void saveConnections(object block_, object containerBlock)
		{
			dynamic block = (Block)block_;
			block.saveConnections(ToBlock(containerBlock));
		}

		public void onchange(object block_, object e)
		{
			dynamic block = (Block)block_;
			block.onchange(ToEvent(e));
		}

		public void customContextMenu(object block_, object options_)
		{
			dynamic block = (Block)block_;
			var options = new ContextMenuOptionList(options_);
			block.customContextMenu(options);
		}

		public object getProcedureCall(object block_)
		{
			dynamic block = (Block)block_;
			return block.getProcedureCall();
		}

		public object getProcedureDef(object block_)
		{
			dynamic block = (Block)block_;
			var ret = block.getProcedureDef();
			if (ret != null) {
				var a = Script.NewArray();
				foreach (var i in ret) {
					if (i is string[])
						Script.Push(a, Script.NewArray(i));
					else
						Script.Push(a, i);
				}
				return a;
			}
			return null;
		}

		public object getVars(object block_)
		{
			dynamic block = (Block)block_;
			var ret = block.getVars();
			if (ret != null) {
				var a = Script.NewArray();
				foreach (var i in ret)
					Script.Push(a, i);
				return a;
			}
			return null;
		}

		public void renameProcedure(object block_, object oldName, object newName)
		{
			dynamic block = (Block)block_;
			block.renameProcedure((string)oldName, (string)newName);
		}

		public void renameVar(object block_, object oldName, object newName)
		{
			dynamic block = (Block)block_;
			block.renameVar((string)oldName, (string)newName);
		}

		public void init(object generator_, object workspace)
		{
			dynamic generator = (Generator)generator_;
			generator.init(ToWorkspace(workspace));
		}

		public string finish(object generator_, object code)
		{
			dynamic generator = (Generator)generator_;
			return generator.finish((string)code);
		}

		public string scrubNakedValue(object generator_, object line)
		{
			dynamic generator = (Generator)generator_;
			return generator.scrubNakedValue((string)line);
		}

		public string scrub_(object generator_, object block, object code)
		{
			dynamic generator = (Generator)generator_;
			return generator.scrub_(ToBlock(block), (string)code);
		}

		public void console_log(object text)
		{
			Console.WriteLine(text.ToString());
		}

		public void console_warn(object text)
		{
			Console.WriteLine(text.ToString());
		}

		public void dump_svg_element(object obj, string text, object element)
		{
			var block = Script.CreateBlock(obj);
			Console.Write(block.type + "(" + block.id + ")" + text + " :");
			var target = (dynamic)element;
			var owner = target.parentNode();
			Console.Write(" "
				+ $"class: {NullOrString(target.getAttribute("class"))} "
				+ $"stroke: {NullOrString(target.getAttribute("stroke"))} "
				+ $"transform: {NullOrString(target.getAttribute("transform"))} "
				+ $"requiredFeatures: {NullOrString(target.getAttribute("requiredFeatures"))} "
				+ $"requiredExtensions: {NullOrString(target.getAttribute("requiredExtensions"))} "
				+ $"systemLanguage: {NullOrString(target.getAttribute("systemLanguage"))} "
				+ $"d: {NullOrString(target.getAttribute("d"))} ");
			if ((owner != null) && !(owner is DBNull)) {
				var clsname = NullOrString(owner.getAttribute("class"));
				Console.Write(" "
					+ $"{clsname}.class: {clsname} "
					+ $"{clsname}.transform: {NullOrString(owner.getAttribute("transform"))} "
					+ $"{clsname}.requiredFeatures: {NullOrString(owner.getAttribute("requiredFeatures"))} "
					+ $"{clsname}.requiredExtensions: {NullOrString(owner.getAttribute("requiredExtensions"))} "
					+ $"{clsname}.systemLanguage: {NullOrString(owner.getAttribute("systemLanguage"))} "
					+ $"{clsname}.d: {NullOrString(owner.getAttribute("d"))} "
				);
			}
			Console.WriteLine();
		}

		private string NullOrString(object text)
		{
			return ((text == null) || (text is DBNull)) ? "null" : text.ToString();
		}
	}

	public class Script
	{
		public static DBNull Undefined = DBNull.Value;
		internal static HtmlDocument Document;
		public static dynamic Bridge;
		public static ScriptHost ScriptHost;

		public static string runtime = @"
var Bridge = {
	instance: null
};
Bridge.NewFunc = function(handler) {
	var ret = function() {
		var a = Bridge.instance.NewArray();
		for(i in arguments) {
			Bridge.instance.Push(a, arguments[i]);
		}
		return Bridge.instance.InvokeHandler(handler, a);
	};
	ret.handler = handler;
	return ret;
}
Bridge.New = function(name, args) {
	var ctor = eval(name);
	var obj = Object.create(ctor.prototype);
	ctor.apply(obj, args);
	return obj;
}
Bridge.ParseInt = function(value, radix) {
	return parseInt(value, radix);
}
Bridge.ParseFloat = function(value) {
	return parseFloat(value);
}
Bridge.Get = function(scope, name) {
	return scope[name];
}
Bridge.Set = function(scope, name, value)
{
	scope[name] = value;
}
Bridge.Replace = function(str, pattern, replacement) {
	return str.replace(pattern, replacement);
}
Bridge.Match = function(str, pattern) {
	return str.match(pattern);
}
Bridge.CreateElement = function(tagname) {
	return document.createElement(tagname);
}
Bridge.CreateTextNode = function(text) {
	return document.createTextNode(text);
}
Bridge.Stringify = function(value) { return JSON.stringify(value); }
Bridge.Parse = function(text) { return JSON.parse(text); }
Bridge.NewRegExp = function(patern, flag) { return new RegExp(patern, flag); }
Bridge.goog_getMsg = function(str, opt_values) {
	return goog.getMsg(str, opt_values);
}
Bridge.goog_dom_createDom = function(v, o, t) {
	return goog.dom.createDom(v, o, t);
}
Bridge.goog_array_equals = function(a, b) {
	return goog.array.equals(a, b);
}
Bridge.goog_debug_getStacktrace = function() {
	return goog.debug.getStacktrace();
}
Bridge.show_stack = false;
Bridge.SetBlocks = function(instance)
{
	var i;
	var template = {
		template: instance,
		instance: null,
		type: instance.type,
		init: function() {
			this.instance = Bridge.instance.NewBlockInstance(this);
			if (Bridge.show_stack) {
				Bridge.show_stack = false;
				alert(Bridge.goog_debug_getStacktrace());
			}
		},
		mutationToDom: function(opt_paramIds) {
			return Bridge.instance.mutationToDom(this.instance, opt_paramIds);
		},
		domToMutation: function(xmlElement) {
			Bridge.instance.domToMutation(this.instance, xmlElement);
		},
		decompose: function(workspace) {
			return Bridge.instance.decompose(this.instance, workspace);
		},
		compose: function(containerBlock) {
			Bridge.instance.compose(this.instance, containerBlock);
		},
		saveConnections: function(containerBlock) {
			Bridge.instance.saveConnections(this.instance, containerBlock);
		},
		onchange: function(e) {
			Bridge.instance.onchange(this.instance, e);
		},
		customContextMenu: function(options) {
			Bridge.instance.customContextMenu(this.instance, options);
		},
		getProcedureCall: function() {
			return Bridge.instance.getProcedureCall(this.instance);
		},
		getProcedureDef: function() {
			return Bridge.instance.getProcedureDef(this.instance);
		},
		getVars: function() {
			return Bridge.instance.getVars(this.template);
		},
		renameProcedure: function(oldName, newName) {
			Bridge.instance.renameProcedure(this.instance, oldName, newName);
		},
		renameVar: function(oldName, newName) {
			Bridge.instance.renameVar(this.instance, oldName, newName);
		},
	};
	if (!Bridge.instance.isDefined(instance, ""mutationToDom"")) {
		delete template.mutationToDom;
	}
	if (!Bridge.instance.isDefined(instance, ""domToMutation"")) {
		delete template.domToMutation;
	}
	if (!Bridge.instance.isDefined(instance, ""decompose"")) {
		delete template.decompose;
	}
	if (!Bridge.instance.isDefined(instance, ""compose"")) {
		delete template.compose;
	}
	if (!Bridge.instance.isDefined(instance, ""saveConnections"")) {
		delete template.saveConnections;
	}
	if (!Bridge.instance.isDefined(instance, ""onchange"")) {
		delete template.onchange;
	}
	if (!Bridge.instance.isDefined(instance, ""customContextMenu"")) {
		delete template.customContextMenu;
	}
	if (!Bridge.instance.isDefined(instance, ""getProcedureCall"")) {
		delete template.getProcedureCall;
	}
	if (!Bridge.instance.isDefined(instance, ""getProcedureDef"")) {
		delete template.getProcedureDef;
	}
	if (!Bridge.instance.isDefined(instance, ""getVars"")) {
		delete template.getVars;
	}
	if (!Bridge.instance.isDefined(instance, ""renameProcedure"")) {
		delete template.renameProcedure;
	}
	if (!Bridge.instance.isDefined(instance, ""renameVar"")) {
		delete template.renameVar;
	}
	Blockly.Blocks[template.type] = template;
	return template;
}
Bridge.NewGenerator = function(name)
{
	return new Blockly.Generator(name);
}
Bridge.SetGenerator = function(generator)
{
	var i;
	var g = {
		instance: generator,
		init: function(workspace) {
			Bridge.instance.init(this.instance, workspace);
		},
		finish: function(code) {
			return Bridge.instance.finish(this.instance, code);
		},
		scrubNakedValue: function(line) {
			return Bridge.instance.scrubNakedValue(this.instance, line);
		},
		scrub_: function(block, code) {
			return Bridge.instance.scrub_(this.instance, block, code);
		},
	};
	var o = generator.instance;
	for (i in g) o[i] = g[i];
	Blockly[o.name_] = o;
	return o;
}
";

		internal static void SetDocument(HtmlDocument document, ScriptHost scriptHost)
		{
			Document = document;
			ScriptHost = scriptHost;
			Bridge = Document.InvokeScript("eval", new object[] { "Bridge" });
			Bridge.instance = scriptHost;
			Blockly.instance = scriptHost.blockly;
		}

		internal static dynamic NewObject()
		{
			return new Microsoft.JScript.JSObject();
		}

		internal static dynamic NewArray(params object[] items)
		{
			var a = new Microsoft.JScript.JSObject();
			foreach (var i in items)
				Microsoft.JScript.ArrayPrototype.push(a, i);
			return a;
		}

		internal static void Push(object a, object first, params object[] items)
		{
			Microsoft.JScript.ArrayPrototype.push(a, first);
			foreach (var i in items)
				Microsoft.JScript.ArrayPrototype.push(a, i);
		}

		internal static dynamic NewFunc(Delegate d)
		{
			System.Diagnostics.Debug.Assert(d != null);
			return Bridge.NewFunc(d);
		}

		internal static dynamic New(string name, object[] args)
		{
			var a = Script.NewArray();
			foreach (var i in args)
				Script.Push(a, i);
			return Bridge.New(name, a);
		}

		internal static int ParseInt(string value, int radix)
		{
			return (int)Bridge.ParseInt(value, radix);
		}

		internal static double ParseFloat(string value)
		{
			return (double)Bridge.ParseFloat(value);
		}

		internal static object Get(object scope, string name)
		{
			return Bridge.Get(scope, name);
		}

		internal static T Get<T>(object scope, string name)
		{
			var ret = Bridge.Get(scope, name);
			if ((ret == null) || (ret is DBNull))
				return default(T);
			return (T)ret;
		}

		internal static void Set(object scope, string name, object value)
		{
			Bridge.Set(scope, name, value);
		}

		internal static void Delete(Blockly.Connection connection)
		{
			Bridge.Delete(connection);
		}

		static Dictionary<string, Type> m_BlockTypes = new Dictionary<string, Type>();

		internal static void SetBlocks(Block block)
		{
			m_BlockTypes.Add(block.type, block.GetType());
			Bridge.SetBlocks(block);
		}

		internal static Block CreateBlock(dynamic target)
		{
			if ((target == null) || (target is DBNull))
				return null;

			var instance = Script.Get(target, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Block)instance;

			var ts = Script.Get<string>(target, "type");
			if ((ts == null) || (ts is DBNull))
				return null;

			var type = m_BlockTypes[ts];
			var ctor = type.GetConstructor(new Type[] { });
			var block = (Block)ctor.Invoke(new object[] { });
			block.instance = target;
			Script.Set(target, "instance", block);
			return block;
		}

		internal static dynamic NewGenerator(string name)
		{
			return Bridge.NewGenerator(name);
		}

		internal static void SetGenerator(Generator generator)
		{
			Bridge.SetGenerator(generator);
		}

		internal static string Replace(string str, dynamic instance, string dst)
		{
			return Bridge.Replace(str, instance, dst);
		}

		internal static object Match(string str, dynamic instance)
		{
			return Bridge.Match(str, instance);
		}

		internal static Element CreateElement(string tagname)
		{
			return Element.Create(Bridge.CreateElement(tagname));
		}

		internal static Element CreateTextNode(string v)
		{
			return Element.Create(Bridge.CreateTextNode(v));
		}

		internal static void PreventDefault(Event @event)
		{
			Bridge.PreventDefault(@event.instance);
		}

		internal static void StopPropagation(Event @event)
		{
			Bridge.StopPropagation(@event.instance);
		}

		internal static string Stringify(object value)
		{
			return Bridge.Stringify(value);
		}

		internal static object Parse(string text)
		{
			return Bridge.Parse(text);
		}

		internal static dynamic NewRegExp(string pattern, string flag)
		{
			return Bridge.NewRegExp(pattern, flag);
		}

		internal static string goog_getMsg(string str, object opt_values)
		{
			return Bridge.goog_getMsg(str, opt_values);
		}

		internal static object goog_dom_createDom(string v, object o, string t)
		{
			return Bridge.goog_dom_createDom(v, o, t);
		}

		internal static object goog_dom_createDom(string v, object o, Element t)
		{
			return Bridge.goog_dom_createDom(v, o, t.instance);
		}

		internal static bool goog_array_equals(Array a, Array b)
		{
			return Bridge.goog_array_equals(a, b);
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

		internal static bool HasMember(object obj, string name)
		{
			var a = GetFieldNames(obj);
			return a.Contains(name);
		}

		internal static object ProceduresGetDefinition(string name, dynamic workspace)
		{
			return ScriptHost.ProceduresGetDefinition(name, workspace);
		}

		internal static void ProceduresMutateCallers(dynamic block)
		{
			ScriptHost.ProceduresMutateCallers(block);
		}

		internal static object XmlWorkspaceToDom(dynamic workspace)
		{
			return ScriptHost.XmlWorkspaceToDom(workspace);
		}

		internal static object XmlBlockToDomWithXY(dynamic block)
		{
			return ScriptHost.XmlBlockToDomWithXY(block);
		}

		internal static object XmlBlockToDom(dynamic block)
		{
			return ScriptHost.XmlBlockToDom(block);
		}

		internal static string XmlDomToText(dynamic dom)
		{
			return ScriptHost.XmlDomToText(dom);
		}

		internal static string XmlDomToPrettyText(dynamic dom)
		{
			return ScriptHost.XmlDomToPrettyText(dom);
		}

		internal static object XmlTextToDom(string text)
		{
			return ScriptHost.XmlTextToDom(text);
		}

		internal static void XmlDomToWorkspace(dynamic xml, dynamic workspace)
		{
			ScriptHost.XmlDomToWorkspace(xml, workspace);
		}

		internal static dynamic XmlDomToBlock(dynamic block, dynamic workspace)
		{
			return ScriptHost.XmlDomToBlock(block, workspace);
		}

		internal static void XmlDeleteNext(dynamic block)
		{
			ScriptHost.XmlDeleteNext(block);
		}

		internal static void EventsEnable()
		{
			ScriptHost.EventsEnable();
		}

		internal static void EventsDisable()
		{
			ScriptHost.EventsDisable();
		}

		internal static object EventsGetDescendantIds_(dynamic block)
		{
			return ScriptHost.EventsGetDescendantIds_(block);
		}

		internal static void EventsSetGroup(string group)
		{
			ScriptHost.EventsSetGroup(group);
		}

		internal static dynamic ContextMenuCallbackFactory(dynamic block, dynamic xml)
		{
			return ScriptHost.ContextMenuCallbackFactory(block, xml);
		}

		internal static Blockly.Workspace WorkspaceGetById(string id)
		{
			return Blockly.WorkspaceSvg.Create(ScriptHost.WorkspaceGetById(id));
		}

		internal static bool NamesEquals(string name1, string name2)
		{
			return ScriptHost.NamesEquals(name1, name2);
		}

		internal static bool MutatorReconnect(dynamic connectionChild, dynamic block, string inputName)
		{
			return ScriptHost.MutatorReconnect(connectionChild, block, inputName);
		}

		internal static Blockly.Events.Abstract CreateEvent(dynamic ev)
		{
			switch ((string)ev.type) {
			case Blockly.Events.CREATE:
				return new Blockly.Events.Create(ev);
			case Blockly.Events.DELETE:
				return new Blockly.Events.Delete(ev);
			case Blockly.Events.CHANGE:
				return new Blockly.Events.Change(ev);
			case Blockly.Events.MOVE:
				return new Blockly.Events.Move(ev);
			case Blockly.Events.UI:
				return new Blockly.Events.Ui(ev);
			default:
				return new Blockly.Events.Abstract(ev);
			}
		}

		internal static Blockly.Input CreateInput(dynamic target)
		{
			if ((target == null) || (target is DBNull))
				return null;

			var instance = Script.Get(target, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Blockly.Input)instance;

			var input = new Blockly.Input(target);
			Script.Set(target, "instance", input);
			return input;
		}

		internal static Blockly.Field CreateField(dynamic ret)
		{
			if ((ret == null) || (ret is DBNull))
				return null;

			var instance = Script.Get(ret, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Blockly.Field)instance;

			var field = new Blockly.Field(ret);
			Script.Set(ret, "instance", field);
			return field;
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

	static class StringExtention
	{
		public static string ToLowerCase(this string str)
		{
			return str.ToLower();
		}

		public static string Replace(this string str, Text.RegularExpressions.Regex regex, string dst)
		{
			return Script.Replace(str, regex.instance, dst);
		}

		public static object Match(this string str, Text.RegularExpressions.Regex regex)
		{
			return Script.Match(str, regex.instance);
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
	}

	class ExternalAttribute : Attribute
	{

	}

	class FieldPropertyAttribute : Attribute
	{

	}

	class NameAttribute : Attribute
	{
		public NameAttribute(bool charcase)
		{
		}

		public NameAttribute(string name)
		{

		}
	}

	class Location
	{
		internal dynamic instance;

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

	public class BlockList : IEnumerable<Block>
	{
		internal dynamic instance;

		public BlockList(object instance)
		{
			this.instance = instance;
		}

		public Block this[int index] {
			get { return Script.CreateBlock(Script.Get(instance, index.ToString())); }
			set { Script.Set(instance, index.ToString(), value == null ? null : value.instance); }
		}

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		public class Enumerator : IEnumerator<Block>
		{
			private BlockList blockList;
			private int index = -1;

			public Enumerator(BlockList blockList)
			{
				this.blockList = blockList;
			}

			public Block Current {
				get { return blockList[index]; }
			}

			object IEnumerator.Current {
				get { return blockList[index]; }
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				index++;
				return index < blockList.Length;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		public IEnumerator<Block> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public static implicit operator BlockList(Block[] value)
		{
			var ary = Script.NewArray();
			foreach (var i in value) {
				Script.Push(ary, i);
			}
			return new BlockList(ary);
		}
	}

	public class FieldList : IEnumerable<Blockly.Field>
	{
		internal dynamic instance;

		public FieldList(object instance)
		{
			this.instance = instance;
		}

		public Blockly.Field this[int index] { get { return Script.CreateField(Script.Get(instance, index.ToString())); } }

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		public class Enumerator : IEnumerator<Blockly.Field>
		{
			private FieldList fieldList;
			private int index = -1;

			public Enumerator(FieldList fieldList)
			{
				this.fieldList = fieldList;
			}

			public Blockly.Field Current {
				get { return fieldList[index]; }
			}

			object IEnumerator.Current {
				get { return fieldList[index]; }
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				index++;
				return index < fieldList.Length;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		public IEnumerator<Blockly.Field> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}

	public class InputList : IEnumerable<Blockly.Input>
	{
		internal dynamic instance;

		public InputList(object instance)
		{
			this.instance = instance;
		}

		public Blockly.Input this[int index] { get { return Script.CreateInput(Script.Get(instance, index.ToString())); } }

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		public class Enumerator : IEnumerator<Blockly.Input>
		{
			private InputList inputList;
			private int index = -1;

			public Enumerator(InputList inputList)
			{
				this.inputList = inputList;
			}

			public Blockly.Input Current {
				get { return inputList[index]; }
			}

			object IEnumerator.Current {
				get { return inputList[index]; }
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				index++;
				return index < inputList.Length;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		public IEnumerator<Blockly.Input> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}

namespace Bridge.Html5
{
	public class Node
	{
		internal dynamic instance;

		public Node(object instance)
		{
			this.instance = instance;
		}

		public string NodeName { get { return instance.nodeName; } }
		public NodeList ChildNodes { get { return new NodeList(instance.childNodes); } }
	}

	public class NodeList
	{
		internal dynamic instance;

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

		internal static Element Create(dynamic instance)
		{
			if ((instance == null) || (instance is DBNull))
				return null;
			return new Element(instance);
		}

		internal string GetAttribute(string name)
		{
			var ret = instance.getAttribute(name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return (string)ret;
		}

		internal void SetAttribute(string name, string value)
		{
			instance.setAttribute(name, value);
		}

		internal void AppendChild(Element parameter)
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

		internal static Element CreateTextNode(string v)
		{
			return Script.CreateTextNode(v);
		}
	}

	public class SVGMatrix
	{
		internal dynamic instance;

		public SVGMatrix(dynamic instance)
		{
			this.instance = instance;
		}
	}

	public class Event
	{
		internal dynamic instance;

		public Event(object instance)
		{
			this.instance = instance;
		}

		internal void PreventDefault()
		{
			Script.PreventDefault(this);
		}

		internal void StopPropagation()
		{
			Script.StopPropagation(this);
		}
	}

	public class JSON
	{
		internal static string Stringify(string[] value)
		{
			//return Script.Stringify(value);
			return Codeplex.Data.DynamicJson.Serialize(value);
		}

		internal static object Parse(string text)
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
		internal dynamic instance;

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
