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
		public object[] lists_create_empty(ListsCreateEmptyBlock block)
		{
			// Create an empty list.
			return new object[] { "[]", ORDER_ATOMIC };
		}

		public object[] lists_create_with(ListsCreateWithBlock block)
		{
			// Create a list with any number of elements of any type.
			var code = new string[block.itemCount_];
			for (var n = 0; n < block.itemCount_; n++) {
				code[n] = Blockly.Ruby.valueToCode(block, "ADD" + n,
					Blockly.Ruby.ORDER_NONE);
				if (String.IsNullOrEmpty(code[n]))
					code[n] = "None";
			}
			var _code = "[" + code.Join(", ") + "]";
			return new object[] { _code, ORDER_ATOMIC };
		}

		public object[] lists_repeat(ListsRepeatBlock block)
		{
			// Create a list with one element repeated.
			var argument0 = Blockly.Ruby.valueToCode(block, "ITEM",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "None";
			var argument1 = Blockly.Ruby.valueToCode(block, "NUM",
				Blockly.Ruby.ORDER_MULTIPLICATIVE);
			if (String.IsNullOrEmpty(argument1)) argument1 = "0";
			var code = "[" + argument0 + "] * " + argument1;
			return new object[] { code, ORDER_MULTIPLICATIVE };
		}

		public object[] lists_length(ListsLengthBlock block)
		{
			// List length.
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "[]";
			return new object[] { argument0 + ".length", ORDER_FUNCTION_CALL };
		}

		public object[] lists_isEmpty(ListsIsEmptyBlock block)
		{
			// Is the list empty?
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "[]";
			var code = argument0 + ".empty?";
			return new object[] { code, ORDER_LOGICAL_NOT };
		}

		public object[] lists_indexOf(ListsIndexOfBlock block)
		{
			// Find an item in the list.
			var search = Blockly.Ruby.valueToCode(block, "FIND",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(search)) search = "[]";
			var list = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(list)) list = "\'\'";
			var finder = block.getFieldValue("END") == "FIRST" ? ".find_first" : ".find_last";
			var code = list + finder + "(" + search + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object lists_getIndex(ListsGetIndexBlock block)
		{
			// Get element at index.
			var mode = block.getFieldValue("MODE");
			if (String.IsNullOrEmpty(mode)) mode = "GET";
			var where = block.getFieldValue("WHERE");
			if (String.IsNullOrEmpty(where)) where = "FROM_START";
			var at = Blockly.Ruby.valueToCode(block, "AT",
				Blockly.Ruby.ORDER_UNARY_SIGN);
			if (String.IsNullOrEmpty(at)) at = "1";
			var list = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(list)) list = "[]";

			if (where == "FIRST") {
				if (mode == "GET") {
					var code = list + ".first";
					return new object[] { code, ORDER_FUNCTION_CALL };
				}
				else {
					var code = list + ".shift";
					if (mode == "GET_REMOVE") {
						return new object[] { code, ORDER_FUNCTION_CALL };
					}
					else if (mode == "REMOVE") {
						return code + "\n";
					}
				}
			}
			else if (where == "LAST") {
				if (mode == "GET") {
					var code = list + ".last";
					return new object[] { code, ORDER_MEMBER };
				}
				else {
					var code = list + ".pop";
					if (mode == "GET_REMOVE") {
						return new object[] { code, ORDER_FUNCTION_CALL };
					}
					else if (mode == "REMOVE") {
						return code + "\n";
					}
				}
			}
			else if (where == "FROM_START") {
				// Blockly uses one-based indicies.
				if (Blockly.isNumber(at)) {
					// If the index is a naked number, decrement it right now.
					at = (Script.ParseInt(at, 10) - 1).ToString();
				}
				else {
					// If the index is dynamic, decrement it in code.
					at = "(" + at + " - 1).to_i";
				}
				if (mode == "GET") {
					var code = list + "[" + at + "]";
					return new object[] { code, ORDER_MEMBER };
				}
				else {
					var code = list + ".delete_at(" + at + ")";
					if (mode == "GET_REMOVE") {
						return new object[] { code, ORDER_FUNCTION_CALL };
					}
					else if (mode == "REMOVE") {
						return code + "\n";
					}
				}
			}
			else if (where == "FROM_END") {
				if (mode == "GET") {
					var code = list + "[-" + at + "]";
					return new object[] { code, ORDER_MEMBER };
				}
				else {
					var code = list + ".delete_at(-" + at + ")";
					if (mode == "GET_REMOVE") {
						return new object[] { code, ORDER_FUNCTION_CALL };
					}
					else if (mode == "REMOVE") {
						return code + "\n";
					}
				}
			}
			else if (where == "RANDOM") {
				if (mode == "GET") {
					var functionName = Blockly.Ruby.provideFunction_(
						"lists_random_item",
						new string[] { "def " + Blockly.Ruby.FUNCTION_NAME_PLACEHOLDER_ + "(myList)",
						 "  myList[rand(myList.size)]",
						 "end" });
					var code = functionName + "(" + list + ")";
					return new object[] { code, ORDER_FUNCTION_CALL };
				}
				else {
					var functionName = Blockly.Ruby.provideFunction_(
						"lists_remove_random_item",
						new string[] { "def " + Blockly.Ruby.FUNCTION_NAME_PLACEHOLDER_ + "(myList)",
						 "  myList.delete_at(rand(myList.size))",
						 "end" });
					var code = functionName + "(" + list + ")";
					if (mode == "GET_REMOVE") {
						return new object[] { code, ORDER_FUNCTION_CALL };
					}
					else if (mode == "REMOVE") {
						return code + "\n";
					}
				}
			}
			throw new Exception("Unhandled combination (lists_getIndex).");
		}

		public string lists_setIndex(ListsSetIndexBlock block)
		{
			// Set element at index.
			var list = Blockly.Ruby.valueToCode(block, "LIST",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(list)) list = "[]";
			var mode = block.getFieldValue("MODE");
			if (String.IsNullOrEmpty(mode)) mode = "GET";
			var where = block.getFieldValue("WHERE");
			if (String.IsNullOrEmpty(where)) where = "FROM_START";
			var at = Blockly.Ruby.valueToCode(block, "AT",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(at)) at = "1";
			var value = Blockly.Ruby.valueToCode(block, "TO",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(value)) value = "None";

			if (where == "FIRST") {
				if (mode == "SET") {
					return list + "[0] = " + value + "\n";
				}
				else if (mode == "INSERT") {
					return list + ".unshift(" + value + ")\n";
				}
			}
			else if (where == "LAST") {
				if (mode == "SET") {
					return list + "[-1] = " + value + "\n";
				}
				else if (mode == "INSERT") {
					return list + ".push(" + value + ")\n";
				}
			}
			else if (where == "FROM_START") {
				// Blockly uses one-based indicies.
				if (Blockly.isNumber(at)) {
					// If the index is a naked number, decrement it right now.
					at = (Script.ParseInt(at, 10) - 1).ToString();
				}
				else {
					// If the index is dynamic, decrement it in code.
					at = "(" + at + " - 1).to_i";
				}
				if (mode == "SET") {
					return list + "[" + at + "] = " + value + "\n";
				}
				else if (mode == "INSERT") {
					return list + ".insert(" + at + ", " + value + ")\n";
				}
			}
			else if (where == "FROM_END") {
				if (mode == "SET") {

					// Blockly uses one-based indicies.
					if (Blockly.isNumber(at)) {
						// If the index is a naked number, decrement it right now.
						at = Script.ParseInt(at, 10).ToString();
					}
					else {
						// If the index is dynamic, decrement it in code.
						at = "(" + at + ").to_i";
					}

					return list + "[-" + at + "] = " + value + "\n";
				}
				else if (mode == "INSERT") {

					// Blockly uses one-based indicies.
					if (Blockly.isNumber(at)) {
						// If the index is a naked number, decrement it right now.
						at = (Script.ParseInt(at, 10) + 1).ToString();
					}
					else {
						// If the index is dynamic, decrement it in code.
						at = "(" + at + " + 1).to_i";
					}

					return list + ".insert(-" + at + ", " + value + ")\n";
				}
			}
			else if (where == "RANDOM") {
				if (mode == "SET") {
					var functionName = Blockly.Ruby.provideFunction_(
						"lists_set_random_item",
						new string[] { "def " + Blockly.Ruby.FUNCTION_NAME_PLACEHOLDER_ + "(myList, value)",
						 "  myList[rand(myList.size)] = value",
						 "end"});
					var code = functionName + "(" + list + ", " + value + ")\n";
					return code;
				}
				else if (mode == "INSERT") {
					var functionName = Blockly.Ruby.provideFunction_(
						"lists_insert_random_item",
						new string[] { "def " + Blockly.Ruby.FUNCTION_NAME_PLACEHOLDER_ + "(myList, value)",
						 "  myList.insert(rand(myList.size), value)",
						 "end" });
					var code = functionName + "(" + list + ", " + value + ")\n";
					return code;
				}
			}
			throw new Exception("Unhandled combination (lists_setIndex).");
		}

		public object[] lists_getSublist(ListsGetSublistBlock block)
		{
			var functionName = Blockly.Ruby.provideFunction_(
				"lists_sublist",
				new string[] { "def " + Blockly.Ruby.FUNCTION_NAME_PLACEHOLDER_ + "(myList, range)",
				 "  myList[range] || []",
				 "end" });
			// Get sublist.
			var list = Blockly.Ruby.valueToCode(block, "LIST",
				Blockly.Ruby.ORDER_MEMBER);
			if (String.IsNullOrEmpty(list)) list = "[]";
			var where1 = block.getFieldValue("WHERE1");
			var where2 = block.getFieldValue("WHERE2");
			var at1 = Blockly.Ruby.valueToCode(block, "AT1",
				Blockly.Ruby.ORDER_ADDITIVE);
			if (String.IsNullOrEmpty(at1)) at1 = "1";
			var at2 = Blockly.Ruby.valueToCode(block, "AT2",
				Blockly.Ruby.ORDER_ADDITIVE);
			if (String.IsNullOrEmpty(at2)) at2 = "1";
			if (where1 == "FIRST" || (where1 == "FROM_START" && at1 == "1")) {
				at1 = "0";
			}
			else if (where1 == "FROM_START") {
				// Blockly uses one-based indicies.
				if (Blockly.isNumber(at1)) {
					at1 = (Script.ParseInt(at1, 10) - 1).ToString();
				}
				else {
					at1 = at1 + ".to_i - 1)";
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
					at2 = "-" + at2 + ".to_i";
				}
			}
			var code = functionName + "(" + list + ", " + at1 + ".." + at2 + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}
	}
}
