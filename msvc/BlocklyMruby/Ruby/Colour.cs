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
		public object[] colour_picker(ColourPickerBlock block)
		{
			// Colour picker.
			var code = "\'" + block.getFieldValue("COLOUR") + "\'";
			return new object[] { code, ORDER_ATOMIC };
		}

		public object[] colour_random(ColourRandomBlock block)
		{
			// Generate a random colour.
			var code = "\'#%06x\' % rand(2**24 - 1)";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] colour_rgb(ColourRGBBlock block)
		{
			// Compose a colour from RGB components expressed as percentages.
			var r = Blockly.Ruby.valueToCode(block, "RED", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(r)) r = "0";
			var g = Blockly.Ruby.valueToCode(block, "GREEN", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(g)) g = "0";
			var b = Blockly.Ruby.valueToCode(block, "BLUE", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(b)) b = "0";
			var code = "colour_rgb(" + r + ", " + g + ", " + b + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] colour_blend(ColourBlendBlock block)
		{
			// Blend two colours together.
			var colour1 = Blockly.Ruby.valueToCode(block, "COLOUR1",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(colour1)) colour1 = "\'#000000\'";
			var colour2 = Blockly.Ruby.valueToCode(block, "COLOUR2",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(colour2)) colour2 = "\'#000000\'";
			var ratio = Blockly.Ruby.valueToCode(block, "RATIO",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(ratio)) ratio = "0";
			var code = "colour_blend(" + colour1 + ", " + colour2 + ", " + ratio + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}
	}
}