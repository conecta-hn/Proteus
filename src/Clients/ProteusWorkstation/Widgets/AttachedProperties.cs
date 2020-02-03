/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheXDS.Proteus.Widgets
{
    public static class ProteusProp
    {
        private static readonly DependencyProperty VerticalScrollBindingProperty = DependencyProperty.RegisterAttached("VerticalScrollBinding", typeof(bool?), typeof(ProteusProp));
        private static readonly DependencyProperty HorizontalScrollBindingProperty = DependencyProperty.RegisterAttached("HorizontalScrollBinding", typeof(bool?), typeof(ProteusProp));
        public static readonly DependencyProperty AccentProperty = DependencyProperty.RegisterAttached("Accent", typeof(Brush), typeof(ProteusProp), new PropertyMetadata(Application.Current?.FindResource("CorpColor")));
        public static readonly DependencyProperty BusyProperty = DependencyProperty.RegisterAttached("Busy", typeof(bool), typeof(ProteusProp), new PropertyMetadata(false));
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(string), typeof(ProteusProp), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextAccentProperty = DependencyProperty.RegisterAttached("TextAccent", typeof(Brush), typeof(ProteusProp), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty TextPressAccentProperty = DependencyProperty.RegisterAttached("TextPressAccent", typeof(Brush), typeof(ProteusProp), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty WarnedProperty = DependencyProperty.RegisterAttached("Warned", typeof(bool), typeof(ProteusProp), new PropertyMetadata(false));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ProteusProp), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty WatermarkAlwaysVisibleProperty = DependencyProperty.RegisterAttached("WatermarkAlwaysVisible", typeof(bool), typeof(ProteusProp), new PropertyMetadata(false));
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ProteusProp), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVerticalOffsetPropertyChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(ProteusProp), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHorizontalOffsetPropertyChanged));
        public static double GetVerticalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(VerticalOffsetProperty);
        }
        public static void SetVerticalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(VerticalOffsetProperty, value);
        }
        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (d)
            {
                case ScrollViewer scrollViewer:
                    BindVerticalOffset(scrollViewer);
                    scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
                    break;
                case ScrollContentPresenter scrollPresenter:
                    scrollPresenter.SetVerticalOffset((double)e.NewValue);

                    break;
            }
        }
        public static void BindVerticalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(VerticalScrollBindingProperty) != null) return;

            scrollViewer.SetValue(VerticalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.VerticalChange == 0)
                    return;
                SetVerticalOffset(scrollViewer, se.VerticalOffset);
            };
        }


        public static double GetHorizontalOffset(UIElement depObj)
        {
            return (double)depObj.GetValue(HorizontalOffsetProperty);
        }
        public static void SetHorizontalOffset(UIElement depObj, double value)
        {
            depObj.SetValue(HorizontalOffsetProperty, value);
        }


        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (d)
            {
                case ScrollViewer scrollViewer:
                    //BindHorizontalOffset(scrollViewer);
                    scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
                    break;
                case ScrollContentPresenter scrollPresenter:
                    scrollPresenter.SetHorizontalOffset((double)e.NewValue);
                    break;
            }
        }
        public static void BindHorizontalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(HorizontalScrollBindingProperty) != null) return;

            scrollViewer.SetValue(HorizontalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.HorizontalChange == 0)
                    return;
                SetHorizontalOffset(scrollViewer, se.HorizontalOffset);
            };
        }
        public static Brush GetAccent(UIElement element)
        {
            return (Brush)element.GetValue(AccentProperty);
        }
        public static void SetAccent(UIElement element, Brush value)
        {
            element.SetValue(AccentProperty, value);
        }
        public static bool GetBusy(UIElement element)
        {
            return (bool)element.GetValue(BusyProperty);
        }
        public static void SetBusy(UIElement element, bool value)
        {
            element.SetValue(BusyProperty, value);
        }
        public static string GetIcon(UIElement element)
        {
            return (string)element.GetValue(IconProperty);
        }
        public static void SetIcon(UIElement element, string value)
        {
            element.SetValue(IconProperty, value);
        }
        public static Brush GetTextAccent(UIElement element)
        {
            return (Brush)element.GetValue(TextAccentProperty);
        }
        public static void SetTextAccent(UIElement element, Brush value)
        {
            element.SetValue(TextAccentProperty, value);
        }
        public static Brush GetTextPressAccent(UIElement element)
        {
            return (Brush)element.GetValue(TextPressAccentProperty);
        }
        public static void SetTextPressAccent(UIElement element, Brush value)
        {
            element.SetValue(TextPressAccentProperty, value);
        }
        public static bool GetWarned(UIElement element)
        {
            return (bool)element.GetValue(WarnedProperty);
        }
        public static void SetWarned(UIElement element, bool value)
        {
            element.SetValue(WarnedProperty, value);
        }
        public static string GetWatermark(UIElement element)
        {
            return (string)element.GetValue(WatermarkProperty);
        }
        public static void SetWatermark(UIElement element, string value)
        {
            element.SetValue(WatermarkProperty, value);
        }
        public static bool GetWatermarkAlwaysVisible(UIElement element)
        {
            return (bool)element.GetValue(WatermarkAlwaysVisibleProperty);
        }
        public static void SetWatermarkAlwaysVisible(UIElement element, bool value)
        {
            element.SetValue(WatermarkAlwaysVisibleProperty, value);
        }
    }
}