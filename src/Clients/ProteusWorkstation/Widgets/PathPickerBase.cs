/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Widgets
{
    public abstract class PathPickerBase : UserControl
    {
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register(
            nameof(Editable), typeof(bool), typeof(FilePicker), new PropertyMetadata(true));
        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(
            nameof(Filters), typeof(List<FileFilter>),
            typeof(FilePicker), new PropertyMetadata(new List<FileFilter>()));
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            nameof(Path), typeof(string), typeof(FilePicker), new PropertyMetadata(string.Empty));

        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }
        public List<FileFilter> Filters
        {
            get => (List<FileFilter>)GetValue(FiltersProperty);
            set => SetValue(FiltersProperty, value);
        }
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        private protected static void Bind(PathPickerBase i, FrameworkElement f, params DependencyProperty[] p)
        {
            Bind(i, f, p.Select(q => (q, q)).ToArray());
        }

        private protected static void Bind(PathPickerBase i, FrameworkElement f, params (DependencyProperty From, DependencyProperty To)[] p)
        {
            foreach (var j in p)
            {
                f.SetBinding(j.To, new Binding
                {
                    Path = new PropertyPath(j.From),
                    Source = i,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            }
        }

        private protected static IEnumerable<string> Decompose(FileFilter f)
        {
            if (!f.Description.IsEmpty()) yield return f.Description;
            yield return string.Join(";", f.Extensions);
        }

        protected void BtnOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var o = new OpenFileDialog
            {
                Title = $"Seleccionar {FileTitle}",
                Filter = string.Join("|", Filters.SelectMany(Decompose)).OrNull() ?? "Todos los archivos|*",
                CheckFileExists = true,
                Multiselect = false,
                DereferenceLinks = true,
                CheckPathExists = true,
            };
            if (o.ShowDialog() ?? false) Path = o.FileName;
        }

        protected virtual string FileTitle { get; } = "archivo";
        protected virtual string Watermark { get; } = "Seleccione un archivo...";
        protected virtual string Icon { get; } = "📄";
        protected abstract TextBox PathBox { get; }
        protected abstract Button BrowseButton { get; }
        protected void InitializeComponent2()
        {
            ProteusProp.SetWatermark(this, Watermark);
            ProteusProp.SetIcon(this, Icon);
            Bind(this, PathBox, ProteusProp.WatermarkProperty, ProteusProp.IconProperty);
            Bind(this, PathBox, (PathProperty, TextBox.TextProperty));
            Bind(this, PathBox, (EditableProperty, IsEnabledProperty));
            BrowseButton.Click += BtnOpen_OnClick;
        }
    }
}