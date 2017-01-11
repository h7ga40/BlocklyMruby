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
		public node math_number(MathNumberBlock block)
		{
			// Numeric value.
			var num = block.getFieldValue("NUM");
			return new_num_node(num);
		}

		static Dictionary<string, string> ARITHMETIC_OPERATORS = new Dictionary<string, string>() {
			{ "ADD", "+" },
			{ "MINUS", "-" },
			{ "MULTIPLY", "*" },
			{ "DIVIDE", "/" },
			{ "POWER", "**" }
		};

		public node math_arithmetic(MathArithmeticBlock block)
		{
			// Basic arithmetic operators, and power.
			var @operator = ARITHMETIC_OPERATORS[block.getFieldValue("OP")];
			var argument0 = valueToCode(block, "A");
			if (argument0 == null) argument0 = new int_node(this, 0);
			var argument1 = valueToCode(block, "B");
			if (argument1 == null) argument1 = new int_node(this, 0);
			var code = argument0 + @operator + argument1;
			return new call_node(this, argument0, intern(@operator), argument1);
		}

		public node math_single(Block block)
		{
			// Math operators with single operand.
			var @operator = block.getFieldValue("OP");
			node code = null;
			node arg;
			if (@operator == "NEG") {
				// Negation is a special case given its different operator precedence.
				code = valueToCode(block, "NUM");
				if (code == null) code = new int_node(this, 0);
				return new call_node(this, code, intern("-@"), (node)null);
			}
			if (@operator == "SIN" || @operator == "COS" || @operator == "TAN") {
				arg = valueToCode(block, "NUM");
				if (arg == null) arg = new int_node(this, 0);
			}
			else {
				arg = valueToCode(block, "NUM");
				if (arg == null) arg = new int_node(this, 0);
			}
			// First, handle cases which generate values that don't need parentheses
			// wrapping the code.
			var math = new const_node(this, intern("Math"));
			switch (@operator) {
			case "ABS":
				code = new call_node(this, arg, intern("abs"), new List<node>(), null);
				break;
			case "ROOT":
				code = new call_node(this, math, intern("sqrt"), new List<node>() { arg }, null);
				break;
			case "LN":
				code = new call_node(this, math, intern("log"), new List<node>() { arg }, null);
				break;
			case "LOG10":
				code = new call_node(this, math, intern("log10"), new List<node>() { arg }, null);
				break;
			case "EXP":
				code = new call_node(this, math, intern("exp"), new List<node>() { arg }, null);
				break;
			case "POW10":
				code = new call_node(this, new int_node(this, 10), intern("exp"), new List<node>() { arg }, null);
				break;
			case "ROUND":
				code = new call_node(this, arg, intern("round"), new List<node>(), null);
				break;
			case "ROUNDUP":
				code = new call_node(this, arg, intern("ceil"), new List<node>(), null);
				break;
			case "ROUNDDOWN":
				code = new call_node(this, arg, intern("floor"), new List<node>(), null);
				break;
			case "SIN":
				arg = new call_node(this, arg, intern("/"), new float_node(this, 180.0));
				arg = new call_node(this, arg, intern("*"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, math, intern("sin"), new List<node>() { arg }, null);
				break;
			case "COS":
				arg = new call_node(this, arg, intern("/"), new float_node(this, 180.0));
				arg = new call_node(this, arg, intern("*"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, math, intern("cos"), new List<node>() { arg }, null);
				break;
			case "TAN":
				arg = new call_node(this, arg, intern("/"), new float_node(this, 180.0));
				arg = new call_node(this, arg, intern("*"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, math, intern("tan"), new List<node>() { arg }, null);
				break;
			}
			if (code != null) {
				return code;
			}
			// Second, handle cases which generate values that may need parentheses
			// wrapping the code.
			switch (@operator) {
			case "ASIN":
				code = new call_node(this, math, intern("asin"), new List<node>() { arg }, null);
				code = new call_node(this, code, intern("/"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, code, intern("*"), new float_node(this, 180.0));
				break;
			case "ACOS":
				code = new call_node(this, math, intern("acos"), new List<node>() { arg }, null);
				code = new call_node(this, code, intern("/"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, code, intern("*"), new float_node(this, 180.0));
				break;
			case "ATAN":
				code = new call_node(this, math, intern("atan"), new List<node>() { arg }, null);
				code = new call_node(this, code, intern("/"), new colon2_node(this, math, intern("PI")));
				code = new call_node(this, code, intern("*"), new float_node(this, 180.0));
				break;
			default:
				throw new Exception("Unknown math operator: " + @operator);
			}
			return new begin_node(this, code, true);
		}

		public node math_constant(MathConstantBlock block)
		{
			// Constants: PI, E, the Golden Ratio, sqrt(2), 1/sqrt(2), INFINITY.
			var constant = block.getFieldValue("CONSTANT");
			var math = new const_node(this, intern("Math"));
			node code;
			switch (constant) {
			case "PI":
				return new colon2_node(this, math, intern("PI"));
			case "E":
				return new colon2_node(this, math, intern("E"));
			case "GOLDEN_RATIO":
				code = new call_node(this, math, intern("sqrt"), new List<node>() { new float_node(this, 5) }, null);
				code = new call_node(this, new float_node(this, 1), intern("+"), code);
				code = new call_node(this, new begin_node(this, code, true), intern("/"), new float_node(this, 2));
				return code;
			case "SQRT2":
				return new call_node(this, math, intern("sqrt"), new List<node>() { new float_node(this, 2) }, null);
			case "SQRT1_2":
				code = new call_node(this, new float_node(this, 1), intern("/"), new float_node(this, 2));
				return new call_node(this, math, intern("sqrt"), new List<node>() { code }, null);
			case "INFINITY":
				return new call_node(this, new float_node(this, 1), intern("/"), new float_node(this, 0));
			}
			return null;
		}

		public node math_number_property(MathNumberPropertyBlock block)
		{
			// Check if a number is even, odd, prime, whole, positive, or negative
			// or if it is divisible by certain number. Returns true or false.
			var number_to_check = valueToCode(block, "NUMBER_TO_CHECK");
			if (number_to_check == null) number_to_check = new int_node(this, 0);
			var dropdown_property = block.getFieldValue("PROPERTY");
			node code = null;
			if (dropdown_property == "PRIME") {
				return new fcall_node(this, intern("is_prime"), new List<node>() { number_to_check }, null);
			}
			switch (dropdown_property) {
			case "EVEN":
				return new call_node(this, number_to_check, intern("even?"), new List<node>(), null);
			case "ODD":
				return new call_node(this, number_to_check, intern("odd?"), new List<node>(), null);
			case "WHOLE":
				code = new call_node(this, number_to_check, intern("%"), new int_node(this, 1));
				return new call_node(this, code, intern("=="), new int_node(this, 0));
			case "POSITIVE":
				return new call_node(this, number_to_check, intern(">"), new List<node>() { new int_node(this, 0) }, null);
			case "NEGATIVE":
				return new call_node(this, number_to_check, intern("<"), new List<node>() { new int_node(this, 0) }, null);
			case "DIVISIBLE_BY":
				var divisor = valueToCode(block, "DIVISOR");
				// If "divisor" is some code that evals to 0, Ruby will raise an error.
				if (divisor == null || (divisor is int_node && ((int_node)divisor).to_i() == 0)
					 || (divisor is float_node && ((float_node)divisor).to_f() == 0.0)) {
					return new false_node(this);
				}
				code = new call_node(this, number_to_check, intern("%"), divisor);
				return new call_node(this, code, intern("=="), new int_node(this, 0));
			}
			return null;
		}

		public node math_change(MathChangeBlock block)
		{
			// Add to a variable in place.
			var argument0 = valueToCode(block, "DELTA");
			if (argument0 == null) argument0 = new int_node(this, 0);
			var varName = get_var_name(block.getFieldValue("VAR"));
			return new op_asgn_node(this, new_var_node(varName), intern("+"), argument0);
		}

		// Rounding functions have a single operand.
		public node math_round(MathRoundBlock block) { return math_single(block); }
		// Trigonometry functions have a single operand.
		public node math_trig(MathTrigBlock block) { return math_single(block); }

		public node math_on_list(MathOnListBlock block)
		{
			// Math functions for lists.
			var func = block.getFieldValue("OP");
			var list = valueToCode(block, "LIST");
			if (list == null) list = new array_node(this, new List<node>());
			node code;
			switch (func) {
			case "SUM":
				code = new call_node(this, list, intern("sum"), new List<node>(), null);
				break;
			case "MIN":
				code = new call_node(this, list, intern("numbers"), new List<node>(), null);
				code = new call_node(this, code, intern("min"), new List<node>(), null);
				break;
			case "MAX":
				code = new call_node(this, list, intern("numbers"), new List<node>(), null);
				code = new call_node(this, code, intern("max"), new List<node>(), null);
				break;
			case "AVERAGE":
				code = new call_node(this, list, intern("average"), new List<node>(), null);
				break;
			case "MEDIAN":
				code = new call_node(this, list, intern("median"), new List<node>(), null);
				break;
			case "MODE":
				// As a list of numbers can contain more than one mode,
				// the returned result is provided as an array.
				// Mode of [3, "x", "x", 1, 1, 2, "3"] -> ["x", 1].
				code = new fcall_node(this, intern("math_modes"), new List<node>() { list }, null);
				break;
			case "STD_DEV":
				code = new call_node(this, list, intern("standard_deviation"), new List<node>(), null);
				break;
			case "RANDOM":
				code = new call_node(this, list, intern("size"), new List<node>(), null);
				code = new fcall_node(this, intern("rand"), new List<node>() { code }, null);
				code = new call_node(this, list, intern("[]"), new List<node>() { code }, null);
				break;
			default:
				throw new Exception("Unknown operator: " + func);
			}
			return code;
		}

		public node math_modulo(MathModuloBlock block)
		{
			// Remainder computation.
			var argument0 = valueToCode(block, "DIVIDEND");
			if (argument0 == null) argument0 = new int_node(this, 0);
			var argument1 = valueToCode(block, "DIVISOR");
			if (argument1 == null) argument1 = new int_node(this, 0);
			return new call_node(this, argument0, intern("%"), argument1);
		}

		public node math_constrain(MathConstrainBlock block)
		{
			// Constrain a number between two limits.
			var argument0 = valueToCode(block, "VALUE");
			if (argument0 == null) argument0 = new int_node(this, 0);
			var argument1 = valueToCode(block, "LOW");
			if (argument1 == null) argument1 = new int_node(this, 0);
			var argument2 = valueToCode(block, "HIGH");
			if (argument2 == null) argument2 = new fcall_node(this, intern("Float"), new List<node>() { new str_node(this, "inf") }, null);
			node code = new array_node(this, new List<node>() { argument0, argument1 });
			code = new call_node(this, code, intern("max"), new List<node>(), null);
			code = new array_node(this, new List<node>() { code, argument2 });
			return new call_node(this, code, intern("min"), new List<node>(), null);
		}

		public node math_random_int(MathRandomIntBlock block)
		{
			// Random integer between [X] and [Y].
			var argument0 = valueToCode(block, "FROM");
			if (argument0 == null) argument0 = new int_node(this, 0);
			var argument1 = valueToCode(block, "TO");
			if (argument1 == null) argument1 = new int_node(this, 0);
			var code = new dot2_node(this, argument0, argument1);
			return new fcall_node(this, intern("rand"), new List<node>() { code }, null);
		}

		public node math_random_float(MathRandomFloatBlock block)
		{
			// Random fraction between 0 and 1.
			return new fcall_node(this, intern("rand"), new List<node>(), null);
		}
	}
}
