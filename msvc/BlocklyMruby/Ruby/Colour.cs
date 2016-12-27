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
			var functionName = Blockly.Ruby.provideFunction_(
				"colour_rgb",
				new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() + "(r, g, b)",
				  "  r = (2.55 * [100, [0, r].max].min).round",
				  "  g = (2.55 * [100, [0, g].max].min).round",
				  "  b = (2.55 * [100, [0, b].max].min).round",
				  "  \'#%02x%02x%02x\' % [r, g, b]",
				  "end" });
			var r = Blockly.Ruby.valueToCode(block, "RED", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(r)) r = "0";
			var g = Blockly.Ruby.valueToCode(block, "GREEN", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(g)) g = "0";
			var b = Blockly.Ruby.valueToCode(block, "BLUE", Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(b)) b = "0";
			var code = functionName + "(" + r + ", " + g + ", " + b + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}

		public object[] colour_blend(ColourBlendBlock block)
		{
			// Blend two colours together.
			var functionName = Blockly.Ruby.provideFunction_(
				"colour_blend",
				new string[] { "def " + Blockly.Ruby.FunctionNamePlaceholder() +
				  "(colour1, colour2, ratio) ",
				  "  _, r1, g1, b1 = colour1.unpack('A1A2A2A2').map {|x| x.to_i(16)}",
				  "  _, r2, g2, b2 = colour2.unpack('A1A2A2A2').map {|x| x.to_i(16)}",
				  "  ratio = [1, [0, ratio].max].min",
				  "  r = (r1 * (1 - ratio) + r2 * ratio).round",
				  "  g = (g1 * (1 - ratio) + g2 * ratio).round",
				  "  b = (b1 * (1 - ratio) + b2 * ratio).round",
				  "  '#%02x%02x%02x' % [r, g, b]",
				  "end" });
			var colour1 = Blockly.Ruby.valueToCode(block, "COLOUR1",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(colour1)) colour1 = "\'#000000\'";
			var colour2 = Blockly.Ruby.valueToCode(block, "COLOUR2",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(colour2)) colour2 = "\'#000000\'";
			var ratio = Blockly.Ruby.valueToCode(block, "RATIO",
				Blockly.Ruby.ORDER_NONE);
			if (String.IsNullOrEmpty(ratio)) ratio = "0";
			var code = functionName + "(" + colour1 + ", " + colour2 + ", " + ratio + ")";
			return new object[] { code, ORDER_FUNCTION_CALL };
		}
	}
}