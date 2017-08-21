using PhotoEditor.Library;
using PhotoEditor.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using _Bitmap = System.Drawing.Bitmap;
using _Image = System.Windows.Controls.Image;

namespace PhotoEditor.Models
{
    public class CustomFilter
    {
        private _Bitmap bmp;
        private _Bitmap result;
        private CustomBitmap myBitmap;
        private _Image image;

        public CustomFilter(_Bitmap bmp, _Image image)
        {
            this.image = image;
            this.bmp = bmp;
        }
        
        /// <summary>
        /// Изменить яркость
        /// </summary>
        public void ChangeBrightness(double value)
        {
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    pixel = PixelFunctions.ChangeBrightness(pixel, value);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }

        private void Init()
        {
            result = new _Bitmap(bmp);
            myBitmap = new CustomBitmap(result);
            myBitmap.LockBits();
        }

        private void GetResult()
        {
            result = myBitmap.GetBitmap;
            bmp = result;
            myBitmap.UnlockBits();
            FileService.AddBitmapToImage(ref image, bmp);
        }

        /// <summary>
        /// Изменить контраст
        /// </summary>
        public void ChangeContrast(double value)
        {
            int valueInt = Convert.ToInt32(value);
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    pixel = PixelFunctions.ChangeContrast(pixel, valueInt);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }
        
        /// <summary>
        /// Применить гамма-коррекцию
        /// </summary>
        public void ApplyGamma(double gamma)
        {
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    if (gamma != 100.0)
                        pixel = PixelFunctions.ChangeGamma(pixel, gamma);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }
        
        /// <summary>
        /// Применить градационную коррекцию
        /// </summary>
        public void ApplyGradeCorrection(Func<int, float> corrFunction)
        {
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    pixel = PixelFunctions.ApplyCorrection(pixel, corrFunction);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }
        
        /// <summary>
        /// Применить черно-белый фильтр
        /// </summary>
        public void ChangeToGray()
        {
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    pixel = PixelFunctions.ApplyGrayFilter(pixel);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }

        /// <summary>
        /// Изменить цветовой баланс
        /// </summary>
        public void ChangeColorBalance(double value, Func<Color, float, Color> change)
        {
            float percent = (float)value * 10.0f;
            Init();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = myBitmap.GetPixel(x, y);
                    pixel = PixelFunctions.ChangeColor(pixel, percent, change);
                    myBitmap.SetPixel(x, y, pixel);
                }
            }
            GetResult();
        }
        
        /// <summary>
        /// Нарисовать виньетку
        /// </summary>
        public void PaintVignette(double[] values)
        {
            result = new _Bitmap(bmp);
            int[] colors;
            int r = 0, g = 0, b = 0;

            r = (int)(values[0] * 15.0);
            g = (int)(values[1] * 15.0);
            b = (int)(values[2] * 15.0);

            colors = new int[] { r, g, b };
            PixelFunctions.PaintVignette(ref result, ref colors);
            bmp = result;
            FileService.AddBitmapToImage(ref image, bmp);
        }
    }
}
