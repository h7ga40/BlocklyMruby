// Porting from 
// https://github.com/jeanlazarou/blockly2ruby
// Copyright (c) 2014 Jean Lazarou
// MIT Lisence
using System;
using Bridge;
using Bridge.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlocklyMruby
{
	[ComVisible(true)]
	public partial class Ruby : Generator
	{
		internal Ruby(object instance)
			: base(instance)
		{
		}

		public Ruby()
			: base("Ruby")
		{
			/**
			 * List of illegal variable names.
			 * This is not intended to be a security feature.  Blockly is 100% client-side,
			 * so bypassing this list is trivial.  This is intended to prevent users from
			 * accidentally clobbering a built-in object or function.
			 * @private
			 */
			addReservedWords(
				"Class,Object,BEGIN,END,__ENCODING__,__END__,__FILE__,__LINE__" +
				"alias,and,begin,break,case,class,def,defined?,do,else,elsif,end,ensure,false,for,if,in,module,next" +
				"nil,not,or,redo,rescue,retry,return,self,super,then,true,undef,unless,until,when,while,yield");

			escapeChars_.Add("\"", "\\\"");
		}

		/**
		 * Order of operation ENUMs.
		 * http://phrogz.net/programmingruby/language.html
		 * http://www.techotopia.com/index.php/Ruby_Operator_Precedence
		 * http://www.tutorialspoint.com/ruby/ruby_operators.htm
		 */
		[Name(false)]
		public int ORDER_ATOMIC = 0;            // 0 "" ...
		[Name(false)]
		public int ORDER_MEMBER = 2;            // . []
		[Name(false)]
		public int ORDER_FUNCTION_CALL = 2;     // ()
		[Name(false)]
		public int ORDER_EXPONENTIATION = 3;    // **
		[Name(false)]
		public int ORDER_LOGICAL_NOT = 4;      // !
		[Name(false)]
		public int ORDER_UNARY_SIGN = 4;        // + -
		[Name(false)]
		public int ORDER_BITWISE_NOT = 4;       // ~
		[Name(false)]
		public int ORDER_MULTIPLICATIVE = 5;    // * / // %
		[Name(false)]
		public int ORDER_ADDITIVE = 6;          // + -
		[Name(false)]
		public int ORDER_BITWISE_SHIFT = 7;     // << >>
		[Name(false)]
		public int ORDER_BITWISE_AND = 8;       // &
		[Name(false)]
		public int ORDER_BITWISE_XOR = 9;       // ^
		[Name(false)]
		public int ORDER_BITWISE_OR = 9;        // |
		[Name(false)]
		public int ORDER_RELATIONAL = 11;       // <, <=, >, >=, <>, !=, ==
		[Name(false)]
		public int ORDER_LOGICAL_AND = 13;      // &&
		[Name(false)]
		public int ORDER_LOGICAL_OR = 14;       // ||
		[Name(false)]
		public int ORDER_CONDITIONAL = 15;      // if unless while until
		[Name(false)]
		public int ORDER_NONE = 99;             // (...)

		/**
		 * Initialise the database of variable names.
		 */
		public override void init(Blockly.Workspace workspace)
		{
			// Create a dictionary of definitions to be printed before the code.
			definitions_ = new Dictionary<string, string>();
			// Create a dictionary mapping desired function names in definitions_
			// to actual function names (to avoid collisions with user functions).
			functionNames_ = new Dictionary<string, string>();

			if (true/*Blockly.Variables != null*/) {
				if (variableDB_ == null) {

					variableDB_ = new Names(RESERVED_WORDS_);

					variableDB_.localVars = null;
					variableDB_.localVarsDB = null;
				}
				else {
					variableDB_.reset();
				}

				var defvars = new List<string>();
				var variables = Blockly.Variables.allVariables(workspace);
				for (var x = 0; x < variables.Length; x++) {
					defvars.Add("@" +
						variableDB_.getName(variables[x], Blockly.Variables.NAME_TYPE) +
						" = nil");
				}
				var code = String.Join("\n", defvars);
				definitions_["variables"] = code;
			}
		}

		string validName(string name)
		{
			return variableDB_.safeName_(name);
		}

		/**
		 * Returns the string containing all definitions
		 * @return {string} Definitions code.
		 */
		string generateDefinitions(string[] helpers)
		{
			var requires = new List<string>();
			var prepares = new List<string>();
			var definitions = new List<string>();
			foreach (var name in definitions_.Keys) {
				var def = definitions_[name];
				if (def == null)
					continue;

				if (name.Match(new Regex("^require__")) != null) {
					requires.Add(def);
				}
				else if (name.Match(new Regex("^prepare__")) != null) {
					prepares.Add(def);
				}
				else
					definitions.Add(def);
			}

			string allDefs = "";

			if (requires.Count > 0)
				allDefs += String.Join("\n", requires) + "\n\n";

			if (helpers.Length > 0)
				allDefs += String.Join("\n", helpers) + "\n\n";

			if (prepares.Count > 0)
				allDefs += String.Join("\n", prepares) + "\n\n";

			if (definitions.Count > 0) {
				allDefs += String.Join("\n", definitions) + "\n\n";
			}
			return allDefs;
		}

		/**
		 * Prepend the generated code with the variable definitions.
		 * @param {string} code Generated code.
		 * @return {string} Completed code.
		 */
		public override string finish(string code)
		{
			// need some helper functions to conform to Blockly's behavior
			var helpers = new string[0];
#if false
			var i = 0;
			helpers[i++] = "def blockly_puts x";
			helpers[i++] = "  if x.is_a?(Array)";
			helpers[i++] = "    puts x.join(',')";
			helpers[i++] = "  else";
			helpers[i++] = "    puts x";
			helpers[i++] = "  end";
			helpers[i++] = "end";

			helpers[i++] = "";

			helpers[i++] = "class Array";
			helpers[i++] = "  def find_first v";
			helpers[i++] = "    i = self.index(v)";
			helpers[i++] = "    i ? i + 1 : 0";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def find_last v";
			helpers[i++] = "    i = self.rindex(v)";
			helpers[i++] = "    i ? i + 1 : 0";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def numbers";
			helpers[i++] = "    self.delete_if {|v| !v.is_a?(Numeric)}";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def sum";
			helpers[i++] = "    self.numbers.inject(0) {|sum, v| sum + v}";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def average";
			helpers[i++] = "    x = self.numbers";
			helpers[i++] = "    x.sum / x.size.to_f";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def standard_deviation";
			helpers[i++] = "    x = self.numbers";
			helpers[i++] = "    return 0 if x.empty?";
			helpers[i++] = "    mean = x.average";
			helpers[i++] = "    variance = x.map {|v| (v - mean) ** 2}.sum / x.size";
			helpers[i++] = "    Math.sqrt(variance)";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def median";
			helpers[i++] = "    x = self.numbers";
			helpers[i++] = "    x.sort!";
			helpers[i++] = "    index  = x.size / 2";
			helpers[i++] = "    x.size.odd? ? x[index] : ((x[index - 1] + x[index]) / 2.0)";
			helpers[i++] = "  end";
			helpers[i++] = "end";

			helpers[i++] = "";

			helpers[i++] = "class String";
			helpers[i++] = "  def find_first v";
			helpers[i++] = "    i = self.index(v)";
			helpers[i++] = "    i ? i + 1 : 0";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def find_last v";
			helpers[i++] = "    i = self.rindex(v)";
			helpers[i++] = "    i ? i + 1 : 0";
			helpers[i++] = "  end";
			helpers[i++] = "end";

			helpers[i++] = "";

			helpers[i++] = "class Float";
			helpers[i++] = "  def even?";
			helpers[i++] = "    false";
			helpers[i++] = "  end";
			helpers[i++] = "";
			helpers[i++] = "  def odd?";
			helpers[i++] = "    false";
			helpers[i++] = "  end";
			helpers[i++] = "end";
#endif
			var indent = INDENT;
			var allDefs = generateDefinitions(helpers);
			allDefs = indent + allDefs.Split("\n").Join("\n" + indent);
			return allDefs.Replace(new Regex("\n\n+", "g"), "\n\n") + "\n" + code;
		}

		/**
		 * Naked values are top-level blocks with outputs that aren't plugged into
		 * anything.
		 * @param {string} line Line of generated code.
		 * @return {string} Legal line of code.
		 */
		public override string scrubNakedValue(string line)
		{
			return line + "\n";
		}

		Dictionary<string, string> escapeChars_ = new Dictionary<string, string>();

		/**
		 * Encode a string as a properly escaped Ruby string, complete with quotes.
		 * @param {string} string Text to encode.
		 * @return {string} Python string.
		 * @private
		 */
		string quote_(string str)
		{
			// copy and modified goog.string.quote:
			// http://docs.closure-library.googlecode.com/git/namespace_goog_string.html
			var s = str.ToString();
			var sb = new List<string>() { "\"" };
			for (var i = 0; i < s.Length; i++) {
				var ch = s.CharAt(i);
				string rch;
				if (!escapeChars_.TryGetValue(ch, out rch))
					rch = ch;
				sb.Add(rch);
			}
			sb.Add("\"");
			return String.Join("", sb);
		}

		/**
		 * Common tasks for generating Ruby from blocks.
		 * Handles comments for the specified block and any connected value blocks.
		 * Calls any statements following this block.
		 * @param {!Blockly.Block} block The current block.
		 * @param {string} code The Ruby code created for this block.
		 * @return {string} Ruby code with comments and subsequent blocks added.
		 * @this {Blockly.CodeGenerator}
		 * @private
		 */
		public override string scrub_(Block block, string code)

		{
			if (code == null) {
				// Block has handled code generation itself.
				return "";
			}
			var commentCode = "";
			// Only collect comments for blocks that aren't inline.
			if (block.outputConnection == null || block.outputConnection.targetConnection == null) {
				// Collect comment for this block.
				var comment = block.getCommentText();
				if (!String.IsNullOrEmpty(comment)) {
					commentCode += this.prefixLines(comment, "# ") + "\n";
				}
				// Collect comments for all value arguments.
				// Don't collect comments for nested statements.
				var inputList = block.inputList;
				for (var x = 0; x < inputList.Length; x++) {
					if (inputList[x].type == Blockly.INPUT_VALUE) {
						var childBlock = inputList[x].connection.targetBlock();
						if (childBlock != null) {
							comment = allNestedComments(childBlock);
							if (!String.IsNullOrEmpty(comment)) {
								commentCode += this.prefixLines(comment, "# ");
							}
						}
					}
				}
			}
			Block nextBlock = null;
			if (block.nextConnection != null)
				nextBlock = block.nextConnection.targetBlock();
			var nextCode = blockToCode(nextBlock);

			return commentCode + code + nextCode;
		}
	}
}
