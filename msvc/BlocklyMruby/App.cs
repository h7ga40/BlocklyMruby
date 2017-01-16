using System.Text;
using Bridge;

namespace BlocklyMruby
{
	static class App
	{
		public static TerminalHost Term;
		public static bool changed;

		internal static void Init()
		{
			BlocklyScript.SetMessage(typeof(Msg));
			BlocklyScript.SetBlocks(new ColourPickerBlock());
			BlocklyScript.SetBlocks(new ColourRandomBlock());
			BlocklyScript.SetBlocks(new ColourRGBBlock());
			BlocklyScript.SetBlocks(new ColourBlendBlock());
			BlocklyScript.SetBlocks(new ListsCreateEmptyBlock());
			BlocklyScript.SetBlocks(new ListsCreateWithBlock());
			BlocklyScript.SetBlocks(new ListsCreateWithContainerBlock());
			BlocklyScript.SetBlocks(new ListsCreateWithItemBlock());
			BlocklyScript.SetBlocks(new ListsRepeatBlock());
			BlocklyScript.SetBlocks(new ListsLengthBlock());
			BlocklyScript.SetBlocks(new ListsIsEmptyBlock());
			BlocklyScript.SetBlocks(new ListsIndexOfBlock());
			BlocklyScript.SetBlocks(new ListsGetIndexBlock());
			BlocklyScript.SetBlocks(new ListsSetIndexBlock());
			BlocklyScript.SetBlocks(new ListsGetSublistBlock());
			BlocklyScript.SetBlocks(new ListsSortBlock());
			BlocklyScript.SetBlocks(new ListsSplitBlock());
			BlocklyScript.SetBlocks(new ControlsIfBlock());
			BlocklyScript.SetBlocks(new ControlsIfIfBlock());
			BlocklyScript.SetBlocks(new ControlsIfElseIfBlock());
			BlocklyScript.SetBlocks(new ControlsIfElseBlock());
			BlocklyScript.SetBlocks(new SwitchCaseNumberBlock());
			BlocklyScript.SetBlocks(new SwitchCaseNumberContainerBlock());
			BlocklyScript.SetBlocks(new SwitchCaseNumberConstBlock());
			BlocklyScript.SetBlocks(new SwitchCaseNumberRangeBlock());
			BlocklyScript.SetBlocks(new SwitchCaseNumberDefaultBlock());
			BlocklyScript.SetBlocks(new SwitchCaseTextBlock());
			BlocklyScript.SetBlocks(new SwitchCaseTextContainerBlock());
			BlocklyScript.SetBlocks(new SwitchCaseTextConstBlock());
			BlocklyScript.SetBlocks(new SwitchCaseTextRangeBlock());
			BlocklyScript.SetBlocks(new SwitchCaseTextDefaultBlock());
			BlocklyScript.SetBlocks(new LogicCompareBlock());
			BlocklyScript.SetBlocks(new LogicOperationBlock());
			BlocklyScript.SetBlocks(new LogicNegateBlock());
			BlocklyScript.SetBlocks(new LogicBooleanBlock());
			BlocklyScript.SetBlocks(new LogicNullBlock());
			BlocklyScript.SetBlocks(new LogicTernaryBlock());
			BlocklyScript.SetBlocks(new ControlsRepeatExtBlock());
			BlocklyScript.SetBlocks(new ControlsRepeatBlock());
			BlocklyScript.SetBlocks(new ControlsWhileUntilBlock());
			BlocklyScript.SetBlocks(new ControlsForBlock());
			BlocklyScript.SetBlocks(new ControlsForEachBlock());
			BlocklyScript.SetBlocks(new ControlsFlowStatementsBlock());
			BlocklyScript.SetBlocks(new MathNumberBlock());
			BlocklyScript.SetBlocks(new MathArithmeticBlock());
			BlocklyScript.SetBlocks(new MathSingleBlock());
			BlocklyScript.SetBlocks(new MathTrigBlock());
			BlocklyScript.SetBlocks(new MathConstantBlock());
			BlocklyScript.SetBlocks(new MathNumberPropertyBlock());
			BlocklyScript.SetBlocks(new MathChangeBlock());
			BlocklyScript.SetBlocks(new MathRoundBlock());
			BlocklyScript.SetBlocks(new MathOnListBlock());
			BlocklyScript.SetBlocks(new MathModuloBlock());
			BlocklyScript.SetBlocks(new MathConstrainBlock());
			BlocklyScript.SetBlocks(new MathRandomIntBlock());
			BlocklyScript.SetBlocks(new MathRandomFloatBlock());
			BlocklyScript.SetBlocks(new ProceduresDefnoreturnBlock());
			BlocklyScript.SetBlocks(new ProceduresDefreturnBlock());
			BlocklyScript.SetBlocks(new ProceduresMutatorcontainerBlock());
			BlocklyScript.SetBlocks(new ProceduresMutatorargBlock());
			BlocklyScript.SetBlocks(new ProceduresCallnoreturnBlock());
			BlocklyScript.SetBlocks(new ProceduresCallreturnBlock());
			BlocklyScript.SetBlocks(new ProceduresIfreturnBlock());
			BlocklyScript.SetBlocks(new TextBlock());
			BlocklyScript.SetBlocks(new TextJoinBlock());
			BlocklyScript.SetBlocks(new TextCreateJoinContainerBlock());
			BlocklyScript.SetBlocks(new TextCreateJoinItemBlock());
			BlocklyScript.SetBlocks(new TextAppendBlock());
			BlocklyScript.SetBlocks(new TextLengthBlock());
			BlocklyScript.SetBlocks(new TextIsEmptyBlock());
			BlocklyScript.SetBlocks(new TextIndexOfBlock());
			BlocklyScript.SetBlocks(new TextCharAtBlock());
			BlocklyScript.SetBlocks(new TextGetSubstringBlock());
			BlocklyScript.SetBlocks(new TextChangeCaseBlock());
			BlocklyScript.SetBlocks(new TextTrimBlock());
			BlocklyScript.SetBlocks(new TextPrintBlock());
			BlocklyScript.SetBlocks(new TextPromptExtBlock());
			BlocklyScript.SetBlocks(new TextPromptBlock());
			BlocklyScript.SetBlocks(new VariablesGetBlock());
			BlocklyScript.SetBlocks(new VariablesSetBlock());
			BlocklyScript.SetGenerator();
		}

		internal static void Init2()
		{
			var workspace = Blockly.getMainWorkspace();
			workspace.addChangeListener(Workspace_Changed);
		}

		private static void Workspace_Changed(Blockly.Events.Abstract obj)
		{
			changed = true;
		}
	}
}
