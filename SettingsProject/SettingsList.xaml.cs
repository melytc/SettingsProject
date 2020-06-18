using System.Collections.Generic;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal sealed partial class SettingsList
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings",
            typeof(IReadOnlyList<Setting>),
            typeof(SettingsList),
            new PropertyMetadata(default(IReadOnlyList<Setting>)));

        public static readonly DependencyProperty SearchViewModelProperty = DependencyProperty.Register("SearchViewModel", typeof(SearchViewModel), typeof(SettingsList), new PropertyMetadata(default(SearchViewModel)));

        public SettingsList()
        {
            InitializeComponent();
        }

        public IReadOnlyList<Setting> Settings
        {
            get => (IReadOnlyList<Setting>)GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public SearchViewModel SearchViewModel
        {
            get => (SearchViewModel)GetValue(SearchViewModelProperty);
            set => SetValue(SearchViewModelProperty, value);
        }
    }
}
