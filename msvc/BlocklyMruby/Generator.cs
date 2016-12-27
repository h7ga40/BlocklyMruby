using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bridge;
using Bridge.Text.RegularExpressions;

namespace BlocklyMruby
{
	[ComVisible(true)]
	public class Generator
	{
		public dynamic instance;

		internal Generator(object instance)
		{
			this.instance = instance;
		}

		/**
		 * Class for a code generator that translates the blocks into a language.
		 * @param {string} name Language name of this generator.
		 * @constructor
		 */
		[Name(false), FieldProperty]
		public Regex FUNCTION_NAME_PLACEHOLDER_REGEXP_ { get { return new Regex(Script.Get(instance, "FUNCTION_NAME_PLACEHOLDER_REGEXP_")); } }

		/**
		 * Arbitrary code to inject into locations that risk causing infinite loops.
		 * Any instances of '%1' will be replaced by the block ID that failed.
		 * E.g. '  checkTimeout(%1)\n'
		 * @type ?string
		 */
		[Name(false), FieldProperty]
		public string INFINITE_LOOP_TRAP {
			get {
				var ret = instance.INFINITE_LOOP_TRAP;
				if ((ret == null) || (ret is DBNull))
					return null;
				return ret;
			}
		}

		/**
		 * The method of indenting.  Defaults to two spaces, but language generators
		 * may override this to increase indent or change to tabs.
		 * @type {string}
		 */
		[Name(false), FieldProperty]
		public string INDENT {
			get { return Script.Get(instance, "INDENT"); }
			set { Script.Set(instance, "INDENT", value); }
		}

		/**
		 * Maximum length for a comment before wrapping.  Does not account for
		 * indenting level.
		 * @type {number}
		 */
		[Name(false), FieldProperty]
		public int COMMENT_WRAP { get { return instance.COMMENT_WRAP; } }

		public string name { get { return Script.Get(instance, "name_"); } }

		[External]
		public Generator(string name)
		{
			instance = Script.NewGenerator(name);

			var methods = GetType().GetMethods();
			foreach (var m in methods) {
				var param = m.GetParameters();
				if (param.Length != 1)
					continue;

				var paramType = param[0].ParameterType;
				if (paramType != typeof(Block) && !paramType.IsSubclassOf(typeof(Block)))
					continue;

				var retType = m.ReturnType;
				if (retType != typeof(object) && retType != typeof(string) && retType != typeof(object[]))
					continue;

				Script.Set(instance, m.Name, Script.NewFunc(new Func<dynamic, object>((block_) => {
					object ret = null;
					try {
						var block = (Block)block_.instance;
						ret = m.Invoke(this, new object[] { block });
						if (ret == null)
							return null;
						if (ret.GetType().IsArray) {
							var ary = (object[])ret;
							if ((ary.Length != 2) || ary[0].GetType() != typeof(string) || ary[1].GetType() != typeof(int))
								throw new InvalidOperationException();
							var json = Codeplex.Data.DynamicJson.Serialize(ary);
							ret = Script.Parse(json);
						}
						return ret;
					}
					catch (Exception e) {
						Console.WriteLine(e.ToString());
					}
					return ret;
				})));
			}
		}

		/// <summary>
		/// Generate code for all blocks in the workspace to the specified language.
		/// </summary>
		/// <param name="workspace">Workspace to generate code from.</param>
		/// <returns>Generated code.</returns>
		[External]
		internal string workspaceToCode(Blockly.Workspace workspace)
		{
			return instance.workspaceToCode(workspace.instance);
		}

		/// <summary>
		/// Prepend a common prefix onto each line of code.
		/// </summary>
		/// <param name="text">text The lines of code.</param>
		/// <param name="prefix">prefix The common prefix.</param>
		/// <returns>The prefixed lines of code.</returns>
		[External]
		internal string prefixLines(string text, string prefix)
		{
			return instance.prefixLines(text, prefix);
		}

		/// <summary>
		/// Recursively spider a tree of blocks, returning all their comments.
		/// </summary>
		/// <param name="block">block The block from which to start spidering.</param>
		/// <returns>Concatenated list of comments.</returns>
		[External]
		internal string allNestedComments(Block block)
		{
			return instance.allNestedComments(block.instance);
		}

		/// <summary>
		/// Generate code for the specified block (and attached blocks).
		/// </summary>
		/// <param name="block">block The block to generate code for.</param>
		/// <returns>For statement blocks, the generated code.
		/// For value blocks, an array containing the generated code and an
		/// operator order value.Returns '' if block is null.</returns>
		[External]
		internal string blockToCode(Block block)
		{
			return instance.blockToCode(block);
		}

		/// <summary>
		/// Generate code representing the specified value input.
		/// </summary>
		/// <param name="block">The block containing the input.</param>
		/// <param name="name">The name of the input.</param>
		/// <param name="outerOrder">The maximum binding strength (minimum order value)</param>
		/// <returns>Generated code or '' if no blocks are connected or the
		/// specified input does not exist.</returns>
		[External]
		internal string valueToCode(Block block, string name, int outerOrder)
		{
			return instance.valueToCode(block.instance, name, outerOrder);
		}

		/// <summary>
		/// Generate code representing the statement.  Indent the code.
		/// </summary>
		/// <param name="block">The block containing the input.</param>
		/// <param name="name">The name of the input.</param>
		/// <returns>Generated code or '' if no blocks are connected.</returns>
		[External]
		internal string statementToCode(Block block, string name)
		{
			return instance.statementToCode(block.instance, name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="branch"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[External]
		internal string addLoopTrap(string branch, string id)
		{
			return instance.addLoopTrap(branch, id);
		}

		/// <summary>
		/// Add one or more words to the list of reserved words for this language.
		/// </summary>
		/// <param name="words">Comma-separated list of words to add to the list.</param>
		[External]
		internal void addReservedWords(string words)
		{
			instance.addReservedWords(words);
		}

		protected Dictionary<string, string> definitions_;
		protected Dictionary<string, string> functionNames_;
		protected Blockly.Names variableDB_;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desiredName"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		[External]
		internal string provideFunction_(string desiredName, string[] code)
		{
			if (!this.definitions_.ContainsKey(desiredName)) {
				var functionName = this.variableDB_.getDistinctName(desiredName, Blockly.Procedures.NAME_TYPE);
				this.functionNames_[desiredName] = functionName;
				var codeText = code.Join("\n").Replace(
					this.FUNCTION_NAME_PLACEHOLDER_REGEXP_, functionName);
				// Change all '  ' indents into the desired indent.
				// To avoid an infinite loop of replacements, change all indents to '\0'
				// character first, then replace them all with the indent.
				// We are assuming that no provided functions contain a literal null char.
				string oldCodeText = null;
				while (oldCodeText != codeText) {
					oldCodeText = codeText;
					codeText = codeText.Replace(new Regex("^((  )*)  ", "gm"), "$1\0");
				}
				codeText = codeText.Replace(new Regex("", "g"), this.INDENT);
				this.definitions_[desiredName] = codeText;
			}
			return this.functionNames_[desiredName];
		}
	}
}
