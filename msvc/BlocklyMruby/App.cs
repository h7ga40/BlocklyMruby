using System.Text;
using Bridge;

namespace BlocklyMruby
{
	class App
	{
		public class Terminal
		{
			StringBuilder log = new StringBuilder();

			internal void flush()
			{
				System.Console.Write(log.ToString());
				if (log.Length != 0)
					log.Clear();
			}

			internal void write(string text)
			{
				log.Append(text);
			}
		}

		public static Terminal Term = new Terminal();

		internal static void Init()
		{
			Script.SetMessage(typeof(Msg));
			Script.SetBlocks(new ColourPickerBlock());
			Script.SetBlocks(new ColourRandomBlock());
			Script.SetBlocks(new ColourRGBBlock());
			Script.SetBlocks(new ColourBlendBlock());
			Script.SetBlocks(new ListsCreateEmptyBlock());
			Script.SetBlocks(new ListsCreateWithBlock());
			Script.SetBlocks(new ListsCreateWithContainerBlock());
			Script.SetBlocks(new ListsCreateWithItemBlock());
			Script.SetBlocks(new ListsRepeatBlock());
			Script.SetBlocks(new ListsLengthBlock());
			Script.SetBlocks(new ListsIsEmptyBlock());
			Script.SetBlocks(new ListsIndexOfBlock());
			Script.SetBlocks(new ListsGetIndexBlock());
			Script.SetBlocks(new ListsSetIndexBlock());
			Script.SetBlocks(new ListsGetSublistBlock());
			Script.SetBlocks(new ListsSortBlock());
			Script.SetBlocks(new ListsSplitBlock());
			Script.SetBlocks(new ControlsIfBlock());
			Script.SetBlocks(new ControlsIfIfBlock());
			Script.SetBlocks(new ControlsIfElseIfBlock());
			Script.SetBlocks(new ControlsIfElseBlock());
			Script.SetBlocks(new SwitchCaseNumberBlock());
			Script.SetBlocks(new SwitchCaseNumberContainerBlock());
			Script.SetBlocks(new SwitchCaseNumberConstBlock());
			Script.SetBlocks(new SwitchCaseNumberRangeBlock());
			Script.SetBlocks(new SwitchCaseNumberDefaultBlock());
			Script.SetBlocks(new SwitchCaseTextBlock());
			Script.SetBlocks(new SwitchCaseTextContainerBlock());
			Script.SetBlocks(new SwitchCaseTextConstBlock());
			Script.SetBlocks(new SwitchCaseTextRangeBlock());
			Script.SetBlocks(new SwitchCaseTextDefaultBlock());
			Script.SetBlocks(new LogicCompareBlock());
			Script.SetBlocks(new LogicOperationBlock());
			Script.SetBlocks(new LogicNegateBlock());
			Script.SetBlocks(new LogicBooleanBlock());
			Script.SetBlocks(new LogicNullBlock());
			Script.SetBlocks(new LogicTernaryBlock());
			Script.SetBlocks(new ControlsRepeatExtBlock());
			Script.SetBlocks(new ControlsRepeatBlock());
			Script.SetBlocks(new ControlsWhileUntilBlock());
			Script.SetBlocks(new ControlsForBlock());
			Script.SetBlocks(new ControlsForEachBlock());
			Script.SetBlocks(new ControlsFlowStatementsBlock());
			Script.SetBlocks(new MathNumberBlock());
			Script.SetBlocks(new MathArithmeticBlock());
			Script.SetBlocks(new MathSingleBlock());
			Script.SetBlocks(new MathTrigBlock());
			Script.SetBlocks(new MathConstantBlock());
			Script.SetBlocks(new MathNumberPropertyBlock());
			Script.SetBlocks(new MathChangeBlock());
			Script.SetBlocks(new MathRoundBlock());
			Script.SetBlocks(new MathOnListBlock());
			Script.SetBlocks(new MathModuloBlock());
			Script.SetBlocks(new MathConstrainBlock());
			Script.SetBlocks(new MathRandomIntBlock());
			Script.SetBlocks(new MathRandomFloatBlock());
			Script.SetBlocks(new ProceduresDefnoreturnBlock());
			Script.SetBlocks(new ProceduresDefreturnBlock());
			Script.SetBlocks(new ProceduresMutatorcontainerBlock());
			Script.SetBlocks(new ProceduresMutatorargBlock());
			Script.SetBlocks(new ProceduresCallnoreturnBlock());
			Script.SetBlocks(new ProceduresCallreturnBlock());
			Script.SetBlocks(new ProceduresIfreturnBlock());
			Script.SetBlocks(new TextBlock());
			Script.SetBlocks(new TextJoinBlock());
			Script.SetBlocks(new TextCreateJoinContainerBlock());
			Script.SetBlocks(new TextCreateJoinItemBlock());
			Script.SetBlocks(new TextAppendBlock());
			Script.SetBlocks(new TextLengthBlock());
			Script.SetBlocks(new TextIsEmptyBlock());
			Script.SetBlocks(new TextIndexOfBlock());
			Script.SetBlocks(new TextCharAtBlock());
			Script.SetBlocks(new TextGetSubstringBlock());
			Script.SetBlocks(new TextChangeCaseBlock());
			Script.SetBlocks(new TextTrimBlock());
			Script.SetBlocks(new TextPrintBlock());
			Script.SetBlocks(new TextPromptExtBlock());
			Script.SetBlocks(new TextPromptBlock());
			Script.SetBlocks(new VariablesGetBlock());
			Script.SetBlocks(new VariablesSetBlock());
			Script.SetGenerator(new Ruby());
		}

		internal static void Init2()
		{
			var workspace = Blockly.getMainWorkspace();
			workspace.addChangeListener(Workspace_Changed);
		}

		private static void Workspace_Changed(Blockly.Events.Abstract obj)
		{
		}
	}
}
