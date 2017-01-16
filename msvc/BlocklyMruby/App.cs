using System;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace BlocklyMruby
{
	static class App
	{
		public static TerminalHost Term;
		public static bool changed = true;
		public static BlocklyScript Script { get; private set; }
		public static Document Document { get; private set; }
		public static Blockly Blockly { get; private set; }

		internal static void Init(Blockly Blockly)
		{
			App.Blockly = Blockly;
			Script = Blockly.Script;
			Document = Script.Document;
			Script.SetMessage(typeof(Msg));
			Script.SetBlocks(new ColourPickerBlock(Blockly));
			Script.SetBlocks(new ColourRandomBlock(Blockly));
			Script.SetBlocks(new ColourRGBBlock(Blockly));
			Script.SetBlocks(new ColourBlendBlock(Blockly));
			Script.SetBlocks(new ListsCreateEmptyBlock(Blockly));
			Script.SetBlocks(new ListsCreateWithBlock(Blockly));
			Script.SetBlocks(new ListsCreateWithContainerBlock(Blockly));
			Script.SetBlocks(new ListsCreateWithItemBlock(Blockly));
			Script.SetBlocks(new ListsRepeatBlock(Blockly));
			Script.SetBlocks(new ListsLengthBlock(Blockly));
			Script.SetBlocks(new ListsIsEmptyBlock(Blockly));
			Script.SetBlocks(new ListsIndexOfBlock(Blockly));
			Script.SetBlocks(new ListsGetIndexBlock(Blockly));
			Script.SetBlocks(new ListsSetIndexBlock(Blockly));
			Script.SetBlocks(new ListsGetSublistBlock(Blockly));
			Script.SetBlocks(new ListsSortBlock(Blockly));
			Script.SetBlocks(new ListsSplitBlock(Blockly));
			Script.SetBlocks(new ControlsIfBlock(Blockly));
			Script.SetBlocks(new ControlsIfIfBlock(Blockly));
			Script.SetBlocks(new ControlsIfElseIfBlock(Blockly));
			Script.SetBlocks(new ControlsIfElseBlock(Blockly));
			Script.SetBlocks(new SwitchCaseNumberBlock(Blockly));
			Script.SetBlocks(new SwitchCaseNumberContainerBlock(Blockly));
			Script.SetBlocks(new SwitchCaseNumberConstBlock(Blockly));
			Script.SetBlocks(new SwitchCaseNumberRangeBlock(Blockly));
			Script.SetBlocks(new SwitchCaseNumberDefaultBlock(Blockly));
			Script.SetBlocks(new SwitchCaseTextBlock(Blockly));
			Script.SetBlocks(new SwitchCaseTextContainerBlock(Blockly));
			Script.SetBlocks(new SwitchCaseTextConstBlock(Blockly));
			Script.SetBlocks(new SwitchCaseTextRangeBlock(Blockly));
			Script.SetBlocks(new SwitchCaseTextDefaultBlock(Blockly));
			Script.SetBlocks(new LogicCompareBlock(Blockly));
			Script.SetBlocks(new LogicOperationBlock(Blockly));
			Script.SetBlocks(new LogicNegateBlock(Blockly));
			Script.SetBlocks(new LogicBooleanBlock(Blockly));
			Script.SetBlocks(new LogicNullBlock(Blockly));
			Script.SetBlocks(new LogicTernaryBlock(Blockly));
			Script.SetBlocks(new ControlsRepeatExtBlock(Blockly));
			Script.SetBlocks(new ControlsRepeatBlock(Blockly));
			Script.SetBlocks(new ControlsWhileUntilBlock(Blockly));
			Script.SetBlocks(new ControlsForBlock(Blockly));
			Script.SetBlocks(new ControlsForEachBlock(Blockly));
			Script.SetBlocks(new ControlsFlowStatementsBlock(Blockly));
			Script.SetBlocks(new MathNumberBlock(Blockly));
			Script.SetBlocks(new MathArithmeticBlock(Blockly));
			Script.SetBlocks(new MathSingleBlock(Blockly));
			Script.SetBlocks(new MathTrigBlock(Blockly));
			Script.SetBlocks(new MathConstantBlock(Blockly));
			Script.SetBlocks(new MathNumberPropertyBlock(Blockly));
			Script.SetBlocks(new MathChangeBlock(Blockly));
			Script.SetBlocks(new MathRoundBlock(Blockly));
			Script.SetBlocks(new MathOnListBlock(Blockly));
			Script.SetBlocks(new MathModuloBlock(Blockly));
			Script.SetBlocks(new MathConstrainBlock(Blockly));
			Script.SetBlocks(new MathRandomIntBlock(Blockly));
			Script.SetBlocks(new MathRandomFloatBlock(Blockly));
			Script.SetBlocks(new ProceduresDefnoreturnBlock(Blockly));
			Script.SetBlocks(new ProceduresDefreturnBlock(Blockly));
			Script.SetBlocks(new ProceduresMutatorcontainerBlock(Blockly));
			Script.SetBlocks(new ProceduresMutatorargBlock(Blockly));
			Script.SetBlocks(new ProceduresCallnoreturnBlock(Blockly));
			Script.SetBlocks(new ProceduresCallreturnBlock(Blockly));
			Script.SetBlocks(new ProceduresIfreturnBlock(Blockly));
			Script.SetBlocks(new TextBlock(Blockly));
			Script.SetBlocks(new TextJoinBlock(Blockly));
			Script.SetBlocks(new TextCreateJoinContainerBlock(Blockly));
			Script.SetBlocks(new TextCreateJoinItemBlock(Blockly));
			Script.SetBlocks(new TextAppendBlock(Blockly));
			Script.SetBlocks(new TextLengthBlock(Blockly));
			Script.SetBlocks(new TextIsEmptyBlock(Blockly));
			Script.SetBlocks(new TextIndexOfBlock(Blockly));
			Script.SetBlocks(new TextCharAtBlock(Blockly));
			Script.SetBlocks(new TextGetSubstringBlock(Blockly));
			Script.SetBlocks(new TextChangeCaseBlock(Blockly));
			Script.SetBlocks(new TextTrimBlock(Blockly));
			Script.SetBlocks(new TextPrintBlock(Blockly));
			Script.SetBlocks(new TextPromptExtBlock(Blockly));
			Script.SetBlocks(new TextPromptBlock(Blockly));
			Script.SetBlocks(new VariablesGetBlock(Blockly));
			Script.SetBlocks(new VariablesSetBlock(Blockly));
			Script.SetGenerator();
		}

		internal static void Init2()
		{
			var workspace = Blockly.getMainWorkspace();
			workspace.addChangeListener(Workspace_Changed);
		}

		private static void Workspace_Changed(Abstract e)
		{
			changed = true;
		}
	}
}
