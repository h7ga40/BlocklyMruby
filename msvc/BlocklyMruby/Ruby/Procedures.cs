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
		public object[] procedures_defreturn(ProcedureDefBlock block)
		{
			Blockly.Ruby.variableDB_.pushScope();

			var args = new List<string>();
			for (var x = 0; x < block.arguments_.Count; x++) {
				args.Add(Blockly.Ruby.variableDB_.addLocalVariable(block.arguments_[x],
					Blockly.Variables.NAME_TYPE));
			}
			var funcName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("NAME"),
				Blockly.Procedures.NAME_TYPE);
			var branch = Blockly.Ruby.statementToCode(block, "STACK");
			if (Blockly.Ruby.INFINITE_LOOP_TRAP != null) {
				branch = Blockly.Ruby.INFINITE_LOOP_TRAP.Replace(new Regex("%1", "g"),
					"\"" + block.id + "\"") + branch;
			}
			var returnValue = Blockly.Ruby.valueToCode(block, "RETURN",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(returnValue)) {
				returnValue = "\n  return " + returnValue + "\n";
			}
			var code = "def " + funcName + "(" + String.Join(", ", args) + ")\n" +
				branch + returnValue + "end";
			code = Blockly.Ruby.scrub_(block, code);
			Blockly.Ruby.definitions_[funcName] = code;

			Blockly.Ruby.variableDB_.popScope();

			return null;
		}

		// Defining a procedure without a return value uses the same generator as
		// a procedure with a return value.
		public object[] procedures_defnoreturn(ProceduresDefnoreturnBlock block)
		{
			return Blockly.Ruby.procedures_defreturn(block);
		}

		public object[] procedures_callreturn(ProceduresCallreturnBlock block)
		{
			// Call a procedure with a return value.
			var funcName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("NAME"),
				Blockly.Procedures.NAME_TYPE);
			var args = new string[0];
			for (var x = 0; x < block.arguments_.Count; x++) {
				args[x] = Blockly.Ruby.valueToCode(block, "ARG" + x,
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(args[x]))
					args[x] = "None";
			}

			var code = funcName + "(" + args.Join(", ") + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public string procedures_callnoreturn(ProceduresCallnoreturnBlock block)
		{
			// Call a procedure with no return value.
			var funcName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("NAME"),
				Blockly.Procedures.NAME_TYPE);
			var args = new List<string>();
			for (var x = 0; x < block.arguments_.Count; x++) {
				args.Add(Blockly.Ruby.valueToCode(block, "ARG" + x, Blockly.Ruby.ORDER_NONE));
				if (String.IsNullOrEmpty(args[x]))
					args[x] = "None";
			}
			var code = funcName + "(" + String.Join(", ", args) + ")\n";
			return code;
		}

		public string procedures_ifreturn(ProceduresIfreturnBlock block)
		{
			// Conditionally return value from a procedure.
			var condition = Blockly.Ruby.valueToCode(block, "CONDITION",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(condition)) condition = "False";
			var code = "if " + condition + "\n";
			if (block.hasReturnValue_) {
				var value = Blockly.Ruby.valueToCode(block, "VALUE",
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(value)) value = "None";
				code += "\n  return " + value + "\n";
			}
			else {
				code += "\n  return\n";
			}
			code += "end\n";

			return code;
		}
	}
}
