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
 * @fileoverview Utility functions for handling procedures.
 * @author fraser@google.com (Neil Fraser)
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bridge;
using Bridge.Html5;
using Bridge.Text.RegularExpressions;
using BlocklyMruby;

public static partial class Blockly
{
	public static class Procedures
	{
		/// <summary>
		/// Category to separate procedure names from variables and generated functions.
		/// </summary>
		public static string NAME_TYPE = "PROCEDURE";

		/// <summary>
		/// Find all user-created procedure definitions in a workspace.
		/// </summary>
		/// <param name="root">Root workspace.</param>
		/// <returns>Pair of arrays, the first contains procedures without return variables, the
		/// second with. Each procedure is defined by a three-element list of name, parameter
		/// list, and return value boolean.</returns>
		public static object[] allProcedures(Blockly.Workspace root)
		{
			var blocks = root.getAllBlocks();
			var proceduresReturn = new List<object[]>();
			var proceduresNoReturn = new List<object[]>();
			for (var i = 0; i < blocks.Length; i++) {
				var block = blocks[i] as ProcedureDefBlock;
				if (block != null) {
					var tuple = block.getProcedureDef();
					if (tuple != null) {
						if ((bool)tuple[2]) {
							proceduresReturn.Add(tuple);
						}
						else {
							proceduresNoReturn.Add(tuple);
						}
					}
				}
			}
			proceduresNoReturn.Sort(procTupleComparator_);
			proceduresReturn.Sort(procTupleComparator_);
			return new object[] { proceduresNoReturn.ToArray(), proceduresReturn.ToArray() };
		}

		/// <summary>
		/// Comparison function for case-insensitive sorting of the first element of
		/// a tuple.
		/// </summary>
		/// <param name="ta">First tuple.</param>
		/// <param name="tb">Second tuple.</param>
		/// <returns>-1, 0, or 1 to signify greater than, equality, or less than.</returns>
		public static int procTupleComparator_(object[] ta, object[] tb)
		{
			return ((string)ta[0]).ToLowerCase().LocaleCompare(((string)tb[0]).ToLowerCase());
		}

		/// <summary>
		/// Ensure two identically-named procedures don't exist.
		/// </summary>
		/// <param name="name">Proposed procedure name.</param>
		/// <param name="block">Block to disambiguate.</param>
		/// <returns>Non-colliding name.</returns>
		public static string findLegalName(string name, Block block)
		{
			if (block.isInFlyout) {
				// Flyouts can have multiple procedures called 'do something'.
				return name;
			}
			while (!isLegalName_(name, block.workspace, block)) {
				// Collision with another procedure.
				var r = name.Match(new Regex(@"^(.*?)(\d+)$"));
				if (r == null) {
					name += "2";
				}
				else {
					name = r[1] + (Script.ParseInt(r[2], 10) + 1);
				}
			}
			return name;
		}

