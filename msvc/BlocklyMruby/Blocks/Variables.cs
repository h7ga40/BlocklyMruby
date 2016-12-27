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
 * @fileoverview Variable blocks for Blockly.
 * @author fraser@google.com (Neil Fraser)
 */
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	public class Variables
	{
		/**
		 * Common HSV hue for all blocks in this category.
		 */
		public static int HUE = 330;
	}

	[ComVisible(true)]
	public class VariablesGetBlock : Block
	{
		public const string type_name = "variables_get";
		string contextMenuType_ = VariablesSetBlock.type_name;

		public VariablesGetBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for variable getter.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.setHelpUrl(Msg.VARIABLES_GET_HELPURL);
			this.setColour(Variables.HUE);
			this.appendDummyInput()
				.appendField(new Blockly.FieldVariable(
				Msg.VARIABLES_DEFAULT_NAME), "VAR");
			this.setOutput(true);
			this.setTooltip(Msg.VARIABLES_GET_TOOLTIP);
			this.contextMenuMsg_ = Msg.VARIABLES_GET_CREATE_SET;
		}

		/**
		 * Add menu option to create getter/setter block for this setter/getter.
		 * @param {!Array} options List of menu options to add to.
		 * @this Blockly.Block
		 */
		public void customContextMenu(ContextMenuOptionList options)
		{
			var option = new ContextMenuOption() { enabled = true };
			var name = this.getFieldValue("VAR");
			option.text = this.contextMenuMsg_.Replace("%1", name);
			var xmlField = goog.dom.createDom("field", null, name);
			xmlField.SetAttribute("name", "VAR");
			var xmlBlock = goog.dom.createDom("block", null, xmlField);
			xmlBlock.SetAttribute("type", this.contextMenuType_);
			option.callback = Blockly.ContextMenu.callbackFactory(this, xmlBlock);
			options.push(option);
		}
	}

	[ComVisible(true)]
	public class VariablesSetBlock : Block
	{
		public const string type_name = "variables_set";
		string contextMenuType_ = VariablesGetBlock.type_name;

		public VariablesSetBlock()
			: base(type_name)
		{
		}

		/**
		 * Block for variable setter.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.jsonInit(new {
				message0 = Msg.VARIABLES_SET,
				args0 = new object[] {
					new {
						type = "field_variable",
						name = "VAR",
						variable = Msg.VARIABLES_DEFAULT_NAME
					},
					new {
						type = "input_value",
						name = "VALUE"
					}
				},
				previousStatement = (Any<string, string[]>)null,
				nextStatement = (Any<string, string[]>)null,
				colour = Variables.HUE,
				tooltip = Msg.VARIABLES_SET_TOOLTIP,
				helpUrl = Msg.VARIABLES_SET_HELPURL

			});
			this.contextMenuMsg_ = Msg.VARIABLES_SET_CREATE_GET;
		}

		/**
		 * Add menu option to create getter/setter block for this setter/getter.
		 * @param {!Array} options List of menu options to add to.
		 * @this Blockly.Block
		 */
		public void customContextMenu(ContextMenuOptionList options)
		{
			var option = new ContextMenuOption() { enabled = true };
			var name = this.getFieldValue("VAR");
			option.text = this.contextMenuMsg_.Replace("%1", name);
			var xmlField = goog.dom.createDom("field", null, name);
			xmlField.SetAttribute("name", "VAR");
			var xmlBlock = goog.dom.createDom("block", null, xmlField);
			xmlBlock.SetAttribute("type", this.contextMenuType_);
			option.callback = Blockly.ContextMenu.callbackFactory(this, xmlBlock);
			options.push(option);
		}
	}
}
