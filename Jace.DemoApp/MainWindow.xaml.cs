using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Jace.Execution;
using Jace.Operations;
using Jace.Operations.BuiltIn;
using Jace.Tokenizer;

namespace Jace.DemoApp;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void calculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ClearScreen();

            var formula = formulaTextBox.Text;

            var reader = new TokenReader(CultureInfo.InvariantCulture);
            var tokens = reader.Read(formula);

            ShowTokens(tokens);

            IFunctionRegistry functionRegistry = new FunctionRegistry(false);

            var astBuilder = new AstBuilder(false, functionRegistry);
            var operation = astBuilder.Build(tokens);

            ShowAbstractSyntaxTree(operation);

            var variables = new Dictionary<string, double>();
            foreach (var variable in GetVariables(operation))
            {
                var value = AskValueOfVariable(variable);
                variables.Add(variable.Name, value);
            }

            var executor = new Interpreter();
            var result = executor.Execute(operation, null, null, variables);

            resultTextBox.Text = result.ToString(CultureInfo.CurrentCulture);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.GetType()}: {ex.Message} \n{ex.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ClearScreen()
    {
        tokensTextBox.Text = string.Empty;
        astTreeView.Items.Clear();
        resultTextBox.Text = string.Empty;
    }

    private void ShowTokens(List<Token> tokens)
    {
        tokensTextBox.Text =
            "[ "
            + string.Join(", ", tokens.Select(token => token.Value switch
            {
                string str => $"\"{str}\"",
                char => $"'{token.Value}'",
                int or double => token.Value.ToString(),
                _ => "Invalid Token"
            }))
            + " ]";
    }

    private void ShowAbstractSyntaxTree(Operation operation)
    {
        astTreeView.Items.Clear();
        astTreeView.Items.Add(CreateTreeViewItem(operation));
    }

    private static TreeViewItem CreateTreeViewItem(Operation operation)
    {
        var item = new TreeViewItem
        {
            Header = GetLabelText(operation)
        };

        switch (operation)
        {
            case BinaryOperation binaryOperation:
                item.Items.Add(CreateTreeViewItem(binaryOperation.Argument1));
                item.Items.Add(CreateTreeViewItem(binaryOperation.Argument2));
                break;
            case Function function:
            {
                foreach (var argument in function.Arguments)
                    item.Items.Add(CreateTreeViewItem(argument));
                break;
            }
        }

        return item;
    }

    private static string GetLabelText(Operation operation)
    {
        var name = operation.GetType().Name;
        var dataType = operation.DataType.ToString();

        var value = operation switch
        {
            IntegerConstant integerConstant => "(" + integerConstant.Value + ")",
            FloatingPointConstant floatingPointConstant => "(" + floatingPointConstant.Value + ")",
            Variable variable => "(" + variable.Name + ")",
            Function function => "(" + function.FunctionName + ")",
            _ => string.Empty
        };

        return FormattableString.Invariant($"{name}<{dataType}>{value}");
    }

    private static List<Variable> GetVariables(Operation operation)
    {
        if (!operation.DependsOnVariables) return [];

        return operation.DependsOnVariables
            ? operation switch
            {
                Variable variable => [variable],
                BinaryOperation binaryOperation => [
                    ..GetVariables(binaryOperation.Argument1), 
                    ..GetVariables(binaryOperation.Argument2)],
                Function function => [..function.Arguments.SelectMany(GetVariables)],
                _ => []
            }
            : [];
    }

    private static double AskValueOfVariable(Variable variable)
    {
        var dialog = new InputDialog(variable.Name);
        dialog.ShowDialog();

        return dialog.Value;
    }
}