		/// <summary>
		/// Does this procedure have a legal name?  Illegal names include names of
		/// procedures already defined.
		/// </summary>
		/// <param name="name">The questionable name.</param>
		/// <param name="workspace">The workspace to scan for collisions.</param>
		/// <param name="opt_exclude">Optional block to exclude from
		/// comparisons (one doesn't want to collide with oneself).</param>
		/// <returns>True if the name is legal.</returns>
		public static bool isLegalName_(string name, Blockly.Workspace workspace, Block opt_exclude = null)
		{
			var blocks = workspace.getAllBlocks();
			// Iterate through every block and check the name.
			for (var i = 0; i < blocks.Length; i++) {
				if (blocks[i] == opt_exclude) {
					continue;
				}
				var block = blocks[i] as ProcedureDefBlock;
				if (block != null) {
					var procName = block.getProcedureDef();
					if (Names.equals((string)procName[0], name)) {
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Rename a procedure.  Called by the editable field.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="name">The proposed new name.</param>
		/// <returns>The accepted name.</returns>
		public static string rename(Blockly.Field field, string name)
		{
			// Strip leading and trailing whitespace.  Beyond this, all names are legal.
			name = name.Replace(new Regex(@"^[\s\xa0] +|[\s\xa0] +$", "g"), "");

			// Ensure two identically-named procedures don't exist.
			var legalName = Procedures.findLegalName(name, field.sourceBlock_);
			var oldName = field.text_;
			if (oldName != name && oldName != legalName) {
				// Rename any callers.
				var blocks = field.sourceBlock_.workspace.getAllBlocks();
				for (var i = 0; i < blocks.Length; i++) {
					var block = blocks[i] as ProcedureCallBlock;
					if (block != null) {
						block.renameProcedure(oldName, legalName);
					}
				}
			}
			return legalName;
		}

		/// <summary>
		/// Construct the blocks required by the flyout for the procedure category.
		/// </summary>
		/// <param name="workspace">The workspace contianing procedures.</param>
		/// <returns>Array of XML block elements.</returns>
		public static Element[] flyoutCategory(Blockly.Workspace workspace)
		{
			var xmlList = new List<Element>();
			if (Script.Get(Blockly.Blocks, "procedures_defnoreturn") != null) {
				// <block type="procedures_defnoreturn" gap="16"></block>
				var block = goog.dom.createDom("block");
				block.SetAttribute("type", "procedures_defnoreturn");
				block.SetAttribute("gap", "16");
				xmlList.Add(block);
			}
			if (Script.Get(Blockly.Blocks, "procedures_defreturn") != null) {
				// <block type="procedures_defreturn" gap="16"></block>
				var block = goog.dom.createDom("block");
				block.SetAttribute("type", "procedures_defreturn");
				block.SetAttribute("gap", "16");
				xmlList.Add(block);
			}
			if (Script.Get(Blockly.Blocks, "procedures_ifreturn") != null) {
				// <block type="procedures_ifreturn" gap="16"></block>
				var block = goog.dom.createDom("block");
				block.SetAttribute("type", "procedures_ifreturn");
				block.SetAttribute("gap", "16");
				xmlList.Add(block);
			}
			if (xmlList.Count != 0) {
				// Add slightly larger gap between system blocks and user calls.
				xmlList[xmlList.Count - 1].SetAttribute("gap", "24");
			}

			var populateProcedures = new Action<object[][], string>((procedureList, templateName) => {
				for (var i = 0; i < procedureList.Length; i++) {
					var name = (string)procedureList[i][0];
					var args = (string[])procedureList[i][1];
					// <block type="procedures_callnoreturn" gap="16">
					//   <mutation name="do something">
					//     <arg name="x"></arg>
					//   </mutation>
					// </block>
					var block = goog.dom.createDom("block");
					block.SetAttribute("type", templateName);
					block.SetAttribute("gap", "16");
					var mutation = goog.dom.createDom("mutation");
					mutation.SetAttribute("name", name);
					block.AppendChild(mutation);
					for (var j = 0; j < args.Length; j++) {
						var arg = goog.dom.createDom("arg");
						arg.SetAttribute("name", args[j]);
						mutation.AppendChild(arg);
					}
					xmlList.Add(block);
				}
			});

			var tuple = Procedures.allProcedures(workspace);
			populateProcedures((object[][])tuple[0], "procedures_callnoreturn");
			populateProcedures((object[][])tuple[1], "procedures_callreturn");
			return xmlList.ToArray();
		}

		/// <summary>
		/// Find all the callers of a named procedure.
		/// </summary>
		/// <param name="name">Name of procedure.</param>
		/// <param name="workspace">The workspace to find callers in.</param>
		/// <returns>Array of caller blocks.</returns>
		public static ProcedureCallBlock[] getCallers(string name, Blockly.Workspace workspace)
		{
			var callers = new List<ProcedureCallBlock>();
			var blocks = workspace.getAllBlocks();
			// Iterate through every block and check the name.
			for (var i = 0; i < blocks.Length; i++) {
				var block = blocks[i] as ProcedureCallBlock;
				if (block != null) {
					var procName = block.getProcedureCall();
					// Procedure name may be null if the block is only half-built.
					if (procName != null && Names.equals(procName, name)) {
						callers.Add(block);
					}
				}
			}
			return callers.ToArray();
		}

		/// <summary>
		/// When a procedure definition changes its parameters, find and edit all its
		/// callers.
		/// </summary>
		/// <param name="defBlock">Procedure definition block.</param>
		public static void mutateCallers(ProcedureDefBlock defBlock)
		{
			var oldRecordUndo = Blockly.Events.recordUndo;
			var name = (string)(defBlock.getProcedureDef())[0];
			var xmlElement = defBlock.mutationToDom(true);
			var callers = Procedures.getCallers(name, defBlock.workspace);
			foreach (var caller in callers) {
				var oldMutationDom = caller.mutationToDom();
				var oldMutation = oldMutationDom != null ? Blockly.Xml.domToText(oldMutationDom) : null;
				caller.domToMutation(xmlElement);
				var newMutationDom = caller.mutationToDom();
				var newMutation = newMutationDom != null ? Blockly.Xml.domToText(newMutationDom) : null;
				if (oldMutation != newMutation) {
					// Fire a mutation on every caller block.  But don't record this as an
					// undo action since it is deterministically tied to the procedure's
					// definition mutation.
					Blockly.Events.recordUndo = false;
					Blockly.Events.fire(Blockly.Events.Change.construct(
						caller, "mutation", null, oldMutation, newMutation));
					Blockly.Events.recordUndo = oldRecordUndo;
				}
			}
		}

		/// <summary>
		/// Find the definition block for the named procedure.
		/// </summary>
		/// <param name="name">Name of procedure.</param>
		/// <param name="workspace">The workspace to search.</param>
		/// <returns>The procedure definition block, or null not found.</returns>
		public static Block getDefinition(string name, Blockly.Workspace workspace)
		{
			// Assume that a procedure definition is a top block.
			var blocks = workspace.getTopBlocks(false);
			for (var i = 0; i < blocks.Length; i++) {
				var block = blocks[i] as ProcedureDefBlock;
				if (block != null) {
					var tuple = block.getProcedureDef();
					if (tuple != null && Names.equals((string)tuple[0], name)) {
						return block;
					}
				}
			}
			return null;
		}
	}
}