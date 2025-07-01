using System.Windows;

namespace Jace.DemoApp;

/// <summary>
///     Interaction logic for InputDialog.xaml
/// </summary>
public partial class InputDialog : Window
{
    public InputDialog(string variableName)
    {
        InitializeComponent();
        questionLabel.Content = $"Please provide a value for variable \"{variableName}\":";
    }

    public double Value { get; private set; }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        while (true)
        {
            if (double.TryParse(valueTextBox.Text, out var result))
            {
                Value = result;
                Close();
                return;
            }

            MessageBox.Show($"Not a valid number: \"{valueTextBox.Text}\"",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}