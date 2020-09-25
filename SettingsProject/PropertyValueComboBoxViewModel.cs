using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    /// <summary>
    /// Maps a property's value to one of its supported values. Joins display name for UI presentation.
    /// </summary>
    internal sealed class PropertyValueComboBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly PropertyValue _propertyValue;

        /// <summary>
        /// A local copy of the underlying property's <see cref="PropertyValue.SupportedValues"/> collection.
        /// If the property's value is not present in its set of supported values, we add an option to this
        /// local copy so that the option can be represented in a combo box.
        /// </summary>
        public IEnumerable<SupportedValue> SupportedValues { get; private set; }

        public PropertyValueComboBoxViewModel(PropertyValue propertyValue)
        {
            _propertyValue = propertyValue;

            SupportedValues = propertyValue.SupportedValues;

            UpdateSupportedValues();

            _propertyValue.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(PropertyValue.SupportedValues):
                        PropertyChanged?.Invoke(this, e);
                        break;
                    case nameof(PropertyValue.EvaluatedValue):
                        UpdateSupportedValues();
                        OnPropertyChanged(nameof(SelectedValue));
                        break;
                }
            };
        }

        public SupportedValue SelectedValue
        {
            get
            {
                // Find a supported value that matches the underlying value
                if (_propertyValue.EvaluatedValue is string value)
                {
                    foreach (var supportedValue in SupportedValues)
                    {
                        if (StringComparers.PropertyValues.Equals(supportedValue.Value, value))
                        {
                            return supportedValue;
                        }
                    }

                    // We expect the underlying value to always be present in our local SupportedValues collection.
                    Assumes.NotReachable();
                }

                return default;
            }
            set
            {
                _propertyValue.EvaluatedValue = value.Value;
                UpdateSupportedValues();
                OnPropertyChanged();
            }
        }

        private void UpdateSupportedValues()
        {
            if (_propertyValue.EvaluatedValue is string value)
            {
                foreach (var supportedValue in SupportedValues)
                {
                    if (StringComparers.PropertyValues.Equals(supportedValue.Value, value))
                    {
                        // The SupportedValues collection contains the underlying property's
                        // value and does not need modification.
                        return;
                    }
                }

                // The underlying property's value is not one of the advertised supported values.
                // We still want to show it in the UI, so prepend a combo box item to the list.
                SupportedValues = new[] { new SupportedValue(value) }.Concat(SupportedValues).ToImmutableArray();
                OnPropertyChanged(nameof(SupportedValues));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}