/**
 * @license
 * Visual Blocks Editor
 *
 * Copyright 2012 Google Inc.
 * https://developers.google.com/blockly/
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @fileoverview Utility functions for generating executable code from
 * Blockly code.
 * @author fraser@google.com (Neil Fraser)
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Bridge;
using Bridge.Text.RegularExpressions;

namespace BlocklyMruby
{
	[ComVisible(true)]
	public abstract class Generator
	{
		internal Generator(object instance)
		{
		}

		public MethodInfo this[string name] {
			get {
				var m = GetType().GetMethod(name);
				var p = m.GetParameters();
				if ((p.Length != 1) || !p[0].ParameterType.IsSubclassOf(typeof(Block)))
					return null;
				var type = m.ReturnParameter.ParameterType;
				if ((type != typeof(object[])) && (type != typeof(string)) && (type == typeof(object))) {
					return null;
				}

				return m;
			}
		}

		public string name_;

		/// <summary>
		/// Class for a code generator that translates the blocks into a language.
		/// </summary>
		/// <param name="name">Language name of this generator.</param>
		public Generator(string name)
		{
			this.name_ = name;
			this.FUNCTION_NAME_PLACEHOLDER_REGEXP_ =
				new Regex(this.FUNCTION_NAME_PLACEHOLDER_, "g");
		}

		/// <summary>
		// Category to separate generated function names from variables and procedures.
		/// </summary>
		public static string NAME_TYPE = "generated_function";

		/// <summary>
		/// Arbitrary code to inject into locations that risk causing infinite loops.
		/// Any instances of '%1' will be replaced by the block ID that failed.
		/// E.g. '  checkTimeout(%1);\n'
		/// </summary>
		public string INFINITE_LOOP_TRAP = null;

		/// <summary>
		/// Arbitrary code to inject before every statement.
		/// Any instances of '%1' will be replaced by the block ID of the statement.
		/// E.g. 'highlight(%1);\n'
		/// </summary>
		public string STATEMENT_PREFIX = null;

		/// <summary>
		/// The method of indenting.  Defaults to two spaces, but language generators
		/// may override this to increase indent or change to tabs.
		/// </summary>
		public string INDENT = "  ";

		/// <summary>
		/// Maximum length for a comment before wrapping.  Does not account for
		/// indenting level.
		/// </summary>
		public int COMMENT_WRAP = 60;

		/// <summary>
		/// List of outer-inner pairings that do NOT require parentheses.
		/// </summary>
		public List<int[]> ORDER_OVERRIDES = new List<int[]>();

		public abstract void init(Blockly.Workspace workspace);
		public abstract string finish(string code);
		public abstract string scrubNakedValue(string line);

		/// <summary>
		/// Generate code for all blocks in the workspace to the specified language.
		/// </summary>
		/// <param name="workspace">workspace Workspace to generate code from.</param>
		/// <returns>Generated code.</returns>
		public string workspaceToCode(Blockly.Workspace workspace)
		{
			if (workspace == null) {
				// Backwards compatibility from before there could be multiple workspaces.
				Script.console_warn("No workspace specified in workspaceToCode call.  Guessing.");
				workspace = Blockly.getMainWorkspace();
			}
			var codes = new List<string>();
			this.init(workspace);
			var blocks = workspace.getTopBlocks(true);
			Block block;
			for (var x = 0; (block = blocks[x]) != null; x++) {
				var line = this.blockToCode(block);
				if (line is object[]) {
					// Value blocks return tuples of code and operator order.
					// Top-level blocks don't care about operator order.
					line = ((object[])line)[0];
				}
				if (line != null) {
					if (block.outputConnection != null/*&& this.scrubNakedValue*/) {
						// This block is a naked value.  Ask the language's code generator if
						// it wants to append a semicolon, or something.
						line = this.scrubNakedValue((string)line);
					}
					codes.Add((string)line);
				}
			}
			var code = String.Join("\n", codes);  // Blank line between each section.
			code = this.finish(code);
			// Final scrubbing of whitespace.
			code = code.Replace(new Regex(@"^\s+\n"), "");
			code = code.Replace(new Regex(@"\n\s+$"), "\n");
			code = code.Replace(new Regex(@"[ \t]+\n", "g"), "\n");
			return code;
		}

		// The following are some helpful functions which can be used by multiple
		// languages.

		/// <summary>
		/// Prepend a common prefix onto each line of code.
		/// </summary>
		/// <param name="text">The lines of code.</param>
		/// <param name="prefix">The common prefix.</param>
		/// <returns>The prefixed lines of code.</returns>
		public string prefixLines(string text, string prefix)
		{
			return prefix + text.Replace(new Regex("(?!\n$)\n", "g"), "\n" + prefix);
		}

		/// <summary>
		/// Recursively spider a tree of blocks, returning all their comments.
		/// </summary>
		/// <param name="block">The block from which to start spidering.</param>
		/// <returns>Concatenated list of comments.</returns>
		public string allNestedComments(Block block)
		{
			var comments = new List<string>();
			var blocks = block.getDescendants();
			for (var i = 0; i < blocks.Length; i++) {
				var comment = blocks[i].getCommentText();
				if (comment != null) {
					comments.Add(comment);
				}
			}
			// Append an empty string to create a trailing line break when joined.
			if (comments.Count != 0) {
				comments.Add("");
			}
			return String.Join("\n", comments);
		}

		/// <summary>
		/// Generate code for the specified block (and attached blocks).
		/// </summary>
		/// <param name="block">The block to generate code for.</param>
		/// <returns>For statement blocks, the generated code.
		/// For value blocks, an array containing the generated code and an
		/// operator order value.  Returns '' if block is null.
		/// </returns>
		public object blockToCode(Block block)
		{
			if (block == null) {
				return "";
			}
			if (block.disabled) {
				// Skip past this block if it is disabled.
				return this.blockToCode(block.getNextBlock());
			}

			var func = this[block.type];
			goog.asserts.assertFunction(func,
				"Language \"%s\" does not know how to generate code for block type \"%s\".",
				this.name_, block.type);
			// First argument to func.call is the value of 'this' in the generator.
			// Prior to 24 September 2013 'this' was the only way to access the block.
			// The current prefered method of accessing the block is through the second
			// argument to func.call, which becomes the first parameter to the generator.
			var code = func.Invoke(this, new object[] { block });
			if (code is object[]) {
				// Value blocks return tuples of code and operator order.
				goog.asserts.assert(block.outputConnection != null,
					"Expecting string from statement block \"%s\".", block.type);
				return new object[] { this.scrub_(block, (string)((object[])code)[0]), ((object[])code)[1] };
			}
			else if (code is string) {
				var id = block.id.Replace(new Regex(@"\$", "g"), "$$$$");  // Issue 251.
				if (this.STATEMENT_PREFIX != null) {
					code = this.STATEMENT_PREFIX.Replace(new Regex("%1", "g"), "'" + id + "'") + code;
				}
				return this.scrub_(block, (string)code);
			}
			else if (code == null) {
				// Block has handled code generation itself.
				return "";
			}
			else {
				goog.asserts.fail("Invalid code generated: %s", (string)code);
				return "";
			}
		}

		/// <summary>
		/// Generate code representing the specified value input.
		/// </summary>
		/// <param name="block">The block containing the input.</param>
		/// <param name="name">The name of the input.</param>
		/// <param name="outerOrder">The maximum binding strength (minimum order value)
		/// of any operators adjacent to "block".</param>
		/// <returns>Generated code or '' if no blocks are connected or the
		/// specified input does not exist.</returns>
		public string valueToCode(Block block, string name, int outerOrder)
		{
			if (Script.IsNaN(outerOrder)) {
				goog.asserts.fail("Expecting valid order from block \"%s\".", block.type);
			}
			var targetBlock = block.getInputTargetBlock(name);
			if (targetBlock == null) {
				return "";
			}
			var tuple = this.blockToCode(targetBlock);
			if (tuple is string && (string)tuple == "") {
				// Disabled block.
				return "";
			}
			// Value blocks must return code and order of operations info.
			// Statement blocks must only return code.
			goog.asserts.assertArray(tuple, "Expecting tuple from value block \"%s\".", targetBlock.type);
			var code = ((object[])tuple)[0];
			var innerOrder = (int)((object[])tuple)[1];
			if (Script.IsNaN(innerOrder)) {
				goog.asserts.fail("Expecting valid order from value block \"%s\".", targetBlock.type);
			}
			if (code == null) {
				return "";
			}

			// Add parentheses if needed.
			var parensNeeded = false;
			var outerOrderClass = System.Math.Floor((double)outerOrder);
			var innerOrderClass = System.Math.Floor((double)innerOrder);
			if (outerOrderClass <= innerOrderClass) {
				if (outerOrderClass == innerOrderClass &&
					(outerOrderClass == 0 || outerOrderClass == 99)) {
					// Don't generate parens around NONE-NONE and ATOMIC-ATOMIC pairs.
					// 0 is the atomic order, 99 is the none order.  No parentheses needed.
					// In all known languages multiple such code blocks are not order
					// sensitive.  In fact in Python ('a' 'b') 'c' would fail.
				}
				else {
					// The operators outside this code are stonger than the operators
					// inside this code.  To prevent the code from being pulled apart,
					// wrap the code in parentheses.
					parensNeeded = true;
					// Check for special exceptions.
					for (var i = 0; i < this.ORDER_OVERRIDES.Count; i++) {
						if (this.ORDER_OVERRIDES[i][0] == outerOrder &&
							this.ORDER_OVERRIDES[i][1] == innerOrder) {
							parensNeeded = false;
							break;
						}
					}
				}
			}
			if (parensNeeded) {
				// Technically, this should be handled on a language-by-language basis.
				// However all known (sane) languages use parentheses for grouping.
				code = "(" + code + ")";
			}
			return (string)code;
		}

		/// <summary>
		/// Generate code representing the statement.  Indent the code.
		/// </summary>
		/// <param name="block">The block containing the input.</param>
		/// <param name="name">The name of the input.</param>
		/// <returns>Generated code or '' if no blocks are connected.</returns>
		public string statementToCode(Block block, string name)
		{
			var targetBlock = block.getInputTargetBlock(name);
			var code = this.blockToCode(targetBlock);
			// Value blocks must return code and order of operations info.
			// Statement blocks must only return code.
			goog.asserts.assertString(code, "Expecting code from statement block \"%s\".",
				targetBlock != null ? targetBlock.type : "");
			if (code != null) {
				code = this.prefixLines((string)code, this.INDENT);
			}
			return (string)code;
		}

		/// <summary>
		/// Add an infinite loop trap to the contents of a loop.
		/// If loop is empty, add a statment prefix for the loop block.
		/// </summary>
		/// <param name="branch">Code for loop contents.</param>
		/// <param name="id">ID of enclosing block.</param>
		/// <returns>Loop contents, with infinite loop trap added.</returns>
		public string addLoopTrap(string branch, string id)
		{
			id = id.Replace(new Regex(@"\$", "g"), "$$$$");  // Issue 251.
			if (this.INFINITE_LOOP_TRAP != null) {
				branch = this.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"), "'" + id + "'") + branch;
			}
			if (this.STATEMENT_PREFIX != null) {
				branch += this.prefixLines(this.STATEMENT_PREFIX.Replace(new Regex("%1", "g"),
					"'" + id + "'"), this.INDENT);
			}
			return branch;
		}

		/// <summary>
		/// Comma-separated list of reserved words.
		/// </summary>
		public string RESERVED_WORDS_ = "";

		/// <summary>
		/// Add one or more words to the list of reserved words for this language.
		/// </summary>
		/// <param name="words">Comma-separated list of words to add to the list.
		/// No spaces.  Duplicates are ok.</param>
		public void addReservedWords(string words)
		{
			this.RESERVED_WORDS_ += words + ",";
		}

		/// <summary>
		/// This is used as a placeholder in functions defined using
		/// Blockly.Generator.provideFunction_.  It must not be legal code that could
		/// legitimately appear in a function definition (or comment), and it must
		/// not confuse the regular expression parser.
		/// </summary>
		public string FUNCTION_NAME_PLACEHOLDER_ = "{leCUI8hutHZI4480Dc}";
		public Regex FUNCTION_NAME_PLACEHOLDER_REGEXP_;

		protected Dictionary<string, string> definitions_;
		protected Dictionary<string, string> functionNames_;
		protected Names variableDB_;

		/// <summary>
		/// Define a function to be included in the generated code.
		/// The first time this is called with a given desiredName, the code is
		/// saved and an actual name is generated.Subsequent calls with the
		/// same desiredName have no effect but have the same return value.
		/// 
		/// It is up to the caller to make sure the same desiredName is not
		/// used for different code values.
		/// 
		/// The code gets output when Blockly.Generator.finish() is called.
		/// </summary>
		/// <param name="desiredName">The desired name of the function (e.g., isPrime).</param>
		/// <param name="code">A list of statements.  Use '  ' for indents.</param>
		/// <returns>The actual name of the new function.  This may differ
		/// from desiredName if the former has already been taken by the user.
		/// </returns>
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
				codeText = codeText.Replace(new Regex("\0", "g"), this.INDENT);
				this.definitions_[desiredName] = codeText;
			}
			return this.functionNames_[desiredName];
		}

		public abstract string scrub_(Block block, string code);
	}
}