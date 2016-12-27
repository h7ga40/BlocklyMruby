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
		public string controls_if(ControlsIfBlock block)
		{
			// If/elseif/else condition.
			var n = 0;
			var argument = Blockly.Ruby.valueToCode(block, "IF" + n,
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument)) argument = "false";
			var branch = Blockly.Ruby.statementToCode(block, "DO" + n);
			if (String.IsNullOrEmpty(branch)) branch = "\n";
			var code = "if " + argument + "\n" + branch;
			for (n = 1; n <= block.elseifCount_; n++) {
				argument = Blockly.Ruby.valueToCode(block, "IF" + n,
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(argument)) argument = "false";
				branch = Blockly.Ruby.statementToCode(block, "DO" + n);
				if (String.IsNullOrEmpty(branch)) branch = "\n";
				code += "elsif " + argument + "\n" + branch;
			}
			if (block.elseCount_ != 0) {
				branch = Blockly.Ruby.statementToCode(block, "ELSE");
				if (String.IsNullOrEmpty(branch)) branch = "\n";
				code += "else\n" + branch;
			}
			code += "end\n";
			return code;
		}

		public object[] logic_compare(LogicCompareBlock block)
		{
			// Comparison operator.
			var OPERATORS = new Dictionary<string, string>();
			OPERATORS.Add("EQ", "==");
			OPERATORS.Add("NEQ", "!=");
			OPERATORS.Add("LT", "<");
			OPERATORS.Add("LTE", "<=");
			OPERATORS.Add("GT", ">");
			OPERATORS.Add("GTE", ">=");
			var @operator = OPERATORS[block.getFieldValue("OP")];
			var order = Blockly.Ruby.ORDER_RELATIONAL;
			var argument0 = Blockly.Ruby.valueToCode(block, "A", order);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var argument1 = Blockly.Ruby.valueToCode(block, "B", order);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var code = argument0 + " " + @operator + " " + argument1;
			return new object[] { code, order };
		}

		public object[] logic_operation(LogicOperationBlock block)
		{
			// Operations 'and', 'or'.
			var @operator = (block.getFieldValue("OP") == "AND") ? "&&" : "||";
			var order = (@operator == "&&") ? Blockly.Ruby.ORDER_LOGICAL_AND :
				Blockly.Ruby.ORDER_LOGICAL_OR;
			var argument0 = Blockly.Ruby.valueToCode(block, "A", order);
			var argument1 = Blockly.Ruby.valueToCode(block, "B", order);
			if (String.IsNullOrEmpty(argument0) && String.IsNullOrEmpty(argument1)) {
				// If there are no arguments, then the return value is false.
				argument0 = "false";
				argument1 = "false";
			}
			else {
				// Single missing arguments have no effect on the return value.
				var defaultArgument = (@operator == "&&") ? "true" : "false";
				if (String.IsNullOrEmpty(argument0)) {
					argument0 = defaultArgument;
				}
				if (String.IsNullOrEmpty(argument1)) {
					argument1 = defaultArgument;
				}
			}
			var code = argument0 + ' ' + @operator + ' ' + argument1;
			return new object[] { code, order };
		}

		public object[] logic_negate(LogicNegateBlock block)
		{
			// Negation.
			var argument0 = Blockly.Ruby.valueToCode(block, "BOOL",
				Blockly.Ruby.ORDER_LOGICAL_NOT);
			if (String.IsNullOrEmpty(argument0)) argument0 = "true";
			var code = "! " + argument0;
			return new object[] { code, ORDER_LOGICAL_NOT };
		}

		public object[] logic_boolean(LogicBooleanBlock block)
		{
			// Boolean values true and false.
			var code = (block.getFieldValue("BOOL") == "TRUE") ? "true" : "false";
			return new object[] { code, ORDER_ATOMIC };
		}

		public object[] logic_null(LogicNullBlock block)
		{
			// Null data type.
			return new object[] { "nil", ORDER_ATOMIC };
		}

		public object[] logic_ternary(LogicTernaryBlock block)
		{
			// Ternary operator.
			var value_if = Blockly.Ruby.valueToCode(block, "IF",
				Blockly.Ruby.ORDER_CONDITIONAL);
			if (String.IsNullOrEmpty(value_if)) value_if = "false";
			var value_then = Blockly.Ruby.valueToCode(block, "THEN",
				Blockly.Ruby.ORDER_CONDITIONAL);
			if (String.IsNullOrEmpty(value_then)) value_then = "nil";
			var value_else = Blockly.Ruby.valueToCode(block, "ELSE",
				Blockly.Ruby.ORDER_CONDITIONAL);
			if (String.IsNullOrEmpty(value_else)) value_else = "nil";
			var code = value_if + " ? " + value_then + " : " + value_else;
			return new object[] { code, ORDER_CONDITIONAL };
		}
	}
}
