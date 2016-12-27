// Porting from 
// https://github.com/jeanlazarou/blockly2ruby
// Copyright (c) 2014 Jean Lazarou
// MIT Lisence
using System;
using Bridge;
using Bridge.Text.RegularExpressions;
using System.Collections.Generic;

namespace BlocklyMruby
{
	partial class Ruby
	{
		public object[] text(TextBlock block)
		{
			// Text value.
			var code = Blockly.Ruby.quote_(block.getFieldValue("TEXT"));
			return new object[] { code, ORDER_ATOMIC };
		}

		public object[] text_join(TextJoinBlock block)
		{
			// Create a string made up of any number of elements of any type.
			string code;
			if (block.itemCount_ == 0) {
				return new object[] { "\'\'", ORDER_ATOMIC };
			}
			else if (block.itemCount_ == 1) {
				var argument0 = Blockly.Ruby.valueToCode(block, "ADD0",
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
				code = argument0 + ".to_s";
				return new object[] { code, ORDER_FUNCTION_CALL };
			}
			else if (block.itemCount_ == 2) {
				var argument0 = Blockly.Ruby.valueToCode(block, "ADD0",
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
				var argument1 = Blockly.Ruby.valueToCode(block, "ADD1",
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(argument1)) argument1 = "\'\'";
				code = argument0 + ".to_s + " + argument1 + ".to_s";
				return new object[] { code, ORDER_UNARY_SIGN };
			}
			else {
				var codes = new string[0];
				for (var n = 0; n < block.itemCount_; n++) {
					var temp = Blockly.Ruby.valueToCode(block, "ADD" + n,
						Blockly.Ruby.ORDER_NONE);
					if (String.IsNullOrEmpty(temp)) temp = "\'\'";
					codes[n] = temp + ".to_s";
				}
				code = codes.Join(" + ");
				return new object[] { code, ORDER_FUNCTION_CALL };
			}
		}

		public string text_append(TextAppendBlock block)
		{
			// Append to a variable in place.
			var varName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("VAR"),
				Blockly.Variables.NAME_TYPE);
			var argument0 = Blockly.Ruby.valueToCode(block, "TEXT",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
			return varName + " = " + varName + ".to_s + (" + argument0 + ").to_s\n";
		}

		public object[] text_length(TextLengthBlock block)
		{
			// String length.
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
			return new object[] { argument0 + ".size", ORDER_FUNCTION_CALL };
		}

		public object[] text_isEmpty(TextIsEmptyBlock block)
		{
			// Is the string null?
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
			var code = argument0 + ".empty?";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] text_indexOf(TextIndexOfBlock block)
		{
			// Search the text for a substring.
			// Should we allow for non-case sensitive???
			var finder = block.getFieldValue("END") == "FIRST" ? ".find_first" : ".find_last";
			var search = Blockly.Ruby.valueToCode(block, "FIND",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(search)) search = "\'\'";
			var text = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(text)) text = "\'\'";
			var code = text + finder + "(" + search + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] text_charAt(TextCharAtBlock block)
		{
			// Get letter at index.
			// Note: Until January 2013 this block did not have the WHERE input.
			var where = block.getFieldValue("WHERE");
			if (String.IsNullOrEmpty(where)) where = "FROM_START";
			var at = Blockly.Ruby.valueToCode(block, "AT",
				Blockly.Ruby.ORDER_UNARY_SIGN);
			if (String.IsNullOrEmpty(at)) at = "1";
			var text = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(text)) text = "\'\'";

			// Blockly uses one-based indicies.
			if (Blockly.isNumber(at)) {
				// If the index is a naked number, decrement it right now.
				at = (Script.ParseInt(at, 10) - 1).ToString();
			}
			else {
				// If the index is dynamic, decrement it in code.
				at = at + ".to_i - 1";
			}

