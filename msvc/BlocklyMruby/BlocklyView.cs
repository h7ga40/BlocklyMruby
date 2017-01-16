using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	[System.Runtime.InteropServices.ComVisible(true)]
	public class BlocklyScriptHost : ScriptHost
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
		public dynamic EventsGetRecordUndo;
		public dynamic EventsSetRecordUndo;
		public dynamic EventsFire;
		public dynamic ContextMenuCallbackFactory;
		public dynamic WorkspaceGetById;
		public dynamic NamesEquals;
		public dynamic MutatorReconnect;
		public dynamic procedures;
		public dynamic variables;

		public dynamic GetBlockInstance(object block_)
		{
			var block = (Block)block_;
			return block.instance;
		}

		public object NewBlockInstance(object instance)
		{
			var block = BlocklyScript.CreateBlock(instance);
			System.Diagnostics.Debug.Assert(block != null);
			((dynamic)block).init();
			return block;
		}

		public Blockly.Workspace ToWorkspace(dynamic workspace)
		{
			return Blockly.WorkspaceSvg.Create(workspace);
		}

		public Block ToBlock(dynamic block)
		{
			var instance = Script.Get(block, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Block)instance;
			return BlocklyScript.CreateBlock(block);
		}

		public Blockly.Events.Abstract ToEvent(dynamic ev)
		{
			return BlocklyScript.CreateEvent(ev);
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

		public void dump_svg_element(object obj, string text, object element)
		{
			var block = BlocklyScript.CreateBlock(obj);
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

		public bool names_equals(object name1, object name2)
		{
			return Names.equals((string)name1, (string)name2);
		}

		public object procedures_flyoutCategory(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Procedures.flyoutCategory(workspace);
			var result = Script.NewArray();
			foreach (var e in ret) {
				Script.Push(result, e.instance);
			}
			return result;
		}

		public object variables_flyoutCategory(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Variables.flyoutCategory(workspace);
			var result = Script.NewArray();
			foreach (var e in ret) {
				Script.Push(result, e.instance);
			}
			return result;
		}

		public object variables_allUsedVariablesBlock(object block_)
		{
			var block = ToBlock(block_);
			var ret = Blockly.Variables.allUsedVariables(block);
			var result = Script.NewArray();
			foreach (var s in ret) {
				Script.Push(result, s);
			}
			return result;
		}

		public object variables_allUsedVariablesWorkspace(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Variables.allUsedVariables(workspace);
			var result = Script.NewArray();
			foreach (var s in ret) {
				Script.Push(result, s);
			}
			return result;
		}

		public string variables_generateUniqueName(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			return Blockly.Variables.generateUniqueName(workspace);
		}

		public void variables_promptName(object promptText, object defaultText, object callback)
		{
			Blockly.Variables.promptName(promptText.ToString(), defaultText.ToString(), (a) => {
				((dynamic)callback).call(callback, a);
			});
		}
	}

	public class BlocklyScript : Script
	{
		internal static HtmlDocument Document;
		public new static BlocklyScriptHost ScriptHost;

		internal static void SetDocument(HtmlDocument document, BlocklyScriptHost scriptHost)
		{
			Document = document;
			ScriptHost = scriptHost;
			Bridge = Document.InvokeScript("eval", new object[] { "Bridge" });
			Bridge.instance = scriptHost;
			Blockly.instance = scriptHost.blockly;
		}

		internal static void SetMessage(Type type)
		{
			var msg = Document.InvokeScript("eval", new object[] { "Blockly.Msg" });

			foreach (var f in type.GetFields()) {
				if (f.FieldType != typeof(string))
					continue;

				if (!f.IsPublic || !f.IsStatic || !f.IsLiteral)
					continue;

				Script.Set(msg, f.Name, f.GetValue(null));
			}
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

		internal static void SetGenerator()
		{
			ScriptHost.procedures = Script.Get(ScriptHost.blockly, "Procedures");
			ScriptHost.variables = Script.Get(ScriptHost.blockly, "Variables");
			Script.Set(ScriptHost.procedures, "NAME_TYPE", Blockly.Procedures.NAME_TYPE);
			Script.Set(ScriptHost.variables, "NAME_TYPE", Blockly.Variables.NAME_TYPE);
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

		internal static bool EventsGetRecordUndo()
		{
			return ScriptHost.EventsGetRecordUndo.call(null);
		}

		internal static void EventsSetRecordUndo(bool value)
		{
			ScriptHost.EventsSetRecordUndo.call(null, value);
		}

		internal static void EventsFire(object ev)
		{
			ScriptHost.EventsFire(ev);
		}

		internal static dynamic ContextMenuCallbackFactory(dynamic block, dynamic xml)
		{
			return ScriptHost.ContextMenuCallbackFactory(block, xml);
		}
	}

	public class BlockList : IEnumerable<Block>
	{
		internal dynamic instance;

		public BlockList(object instance)
		{
			this.instance = instance;
		}

		public Block this[int index] {
			get { return BlocklyScript.CreateBlock(Script.Get(instance, index.ToString())); }
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

		public Blockly.Field this[int index] { get { return BlocklyScript.CreateField(Script.Get(instance, index.ToString())); } }

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

		public Blockly.Input this[int index] { get { return BlocklyScript.CreateInput(Script.Get(instance, index.ToString())); } }

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
