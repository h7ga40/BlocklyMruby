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
 * @fileoverview Procedure blocks for Blockly.
 * @author fraser@google.com (Neil Fraser)
 */
using System;
using System.Linq;
using System.Collections.Generic;
using Bridge;
using Bridge.Html5;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace BlocklyMruby
{
	[ComVisible(true)]
	public abstract class ProceduresDefBlock : Block
	{
		protected string callType_;
		JsArray<string> paramIds_;
		bool hasStatements_;
		internal JsArray<string> arguments_;
		public Connection statementConnection_;

		public ProceduresDefBlock(Blockly blockly, string type)
			: base(blockly, type)
		{
		}

		/**
		 * Add or remove the statement block from this function definition.
		 * @param {boolean} hasStatements True if a statement block is needed.
		 * @this Blockly.Block
		 */
		public void setStatements_(bool hasStatements)
		{
			if (this.hasStatements_ == hasStatements) {
				return;
			}
			if (hasStatements) {
				this.appendStatementInput("STACK")
					.appendField(Msg.PROCEDURES_DEFNORETURN_DO);
				if (this.getInput("RETURN") != null) {
					this.moveInputBefore("STACK", "RETURN");
				}
			}
			else {
				this.removeInput("STACK", true);
			}
			this.hasStatements_ = hasStatements;
		}

		/**
		 * Update the display of parameters for this procedure definition block.
		 * Display a warning if there are duplicately named parameters.
		 * @private
		 * @this Blockly.Block
		 */
		public void updateParams_()
		{
			// Check for duplicated arguments.
			var badArg = false;
			var hash = Bridge.Script.NewObject();
			for (var i = 0; i < this.arguments_.Length; i++) {
				var ret = Script.Get(hash, "arg_" + this.arguments_[i].ToLower());
				if ((ret != null) && !(ret is DBNull)) {
					badArg = true;
					break;
				}
				Script.Set(hash, "arg_" + this.arguments_[i].ToLower(), true);
			}
			if (badArg) {
				this.setWarningText(Msg.PROCEDURES_DEF_DUPLICATE_WARNING);
			}
			else {
				this.setWarningText(null);
			}
			// Merge the arguments into a human-readable list.
			var paramString = "";
			if (this.arguments_.Length != 0) {
				paramString = Msg.PROCEDURES_BEFORE_PARAMS +
					" " + String.Join(", ", this.arguments_);
			}
			// The params field is deterministic based on the mutation,
			// no need to fire a change event.
			Blockly.Events.disable();
			try {
				this.setFieldValue(paramString, "PARAMS");
			}
			finally {
				Blockly.Events.enable();
			}
		}

		/**
		 * Create XML to represent the argument inputs.
		 * @param {=boolean} opt_paramIds If true include the IDs of the parameter
		 *     quarks.  Used by Blockly.Procedures.mutateCallers for reconnection.
		 * @return {!Element} XML storage element.
		 * @this Blockly.Block
		 */
		public Element mutationToDom(bool opt_paramIds)
		{
			var container = Document.CreateElement("mutation");
			if (opt_paramIds) {
				container.SetAttribute("name", this.getFieldValue("NAME"));
			}
			for (var i = 0; i < this.arguments_.Length; i++) {
				var parameter = Document.CreateElement("arg");
				parameter.SetAttribute("name", this.arguments_[i]);
				if (opt_paramIds && this.paramIds_ != null) {
					parameter.SetAttribute("paramId", this.paramIds_[i]);
				}
				container.AppendChild(parameter);
			}

			// Save whether the statement input is visible.
			if (!this.hasStatements_) {
				container.SetAttribute("statements", "false");
			}
			return container;
		}

		/**
		 * Parse XML to restore the argument inputs.
		 * @param {!Element} xmlElement XML storage element.
		 * @this Blockly.Block
		 */
		public void domToMutation(Element xmlElement)
		{
			this.arguments_ = new JsArray<string>();
			Element childNode;
			for (var i = 0; (childNode = (dynamic)xmlElement.ChildNodes[i]) != null; i++) {
				if (childNode.NodeName.ToLower() == "arg") {
					this.arguments_.Push(childNode.GetAttribute("name"));
				}
			}
			this.updateParams_();
			Blockly.Procedures.mutateCallers(this);

			// Show or hide the statement input.
			this.setStatements_(xmlElement.GetAttribute("statements") != "false");
		}

		/**
		 * Populate the mutator's dialog with this block's components.
		 * @param {!Workspace} workspace Mutator's workspace.
		 * @return {!Blockly.Block} Root block in mutator.
		 * @this Blockly.Block
		 */
		public Block decompose(Workspace workspace)
		{
			var containerBlock = workspace.newBlock(ProceduresMutatorcontainerBlock.type_name);
			containerBlock.initSvg();

			// Check/uncheck the allow statement box.
			if (this.getInput("RETURN") != null) {
				containerBlock.setFieldValue(this.hasStatements_ ? "TRUE" : "FALSE", "STATEMENTS");
			}
			else {
				containerBlock.getInput("STATEMENT_INPUT").setVisible(false);
			}

			// Parameter list.
			var connection = containerBlock.getInput("STACK").connection;
			for (var i = 0; i < this.arguments_.Length; i++) {
				var paramBlock = (ProceduresMutatorargBlock)workspace.newBlock(ProceduresMutatorargBlock.type_name);
				paramBlock.initSvg();
				paramBlock.setFieldValue(this.arguments_[i], "NAME");
				// Store the old location.
				paramBlock.oldLocation = i;
				connection.connect(paramBlock.previousConnection);
				connection = paramBlock.nextConnection;
			}
			// Initialize procedure's callers with blank IDs.
			Blockly.Procedures.mutateCallers(this);
			return containerBlock;
		}

		/**
		 * Reconfigure this block based on the mutator dialog's components.
		 * @param {!Blockly.Block} containerBlock Root block in mutator.
		 * @this Blockly.Block
		 */
		public void compose(Block containerBlock)
		{
			// Parameter list.
			this.arguments_ = new JsArray<string>();
			this.paramIds_ = new JsArray<string>();
			var paramBlock = containerBlock.getInputTargetBlock("STACK");
			while (paramBlock != null) {
				this.arguments_.Push(paramBlock.getFieldValue("NAME"));
				this.paramIds_.Push(paramBlock.id);
				paramBlock = (paramBlock.nextConnection != null) ?
					paramBlock.nextConnection.targetBlock() : null;
			}
			this.updateParams_();
			Blockly.Procedures.mutateCallers(this);

			// Show/hide the statement input.
			var hasStatements_ = containerBlock.getFieldValue("STATEMENTS");
			if (hasStatements_ != null) {
				var hasStatements = hasStatements_ == "TRUE";
				if (this.hasStatements_ != hasStatements) {
					if (hasStatements) {
						this.setStatements_(true);
						// Restore the stack, if one was saved.
						Mutator.reconnect(this.statementConnection_, this, "STACK");
						this.statementConnection_ = null;
					}
					else {
						// Save the stack, then disconnect it.
						var stackConnection = this.getInput("STACK").connection;
						this.statementConnection_ = stackConnection.targetConnection;
						if (this.statementConnection_ != null) {
							var stackBlock = stackConnection.targetBlock();
							stackBlock.unplug();
							stackBlock.bumpNeighbours_();
						}
						this.setStatements_(false);
					}
				}
			}
		}

		/**
		 * Return all variables referenced by this block.
		 * @return {!Array.<string>} List of variable names.
		 * @this Blockly.Block
		 */
		public string[] getVars()
		{
			return this.arguments_ == null ? null : this.arguments_.ToArray();
		}

		/**
		 * Notification that a variable is renaming.
		 * If the name matches one of this block's variables, rename it.
		 * @param {string} oldName Previous name of variable.
		 * @param {string} newName Renamed variable.
		 * @this Blockly.Block
		 */
		public void renameVar(string oldName, string newName)
		{
			var change = false;
			for (var i = 0; i < this.arguments_.Length; i++) {
				if (Blockly.Names.equals(oldName, this.arguments_[i])) {
					this.arguments_[i] = newName;
					change = true;
				}
			}
			if (change) {
				this.updateParams_();
				// Update the mutator's variables if the mutator is open.
				if (this.mutator.isVisible()) {
					var blocks = this.mutator.workspace_.getAllBlocks();
					Block block;
					for (var i = 0; (block = blocks[i]) != null; i++) {
						if (block.type == ProceduresMutatorargBlock.type_name &&
							Blockly.Names.equals(oldName, block.getFieldValue("NAME"))) {
							block.setFieldValue(newName, "NAME");
						}
					}
				}
			}
		}

		/**
		 * Add custom menu options to this block's context menu.
		 * @param {!Array} options List of menu options to add to.
		 * @this Blockly.Block
		 */
		public void customContextMenu(ContextMenuOptionList options)
		{
			// Add option to create caller.
			var option = new ContextMenuOption() { enabled = true };
			var name = this.getFieldValue("NAME");
			option.text = Msg.PROCEDURES_CREATE_DO.Replace("%1", name);
			var xmlMutation = goog.dom.createDom(Script, "mutation");
			xmlMutation.SetAttribute("name", name);
			for (var i = 0; i < this.arguments_.Length; i++) {
				var xmlArg = goog.dom.createDom(Script, "arg");
				xmlArg.SetAttribute("name", this.arguments_[i]);
				xmlMutation.AppendChild(xmlArg);
			}
			var xmlBlock = goog.dom.createDom(Script, "block", null, xmlMutation);
			xmlBlock.SetAttribute("type", this.callType_);
			option.callback = ContextMenu.callbackFactory(this, xmlBlock);
			options.Push(option);

			// Add options to create getters for each parameter.
			if (!this.isCollapsed()) {
				for (var i = 0; i < this.arguments_.Length; i++) {
					option = new ContextMenuOption() { enabled = true };
					name = this.arguments_[i];
					option.text = Msg.VARIABLES_SET_CREATE_GET.Replace("%1", name);
					var xmlField = goog.dom.createDom(Script, "field", null, name);
					xmlField.SetAttribute("name", "VAR");
					xmlBlock = goog.dom.createDom(Script, "block", null, xmlField);
					xmlBlock.SetAttribute("type", VariablesGetBlock.type_name);
					option.callback = ContextMenu.callbackFactory(this, xmlBlock);
					options.Push(option);
				}
			}
		}

		public abstract Tuple<string, string[], bool> getProcedureDef();
	}

	[ComVisible(true)]
	public class ProceduresDefnoreturnBlock : ProceduresDefBlock
	{
		public const string type_name = "procedures_defnoreturn";

		public ProceduresDefnoreturnBlock(Blockly blockly)
			: base(blockly, type_name)
		{
			callType_ = ProceduresCallnoreturnBlock.type_name;
		}

		/**
		 * Block for defining a procedure with no return value.
		 * @this Blockly.Block
		 */
		public void init()
		{
			var nameField = new FieldTextInput(Blockly,
				Msg.PROCEDURES_DEFNORETURN_PROCEDURE);
			nameField.setValidator((str) => { return Blockly.Procedures.rename(nameField, str); });
			nameField.setSpellcheck(false);
			this.appendDummyInput()
				.appendField(Msg.PROCEDURES_DEFNORETURN_TITLE)
				.appendField(nameField, "NAME")
				.appendField("", "PARAMS");
			this.setMutator(new Mutator(Blockly, new[] { ProceduresMutatorargBlock.type_name }));
			if ((this.workspace.options.comments ||
				 (this.workspace.options.parentWorkspace != null &&
				  this.workspace.options.parentWorkspace.options.comments)) &&
				!String.IsNullOrEmpty(Msg.PROCEDURES_DEFNORETURN_COMMENT)) {
				this.setCommentText(Msg.PROCEDURES_DEFNORETURN_COMMENT);
			}
			this.setColour(Blockly.Procedures.HUE);
			this.setTooltip(Msg.PROCEDURES_DEFNORETURN_TOOLTIP);
			this.setHelpUrl(Msg.PROCEDURES_DEFNORETURN_HELPURL);
			this.arguments_ = new JsArray<string>();
			this.setStatements_(true);
			this.statementConnection_ = null;
		}

		/**
		 * Return the signature of this procedure definition.
		 * @return {!Array} Tuple containing three elements:
		 *     - the name of the defined procedure,
		 *     - a list of all its arguments,
		 *     - that it DOES NOT have a return value.
		 * @this Blockly.Block
		 */
		public override Tuple<string, string[], bool> getProcedureDef()
		{
			return new Tuple<string, string[], bool>(this.getFieldValue("NAME"), this.arguments_, false);
		}
	}

	[ComVisible(true)]
	public class ProceduresDefreturnBlock : ProceduresDefBlock
	{
		public const string type_name = "procedures_defreturn";

		public ProceduresDefreturnBlock(Blockly blockly)
			: base(blockly, type_name)
		{
			callType_ = ProceduresCallreturnBlock.type_name;
		}

		/**
		 * Block for defining a procedure with a return value.
		 * @this Blockly.Block
		 */
		public void init()
		{
			var nameField = new FieldTextInput(Blockly,
				Msg.PROCEDURES_DEFRETURN_PROCEDURE);
			nameField.setValidator((str) => { return Blockly.Procedures.rename(nameField, str); });
			nameField.setSpellcheck(false);
			this.appendDummyInput()
				.appendField(Msg.PROCEDURES_DEFRETURN_TITLE)
				.appendField(nameField, "NAME")
				.appendField("", "PARAMS");
			this.appendValueInput("RETURN")
				.setAlign(Blockly.ALIGN_RIGHT)
				.appendField(Msg.PROCEDURES_DEFRETURN_RETURN);
			this.setMutator(new Mutator(Blockly, new[] { ProceduresMutatorargBlock.type_name }));
			if ((this.workspace.options.comments ||
				 (this.workspace.options.parentWorkspace != null &&
				  this.workspace.options.parentWorkspace.options.comments)) &&
				!String.IsNullOrEmpty(Msg.PROCEDURES_DEFRETURN_COMMENT)) {
				this.setCommentText(Msg.PROCEDURES_DEFRETURN_COMMENT);
			}
			this.setColour(Blockly.Procedures.HUE);
			this.setTooltip(Msg.PROCEDURES_DEFRETURN_TOOLTIP);
			this.setHelpUrl(Msg.PROCEDURES_DEFRETURN_HELPURL);
			this.arguments_ = new JsArray<string>();
			this.setStatements_(true);
			this.statementConnection_ = null;
		}

		/**
		 * Return the signature of this procedure definition.
		 * @return {!Array} Tuple containing three elements:
		 *     - the name of the defined procedure,
		 *     - a list of all its arguments,
		 *     - that it DOES have a return value.
		 * @this Blockly.Block
		 */
		public override Tuple<string, string[], bool> getProcedureDef()
		{
			return new Tuple<string, string[], bool>(this.getFieldValue("NAME"), this.arguments_, true);
		}
	}

	[ComVisible(true)]
	public class ProceduresMutatorcontainerBlock : Block
	{
		public const string type_name = "procedures_mutatorcontainer";

		public ProceduresMutatorcontainerBlock(Blockly blockly)
			: base(blockly, type_name)
		{
		}

		/**
		 * Mutator block for procedure container.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.appendDummyInput()
				.appendField(Msg.PROCEDURES_MUTATORCONTAINER_TITLE);
			this.appendStatementInput("STACK");
			this.appendDummyInput("STATEMENT_INPUT")
				.appendField(Msg.PROCEDURES_ALLOW_STATEMENTS)
				.appendField(new FieldCheckbox(Blockly, "TRUE"), "STATEMENTS");
			this.setColour(Blockly.Procedures.HUE);
			this.setTooltip(Msg.PROCEDURES_MUTATORCONTAINER_TOOLTIP);
			this.contextMenu = false;
		}
	}

	[ComVisible(true)]
	public class ProceduresMutatorargBlock : Block
	{
		public const string type_name = "procedures_mutatorarg";
		public int oldLocation;

		public ProceduresMutatorargBlock(Blockly blockly)
			: base(blockly, type_name)
		{
		}

		/**
		 * Mutator block for procedure argument.
		 * @this Blockly.Block
		 */
		public void init()
		{
			var field = new FieldTextInput(Blockly, "x", this.validator_);
			this.appendDummyInput()
				.appendField(Msg.PROCEDURES_MUTATORARG_TITLE)
				.appendField(field, "NAME");
			this.setPreviousStatement(true);
			this.setNextStatement(true);
			this.setColour(Blockly.Procedures.HUE);
			this.setTooltip(Msg.PROCEDURES_MUTATORARG_TOOLTIP);
			this.contextMenu = false;

			// Create the default variable when we drag the block in from the flyout.
			// Have to do this after installing the field on the block.
			field.onFinishEditing_ = this.createNewVar_;
			field.onFinishEditing_("x");
		}

		/**
		 * Obtain a valid name for the procedure.
		 * Merge runs of whitespace.  Strip leading and trailing whitespace.
		 * Beyond this, all names are legal.
		 * @param {string} newVar User-supplied name.
		 * @return {?string} Valid name, or null if a name was not specified.
		 * @private
		 * @this Blockly.Block
		 */
		public string validator_(string newVar)
		{
			newVar = Regex.Replace(Regex.Replace(newVar, "[\\s\\xa0]+", " "), "^ | $", "");
			return newVar;
		}

		/**
		 * Called when focusing away from the text field.
		 * Creates a new variable with this name.
		 * @param {string} newText The new variable name.
		 * @private
		 * @this FieldTextInput
		 */
		public void createNewVar_(string newText)
		{
			var source = this/*((FieldTextInput)this).sourceBlock_*/;
			if (source != null && source.workspace != null && source.workspace.options != null
				&& source.workspace.options.parentWorkspace != null) {
				source.workspace.options.parentWorkspace.createVariable(newText);
			}
		}
	}

	[ComVisible(true)]
	public class ProceduresCallBlock : Block
	{
		internal JsArray<string> arguments_;
		protected Dictionary<string, Connection> quarkConnections_;
		protected JsArray<string> quarkIds_;
		protected bool rendered;
		protected string defType_;

		public ProceduresCallBlock(Blockly blockly, string type)
			: base(blockly, type)
		{
		}

		/**
		 * Returns the name of the procedure this block calls.
		 * @return {string} Procedure name.
		 * @this Blockly.Block
		 */
		public string getProcedureCall()
		{
			// The NAME field is guaranteed to exist, null will never be returned.
			return /** @type {string} */ (this.getFieldValue("NAME"));
		}

		/**
		 * Notification that a procedure is renaming.
		 * If the name matches this block's procedure, rename it.
		 * @param {string} oldName Previous name of procedure.
		 * @param {string} newName Renamed procedure.
		 * @this Blockly.Block
		 */
		public void renameProcedure(string oldName, string newName)
		{
			if (Blockly.Names.equals(oldName, this.getProcedureCall())) {
				this.setFieldValue(newName, "NAME");
				this.setTooltip(
					(this.outputConnection != null ? Msg.PROCEDURES_CALLRETURN_TOOLTIP :
					 Msg.PROCEDURES_CALLNORETURN_TOOLTIP)
					.Replace("%1", newName));
			}
		}

		/**
		 * Notification that the procedure's parameters have changed.
		 * @param {!Array.<string>} paramNames New param names, e.g. ["x", "y", "z"].
		 * @param {!Array.<string>} paramIds IDs of params (consistent for each
		 *     parameter through the life of a mutator, regardless of param renaming),
		 *     e.g. ["piua", "f8b_", "oi.o"].
		 * @private
		 * @this Blockly.Block
		 */
		public void setProcedureParameters_(JsArray<string> paramNames, JsArray<string> paramIds)
		{
			// Data structures:
			// this.arguments = ["x", "y"]
			//     Existing param names.
			// this.quarkConnections_ {piua: null, f8b_: Connection}
			//     Look-up of paramIds to connections plugged into the call block.
			// this.quarkIds_ = ["piua", "f8b_"]
			//     Existing param IDs.
			// Note that quarkConnections_ may include IDs that no longer exist, but
			// which might reappear if a param is reattached in the mutator.
			var defBlock = Blockly.Procedures.getDefinition(this.getProcedureCall(),
				this.workspace);
			var mutatorOpen = defBlock != null && defBlock.mutator != null &&
				defBlock.mutator.isVisible();
			if (!mutatorOpen) {
				this.quarkConnections_ = new Dictionary<string, Connection>();
				this.quarkIds_ = null;
			}
			if (paramIds == null) {
				// Reset the quarks (a mutator is about to open).
				return;
			}
			if (goog.array.equals(Script, this.arguments_.ToArray(), paramNames.ToArray())) {
				// No change.
				this.quarkIds_ = paramIds;
				return;
			}
			if (paramIds.Length != paramNames.Length) {
				throw new Exception("Error: paramNames and paramIds must be the same length.");
			}
			this.setCollapsed(false);
			if (this.quarkIds_ == null) {
				// Initialize tracking for this block.
				this.quarkConnections_ = new Dictionary<string, Connection>();
				if (String.Join("\n", paramNames) == String.Join("\n", this.arguments_)) {
					// No change to the parameters, allow quarkConnections_ to be
					// populated with the existing connections.
					this.quarkIds_ = paramIds;
				}
				else {
					this.quarkIds_ = new JsArray<string>();
					for (int i = 0; i < this.arguments_.Length; i++) {
						this.quarkIds_.Push(i.ToString());
					}
				}
			}
			// Switch off rendering while the block is rebuilt.
			var savedRendered = this.rendered;
			this.rendered = false;
			// Update the quarkConnections_ with existing connections.
			for (var i = 0; i < this.arguments_.Length; i++) {
				var input = this.getInput("ARG" + i);
				if (input != null) {
					var connection = input.connection.targetConnection;
					this.quarkConnections_[this.quarkIds_[i]] = connection;
					if (mutatorOpen && connection != null &&
						paramIds.IndexOf(this.quarkIds_[i]) == -1) {
						// This connection should no longer be attached to this block.
						connection.disconnect();
						connection.getSourceBlock().bumpNeighbours_();
					}
				}
			}
			// Rebuild the block's arguments.
			this.arguments_ = new JsArray<string>();
			this.arguments_ = (JsArray<string>)this.arguments_.Concat(paramNames);
			this.updateShape_();
			this.quarkIds_ = paramIds;
			// Reconnect any child blocks.
			if (this.quarkIds_ != null) {
				for (var i = 0; i < this.arguments_.Length; i++) {
					var quarkId = this.quarkIds_[i];
					if (quarkId == null)
						continue;
					Connection connection;
					if (this.quarkConnections_.TryGetValue(quarkId, out connection)) {
						if (!Mutator.reconnect(connection, this, "ARG" + i)) {
							// Block no longer exists or has been attached elsewhere.
							this.quarkConnections_.Remove(quarkId);
						}
					}
				}
			}
			// Restore rendering and show the changes.
			this.rendered = savedRendered;
			if (this.rendered) {
				this.render();
			}
		}

		/**
		 * Modify this block to have the correct number of arguments.
		 * @private
		 * @this Blockly.Block
		 */
		private void updateShape_()
		{
			int i;
			for (i = 0; i < this.arguments_.Length; i++) {
				var field = this.getField("ARGNAME" + i);
				if (field != null) {
					// Ensure argument name is up to date.
					// The argument name field is deterministic based on the mutation,
					// no need to fire a change event.
					Blockly.Events.disable();
					try {
						field.setValue(this.arguments_[i]);
					}
					finally {
						Blockly.Events.enable();
					}
				}
				else {
					// Add new input.
					field = new FieldLabel(Blockly, this.arguments_[i]);
					var input = this.appendValueInput("ARG" + i)
						.setAlign(Blockly.ALIGN_RIGHT)
						.appendField(field, "ARGNAME" + i);
					input.init();
				}
			}
			// Remove deleted inputs.
			while (this.getInput("ARG" + i) != null) {
				this.removeInput("ARG" + i);
				i++;
			}
			// Add "with:" if there are parameters, remove otherwise.
			var topRow = this.getInput("TOPROW");
			if (topRow != null) {
				if (this.arguments_.Length != 0) {
					if (this.getField("WITH") == null) {
						topRow.appendField(Msg.PROCEDURES_CALL_BEFORE_PARAMS, "WITH");
						topRow.init();
					}
				}
				else {
					if (this.getField("WITH") != null) {
						topRow.removeField("WITH");
					}
				}
			}
		}

		/**
		 * Create XML to represent the (non-editable) name and arguments.
		 * @return {!Element} XML storage element.
		 * @this Blockly.Block
		 */
		public Element mutationToDom(bool opt_paramIds = false)
		{
			var container = Document.CreateElement("mutation");
			container.SetAttribute("name", this.getProcedureCall());
			for (var i = 0; i < this.arguments_.Length; i++) {
				var parameter = Document.CreateElement("arg");
				parameter.SetAttribute("name", this.arguments_[i]);
				container.AppendChild(parameter);
			}
			return container;
		}

		/**
		 * Parse XML to restore the (non-editable) name and parameters.
		 * @param {!Element} xmlElement XML storage element.
		 * @this Blockly.Block
		 */
		public void domToMutation(Element xmlElement)
		{
			var name = xmlElement.GetAttribute("name");
			this.renameProcedure(this.getProcedureCall(), name);
			var args = new JsArray<string>();
			var paramIds = new JsArray<string>();
			Element childNode;
			for (var i = 0; (childNode = (dynamic)xmlElement.ChildNodes[i]) != null; i++) {
				if (childNode.NodeName.ToLower() == "arg") {
					args.Push(childNode.GetAttribute("name"));
					var paramId = childNode.GetAttribute("paramId");
					if (paramId == null) paramId = i.ToString();
					paramIds.Push(paramId);
				}
			}
			this.setProcedureParameters_(args, paramIds);
		}

		/**
		 * Notification that a variable is renaming.
		 * If the name matches one of this block's variables, rename it.
		 * @param {string} oldName Previous name of variable.
		 * @param {string} newName Renamed variable.
		 * @this Blockly.Block
		 */
		public void renameVar(string oldName, string newName)
		{
			for (var i = 0; i < this.arguments_.Length; i++) {
				if (Blockly.Names.equals(oldName, this.arguments_[i])) {
					this.arguments_[i] = newName;
					this.getField("ARGNAME" + i).setValue(newName);
				}
			}
		}

		/**
		 * Procedure calls cannot exist without the corresponding procedure
		 * definition.  Enforce this link whenever an event is fired.
		 * @this Blockly.Block
		 */
		public void onchange(Abstract e)
		{
			if (this.workspace == null || this.workspace.isFlyout) {
				// Block is deleted or is in a flyout.
				return;
			}
			if (e.type == Events.CREATE &&
				Array.IndexOf(((Create)e).ids, this.id) != -1) {
				// Look for the case where a procedure call was created (usually through
				// paste) and there is no matching definition.  In this case, create
				// an empty definition block with the correct signature.
				var name = this.getProcedureCall();
				var def = Blockly.Procedures.getDefinition(name, this.workspace);
				if (def != null && (def.type != this.defType_ ||
					JSON.Stringify(((ProceduresDefBlock)def).arguments_.ToArray()) != JSON.Stringify(this.arguments_.ToArray()))) {
					// The signatures don"t match.
					def = null;
				}
				if (def == null) {
					Blockly.Events.setGroup(e.group);
					/**
					 * Create matching definition block.
					 * <xml>
					 *   <block type="procedures_defreturn" x="10" y="20">
					 *     <mutation name="test">
					 *       <arg name="x"></arg>
					 *     </mutation>
					 *     <field name="NAME">test</field>
					 *   </block>
					 * </xml>
					 */
					var xml = goog.dom.createDom(Script, "xml");
					var block = goog.dom.createDom(Script, "block");
					block.SetAttribute("type", this.defType_);
					var xy = this.getRelativeToSurfaceXY();
					var x = xy.x + Blockly.SNAP_RADIUS * (this.RTL ? -1 : 1);
					var y = xy.y + Blockly.SNAP_RADIUS * 2;
					block.SetAttribute("x", x.ToString());
					block.SetAttribute("y", y.ToString());
					var mutation = this.mutationToDom(false);
					block.AppendChild(mutation);
					var field = goog.dom.createDom(Script, "field");
					field.SetAttribute("name", "NAME");
					field.AppendChild(Document.CreateTextNode(this.getProcedureCall()));
					block.AppendChild(field);
					xml.AppendChild(block);
					Blockly.Xml.domToWorkspace(xml, this.workspace);
					Blockly.Events.setGroup(false.ToString());
				}
			}
			else if (e.type == Events.DELETE) {
				// Look for the case where a procedure definition has been deleted,
				// leaving this block (a procedure call) orphaned.  In this case, delete
				// the orphan.
				var name = this.getProcedureCall();
				var def = Blockly.Procedures.getDefinition(name, this.workspace);
				if (def == null) {
					Blockly.Events.setGroup(e.group);
					this.dispose(true/*, false*/);
					Blockly.Events.setGroup(false.ToString());
				}
			}
		}
	}

	[ComVisible(true)]
	public class ProceduresCallnoreturnBlock : ProceduresCallBlock
	{
		public const string type_name = "procedures_callnoreturn";

		public ProceduresCallnoreturnBlock(Blockly blockly)
			: base(blockly, type_name)
		{
			defType_ = ProceduresDefnoreturnBlock.type_name;
		}

		/**
		 * Block for calling a procedure with no return value.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.appendDummyInput("TOPROW")
				.appendField(this.id, "NAME");
			this.setPreviousStatement(true);
			this.setNextStatement(true);
			this.setColour(Blockly.Procedures.HUE);
			// Tooltip is set in renameProcedure.
			this.setHelpUrl(Msg.PROCEDURES_CALLNORETURN_HELPURL);
			this.arguments_ = new JsArray<string>();
			this.quarkConnections_ = new Dictionary<string, Connection>();
			this.quarkIds_ = null;
		}

		/**
		 * Add menu option to find the definition block for this call.
		 * @param {!Array} options List of menu options to add to.
		 * @this Blockly.Block
		 */
		public void customContextMenu(ContextMenuOptionList options)
		{
			var option = new ContextMenuOption() { enabled = true };
			option.text = Msg.PROCEDURES_HIGHLIGHT_DEF;
			var name = this.getProcedureCall();
			var workspace = this.workspace;
			option.callback = Script.NewFunc(new Action(() => {
				var def = Blockly.Procedures.getDefinition(name, workspace);
				if (def != null)
					def.select();
			}));
			options.Push(option);
		}
	}

	[ComVisible(true)]
	public class ProceduresCallreturnBlock : ProceduresCallBlock
	{
		public const string type_name = "procedures_callreturn";

		public ProceduresCallreturnBlock(Blockly blockly)
			: base(blockly, type_name)
		{
			defType_ = ProceduresDefreturnBlock.type_name;
		}

		/**
		 * Block for calling a procedure with a return value.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.appendDummyInput("TOPROW")
				.appendField("", "NAME");
			this.setOutput(true);
			this.setColour(Blockly.Procedures.HUE);
			// Tooltip is set in domToMutation.
			this.setHelpUrl(Msg.PROCEDURES_CALLRETURN_HELPURL);
			this.arguments_ = new JsArray<string>();
			this.quarkConnections_ = new Dictionary<string, Connection>();
			this.quarkIds_ = null;
		}
	}

	[ComVisible(true)]
	public class ProceduresIfreturnBlock : Block
	{
		public const string type_name = "procedures_ifreturn";
		internal bool hasReturnValue_;

		public ProceduresIfreturnBlock(Blockly blockly)
			: base(blockly, type_name)
		{
		}

		/**
		 * Block for conditionally returning a value from a procedure.
		 * @this Blockly.Block
		 */
		public void init()
		{
			this.appendValueInput("CONDITION")
				.setCheck("Boolean")
				.appendField(Msg.CONTROLS_IF_MSG_IF);
			this.appendValueInput("VALUE")
				.appendField(Msg.PROCEDURES_DEFRETURN_RETURN);
			this.setInputsInline(true);
			this.setPreviousStatement(true);
			this.setNextStatement(true);
			this.setColour(Blockly.Procedures.HUE);
			this.setTooltip(Msg.PROCEDURES_IFRETURN_TOOLTIP);
			this.setHelpUrl(Msg.PROCEDURES_IFRETURN_HELPURL);
			this.hasReturnValue_ = true;
		}

		/**
		 * Create XML to represent whether this block has a return value.
		 * @return {!Element} XML storage element.
		 * @this Blockly.Block
		 */
		public Element mutationToDom(bool opt_paramIds)
		{
			var container = Document.CreateElement("mutation");
			container.SetAttribute("value", this.hasReturnValue_ ? "1" : "0");
			return container;
		}

		/**
		 * Parse XML to restore whether this block has a return value.
		 * @param {!Element} xmlElement XML storage element.
		 * @this Blockly.Block
		 */
		public void domToMutation(Element xmlElement)
		{
			var value = xmlElement.GetAttribute("value");
			this.hasReturnValue_ = (value == "1");
			if (!this.hasReturnValue_) {
				this.removeInput("VALUE");
				this.appendDummyInput("VALUE")
				  .appendField(Msg.PROCEDURES_DEFRETURN_RETURN);
			}
		}

		/**
		 * Called whenever anything on the workspace changes.
		 * Add warning if this flow block is not nested inside a loop.
		 * @param {!Abstract} e Change event.
		 * @this Blockly.Block
		 */
		public void onchange(Abstract e)
		{
			if (((WorkspaceSvg)this.workspace).isDragging() != 0) {
				return;  // Don't change state at the start of a drag.
			}
			var legal = false;
			// Is the block nested in a procedure?
			var block = (Block)this;
			do {
				if (Array.IndexOf(this.FUNCTION_TYPES, block.type) != -1) {
					legal = true;
					break;
				}
				block = block.getSurroundParent();
			} while (block != null);
			if (legal) {
				// If needed, toggle whether this block has a return value.
				if (block.type == ProceduresDefnoreturnBlock.type_name && this.hasReturnValue_) {
					this.removeInput("VALUE");
					this.appendDummyInput("VALUE")
					  .appendField(Msg.PROCEDURES_DEFRETURN_RETURN);
					this.hasReturnValue_ = false;
				}
				else if (block.type == ProceduresDefreturnBlock.type_name && !this.hasReturnValue_) {
					this.removeInput("VALUE");
					this.appendValueInput("VALUE")
					  .appendField(Msg.PROCEDURES_DEFRETURN_RETURN);
					this.hasReturnValue_ = true;
				}
				this.setWarningText(null);
				if (!this.isInFlyout) {
					this.setDisabled(false);
				}
			}
			else {
				this.setWarningText(Msg.PROCEDURES_IFRETURN_WARNING);
				if (!this.isInFlyout && !this.getInheritedDisabled()) {
					this.setDisabled(true);
				}
			}
		}

		/**
		 * List of block types that are functions and thus do not need warnings.
		 * To add a new function type add this to your code:
		 * Blockly.Blocks["procedures_ifreturn"].FUNCTION_TYPES.push("custom_func");
		 */
		string[] FUNCTION_TYPES = new[] { ProceduresDefnoreturnBlock.type_name, ProceduresDefreturnBlock.type_name };
	}
}
