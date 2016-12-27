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
		public object[] variables_get(VariablesGetBlock block)
		{
			// Variable getter.
			var code = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("VAR"),
				Blockly.Variables.NAME_TYPE);
			return new object[] { code, ORDER_ATOMIC };
		}

		public string variables_set(VariablesSetBlock block)
		{
			// Variable setter.
			var argument0 = Blockly.Ruby.valueToCode(block, "VALUE",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(argument0)) argument0 = "0";
			var varName = Blockly.Ruby.variableDB_.getRubyName(block.getFieldValue("VAR"),
				Blockly.Variables.NAME_TYPE);
			return varName + " = " + argument0 + "\n";
		}
	}
}
