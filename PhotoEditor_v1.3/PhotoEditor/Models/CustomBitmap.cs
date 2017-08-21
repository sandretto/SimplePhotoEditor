using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace PhotoEditor.Models
{
    public class CustomBitmap
    {
        #region Fields

        private Bitmap bmp;
        private BitmapData data;
        private IntPtr iPointer;

        #endregion Fields

        #region Properties

        public byte[] ArrayOfPixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Bitmap GetBitmap => bmp;
        public IntPtr GetHBitmap => iPointer;

        #endregion Properties

        #region Constructor

        public CustomBitmap(Bitmap bmp)
        {
            this.bmp = bmp;
            iPointer = IntPtr.Zero;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Блокировка CustomBitmap в системной памяти
        /// </summary>
        public void LockBits()
        {
            try
            {
                Width = bmp.Width;
                Height = bmp.Height;

                int pixelCount = Width * Height;
                Rectangle rect = new Rectangle(0, 0, Width, Height);
                Depth = Image.GetPixelFormatSize(bmp.PixelFormat);
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    MessageBox.Show("Поддерживаются только 8, 24, 32-bpp изображения");
                }
                data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                int step = (int)(Depth * 0.125);
                ArrayOfPixels = new byte[pixelCount * step];
                iPointer = data.Scan0;
                Marshal.Copy(iPointer, ArrayOfPixels, 0, ArrayOfPixels.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Разблокировка CustomBitmap в системной памяти
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                Marshal.Copy(ArrayOfPixels, 0, iPointer, ArrayOfPixels.Length);
                bmp.UnlockBits(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Получение пикселя
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            Color color = Color.Empty;
            int colorComponents = (int)(Depth * 0.125);
            int i = ((y * Width) + x) * colorComponents;
            byte a, r, g, b;
            try
            {
                if (i > ArrayOfPixels.Length - colorComponents)
                {
                    throw new IndexOutOfRangeException();
                }
                switch (Depth)
                {
                    case 32:
                        b = ArrayOfPixels[i];
                        g = ArrayOfPixels[i + 1];
                        r = ArrayOfPixels[i + 2];
                        a = ArrayOfPixels[i + 3];
                        color = Color.FromArgb(a, r, g, b);
                        break;

                    case 24:
                        b = ArrayOfPixels[i];
                        g = ArrayOfPixels[i + 1];
                        r = ArrayOfPixels[i + 2];
                        color = Color.FromArgb(r, g, b);
                        break;

                    case 8:
                        b = ArrayOfPixels[i];
                        color = Color.FromArgb(b, b, b);
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            return color;
        }

        /// <summary>
        /// Установка пикселя
        /// </summary>
        public void SetPixel(int x, int y, Color color)
        {
            int colorComponents = (int)(Depth * 0.125);
            int i = ((y * Width) + x) * colorComponents;

            try
            {
                switch (Depth)
                {
                    case 32:
                        ArrayOfPixels[i] = color.B;
                        ArrayOfPixels[i + 1] = color.G;
                        ArrayOfPixels[i + 2] = color.R;
                        ArrayOfPixels[i + 3] = color.A;
                        break;

                    case 24:
                        ArrayOfPixels[i] = color.B;
                        ArrayOfPixels[i + 1] = color.G;
                        ArrayOfPixels[i + 2] = color.R;
                        break;

                    case 8:
                        ArrayOfPixels[i] = color.B;
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        #endregion Methods
    }
}