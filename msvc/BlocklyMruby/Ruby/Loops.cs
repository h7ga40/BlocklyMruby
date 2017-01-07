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
		public string controls_repeat(ControlsRepeatBlock block)
		{
			// Repeat n times (internal number).
			var repeats = Script.ParseInt(block.getFieldValue("TIMES"), 10);
			var branch = Blockly.Ruby.statementToCode(block, "DO");
			if (String.IsNullOrEmpty(branch)) branch = "end\n";
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					"\'" + block.id + "\'") + branch;
			}
			var code = repeats + ".times do\n" + branch + "end\n";
			return code;
		}

		public string controls_repeat_ext(ControlsRepeatExtBlock block)
		{
			// Repeat n times (external number).
			var repeats = Blockly.Ruby.valueToCode(block, "TIMES",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(repeats)) repeats = "0";
			if (Blockly.isNumber(repeats)) {
				repeats = Script.ParseInt(repeats, 10).ToString();
			}
			else {
				repeats = repeats + ".to_i";
			}
			var branch = Blockly.Ruby.statementToCode(block, "DO");
			if (String.IsNullOrEmpty(branch)) branch = "end\n";
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					"\'" + block.id + "\'") + branch;
			}
			var code = repeats + ".times do\n" + branch + "end\n";
			return code;
		}

		public string controls_whileUntil(ControlsWhileUntilBlock block)
		{
			// Do while/until loop.
			var until = block.getFieldValue("MODE") == "UNTIL";
			var argument0 = Blockly.Ruby.valueToCode(block, "BOOL",
				until ? Blockly.Ruby.ORDER_LOGICAL_NOT :
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "false";
			var branch = Blockly.Ruby.statementToCode(block, "DO");
			if (String.IsNullOrEmpty(branch)) branch = "end\n";
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					"\"" + block.id + "\"") + branch;
			}
			var mode = until ? "until " : "while ";
			return mode + argument0 + " do\n" + branch + "end\n";
		}

		public string controls_for(ControlsForBlock block)
		{
			// For loop.
			Blockly.Ruby.variableDB_.pushScope();

			var loopVar = Blockly.Ruby.variableDB_.addLocalVariable(
				block.getFieldValue("VAR"), Blockly.Variables.NAME_TYPE);
			var fromVal = Blockly.Ruby.valueToCode(block, "FROM",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(fromVal)) fromVal = "0";
			var toVal = Blockly.Ruby.valueToCode(block, "TO",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(toVal)) toVal = "0";
			var increment = Blockly.Ruby.valueToCode(block, "BY",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(increment)) increment = null;
			var branch = Blockly.Ruby.statementToCode(block, "DO");
			if (String.IsNullOrEmpty(branch)) branch = "";
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					'"' + block.id + '"') + branch;
			}

			Func<string, string, string, string> generateForLoop = (_fromVal, _toVal, _increment) => {
				return "for_loop from: (" + _fromVal + "), "
								 + "to: (" + _toVal + "), "
								 + "by: (" + _increment + ")";
			};

			var code = "";
			string _forLoop;

			if (Blockly.isNumber(fromVal) && Blockly.isNumber(toVal) &&
				(increment == null || Blockly.isNumber(increment))) {

				if (increment == null) increment = "1";

				// All parameters are simple numbers.
				fromVal = Script.ParseFloat(fromVal).ToString();
				toVal = Script.ParseFloat(toVal).ToString();
				increment = Script.ParseFloat(increment).ToString();

				_forLoop = generateForLoop(fromVal, toVal, increment);
			}
			else {
				if (increment == null) {
					increment = "1";
				}
				else {
					increment = "(" + increment + ").to_f";
				}

				_forLoop = generateForLoop(fromVal + ".to_f", toVal + ".to_f", increment);
			}

			Blockly.Ruby.variableDB_.popScope();

			code += _forLoop + " do |" + loopVar + "|\n" + branch + "end\n";

			return code;
		}

		public string controls_forEach(ControlsForEachBlock block)
		{
			// For each loop.
			Blockly.Ruby.variableDB_.pushScope();

			var loopVar = Blockly.Ruby.variableDB_.addLocalVariable(
				block.getFieldValue("VAR"), Blockly.Variables.NAME_TYPE);
			var argument0 = Blockly.Ruby.valueToCode(block, "LIST",
				Blockly.Ruby.ORDER_RELATIONAL);
			if (String.IsNullOrEmpty(argument0)) argument0 = "[]";
			var branch = Blockly.Ruby.statementToCode(block, "DO");
			if (String.IsNullOrEmpty(branch)) branch = "end\n";
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					"\"" + block.id + "\"") + branch;
			}

			Blockly.Ruby.variableDB_.popScope();

			var code = argument0 + ".each do |" + loopVar + "|\n" + branch + "end\n";
			return code;
		}

		public string controls_flow_statements(ControlsFlowStatementsBlock block)
		{
			// Flow statements: continue, break.
			switch (block.getFieldValue("FLOW")) {
			case "BREAK":
				return "break\n";
			case "CONTINUE":
				return "next\n";
			}
			throw new Exception("Unknown flow statement.");
		}
	}
}
