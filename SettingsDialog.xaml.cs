using Microsoft.UI.Xaml.Controls;

namespace MiniKeyboard
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load settings from app settings (would be from registry or config file)
            // For now, using default values
            AlwaysOnTopToggle.IsOn = true;
            OpacitySlider.Value = 1.0;
            ButtonSizeComboBox.SelectedIndex = 1; // Medium
            ThemeComboBox.SelectedIndex = 0; // Default
            StartWithWindowsToggle.IsOn = false;
            MinimizeToTrayToggle.IsOn = false;
        }

        public SettingsModel GetSettings()
        {
            return new SettingsModel
            {
                AlwaysOnTop = AlwaysOnTopToggle.IsOn,
                WindowOpacity = OpacitySlider.Value,
                ButtonSize = (ButtonSizeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Medium",
                Theme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Default",
                StartWithWindows = StartWithWindowsToggle.IsOn,
                MinimizeToTray = MinimizeToTrayToggle.IsOn
            };
        }
    }

    public class SettingsModel
    {
        public bool AlwaysOnTop { get; set; }
        public double WindowOpacity { get; set; }
        public string ButtonSize { get; set; }
        public string Theme { get; set; }
        public bool StartWithWindows { get; set; }
        public bool MinimizeToTray { get; set; }
    }
}