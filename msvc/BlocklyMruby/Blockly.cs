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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bridge;
using Bridge.Html5;
using BlocklyMruby;

/**
 * @fileoverview Core JavaScript library for Blockly.
 * @author fraser@google.com (Neil Fraser)
 */
[External]
public static partial class Blockly
{
	internal static dynamic instance;

	/// <summary>
	/// Number of pixels the mouse must move before a drag starts.
	/// </summary>
	[Name(false), FieldProperty]
	public static int DRAG_RADIUS { get { return instance.DRAG_RADIUS; } }

	/// <summary>
	/// Maximum misalignment between connections for them to snap together.
	/// </summary>
	[Name(false), FieldProperty]
	public static int SNAP_RADIUS { get { return instance.SNAP_RADIUS; } }

	/// <summary>
	/// Delay in ms between trigger and bumping unconnected block out of alignment.
	/// </summary>
	[Name(false), FieldProperty]
	public static int BUMP_DELAY { get { return instance.BUMP_DELAY; } }

	/// <summary>
	/// Number of characters to truncate a collapsed block to.
	/// </summary>
	[Name(false), FieldProperty]
	public static int COLLAPSE_CHARS { get { return instance.COLLAPSE_CHARS; } }

	/// <summary>
	/// Length in ms for a touch to become a long press.
	/// </summary>
	[Name(false), FieldProperty]
	public static int LONGPRESS { get { return instance.LONGPRESS; } }

	/// <summary>
	/// Prevent a sound from playing if another sound preceded it within this many
	/// miliseconds.
	/// </summary>
	[Name(false), FieldProperty]
	public static int SOUND_LIMIT { get { return instance.SOUND_LIMIT; } }

	/// <summary>
	/// The richness of block colours, regardless of the hue.
	/// Must be in the range of 0 (inclusive) to 1 (exclusive).
	/// </summary>
	[Name(false), FieldProperty]
	public static double HSV_SATURATION { get { return instance.HSV_SATURATION; } }

	/// <summary>
	/// The intensity of block colours, regardless of the hue.
	/// Must be in the range of 0 (inclusive) to 1 (exclusive).
	/// </summary>
	[Name(false), FieldProperty]
	public static double HSV_VALUE { get { return instance.HSV_VALUE; } }

	/// <summary>
	/// Sprited icons and images.
	/// </summary>
	[Name(false), FieldProperty]
	public static object SPRITE { get { return instance.SPRITE; } }

	/// <summary>
	/// Required name space for SVG elements.
	/// </summary>
	[Name(false), FieldProperty]
	public static string SVG_NS { get { return instance.SVG_NS; } }

	/// <summary>
	/// Required name space for HTML elements.
	/// </summary>
	[Name(false), FieldProperty]
	public static string HTML_NS { get { return instance.HTML_NS; } }

	/// <summary>
	/// ENUM for a right-facing value input.  E.g. 'set item to' or 'return'.
	/// </summary>
	[Name(false), FieldProperty]
	public static int INPUT_VALUE { get { return instance.INPUT_VALUE; } }

	/// <summary>
	/// ENUM for a left-facing value output.  E.g. 'random fraction'.
	/// </summary>
	[Name(false), FieldProperty]
	public static int OUTPUT_VALUE { get { return instance.OUTPUT_VALUE; } }

	/// <summary>
	/// ENUM for a down-facing block stack.  E.g. 'if-do' or 'else'.
	/// </summary>
	[Name(false), FieldProperty]
	public static int NEXT_STATEMENT { get { return instance.NEXT_STATEMENT; } }

	/// <summary>
	/// ENUM for an up-facing block stack.  E.g. 'break out of loop'.
	/// </summary>
	[Name(false), FieldProperty]
	public static int PREVIOUS_STATEMENT { get { return instance.PREVIOUS_STATEMENT; } }

	/// <summary>
	/// ENUM for an dummy input.  Used to add field(s) with no input.
	/// </summary>
	[Name(false), FieldProperty]
	public static int DUMMY_INPUT { get { return instance.DUMMY_INPUT; } }

	/// <summary>
	/// ENUM for left alignment.
	/// </summary>
	[Name(false), FieldProperty]
	public static int ALIGN_LEFT { get { return instance.ALIGN_LEFT; } }

	/// <summary>
	/// ENUM for centre alignment.
	/// </summary>
	[Name(false), FieldProperty]
	public static int ALIGN_CENTRE { get { return instance.ALIGN_CENTRE; } }

	/// <summary>
	/// ENUM for right alignment.
	/// </summary>
	[Name(false), FieldProperty]
	public static int ALIGN_RIGHT { get { return instance.ALIGN_RIGHT; } }

	/// <summary>
	/// ENUM for no drag operation.
	/// </summary>
	[Name(false), FieldProperty]
	public static int DRAG_NONE { get { return instance.DRAG_NONE; } }

	/// <summary>
	/// ENUM for inside the sticky DRAG_RADIUS.
	/// </summary>
	[Name(false), FieldProperty]
	public static int DRAG_STICKY { get { return instance.DRAG_STICKY; } }

	/// <summary>
	/// ENUM for inside the non-sticky DRAG_RADIUS, for differentiating between
	/// clicks and drags.
	/// </summary>
	[Name(false), FieldProperty]
	public static int DRAG_BEGIN { get { return instance.DRAG_BEGIN; } }

	/// <summary>
	/// ENUM for freely draggable (outside the DRAG_RADIUS, if one applies).
	/// </summary>
	[Name(false), FieldProperty]
	public static int DRAG_FREE { get { return instance.DRAG_FREE; } }

	/// <summary>
	/// Lookup table for determining the opposite type of a connection.
	/// </summary>
	[Name(false), FieldProperty]
	public static int[] OPPOSITE_TYPE { get { return instance.OPPOSITE_TYPE; } }

	/// <summary>
	/// ENUM for toolbox and flyout at top of screen.
	/// </summary>
	[Name(false), FieldProperty]
	public static int TOOLBOX_AT_TOP { get { return instance.TOOLBOX_AT_TOP; } }

	/// <summary>
	/// ENUM for toolbox and flyout at bottom of screen.
	/// </summary>
	[Name(false), FieldProperty]
	public static int TOOLBOX_AT_BOTTOM { get { return instance.TOOLBOX_AT_BOTTOM; } }

	/// <summary>
	/// ENUM for toolbox and flyout at left of screen.
	/// </summary>
	[Name(false), FieldProperty]
	public static int TOOLBOX_AT_LEFT { get { return instance.TOOLBOX_AT_LEFT; } }

	/// <summary>
	/// ENUM for toolbox and flyout at right of screen.
	/// </summary>
	[Name(false), FieldProperty]
	public static int TOOLBOX_AT_RIGHT { get { return instance.TOOLBOX_AT_RIGHT; } }

	/// <summary>
	/// The main workspace most recently used.
	/// Set by Blockly.WorkspaceSvg.prototype.markFocused
	/// </summary>
	[FieldProperty]
	public static WorkspaceSvg mainWorkspace { get { return WorkspaceSvg.Create(instance.mainWorkspace); } }

	/// <summary>
	/// Currently selected block.
	/// </summary>
	[FieldProperty]
	public static Block selected { get { return BlocklyScript.CreateBlock(instance.selected); } }

	[Name(false), FieldProperty]
	public static object Blocks { get { return instance.Blocks; } }

	public static class Xml
	{
		/// <summary>
		/// Encode a block tree as XML.
		/// </summary>
		/// <param name="workspace">workspace The workspace containing blocks.</param>
		/// <returns>XML document.</returns>
		public static Element workspaceToDom(Workspace workspace)
		{
			return Element.Create(BlocklyScript.XmlWorkspaceToDom(workspace.instance));
		}

		/// <summary>
		/// Encode a block subtree as XML with XY coordinates.
		/// </summary>
		/// <param name="block">The root block to encode.</param>
		/// <returns>Tree of XML elements.</returns>
		public static Element blockToDomWithXY(Block block)
		{
			return Element.Create(BlocklyScript.XmlBlockToDomWithXY(block.instance));
		}

		/// <summary>
		/// Encode a block subtree as XML.
		/// </summary>
		/// <param name="block">The root block to encode.</param>
		/// <returns>Tree of XML elements.</returns>
		public static Element blockToDom(Block block)
		{
			return Element.Create(BlocklyScript.XmlBlockToDom(block.instance));
		}

		/// <summary>
		/// Converts a DOM structure into plain text.
		/// Currently the text format is fairly ugly: all one line with no whitespace.
		/// </summary>
		/// <param name="dom">A tree of XML elements.</param>
		/// <returns>Text representation.</returns>
		public static string domToText(Element dom)
		{
			return BlocklyScript.XmlDomToText(dom.instance);
		}

		/// <summary>
		/// Converts a DOM structure into properly indented text.
		/// </summary>
		/// <param name="dom">A tree of XML elements.</param>
		/// <returns>Text representation.</returns>
		public static string domToPrettyText(Element dom)
		{
			return BlocklyScript.XmlDomToPrettyText(dom.instance);
		}

		/// <summary>
		/// Converts plain text into a DOM structure.
		/// Throws an error if XML doesn't parse.
		/// </summary>
		/// <param name="text">Text representation.</param>
		/// <returns>A tree of XML elements.</returns>
		public static Element textToDom(string text)
		{
			return Element.Create(BlocklyScript.XmlTextToDom(text));
		}

		/// <summary>
		/// Decode an XML DOM and create blocks on the workspace.
		/// </summary>
		/// <param name="xml">XML DOM.</param>
		/// <param name="workspace">The workspace.</param>
		public static void domToWorkspace(Element xml, Workspace workspace)
		{
			BlocklyScript.XmlDomToWorkspace(xml.instance, workspace.instance);
		}

		/// <summary>
		/// Decode an XML block tag and create a block (and possibly sub blocks) on the
		/// workspace.
		/// </summary>
		/// <param name="xmlBlock">XML block element.</param>
		/// <param name="workspace">The workspace.</param>
		/// <returns>The root block created.</returns>
		public static Block domToBlock(Element xmlBlock, Workspace workspace)
		{
			return BlocklyScript.CreateBlock(BlocklyScript.XmlDomToBlock(xmlBlock.instance, workspace.instance));
		}

		/// <summary>
		/// Remove any 'next' block (statements in a stack).
		/// </summary>
		/// <param name="xmlBlock">XML block element.</param>
		public static void deleteNext(Element xmlBlock)
		{
			BlocklyScript.XmlDeleteNext(xmlBlock.instance);
		}
	}

	/**
	 * @fileoverview Object representing a workspace.
	 * @author fraser@google.com (Neil Fraser)
	 */
	public class Workspace
	{
		internal dynamic instance;
		[FieldProperty]
		public string id { get { return instance.id; } }
		[FieldProperty]
		public Options options { get { return new Options(instance.options); } }
		[Name(false), FieldProperty]
		public bool RTL { get { return instance.RTL; } }
		[FieldProperty]
		public bool horizontalLayout { get { return instance.horizontalLayout; } }
		[FieldProperty]
		public int toolboxPosition { get { return instance.toolboxPosition; } }
		[FieldProperty]
		public bool isFlyout { get { return instance.isFlyout; } }

