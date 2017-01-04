/**
 * @license
 * Visual Blocks Editor
 *
 * Copyright 2012 Google Inc.
 * https://developers.google.com/blockly/
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @fileoverview Math blocks for Blockly.
 * @author q.neutron@gmail.com (Quynh Neutron)
 */
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	public class Math
	{
		/**
		 * Common HSV hue for all blocks in this category.
		 */
		public static int HUE = 230;
	}

	[ComVisible(true)]
	public class MathNumberBlock : Block
	{
		public const string type_name = "math_number";

		public MathNumberBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for numeric value.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.setHelpUrl(Msg.MATH_NUMBER_HELPURL);
			this.setColour(Math.HUE);
			this.appendDummyInput()
				.appendField(new Blockly.FieldNumber("0", "-Infinity", "Infinity", 0), "NUM");
			this.setOutput(true, "Number");
			// Assign "this" to a variable for use in the tooltip closure below.
			var thisBlock = this;
			// Number block is trivial.  Use tooltip of parent block if it exists.
			this.setTooltip(new Func<string>(() => {
				var parent = thisBlock.getParent();
				return (parent != null && parent.getInputsInline() && !String.IsNullOrEmpty(parent.tooltip)) ? parent.tooltip :
					Msg.MATH_NUMBER_TOOLTIP;
			}));
		}
	}

	[ComVisible(true)]
	public class MathArithmeticBlock : Block
	{
		public const string type_name = "math_arithmetic";

		public MathArithmeticBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for basic arithmetic operator.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = "%1 %2 %3",
				args0 = new object[] {
					new {
						type = "input_value",
						name = "A",
						check = "Number"
					},
					new {
						type = "field_dropdown",
						name = "OP",
						options = new [] {
							new [] { Msg.MATH_ADDITION_SYMBOL, "ADD" },
							new [] { Msg.MATH_SUBTRACTION_SYMBOL, "MINUS" },
							new [] { Msg.MATH_MULTIPLICATION_SYMBOL, "MULTIPLY" },
							new [] { Msg.MATH_DIVISION_SYMBOL, "DIVIDE" },
							new [] { Msg.MATH_POWER_SYMBOL, "POWER" }
						}
					},
					new {
						type = "input_value",
						name = "B",
						check = "Number"
					}
				},
				inputsInline = true,
				output = "Number",
				colour = Math.HUE,
				helpUrl = Msg.MATH_ARITHMETIC_HELPURL
			});
			// Assign "this" to a variable for use in the tooltip closure below.
			var thisBlock = this;
			this.setTooltip(new Func<string>(() => {
				switch (thisBlock.getFieldValue("OP")) {
				case "ADD": return Msg.MATH_ARITHMETIC_TOOLTIP_ADD;
				case "MINUS": return Msg.MATH_ARITHMETIC_TOOLTIP_MINUS;
				case "MULTIPLY": return Msg.MATH_ARITHMETIC_TOOLTIP_MULTIPLY;
				case "DIVIDE": return Msg.MATH_ARITHMETIC_TOOLTIP_DIVIDE;
				case "POWER": return Msg.MATH_ARITHMETIC_TOOLTIP_POWER;
				};
				return "";
			}));
		}
	}

	[ComVisible(true)]
	public class MathSingleBlock : Block
	{
		public const string type_name = "math_single";

		public MathSingleBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for advanced math operators with single operand.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = "%1 %2",
				args0 = new object[] {
					new {
						type = "field_dropdown",
						name = "OP",
						options = new [] {
							new [] { Msg.MATH_SINGLE_OP_ROOT, "ROOT" },
							new [] { Msg.MATH_SINGLE_OP_ABSOLUTE, "ABS" },
							new [] { "-", "NEG" },
							new [] { "ln", "LN" },
							new [] { "log10", "LOG10" },
							new [] { "e^", "EXP" },
							new [] { "10^", "POW10" }
						}
					},
					new {
						type = "input_value",
						name = "NUM",
						check = "Number"
					}
				},
				output = "Number",
				colour = Math.HUE,
				helpUrl = Msg.MATH_SINGLE_HELPURL
			});
			// Assign "this" to a variable for use in the tooltip closure below.
			var thisBlock = this;
			this.setTooltip(new Func<string>(() => {
				switch (thisBlock.getFieldValue("OP")) {
				case "ROOT": return Msg.MATH_SINGLE_TOOLTIP_ROOT;
				case "ABS": return Msg.MATH_SINGLE_TOOLTIP_ABS;
				case "NEG": return Msg.MATH_SINGLE_TOOLTIP_NEG;
				case "LN": return Msg.MATH_SINGLE_TOOLTIP_LN;
				case "LOG10": return Msg.MATH_SINGLE_TOOLTIP_LOG10;
				case "EXP": return Msg.MATH_SINGLE_TOOLTIP_EXP;
				case "POW10": return Msg.MATH_SINGLE_TOOLTIP_POW10;
				}
				return "";
			}));
		}
	}

	[ComVisible(true)]
	public class MathTrigBlock : Block
	{
		public const string type_name = "math_trig";

		public MathTrigBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for trigonometry operators.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = "%1 %2",
				args0 = new object[] {
					new {
						type = "field_dropdown",
						name = "OP",
						options = new [] {
							new [] { Msg.MATH_TRIG_SIN, "SIN" },
							new [] { Msg.MATH_TRIG_COS, "COS" },
							new [] { Msg.MATH_TRIG_TAN, "TAN" },
							new [] { Msg.MATH_TRIG_ASIN, "ASIN" },
							new [] { Msg.MATH_TRIG_ACOS, "ACOS" },
							new [] { Msg.MATH_TRIG_ATAN, "ATAN" }
						}
					},
					new {
						type = "input_value",
						name = "NUM",
						check = "Number"
					}
				},
				output = "Number",
				colour = Math.HUE,
				helpUrl = Msg.MATH_TRIG_HELPURL
			});
			// Assign "this" to a variable for use in the tooltip closure below.
			var thisBlock = this;
			this.setTooltip(new Func<string>(() => {
				switch (thisBlock.getFieldValue("OP")) {
				case "SIN": return Msg.MATH_TRIG_TOOLTIP_SIN;
				case "COS": return Msg.MATH_TRIG_TOOLTIP_COS;
				case "TAN": return Msg.MATH_TRIG_TOOLTIP_TAN;
				case "ASIN": return Msg.MATH_TRIG_TOOLTIP_ASIN;
				case "ACOS": return Msg.MATH_TRIG_TOOLTIP_ACOS;
				case "ATAN": return Msg.MATH_TRIG_TOOLTIP_ATAN;
				}
				return "";
			}));
		}
	}

	[ComVisible(true)]
	public class MathConstantBlock : Block
	{
		public const string type_name = "math_constant";

		public MathConstantBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for constants: PI, E, the Golden Ratio, sqrt(2), 1/sqrt(2), INFINITY.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = "%1",
				args0 = new[] {
					new {
						type = "field_dropdown",
						name = "CONSTANT",
						options = new [] {
							new [] { "\u03c0", "PI" },
							new [] { "e", "E" },
							new [] { "\u03c6", "GOLDEN_RATIO" },
							new [] { "sqrt(2)", "SQRT2" },
							new [] { "sqrt(\u00bd)", "SQRT1_2" },
							new [] { "\u221e", "INFINITY" }
						}
					}
				},
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_CONSTANT_TOOLTIP,
				helpUrl = Msg.MATH_CONSTANT_HELPURL
			});
		}
	}

	[ComVisible(true)]
	public class MathNumberPropertyBlock : Block
	{
		public const string type_name = "math_number_property";

		public MathNumberPropertyBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for checking if a number is even, odd, prime, whole, positive,
		 * negative or if it is divisible by certain number.
		 * @this Blockly.Block
		 */
		public void init()
		{
			var PROPERTIES = new[] {
				new [] {Msg.MATH_IS_EVEN, "EVEN"},
				new [] {Msg.MATH_IS_ODD, "ODD"},
				new [] {Msg.MATH_IS_PRIME, "PRIME"},
				new [] {Msg.MATH_IS_WHOLE, "WHOLE"},
				new [] {Msg.MATH_IS_POSITIVE, "POSITIVE"},
				new [] {Msg.MATH_IS_NEGATIVE, "NEGATIVE"},
				new [] {Msg.MATH_IS_DIVISIBLE_BY, "DIVISIBLE_BY"}
			};
			this.setColour(Math.HUE);
			this.appendValueInput("NUMBER_TO_CHECK")
				.setCheck("Number");
			var dropdown = new Blockly.FieldDropdown(PROPERTIES);
			dropdown.setValidator((option) => {
				var divisorInput = (option == "DIVISIBLE_BY");
				((MathNumberPropertyBlock)dropdown.sourceBlock_).updateShape_(divisorInput);
				return Script.Undefined;
			});
			this.appendDummyInput()
				.appendField(dropdown, "PROPERTY");
			this.setInputsInline(true);
			this.setOutput(true, "Boolean");
			this.setTooltip(Msg.MATH_IS_TOOLTIP);
		}

		/**
		 * Create XML to represent whether the "divisorInput" should be present.
		 * @return {Element} XML storage element.
		 * @this Blockly.Block
		 */
		public Element mutationToDom(bool opt_paramIds)
		{
			var container = Document.CreateElement("mutation");
			var divisorInput = (this.getFieldValue("PROPERTY") == "DIVISIBLE_BY");
			container.SetAttribute("divisor_input", divisorInput.ToString());
			return container;
		}

		/**
		 * Parse XML to restore the "divisorInput".
		 * @param {!Element} xmlElement XML storage element.
		 * @this Blockly.Block
		 */
		public void domToMutation(Element xmlElement)
		{
			var divisorInput = (xmlElement.GetAttribute("divisor_input") == "true");
			this.updateShape_(divisorInput);
		}

		/**
		 * Modify this block to have (or not have) an input for "is divisible by".
		 * @param {boolean} divisorInput True if this block has a divisor input.
		 * @private
		 * @this Blockly.Block
		 */
		public void updateShape_(bool divisorInput)
		{
			// Add or remove a Value Input.
			var inputExists = this.getInput("DIVISOR");
			if (divisorInput) {
				if (inputExists == null) {
					this.appendValueInput("DIVISOR")
						.setCheck("Number");
				}
			}
			else if (inputExists != null) {
				this.removeInput("DIVISOR");
			}
		}
	}

	[ComVisible(true)]
	public class MathChangeBlock : Block
	{
		public const string type_name = "math_change";

		public MathChangeBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for adding to a variable in place.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.MATH_CHANGE_TITLE,
				args0 = new object[] {
					new {
						type = "field_variable",
						name = "VAR",
						variable = Msg.MATH_CHANGE_TITLE_ITEM
					},
					new {
						type = "input_value",
						name = "DELTA",
						check = "Number"
					}
				},
				previousStatement = (Any<string, string[]>)null,
				nextStatement = (Any<string, string[]>)null,
				colour = Blockly.Variables.HUE,
				helpUrl = Msg.MATH_CHANGE_HELPURL
			});
			// Assign "this" to a variable for use in the tooltip closure below.
			var thisBlock = this;
			this.setTooltip(new Func<string>(() => {
				return Msg.MATH_CHANGE_TOOLTIP.Replace("%1",
					thisBlock.getFieldValue("VAR"));
			}));
		}
	}

	[ComVisible(true)]
	public class MathRoundBlock : Block
	{
		public const string type_name = "math_round";

		public MathRoundBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for rounding functions.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = "%1 %2",
				args0 = new object[] {
					new {
						type = "field_dropdown",
						name = "OP",
						options = new [] {
							new [] { Msg.MATH_ROUND_OPERATOR_ROUND, "ROUND" },
							new [] { Msg.MATH_ROUND_OPERATOR_ROUNDUP, "ROUNDUP" },
							new [] { Msg.MATH_ROUND_OPERATOR_ROUNDDOWN, "ROUNDDOWN" }
						}
					},
					new {
						type = "input_value",
						name = "NUM",
						check = "Number"
					}
				},
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_ROUND_TOOLTIP,
				helpUrl = Msg.MATH_ROUND_HELPURL
			});
		}
	}

	[ComVisible(true)]
	public class MathOnListBlock : Block
	{
		public const string type_name = "math_on_list";

		public MathOnListBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for evaluating a list of numbers to return sum, average, min, max,
		 * etc.  Some functions also work on text (min, max, mode, median).
		 * @this Blockly.Block
		 */
		public void init()
		{
			var OPERATORS = new[] {
				new [] {Msg.MATH_ONLIST_OPERATOR_SUM, "SUM"},
				new [] {Msg.MATH_ONLIST_OPERATOR_MIN, "MIN"},
				new [] {Msg.MATH_ONLIST_OPERATOR_MAX, "MAX"},
				new [] {Msg.MATH_ONLIST_OPERATOR_AVERAGE, "AVERAGE"},
				new [] {Msg.MATH_ONLIST_OPERATOR_MEDIAN, "MEDIAN"},
				new [] {Msg.MATH_ONLIST_OPERATOR_MODE, "MODE"},
				new [] {Msg.MATH_ONLIST_OPERATOR_STD_DEV, "STD_DEV"},
				new [] {Msg.MATH_ONLIST_OPERATOR_RANDOM, "RANDOM"}
			};
			// Assign "this" to a variable for use in the closures below.
			var thisBlock = this;
			this.setHelpUrl(Msg.MATH_ONLIST_HELPURL);
			this.setColour(Math.HUE);
			this.setOutput(true, "Number");
			var dropdown = new Blockly.FieldDropdown(OPERATORS, (newOp) => {
				thisBlock.updateType_(newOp);
				return Script.Undefined;
			});
			this.appendValueInput("LIST")
				.setCheck("Array")
				.appendField(dropdown, "OP");
			this.setTooltip(new Func<string>(() => {
				switch (thisBlock.getFieldValue("OP")) {
				case "SUM": return Msg.MATH_ONLIST_TOOLTIP_SUM;
				case "MIN": return Msg.MATH_ONLIST_TOOLTIP_MIN;
				case "MAX": return Msg.MATH_ONLIST_TOOLTIP_MAX;
				case "AVERAGE": return Msg.MATH_ONLIST_TOOLTIP_AVERAGE;
				case "MEDIAN": return Msg.MATH_ONLIST_TOOLTIP_MEDIAN;
				case "MODE": return Msg.MATH_ONLIST_TOOLTIP_MODE;
				case "STD_DEV": return Msg.MATH_ONLIST_TOOLTIP_STD_DEV;
				case "RANDOM": return Msg.MATH_ONLIST_TOOLTIP_RANDOM;
				}
				return "";
			}));
		}

		/**
		 * Modify this block to have the correct output type.
		 * @param {string} newOp Either "MODE" or some op than returns a number.
		 * @private
		 * @this Blockly.Block
		 */
		public void updateType_(string newOp)
		{
			if (newOp == "MODE") {
				this.outputConnection.setCheck("Array");
			}
			else {
				this.outputConnection.setCheck("Number");
			}
		}

		/**
		 * Create XML to represent the output type.
		 * @return {Element} XML storage element.
		 * @this Blockly.Block
		 */
		public Element mutationToDom(bool opt_paramIds)
		{
			var container = Document.CreateElement("mutation");
			container.SetAttribute("op", this.getFieldValue("OP"));
			return container;
		}

		/**
		 * Parse XML to restore the output type.
		 * @param {!Element} xmlElement XML storage element.
		 * @this Blockly.Block
		 */
		public void domToMutation(Element xmlElement)
		{
			this.updateType_(xmlElement.GetAttribute("op"));
		}
	}

	[ComVisible(true)]
	public class MathModuloBlock : Block
	{
		public const string type_name = "math_modulo";

		public MathModuloBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for remainder of a division.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.MATH_MODULO_TITLE,
				args0 = new[] {
					new {
						type = "input_value",
						name = "DIVIDEND",
						check = "Number"
					},
					new {
						type = "input_value",
						name = "DIVISOR",
						check = "Number"
					}
				},
				inputsInline = true,
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_MODULO_TOOLTIP,
				helpUrl = Msg.MATH_MODULO_HELPURL
			});
		}
	}

	[ComVisible(true)]
	public class MathConstrainBlock : Block
	{
		public const string type_name = "math_constrain";

		public MathConstrainBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for constraining a number between two limits.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.MATH_CONSTRAIN_TITLE,
				args0 = new[] {
					new {
						type = "input_value",
						name = "VALUE",
						check = "Number"
					},
					new {
						type = "input_value",
						name = "LOW",
						check = "Number"
					},
					new {
						type = "input_value",
						name = "HIGH",
						check = "Number"
					}
				},
				inputsInline = true,
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_CONSTRAIN_TOOLTIP,
				helpUrl = Msg.MATH_CONSTRAIN_HELPURL
			});
		}
	}

	[ComVisible(true)]
	public class MathRandomIntBlock : Block
	{
		public const string type_name = "math_random_int";

		public MathRandomIntBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for random integer between [X] and [Y].
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.MATH_RANDOM_INT_TITLE,
				args0 = new[] {
					new {
						type = "input_value",
						name = "FROM",
						check = "Number"
					},
					new {
						type = "input_value",
						name = "TO",
						check = "Number"
					}
				},
				inputsInline = true,
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_RANDOM_INT_TOOLTIP,
				helpUrl = Msg.MATH_RANDOM_INT_HELPURL
			});
		}
	}

	[ComVisible(true)]
	public class MathRandomFloatBlock : Block
	{
		public const string type_name = "math_random_float";

		public MathRandomFloatBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for random fraction between 0 and 1.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.MATH_RANDOM_FLOAT_TITLE_RANDOM,
				output = "Number",
				colour = Math.HUE,
				tooltip = Msg.MATH_RANDOM_FLOAT_TOOLTIP,
				helpUrl = Msg.MATH_RANDOM_FLOAT_HELPURL
			});
		}
	}
}
