using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI;
using static Microsoft.UI.Win32Interop;

namespace MiniKeyboard
{
    public sealed partial class MainWindow : Window
    {
        private KeyboardHook _keyboardHook;
        private ObservableCollection<KeyButtonModel> _buttons;

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeWindow();
            InitializeButtons();
            InitializeKeyboardHook();
            LoadStyles();
        }

        private void InitializeWindow()
        {
            // Get window handle for Win32 operations
            var windowHandle = WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Set window properties for overlay behavior
            appWindow.Resize(new SizeInt32(300, 400));
            appWindow.Move(new PointInt32(100, 100));
            
            // Set always on top
            var presenter = appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsAlwaysOnTop = true;
                presenter.IsResizable = false;
                presenter.IsMinimizable = false;
                presenter.IsMaximizable = false;
            }

            this.Title = "Mini Keyboard";
        }

        private void InitializeButtons()
        {
            _buttons = new ObservableCollection<KeyButtonModel>();
            
            // Add default buttons
            _buttons.Add(new KeyButtonModel { DisplayText = "Ctrl", KeyCombination = "LControlKey", Icon = "🎮" });
            _buttons.Add(new KeyButtonModel { DisplayText = "Alt", KeyCombination = "LAltKey", Icon = "⌨" });
            _buttons.Add(new KeyButtonModel { DisplayText = "Shift", KeyCombination = "LShiftKey", Icon = "⇧" });
            _buttons.Add(new KeyButtonModel { DisplayText = "Ctrl+A", KeyCombination = "LControlKey+A", Icon = "📝" });
            _buttons.Add(new KeyButtonModel { DisplayText = "Ctrl+C", KeyCombination = "LControlKey+C", Icon = "📋" });
            _buttons.Add(new KeyButtonModel { DisplayText = "Ctrl+V", KeyCombination = "LControlKey+V", Icon = "📄" });
            
            ButtonsContainer.ItemsSource = _buttons;
        }

        private void InitializeKeyboardHook()
        {
            _keyboardHook = new KeyboardHook();
        }

        private void LoadStyles()
        {
            // Define custom button style in code-behind
            var keyButtonStyle = new Style(typeof(Button));
            keyButtonStyle.Setters.Add(new Setter(Button.MinWidthProperty, 45));
            keyButtonStyle.Setters.Add(new Setter(Button.MinHeightProperty, 35));
            keyButtonStyle.Setters.Add(new Setter(Button.FontSizeProperty, 10));
            keyButtonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(4)));
            
            ((FrameworkElement)this.Content).Resources["KeyButtonStyle"] = keyButtonStyle;
        }

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is KeyButtonModel model)
            {
                _keyboardHook.SendKeyDown(model.KeyCombination);
            }
        }

        private void KeyButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Context menu will show automatically
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAddEditDialog(null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem)
            {
                var button = ((MenuFlyout)menuItem.Parent).Target as Button;
                if (button?.Tag is KeyButtonModel model)
                {
                    ShowAddEditDialog(model);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem)
            {
                var button = ((MenuFlyout)menuItem.Parent).Target as Button;
                if (button?.Tag is KeyButtonModel model)
                {
                    _buttons.Remove(model);
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Show settings dialog
            ShowSettingsDialog();
        }

        private async void ShowAddEditDialog(KeyButtonModel existingModel)
        {
            var dialog = new AddEditButtonDialog
            {
                XamlRoot = this.Content.XamlRoot
            };

            if (existingModel != null)
            {
                dialog.SetModel(existingModel);
            }

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var model = dialog.GetModel();
                
                if (existingModel != null)
                {
                    // Update existing
                    var index = _buttons.IndexOf(existingModel);
                    _buttons[index] = model;
                }
                else
                {
                    // Add new
                    _buttons.Add(model);
                }
            }
        }

        private async void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}