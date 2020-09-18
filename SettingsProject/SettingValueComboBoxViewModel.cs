using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingValueComboBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly SettingValue _settingValue;

        public SettingValueComboBoxViewModel(SettingValue settingValue)
        {
            _settingValue = settingValue;

            _settingValue.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SettingValue.SupportedValues):
                        PropertyChanged?.Invoke(this, e);
                        break;
                    case nameof(SettingValue.EvaluatedValue):
                        OnPropertyChanged(nameof(SelectedValue));
                        break;
                }
            };
        }

        public ImmutableArray<SupportedValue> SupportedValues => _settingValue.SupportedValues;

        public SupportedValue SelectedValue
        {
            get => _settingValue.SupportedValues.FirstOrDefault(v => StringComparers.SettingValues.Equals(v.Value, _settingValue.EvaluatedValue as string));
            set
            {
                foreach (var supportedValue in _settingValue.SupportedValues)
                {
                    if (StringComparers.SettingValues.Equals(supportedValue.Value, value.Value))
                    {
                        _settingValue.EvaluatedValue = supportedValue.Value;
                        OnPropertyChanged();
                        return;
                    }
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}