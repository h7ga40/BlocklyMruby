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
		public node lists_create_empty(ListsCreateEmptyBlock block)
		{
			// Create an empty list.
			var p = new List<node>();
			return new array_node(this, p);
		}

		public node lists_create_with(ListsCreateWithBlock block)
		{
			// Create a list with any number of elements of any type.
			var p = new List<node>(block.itemCount_);
			for (var n = 0; n < block.itemCount_; n++) {
				var i = valueToCode(block, "ADD" + n);
				if (i == null)
					i = new nil_node(this);
				p.Add(i);
			}
			return new array_node(this, p);
		}

		public node lists_repeat(ListsRepeatBlock block)
		{
			// Create a list with one element repeated.
			var argument0 = valueToCode(block, "ITEM");
			if (argument0 == null) argument0 = new nil_node(this);
			var argument1 = valueToCode(block, "NUM");
			if (argument1 == null) argument1 = new int_node(this, 0);
			var a = new array_node(this, new List<node>() { argument0 });
			return new call_node(this, a, intern("*"), argument1);
		}

		public node lists_length(ListsLengthBlock block)
		{
			// List length.
			var argument0 = valueToCode(block, "VALUE");
			if (argument0 == null) argument0 = new array_node(this, new List<node>());
			return new call_node(this, argument0, intern("length"), new List<node>(), null);
		}

		public node lists_isEmpty(ListsIsEmptyBlock block)
		{
			// Is the list empty?
			var argument0 = valueToCode(block, "VALUE");
			if (argument0 == null) argument0 = new array_node(this, new List<node>());
			return new call_node(this, argument0, intern("empty?"), new List<node>(), null);
		}

		public node lists_indexOf(ListsIndexOfBlock block)
		{
			// Find an item in the list.
			var search = valueToCode(block, "FIND");
			if (search == null) search = new str_node(this, "");
			var list = valueToCode(block, "VALUE");
			if (list == null) list = new array_node(this, new List<node>());
			var finder = block.getFieldValue("END") == "FIRST" ? "find_first" : "find_last";
			var p = new List<node>() {
				search
			};
			return new call_node(this, list, intern("finder"), p, null);
		}

		public node lists_getIndex(ListsGetIndexBlock block)
		{
			// Get element at index.
			var mode = block.getFieldValue("MODE");
			if (String.IsNullOrEmpty(mode)) mode = "GET";
			var where = block.getFieldValue("WHERE");
			if (String.IsNullOrEmpty(where)) where = "FROM_START";
			var at = valueToCode(block, "AT");
			if (at == null) at = new int_node(this, 1);
			var list = valueToCode(block, "VALUE");
			if (list == null) list = new array_node(this, new List<node>());

			if (where == "FIRST") {
				if (mode == "GET") {
					return new call_node(this, list, intern("first"), new List<node>(), null);
				}
				else {
					if (mode == "GET_REMOVE") {
						return new call_node(this, list, intern("shift"), new List<node>(), null);
					}
					else if (mode == "REMOVE") {
						return new call_node(this, list, intern("shift"), new List<node>(), null);
					}
				}
			}
			else if (where == "LAST") {
				if (mode == "GET") {
					return new call_node(this, list, intern("last"), new List<node>(), null);
				}
				else {
					var code = list + ".pop";
					if (mode == "GET_REMOVE") {
						return new call_node(this, list, intern("pop"), new List<node>(), null);
					}
					else if (mode == "REMOVE") {
						return new call_node(this, list, intern("pop"), new List<node>(), null);
					}
				}
			}
			else if (where == "FROM_START") {
				// Blockly uses one-based indicies.
				if (at is int_node) {
					// If the index is a naked number, decrement it right now.
					at = new int_node(this, (int)(((int_node)at).to_i() - 1));
				}
				else {
					// If the index is dynamic, decrement it in code.
					at = new begin_node(this, new call_node(this, at, intern("-"), new int_node(this, 1)), true);
					at = new call_node(this, at, intern("to_i"), new List<node>(), null);
				}
				if (mode == "GET") {
					return new call_node(this, list, intern("[]"), new List<node>() { at }, null);
				}
				else if (mode == "GET_REMOVE") {
					return new call_node(this, list, intern("delete_at"), new List<node>() { at }, null);
				}
				else if (mode == "REMOVE") {
					return new call_node(this, list, intern("delete_at"), new List<node>() { at }, null);
				}
			}
			else if (where == "FROM_END") {
				at = new call_node(this, at, intern("-@"), (node)null);
				if (mode == "GET") {
					return new call_node(this, list, intern("[]"), new List<node>() { at }, null);
				}
				else if (mode == "GET_REMOVE") {
					return new call_node(this, list, intern("delete_at"), new List<node>() { at }, null);
				}
				else if (mode == "REMOVE") {
					return new call_node(this, list, intern("delete_at"), new List<node>() { at }, null);
				}
			}
			else if (where == "RANDOM") {
				if (mode == "GET") {
					return new fcall_node(this, intern("lists_random_item"), new List<node>() { list }, null);
				}
				else {
					if (mode == "GET_REMOVE") {
						return new fcall_node(this, intern("lists_remove_random_item"), new List<node>() { list }, null);
					}
					else if (mode == "REMOVE") {
						return new fcall_node(this, intern("lists_remove_random_item"), new List<node>() { list }, null);
					}
				}
			}
			throw new Exception("Unhandled combination (lists_getIndex).");
		}

		public node lists_setIndex(ListsSetIndexBlock block)
		{
			// Set element at index.
			var list = valueToCode(block, "LIST");
			if (list == null) list = new array_node(this, new List<node>());
			var mode = block.getFieldValue("MODE");
			if (String.IsNullOrEmpty(mode)) mode = "GET";
			var where = block.getFieldValue("WHERE");
			if (String.IsNullOrEmpty(where)) where = "FROM_START";
			var at = valueToCode(block, "AT");
			if (at == null) at = new int_node(this, 1);
			var value = valueToCode(block, "TO");
			if (value == null) value = new nil_node(this);

			if (where == "FIRST") {
				if (mode == "SET") {
					return new asgn_node(this, new call_node(this, list, intern("[]"), new List<node>() { new int_node(this, 0) }, null), value);
				}
				else if (mode == "INSERT") {
					return new call_node(this, list, intern("unshift"), new List<node>() { value }, null);
				}
			}
			else if (where == "LAST") {
				if (mode == "SET") {
					return new asgn_node(this, new call_node(this, list, intern("[]"), new List<node>() { new int_node(this, -1) }, null), value);
				}
				else if (mode == "INSERT") {
					return new call_node(this, list, intern("push"), new List<node>() { value }, null);
				}
			}
			else if (where == "FROM_START") {
				// Blockly uses one-based indicies.
				if (at is int_node) {
					// If the index is a naked number, decrement it right now.
					at = new int_node(this, (int)(((int_node)at).to_i() - 1));
				}
				else {
					// If the index is dynamic, decrement it in code.
					at = new begin_node(this, new call_node(this, at, intern("-"), new int_node(this, 1)), true);
					at = new call_node(this, at, intern("to_i"), new List<node>(), null);
				}
				if (mode == "SET") {
					return new asgn_node(this, new call_node(this, list, intern("[]"), new List<node>() { at }, null), value);
				}
				else if (mode == "INSERT") {
					return new call_node(this, list, intern("insert"), new List<node>() { at, value }, null);
				}
			}
			else if (where == "FROM_END") {
				if (mode == "SET") {
					// Blockly uses one-based indicies.
					if (at is int_node) {
						// If the index is a naked number, decrement it right now.
					}
					else {
						// If the index is dynamic, decrement it in code.
						at = new call_node(this, at, intern("to_i"), new List<node>(), null);
					}
					return new asgn_node(this, new call_node(this, list, intern("[]"), new List<node>() { at }, null), value);
				}
				else if (mode == "INSERT") {
					// Blockly uses one-based indicies.
					if (at is int_node) {
						// If the index is a naked number, decrement it right now.
						at = new int_node(this, (int)(((int_node)at).to_i() + 1));
					}
					else {
						// If the index is dynamic, decrement it in code.
						at = new begin_node(this, new call_node(this, at, intern("+"), new int_node(this, 1)), true);
						at = new call_node(this, at, intern("to_i"), new List<node>(), null);
					}

					at = new call_node(this, at, intern("-@"), (node)null);
					return new call_node(this, list, intern("insert"), new List<node>() { at, value }, null);
				}
			}
			else if (where == "RANDOM") {
				if (mode == "SET") {
					return new fcall_node(this, intern("lists_set_random_item"), new List<node>() { list, value }, null);
				}
				else if (mode == "INSERT") {
					return new fcall_node(this, intern("lists_insert_random_item"), new List<node>() { list, value }, null);
				}
			}
			throw new Exception("Unhandled combination (lists_setIndex).");
		}

		public node lists_getSublist(ListsGetSublistBlock block)
		{
			// Get sublist.
			var list = valueToCode(block, "LIST");
			if (list == null) list = new array_node(this, new List<node>());
			var where1 = block.getFieldValue("WHERE1");
			var where2 = block.getFieldValue("WHERE2");
			var at1 = valueToCode(block, "AT1");
			if (at1 == null) at1 = new int_node(this, 1);
			var at2 = valueToCode(block, "AT2");
			if (at2 == null) at2 = new int_node(this, 1);
			if (where1 == "FIRST" || (where1 == "FROM_START" && at1 is int_node && ((int_node)at1).to_i() == 1)) {
				at1 = new int_node(this, 0);
			}
			else if (where1 == "FROM_START") {
				// Blockly uses one-based indicies.
				if (at1 is int_node) {
					at1 = new int_node(this, (int)(((int_node)at1).to_i() - 1));
				}
				else {
					at1 = new call_node(this, at1, intern("to_i"), new List<node>(), null);
					at1 = new call_node(this, at1, intern("-"), new int_node(this, 1));
				}
			}
			else if (where1 == "FROM_END") {
				if (at1 is int_node) {
					at1 = new int_node(this, (int)(-((int_node)at1).to_i()));
				}
				else {
					at1 = new call_node(this, at1, intern("-@"), (node)null);
					at1 = new call_node(this, at1, intern("to_i"), new List<node>(), null);
				}
			}
			if (where2 == "LAST" || (where2 == "FROM_END" && at2 is int_node && ((int_node)at2).to_i() == 1)) {
				at2 = new int_node(this, -1);
			}
			else if (where2 == "FROM_START") {
				if (at2 is int_node) {
					at2 = new int_node(this, (int)(((int_node)at2).to_i() - 1));
				}
				else {
					at2 = new call_node(this, at2, intern("to_i"), new List<node>(), null);
					at2 = new call_node(this, at2, intern("-"), new int_node(this, 1));
				}
			}
			else if (where2 == "FROM_END") {
				if (at2 is int_node) {
					at2 = new int_node(this, (int)(-((int_node)at2).to_i()));
				}
				else {
					at2 = new call_node(this, at2, intern("-@"), (node)null);
					at2 = new call_node(this, at2, intern("to_i"), new List<node>(), null);
				}
			}
			return new fcall_node(this, intern("lists_sublist"), new List<node>() { list, at1, at2 }, null);
		}
	}
}
