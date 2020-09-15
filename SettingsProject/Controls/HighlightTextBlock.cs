using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    // TODO may be able to get better perf by not deriving from TextBlock and using GlyphRun/Glyph instead

    internal class HighlightTextBlock : TextBlock
    {
        static HighlightTextBlock()
        {
            TextProperty.OverrideMetadata(
                typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata("", null, (d, e) => ((HighlightTextBlock)d).OnCoerceTextPropertyValue(e)));
        }

        public static readonly DependencyProperty HighlightTextProperty = DependencyProperty.Register(
            nameof(HighlightText),
            typeof(string),
            typeof(HighlightTextBlock),
            new PropertyMetadata("", (d, e) => ((HighlightTextBlock)d).OnHighlightTextChanged(e.NewValue)));

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            nameof(HighlightBrush),
            typeof(Brush),
            typeof(HighlightTextBlock),
            new PropertyMetadata(Brushes.Yellow));

        public string HighlightText
        {
            get => (string)GetValue(HighlightTextProperty);
            set => SetValue(HighlightTextProperty, value);
        }

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        private bool _hasHighlights;
        private bool _isUpdating;

        private object OnCoerceTextPropertyValue(object? baseValue)
        {
            if (_isUpdating)
            {
                return baseValue ?? "";
            }

            var highlightText = HighlightText;

            if (string.IsNullOrWhiteSpace(highlightText))
            {
                if (_hasHighlights && baseValue is string s)
                {
                    _isUpdating = true;
                    Inlines.Clear();
                    Inlines.Add(new Run(s));
                    _hasHighlights = false;
                    _isUpdating = false;
                }
            }
            else
            {
                if (baseValue is string s && !string.IsNullOrWhiteSpace(s))
                {
                    highlightText = highlightText.Trim();

                    int searchIndex = 0;

                    int GetNextMatchIndex() => s.IndexOf(highlightText, searchIndex, StringComparison.CurrentCultureIgnoreCase);

                    var matchIndex = GetNextMatchIndex();

                    if (matchIndex == -1)
                    {
                        if (_hasHighlights)
                        {
                            _isUpdating = true;
                            Inlines.Clear();
                            Inlines.Add(new Run(s));
                            _hasHighlights = false;
                            _isUpdating = false;
                        }

                        return baseValue;
                    }

                    _hasHighlights = true;

                    var highlightBrush = HighlightBrush;

                    _isUpdating = true;

                    Inlines.Clear();

                    while (matchIndex != -1)
                    {
                        Inlines.Add(new Run(s.Substring(searchIndex, matchIndex - searchIndex)));
                        Inlines.Add(new Run(s.Substring(matchIndex, highlightText.Length)) { Background = highlightBrush });
                        searchIndex = matchIndex + highlightText.Length;
                        matchIndex = GetNextMatchIndex();
                    }

                    if (searchIndex < s.Length)
                    {
                        Inlines.Add(new Run(s.Substring(searchIndex)));
                    }

                    _isUpdating = false;
                }
            }

            return baseValue ?? "";
        }

        private void OnHighlightTextChanged(object newValue)
        {
            InvalidateProperty(TextProperty);
        }
    }
}