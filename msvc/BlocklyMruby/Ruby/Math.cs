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
		public object[] math_number(MathNumberBlock block)
		{
			// Numeric value.
			var code = Script.ParseFloat(block.getFieldValue("NUM"));
			var order = code < 0 ? Blockly.Ruby.ORDER_UNARY_SIGN :
						Blockly.Ruby.ORDER_ATOMIC;
			return new object[] { code.ToString(), order };
		}

		public object[] math_arithmetic(MathArithmeticBlock block)
		{
			// Basic arithmetic operators, and power.
			var OPERATORS = new Dictionary<string, object[]>();
			OPERATORS.Add("ADD", new object[] { " + ", Blockly.Ruby.ORDER_ADDITIVE });
			OPERATORS.Add("MINUS", new object[] { " - ", Blockly.Ruby.ORDER_ADDITIVE });
			OPERATORS.Add("MULTIPLY", new object[] { " * ", Blockly.Ruby.ORDER_MULTIPLICATIVE });
			OPERATORS.Add("DIVIDE", new object[] { " / ", Blockly.Ruby.ORDER_MULTIPLICATIVE });
			OPERATORS.Add("POWER", new object[] { " ** ", Blockly.Ruby.ORDER_EXPONENTIATION });
			var tuple = OPERATORS[block.getFieldValue("OP")];
			var @operator = tuple[0];
			var order = (int)tuple[1];
			var argument0 = Blockly.Ruby.valueToCode(block, "A", order);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var argument1 = Blockly.Ruby.valueToCode(block, "B", order);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var code = argument0 + @operator + argument1;
			return new object[] { code, order };
		}

		public object[] math_single(Block block)
		{
			// Math operators with single operand.
			var @operator = block.getFieldValue("OP");
			string code = null;
			string arg;
			if (@operator == "NEG") {
				// Negation is a special case given its different operator precedence.
				code = Blockly.Ruby.valueToCode(block, "NUM",
					Blockly.Ruby.ORDER_UNARY_SIGN);
				if (String.IsNullOrEmpty(code)) code = "0";
				return new object[] { "-" + code, ORDER_UNARY_SIGN };
			}
			if (@operator == "SIN" || @operator == "COS" || @operator == "TAN") {
				arg = "(" + Blockly.Ruby.valueToCode(block, "NUM",
					Blockly.Ruby.ORDER_MULTIPLICATIVE) + ")";
				if (String.IsNullOrEmpty(arg)) arg = "0";
			}
			else {
				arg = "(" + Blockly.Ruby.valueToCode(block, "NUM",
					Blockly.Ruby.ORDER_NONE) + ")";
				if (String.IsNullOrEmpty(arg)) arg = "0";
			}
			// First, handle cases which generate values that don't need parentheses
			// wrapping the code.
			switch (@operator) {
			case "ABS":
				code = arg + ".abs";
				break;
			case "ROOT":
				code = "Math.sqrt(" + arg + ")";
				break;
			case "LN":
				code = "Math.log(" + arg + ")";
				break;
			case "LOG10":
				code = "Math.log10(" + arg + ")";
				break;
			case "EXP":
				code = "Math.exp(" + arg + ")";
				break;
			case "POW10":
				code = "(10 ** " + arg + ")";
				break;
			case "ROUND":
				code = arg + ".round";
				break;
			case "ROUNDUP":
				code = arg + ".ceil";
				break;
			case "ROUNDDOWN":
				code = arg + ".floor";
				break;
			case "SIN":
				code = "Math.sin(" + arg + " / 180.0 * Math::PI)";
				break;
			case "COS":
				code = "Math.cos(" + arg + " / 180.0 * Math::PI)";
				break;
			case "TAN":
				code = "Math.tan(" + arg + " / 180.0 * Math::PI)";
				break;
			}
			if (!String.IsNullOrEmpty(code)) {
				return new object[] { code, ORDER_FUNCTION_CALL };
			}
			// Second, handle cases which generate values that may need parentheses
			// wrapping the code.
			switch (@operator) {
			case "ASIN":
				code = "Math.asin(" + arg + ") / Math::PI * 180";
				break;
			case "ACOS":
				code = "Math.acos(" + arg + ") / Math::PI * 180";
				break;
			case "ATAN":
				code = "Math.atan(" + arg + ") / Math::PI * 180";
				break;
			default:
				throw new Exception("Unknown math operator: " + @operator);
			}
			return new object[] { code, ORDER_MULTIPLICATIVE };
		}

		public object[] math_constant(MathConstantBlock block)
		{
			// Constants: PI, E, the Golden Ratio, sqrt(2), 1/sqrt(2), INFINITY.
			var CONSTANTS = new Dictionary<string, object[]>();
			CONSTANTS.Add("PI", new object[] { "Math::PI", Blockly.Ruby.ORDER_MEMBER });
			CONSTANTS.Add("E", new object[] { "Math::E", Blockly.Ruby.ORDER_MEMBER });
			CONSTANTS.Add("GOLDEN_RATIO", new object[] { "(1 + Math.sqrt(5)) / 2", Blockly.Ruby.ORDER_MULTIPLICATIVE });
			CONSTANTS.Add("SQRT2", new object[] { "Math.sqrt(2)", Blockly.Ruby.ORDER_MEMBER });
			CONSTANTS.Add("SQRT1_2", new object[] { "Math.sqrt(1.0 / 2)", Blockly.Ruby.ORDER_MEMBER });
			CONSTANTS.Add("INFINITY", new object[] { "1/0.0", Blockly.Ruby.ORDER_ATOMIC });
			var constant = block.getFieldValue("CONSTANT");
			return CONSTANTS[constant];
		}

		public object[] math_number_property(MathNumberPropertyBlock block)
		{
			// Check if a number is even, odd, prime, whole, positive, or negative
			// or if it is divisible by certain number. Returns true or false.
			var number_to_check = Blockly.Ruby.valueToCode(block, "NUMBER_TO_CHECK",
				Blockly.Ruby.ORDER_MULTIPLICATIVE);
			if (String.IsNullOrEmpty(number_to_check)) number_to_check = "0";
			var dropdown_property = block.getFieldValue("PROPERTY");
			string code = null;
			if (dropdown_property == "PRIME") {
				code = "is_prime(" + number_to_check + ")";
				return new object[] { code, ORDER_FUNCTION_CALL };
			}
			switch (dropdown_property) {
			case "EVEN":
				code = number_to_check + ".even?";
				break;
			case "ODD":
				code = number_to_check + ".odd?";
				break;
			case "WHOLE":
				code = number_to_check + " % 1 == 0";
				break;
			case "POSITIVE":
				code = number_to_check + " > 0";
				break;
			case "NEGATIVE":
				code = number_to_check + " < 0";
				break;
			case "DIVISIBLE_BY":
				var divisor = Blockly.Ruby.valueToCode(block, "DIVISOR",
					Blockly.Ruby.ORDER_MULTIPLICATIVE);
				// If "divisor" is some code that evals to 0, Ruby will raise an error.
				if (String.IsNullOrEmpty(divisor) || divisor == "0") {
					return new object[] { "false", ORDER_ATOMIC };
				}
				code = number_to_check + " % " + divisor + " == 0";
				break;
			}
			return new object[] { code, ORDER_RELATIONAL };
		}

		public string math_change(MathChangeBlock block)
		{
			// Add to a variable in place.
			var argument0 = Blockly.Ruby.valueToCode(block, "DELTA",
				Blockly.Ruby.ORDER_ADDITIVE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var varName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("VAR"),
				Blockly.Variables.NAME_TYPE);
			return varName + " += " + argument0 + "\n";
		}

		// Rounding functions have a single operand.
		public object[] math_round(MathRoundBlock block) { return math_single(block); }
		// Trigonometry functions have a single operand.
		public object[] math_trig(MathTrigBlock block) { return math_single(block); }

		public object[] math_on_list(MathOnListBlock block)
		{
			// Math functions for lists.
			var func = block.getFieldValue("OP");
			var list = Blockly.Ruby.valueToCode(block, "LIST",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(list)) list = "[]";
			string code;
			switch (func) {
			case "SUM":
				code = list + ".sum";
				break;
			case "MIN":
				code = list + ".numbers.min";
				break;
			case "MAX":
				code = list + ".numbers.max";
				break;
			case "AVERAGE":
				code = list + ".average";
				break;
			case "MEDIAN":
				code = list + ".median";
				break;
			case "MODE":
				// As a list of numbers can contain more than one mode,
				// the returned result is provided as an array.
				// Mode of [3, "x", "x", 1, 1, 2, "3"] -> ["x", 1].
				code = "math_modes(" + list + ")";
				break;
			case "STD_DEV":
				code = list + ".standard_deviation";
				break;
			case "RANDOM":
				code = list + "[rand(" + list + ".size)]";
				break;
			default:
				throw new Exception("Unknown operator: " + func);
			}
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] math_modulo(MathModuloBlock block)
		{
			// Remainder computation.
			var argument0 = Blockly.Ruby.valueToCode(block, "DIVIDEND",
				Blockly.Ruby.ORDER_MULTIPLICATIVE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var argument1 = Blockly.Ruby.valueToCode(block, "DIVISOR",
				Blockly.Ruby.ORDER_MULTIPLICATIVE);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var code = argument0 + " % " + argument1;
			return new object[] { code, ORDER_MULTIPLICATIVE };
		}

		public object[] math_constrain(MathConstrainBlock block)
		{
			// Constrain a number between two limits.
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var argument1 = Blockly.Ruby.valueToCode(block, "LOW",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var argument2 = Blockly.Ruby.valueToCode(block, "HIGH",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument2)) argument2 = "float(\'inf\')";
			var code = "[[" + argument0 + ", " + argument1 + "].max, " +
				argument2 + "].min";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] math_random_int(MathRandomIntBlock block)
		{
			// Random integer between [X] and [Y].
			var argument0 = Blockly.Ruby.valueToCode(block, "FROM",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var argument1 = Blockly.Ruby.valueToCode(block, "TO",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var code = "rand(" + argument0 + ".." + argument1 + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] math_random_float(MathRandomFloatBlock block)
		{
			// Random fraction between 0 and 1.
			return new object[] { "rand", ORDER_FUNCTION_CALL };
		}
	}
}
