using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	public class BlocklyView : WebConsole
	{
		public Blockly Blockly { get; private set; }

		public BlocklyView()
		{
			Open(new ResourceReader("BlocklyMrubyRes"), new BlocklyScriptingHost(this));

			Application.Idle += Application_Idle;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			Navigate("blockly.html");
		}

		protected override bool DocumentCompleted1(Uri Url)
		{
			if (Url.Scheme == "about")
				return false;

			InvokeScript("load_blockly");
			return ((BlocklyScriptingHost)ObjectForScripting).blockly != null;
		}

		protected override Script NewScript(ScriptingHost scriptingHost)
		{
			var script = new BlocklyScript((BlocklyScriptingHost)scriptingHost);
			Blockly = script.Blockly;
			return script;
		}

		protected override void DocumentCompleted(Uri Url)
		{
			if (Blockly == null)
				return;

			base.DocumentCompleted(Url);

			App.Init(Blockly);
			InvokeScript("start_blockly");
			App.Init2();
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class BlocklyScriptingHost : ScriptingHost
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
		internal new BlocklyScript Script { get { return (BlocklyScript)base.Script; } }
		internal Blockly Blockly { get { return ((BlocklyView)View).Blockly; } }

		public BlocklyScriptingHost(BlocklyView view)
			: base(view)
		{
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

		public Workspace ToWorkspace(dynamic workspace)
		{
			return WorkspaceSvg.Create(Blockly, workspace);
		}

		public Block ToBlock(dynamic block)
		{
			var instance = Script.Get(block, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Block)instance;
			return Script.CreateBlock(block);
		}

		public Abstract ToEvent(dynamic ev)
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
			var options = new ContextMenuOptionList(Script, options_);
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
				var a = Bridge.Script.NewArray();
				foreach (var i in ret) {
					if (i is string[])
						Bridge.Script.Push(a, Bridge.Script.NewArray(i));
					else
						Bridge.Script.Push(a, i);
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
				var a = Bridge.Script.NewArray();
				foreach (var i in ret)
					Bridge.Script.Push(a, i);
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

		public bool names_equals(object name1, object name2)
		{
			return Names.equals((string)name1, (string)name2);
		}

		public object procedures_flyoutCategory(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Procedures.flyoutCategory(workspace);
			var result = Bridge.Script.NewArray();
			foreach (var e in ret) {
				Bridge.Script.Push(result, e.instance);
			}
			return result;
		}

		public object variables_flyoutCategory(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Variables.flyoutCategory(workspace);
			var result = Bridge.Script.NewArray();
			foreach (var e in ret) {
				Bridge.Script.Push(result, e.instance);
			}
			return result;
		}

		public object variables_allUsedVariablesBlock(object block_)
		{
			var block = ToBlock(block_);
			var ret = Blockly.Variables.allUsedVariables(block);
			var result = Bridge.Script.NewArray();
			foreach (var s in ret) {
				Bridge.Script.Push(result, s);
			}
			return result;
		}

		public object variables_allUsedVariablesWorkspace(object workspace_)
		{
			var workspace = ToWorkspace(workspace_);
			var ret = Blockly.Variables.allUsedVariables(workspace);
			var result = Bridge.Script.NewArray();
			foreach (var s in ret) {
				Bridge.Script.Push(result, s);
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
		dynamic Blockly_Msg;
		Dictionary<string, Type> m_BlockTypes = new Dictionary<string, Type>();
		public new BlocklyScriptingHost ScriptHost { get { return (BlocklyScriptingHost)base.ScriptHost; } }
		public Blockly Blockly { get; }

		public BlocklyScript(BlocklyScriptingHost scriptHost)
			: base(scriptHost)
		{
			Bridge = ScriptHost.bridge;
			Blockly_Msg = Get(ScriptHost.blockly, "Msg");
			Blockly = new Blockly(this, ScriptHost.blockly);
		}

		public void SetMessage(Type type)
		{
			var msg = Blockly_Msg;

			foreach (var f in type.GetFields()) {
				if (f.FieldType != typeof(string))
					continue;

				if (!f.IsPublic || !f.IsStatic || !f.IsLiteral)
					continue;

				Set(msg, f.Name, f.GetValue(null));
			}
		}

		internal void SetBlocks(Block block)
		{
			m_BlockTypes.Add(block.type, block.GetType());
			Bridge.SetBlocks(block);
		}

		internal Block CreateBlock(dynamic target)
		{
			if ((target == null) || (target is DBNull))
				return null;

			var instance = Get(target, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Block)instance;

			var ts = Get<string>(target, "type");
			if ((ts == null) || (ts is DBNull))
				return null;

			var type = m_BlockTypes[ts];
			var ctor = type.GetConstructor(new Type[] { typeof(Blockly) });
			var block = (Block)ctor.Invoke(new object[] { Blockly });
			block.instance = target;
			Set(target, "instance", block);
			return block;
		}

		internal void SetGenerator()
		{
			ScriptHost.procedures = Get(ScriptHost.blockly, "Procedures");
			ScriptHost.variables = Get(ScriptHost.blockly, "Variables");
			Set(ScriptHost.procedures, "NAME_TYPE", Procedures.NAME_TYPE);
			Set(ScriptHost.variables, "NAME_TYPE", Variables.NAME_TYPE);
		}

		internal string goog_getMsg(string str, object opt_values)
		{
			return Bridge.goog_getMsg(str, opt_values);
		}

		internal object goog_dom_createDom(string v, object o, string t)
		{
			return Bridge.goog_dom_createDom(v, o, t);
		}

		internal object goog_dom_createDom(string v, object o, Element t)
		{
			return Bridge.goog_dom_createDom(v, o, t.instance);
		}

		internal bool goog_array_equals(Array a, Array b)
		{
			return Bridge.goog_array_equals(a, b);
		}

		internal object ProceduresGetDefinition(string name, dynamic workspace)
		{
			return ScriptHost.ProceduresGetDefinition(name, workspace);
		}

		internal void ProceduresMutateCallers(dynamic block)
		{
			ScriptHost.ProceduresMutateCallers(block);
		}

		internal object XmlWorkspaceToDom(dynamic workspace)
		{
			return ScriptHost.XmlWorkspaceToDom(workspace);
		}

		internal object XmlBlockToDomWithXY(dynamic block)
		{
			return ScriptHost.XmlBlockToDomWithXY(block);
		}

		internal object XmlBlockToDom(dynamic block)
		{
			return ScriptHost.XmlBlockToDom(block);
		}

		internal string XmlDomToText(dynamic dom)
		{
			return ScriptHost.XmlDomToText(dom);
		}

		internal string XmlDomToPrettyText(dynamic dom)
		{
			return ScriptHost.XmlDomToPrettyText(dom);
		}

		internal object XmlTextToDom(string text)
		{
			return ScriptHost.XmlTextToDom(text);
		}

		internal void XmlDomToWorkspace(dynamic xml, dynamic workspace)
		{
			ScriptHost.XmlDomToWorkspace(xml, workspace);
		}

		internal dynamic XmlDomToBlock(dynamic block, dynamic workspace)
		{
			return ScriptHost.XmlDomToBlock(block, workspace);
		}

		internal void XmlDeleteNext(dynamic block)
		{
			ScriptHost.XmlDeleteNext(block);
		}

		internal void EventsEnable()
		{
			ScriptHost.EventsEnable();
		}

		internal void EventsDisable()
		{
			ScriptHost.EventsDisable();
		}

		internal object EventsGetDescendantIds_(dynamic block)
		{
			return ScriptHost.EventsGetDescendantIds_(block);
		}

		internal void EventsSetGroup(string group)
		{
			ScriptHost.EventsSetGroup(group);
		}

		internal bool EventsGetRecordUndo()
		{
			return ScriptHost.EventsGetRecordUndo.call(null);
		}

		internal void EventsSetRecordUndo(bool value)
		{
			ScriptHost.EventsSetRecordUndo.call(null, value);
		}

		internal void EventsFire(object ev)
		{
			ScriptHost.EventsFire(ev);
		}

		internal dynamic ContextMenuCallbackFactory(dynamic block, dynamic xml)
		{
			return ScriptHost.ContextMenuCallbackFactory(block, xml);
		}

		internal Workspace WorkspaceGetById(string id)
		{
			return WorkspaceSvg.Create(Blockly, ScriptHost.WorkspaceGetById(id));
		}

		internal bool NamesEquals(string name1, string name2)
		{
			return ScriptHost.NamesEquals(name1, name2);
		}

		internal bool MutatorReconnect(dynamic connectionChild, dynamic block, string inputName)
		{
			return ScriptHost.MutatorReconnect(connectionChild, block, inputName);
		}

		internal Abstract CreateEvent(dynamic ev)
		{
			switch ((string)ev.type) {
			case Events.CREATE:
				return new Create(Blockly, ev);
			case Events.DELETE:
				return new Delete(Blockly, ev);
			case Events.CHANGE:
				return new Change(Blockly, ev);
			case Events.MOVE:
				return new Move(Blockly, ev);
			case Events.UI:
				return new Ui(Blockly, ev);
			default:
				return new Abstract(Blockly, ev);
			}
		}

		internal Input CreateInput(dynamic target)
		{
			if ((target == null) || (target is DBNull))
				return null;

			var instance = Get(target, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Input)instance;

			var input = new Input(Blockly, target);
			Set(target, "instance", input);
			return input;
		}

		internal Field CreateField(dynamic ret)
		{
			if ((ret == null) || (ret is DBNull))
				return null;

			var instance = Get(ret, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Field)instance;

			var field = new Field(Blockly, ret);
			Set(ret, "instance", field);
			return field;
		}
	}


	public class BlockList : IEnumerable<Block>
	{
		internal dynamic instance;
		public static BlocklyScript Script { get; private set; }

		public BlockList(BlocklyScript script, object instance)
		{
			Script = script;
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
			var ary = Bridge.Script.NewArray();
			foreach (var i in value) {
				Bridge.Script.Push(ary, i);
			}
			return new BlockList(Script, ary);
		}
	}

	public class FieldList : IEnumerable<Field>
	{
		internal dynamic instance;
		public static BlocklyScript Script { get; private set; }

		public FieldList(BlocklyScript script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public Field this[int index] { get { return Script.CreateField(Script.Get(instance, index.ToString())); } }

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		public class Enumerator : IEnumerator<Field>
		{
			private FieldList fieldList;
			private int index = -1;

			public Enumerator(FieldList fieldList)
			{
				this.fieldList = fieldList;
			}

			public Field Current {
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

		public IEnumerator<Field> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}

	public class InputList : IEnumerable<Input>
	{
		internal dynamic instance;
		public static BlocklyScript Script { get; private set; }

		public InputList(BlocklyScript script, object instance)
		{
			Script = script;
			this.instance = instance;
		}

		public Input this[int index] { get { return Script.CreateInput(Script.Get(instance, index.ToString())); } }

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		public class Enumerator : IEnumerator<Input>
		{
			private InputList inputList;
			private int index = -1;

			public Enumerator(InputList inputList)
			{
				this.inputList = inputList;
			}

			public Input Current {
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

		public IEnumerator<Input> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
