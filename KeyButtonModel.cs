using System.ComponentModel;

namespace MiniKeyboard
{
    public class KeyButtonModel : INotifyPropertyChanged
    {
        private string _displayText;
        private string _keyCombination;
        private string _icon;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string KeyCombination
        {
            get => _keyCombination;
            set
            {
                _keyCombination = value;
                OnPropertyChanged(nameof(KeyCombination));
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}