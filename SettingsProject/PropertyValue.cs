using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class PropertyValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object _evaluatedValue;
        private string _unevaluatedValue;
        private ImmutableArray<SupportedValue> _supportedValues = ImmutableArray<SupportedValue>.Empty;

        public PropertyValue(string unevaluatedValue, object evaluatedValue, ImmutableDictionary<string, string>? configurationDimensions = null)
        {
            ConfigurationDimensions = configurationDimensions ?? ImmutableDictionary<string, string>.Empty;
            _evaluatedValue = evaluatedValue;
            _unevaluatedValue = unevaluatedValue;
        }

        // the set of dimensions for which this value is specified. any omitted dimensions are invariant.
        // empty if this value applies to all configurations
        public ImmutableDictionary<string, string> ConfigurationDimensions { get; }

        public DataTemplate? Template => ConfigurationDimensions.IsEmpty ? Parent?.Editor?.UnconfiguredDataTemplate : Parent?.Editor?.ConfiguredDataTemplate;

        public Property? Parent { get; internal set; }

        public ImmutableArray<SupportedValue> SupportedValues
        {
            get => _supportedValues;
            set
            {
                _supportedValues = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets and sets the current evaluated value of the property.
        /// </summary>
        public object EvaluatedValue
        {
            get => _evaluatedValue;
            set
            {
                if (!Equals(value, EvaluatedValue))
                {
                    _evaluatedValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the current unevaluated value of the property.
        /// </summary>
        public string UnevaluatedValue
        {
            get => _unevaluatedValue;
            set
            {
                if (!Equals(value, UnevaluatedValue))
                {
                    _unevaluatedValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public PropertyValue Clone() => new PropertyValue(_unevaluatedValue, _evaluatedValue, ConfigurationDimensions);

        public override string ToString()
        {
            return Equals(_evaluatedValue, _unevaluatedValue)
                ? $"[{string.Join(" & ", ConfigurationDimensions.Values)}] = {_evaluatedValue}"
                : $"[{string.Join(" & ", ConfigurationDimensions.Values)}] = EVAL({_unevaluatedValue}) = {_evaluatedValue}";
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}