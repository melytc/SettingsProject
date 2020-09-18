using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    [TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
    internal sealed class SettingsList : Control
    {
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            nameof(Settings),
            typeof(IReadOnlyList<Setting>),
            typeof(SettingsList),
            new PropertyMetadata(null, (d, e) => ((SettingsList)d).OnSettingsChanged()));

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            nameof(SearchText),
            typeof(string),
            typeof(SettingsList),
            new PropertyMetadata(""));
        
        public static readonly DependencyProperty CurrentSectionProperty = DependencyProperty.Register(
            nameof(CurrentSection),
            typeof(NavigationSection),
            typeof(SettingsList),
            new PropertyMetadata(default(NavigationSection), (d, e) => ((SettingsList)d).OnCurrentSectionChanged()));

        static SettingsList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsList), new FrameworkPropertyMetadata(typeof(SettingsList)));
        }

        public IReadOnlyList<Setting> Settings
        {
            get => (IReadOnlyList<Setting>)GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public NavigationSection CurrentSection
        {
            get => (NavigationSection)GetValue(CurrentSectionProperty);
            set => SetValue(CurrentSectionProperty, value);
        }

        private bool _ignoreNextCurrentSectionChangeEvent;

        private void OnCurrentSectionChanged()
        {
            if (_ignoreNextCurrentSectionChangeEvent)
            {
                _ignoreNextCurrentSectionChangeEvent = false;
                return;
            }

            var section = CurrentSection;

            var viewSource = (ListCollectionView)CollectionViewSource.GetDefaultView(Settings);

            Assumes.NotNull(viewSource.Groups);

            CollectionViewGroup? group = viewSource.Groups.Cast<CollectionViewGroup>().FirstOrDefault(g => g.Name.Equals(section.Page));

            _scrollToTopGroup = group;
            _scrollToSubGroup = null;

            if (group == null)
            {
                return;
            }

            if (group.Items.Count != 0)
            {
                var firstSubGroup = (CollectionViewGroup)group.Items[0];
                if ((string)firstSubGroup.Name != section.Category)
                {
                    group = group.Items.Cast<CollectionViewGroup>().FirstOrDefault(g => g.Name.Equals(section.Category));
                    _scrollToSubGroup = group;
                }
            }

            Assumes.NotNull(_itemsControl);
            var pageGroupContainer = (GroupItem)_itemsControl.ItemContainerGenerator.ContainerFromItem(group);

            pageGroupContainer?.BringIntoView();
        }

        private void OnSettingsChanged()
        {
            _scrollViewer?.ScrollToTop();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = (ItemsControl?)GetTemplateChild("PART_ItemsControl");
            Assumes.NotNull(_itemsControl);
            _itemsControl.ApplyTemplate();

            VisualTreeUtil.TryFindDescendentBreadthFirst(_itemsControl, out _scrollViewer);
            Assumes.NotNull(_scrollViewer);
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }

        private CollectionViewGroup? _scrollToTopGroup;
        private CollectionViewGroup? _scrollToSubGroup;
        private bool _ignoreNextScrollEvent;

        private ItemsControl? _itemsControl;
        private ScrollViewer? _scrollViewer;

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_ignoreNextScrollEvent)
            {
                _ignoreNextScrollEvent = false;
                return;
            }

            Assumes.NotNull(_itemsControl);

            // TODO before 'scroll to top' logic, ensure we haven't scrolled to this group already anyway (can happen when scrolling up after navigating to a subgroup)

            var topGroup = _scrollToTopGroup;
            if (topGroup != null)
            {
                var subGroup = _scrollToSubGroup;
                _scrollToTopGroup = null;
                _scrollToSubGroup = null;

                if (!VisualTreeUtil.TryFindDescendentBreadthFirst(_itemsControl, out ScrollViewer? scrollViewer))
                {
                    return;
                }

                var viewSource = (ListCollectionView)CollectionViewSource.GetDefaultView(Settings);

                double offset = 0;
                foreach (CollectionViewGroup g1 in viewSource.Groups)
                {
                    if (ReferenceEquals(g1, topGroup))
                    {
                        if (subGroup != null)
                        {
                            foreach (CollectionViewGroup g2 in g1.Items)
                            {
                                if (ReferenceEquals(g2, subGroup))
                                {
                                    break;
                                }

                                var container = (GroupItem)_itemsControl.ItemContainerGenerator.ContainerFromItem(g2);

                                offset += container.ActualHeight;
                            }
                        }

                        break;
                    }

                    offset += ((FrameworkElement)_itemsControl.ItemContainerGenerator.ContainerFromItem(g1)).ActualHeight;
                }

                _ignoreNextScrollEvent = true;
                scrollViewer.ScrollToVerticalOffset(offset);

                return;
            }

            VisualTreeHelper.HitTest(
                _itemsControl, 
                null, 
                result =>
                {
                    var setting = TryFindSetting(result.VisualHit);

                    if (setting != null)
                    {
                        var section = new NavigationSection(setting.Page, setting.Category);

                        if (section != CurrentSection)
                        {
                            _ignoreNextCurrentSectionChangeEvent = true;
                            CurrentSection = section;
                        }
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
