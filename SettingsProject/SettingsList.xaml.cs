using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    internal sealed partial class SettingsList
    {
        public event Action<Setting>? ScrolledSettingChanged;

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

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            VisualTreeHelper.HitTest(
                _itemsControl, 
                null, 
                result =>
                {
                    var setting = TryFindSetting(result.VisualHit);

                    if (setting != null)
                    {
                        ScrolledSettingChanged?.Invoke(setting);
                    }

                    return HitTestResultBehavior.Stop;
                }, 
                new PointHitTestParameters(new Point(0, 0)));

            static Setting? TryFindSetting(DependencyObject? obj)
            {
                while (true)
                {
                    if (obj is null)
                    {
                        return null;
                    }

                    if (obj is FrameworkElement fe)
                    {
                        if (Try(fe.DataContext, out var s1))
                        {
                            return s1;
                        }

                        if (fe.DataContext is Setting s2)
                        {
                            return s2;
                        }
                    }

                    obj = VisualTreeHelper.GetParent(obj);
                }

                static bool Try(object o, out Setting? s)
                {
                    while (true)
                    {
                        if (o is CollectionViewGroup g)
                        {
                            var firstItem = g.Items.FirstOrDefault();

                            if (firstItem is Setting s1)
                            {
                                s = s1;
                                return true;
                            }

                            if (firstItem != null && !g.IsBottomLevel)
                            {
                                o = firstItem;
                                continue;
                            }
                        }

                        s = null;
                        return false;
                    }
                }
            }
        }
    }
}