			string code, functionName;
			switch (where) {
			case "FIRST":
				code = text + "[0]";
				return new object[] { code, ORDER_MEMBER };
			case "LAST":
				code = text + "[-1]";
				return new object[] { code, ORDER_MEMBER };
			case "FROM_START":
				functionName = Blockly.Ruby.provideFunction_(
					"text_get_from_start",
					new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + " (text, index)",
					 "  return \"\" if index < 0",
					 "  text[index] || \"\"",
					 "end" });
				code = functionName + "(" + text + ", " + at + ")";
				return new object[] { code, ORDER_FUNCTION_CALL };
			case "FROM_END":
				functionName = Blockly.Ruby.provideFunction_(
					"text_get_from_end",
					new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + " (text, index)",
					 "  return \"\" if index < 0",
					 "  text[-index-1] || \"\"'",
					 "end" });
				code = functionName + "(" + text + ", " + at + ")";
				return new object[] { code, ORDER_FUNCTION_CALL };
			case "RANDOM":
				functionName = Blockly.Ruby.provideFunction_(
					"text_random_letter",
					new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + " (text)",
					 "  text[rand(text.size)]",
					 "end" });
				code = functionName + "(" + text + ")";
				return new object[] { code, ORDER_FUNCTION_CALL };
			}
			throw new Exception("Unhandled option (text_charAt).");
		}

		public object[] text_getSubstring(TextGetSubstringBlock block)
		{
			// Get substring.
			var text = Blockly.Ruby.valueToCode(block, "STRING",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(text)) text = "\'\'";
			var where1 = block.getFieldValue("WHERE1");
			var where2 = block.getFieldValue("WHERE2");
			var at1 = Blockly.Ruby.valueToCode(block, "AT1",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(at1)) at1 = "1";
			var at2 = Blockly.Ruby.valueToCode(block, "AT2",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(at2)) at2 = "1";
			if (where1 == "FIRST" || (where1 == "FROM_START" && at1 == "1")) {
				at1 = "0";
			}
			else if (where1 == "FROM_START") {
				// Blockly uses one-based indicies.
				if (Blockly.isNumber(at1)) {
					// If the index is a naked number, decrement it right now.
					at1 = (Script.ParseInt(at1, 10) - 1).ToString();
				}
				else {
					// If the index is dynamic, decrement it in code.
					at1 = at1 + ".to_i - 1";
				}
			}
			else if (where1 == "FROM_END") {
				if (Blockly.isNumber(at1)) {
					at1 = (-Script.ParseInt(at1, 10)).ToString();
				}
				else {
					at1 = "-" + at1 + ".to_i";
				}
			}
			if (where2 == "LAST" || (where2 == "FROM_END" && at2 == "1")) {
				at2 = "-1";
			}
			else if (where2 == "FROM_START") {
				if (Blockly.isNumber(at2)) {
					at2 = (Script.ParseInt(at2, 10) - 1).ToString();
				}
				else {
					at2 = at2 + ".to_i - 1";
				}
			}
			else if (where2 == "FROM_END") {
				if (Blockly.isNumber(at2)) {
					at2 = (-Script.ParseInt(at2, 10)).ToString();
				}
				else {
					at2 = at2 + ".to_i";
				}
			}
			var code = text + "[" + at1 + ".." + at2 + "]";
			return new object[] { code, ORDER_MEMBER };
		}

		public object[] text_changeCase(TextChangeCaseBlock block)
		{
			// Change capitalization.
			var OPERATORS = new Dictionary<string, string>();
			OPERATORS.Add("UPPERCASE", ".upcase");
			OPERATORS.Add("LOWERCASE", ".downcase");
			OPERATORS.Add("TITLECASE", null);
			string code;
			var @operator = OPERATORS[block.getFieldValue("CASE")];
			if (!String.IsNullOrEmpty(@operator)) {
				@operator = OPERATORS[block.getFieldValue("CASE")];
				var argument0 = Blockly.Ruby.valueToCode(block, "TEXT",
					Blockly.Ruby.ORDER_MEMBER);
				if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
				code = argument0 + @operator;
			}
			else {
				// Title case is not a native Ruby function. Define one.
				var functionName = Blockly.Ruby.provideFunction_(
					"text_to_title_case",
					new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + "(str)",
					  "  str.gsub(/\\S+/) {|txt| txt.capitalize}",
					  "end" });
				var argument0 = Blockly.Ruby.valueToCode(block, "TEXT",
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
				code = functionName + "(" + argument0 + ")";
			}

			return new object[] { code, ORDER_MEMBER };
		}

		public object[] text_trim(TextTrimBlock block)
		{
			// Trim spaces.
			var OPERATORS = new Dictionary<string, string>();
			OPERATORS.Add("LEFT", ".lstrip");
			OPERATORS.Add("RIGHT", ".rstrip");
			OPERATORS.Add("BOTH", ".strip");
			var @operator = OPERATORS[block.getFieldValue("MODE")];
			var argument0 = Blockly.Ruby.valueToCode(block, "TEXT",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
			var code = argument0 + @operator;
			return new object[] { code, ORDER_MEMBER };
		}

		public string text_print(TextPrintBlock block)

		{
			// Print statement.
			var argument0 = Blockly.Ruby.valueToCode(block, "TEXT",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "\'\'";
			return "blockly_puts(" + argument0 + ")\n";
		}

		public object[] text_prompt(TextPromptBlock block)
		{
			// Prompt function.
			var functionName = Blockly.Ruby.provideFunction_(
				"text_prompt",
				new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + "(msg):",
				 "    print (msg)",
				 "    $stdin.gets" });
			var msg = Blockly.Ruby.quote_(block.getFieldValue("TEXT"));
			var code = functionName + "(" + msg + ")";
			var toNumber = block.getFieldValue("TYPE") == "NUMBER";
			if (toNumber) {
				code = code + ".to_f";
			}
			return new object[] { code, ORDER_FUNCTION_CALL };
		}
	}
}
