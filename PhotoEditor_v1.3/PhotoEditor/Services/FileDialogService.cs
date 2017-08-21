using Microsoft.Win32;
using System.Windows.Controls;
using _Bitmap = System.Drawing.Bitmap;

namespace PhotoEditor.Services
{
    public interface IFileDialogService<T>
    {
        T Open();

        void Save(T file);
    }

    public class PictureFileDialogService : IFileDialogService<_Bitmap>
    {
        private string path;
        private Image image;

        public PictureFileDialogService(Image image)
        {
            this.image = image;
        }

        /// <summary>
        /// Чтение картинки из файла
        /// </summary>
        public _Bitmap Open()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                if (openFile.FileName != null)
                {
                    path = openFile.FileName;
                }
            }
            _Bitmap bmp = new _Bitmap(path);
            FileService.AddBitmapToImage(ref image, bmp);
            return bmp;
        }

        /// <summary>
        /// Сохранить картинку
        /// </summary>
        public void Save(_Bitmap bmp)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                DefaultExt = "png",
                Filter = "Рисунки (*.png)|*.png|Все файлы (*.*)|*.*"
            };
            if (saveFile.ShowDialog() == true)
            {
                bmp.Save(saveFile.FileName,
                       System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}