		/// <summary>
		/// A list of all of the named variables in the workspace, including variables
		/// that are not currently in use.
		/// </summary>
		[FieldProperty]
		public string[] variableList {
			get {
				var ret = instance.variableList;
				int len = Script.Get(ret, "length");
				var result = new List<string>(len);
				for (int i = 0; i < len; i++) {
					result.Add(Script.Get<string>(ret, i.ToString()));
				}
				return result.ToArray();
			}
		}

		/// <summary>
		/// Workspaces may be headless.
		/// True if visible.  False if headless.
		/// </summary>
		[FieldProperty]
		public bool rendered { get { return instance.rendered; } }

		/// <summary>
		/// Maximum number of undo events in stack.
		/// 0 to turn off undo, Infinity for unlimited.
		/// </summary>
		[Name(false), FieldProperty]
		public int MAX_UNDO { get { return instance.MAX_UNDO; } }

		protected Workspace(object instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Class for a workspace.  This is a data structure that contains blocks.
		/// There is no UI, and can be created headlessly.
		/// </summary>
		/// <param name="opt_options">Dictionary of options.</param>
		public void construct(Options opt_options = null)
		{
			instance = Script.New("Blockly.Workspace", new object[] { opt_options });
		}

		/// <summary>
		/// Dispose of this workspace.
		/// Unlink from all DOM elements to prevent memory leaks.
		/// </summary>
		public void dispose()
		{
			instance.dispose();
		}

		/// <summary>
		/// Angle away from the horizontal to sweep for blocks.  Order of execution is
		/// generally top to bottom, but a small angle changes the scan to give a bit of
		/// a left to right bias (reversed in RTL).  Units are in degrees.
		/// See: http://tvtropes.org/pmwiki/pmwiki.php/Main/DiagonalBilling.
		/// </summary>
		public static int SCAN_ANGLE = 3;

		/// <summary>
		/// Add a block to the list of top blocks.
		/// </summary>
		/// <param name="block">Block to remove.</param>
		public void addTopBlock(Block block)
		{
			instance.addTopBlock.call(instance, block.instance);
		}

		/// <summary>
		/// Remove a block from the list of top blocks.
		/// </summary>
		/// <param name="block">Block to remove.</param>
		public void removeTopBlock(Block block)
		{
			instance.removeTopBlock.call(instance, block.instance);
		}

		/// <summary>
		/// Finds the top-level blocks and returns them.  Blocks are optionally sorted
		/// by position; top to bottom (with slight LTR or RTL bias).
		/// </summary>
		/// <param name="ordered">Sort the list if true.</param>
		/// <returns>The top-level block objects.</returns>
		public BlockList getTopBlocks(bool ordered)
		{
			var blocks = instance.getTopBlocks.call(instance, ordered);
			if ((blocks == null) || (blocks is DBNull))
				return null;
			return new BlockList(blocks);
		}

		/// <summary>
		/// Find all blocks in workspace.  No particular order.
		/// </summary>
		/// <returns></returns>
		public BlockList getAllBlocks()
		{
			var blocks = instance.getAllBlocks.call(instance);
			if ((blocks == null) || (blocks is DBNull))
				return null;
			return new BlockList(blocks);
		}

		/// <summary>
		/// Dispose of all blocks in workspace.
		/// </summary>
		public void clear()
		{
			instance.clear.call(instance);
		}

		/// <summary>
		/// Returns the horizontal offset of the workspace.
		/// Intended for LTR/RTL compatibility in XML.
		/// Not relevant for a headless workspace.
		/// </summary>
		/// <returns>Width.</returns>
		public int getWidth()
		{
			var ret = instance.getWidth.call(instance);
			if ((ret == null) || (ret is DBNull))
				return 0;
			return ret;
		}


		/// <summary>
		/// Obtain a newly created block.
		/// </summary>
		/// <param name="prototypeName">Name of the language object containing
		/// type-specific functions for this block.</param>
		/// <param name="opt_id">Optional ID.  Use this ID if provided, otherwise
		/// create a new id.</param>
		/// <returns>The created block.</returns>
		public Block newBlock(string prototypeName, string opt_id = null)
		{
			return BlocklyScript.CreateBlock(instance.newBlock.call(instance, prototypeName, opt_id));
		}

		/// <summary>
		/// The number of blocks that may be added to the workspace before reaching
		/// the maxBlocks.
		/// </summary>
		/// <returns></returns>
		public int remainingCapacity()
		{
			return instance.remainingCapacity.call(instance);
		}

		/// <summary>
		/// Undo or redo the previous action.
		/// </summary>
		/// <param name="redo">False if undo, true if redo.</param>
		public void undo(bool redo)
		{
			instance.undo.call(instance, redo);
		}

		/// <summary>
		/// Clear the undo/redo stacks.
		/// </summary>
		public void clearUndo()
		{
			instance.clearUndo.call(instance);
		}

		Dictionary<object, Action<Events.Abstract>> Listeners = new Dictionary<object, Action<Events.Abstract>>();

		/// <summary>
		/// When something in this workspace changes, call a function.
		/// </summary>
		/// <param name="func">Function to call.</param>
		/// <returns>Function that can be passed to
		/// removeChangeListener.</returns>
		public Action<Events.Abstract> addChangeListener(Action<Events.Abstract> func)
		{
			var a = func == null ? null : Script.NewFunc(new Action<object>((e) => {
				func(BlocklyScript.CreateEvent(e));
			}));
			if (a != null)
				Listeners.Add(a, func);
			var b = instance.addChangeListener.call(instance, a);
			Action<Events.Abstract> ret;
			if (!Listeners.TryGetValue(b, out ret))
				return null;
			return ret;
		}

		/// <summary>
		/// Stop listening for this workspace's changes.
		/// </summary>
		/// <param name="func">Function to stop calling.</param>
		public void removeChangeListener(Action<Events.Abstract> func)
		{
			object _func = null;
			foreach (var kvp in Listeners) {
				if (kvp.Value != func)
					continue;

				_func = kvp.Key;
				break;
			}
			if (_func != null)
				instance.removeChangeListener.call(instance, _func);
		}

		/// <summary>
		/// Fire a change event.
		/// </summary>
		/// <param name="event">Event to fire.</param>
		public void fireChangeListener(Events.Abstract @event)
		{
			instance.fireChangeListener.call(instance, @event.instance);
		}

		/// <summary>
		/// Find the block on this workspace with the specified ID.
		/// </summary>
		/// <param name="id">ID of block to find.</param>
		/// <returns>The sought after block or null if not found.</returns>
		public Block getBlockById(string id)
		{
			var ret = instance.getBlockById.call(instance, id);
			if ((ret == null) || (ret is DBNull))
				return null;
			return BlocklyScript.CreateBlock(ret);
		}

		/// <summary>
		/// Find the workspace with the specified ID.
		/// </summary>
		/// <param name="id">ID of workspace to find.</param>
		/// <returns>The sought after workspace or null if not found.</returns>
		public static Workspace getById(string id)
		{
			return BlocklyScript.WorkspaceGetById(id);
		}

		/// <summary>
		/// Create a variable with the given name.
		/// TODO: #468
		/// </summary>
		/// <param name="name">The new variable's name.</param>
		public void createVariable(string name)
		{
			instance.createVariable.call(instance, name);
		}

		/// <summary>
		/// Check whether a variable exists with the given name.  The check is
		/// case-insensitive.
		/// </summary>
		/// <param name="name">The name to check for.</param>
		/// <returns>The index of the name in the variable list, or -1 if it is
		/// not present.</returns>
		internal int variableIndexOf(string name)
		{
			return instance.variableIndexOf.call(instance, name);
		}
	}

	public class WorkspaceSvg : Workspace
	{
		public WorkspaceSvg(object instance)
			: base(instance)
		{
		}

