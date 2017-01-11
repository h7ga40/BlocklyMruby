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
		public node controls_repeat(ControlsRepeatBlock block)
		{
			// Repeat n times (internal number).
			var repeats = new int_node(this, Script.ParseInt(block.getFieldValue("TIMES"), 10));
			var branch = statementToCode(block, "DO");
			if (branch == null) branch = new nil_node(this);
			return new call_node(this, repeats, intern("times"), new List<node>(), new block_node(this, new List<node>(), branch, false));
		}

		public node controls_repeat_ext(ControlsRepeatExtBlock block)
		{
			// Repeat n times (external number).
			var repeats = valueToCode(block, "TIMES");
			if (repeats == null) repeats = new int_node(this, 0);
			if (repeats is int_node) {
			}
			else {
				repeats = new call_node(this, repeats, intern("to_i"), new List<node>(), null);
			}
			var branch = statementToCode(block, "DO");
			if (branch == null) branch = new nil_node(this);
			return new call_node(this, repeats, intern("times"), new List<node>(), new block_node(this, new List<node>(), branch, false));
		}

		public node controls_whileUntil(ControlsWhileUntilBlock block)
		{
			// Do while/until loop.
			var until = block.getFieldValue("MODE") == "UNTIL";
			var argument0 = valueToCode(block, "BOOL");
			if (argument0 == null) argument0 = new false_node(this);
			var branch = statementToCode(block, "DO");
			if (branch == null) branch = new nil_node(this);
			if (until)
				return new until_node(this, argument0, branch);
			else
				return new while_node(this, argument0, branch);
		}

		public node controls_for(ControlsForBlock block)
		{
			// For loop.
			var lv = local_switch();

			var loopVar = local_add_f(block.getFieldValue("VAR"));
			var fromVal = valueToCode(block, "FROM");
			if (fromVal == null) fromVal = new int_node(this, 0);
			var toVal = valueToCode(block, "TO");
			if (toVal == null) toVal = new int_node(this, 0);
			var increment = valueToCode(block, "BY");
			var branch = statementToCode(block, "DO");
			if (branch == null) branch = new nil_node(this);

			if (fromVal is int_node && toVal is int_node &&
				(increment == null || increment is int_node)) {

				if (increment == null) increment = new int_node(this, 1);

				// All parameters are simple numbers.
			}
			else {
				fromVal = new call_node(this, fromVal, intern("to_f"), new List<node>(), null);
				toVal = new call_node(this, toVal, intern("to_f"), new List<node>(), null);
				if (increment == null)
					increment = new float_node(this, 1);
				else
					increment = new call_node(this, increment, intern("to_f"), new List<node>(), null);
			}

			local_resume(lv);

			var arg = new hash_node(this, new List<hash_node.kv_t>() {
				new hash_node.kv_t(new sym_node(this, intern("from")), fromVal),
				new hash_node.kv_t(new sym_node(this, intern("to")), toVal),
				new hash_node.kv_t(new sym_node(this, intern("by")), increment),
			});
			var exec = new block_node(this, new List<node>() { new arg_node(this, loopVar) }, branch, false);
			return new fcall_node(this, intern("for_loop"), new List<node>() { arg }, exec);
		}

		public node controls_forEach(ControlsForEachBlock block)
		{
			// For each loop.
			var lv = local_switch();

			var loopVar = local_add_f(block.getFieldValue("VAR"));
			var argument0 = valueToCode(block, "LIST");
			if (argument0 == null) argument0 = new array_node(this, new List<node>());
			var branch = statementToCode(block, "DO");
			if (branch == null) branch = new nil_node(this);

			local_resume(lv);

			var exec = new block_node(this, new List<node>() { new arg_node(this, loopVar) }, branch, false);
			return new call_node(this, argument0, intern("each"), new List<node>(), exec);
		}

		public node controls_flow_statements(ControlsFlowStatementsBlock block)
		{
			// Flow statements: continue, break.
			switch (block.getFieldValue("FLOW")) {
			case "BREAK":
				return new break_node(this, null);
			case "CONTINUE":
				return new next_node(this, null);
			}
			throw new Exception("Unknown flow statement.");
		}
	}
}
