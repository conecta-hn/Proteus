/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Windows.Controls;
using static TheXDS.MCART.WpfUi;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Interaction logic for PicturePicker.xaml
    /// </summary>
    public partial class PicturePicker : PathPickerBase
    {
        public PicturePicker()
        {
            InitializeComponent();
            InitializeComponent2();
            Filters.AddRange(new[] {
                new FileFilter("Imágenes", new[] { "jpg", "jpeg", "png", "gif", "bmp" }),
                new FileFilter("Imagen JPEG", new[] { "jpg", "jpeg", "jpe" }),
                new FileFilter("Imagen PNG", "png"),
                new FileFilter("Imagen GIF", "gif"),
                new FileFilter("Imagen BMP", new[] { "bmp", "dib", "rle" }),
                new FileFilter("Imagen TIFF", new[] { "tiff", "tif" }),
            });
        }


        protected override TextBox PathBox => Txt;

        protected override Button BrowseButton => BtnOpen;

        protected override string FileTitle => "imagen";

        protected override string Watermark => "Seleccione una imagen...";

        protected override string Icon => "📷";

        private async void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Img.Source = Uri.TryCreate(Path, UriKind.Absolute,out var u) ? await GetBitmapAsync(u) : null;
        }
    }
}