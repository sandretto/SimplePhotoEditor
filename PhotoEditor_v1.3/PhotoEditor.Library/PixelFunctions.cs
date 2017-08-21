using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PhotoEditor.Library
{
    public class PixelFunctions
    {
        #region Constants

        private const double gammaCoefficient = 0.0039215686274509803921568627451; // ~= 1/255
        private const float k = 32.0f * 0.0011805097869175733145636819401f;
        private const float k_1 = 1.0f / k; //7,96875

        #endregion Constants

        #region Utilities

        /// <summary>
        /// Ограничение на значение цвета
        /// </summary>
        public static int Normalize(int color)
        {
            if (color > 255) color = 255;
            if (color < 0) color = 0;
            return color;
        }

        public static float Normalize(float color)
        {
            if (color > 255.0f) color = 255.0f;
            if (color < 0.0f) color = 0.0f;
            return color;
        }

        public static double Normalize(double color)
        {
            if (color > 255.0) color = 255.0;
            if (color < 0.0) color = 0.0;
            return color;
        }

        #endregion Utilities
        
        #region Grade Correction

        /// <summary>
        /// Делегат, в который передаем корректирующую функцию
        /// </summary>
        public delegate float CorrFunction(int color);

        public static Color ApplyCorrection(Color pixel, Func<int, float> fn)
        {
            return Color.FromArgb(Convert.ToInt32(fn(pixel.R)),
                Convert.ToInt32(fn(pixel.G)), Convert.ToInt32(fn(pixel.B)));
        }

        public static float CorrBySin(int color)
        {
            color = Normalize(color);
            float result =
                (127.5f) * (float)Math.Sin(Math.PI *
                0.0039215686274509803921568627451 * color - Math.PI * 0.5) + (127.5f);
            result = Normalize(result);
            return result;
        }

        public static float CorrByExp(int color)
        {
            color = Normalize(color);
            float result = (float)Math.Exp(k * color) - 1.0f;
            result = Normalize(result);
            return result;
        }

        public static float CorrByLog(int color)
        {
            color = Normalize(color);
            float result = (float)Math.Log(color + 1.0f) * k_1;
            result = Normalize(result);
            return result;
        }

        #endregion Grade Correction

        #region Gamma Correction

        public static Color ChangeGamma(Color pixel, double g)
        {
            return Color.FromArgb(ChangeGamma(pixel.R, g),
                ChangeGamma(pixel.G, g),
                ChangeGamma(pixel.B, g));
        }

        public static int ChangeGamma(int color, double gamma)
        {
            double result = Math.Pow(color * gammaCoefficient, gamma) * 255.0;
            result = Normalize(result);
            return Convert.ToInt16(result);
        }

        #endregion Gamma Correction

        #region Gray Filter

        /// <summary>
        /// Применение серого фильтра
        /// </summary>
        public static Color ApplyGrayFilter(Color pixel)
        {
            int avg = (pixel.R + pixel.G + pixel.B + 1) / 3;
            return Color.FromArgb(avg, avg, avg);
        }

        #endregion Gray Filter

        #region Brightness Filter

        /// <summary>
        /// Изменение яркости
        /// </summary>
        public static Color ChangeBrightness(Color pixel, double p)
        {
            return Color.FromArgb(ChangeBrightness(pixel.R, p),
                ChangeBrightness(pixel.G, p),
                ChangeBrightness(pixel.B, p));
        }

        private static int ChangeBrightness(int color, double percent)
        {
            double p = percent * 0.01;
            int result = Convert.ToInt16(color + p * 128.0);
            result = Normalize(result);
            return result;
        }

        #endregion Brightness Filter

        #region Contrast Filter

        public static Color ChangeContrast(Color pixel, int percent)
        {
            float p = percent * 0.01f;
            return Color.FromArgb(ChangeContrast(pixel.R, p),
               ChangeContrast(pixel.G, p),
               ChangeContrast(pixel.B, p));
        }

        private static int ChangeContrast(int color, float p)
        {
            int result;
            if (p < 0f) result = Convert.ToInt16(color + p * (color - 128));
            else
                result = Convert.ToInt16(128.0f + (color - 128.0f) / (1.0f - p));
            result = Normalize(result);
            return result;
        }

        #endregion Contrast Filter

        #region Color Balance

        public static Color ChangeColor(Color pixel, float percent, Func<Color, float, Color> fn)
        {
            return fn(pixel, percent);
        }

        public static Color ChangeRed(Color pixel, float percent)
        {
            float p = percent * 0.1f;
            return Color.FromArgb(ChangeBrightness(pixel.R, p),
                pixel.G,
                pixel.B);
        }

        public static Color ChangeGreen(Color pixel, float percent)
        {
            float p = percent * 0.1f;
            return Color.FromArgb(
                pixel.R,
                ChangeBrightness(pixel.G, p),
                pixel.B);
        }

        public static Color ChangeBlue(Color pixel, float percent)
        {
            float p = percent * 0.1f;
            return Color.FromArgb(pixel.R,
                pixel.G,
                ChangeBrightness(pixel.B, p));
        }

        #endregion Color Balance

        #region Vignette

        public static void PaintVignette(ref Bitmap result, ref int[] colors)
        {
            Graphics g = Graphics.FromImage(result);
            Rectangle bounds = new Rectangle(0, 0, result.Width, result.Height);
            //прямоугольник для вписания эллипса
            Rectangle ellipsebounds = bounds;
            //смещение границ прямоугольника
            ellipsebounds.Offset(-ellipsebounds.X, -ellipsebounds.Y);
            int x = ellipsebounds.Width - (int)Math.Round(0.70712 * ellipsebounds.Width);
            int y = ellipsebounds.Height - (int)Math.Round(0.70712 * ellipsebounds.Height);
            //увеличиваем границы прямоугольника на заданную величину
            ellipsebounds.Inflate(x, y);
            GraphicsPath gpath = new GraphicsPath();
            gpath.AddEllipse(ellipsebounds);
            PathGradientBrush brush = new PathGradientBrush(gpath)
            {
                WrapMode = WrapMode.Tile,
                CenterColor = Color.FromArgb(0, 0, 0, 0),
                SurroundColors = new Color[]
            {
                Color.FromArgb(255, colors[0], colors[1], colors[2])
            }
            };
            //шаблон смешивания
            Blend blend = new Blend()
            {
                Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f },
                Factors = new float[] { 0.0f, 0.5f, 1f, 1f, 1.0f, 1.0f }
            };
            brush.Blend = blend;
            Region oldClip = g.Clip;
            g.Clip = new Region(bounds);
            g.FillRectangle(brush, ellipsebounds);
            g.Clip = oldClip;
        }

        #endregion Vignette
    }
}