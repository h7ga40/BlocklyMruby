﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	[ComVisible(true)]
	public class Block
	{
		public dynamic instance;
		public string type;

		[External, FieldProperty]
		internal string id { get { return instance.id; } }
		[External, FieldProperty]
		internal Blockly.Connection outputConnection {
			get { return Blockly.Connection.Create(instance.outputConnection); }
		}
		[External, FieldProperty]
		internal Blockly.Connection nextConnection {
			get { return Blockly.Connection.Create(instance.nextConnection); }
		}
		[External, FieldProperty]
		internal Blockly.Connection previousConnection {
			get { return Blockly.Connection.Create(instance.previousConnection); }
		}
		[External, FieldProperty]
		internal InputList inputList {
			get {
				return new InputList(instance.inputList);
			}
		}
		[External, FieldProperty]
		internal bool inputsInline {
			get {
				var ret = instance.inputsInline;
				if ((ret == null) || (ret is DBNull) || !(ret is bool))
					return false;
				return (bool)ret;
			}
		}
		[External, FieldProperty]
		internal bool disabled { get { return instance.disabled; } }
		[External, FieldProperty]
		internal string tooltip { get { return instance.tooltip; } }
		[External, FieldProperty]
		internal bool contextMenu { get { return instance.contextMenu; } set { instance.contextMenu = value; } }
		[External, FieldProperty]
		internal string contextMenuMsg_ {
			get { return Script.HasMember(instance, "contextMenuMsg_") ? instance.contextMenuMsg_ : null; }
			set { Script.Set(instance, "contextMenuMsg_", value); }
		}
		[External, FieldProperty]
		internal Any<string, Blockly.Comment> comment {
			get {
				var str = instance.comment as string;
				if (str != null) return str;
				return new Blockly.Comment(instance.comment);
			}
		}
		[External, FieldProperty]
		internal bool RTL { get { return instance.RTL; } }
		[External, FieldProperty]
		internal BlockList prevBlocks_ {
			get {
				var blocks = Script.Get<dynamic>(instance, "prevBlocks_");
				if ((blocks == null) || (blocks is DBNull))
					return null;
				return new BlockList(blocks);
			}
			set {
				Script.Set(instance, "prevBlocks_", value == null ? null : value.instance);
			}
		}
		[External, FieldProperty]
		internal Blockly.Workspace workspace { get { return Blockly.WorkspaceSvg.Create(instance.workspace); } }
		[External, FieldProperty]
		internal Blockly.Mutator mutator { get { return Blockly.Mutator.Create(instance.mutator); } }
		[External, FieldProperty]
		internal bool isInFlyout { get { return instance.isInFlyout; } }

		internal Block(string type)
		{
			this.type = type;
		}

		[External]
		internal void bumpNeighbours_()
		{
			instance.bumpNeighbours_.call(instance);
		}

		/// <summary>
		/// Dispose of this block.
		/// </summary>
		/// <param name="healStack">healStack If true, then try to heal any gap by connecting
		/// the next statement with the previous statement.  Otherwise, dispose of
		/// all children of this block.</param>
		[External]
		internal virtual void dispose(bool healStack)
		{
			instance.dispose.call(instance, healStack);
		}

		/// <summary>
		/// Unplug this block from its superior block.  If this block is a statement,
		/// optionally reconnect the block underneath with the block on top.
		/// </summary>
		/// <param name="opt_healStack">opt_healStack Disconnect child statement and reconnect
		/// stack.  Defaults to false.</param>
		[External]
		internal void unplug(bool opt_healStack = false)
		{
			instance.unplug.call(instance, opt_healStack);
		}

		/// <summary>
		/// Return the parent block or null if this block is at the top level.
		/// </summary>
		/// <returns>The block that holds the current block.</returns>
		[External]
		internal Block getParent()
		{
			var ret = instance.getParent.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateBlock(ret);
		}

		/// <summary>
		/// Return the input that connects to the specified block.
		/// </summary>
		/// <param name="block">A block connected to an input on this block.</param>
		/// <returns>The input that connects to the specified block.</returns>
		[External]
		internal Blockly.Input getInputWithBlock(Block block)
		{
			var ret = instance.getInputWithBlock.call(instance, block.instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateInput(ret);
		}

		/// <summary>
		/// Return the parent block that surrounds the current block, or null if this
		/// block has no surrounding block.  A parent block might just be the previous
		/// statement, whereas the surrounding block is an if statement, while loop, etc.
		/// </summary>
		/// <returns>The block that surrounds the current block.</returns>
		[External]
		internal Block getSurroundParent()
		{
			var ret = instance.getSurroundParent.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateBlock(ret);
		}

		/// <summary>
		/// Return the next statement block directly connected to this block.
		/// </summary>
		/// <returns>The next statement block or null.</returns>
		[External]
		internal Block getNextBlock()
		{
			var ret = instance.getNextBlock.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateBlock(ret);
		}

		/// <summary>
		/// Return the top-most block in this block's tree.
		/// This will return itself if this block is at the top level.
		/// </summary>
		/// <returns>The root block.</returns>
		[External]
		internal Block getRootBlock()
		{
			var ret = instance.getRootBlock.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateBlock(ret);
		}

		/// <summary>
		/// Find all the blocks that are directly nested inside this one.
		/// Includes value and block inputs, as well as any following statement.
		/// Excludes any connection on an output tab or any preceding statement.
		/// </summary>
		/// <returns>Array of blocks.</returns>
		[External]
		internal BlockList getChildren()
		{
			var childlen = instance.getChildren.call(instance);
			if ((childlen == null) || (childlen is DBNull))
				return null;
			return new BlockList(childlen);
		}

		/// <summary>
		/// Set parent of this block to be a new block or null.
		/// </summary>
		/// <param name="block">newParent New parent block.</param>
		[External]
		internal void setParent(Block block)
		{
			instance.setParent.call(instance, block.instance);
		}

		/// <summary>
		/// Find all the blocks that are directly or indirectly nested inside this one.
		/// Includes this block in the list.
		/// Includes value and block inputs, as well as any following statements.
		/// Excludes any connection on an output tab or any preceding statements.
		/// </summary>
		/// <returns>Flattened array of blocks.</returns>
		[External]
		internal BlockList getDescendants()
		{
			var descendants = instance.getDescendants.call(instance);
			if ((descendants == null) || (descendants is DBNull))
				return null;
			return new BlockList(descendants);
		}

		/// <summary>
		/// Get whether this block is deletable or not.
		/// </summary>
		/// <returns>True if deletable.</returns>
		[External]
		internal bool isDeletable()
		{
			return instance.isDeletable.call(instance);
		}

		/// <summary>
		/// Set whether this block is deletable or not.
		/// </summary>
		/// <param name="deletable">True if deletable.</param>
		[External]
		internal void setDeletable(bool deletable)
		{
			instance.setDeletable.call(instance, deletable);
		}

		/// <summary>
		/// Get whether this block is movable or not.
		/// </summary>
		/// <returns>True if movable.</returns>
		[External]
		internal bool isMovable()
		{
			return instance.isMovable.call(instance);
		}

		/// <summary>
		/// Set whether this block is movable or not.
		/// </summary>
		/// <param name="movable">True if movable.</param>
		[External]
		internal void setMovable(bool movable)
		{
			instance.setMovable.call(instance, movable);
		}

		/// <summary>
		/// Get whether this block is a shadow block or not.
		/// </summary>
		/// <returns>True if a shadow.</returns>
		[External]
		internal bool isShadow()
		{
			return instance.isShadow.call(instance);
		}

		/// <summary>
		/// Set whether this block is a shadow block or not.
		/// </summary>
		/// <param name="shadow">True if a shadow.</param>
		[External]
		internal void setShadow(bool shadow)
		{
			instance.setShadow.call(instance, shadow);
		}

		/// <summary>
		/// Get whether this block is editable or not.
		/// </summary>
		/// <returns>True if editable.</returns>
		[External]
		internal bool isEditable()
		{
			return instance.isEditable.call(instance);
		}

		/// <summary>
		/// Set whether this block is editable or not.
		/// </summary>
		/// <param name="editable">True if editable.</param>
		[External]
		internal void setEditable(bool editable)
		{
			instance.setEditable.call(instance, editable);
		}

		/// <summary>
		/// Set whether the connections are hidden (not tracked in a database) or not.
		/// Recursively walk down all child blocks (except collapsed blocks).
		/// </summary>
		/// <param name="hidden">True if connections are hidden.</param>
		[External]
		internal void setConnectionsHidden(bool hidden)
		{
			instance.setConnectionsHidden.call(instance, hidden);
		}

		/// <summary>
		/// Set the URL of this block's help page.
		/// </summary>
		/// <param name="url">URL string for block help, or function that
		/// returns a URL.  Null for no help.</param>
		[External]
		internal void setHelpUrl(Any<string, Func<string>> url)
		{
			var func = url.As<Func<string>>();
			if (func == null)
				instance.setHelpUrl.call(instance, (string)url);
			else
				instance.setHelpUrl.call(instance, Script.NewFunc(func));
		}

		/// <summary>
		/// Change the tooltip text for a block.
		/// </summary>
		/// <param name="newTip">newTip Text for tooltip or a parent element to
		/// link to for its tooltip.  May be a function that returns a string.</param>
		[External]
		internal void setTooltip(Any<string, Func<string>> newTip)
		{
			var func = newTip.As<Func<string>>();
			if (func == null)
				instance.setTooltip.call(instance, (string)newTip);
			else
				instance.setTooltip.call(instance, Script.NewFunc(func));
		}

		/// <summary>
		/// Get the colour of a block.
		/// </summary>
		/// <returns>#RRGGBB string.</returns>
		[External]
		internal string getColour()
		{
			var ret = instance.getColour.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Change the colour of a block.
		/// </summary>
		/// <param name="colour">HSV hue value, or #RRGGBB string.</param>
		[External]
		internal void setColour(Any<int, string> colour)
		{
			instance.setColour.call(instance, colour.Value);
		}

		/// <summary>
		/// Returns the named field from a block.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <returns>Named field, or null if field does not exist.</returns>
		[External]
		internal Blockly.Field getField(string name)
		{
			return Script.CreateField(instance.getField.call(instance, name));
		}

		/// <summary>
		/// Return all variables referenced by this block.
		/// </summary>
		/// <returns>List of variable names.</returns>
		[External]
		internal string[] getVars_()
		{
			var vars = instance.getVars.call(instance);
			if ((vars == null) || (vars is DBNull))
				return null;
			var ret = new List<string>();
			int len = vars.length;
			for (int i = 0; i < len; i++) {
				ret.Add(Script.Get(vars, i.ToString()));
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Notification that a variable is renaming.
		/// If the name matches one of this block's variables, rename it.
		/// </summary>
		/// <param name="oldName">Previous name of variable.</param>
		/// <param name="newName">Renamed variable.</param>
		[External]
		internal void renameVar_(string oldName, string newName)
		{
			instance.renameVar.call(instance, oldName, newName);
		}

		/// <summary>
		/// Returns the language-neutral value from the field of a block.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <returns>Value from the field or null if field does not exist.</returns>
		[External]
		internal string getFieldValue(string name)
		{
			var ret = instance.getFieldValue.call(instance, name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Returns the language-neutral value from the field of a block.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <returns>Value from the field or null if field does not exist.</returns>
		[External]
		internal string getTitleValue(string name)
		{
			var ret = instance.getTitleValue.call(instance, name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Change the field value for a block (e.g. 'CHOOSE' or 'REMOVE').
		/// </summary>
		/// <param name="newValue">Value to be the new field.</param>
		/// <param name="name">The name of the field.</param>
		[External]
		internal void setFieldValue(string newValue, string name)
		{
			instance.setFieldValue.call(instance, newValue, name);
		}

		/// <summary>
		/// Change the field value for a block (e.g. 'CHOOSE' or 'REMOVE').
		/// </summary>
		/// <param name="newValue">Value to be the new field.</param>
		/// <param name="name">The name of the field.</param>
		[External]
		internal void setTitleValue(string newValue, string name)
		{
			instance.setTitleValue.call(instance, newValue, name);
		}

		/// <summary>
		/// Set whether this block can chain onto the bottom of another block.
		/// </summary>
		/// <param name="newBoolean">True if there can be a previous statement.</param>
		/// <param name="opt_check">Statement type or
		/// list of statement types.  Null/undefined if any type could be connected.</param>
		[External]
		internal void setPreviousStatement(bool newBoolean, Any<string, string[]> opt_check = null)
		{
			var check = opt_check == null ? null : opt_check.As<string[]>();
			if (check != null)
				instance.setPreviousStatement.call(instance, newBoolean, Script.NewArray(check));
			else
				instance.setPreviousStatement.call(instance, newBoolean, (string)opt_check);
		}

		/// <summary>
		/// Set whether another block can chain onto the bottom of this block.
		/// </summary>
		/// <param name="newBoolean">True if there can be a next statement.</param>
		/// <param name="opt_check">Statement type or
		/// list of statement types.  Null/undefined if any type could be connected.</param>
		[External]
		internal void setNextStatement(bool newBoolean, Any<string, string[]> opt_check = null)
		{
			var check = opt_check == null ? null : opt_check.As<string[]>();
			if (check != null)
				instance.setNextStatement.call(instance, newBoolean, Script.NewArray(check));
			else
				instance.setNextStatement.call(instance, newBoolean, (string)opt_check);
		}

		/// <summary>
		/// Set whether this block returns a value.
		/// </summary>
		/// <param name="newBoolean">True if there is an output.</param>
		/// <param name="opt_check">Returned type or list
		/// of returned types.  Null or undefined if any type could be returned
		/// (e.g. variable get).</param>
		[External]
		internal void setOutput(bool newBoolean, Any<string, string[]> opt_check = null)
		{
			var check = opt_check == null ? null : opt_check.As<string[]>();
			if (check != null)
				instance.setOutput.call(instance, newBoolean, Script.NewArray(check));
			else
				instance.setOutput.call(instance, newBoolean, (string)opt_check);
		}

		/// <summary>
		/// Set whether value inputs are arranged horizontally or vertically.
		/// </summary>
		/// <param name="newBoolean">True if inputs are horizontal.</param>
		[External]
		internal void setInputsInline(bool newBoolean)
		{
			instance.setInputsInline.call(instance, newBoolean);
		}

		/// <summary>
		/// Get whether value inputs are arranged horizontally or vertically.
		/// </summary>
		/// <returns>True if inputs are horizontal.</returns>
		[External]
		internal bool getInputsInline()
		{
			var ret = instance.getInputsInline.call(instance);
			if ((ret == null) || (ret is DBNull))
				return false;
			return ret;
		}

		/// <summary>
		/// Set whether the block is disabled or not.
		/// </summary>
		/// <param name="disabled">True if disabled.</param>
		[External]
		internal void setDisabled(bool disabled)
		{
			instance.setDisabled.call(instance, disabled);
		}

		/// <summary>
		/// Get whether the block is disabled or not due to parents.
		/// The block's own disabled property is not considered.
		/// </summary>
		/// <returns>True if disabled.</returns>
		[External]
		internal bool getInheritedDisabled()
		{
			var ret = instance.getInheritedDisabled.call(instance);
			if ((ret == null) || (ret is DBNull))
				return false;
			return ret;
		}

		/// <summary>
		/// Get whether the block is collapsed or not.
		/// </summary>
		/// <returns>True if collapsed.</returns>
		[External]
		internal bool isCollapsed()
		{
			return instance.isCollapsed.call(instance);
		}

		/// <summary>
		/// Set whether the block is collapsed or not.
		/// </summary>
		/// <param name="collapsed">True if collapsed.</param>
		[External]
		internal void setCollapsed(bool collapsed)
		{
			instance.setCollapsed.call(instance, collapsed);
		}

		/// <summary>
		/// Create a human-readable text representation of this block and any children.
		/// </summary>
		/// <param name="opt_maxLength">Truncate the string to this length.</param>
		/// <returns>Text of block.</returns>
		[External]
		internal string toString(int opt_maxLength = 0)
		{
			return instance.toString.call(instance, opt_maxLength);
		}

		/// <summary>
		/// Shortcut for appending a value input row.
		/// </summary>
		/// <param name="name">Language-neutral identifier which may used to find this
		/// input again.  Should be unique to this block.</param>
		/// <returns>The input object created.</returns>
		[External]
		internal Blockly.Input appendValueInput(string name)
		{
			return Script.CreateInput(instance.appendValueInput.call(instance, name));
		}

		/// <summary>
		/// Shortcut for appending a statement input row.
		/// </summary>
		/// <param name="name">Language-neutral identifier which may used to find this
		/// input again.  Should be unique to this block.</param>
		/// <returns>The input object created.</returns>
		[External]
		internal Blockly.Input appendStatementInput(string name)
		{
			return Script.CreateInput(instance.appendStatementInput.call(instance, name));
		}

		/// <summary>
		/// Shortcut for appending a dummy input row.
		/// </summary>
		/// <param name="opt_name">Language-neutral identifier which may used to find
		/// this input again.  Should be unique to this block.</param>
		/// <returns>The input object created.</returns>
		[External]
		internal Blockly.Input appendDummyInput(string opt_name = null)
		{
			return Script.CreateInput(instance.appendDummyInput.call(instance, opt_name));
		}

		/// <summary>
		/// Initialize this block using a cross-platform, internationalization-friendly
		/// JSON description.
		/// </summary>
		/// <param name="json">Structured data describing the block.</param>
		[External]
		internal void jsonInit(object json)
		{
			var s = Codeplex.Data.DynamicJson.Serialize(json);
			instance.jsonInit.call(instance, Script.Parse(s));
		}

		/// <summary>
		/// Move a named input to a different location on this block.
		/// </summary>
		/// <param name="name">The name of the input to move.</param>
		/// <param name="refName">Name of input that should be after the moved input,
		/// or null to be the input at the end.</param>
		[External]
		internal void moveInputBefore(string name, string refName)
		{
			instance.moveInputBefore.call(instance, name, refName);
		}

		/// <summary>
		/// Move a numbered input to a different location on this block.
		/// </summary>
		/// <param name="inputIndex">Index of the input to move.</param>
		/// <param name="refIndex">Index of input that should be after the moved input.</param>
		[External]
		internal void moveNumberedInputBefore(int inputIndex, int refIndex)
		{
			instance.moveNumberedInputBefore.call(instance, inputIndex, refIndex);
		}

		/// <summary>
		/// Remove an input from this block.
		/// </summary>
		/// <param name="name">The name of the input.</param>
		/// <param name="opt_quiet">True to prevent error if input is not present.</param>
		[External]
		internal void removeInput(string name, bool opt_quiet = false)
		{
			instance.removeInput.call(instance, name, opt_quiet);
		}

		/// <summary>
		/// Fetches the named input object.
		/// </summary>
		/// <param name="name">The name of the input.</param>
		/// <returns>The input object, or null if input does not exist.</returns>
		[External]
		internal Blockly.Input getInput(string name)
		{
			var ret = instance.getInput.call(instance, name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateInput(ret);
		}

		/// <summary>
		/// Fetches the block attached to the named input.
		/// </summary>
		/// <param name="name">The name of the input.</param>
		/// <returns>The attached value block, or null if the input is
		/// either disconnected or if the input does not exist.</returns>
		[External]
		internal Block getInputTargetBlock(string name)
		{
			var ret = instance.getInputTargetBlock.call(instance, name);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Script.CreateBlock(ret);
		}

		/// <summary>
		/// Returns the comment on this block (or '' if none).
		/// </summary>
		/// <returns>Block's comment.</returns>
		[External]
		internal string getCommentText()
		{
			var ret = instance.getCommentText.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Set this block's comment text.
		/// </summary>
		/// <param name="text">text The text, or null to delete.</param>
		[External]
		internal void setCommentText(string text)
		{
			instance.setCommentText.call(instance, text);
		}

		/// <summary>
		/// Set this block's warning text.
		/// </summary>
		/// <param name="text">The text, or null to delete.</param>
		[External]
		internal void setWarningText(string text)
		{
			instance.setWarningText.call(instance, text);
		}

		/// <summary>
		/// Give this block a mutator dialog.
		/// </summary>
		/// <param name="mutator">A mutator dialog instance or null to remove.</param>
		[External]
		internal void setMutator(Blockly.Mutator mutator)
		{
			instance.setMutator.call(instance, mutator.instance);
		}

		/// <summary>
		/// Return the coordinates of the top-left corner of this block relative to the
		/// drawing surface's origin (0,0).
		/// </summary>
		/// <returns>Object with .x and .y properties.</returns>
		[External]
		internal goog.math.Coordinate getRelativeToSurfaceXY()
		{
			return new goog.math.Coordinate(instance.getRelativeToSurfaceXY.call(instance));
		}

		/// <summary>
		/// Move a block by a relative offset.
		/// </summary>
		/// <param name="dx">Horizontal offset.</param>
		/// <param name="dy">Vertical offset.</param>
		[External]
		internal void moveBy(double dx, double dy)
		{
			instance.moveBy.call(instance, dx, dy);
		}

		// BlockSvg

		/// <summary>
		/// Create and initialize the SVG representation of the block.
		/// May be called more than once.
		/// </summary>
		[External]
		internal void initSvg()
		{
			instance.initSvg.call(instance);
		}

		/// <summary>
		/// Select this block.  Highlight it visually.
		/// </summary>
		[External]
		internal void select()
		{
			instance.select.call(instance);
		}

		/// <summary>
		/// Unselect this block.  Remove its highlighting.
		/// </summary>
		[External]
		internal void unselect()
		{
			instance.unselect.call(instance);
		}

		/// <summary>
		/// Returns a list of mutator, comment, and warning icons.
		/// </summary>
		/// <returns>List of icons.</returns>
		[External]
		internal object[] getIcons()
		{
			var ret = instance.getIcons.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return new object[] { ret };
		}


		/// <summary>
		/// Snap this block to the nearest grid point.
		/// </summary>
		[External]
		internal void snapToGrid()
		{
			instance.snapToGrid.call(instance);
		}

		/// <summary>
		/// Returns a bounding box describing the dimensions of this block
		/// and any blocks stacked below it.
		/// </summary>
		/// <returns>Object with height and width properties.</returns>
		[External]
		internal goog.math.Size getHeightWidth()
		{
			return new goog.math.Size(instance.getHeightWidth.call(instance));
		}

		/// <summary>
		/// Returns the coordinates of a bounding box describing the dimensions of this
		/// block and any blocks stacked below it.
		/// </summary>
		/// <returns>Object with top left and bottom right coordinates of the bounding box.</returns>
		[External]
		internal Blockly.Rectangle getBoundingRectangle()
		{
			return new Blockly.Rectangle(instance.getBoundingRectangle.call(instance));
		}

		/// <summary>
		/// Open the next (or previous) FieldTextInput.
		/// </summary>
		/// <param name="start">Current location.</param>
		/// <param name="forward">If true go forward, otherwise backward.</param>
		[External]
		internal void tab(Any<Blockly.Field, Block> start, bool forward)
		{
			instance.tab.call(instance, start.Value, forward);
		}

		/// <summary>
		/// Add or remove the UI indicating if this block is movable or not.
		/// </summary>
		[External]
		internal void updateMovable()
		{
			instance.updateMovable.call(instance);
		}

		/// <summary>
		/// Return the root node of the SVG or null if none exists.
		/// </summary>
		/// <returns>The root SVG node (probably a group).</returns>
		[External]
		internal Element getSvgRoot()
		{
			var ret = instance.getSvgRoot.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Element.Create(ret);
		}

		/// <summary>
		/// Play some UI effects (sound, animation) when disposing of a block.
		/// </summary>
		[External]
		internal void disposeUiEffect()
		{
			instance.disposeUiEffect.call(instance);
		}

		/// <summary>
		/// Play some UI effects (sound, ripple) after a connection has been established.
		/// </summary>
		[External]
		internal void connectionUiEffect()
		{
			instance.connectionUiEffect.call(instance);
		}

		/// <summary>
		/// Play some UI effects (sound, animation) when disconnecting a block.
		/// </summary>
		[External]
		internal void disconnectUiEffect()
		{
			instance.disconnectUiEffect.call(instance);
		}

		/// <summary>
		/// Change the colour of a block.
		/// </summary>
		[External]
		internal void updateColour()
		{
			instance.updateColour.call(instance);
		}

		/// <summary>
		/// Enable or disable a block.
		/// </summary>
		[External]
		internal void updateDisabled()
		{
			instance.updateDisabled.call(instance);
		}

		/// <summary>
		/// Select this block.  Highlight it visually.
		/// </summary>
		[External]
		internal void addSelect()
		{
			instance.addSelect.call(instance);
		}

		/// <summary>
		/// Unselect this block.  Remove its highlighting.
		/// </summary>
		[External]
		internal void removeSelect()
		{
			instance.removeSelect.call(instance);
		}

		/// <summary>
		/// Adds the dragging class to this block.
		/// Also disables the highlights/shadows to improve performance.
		/// </summary>
		[External]
		internal void addDragging()
		{
			instance.addDragging.call(instance);
		}

		/// <summary>
		/// Removes the dragging class from this block.
		/// </summary>
		[External]
		internal void removeDragging()
		{
			instance.removeDragging.call(instance);
		}

		/// <summary>
		/// Render the block.
		/// Lays out and reflows a block based on its contents and settings.
		/// </summary>
		/// <param name="opt_bubble">If false, just render this block.
		/// If true, also render block's parent, grandparent, etc.  Defaults to true.</param>
		[External]
		internal void render(bool opt_bubble = false)
		{
			instance.render.call(instance, opt_bubble);
		}
	}

	public class ContextMenuOption
	{
		internal dynamic instance;

		public bool enabled {
			get {
				return Script.Get(instance, "enabled");
			}
			set {
				Script.Set(instance, "enabled", value);
			}
		}
		public string text {
			get {
				return Script.Get(instance, "text");
			}
			set {
				Script.Set(instance, "text", value);
			}
		}
		public dynamic callback {
			get {
				return Script.Get(instance, "callback");
			}
			set {
				Script.Set(instance, "callback", value);
			}
		}

		public ContextMenuOption()
		{
			instance = Script.NewObject();
		}

		internal void push(ContextMenuOption option)
		{
			object value = enabled;
			if ((value != null) || !(value is DBNull)) {
				Script.Set(instance, "enabled", value);
			}
			value = text;
			if ((value != null) || !(value is DBNull)) {
				Script.Set(instance, "text", value);
			}
			value = callback;
			if ((value != null) || !(value is DBNull)) {
				Script.Set(instance, "callback", value);
			}
		}
	}

	public class ContextMenuOptionList
	{
		internal dynamic instance;

		public ContextMenuOptionList(object instance)
		{
			this.instance = instance;
		}

		public ContextMenuOption this[int index] {
			get { return Script.Get(instance, index.ToString()); }
			set { Script.Set(instance, index.ToString(), value == null ? null : value.instance); }
		}

		public int Length { get { return Script.Get<int>(instance, "length"); } }

		internal void push(ContextMenuOption option)
		{
			instance.push.call(instance, option.instance);
		}
	}
}

[Name("Blockly.Msg")]
public class Msg
{
	/*"@metadata": {
		"authors": [
			"Shirayuki",
			"Oda",
			"아라",
			"Otokoume",
			"Sujiniku",
			"Sgk",
			"TAKAHASHI Shuuji"
		]
	},*/
	public const string ADD_COMMENT = "コメントを追加";
	public const string CHANGE_VALUE_TITLE = "値を変更します。";
	public const string CLEAN_UP = "ブロックの整理";
	public const string COLLAPSE_ALL = "全てのブロックを折りたたむ";
	public const string COLLAPSE_BLOCK = "ブロックを折りたたむ";
	public const string COLOUR_BLEND_COLOUR1 = "色 1";
	public const string COLOUR_BLEND_COLOUR2 = "色 2";
	public const string COLOUR_BLEND_HELPURL = "http://meyerweb.com/eric/tools/color-blend/";
	public const string COLOUR_BLEND_RATIO = "割合";
	public const string COLOUR_BLEND_TITLE = "ブレンド";
	public const string COLOUR_BLEND_TOOLTIP = "ブレンド2 つの色を指定された比率に混ぜる(0.0 ～ 1.0)。";
	public const string COLOUR_PICKER_HELPURL = "https://ja.wikipedia.org/wiki/色";
	public const string COLOUR_PICKER_TOOLTIP = "パレットから色を選んでください。";
	public const string COLOUR_RANDOM_HELPURL = "http://randomcolour.com";  // untranslated
	public const string COLOUR_RANDOM_TITLE = "ランダムな色";
	public const string COLOUR_RANDOM_TOOLTIP = "ランダムな色を選択します。";
	public const string COLOUR_RGB_BLUE = "青";
	public const string COLOUR_RGB_GREEN = "緑";
	public const string COLOUR_RGB_HELPURL = "http://www.december.com/html/spec/colorper.html";
	public const string COLOUR_RGB_RED = "赤";
	public const string COLOUR_RGB_TITLE = "カラーと";
	public const string COLOUR_RGB_TOOLTIP = "赤、緑、および青の指定された量で色を作成します。すべての値は 0 ～ 100 の間でなければなりません。";
	public const string CONTROLS_FLOW_STATEMENTS_HELPURL = "https://github.com/google/blockly/wiki/Loops#loop-termination-blocks";  // untranslated
	public const string CONTROLS_FLOW_STATEMENTS_OPERATOR_BREAK = "ループから抜け出す";
	public const string CONTROLS_FLOW_STATEMENTS_OPERATOR_CONTINUE = "ループの次の反復処理を続行します。";
	public const string CONTROLS_FLOW_STATEMENTS_TOOLTIP_BREAK = "含むループから抜け出します。";
	public const string CONTROLS_FLOW_STATEMENTS_TOOLTIP_CONTINUE = "このループの残りの部分をスキップし、次のイテレーションに進みます。";
	public const string CONTROLS_FLOW_STATEMENTS_WARNING = "注意: このブロックは、ループ内でのみ使用します。";
	public const string CONTROLS_FOREACH_HELPURL = "https://github.com/google/blockly/wiki/Loops#for-each";  // untranslated
	public const string CONTROLS_FOREACH_INPUT_DO = CONTROLS_REPEAT_INPUT_DO;
	public const string CONTROLS_FOREACH_TITLE = "各項目の %1 リストで %2";
	public const string CONTROLS_FOREACH_TOOLTIP = "リストの各項目に対して変数 '%1' のアイテムに設定し、いくつかのステートメントをしてください。";
	public const string CONTROLS_FOR_HELPURL = "https://github.com/google/blockly/wiki/Loops#count-with";  // untranslated
	public const string CONTROLS_FOR_INPUT_DO = CONTROLS_REPEAT_INPUT_DO;
	public const string CONTROLS_FOR_TITLE = "で、カウントします。 %1 %2 から%3、 %4 で";
	public const string CONTROLS_FOR_TOOLTIP = "変数 \"%1\"は、指定した間隔ごとのカウントを開始番号から 終了番号まで、値をとり、指定したブロックを行う必要があります。";
	public const string CONTROLS_IF_ELSEIF_TITLE_ELSEIF = CONTROLS_IF_MSG_ELSEIF;
	public const string CONTROLS_IF_ELSEIF_TOOLTIP = "場合に条件にブロック追加。";
	public const string CONTROLS_IF_ELSE_TITLE_ELSE = CONTROLS_IF_MSG_ELSE;
	public const string CONTROLS_IF_ELSE_TOOLTIP = "Ifブロックに、すべてをキャッチする条件を追加。";
	public const string CONTROLS_IF_HELPURL = "https://github.com/google/blockly/wiki/IfElse";  // untranslated
	public const string CONTROLS_IF_IF_TITLE_IF = CONTROLS_IF_MSG_IF;
	public const string CONTROLS_IF_IF_TOOLTIP = "追加、削除、またはセクションを順序変更して、ブロックをこれを再構成します。";
	public const string CONTROLS_IF_MSG_ELSE = "他";
	public const string CONTROLS_IF_MSG_ELSEIF = "他でもし";
	public const string CONTROLS_IF_MSG_IF = "もし";
	public const string CONTROLS_IF_MSG_THEN = CONTROLS_REPEAT_INPUT_DO;
	public const string CONTROLS_IF_TOOLTIP_1 = "値が true の場合はその後ステートメントを行をいくつかします。";
	public const string CONTROLS_IF_TOOLTIP_2 = "値が true 場合は、ステートメントの最初のブロックを行います。それ以外の場合は、ステートメントの 2 番目のブロックを行います。";
	public const string CONTROLS_IF_TOOLTIP_3 = "最初の値が true 場合は、ステートメントの最初のブロックを行います。それ以外の場合は、2 番目の値が true の場合、ステートメントの 2 番目のブロックをします。";
	public const string CONTROLS_IF_TOOLTIP_4 = "最初の値が true 場合は、ステートメントの最初のブロックを行います。2 番目の値が true の場合は、ステートメントの 2 番目のブロックを行います。それ以外の場合は最後のブロックのステートメントを行います。";
	public const string CONTROLS_REPEAT_HELPURL = "https://ja.wikipedia.org/wiki/for文";
	public const string CONTROLS_REPEAT_INPUT_DO = "してください";
	public const string CONTROLS_REPEAT_TITLE = "%1 回、繰り返します";
	public const string CONTROLS_REPEAT_TOOLTIP = "いくつかのステートメントを数回行います。";
	public const string CONTROLS_WHILEUNTIL_HELPURL = "https://github.com/google/blockly/wiki/Loops#repeat";  // untranslated
	public const string CONTROLS_WHILEUNTIL_INPUT_DO = CONTROLS_REPEAT_INPUT_DO;
	public const string CONTROLS_WHILEUNTIL_OPERATOR_UNTIL = "次になるまで繰り返す";
	public const string CONTROLS_WHILEUNTIL_OPERATOR_WHILE = "次の間、繰り返す";
	public const string CONTROLS_WHILEUNTIL_TOOLTIP_UNTIL = "値は false の間、いくつかのステートメントを行います。";
	public const string CONTROLS_WHILEUNTIL_TOOLTIP_WHILE = "値は true の間、いくつかのステートメントを行います。";
	public const string DELETE_ALL_BLOCKS = "%1件のすべてのブロックを消しますか？";
	public const string DELETE_BLOCK = "ブロックを消す";
	public const string DELETE_VARIABLE = "変数 %1 を消す";
	public const string DELETE_VARIABLE_CONFIRMATION = "%1 ヶ所で使われている変数 '%2' を削除しますか？";
	public const string DELETE_X_BLOCKS = "%1 個のブロックを消す";
	public const string DISABLE_BLOCK = "ブロックを無効にします。";
	public const string DUPLICATE_BLOCK = "複製";
	public const string ENABLE_BLOCK = "ブロックを有効にします。";
	public const string EXPAND_ALL = "ブロックを展開します。";
	public const string EXPAND_BLOCK = "ブロックを展開します。";
	public const string EXTERNAL_INPUTS = "外部入力";
	public const string HELP = "ヘルプ";
	public const string INLINE_INPUTS = "インライン入力";
	public const string LISTS_CREATE_EMPTY_HELPURL = "https://github.com/google/blockly/wiki/Lists#create-empty-list";
	public const string LISTS_CREATE_EMPTY_TITLE = "空のリストを作成します。";
	public const string LISTS_CREATE_EMPTY_TOOLTIP = "長さゼロ、データ レコード空のリストを返します";
	public const string LISTS_CREATE_WITH_CONTAINER_TITLE_ADD = "リスト";
	public const string LISTS_CREATE_WITH_CONTAINER_TOOLTIP = "追加、削除、またはセクションを順序変更して、ブロックを再構成します。";
	public const string LISTS_CREATE_WITH_HELPURL = "https://github.com/google/blockly/wiki/Lists#create-list-with";  // untranslated
	public const string LISTS_CREATE_WITH_INPUT_WITH = "これを使ってリストを作成します。";
	public const string LISTS_CREATE_WITH_ITEM_TITLE = VARIABLES_DEFAULT_NAME;
	public const string LISTS_CREATE_WITH_ITEM_TOOLTIP = "リストにアイテムを追加します。";
	public const string LISTS_CREATE_WITH_TOOLTIP = "アイテム数かぎりないのリストを作成します。";
	public const string LISTS_GET_INDEX_FIRST = "最初";
	public const string LISTS_GET_INDEX_FROM_END = "終しまいから #";
	public const string LISTS_GET_INDEX_FROM_START = "#";
	public const string LISTS_GET_INDEX_GET = "取得";
	public const string LISTS_GET_INDEX_GET_REMOVE = "取得と削除";
	public const string LISTS_GET_INDEX_HELPURL = LISTS_INDEX_OF_HELPURL;
	public const string LISTS_GET_INDEX_INPUT_IN_LIST = LISTS_INLIST;
	public const string LISTS_GET_INDEX_LAST = "最後";
	public const string LISTS_GET_INDEX_RANDOM = "ランダム";
	public const string LISTS_GET_INDEX_REMOVE = "削除";
	public const string LISTS_GET_INDEX_TAIL = "";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_FIRST = "リストの最初の項目を返信します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_FROM = "リスト内の指定位置にある項目を返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_LAST = "リストの最後の項目を返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_RANDOM = "ランダム アイテム リストを返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_FIRST = "リスト内の最初の項目を削除したあと返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_FROM = "リスト内の指定位置にある項目を削除し、返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_LAST = "リスト内の最後の項目を削除したあと返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_RANDOM = "リストのランダムなアイテムを削除し、返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_REMOVE_FIRST = "リスト内の最初の項目を削除します。";
	public const string LISTS_GET_INDEX_TOOLTIP_REMOVE_FROM = "リスト内の指定位置にある項目を返します。";
	public const string LISTS_GET_INDEX_TOOLTIP_REMOVE_LAST = "リスト内の最後の項目を削除します。";
	public const string LISTS_GET_INDEX_TOOLTIP_REMOVE_RANDOM = "リスト内にある任意のアイテムを削除します。";
	public const string LISTS_GET_SUBLIST_END_FROM_END = "最後から＃へ";
	public const string LISTS_GET_SUBLIST_END_FROM_START = "＃へ";
	public const string LISTS_GET_SUBLIST_END_LAST = "最後へ";
	public const string LISTS_GET_SUBLIST_HELPURL = "https://github.com/google/blockly/wiki/Lists#getting-a-sublist";  // untranslated
	public const string LISTS_GET_SUBLIST_INPUT_IN_LIST = LISTS_INLIST;
	public const string LISTS_GET_SUBLIST_START_FIRST = "最初からサブリストを取得する。";
	public const string LISTS_GET_SUBLIST_START_FROM_END = "端から #のサブリストを取得します。";
	public const string LISTS_GET_SUBLIST_START_FROM_START = "# からサブディレクトリのリストを取得します。";
	public const string LISTS_GET_SUBLIST_TAIL = "";
	public const string LISTS_GET_SUBLIST_TOOLTIP = "リストの指定された部分のコピーを作成してくださ。";
	public const string LISTS_INDEX_FROM_END_TOOLTIP = "%1 は、最後の項目です。";
	public const string LISTS_INDEX_FROM_START_TOOLTIP = "%1 は、最初の項目です。";
	public const string LISTS_INDEX_OF_FIRST = "最初に見つかった項目を検索します。";
	public const string LISTS_INDEX_OF_HELPURL = "https://github.com/google/blockly/wiki/Lists#getting-items-from-a-list";  // untranslated
	public const string LISTS_INDEX_OF_INPUT_IN_LIST = LISTS_INLIST;
	public const string LISTS_INDEX_OF_LAST = "最後に見つかったアイテムを見つける";
	public const string LISTS_INDEX_OF_TOOLTIP = "リスト項目の最初/最後に出現するインデックス位置を返します。項目が見つからない場合は %1 を返します。";
	public const string LISTS_INLIST = "リストで";
	public const string LISTS_ISEMPTY_HELPURL = "https://github.com/google/blockly/wiki/Lists#is-empty";  // untranslated
	public const string LISTS_ISEMPTY_TITLE = "%1 が空";
	public const string LISTS_ISEMPTY_TOOLTIP = "リストが空の場合は、true を返します。";
	public const string LISTS_LENGTH_HELPURL = "https://github.com/google/blockly/wiki/Lists#length-of";  // untranslated
	public const string LISTS_LENGTH_TITLE = " %1の長さ";
	public const string LISTS_LENGTH_TOOLTIP = "リストの長さを返します。";
	public const string LISTS_REPEAT_HELPURL = "https://github.com/google/blockly/wiki/Lists#create-list-with";  // untranslated
	public const string LISTS_REPEAT_TITLE = "アイテム %1 と一緒にリストを作成し %2 回繰り";
	public const string LISTS_REPEAT_TOOLTIP = "指定された値をなんどか繰り返してリストを作ります。";
	public const string LISTS_SET_INDEX_HELPURL = "https://github.com/google/blockly/wiki/Lists#in-list--set";  // untranslated
	public const string LISTS_SET_INDEX_INPUT_IN_LIST = LISTS_INLIST;
	public const string LISTS_SET_INDEX_INPUT_TO = "として";
	public const string LISTS_SET_INDEX_INSERT = "挿入します。";
	public const string LISTS_SET_INDEX_SET = "セット";
	public const string LISTS_SET_INDEX_TOOLTIP = "";
	public const string LISTS_SET_INDEX_TOOLTIP_INSERT_FIRST = "リストの先頭に項目を挿入します。";
	public const string LISTS_SET_INDEX_TOOLTIP_INSERT_FROM = "リスト内の指定位置に項目を挿入します。";
	public const string LISTS_SET_INDEX_TOOLTIP_INSERT_LAST = "リストの末尾に項目を追加します。";
	public const string LISTS_SET_INDEX_TOOLTIP_INSERT_RANDOM = "リストに項目をランダムに挿入します。";
	public const string LISTS_SET_INDEX_TOOLTIP_SET_FIRST = "リスト内に最初の項目を設定します。";
	public const string LISTS_SET_INDEX_TOOLTIP_SET_FROM = "リスト内の指定された位置に項目を設定します。";
	public const string LISTS_SET_INDEX_TOOLTIP_SET_LAST = "リスト内の最後の項目を設定します。";
	public const string LISTS_SET_INDEX_TOOLTIP_SET_RANDOM = "リスト内にランダムなアイテムを設定します。";
	public const string LISTS_SORT_HELPURL = "https://github.com/google/blockly/wiki/Lists#sorting-a-list";  // untranslated
	public const string LISTS_SORT_ORDER_ASCENDING = "昇順";
	public const string LISTS_SORT_ORDER_DESCENDING = "降順";
	public const string LISTS_SORT_TITLE = "sort %1 %2 %3";  // untranslated
	public const string LISTS_SORT_TOOLTIP = "Sort a copy of a list.";  // untranslated
	public const string LISTS_SORT_TYPE_IGNORECASE = "alphabetic, ignore case";  // untranslated
	public const string LISTS_SORT_TYPE_NUMERIC = "numeric";  // untranslated
	public const string LISTS_SORT_TYPE_TEXT = "alphabetic";  // untranslated
	public const string LISTS_SPLIT_HELPURL = "https://github.com/google/blockly/wiki/Lists#splitting-strings-and-joining-lists";  // untranslated
	public const string LISTS_SPLIT_LIST_FROM_TEXT = "テキストからリストを作る";
	public const string LISTS_SPLIT_TEXT_FROM_LIST = "リストからテキストを作る";
	public const string LISTS_SPLIT_TOOLTIP_JOIN = "Join a list of texts into one text, separated by a delimiter.";  // untranslated
	public const string LISTS_SPLIT_TOOLTIP_SPLIT = "Split text into a list of texts, breaking at each delimiter.";  // untranslated
	public const string LISTS_SPLIT_WITH_DELIMITER = "with delimiter";  // untranslated
	public const string LOGIC_BOOLEAN_FALSE = "false";
	public const string LOGIC_BOOLEAN_HELPURL = "https://github.com/google/blockly/wiki/Logic#values";  // untranslated
	public const string LOGIC_BOOLEAN_TOOLTIP = "True または false を返します。";
	public const string LOGIC_BOOLEAN_TRUE = "true";
	public const string LOGIC_COMPARE_HELPURL = "https://ja.wikipedia.org/wiki/不等式";
	public const string LOGIC_COMPARE_TOOLTIP_EQ = "もし両方がお互いに等しく入力した場合は true を返します。";
	public const string LOGIC_COMPARE_TOOLTIP_GT = "最初の入力が 2 番目の入力よりも大きい場合は true を返します。";
	public const string LOGIC_COMPARE_TOOLTIP_GTE = "もし入力がふたつめの入よりも大きかったらtrueをり返します。";
	public const string LOGIC_COMPARE_TOOLTIP_LT = "最初の入力が 2 番目の入力よりも小さいい場合は true を返します。";
	public const string LOGIC_COMPARE_TOOLTIP_LTE = "もし、最初の入力が二つ目入力より少ないか、おなじであったらTRUEをかえしてください";
	public const string LOGIC_COMPARE_TOOLTIP_NEQ = "両方の入力が互いに等しくない場合に true を返します。";
	public const string LOGIC_NEGATE_HELPURL = "https://ja.wikipedia.org/wiki/否定";
	public const string LOGIC_NEGATE_TITLE = "%1 ではないです。";
	public const string LOGIC_NEGATE_TOOLTIP = "入力が false の場合は、true を返します。入力が true の場合は false を返します。";
	public const string LOGIC_NULL = "null";
	public const string LOGIC_NULL_HELPURL = "https://en.wikipedia.org/wiki/Nullable_type";
	public const string LOGIC_NULL_TOOLTIP = "Null を返します。";
	public const string LOGIC_OPERATION_AND = "そして";
	public const string LOGIC_OPERATION_HELPURL = "https://github.com/google/blockly/wiki/Logic#logical-operations";  // untranslated
	public const string LOGIC_OPERATION_OR = "または";
	public const string LOGIC_OPERATION_TOOLTIP_AND = "両方の入力が同じ場合は true を返します。";
	public const string LOGIC_OPERATION_TOOLTIP_OR = "最低少なくとも 1 つの入力が true の場合は true を返します。";
	public const string LOGIC_TERNARY_CONDITION = "テスト";
	public const string LOGIC_TERNARY_HELPURL = "https://ja.wikipedia.org/wiki/%3F:";
	public const string LOGIC_TERNARY_IF_FALSE = "false の場合";
	public const string LOGIC_TERNARY_IF_TRUE = "true の場合";
	public const string LOGIC_TERNARY_TOOLTIP = "'テスト' の条件をチェックします。条件が true の場合、'true' の値を返します。それ以外の場合 'false' のを返します。";
	public const string MATH_ADDITION_SYMBOL = "+";
	public const string MATH_ARITHMETIC_HELPURL = "https://ja.wikipedia.org/wiki/算術";
	public const string MATH_ARITHMETIC_TOOLTIP_ADD = "2 つの数の合計を返します。";
	public const string MATH_ARITHMETIC_TOOLTIP_DIVIDE = "2 つの数の商を返します。";
	public const string MATH_ARITHMETIC_TOOLTIP_MINUS = "2 つの数の差を返します。";
	public const string MATH_ARITHMETIC_TOOLTIP_MULTIPLY = "2 つの数の積を返します。";
	public const string MATH_ARITHMETIC_TOOLTIP_POWER = "最初の数を2 番目の値で累乗した結果を返します。";
	public const string MATH_CHANGE_HELPURL = "https://ja.wikipedia.org/wiki/加法";
	public const string MATH_CHANGE_TITLE = "変更 %1 に %2";
	public const string MATH_CHANGE_TITLE_ITEM = VARIABLES_DEFAULT_NAME;
	public const string MATH_CHANGE_TOOLTIP = "'%1' をたします。";
	public const string MATH_CONSTANT_HELPURL = "https://ja.wikipedia.org/wiki/数学定数";
	public const string MATH_CONSTANT_TOOLTIP = "いずれかの共通の定数のを返す: π (3.141…), e (2.718…), φ (1.618…), sqrt(2) (1.414…), sqrt(½) (0.707…), or ∞ (無限).";
	public const string MATH_CONSTRAIN_HELPURL = "https://en.wikipedia.org/wiki/Clamping_%28graphics%29";  // untranslated
	public const string MATH_CONSTRAIN_TITLE = "制限%1下リミット%2上限リミット%3";
	public const string MATH_CONSTRAIN_TOOLTIP = "値を、上限 x と下限 y の間に制限する（上限と下限が、x と  y　とに同じ場合は、上限の値は　x, 下限の値はy）。";
	public const string MATH_DIVISION_SYMBOL = "÷";
	public const string MATH_IS_DIVISIBLE_BY = "割り切れる";
	public const string MATH_IS_EVEN = "わ偶数";
	public const string MATH_IS_NEGATIVE = "負の値";
	public const string MATH_IS_ODD = "奇数です。";
	public const string MATH_IS_POSITIVE = "正の値";
	public const string MATH_IS_PRIME = "素数です";
	public const string MATH_IS_TOOLTIP = "数字が、偶数、奇数、素数、整数、正数、負数、またはそれが特定の数で割り切れる場合かどうかを確認してください。どの制限が一つでも本当でしたら true をかえしてください、そうでない場合わ falseを返してください。";
	public const string MATH_IS_WHOLE = "は整数";
	public const string MATH_MODULO_HELPURL = "https://en.wikipedia.org/wiki/Modulo_operation";
	public const string MATH_MODULO_TITLE = "残りの %1 ÷ %2";
	public const string MATH_MODULO_TOOLTIP = "2つの数値を除算した余りを返します。";
	public const string MATH_MULTIPLICATION_SYMBOL = "×";
	public const string MATH_NUMBER_HELPURL = "https://ja.wikipedia.org/wiki/数";
	public const string MATH_NUMBER_TOOLTIP = "数です。";
	public const string MATH_ONLIST_HELPURL = "";
	public const string MATH_ONLIST_OPERATOR_AVERAGE = "リストの平均";
	public const string MATH_ONLIST_OPERATOR_MAX = "リストの最大値";
	public const string MATH_ONLIST_OPERATOR_MEDIAN = "リストの中央値";
	public const string MATH_ONLIST_OPERATOR_MIN = "リストの最小の数";
	public const string MATH_ONLIST_OPERATOR_MODE = "一覧モード";
	public const string MATH_ONLIST_OPERATOR_RANDOM = "リストのランダム アイテム";
	public const string MATH_ONLIST_OPERATOR_STD_DEV = "リストの標準偏差";
	public const string MATH_ONLIST_OPERATOR_SUM = "リストの合計";
	public const string MATH_ONLIST_TOOLTIP_AVERAGE = "リストの数値の平均 (算術平均) を返します。";
	public const string MATH_ONLIST_TOOLTIP_MAX = "リストの最大数を返します。";
	public const string MATH_ONLIST_TOOLTIP_MEDIAN = "リストの中央値の数を返します。";
	public const string MATH_ONLIST_TOOLTIP_MIN = "リストの最小数を返します。";
	public const string MATH_ONLIST_TOOLTIP_MODE = "リストで最も一般的な項目のリストを返します。";
	public const string MATH_ONLIST_TOOLTIP_RANDOM = "リストからランダムに要素を返します。";
	public const string MATH_ONLIST_TOOLTIP_STD_DEV = "リウトの標準偏差をかえす";
	public const string MATH_ONLIST_TOOLTIP_SUM = "全部リストの数をたして返す";
	public const string MATH_POWER_SYMBOL = "^";
	public const string MATH_RANDOM_FLOAT_HELPURL = "https://en.wikipedia.org/wiki/Random_number_generation";
	public const string MATH_RANDOM_FLOAT_TITLE_RANDOM = "ランダムな分数";
	public const string MATH_RANDOM_FLOAT_TOOLTIP = "ランダムな分数を返すー0.0 (包括) の間のと 1.0 (排他的な)。";
	public const string MATH_RANDOM_INT_HELPURL = "https://en.wikipedia.org/wiki/Random_number_generation";
	public const string MATH_RANDOM_INT_TITLE = "%1 から %2 への無作為の整数";
	public const string MATH_RANDOM_INT_TOOLTIP = "指定した下限の間、無作為なランダムな整数を返します。";
	public const string MATH_ROUND_HELPURL = "https://ja.wikipedia.org/wiki/端数処理";
	public const string MATH_ROUND_OPERATOR_ROUND = "概数";
	public const string MATH_ROUND_OPERATOR_ROUNDDOWN = "端数を切り捨てる";
	public const string MATH_ROUND_OPERATOR_ROUNDUP = "数値を切り上げ";
	public const string MATH_ROUND_TOOLTIP = "数値を切り上げるか切り捨てる";
	public const string MATH_SINGLE_HELPURL = "https://ja.wikipedia.org/wiki/平方根";
	public const string MATH_SINGLE_OP_ABSOLUTE = "絶対値";
	public const string MATH_SINGLE_OP_ROOT = "平方根";
	public const string MATH_SINGLE_TOOLTIP_ABS = "絶対値を返す";
	public const string MATH_SINGLE_TOOLTIP_EXP = "数値の e 粂を返す";
	public const string MATH_SINGLE_TOOLTIP_LN = "数値の自然対数をかえしてください";
	public const string MATH_SINGLE_TOOLTIP_LOG10 = "log 10 を返す。";
	public const string MATH_SINGLE_TOOLTIP_NEG = "負の数を返す";
	public const string MATH_SINGLE_TOOLTIP_POW10 = "１０の　x　乗";
	public const string MATH_SINGLE_TOOLTIP_ROOT = "平方根を返す";
	public const string MATH_SUBTRACTION_SYMBOL = "-";
	public const string MATH_TRIG_ACOS = "acos";
	public const string MATH_TRIG_ASIN = "asin";
	public const string MATH_TRIG_ATAN = "atan";
	public const string MATH_TRIG_COS = "cos";
	public const string MATH_TRIG_HELPURL = "https://ja.wikipedia.org/wiki/三角関数";
	public const string MATH_TRIG_SIN = "sin";
	public const string MATH_TRIG_TAN = "tan";
	public const string MATH_TRIG_TOOLTIP_ACOS = "arccosine の値を返す";
	public const string MATH_TRIG_TOOLTIP_ASIN = "番号のarcsine を返すます";
	public const string MATH_TRIG_TOOLTIP_ATAN = "番号のarctangent を返すます";
	public const string MATH_TRIG_TOOLTIP_COS = "番号のcosineの次数を返す";
	public const string MATH_TRIG_TOOLTIP_SIN = "番号のsineの次数を返す";
	public const string MATH_TRIG_TOOLTIP_TAN = "番号のtangentの次数を返す";
	public const string NEW_VARIABLE = "新しい変数";
	public const string NEW_VARIABLE_TITLE = "新しい変数の、名前";
	public const string ORDINAL_NUMBER_SUFFIX = "";
	public const string PROCEDURES_ALLOW_STATEMENTS = "allow statements";  // untranslated
	public const string PROCEDURES_BEFORE_PARAMS = "で。";
	public const string PROCEDURES_CALLNORETURN_HELPURL = "https://ja.wikipedia.org/wiki/サブルーチン";
	public const string PROCEDURES_CALLNORETURN_TOOLTIP = "ユーザー定義関数 '%1' を実行します。";
	public const string PROCEDURES_CALLRETURN_HELPURL = "https://ja.wikipedia.org/wiki/サブルーチン";
	public const string PROCEDURES_CALLRETURN_TOOLTIP = "ユーザー定義関数 '%1' を実行し、その出力を使用します。";
	public const string PROCEDURES_CALL_BEFORE_PARAMS = "で。";
	public const string PROCEDURES_CREATE_DO = "%1をつくる";
	public const string PROCEDURES_DEFNORETURN_COMMENT = "Describe this function...";  // untranslated
	public const string PROCEDURES_DEFNORETURN_DO = "";
	public const string PROCEDURES_DEFNORETURN_HELPURL = "https://ja.wikipedia.org/wiki/サブルーチン";
	public const string PROCEDURES_DEFNORETURN_PROCEDURE = "何かしてください";
	public const string PROCEDURES_DEFNORETURN_TITLE = "宛先";
	public const string PROCEDURES_DEFNORETURN_TOOLTIP = "出力なしで関数を作成します。";
	public const string PROCEDURES_DEFRETURN_COMMENT = PROCEDURES_DEFNORETURN_COMMENT;
	public const string PROCEDURES_DEFRETURN_HELPURL = "https://ja.wikipedia.org/wiki/サブルーチン";
	public const string PROCEDURES_DEFRETURN_PROCEDURE = PROCEDURES_DEFNORETURN_PROCEDURE;
	public const string PROCEDURES_DEFRETURN_RETURN = "返す";
	public const string PROCEDURES_DEFRETURN_TITLE = PROCEDURES_DEFNORETURN_TITLE;
	public const string PROCEDURES_DEFRETURN_TOOLTIP = "出力を持つ関数を作成します。";
	public const string PROCEDURES_DEF_DUPLICATE_WARNING = "警告: この関数は、重複するパラメーターがあります。";
	public const string PROCEDURES_HIGHLIGHT_DEF = "関数の内容を強調表示します。";
	public const string PROCEDURES_IFRETURN_HELPURL = "http://c2.com/cgi/wiki?GuardClause";  // untranslated
	public const string PROCEDURES_IFRETURN_TOOLTIP = "1番目値が true の場合、2 番目の値を返します。";
	public const string PROCEDURES_IFRETURN_WARNING = "警告: このブロックは、関数定義内でのみ使用できます。";
	public const string PROCEDURES_MUTATORARG_TITLE = "入力名:";
	public const string PROCEDURES_MUTATORARG_TOOLTIP = "Add an input to the function.";  // untranslated
	public const string PROCEDURES_MUTATORCONTAINER_TITLE = "入力";
	public const string PROCEDURES_MUTATORCONTAINER_TOOLTIP = "Add, remove, or reorder inputs to this function.";  // untranslated
	public const string REDO = "やり直し";
	public const string REMOVE_COMMENT = "コメントを削除";
	public const string RENAME_VARIABLE = "変数の名前を変更.";
	public const string RENAME_VARIABLE_TITLE = "%1の変数すべてを名前変更します。";
	public const string TEXT_APPEND_APPENDTEXT = "テキストを追加します。";
	public const string TEXT_APPEND_HELPURL = "https://github.com/google/blockly/wiki/Text#text-modification";  // untranslated
	public const string TEXT_APPEND_TO = "宛先";
	public const string TEXT_APPEND_TOOLTIP = "変数 '%1' にいくつかのテキストを追加します。";
	public const string TEXT_APPEND_VARIABLE = VARIABLES_DEFAULT_NAME;
	public const string TEXT_CHANGECASE_HELPURL = "https://github.com/google/blockly/wiki/Text#adjusting-text-case";  // untranslated
	public const string TEXT_CHANGECASE_OPERATOR_LOWERCASE = "小文字に";
	public const string TEXT_CHANGECASE_OPERATOR_TITLECASE = "タイトル ケースに";
	public const string TEXT_CHANGECASE_OPERATOR_UPPERCASE = "大文字に変換する";
	public const string TEXT_CHANGECASE_TOOLTIP = "別のケースに、テキストのコピーを返します。";
	public const string TEXT_CHARAT_FIRST = "最初の文字を得る";
	public const string TEXT_CHARAT_FROM_END = "一番最後の言葉、キャラクターを所得";
	public const string TEXT_CHARAT_FROM_START = "文字# を取得";
	public const string TEXT_CHARAT_HELPURL = "https://github.com/google/blockly/wiki/Text#extracting-text";  // untranslated
	public const string TEXT_CHARAT_INPUT_INTEXT = "テキストで";
	public const string TEXT_CHARAT_LAST = "最後の文字を得る";
	public const string TEXT_CHARAT_RANDOM = "ランダムな文字を得る";
	public const string TEXT_CHARAT_TAIL = "";
	public const string TEXT_CHARAT_TOOLTIP = "指定された位置に文字を返します。";
	public const string TEXT_CREATE_JOIN_ITEM_TITLE_ITEM = VARIABLES_DEFAULT_NAME;
	public const string TEXT_CREATE_JOIN_ITEM_TOOLTIP = "テキスト をアイテム追加します。";
	public const string TEXT_CREATE_JOIN_TITLE_JOIN = "結合";
	public const string TEXT_CREATE_JOIN_TOOLTIP = "追加、削除、またはセクションを順序変更して、ブロックを再構成します。";
	public const string TEXT_GET_SUBSTRING_END_FROM_END = "文字列の＃ 終わりからの＃";
	public const string TEXT_GET_SUBSTRING_END_FROM_START = "# の文字";
	public const string TEXT_GET_SUBSTRING_END_LAST = "最後のの文字";
	public const string TEXT_GET_SUBSTRING_HELPURL = "https://github.com/google/blockly/wiki/Text#extracting-a-region-of-text";  // untranslated
	public const string TEXT_GET_SUBSTRING_INPUT_IN_TEXT = "テキストで";
	public const string TEXT_GET_SUBSTRING_START_FIRST = "部分文字列を取得する。";
	public const string TEXT_GET_SUBSTRING_START_FROM_END = "部分文字列を取得する #端から得る";
	public const string TEXT_GET_SUBSTRING_START_FROM_START = "文字列からの部分文字列を取得 ＃";
	public const string TEXT_GET_SUBSTRING_TAIL = "";
	public const string TEXT_GET_SUBSTRING_TOOLTIP = "テキストの指定部分を返します。";
	public const string TEXT_INDEXOF_HELPURL = "https://github.com/google/blockly/wiki/Text#finding-text";  // untranslated
	public const string TEXT_INDEXOF_INPUT_INTEXT = "テキストで";
	public const string TEXT_INDEXOF_OPERATOR_FIRST = "テキストの最初の出現箇所を検索します。";
	public const string TEXT_INDEXOF_OPERATOR_LAST = "テキストの最後に見つかったを検索します。";
	public const string TEXT_INDEXOF_TAIL = "";
	public const string TEXT_INDEXOF_TOOLTIP = "最初のテキストの二番目のてきすとの、最初と最後の、出現したインデックスをかえします。テキストが見つからない場合は %1 を返します。";
	public const string TEXT_ISEMPTY_HELPURL = "https://github.com/google/blockly/wiki/Text#checking-for-empty-text";  // untranslated
	public const string TEXT_ISEMPTY_TITLE = "%1 が空";
	public const string TEXT_ISEMPTY_TOOLTIP = "指定されたテキストが空の場合は、true を返します。";
	public const string TEXT_JOIN_HELPURL = "https://github.com/google/blockly/wiki/Text#text-creation";  // untranslated
	public const string TEXT_JOIN_TITLE_CREATEWITH = "テキストを作成します。";
	public const string TEXT_JOIN_TOOLTIP = "任意の数の項目一部を一緒に接合してテキストの作成します。";
	public const string TEXT_LENGTH_HELPURL = "https://github.com/google/blockly/wiki/Text#text-modification";  // untranslated
	public const string TEXT_LENGTH_TITLE = "%1 の長さ";
	public const string TEXT_LENGTH_TOOLTIP = "指定されたテキストの文字 (スペースを含む) の数を返します。";
	public const string TEXT_PRINT_HELPURL = "https://github.com/google/blockly/wiki/Text#printing-text";  // untranslated
	public const string TEXT_PRINT_TITLE = "%1 を印刷します。";
	public const string TEXT_PRINT_TOOLTIP = "指定したテキスト、番号または他の値を印刷します。";
	public const string TEXT_PROMPT_HELPURL = "https://github.com/google/blockly/wiki/Text#getting-input-from-the-user";  // untranslated
	public const string TEXT_PROMPT_TOOLTIP_NUMBER = "ユーザーにプロンプトで数字のインプットを求めます";
	public const string TEXT_PROMPT_TOOLTIP_TEXT = "ユーザーにプロンプトでテキストのインプットを求めます";
	public const string TEXT_PROMPT_TYPE_NUMBER = "プロンプトで数字の入力を求める";
	public const string TEXT_PROMPT_TYPE_TEXT = "プロンプトでテキストの入力を求める";
	public const string TEXT_TEXT_HELPURL = "https://ja.wikipedia.org/wiki/文字列";
	public const string TEXT_TEXT_TOOLTIP = "文字、単語、または行のテキスト。";
	public const string TEXT_TRIM_HELPURL = "https://github.com/google/blockly/wiki/Text#trimming-removing-spaces";  // untranslated
	public const string TEXT_TRIM_OPERATOR_BOTH = "両端のスペースを取り除く";
	public const string TEXT_TRIM_OPERATOR_LEFT = "左端のスペースを取り除く";
	public const string TEXT_TRIM_OPERATOR_RIGHT = "右端のスペースを取り除く";
	public const string TEXT_TRIM_TOOLTIP = "スペースを 1 つまたは両方の端から削除したのち、テキストのコピーを返します。";
	public const string TODAY = "今日";
	public const string UNDO = "取り消し";
	public const string VARIABLES_DEFAULT_NAME = "項目";
	public const string VARIABLES_GET_CREATE_SET = "'セット%1を作成します。";
	public const string VARIABLES_GET_HELPURL = "https://github.com/google/blockly/wiki/Variables#get";  // untranslated
	public const string VARIABLES_GET_TOOLTIP = "この変数の値を返します。";
	public const string VARIABLES_SET = "セット %1 宛先 %2";
	public const string VARIABLES_SET_CREATE_GET = "'%1 を取得' を作成します。";
	public const string VARIABLES_SET_HELPURL = "https://github.com/google/blockly/wiki/Variables#set";  // untranslated
	public const string VARIABLES_SET_TOOLTIP = "この入力を変数と等しくなるように設定します。";
	public const string VARIABLE_ALREADY_EXISTS = "変数名 '%1' は既に存在しています。";
	public const string VIEWS_MAIN_MENU_VIEW_LOAD_ERROR = "{$filename}{$error}";
	public const string VIEWS_MAIN_MENU_VIEW_LOAD_SUCCEEDED = "ロードしました";
}
