using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using _Bitmap = System.Drawing.Bitmap;
using _ImagingInterop = System.Windows.Interop.Imaging;

namespace PhotoEditor.Services
{
    public static class FileService
    {
        /// <summary>
        /// Добавление картинки на Image
        /// </summary>

        public static void AddBitmapToImage(ref Image image, _Bitmap b)
        {
            image.Source = _ImagingInterop.CreateBitmapSourceFromHBitmap(b.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}