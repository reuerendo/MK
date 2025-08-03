using Microsoft.UI.Xaml.Controls;

namespace MiniKeyboard
{
    public sealed partial class AddEditButtonDialog : ContentDialog
    {
        public AddEditButtonDialog()
        {
            this.InitializeComponent();
        }

        public void SetModel(KeyButtonModel model)
        {
            DisplayTextBox.Text = model.DisplayText;
            KeyCombinationBox.Text = model.KeyCombination;
            
            // Set icon selection
            foreach (ComboBoxItem item in IconComboBox.Items)
            {
                if (item.Tag?.ToString() == model.Icon)
                {
                    IconComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        public KeyButtonModel GetModel()
        {
            return new KeyButtonModel
            {
                DisplayText = DisplayTextBox.Text?.Trim() ?? "",
                KeyCombination = KeyCombinationBox.Text?.Trim() ?? "",
                Icon = (IconComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "⌨"
            };
        }
    }
}