		internal static WorkspaceSvg Create(dynamic workspace)
		{
			if ((workspace == null) || (workspace is DBNull))
				return null;

			var instance = Script.Get(workspace, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (WorkspaceSvg)instance;

			var ret = new WorkspaceSvg(workspace);
			Script.Set(workspace, "instance", ret);
			return ret;
		}

		public new void construct(Options options)
		{
			instance = Script.New("Blockly.WorkspaceSvg", new object[] { options });
		}

		public Metrics getMetrics()
		{
			var ret = instance.getMetrics.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		public void setMetrics(Metrics metrics)
		{
			instance.setMetrics.call(instance, metrics);
		}

		/// <summary>
		/// Getter for the inverted screen CTM.
		/// </summary>
		/// <returns>The matrix to use in mouseToSvg</returns>
		public SVGMatrix getInverseScreenCTM()
		{
			var ret = instance.getInverseScreenCTM.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return new SVGMatrix(ret);
		}

		/// <summary>
		/// Update the inverted screen CTM.
		/// </summary>
		public void updateInverseScreenCTM()
		{
			instance.updateInverseScreenCTM.call(instance);
		}

		/// <summary>
		/// Save resize handler data so we can delete it later in dispose.
		/// </summary>
		/// <param name="handler">Data that can be passed to unbindEvent_.</param>
		public void setResizeHandlerWrapper(Action handler)
		{
			instance.setResizeHandlerWrapper.call(instance, handler);
		}

		/// <summary>
		/// Create the workspace DOM elements.
		/// </summary>
		/// <param name="opt_backgroundClass">opt_backgroundClass Either 'blocklyMainBackground' or
		/// 'blocklyMutatorBackground'.</param>
		/// <returns>The workspace's SVG group.</returns>
		public Element createDom(string opt_backgroundClass = null)
		{
			return Element.Create(instance.createDom.call(instance, opt_backgroundClass));
		}

		/// <summary>
		/// Dispose of this workspace.
		/// Unlink from all DOM elements to prevent memory leaks.
		/// </summary>
		public new void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Obtain a newly created block.
		/// </summary>
		/// <param name="prototypeName">Name of the language object containing
		/// type-specific functions for this block.</param>
		/// <param name="opt_id">Optional ID.  Use this ID if provided, otherwise
		/// create a new id.</param>
		/// <returns>The created block.</returns>
		public new BlockSvg newBlock(string prototypeName, string opt_id = null)
		{
			return new BlockSvg(instance.newBlock.call(instance, prototypeName, opt_id));
		}

		/// <summary>
		/// Resize the parts of the workspace that change when the workspace
		/// contents (e.g. block positions) change.  This will also scroll the
		/// workspace contents if needed.
		/// </summary>
		public void resizeContents()
		{
			instance.resizeContents.call(instance);
		}

		/// <summary>
		/// Resize and reposition all of the workspace chrome (toolbox,
		/// trash, scrollbars etc.)
		/// This should be called when something changes that
		/// requires recalculating dimensions and positions of the
		/// trash, zoom, toolbox, etc. (e.g.window resize).
		/// </summary>
		public void resize()
		{
			instance.resize.call(instance);
		}

		/// <summary>
		/// Get the SVG element that forms the drawing surface.
		/// </summary>
		/// <returns>SVG element.</returns>
		public Element getCanvas()
		{
			var ret = instance.getCanvas.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Element.Create(ret);
		}

		/// <summary>
		/// Get the SVG element that forms the bubble surface.
		/// </summary>
		/// <returns>SVG element.</returns>
		public Element getBubbleCanvas()
		{
			var ret = instance.getBubbleCanvas.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Element.Create(ret);
		}

		/// <summary>
		/// Get the SVG element that contains this workspace.
		/// </summary>
		/// <returns>SVG element.</returns>
		public Element getParentSvg()
		{
			var ret = instance.getParentSvg.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Element.Create(ret);
		}

		/// <summary>
		/// Translate this workspace to new coordinates.
		/// </summary>
		/// <param name="x">Horizontal translation.</param>
		/// <param name="y">Vertical translation.</param>
		public void translate(int x, int y)
		{
			instance.translate.call(instance, x, y);
		}

		/// <summary>
		/// Returns the horizontal offset of the workspace.
		/// Intended for LTR/RTL compatibility in XML.
		/// </summary>
		/// <returns>Width.</returns>
		public new int getWidth()
		{
			var ret = instance.getWidth.call(instance);
			if ((ret == null) || (ret is DBNull))
				return 0;
			return ret;
		}

		/// <summary>
		/// Toggles the visibility of the workspace.
		/// Currently only intended for main workspace.
		/// </summary>
		/// <param name="isVisible">True if workspace should be visible.</param>
		public void setVisible(bool isVisible)
		{
			instance.setVisible.call(instance, isVisible);
		}

		/// <summary>
		/// Render all blocks in workspace.
		/// </summary>
		public void render()
		{
			instance.render.call(instance);
		}

		/// <summary>
		/// Turn the visual trace functionality on or off.
		/// </summary>
		/// <param name="armed">True if the trace should be on.</param>
		public void traceOn(bool armed)
		{
			instance.traceOn.call(instance, armed);
		}

		/// <summary>
		/// Highlight or unhighlight a block in the workspace.
		/// </summary>
		/// <param name="id">ID of block to highlight/unhighlight,
		/// or null for no block (used to unhighlight all blocks).</param>
		/// <param name="opt_state">If undefined, highlight specified block and
		/// automatically unhighlight all others.  If true or false, manually
		/// highlight/unhighlight the specified block.</param>
		public void highlightBlock(string id, bool opt_state = false)
		{
			if (String.IsNullOrEmpty(id))
				instance.highlightBlock.call(instance, id);
			else
				instance.highlightBlock.call(instance, id, opt_state);
		}

		/// <summary>
		/// Paste the provided block onto the workspace.
		/// </summary>
		/// <param name="xmlBlock">XML block element.</param>
		public void paste(Element xmlBlock)
		{
			instance.paste.call(instance, xmlBlock.instance);
		}

		/// <summary>
		/// Make a list of all the delete areas for this workspace.
		/// </summary>
		public void recordDeleteAreas()
		{
			instance.recordDeleteAreas.call(instance);
		}

		/// <summary>
		/// Is the mouse event over a delete area (toolbox or non-closing flyout)?
		/// Opens or closes the trashcan and sets the cursor as a side effect.
		/// </summary>
		/// <param name="e">Mouse move event.</param>
		/// <returns></returns>
		public bool isDeleteArea(Event e)
		{
			return instance.isDeleteArea.call(instance, e.instance);
		}

		/// <summary>
		/// Start tracking a drag of an object on this workspace.
		/// </summary>
		/// <param name="e">Mouse down event.</param>
		/// <param name="xy">Starting location of object.</param>
		public void startDrag(Event e, goog.math.Coordinate xy)
		{
			instance.startDrag.call(instance, e.instance, xy);
		}

		/// <summary>
		/// Track a drag of an object on this workspace.
		/// </summary>
		/// <param name="e">Mouse move event.</param>
		/// <returns>New location of object.</returns>
		public goog.math.Coordinate moveDrag(Event e)
		{
			return instance.moveDrag.call(instance, e.instance);
		}

		/// <summary>
		/// Is the user currently dragging a block or scrolling the flyout/workspace?
		/// </summary>
		/// <returns>True if currently dragging or scrolling.</returns>
		public int isDragging()
		{
			var ret = instance.isDragging.call(instance);
			if (ret is int)
				return ret;
			if ((ret == null) || (ret is DBNull))
				return 0;
			return 1;
		}

		/// <summary>
		/// Calculate the bounding box for the blocks on the workspace.
		/// </summary>
		/// <returns>Contains the position and size of the bounding box
		/// containing the blocks on the workspace.
		/// {x: 0, y: 0, width: 0, height: 0}</returns>
		public object getBlocksBoundingBox()
		{
			var ret = instance.getBlocksBoundingBox.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Play a named sound at specified volume.  If volume is not specified,
		/// use full volume (1).
		/// </summary>
		/// <param name="name">Name of sound.</param>
		/// <param name="opt_volume">Volume of sound (0-1).</param>
		public void playAudio(string name, double opt_volume = 1)
		{
			instance.playAudio.call(instance, name, opt_volume);
		}

		/// <summary>
		/// Modify the block tree on the existing toolbox.
		/// </summary>
		/// <param name="tree">DOM tree of blocks, or text representation of same.</param>
		public void updateToolbox(Any<Node, string> tree)
		{
			var node = tree.As<Node>();
			if (node != null)
				instance.updateToolbox.call(instance, node.instance);
			else
				instance.updateToolbox.call(instance, tree.Value);
		}

		/// <summary>
		/// Mark this workspace as the currently focused main workspace.
		/// </summary>
		public void markFocused()
		{
			instance.markFocused.call(instance);
		}

		/// <summary>
		/// Zooming the blocks centered in (x, y) coordinate with zooming in or out.
		/// </summary>
		/// <param name="x">X coordinate of center.</param>
		/// <param name="y">Y coordinate of center.</param>
		/// <param name="type">Type of zooming (-1 zooming out and 1 zooming in).</param>
		public void zoom(double x, double y, double type)
		{
			instance.zoom.call(instance, x, y, type);
		}

		/// <summary>
		/// Zooming the blocks centered in the center of view with zooming in or out.
		/// </summary>
		/// <param name="type"></param>
		public void zoomCenter(double type)
		{
			instance.zoomCenter.call(instance, type);
		}

		/// <summary>
		/// Zoom the blocks to fit in the workspace if possible.
		/// </summary>
		public void zoomToFit()
		{
			instance.zoomToFit.call(instance);
		}

		/// <summary>
		/// Center the workspace.
		/// </summary>
		public void scrollCenter()
		{
			instance.scrollCenter.call(instance);
		}

		/// <summary>
		/// Set the workspace's zoom factor.
		/// </summary>
		/// <param name="newScale">Zoom factor.</param>
		public void setScale(double newScale)
		{
			instance.setScale.call(instance, newScale);
		}
	}

	public class Metrics
	{
		///<summary>Height of the visible rectangle</summary>
		public double viewHeight;
		///<summary>Width of the visible rectangle</summary>
		public double viewWidth;
		///<summary>Height of the contents</summary>
		public double contentHeight;
		///<summary>Width of the contents</summary>
		public double contentWidth;
		///<summary>Offset of top edge of visible rectangle from parent</summary>
		public double viewTop;
		///<summary>Offset of the top-most content from the y=0 coordinate</summary>
		public double contentTop;
		///<summary>Top-edge of view</summary>
		public double absoluteTop;
		///<summary>Offset of the left edge of visible rectangle from parent</summary>
		public double viewLeft;
		///<summary>Offset of the left-most content from the x=0 coordinate</summary>
		public double contentLeft;
		///<summary>Left-edge of view</summary>
		public double absoluteLeft;
	}

	public static class Events
	{
		[Name(false)]
		public const string CREATE = "create";
		[Name(false)]
		public const string DELETE = "delete";
		[Name(false)]
		public const string CHANGE = "change";
		[Name(false)]
		public const string MOVE = "move";
		[Name(false)]
		public const string UI = "ui";

		public static bool recordUndo {
			get { return BlocklyScript.EventsGetRecordUndo(); }
			set { BlocklyScript.EventsSetRecordUndo(value); }
		}

		private static string[] getDescendantIds_(Block block)
		{
			var ret = BlocklyScript.EventsGetDescendantIds_(block.instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return new string[] { ret };
		}

		public class Abstract
		{
			internal dynamic instance;

			/// <summary>
			/// One of Blockly.Events.CREATE, Blockly.Events.DELETE, Blockly.Events.CHANGE, Blockly.Events.MOVE, Blockly.Events.UI.
			/// </summary>
			[Name(false), FieldProperty]
			internal string type { get { return instance.type; } }

			/// <summary>
			/// UUID of workspace. The workspace can be found with Blockly.Workspace.getById(event.workspaceId)
			/// </summary>
			[Name(false), FieldProperty]
			internal string workspaceId { get { return instance.workspaceId; } }

			/// <summary>
			/// UUID of block. The block can be found with workspace.getBlockById(event.blockId)
			/// </summary>
			[Name(false), FieldProperty]
			internal string blockId { get { return instance.blockId; } }

			/// <summary>
			/// UUID of group. Some events are part of an indivisible group, such as inserting a statement in a stack.
			/// </summary>
			[Name(false), FieldProperty]
			internal string group { get { return instance.group; } }

			[Name(false), FieldProperty]
			internal bool recordUndo { get { return instance.recordUndo; } }

			public Abstract(object instance)
			{
				this.instance = instance;
			}
		}

		public class Create : Abstract
		{
			/// <summary>
			/// An XML tree defining the new block and any connected child blocks.
			/// </summary>
			[Name(false), FieldProperty]
			internal Element xml {
				get { return Element.Create(instance.xml); }
				private set { instance.xml = xml.instance; }
			}

			/// <summary>
			/// An array containing the UUIDs of the new block and any connected child blocks.
			/// </summary>
			[Name(false), FieldProperty]
			internal string[] ids {
				get {
					var ret = instance.ids;
					var result = new List<string>();
					var len = Script.Get(ret, "length");
					for (int i = 0; i < len; i++) {
						result.Add(Script.Get(ret, i.ToString()));
					}
					return result.ToArray();
				}
				private set {
					instance.ids = Script.NewArray(value);
				}
			}

			public Create(object instance)
				: base(instance)
			{
			}

			/// <summary>
			/// Class for a block creation event.
			/// </summary>
			/// <param name="block">The created block.  Null for a blank event.</param>
			public void construct(Block block)
			{
				instance = Script.New("Blockly.Events.Create", new object[] { block, CREATE });
				if (block == null) {
					return;  // Blank event to be populated by fromJson.
				}
				xml = Blockly.Xml.blockToDomWithXY(block.instance);
				ids = Blockly.Events.getDescendantIds_(block.instance);
			}
		}

		public class Delete : Abstract
		{
			/// <summary>
			/// An XML tree defining the deleted block and any connected child blocks.
			/// </summary>
			[Name(false), FieldProperty]
			internal Element oldXml {
				get { return Element.Create(instance.oldXml); }
				private set { instance.oldXml = value.instance; }
			}

			/// <summary>
			/// An array containing the UUIDs of the deleted block and any connected child blocks.
			/// </summary>
			[Name(false), FieldProperty]
			internal string[] ids {
				get {
					var ret = instance.ids;
					var result = new List<string>();
					var len = Script.Get(ret, "length");
					for (int i = 0; i < len; i++) {
						result.Add(Script.Get(ret, i.ToString()));
					}
					return result.ToArray();
				}
				private set {
					instance.ids = Script.NewArray(value);
				}
			}

			public Delete(object instance)
				: base(instance)
			{
			}

			/// <summary>
			/// Class for a block deletion event.
			/// </summary>
			/// <param name="block">The deleted block.  Null for a blank event.</param>
			public void construct(Block block)
			{
				instance = Script.New("Blockly.Events.Delete", new object[] { block, DELETE });

				if (block == null) {
					return;  // Blank event to be populated by fromJson.
				}
				if (block.getParent() != null) {
					throw new Exception("Connected blocks cannot be deleted.");
				}
				oldXml = Blockly.Xml.blockToDomWithXY(block.instance);
				ids = Blockly.Events.getDescendantIds_(block.instance);
			}
		}

		public class Change : Abstract
		{
			/// <summary>
			/// One of 'field', 'comment', 'collapsed', 'disabled', 'inline', 'mutate'
			/// </summary>
			internal string element { get { return instance.element; } }

			/// <summary>
			/// Name of the field if this is a change to a field.
			/// </summary>
			internal string name { get { return instance.name; } }

			/// <summary>
			/// Original value.
			/// </summary>
			internal object oldValue { get { return instance.oldValue; } }

			/// <summary>
			/// Changed value.
			/// </summary>
			internal object newValue { get { return instance.newValue; } }

			public Change(object instance)
				: base(instance)
			{
			}

			/// <summary>
			/// Class for a block change event.
			/// </summary>
			/// <param name="block">The changed block.  Null for a blank event.</param>
			/// <param name="element">One of 'field', 'comment', 'disabled', etc.</param>
			/// <param name="name">Name of input or field affected, or null.</param>
			/// <param name="oldValue">Previous value of element.</param>
			/// <param name="newValue">New value of element.</param>
			public static Change construct(Block block, string element, string name, string oldValue, string newValue)
			{
				var instance = Script.New("Blockly.Events.Change", new object[] { block.instance, CHANGE });
				if (block == null) {
					return null;  // Blank event to be populated by fromJson.
				}
				instance.element = element;
				instance.name = name;
				instance.oldValue = oldValue;
				instance.newValue = newValue;
				return new Change(instance);
			}
		}

		public class Move : Abstract
		{
			/// <summary>
			/// UUID of old parent block. Undefined if it was a top level block.
			/// </summary>
			internal string oldParentId { get { return instance.oldParentId; } }

			/// <summary>
			/// Name of input on old parent. Undefined if it was a top level block or parent's next block.
			/// </summary>
			internal string oldInputName { get { return instance.oldInputName; } }

			/// <summary>
			/// X and Y coordinates if it was a top level block. Undefined if it had a parent.
			/// </summary>
			internal object oldCoordinate { get { return instance.oldCoordinate; } }

			/// <summary>
			/// UUID of new parent block. Undefined if it is a top level block.
			/// </summary>
			internal string newParentId { get { return instance.newParentId; } }

			/// <summary>
			/// Name of input on new parent. Undefined if it is a top level block or parent's next block.
			/// </summary>
			internal string newInputName { get { return instance.newInputName; } }

			/// <summary>
			/// X and Y coordinates if it is a top level block. Undefined if it has a parent.
			/// </summary>
			internal object newCoordinate { get { return instance.newCoordinate; } }

			public Move(object instance)
				: base(instance)
			{
			}

			/// <summary>
			/// Class for a block move event.  Created before the move.
			/// </summary>
			/// <param name="block">The moved block.  Null for a blank event.</param>
			public void construct(Block block)
			{
				instance = Script.New("Blockly.Events.Move", new object[] { block, MOVE });
				if (block == null) {
					return;  // Blank event to be populated by fromJson.
				}
				var location = this.currentLocation_();
				instance.oldParentId = location.parentId;
				instance.oldInputName = location.inputName;
				instance.oldCoordinate = location.coordinate;
			}

			/**
			 * Record the block's new location.  Called after the move.
			 */
			public void recordNew()
			{
				var location = this.currentLocation_();
				instance.newParentId = location.parentId;
				instance.newInputName = location.inputName;
				instance.newCoordinate = location.coordinate;
			}

			/**
			 * Returns the parentId and input if the block is connected,
			 *   or the XY location if disconnected.
			 * @return {!Object} Collection of location info.
			 * @private
			 */
			private Location currentLocation_()
			{
				return new Location(instance.currentLocation_.call(instance));
			}
		}

		public class Ui : Abstract
		{
			/// <summary>
			/// One of 'selected', 'category', 'click', 'commentOpen', 'mutatorOpen', 'warningOpen'
			/// </summary>
			internal string element { get { return instance.element; } }

			/// <summary>
			/// Original value.
			/// </summary>
			internal object oldValue { get { return instance.oldValue; } }

			/// <summary>
			/// Changed value.
			/// </summary>
			internal object newValue { get { return instance.newValue; } }

			public Ui(object instance)
				: base(instance)
			{
			}

			/// <summary>
			/// Class for a UI event.
			/// </summary>
			/// <param name="block">The affected block.</param>
			/// <param name="element">One of 'selected', 'comment', 'mutator', etc.</param>
			/// <param name="oldValue">Previous value of element.</param>
			/// <param name="newValue">New value of element.</param>
			public void construct(Block block, string element, string oldValue, string newValue)
			{
				instance = Script.New("Blockly.Events.Ui", new object[] { block, UI });
				instance.element = element;
				instance.oldValue = oldValue;
				instance.newValue = newValue;
			}
		}

		internal static void enable()
		{
			BlocklyScript.EventsEnable();
		}

		internal static void disable()
		{
			BlocklyScript.EventsDisable();
		}

		internal static void setGroup(string group)
		{
			BlocklyScript.EventsSetGroup(group);
		}

		internal static void fire(Abstract ev)
		{
			BlocklyScript.EventsFire(ev.instance);
		}
	}

	public class Options
	{
		internal dynamic instance;
		public Node languageTree { get { return new Node(instance.languageTree); } }
		public bool oneBasedIndex { get { return instance.oneBasedIndex; } }
		public Workspace parentWorkspace { get { return WorkspaceSvg.Create(instance.parentWorkspace); } }
		public bool comments { get { if (Script.HasMember(instance, "comments")) return instance.comments; else return false; } }

		public Options(object instance)
		{
			this.instance = instance;
		}
	}

	public class BlockSvg : Block
	{
		[Name(false)]
		public static int TAB_HEIGHT = 20;

		public BlockSvg(string type) : base(type)
		{
		}
	}


	public class Comment : Icon
	{
		public Comment(object instance)
			: base(instance)
		{
			this.instance = instance;
		}

		public new void construct(Block block)
		{
			base.construct(block);
		}
	}

	public class Input
	{
		internal dynamic instance;
		public int type { get { return instance.type; } }
		public string name { get { return instance.name; } }
		public Block block { get { return BlocklyScript.CreateBlock(Script.Get(instance, "block")); } }
		public Connection connection { get { return Connection.Create(instance.connection); } }
		public FieldList fieldRow {
			get {
				return new FieldList(instance.fieldRow);
			}
		}

		public Input(object instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Class for an input with an optional field.
		/// </summary>
		/// <param name="type">The type of the input.</param>
		/// <param name="name">Language-neutral identifier which may used to find this
		/// input again.</param>
		/// <param name="block">The block containing this input.</param>
		/// <param name="connection">Optional connection for this input.</param>
		public void contstract(int type, string name, Block block, Connection connection)
		{
			instance.type = type;
			instance.name = name;
			instance.block = block.instance;
			instance.connection = connection.instance;
		}

		/// <summary>
		/// Add an item to the end of the input's field row.
		/// </summary>
		/// <param name="field">Something to add as a field.</param>
		/// <param name="opt_name">Language-neutral identifier which may used to find
		/// this field again.Should be unique to the host block.</param>
		/// <returns>The input being append to (to allow chaining).</returns>
		public Input appendField(Any<string, Field> field, string opt_name = null)
		{
			var f = field.As<Field>();
			if (f != null)
				return BlocklyScript.CreateInput(instance.appendField.call(instance, f.instance, opt_name));
			else
				return BlocklyScript.CreateInput(instance.appendField.call(instance, field.Value, opt_name));
		}

		/// <summary>
		/// Add an item to the end of the input's field row.
		/// </summary>
		/// <param name="field">Something to add as a field.</param>
		/// <param name="opt_name">Language-neutral identifier which may used to find
		/// this field again.Should be unique to the host block.</param>
		/// <returns>The input being append to (to allow chaining).</returns>
		public Input appendTitle(Any<string, Field> field, string opt_name = null)
		{
			var f = field.As<Field>();
			if (f != null)
				return BlocklyScript.CreateInput(instance.appendTitle.call(instance, f.instance, opt_name));
			else
				return BlocklyScript.CreateInput(instance.appendTitle.call(instance, field.Value, opt_name));
		}

		/// <summary>
		/// Remove a field from this input.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public void removeField(string name)
		{
			instance.removeField.call(instance, name);
		}

		/// <summary>
		/// Gets whether this input is visible or not.
		/// </summary>
		/// <returns>True if visible.</returns>
		public bool isVisible()
		{
			return instance.isVisible.call(instance);
		}

		/// <summary>
		/// Sets whether this input is visible or not.
		/// Used to collapse/uncollapse a block.
		/// </summary>
		/// <param name="visible">True if visible.</param>
		/// <returns>List of blocks to render.</returns>
		public BlockList setVisible(bool visible)
		{
			return new BlockList(instance.setVisible.call(instance, visible));
		}

		/// <summary>
		/// Change a connection's compatibility.
		/// </summary>
		/// <param name="check">Compatible value type or
		/// list of value types.Null if all types are compatible.</param>
		/// <returns>The input being modified (to allow chaining).</returns>
		public Input setCheck(Any<string, string[]> check)
		{
			var chk = check == null ? null : check.As<string[]>();
			if (chk != null)
				return BlocklyScript.CreateInput(instance.setCheck.call(instance, Script.NewArray(check)));
			else
				return BlocklyScript.CreateInput(instance.setCheck.call(instance, (string)check));
		}

		/// <summary>
		/// Change the alignment of the connection's field(s).
		/// </summary>
		/// <param name="align">align One of Blockly.ALIGN_LEFT, ALIGN_CENTRE, ALIGN_RIGHT.
		/// In RTL mode directions are reversed, and ALIGN_RIGHT aligns to the left.</param>
		/// <returns>The input being modified (to allow chaining).</returns>
		public Input setAlign(double align)
		{
			return BlocklyScript.CreateInput(instance.setAlign.call(instance, align));
		}

		/// <summary>
		/// Initialize the fields on this input.
		/// </summary>
		public void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Sever all links to this input.
		/// </summary>
		public void dispose()
		{
			instance.dispose.call(instance);
		}
	}

	public class Icon
	{
		internal dynamic instance;

		public Icon(object instance)
		{
			this.instance = instance;
		}

		public void construct(Block block)
		{
			var val = block == null ? null : block.instance;
			Script.Set(instance, "block", val);
		}
	}

	public class Mutator : Icon
	{
		public Workspace workspace_ { get { return WorkspaceSvg.Create(instance.workspace_); } }
		public int workspaceWidth_ { get { return instance.workspaceWidth_; } }
		public int workspaceHeight_ { get { return instance.workspaceHeight_; } }

		protected Mutator(object instance)
			: base(instance)
		{
		}

		internal static Mutator Create(object mutator)
		{
			if ((mutator == null) || (mutator is DBNull))
				return null;

			var instance = Script.Get(mutator, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Mutator)instance;

			var ret = new Mutator(mutator);
			Script.Set(mutator, "instance", ret);
			return ret;
		}

		public Mutator(string[] quarkNames)
			: base((object)Script.New("Blockly.Mutator", new object[] { Script.NewArray(quarkNames) }))
		{
		}

		/// <summary>
		/// Reconnect an block to a mutated input.
		/// </summary>
		/// <param name="connectionChild">Connection on child block.</param>
		/// <param name="block">Parent block.</param>
		/// <param name="inputName">Name of input on parent block.</param>
		/// <returns>True iff a reconnection was made, false otherwise.</returns>
		internal static bool reconnect(Connection connectionChild, Block block, string inputName)
		{
			return BlocklyScript.MutatorReconnect(connectionChild == null ? null : connectionChild.instance, block.instance, inputName);
		}

		internal bool isVisible()
		{
			var ret = instance.isVisible();
			if ((ret == null) || (ret is DBNull) || !(ret is bool))
				return false;
			return (bool)ret;
		}
	}

	public class Connection
	{
		internal dynamic instance;

		public Connection targetConnection { get { return Connection.Create(instance.targetConnection); } }

		protected Connection(object instance)
		{
			this.instance = instance;
		}

		public static Connection Create(object connection)
		{
			if ((connection == null) || (connection is DBNull))
				return null;

			var instance = Script.Get(connection, "instance");
			if ((instance != null) && !(instance is DBNull))
				return (Connection)instance;

			var ret = new Connection(connection);
			Script.Set(connection, "instance", ret);
			return ret;
		}


		public void construct(Block source, int type)
		{
			instance.source = source.instance;
			instance.type = source.type;
		}

		public void connect(Connection previousConnection)
		{
			instance.connect.call(instance, previousConnection.instance);
		}

		public void disconnect()
		{
			instance.disconnect.call(instance);
		}

		public Block targetBlock()
		{
			return BlocklyScript.CreateBlock(instance.targetBlock.call(instance));
		}

		public bool checkType_(Connection connection)
		{
			return instance.checkType_.call(instance, connection.instance);
		}

		public Connection setCheck(object check)
		{
			return Connection.Create(instance.setCheck.call(instance, check));
		}

		internal Block getSourceBlock()
		{
			var ret = instance.getSourceBlock.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return BlocklyScript.CreateBlock(ret);
		}
	}

	public class RenderedConnection : Connection
	{
		public RenderedConnection(object instance)
			: base(instance)
		{
		}

		public RenderedConnection(Block source, int type)
			: base(null)
		{
			instance = Script.New("Blockly.RenderedConnection", new object[] { source.instance, type });
			construct(source, type);
		}
	}

	public class Field
	{
		internal dynamic instance;

		public string text_ { get { return instance.text_; } }
		public Block sourceBlock_ { get { return BlocklyScript.CreateBlock(instance.sourceBlock_); } }

		public Field(object instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Abstract class for an editable field.
		/// </summary>
		/// <param name="text">The initial content of the field.</param>
		/// <param name="opt_validator">An optional function that is called
		/// to validate any constraints on what the user entered.  Takes the new
		/// text as an argument and returns either the accepted text, a replacement
		/// text, or null to abort the change.</param>
		public Field(string text, Func<string, object> opt_validator = null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.Field", new object[] { text, func });
		}

		/// <summary>
		/// Name of field.  Unique within each block.
		/// Static labels are usually unnamed.
		/// </summary>
		public string name { get { return instance.name; } }

		/// <summary>
		/// Maximum characters of text to display before adding an ellipsis.
		/// </summary>
		public int maxDisplayLength { get { return instance.maxDisplayLength; } }

		/// <summary>
		/// Non-breaking space.
		/// </summary>
		[Name(false)]
		public string NBSP { get { return instance.NBSP; } }

		/// <summary>
		/// Editable fields are saved by the XML renderer, non-editable fields are not.
		/// </summary>
		[Name(false)]
		public bool EDITABLE { get { return instance.EDITABLE; } }

		/// <summary>
		/// Attach this field to a block.
		/// </summary>
		/// <param name="block">The block containing this field.</param>
		public void setSourceBlock(Block block)
		{
			instance.setSourceBlock.call(instance, block.instance);
		}

		/// <summary>
		/// Install this field on a block.
		/// </summary>
		public virtual void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Dispose of all DOM objects belonging to this editable field.
		/// </summary>
		public virtual void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Add or remove the UI indicating if this field is editable or not.
		/// </summary>
		public void updateEditable()
		{
			instance.updateEditable.call(instance);
		}

		/// <summary>
		/// Gets whether this editable field is visible or not.
		/// </summary>
		/// <returns>True if visible.</returns>
		public bool isVisible()
		{
			return instance.isVisible.call(instance);
		}

		/// <summary>
		/// Sets whether this editable field is visible or not.
		/// </summary>
		/// <param name="visible">True if visible.</param>
		public void setVisible(bool visible)
		{
			instance.setVisible.call(instance, visible);
		}

		/// <summary>
		/// Sets a new validation function for editable fields.
		/// </summary>
		/// <param name="handler">New validation function, or null.</param>
		public virtual void setValidator(Func<string, object> handler)
		{
			var func = handler == null ? null : Script.NewFunc(handler);
			instance.setValidator.call(instance, func);
		}

		/// <summary>
		/// Gets the validation function for editable fields.
		/// </summary>
		/// <returns>Validation function, or null.</returns>
		public Func<string, string> getValidator()
		{
			var ret = instance.getValidator.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return (Func<string, string>)ret.handler;
		}

		/// <summary>
		/// Gets the group element for this editable field.
		/// Used for measuring the size and for positioning.
		/// </summary>
		/// <returns>The group element.</returns>
		public virtual Element getSvgRoot()
		{
			var ret = instance.getSvgRoot.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return Element.Create(ret);
		}

		/// <summary>
		/// Start caching field widths.  Every call to this function MUST also call
		/// stopCache.  Caches must not survive between execution threads.
		/// </summary>
		public void startCache()
		{
			instance.startCache.call(instance);
		}

		/// <summary>
		/// Stop caching field widths.  Unless caching was already on when the
		/// corresponding call to startCache was made.
		/// </summary>
		public void stopCache()
		{
			instance.stopCache.call(instance);
		}

		/// <summary>
		/// Returns the height and width of the field.
		/// </summary>
		/// <returns>Height and width.</returns>
		public goog.math.Size getSize()
		{
			var ret = instance.getSize.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return new goog.math.Size(ret);
		}

		/// <summary>
		/// Get the text from this field.
		/// </summary>
		/// <returns>Current text.</returns>
		public virtual string getText()
		{
			var ret = instance.getText.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// Set the text in this field.  Trigger a rerender of the source block.
		/// </summary>
		/// <param name="text">New text.</param>
		public virtual void setText(string text)
		{
			instance.setText.call(instance, text);
		}

		/// <summary>
		/// By default there is no difference between the human-readable text and
		/// the language-neutral values.  Subclasses (such as dropdown) may define this.
		/// </summary>
		/// <returns>Current text.</returns>
		public virtual string getValue()
		{
			var ret = instance.getValue.call(instance);
			if ((ret == null) || (ret is DBNull))
				return null;
			return ret;
		}

		/// <summary>
		/// By default there is no difference between the human-readable text and
		/// the language-neutral values.  Subclasses (such as dropdown) may define this.
		/// </summary>
		/// <param name="newText">New text.</param>
		public virtual void setValue(string newText)
		{
			instance.setValue.call(instance, newText);
		}

		/// <summary>
		/// Change the tooltip text for this field.
		/// </summary>
		/// <param name="newTip">newTip Text for tooltip or a parent element to
		/// link to for its tooltip.</param>
		public virtual void setTooltip(Any<string, Element> newTip)
		{
			var element = newTip.As<Element>();
			if (element != null)
				instance.setTooltip.call(instance, element.instance);
			else
				instance.setTooltip.call(instance, newTip.Value);
		}
	}

	public class FieldLabel : Field
	{
		/// <summary>
		/// Class for a non-editable field.
		/// </summary>
		/// <param name="text">The initial content of the field.</param>
		/// <param name="opt_class">Optional CSS class for the field's text.</param>
		public FieldLabel(string text, string opt_class = null)
			: base(null)
		{
			instance = Script.New("Blockly.FieldLabel", new object[] { text, opt_class });
		}

		/// <summary>
		/// Install this text on a block.
		/// </summary>
		public override void init()
		{
			base.init();
		}

		/// <summary>
		/// Dispose of all DOM objects belonging to this text.
		/// </summary>
		public override void dispose()
		{
			base.dispose();
		}

		/// <summary>
		/// Gets the group element for this field.
		/// Used for measuring the size and for positioning.
		/// </summary>
		/// <returns></returns>
		public override Element getSvgRoot()
		{
			return base.getSvgRoot();
		}

		/// <summary>
		/// Change the tooltip text for this field.
		/// </summary>
		/// <param name="newTip">Text for tooltip or a parent element to
		/// link to for its tooltip.</param>
		public override void setTooltip(Any<string, Element> newTip)
		{
			var element = newTip.As<Element>();
			if (element != null)
				instance.setTooltip.call(instance, element.instance);
			else
				instance.setTooltip.call(instance, newTip.Value);
		}
	}

	public class FieldCheckbox : Field
	{
		/// <summary>
		/// Class for a checkbox field.
		/// </summary>
		/// <param name="state">The initial state of the field ('TRUE' or 'FALSE').</param>
		/// <param name="opt_validator">A function that is executed when a new
		/// option is selected.  Its sole argument is the new checkbox state.  If
		/// it returns a value, this becomes the new checkbox state, unless the
		/// value is null, in which case the change is aborted.</param>
		public FieldCheckbox(string state, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldCheckbox", new object[] { state, func });
		}

		/// <summary>
		/// Character for the checkmark.
		/// </summary>
		public static string CHECK_CHAR = "\u2713";

		/// <summary>
		/// Mouse cursor style when over the hotspot that initiates editability.
		/// </summary>
		public string CURSOR { get { return instance.CURSOR; } }

		/// <summary>
		/// Install this checkbox on a block.
		/// </summary>
		public override void init()
		{
			base.init();
		}

		/// <summary>
		/// Return 'TRUE' if the checkbox is checked, 'FALSE' otherwise.
		/// </summary>
		/// <returns>Current state.</returns>
		public override string getValue()
		{
			return base.getValue();
		}

		/// <summary>
		/// Set the checkbox to be checked if strBool is 'TRUE', unchecks otherwise.
		/// </summary>
		/// <param name="strBool">New state.</param>
		public override void setValue(string strBool)
		{
			base.setValue(strBool);
		}
	}

	public class FieldDropdown : Field
	{
		/// <summary>
		/// Class for an editable dropdown field.
		/// </summary>
		/// <param name="menuGenerator">An array of
		/// options for a dropdown list, or a function which generates these options.</param>
		/// <param name="opt_validator">A function that is executed when a new
		/// option is selected, with the newly selected value as its sole argument.
		/// If it returns a value, that value (which must be one of the options) will
		/// become selected in place of the newly selected option, unless the return
		/// value is null, in which case the change is aborted.
		/// </param>
		public FieldDropdown(Any<string[][], Func<string[][]>> menuGenerator, Func<string, object> opt_validator = null)
			: base(null)
		{
			var gen = menuGenerator.As<Func<string[][]>>() == null ?
				ForScript(menuGenerator.As<string[][]>())
				: Script.NewFunc(new Func<object>(() => {
					return ForScript(menuGenerator.As<Func<string[][]>>()());
				}));
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			if (gen != null)
				instance = Script.New("Blockly.FieldDropdown", new object[] { gen, func });
			else
				instance = Script.New("Blockly.FieldDropdown", new object[] { menuGenerator.Value, func });
		}

		public static dynamic ForScript(string[][] a)
		{
			var b = Script.NewArray();
			foreach (var i in a) {
				var c = Script.NewArray();
				foreach (var j in i) {
					Script.Push(c, j);
				}
				Script.Push(b, c);
			}
			return b;
		}

		/// <summary>
		/// Horizontal distance that a checkmark ovehangs the dropdown.
		/// </summary>
		public static int CHECKMARK_OVERHANG = 25;

		/// <summary>
		/// Android can't (in 2014) display "▾", so use "▼" instead.
		/// </summary>
		public static string ARROW_CHAR = "\u25BE";

		/// <summary>
		/// Mouse cursor style when over the hotspot that initiates the editor.
		/// </summary>
		public string CURSOR { get { return instance.CURSOR; } }

		/// <summary>
		/// Install this dropdown on a block.
		/// </summary>
		public override void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Get the language-neutral value from this dropdown menu.
		/// </summary>
		/// <returns>Current text.</returns>
		public override string getValue()
		{
			return instance.getValue.call(instance);
		}

		/// <summary>
		/// Set the language-neutral value for this dropdown menu.
		/// </summary>
		/// <param name="newText">New value to set.</param>
		public override void setValue(string newText)
		{
			instance.setValue.call(instance, newText);
		}

		/// <summary>
		/// Set the text in this field.  Trigger a rerender of the source block.
		/// </summary>
		/// <param name="text">New text.</param>
		public override void setText(string text)
		{
			instance.setText.call(instance, text);
		}

		/// <summary>
		/// Close the dropdown menu if this input is being deleted.
		/// </summary>
		public override void dispose()
		{
			instance.dispose.call(instance);
		}
	}

	public class FieldTextInput : Field
	{
		public Action<string> onFinishEditing_ {
			get {
				var func = Script.Get(instance, "onFinishEditing_");
				if ((func == null) || (func is DBNull))
					return null;
				var handler = Script.Get(func, "handler");
				if (handler == null) {
					var ret = new Action<string>((str) => {
						func.call(instance, str);
					});
					Script.Set(func, "handler", ret);
				}
				return handler;
			}
			set {
				var func = value == null ? null : Script.NewFunc(value);
				Script.Set(instance, "onFinishEditing_", func);
			}
		}

		/// <summary>
		/// Class for an editable text field.
		/// </summary>
		/// <param name="text">The initial content of the field.</param>
		/// <param name="opt_validator">An optional function that is called
		/// to validate any constraints on what the user entered.  Takes the new
		/// text as an argument and returns either the accepted text, a replacement
		/// text, or null to abort the change.</param>
		public FieldTextInput(string text, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldTextInput", new object[] { text, func });
		}

		/// <summary>
		/// Point size of text.  Should match blocklyText's font-size in CSS.
		/// </summary>
		public static int FONTSIZE = 11;

		/// <summary>
		/// Mouse cursor style when over the hotspot that initiates the editor.
		/// </summary>
		public string CURSOR { get { return instance.CURSOR; } }

		/// <summary>
		/// Close the input widget if this input is being deleted.
		/// </summary>
		public override void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Set the text in this field.
		/// </summary>
		/// <param name="text">New text.</param>
		public override void setValue(string text)
		{
			instance.setValue.call(instance, text);
		}

		/// <summary>
		/// Set whether this field is spellchecked by the browser.
		/// </summary>
		/// <param name="check">True if checked.</param>
		public void setSpellcheck(bool check)
		{
			instance.setSpellcheck.call(instance, check);
		}

		/// <summary>
		/// Ensure that only a number may be entered.
		/// </summary>
		/// <param name="text">The user's text.</param>
		/// <returns>A string representing a valid number, or null if invalid.</returns>
		public virtual string numberValidator(string text)
		{
			return instance.numberValidator.call(instance, text);
		}

		/// <summary>
		/// Ensure that only a nonnegative integer may be entered.
		/// </summary>
		/// <param name="test">The user's text.</param>
		/// <returns>A string representing a valid int, or null if invalid.</returns>
		public string nonnegativeIntegerValidator(string test)
		{
			return instance.nonnegativeIntegerValidator.call(instance, test);
		}
	}

	public class FieldDate : Field
	{
		/// <summary>
		/// Class for a date input field.
		/// </summary>
		/// <param name="date">The initial date.</param>
		/// <param name="opt_validator">A function that is executed when a new
		/// date is selected.  Its sole argument is the new date value.  Its
		/// return value becomes the selected date, unless it is undefined, in
		/// which case the new date stands, or it is null, in which case the change
		/// is aborted.</param>
		public FieldDate(string date, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldDate", new object[] { date, func });
		}

		/// <summary>
		/// Mouse cursor style when over the hotspot that initiates the editor.
		/// </summary>
		public string CURSOR { get { return instance.CURSOR; } }

		/// <summary>
		/// Close the colour picker if this input is being deleted.
		/// </summary>
		public override void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Return the current date.
		/// </summary>
		/// <returns>Current date.</returns>
		public override string getValue()
		{
			return instance.getValue.call(instance);
		}

		/// <summary>
		/// Set the date.
		/// </summary>
		/// <param name="newText">The new date.</param>
		public override void setValue(string newText)
		{
			instance.setValue.call(instance, newText);
		}

		/// <summary>
		/// CSS for date picker.  See css.js for use.
		/// </summary>
		public string[] CSS {
			get {
				var ret = instance.CSS;
				int len = Script.Get(ret, "length");
				var result = new List<string>(len);
				for (int i = 0; i < len; i++) {
					result.Add(Script.Get<string>(ret, i.ToString()));
				}
				return result.ToArray();
			}
		}
	}

	public class FieldColour : Field
	{
		/// <summary>
		/// Class for a colour input field.
		/// </summary>
		/// <param name="colour">The initial colour in '#rrggbb' format.</param>
		/// <param name="opt_validator">A function that is executed when a new
		/// colour is selected.  Its sole argument is the new colour value.  Its
		/// return value becomes the selected colour, unless it is undefined, in
		/// which case the new colour stands, or it is null, in which case the change
		/// is aborted.</param>
		public FieldColour(string colour, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldColour", new object[] { colour, func });
		}

		/// <summary>
		/// Install this field on a block.
		/// </summary>
		public override void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Mouse cursor style when over the hotspot that initiates the editor.
		/// </summary>
		public string CURSOR { get { return instance.CURSOR; } }

		/// <summary>
		/// Close the colour picker if this input is being deleted.
		/// </summary>
		public override void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Return the current colour.
		/// </summary>
		/// <returns>Current colour in '#rrggbb' format.</returns>
		public override string getValue()
		{
			return instance.getValue.call(instance);
		}

		/// <summary>
		/// Set the colour.
		/// </summary>
		/// <param name="colour">The new colour in '#rrggbb' format.</param>
		public override void setValue(string colour)
		{
			instance.setValue.call(instance, colour);
		}

		/// <summary>
		/// Get the text from this field.  Used when the block is collapsed.
		/// </summary>
		/// <returns>Current text.</returns>
		public override string getText()
		{
			return instance.getText.call(instance);
		}

		/// <summary>
		/// An array of colour strings for the palette.
		/// See bottom of this page for the default:
		/// http://docs.closure-library.googlecode.com/git/closure_goog_ui_colorpicker.js.source.html
		/// </summary>
		public static string[] COLOURS = new string[0];

		/// <summary>
		/// Set a custom colour grid for this field.
		/// </summary>
		/// <param name="colours">Array of colours for this block,
		/// or null to use default (Blockly.FieldColour.COLOURS).</param>
		/// <returns>Returns itself (for method chaining).</returns>
		public FieldColour setColours(string[] colours)
		{
			return new FieldColour(instance.setColours.call(instance, colours));
		}

		/// <summary>
		/// Set a custom grid size for this field.
		/// </summary>
		/// <param name="columns">Number of columns for this block,
		/// or 0 to use default (Blockly.FieldColour.COLUMNS).</param>
		/// <returns>Returns itself (for method chaining).</returns>
		public FieldColour setColumns(int columns)
		{
			return new FieldColour(instance.setColumns.call(instance, columns));
		}
	}

	public class FieldNumber : FieldTextInput
	{
		/// <summary>
		/// Class for an editable number field.
		/// </summary>
		/// <param name="value">The initial content of the field.</param>
		/// <param name="opt_min">Minimum value.</param>
		/// <param name="opt_max">Maximum value.</param>
		/// <param name="opt_precision">Precision for value.</param>
		/// <param name="opt_validator">An optional function that is called
		/// to validate any constraints on what the user entered.  Takes the new
		/// text as an argument and returns either the accepted text, a replacement
		/// text, or null to abort the change.</param>
		public FieldNumber(string value, Any<string, int> opt_min,
			Any<string, int> opt_max, Any<string, int> opt_precision,
			Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldNumber", new object[] { value, opt_min.ToString(), opt_max.ToString(), opt_precision.ToString(), func });
		}

		/// <summary>
		/// Set the maximum, minimum and precision constraints on this field.
		/// Any of these properties may be undefiend or NaN to be disabled.
		/// Setting precision (usually a power of 10) enforces a minimum step between
		/// values. That is, the user's value will rounded to the closest multiple of
		/// precision. The least significant digit place is inferred from the precision.
		/// Integers values can be enforces by choosing an integer precision.
		/// </summary>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="precision">Precision for value.</param>
		public void setConstraints(Any<string, int> min, Any<string, int> max,
			Any<string, int> precision)
		{
			instance.setConstraints.call(instance, min.ToString(), max.ToString(), precision.ToString());
		}

		/// <summary>
		/// Sets a new change handler for number field.
		/// </summary>
		/// <param name="handler">New change handler, or null.</param>
		public override void setValidator(Func<string, object> handler)
		{
			instance.setValidator.call(instance, handler);
		}

		/// <summary>
		/// Ensure that only a number in the correct range may be entered.
		/// </summary>
		/// <param name="text">The user's text.</param>
		/// <returns>A string representing a valid number, or null if invalid.</returns>
		public override string numberValidator(string text)
		{
			return instance.numberValidator.call(instance, text);
		}
	}

	public class FieldAngle : FieldTextInput
	{
		/// <summary>
		/// Class for an editable angle field.
		/// </summary>
		/// <param name="text">The initial content of the field.</param>
		/// <param name="opt_validator">An optional function that is called
		/// to validate any constraints on what the user entered.  Takes the new
		/// text as an argument and returns the accepted text or null to abort
		/// the change.</param>
		public FieldAngle(string text, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldAngle", new object[] { text, func });
		}

		/// <summary>
		/// Sets a new change handler for angle field.
		/// </summary>
		/// <param name="handler">New change handler, or null.</param>
		public override void setValidator(Func<string, object> handler)
		{
			instance.setValidator.call(instance, handler);
		}

		/// <summary>
		/// Round angles to the nearest 15 degrees when using mouse.
		/// Set to 0 to disable rounding.
		/// </summary>
		public static int ROUND = 15;

		/// <summary>
		/// Half the width of protractor image.
		/// </summary>
		public static int HALF = 100 / 2;

		/// <summary>
		/// Angle increases clockwise (true) or counterclockwise (false).
		/// </summary>
		public static bool CLOCKWISE = false;

		/// <summary>
		/// Offset the location of 0 degrees (and all angles) by a constant.
		/// Usually either 0 (0 = right) or 90 (0 = up).
		/// </summary>
		public static int OFFSET = 0;

		/// <summary>
		/// Maximum allowed angle before wrapping.
		/// Usually either 360 (for 0 to 359.9) or 180 (for -179.9 to 180).
		/// </summary>
		public static int WRAP = 360;

		/// <summary>
		/// Radius of protractor circle.  Slightly smaller than protractor size since
		/// otherwise SVG crops off half the border at the edges.
		/// </summary>
		public static int RADIUS = HALF - 1;

		/// <summary>
		/// Set the angle to match the mouse's position.
		/// </summary>
		/// <param name="e">Mouse move event.</param>
		public void onMouseMove(Event e)
		{
			instance.onMouseMove.call(instance, e.instance);
		}

		/// <summary>
		/// Insert a degree symbol.
		/// </summary>
		/// <param name="text">New text.</param>
		public override void setText(string text)
		{
			instance.setText.call(instance, text);
		}

		/// <summary>
		/// Ensure that only an angle may be entered.
		/// </summary>
		/// <param name="text">The user's text.</param>
		/// <returns>A string representing a valid angle, or null if invalid.</returns>
		public string angleValidator(string text)
		{
			return instance.angleValidator.call(instance, text);
		}
	}

	public class FieldImage : Field
	{
		/// <summary>
		/// Class for an image.
		/// </summary>
		/// <param name="src">The URL of the image.</param>
		/// <param name="width">Width of the image.</param>
		/// <param name="height">Height of the image.</param>
		/// <param name="opt_alt">Optional alt text for when block is collapsed.</param>
		public FieldImage(string src, int width, int height, string opt_alt = "")
			: base(null)
		{
			instance = Script.New("Blockly.FieldImage", new object[] { src, width, height, opt_alt });
		}

		/// <summary>
		/// Install this image on a block.
		/// </summary>
		public override void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Dispose of all DOM objects belonging to this text.
		/// </summary>
		public override void dispose()
		{
			instance.dispose.call(instance);
		}

		/// <summary>
		/// Change the tooltip text for this field.
		/// </summary>
		/// <param name="newTip">Text for tooltip or a parent element to
		/// link to for its tooltip.</param>
		public override void setTooltip(Any<string, Element> newTip)
		{
			var element = newTip.As<Element>();
			if (element != null)
				instance.setTooltip.call(instance, element.instance);
			else
				instance.setTooltip.call(instance, newTip.Value);
		}

		/// <summary>
		/// Get the source URL of this image.
		/// </summary>
		/// <returns>Current text.</returns>
		public override string getValue()
		{
			return instance.getValue.call(instance);
		}

		/// <summary>
		/// Set the source URL of this image.
		/// </summary>
		/// <param name="newText">New source.</param>
		public override void setValue(string newText)
		{
			instance.setValue.call(instance, newText);
		}

		/// <summary>
		/// Set the alt text of this image.
		/// </summary>
		/// <param name="alt">New alt text.</param>
		public override void setText(string alt)
		{
			instance.setText.call(instance, alt);
		}
	}

	public class FieldVariable : Field
	{
		/// <summary>
		/// Class for a variable's dropdown field.
		/// </summary>
		/// <param name="varname">The default name for the variable.  If null,
		/// a unique variable name will be generated.</param>
		/// <param name="opt_validator">A function that is executed when a new
		/// option is selected.  Its sole argument is the new option value.</param>
		public FieldVariable(string varname, Func<string, object> opt_validator = null)
			: base(null)
		{
			var func = opt_validator == null ? null : Script.NewFunc(opt_validator);
			instance = Script.New("Blockly.FieldVariable", new object[] { varname, func });
		}

		/// <summary>
		/// Sets a new change handler for angle field.
		/// </summary>
		/// <param name="handler">New change handler, or null.</param>
		public override void setValidator(Func<string, object> handler)
		{
			instance.setValidator.call(instance, handler);
		}

		/// <summary>
		/// Install this dropdown on a block.
		/// </summary>
		public override void init()
		{
			instance.init.call(instance);
		}

		/// <summary>
		/// Get the variable's name (use a variableDB to convert into a real name).
		/// Unline a regular dropdown, variables are literal and have no neutral value.
		/// </summary>
		/// <returns>Current text.</returns>
		public override string getValue()
		{
			return instance.getValue.call(instance);
		}

		/// <summary>
		/// Set the variable name.
		/// </summary>
		/// <param name="newValue">New text.</param>
		public override void setValue(string newValue)
		{
			instance.setValue.call(instance, newValue);
		}

		/// <summary>
		/// Return a sorted list of variable names for variable dropdown menus.
		/// Include a special option at the end for creating a new variable name.
		/// </summary>
		/// <returns>Array of variable names.</returns>
		public string[] dropdownCreate()
		{
			var ret = instance.dropdownCreate.call(instance);
			int len = Script.Get(ret, "length");
			var result = new List<string>(len);
			for (int i = 0; i < len; i++) {
				result.Add(Script.Get<string>(ret, i.ToString()));
			}
			return result.ToArray();
		}

		/// <summary>
		/// Event handler for a change in variable name.
		/// Special case the 'New variable...' and 'Rename variable...' options.
		/// In both of these special cases, prompt the user for a new name.
		/// </summary>
		/// <param name="text">The selected dropdown menu option.</param>
		/// <returns>An acceptable new variable name, or null if
		/// change is to be either aborted (cancel button) or has been already
		/// handled (rename), or undefined if an existing variable was chosen.</returns>
		public string dropdownChange(string text)
		{
			return instance.dropdownChange.call(instance, text);
		}
	}

	/// <summary>
	/// Register a callback function associated with a given key, for clicks on
	/// buttons and labels in the flyout.
	/// For instance, a button specified by the XML
	/// <button text="create variable" callbackKey="CREATE_VARIABLE"></button>
	/// should be matched by a call to
	/// registerButtonCallback("CREATE_VARIABLE", yourCallbackFunction).
	/// </summary>
	/// <param name="key">The name to use to look up this function.</param>
	/// <param name="func">The function to call when the
	/// given button is clicked.</param>
	internal static void registerButtonCallback(string key, Action<FlyoutButton> func)
	{
		instance.registerButtonCallback.call(instance, key, Script.NewFunc(new Action<object>((btn) => {
			func(FlyoutButton.Create(btn));
		})));
	}

	/// <summary>
	/// Convert a hue (HSV model) into an RGB hex triplet.
	/// </summary>
	/// <param name="hue">Hue on a colour wheel (0-360).</param>
	/// <returns>RGB code, e.g. '#5ba65b'.</returns>
	public static string hueToRgb(int hue)
	{
		return instance.hueToRgb.call(instance, hue);
	}

	/// <summary>
	/// Returns the dimensions of the specified SVG image.
	/// </summary>
	/// <param name="svg">SVG image.</param>
	/// <returns>Contains width and height properties.</returns>
	public static goog.math.Size svgSize(Element svg)
	{
		return instance.svgSize.call(instance, svg.instance);
	}

	/// <summary>
	/// Size the workspace when the contents change.  This also updates
	/// scrollbars accordingly.
	/// </summary>
	/// <param name="workspace"></param>
	public static void resizeSvgContents(WorkspaceSvg workspace)
	{
		instance.resizeSvgContents.call(instance, workspace.instance);
	}

	/// <summary>
	/// Size the SVG image to completely fill its container. Call this when the view
	/// actually changes sizes (e.g. on a window resize/device orientation change).
	/// See Blockly.resizeSvgContents to resize the workspace when the contents
	/// change (e.g. when a block is added or removed).
	/// Record the height/width of the SVG image.
	/// </summary>
	/// <param name="workspace">Any workspace in the SVG.</param>
	public static void svgResize(WorkspaceSvg workspace)
	{
		instance.svgResize.call(instance, workspace.instance);
	}

	/// <summary>
	/// Close tooltips, context menus, dropdown selections, etc.
	/// </summary>
	/// <param name="opt_allowToolbox">If true, don't close the toolbox.</param>
	public static void hideChaff(bool opt_allowToolbox = false)
	{
		instance.hideChaff.call(instance, opt_allowToolbox);
	}

	/// <summary>
	/// When something in Blockly's workspace changes, call a function.
	/// </summary>
	/// <param name="func">Function to call.</param>
	/// <returns>Opaque data that can be passed to removeChangeListener.</returns>
	public static Action<Events.Abstract> addChangeListener(Action<Events.Abstract> func)
	{
		// Backwards compatability from before there could be multiple workspaces.
		Console.WriteLine("Deprecated call to Blockly.addChangeListener, " +
					 "use workspace.addChangeListener instead.");
		return getMainWorkspace().addChangeListener(func);
	}

	/// <summary>
	/// Returns the main workspace.  Returns the last used main workspace (based on
	/// focus).  Try not to use this function, particularly if there are multiple
	/// Blockly instances on a page.
	/// </summary>
	/// <returns>The main workspace.</returns>
	public static WorkspaceSvg getMainWorkspace()
	{
		return mainWorkspace;
	}

	/// <summary>
	/// Wrapper to window.alert() that app developers may override to
	/// provide alternatives to the modal browser window.
	/// </summary>
	/// <param name="message">The message to display to the user.</param>
	/// <param name="opt_callback">The callback when the alert is dismissed.</param>
	internal static void alert(string message, Action opt_callback = null)
	{
		instance.alert.call(instance, message, opt_callback == null ? null : Script.NewFunc(opt_callback));
	}

	/// <summary>
	/// Wrapper to window.prompt() that app developers may override to provide
	/// alternatives to the modal browser window. Built-in browser prompts are
	/// often used for better text input experience on mobile device. We strongly
	/// recommend testing mobile when overriding this.
	/// </summary>
	/// <param name="message">The message to display to the user.</param>
	/// <param name="defaultValue">The value to initialize the prompt with.</param>
	/// <param name="callback">The callback for handling user reponse.</param>
	internal static void prompt(string message, string defaultValue, Action<string> callback)
	{
		instance.prompt.call(instance, message, defaultValue, Script.NewFunc(callback));
	}

	/// <summary>
	/// Inject a Blockly editor into the specified container element (usually a div).
	/// </summary>
	/// <param name="container">Containing element, or its ID, or a CSS selector.</param>
	/// <param name="opt_options">opt_options Optional dictionary of options.</param>
	/// <returns>Newly created main workspace.</returns>
	public static WorkspaceSvg inject(Any<string, Element> container, object opt_options = null)
	{
		var element = container.As<Element>();
		if (element != null)
			return WorkspaceSvg.Create(instance.inject.call(instance, element.instance, opt_options));
		else
			return WorkspaceSvg.Create(instance.inject.call(instance, container.Value, opt_options));
	}

	/// <summary>
	/// Modify the block tree on the existing toolbox.
	/// </summary>
	/// <param name="tree">DOM tree of blocks, or text representation of same.</param>
	public static void updateToolbox(Any<Node, string> tree)
	{
		var node = tree.As<Node>();
		if (node != null)
			instance.updateToolbox.call(instance, node.instance);
		else
			instance.updateToolbox.call(instance, tree.Value);
	}

	/// <summary>
	/// Don't do anything for this event, just halt propagation.
	/// </summary>
	/// <param name="e">An event.</param>
	public static void noEvent(Event e)
	{
		// This event has been handled.  No need to bubble up to the document.
		e.PreventDefault();
		e.StopPropagation();
	}

	/// <summary>
	/// Helper method for creating SVG elements.
	/// </summary>
	/// <param name="name">Element's tag name.</param>
	/// <param name="attrs">Dictionary of attribute names and values.</param>
	/// <param name="parent">Optional parent on which to append the element.</param>
	/// <param name="opt_workspace">Optional workspace for access to context (scale...).</param>
	/// <returns>Newly created SVG element.</returns>
	public static Element createSvgElement(string name, object attrs, Element parent, Blockly.Workspace opt_workspace)
	{
		return instance.createSvgElement.call(instance, name, attrs, parent, opt_workspace);
	}

	/// <summary>
	/// Is this event a right-click?
	/// </summary>
	/// <param name="e">Mouse event.</param>
	/// <returns>True if right-click.</returns>
	public static bool isRightButton(Event e)
	{
		return instance.isRightButton.call(instance, e.instance);
	}

	/// <summary>
	/// Return the converted coordinates of the given mouse event.
	/// The origin (0,0) is the top-left corner of the Blockly svg.
	/// </summary>
	/// <param name="e">Mouse event.</param>
	/// <param name="svg">SVG element.</param>
	/// <param name="matrix">Inverted screen CTM to use.</param>
	/// <returns>Object with .x and .y properties.</returns>
	public static goog.math.Coordinate mouseToSvg(Event e, Element svg, SVGMatrix matrix)
	{
		return instance.mouseToSvg.call(instance, e.instance, svg.instance, matrix.instance);
	}

	/// <summary>
	/// Given an array of strings, return the length of the shortest one.
	/// </summary>
	/// <param name="array">Array of strings.</param>
	/// <returns>Length of shortest string.</returns>
	public static int shortestStringLength(string[] array)
	{
		return instance.shortestStringLength.call(instance, array);
	}

	/// <summary>
	/// Given an array of strings, return the length of the common prefix.
	/// Words may not be split.  Any space after a word is included in the length.
	/// </summary>
	/// <param name="array">Array of strings.</param>
	/// <param name="opt_shortest">Length of shortest string.</param>
	/// <returns>Length of common prefix.</returns>
	public static int commonWordPrefix(string[] array, int opt_shortest = 0)
	{
		return instance.commonWordPrefix.call(instance, Script.NewArray(array), opt_shortest);
	}

	/// <summary>
	/// Given an array of strings, return the length of the common suffix.
	/// Words may not be split.  Any space after a word is included in the length.
	/// </summary>
	/// <param name="array">Array of strings.</param>
	/// <param name="opt_shortest">Length of shortest string.</param>
	/// <returns>Length of common suffix.</returns>
	public static int commonWordSuffix(string[] array, int opt_shortest = 0)
	{
		return instance.commonWordSuffix.call(instance, Script.NewArray(array), opt_shortest);
	}

	/// <summary>
	/// Is the given string a number (includes negative and decimals).
	/// </summary>
	/// <param name="str">Input string.</param>
	/// <returns>True if number, false otherwise.</returns>
	public static bool isNumber(string str)
	{
		return instance.isNumber.call(instance, str);
	}

	/// <summary>
	/// Parse a string with any number of interpolation tokens (%1, %2, ...).
	/// '%' characters may be self-escaped (%%).
	/// </summary>
	/// <param name="message">Text containing interpolation tokens.</param>
	/// <returns>Array of strings and numbers.</returns>
	public static Any<string, int> tokenizeInterpolation(string message)
	{
		return instance.tokenizeInterpolation.call(instance, message);
	}

	/// <summary>
	/// Generate a unique ID.  This should be globally unique.
	/// 87 characters ^ 20 length > 128 bits (better than a UUID).
	/// </summary>
	/// <returns>A globally unique ID string.</returns>
	public static string genUid()
	{
		return instance.genUid.call(instance);
	}

	/// <summary>
	/// Wrap text to the specified width.
	/// </summary>
	/// <param name="text">Text to wrap.</param>
	/// <param name="wrap">Width to wrap each line.</param>
	/// <returns></returns>
	public static string wrap(string text, int wrap)
	{
		return instance.wrap.call(instance, text, wrap);
	}

	public class Rectangle
	{
		public goog.math.Coordinate topLeft;
		public goog.math.Coordinate bottomRight;

		public Rectangle(dynamic comobj)
		{
			this.topLeft = new goog.math.Coordinate(comobj.topLeft);
			this.bottomRight = new goog.math.Coordinate(comobj.bottomRight);
		}
	}

	public static class ContextMenu
	{
		public static dynamic callbackFactory(Block block, Element xml)
		{
			return BlocklyScript.ContextMenuCallbackFactory(block.instance, xml.instance);
		}
	}

	internal class FlyoutButton
	{
		internal dynamic instance;

		public FlyoutButton(object instance)
		{
			this.instance = instance;
		}

		public static FlyoutButton Create(dynamic instance)
		{
			if ((instance == null) || (instance is DBNull))
				return null;
			return new FlyoutButton(instance);
		}

		internal Workspace getTargetWorkspace()
		{
			return WorkspaceSvg.Create(instance.getTargetWorkspace.call(instance));
		}
	}
}

/// <summary>
/// Allow for switching between one and zero based indexing for lists and text,
/// one based by default.
/// </summary>
[External]
public class Blocks
{
	public static bool ONE_BASED_INDEXING = true;
}

/// <summary>
/// Google's common JavaScript library
/// https://developers.google.com/closure/library/
/// </summary>
[External]
public static class goog
{
	public static string getMsg(string str, object opt_values)
	{
		return BlocklyScript.goog_getMsg(str, opt_values);
	}

	public static class dom
	{
		public static Element createDom(string v, object o = null, Any<string, Element> t = null)
		{
			var element = (t == null) ? null : t.As<Element>();
			if (element != null)
				return Element.Create(BlocklyScript.goog_dom_createDom(v, o, element));
			else
				return Element.Create(BlocklyScript.goog_dom_createDom(v, o, (string)t));
		}
	}

	public static class math
	{
		[ComVisible(true)]
		public class Size
		{
			public double width;
			public double height;

			public Size(double width, double height)
			{
				this.width = width;
				this.height = height;
			}

			public Size(dynamic comobj)
			{
				width = Script.Get<double>(comobj, "width");
				height = Script.Get<double>(comobj, "height");
			}
		}

		[ComVisible(true)]
		public class Coordinate
		{
			public double x;
			public double y;

			public Coordinate(dynamic comobj)
			{
				x = Script.Get<double>(comobj, "x");
				y = Script.Get<double>(comobj, "y");
			}
		}
	}

	public static class @string
	{
		public class CaseInsensitiveCompare : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				return x.ToLower().CompareTo(y.ToLower());
			}
		}

		public static CaseInsensitiveCompare caseInsensitiveCompare = new CaseInsensitiveCompare();
	}

	public static class array
	{
		internal static bool equals(Array a, Array b)
		{
			return BlocklyScript.goog_array_equals(a, b);
		}
	}

	public static class asserts
	{
		internal static void assert(bool cond, string format, params string[] args)
		{
			if (!cond) throw new NotImplementedException();
		}

		internal static void assertArray(object array, string format, params string[] args)
		{
			assert(array is Array, format, args);
		}

		internal static void assertFunction(object func, string format, params string[] args)
		{
			assert(func is System.Reflection.MethodInfo, format, args);
		}

		internal static void assertString(object str, string format, params string[] args)
		{
			assert(str is String, format, args);
		}

		internal static void fail(string format, params string[] args)
		{
			throw new NotImplementedException();
		}
	}
}
