using PhotoEditor.Services;
using PhotoEditor.Library;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using PhotoEditor.Models;

namespace PhotoEditor.ViewModels
{
    public class PictureViewModel : INotifyPropertyChanged
    {
        #region Fields

        private System.Drawing.Bitmap bmp;
        private System.Drawing.Bitmap buffer;
        private Visibility elementVisible = Visibility.Hidden;
        private CustomFilter filter;
        private Image image;
        private RelayCommand openCommand;
        private RelayCommand saveCommand;
        private RelayCommand getHelpCommand;
        private RelayCommand undoCommand;
        private RelayCommand applyFilterCommandGrade;
        private RelayCommand applyFilterCommandGamma;
        private RelayCommand applyFilterGray;
        private RelayCommand applyFilterBrightness;
        private RelayCommand applyFilterContrast;
        private RelayCommand applyFilterColorBalance;
        private RelayCommand applyFilterCommandVignette;
        private bool cbItemSineCorrection;
        private bool cbItemExpCorrection;
        private bool cbItemLogCorrection;
        private bool _checked;
        private double sliderValueGamma;
        private double sliderValueBright;
        private double sliderValueContrast;
        private double sliderValueRed;
        private double sliderValueGreen;
        private double sliderValueBlue;
        private double sliderValueRedV;
        private double sliderValueGreenV;
        private double sliderValueBlueV;
        private bool blackVignetteChecked;
        private PictureFileDialogService dialogService;
        private Stack<RelayCommand> commands;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Инициализация модели
        /// </summary>
        public PictureViewModel(Image image)
        {
            this.image = image;
            commands = new Stack<RelayCommand>();
        }

        #endregion Constructor

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string v = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        #endregion Events

        #region Properties

        public Image SelectedImage
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged("SelectedImage");
            }
        }

        public bool CbItemSineCorrection
        {
            get { return cbItemSineCorrection; }
            set
            {
                cbItemSineCorrection = value;
                OnPropertyChanged("CbItemSineCorrection");
            }
        }

        public bool CbItemExpCorrection
        {
            get { return cbItemExpCorrection; }
            set
            {
                cbItemExpCorrection = value;
                OnPropertyChanged("CbItemExpCorrection");
            }
        }

        public bool CbItemLogCorrection
        {
            get { return cbItemLogCorrection; }
            set
            {
                cbItemLogCorrection = value;
                OnPropertyChanged("CbItemLogCorrection");
            }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                OnPropertyChanged("Checked");
            }
        }

        public double SliderValueGamma
        {
            get { return sliderValueGamma; }
            set
            {
                sliderValueGamma = value;
                OnPropertyChanged("SliderValueGamma");
            }
        }

        public double SliderValueBright
        {
            get { return sliderValueBright; }
            set
            {
                sliderValueBright = value;
                OnPropertyChanged("SliderValueBright");
            }
        }

        public double SliderValueContrast
        {
            get { return sliderValueContrast; }
            set
            {
                sliderValueContrast = value;
                OnPropertyChanged("SliderValueContrast");
            }
        }

        public double SliderValueRed
        {
            get { return sliderValueRed; }
            set
            {
                sliderValueRed = value;
                OnPropertyChanged("SliderValueRed");
            }
        }

        public double SliderValueGreen
        {
            get { return sliderValueGreen; }
            set
            {
                sliderValueGreen = value;
                OnPropertyChanged("SliderValueGreen");
            }
        }

        public double SliderValueBlue
        {
            get { return sliderValueBlue; }
            set
            {
                sliderValueBlue = value;
                OnPropertyChanged("SliderValueBlue");
            }
        }

        public double SliderValueRedV
        {
            get { return sliderValueRedV; }
            set
            {
                sliderValueRedV = value;
                OnPropertyChanged("SliderValueRedV");
            }
        }

        public double SliderValueGreenV
        {
            get { return sliderValueGreenV; }
            set
            {
                sliderValueGreenV = value;
                OnPropertyChanged("SliderValueGreenV");
            }
        }

        public double SliderValueBlueV
        {
            get { return sliderValueBlueV; }
            set
            {
                sliderValueBlueV = value;
                OnPropertyChanged("SliderValueBlueV");
            }
        }

        public bool BlackVignetteChecked
        {
            get { return blackVignetteChecked; }
            set
            {
                blackVignetteChecked = value;
                OnPropertyChanged("BlackVignetteChecked");
            }
        }

        public Visibility ElementVisible
        {
            get { return elementVisible; }
            set
            {
                elementVisible = value;
                OnPropertyChanged("ElementVisible");
            }
        }

        #endregion Properties

        #region Commands

        /// <summary>
        /// Команда "Открыть файл"
        /// </summary>
        public RelayCommand OpenCommand => openCommand ??
                    (openCommand = new RelayCommand(obj =>
                    {
                        OpenFile();
                    }));

        /// <summary>
        /// Команда "Сохранить в файл"
        /// </summary>
        public RelayCommand SaveCommand => saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {
                      SaveToFile();
                  }));

        /// <summary>
        /// Команда "О программе"
        /// </summary>
        public RelayCommand GetHelpCommand => getHelpCommand ??
                  (getHelpCommand = new RelayCommand(obj =>
                  {
                      GetInfo();
                  }));
        
        /// <summary>
        /// Команда "Отмена"
        /// </summary>
        public RelayCommand UndoCommand => undoCommand ??
                  (undoCommand = new RelayCommand(obj =>
                  {
                      ApplyUndo();
                  }));

        /// <summary>
        /// Команда "Применение градационной коррекции"
        /// </summary>
        public RelayCommand ApplyGradeFilterCommand =>
            applyFilterCommandGrade ??
            (applyFilterCommandGrade = new RelayCommand(obj =>
            {
                ApplyGradeCorrection();
            }));
        
        /// <summary>
        /// Команда "Применение гамма-коррекции"
        /// </summary>
        public RelayCommand ApplyFilterCommandGamma =>
            applyFilterCommandGamma ??
            (applyFilterCommandGamma = new RelayCommand(obj =>
            {
                ApplyGammaCorrection();
            }));

        /// <summary>
        /// Команда "Применение черно-белого фильтра"
        /// </summary>
        public RelayCommand ApplyFilterGray =>
            applyFilterGray ??
            (applyFilterGray = new RelayCommand(obj =>
            {
                if (Checked)
                    ApplyGrayFilter();
                else
                {
                    bmp = buffer;
                    ApplyUndo();
                }
            }));

        /// <summary>
        /// Команда "Изменение яркости"
        /// </summary>
        public RelayCommand ApplyFilterCommandBrightness =>
            applyFilterBrightness ??
            (applyFilterBrightness = new RelayCommand(obj =>
            {
                ApplyBrightnessFilter();
            }));

        /// <summary>
        /// Команда "Изменение контраста"
        /// </summary>
        public RelayCommand ApplyFilterCommandContrast =>
            applyFilterContrast ??
            (applyFilterContrast = new RelayCommand(obj =>
            {
                ApplyContrastFilter();
            }));

        /// <summary>
        /// Команда "Изменение цветового баланса"
        /// </summary>
        public RelayCommand ApplyFilterCommandColorBalance =>
            applyFilterColorBalance ??
            (applyFilterColorBalance = new RelayCommand(obj =>
            {
                ApplyColorBalanceFilter();
            }));

        /// <summary>
        /// Команда "Применение виньетирования"
        /// </summary>
        public RelayCommand ApplyFilterCommandVignette =>
            applyFilterCommandVignette ??
            (applyFilterCommandVignette = new RelayCommand(obj =>
            {
                ApplyVignette();
            }));

        #endregion Commands

        #region Methods

        private void OpenFile()
        {
            try
            {
                dialogService = new PictureFileDialogService(image);
                System.Drawing.Bitmap _base = dialogService.Open();
                buffer = _base;
                bmp = _base;
                ElementVisible = Visibility.Visible;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Вы забыли загрузить изображение", "Упс!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + ex.Message + " " + ex.StackTrace);
            }
        }

        private void SaveToFile()
        {
            try
            {
                dialogService = new PictureFileDialogService(image);
                dialogService.Save(bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private static void GetInfo()
        {
            try
            {
                MessageBox.Show("Простой фоторедактор с применением шаблона MVVM");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyUndo()
        {
            try
            {
                image.Source = null;
                bmp = buffer;
                FileService.AddBitmapToImage(ref image, bmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyGradeCorrection()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (CbItemSineCorrection)
                {
                    filter.ApplyGradeCorrection(PixelFunctions.CorrBySin);
                }
                else if (CbItemExpCorrection)
                {
                    filter.ApplyGradeCorrection(PixelFunctions.CorrByExp);
                }
                else if (CbItemLogCorrection)
                {
                    filter.ApplyGradeCorrection(PixelFunctions.CorrByLog);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyGammaCorrection()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (SliderValueGamma != 0)
                {
                    filter.ApplyGamma(SliderValueGamma);
                }
                else
                {
                    FileService.AddBitmapToImage(ref image, bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyGrayFilter()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                filter.ChangeToGray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyBrightnessFilter()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (sliderValueBright != 0)
                {
                    filter.ChangeBrightness(sliderValueBright);
                }
                else
                {
                    FileService.AddBitmapToImage(ref image, bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyContrastFilter()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (sliderValueContrast != 0)
                {
                    filter.ChangeContrast(sliderValueContrast);
                }
                else
                {
                    FileService.AddBitmapToImage(ref image, bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyColorBalanceFilter()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (sliderValueRed != 0 || sliderValueGreen != 0 || sliderValueBlue != 0)
                {
                    if (sliderValueRed != 0)
                    {
                        filter.ChangeColorBalance(sliderValueRed, PixelFunctions.ChangeRed);
                    }
                    if (sliderValueGreen != 0)
                    {
                        filter.ChangeColorBalance(sliderValueRed, PixelFunctions.ChangeGreen);
                    }
                    if (sliderValueBlue != 0)
                    {
                        filter.ChangeColorBalance(sliderValueRed, PixelFunctions.ChangeBlue);
                    }
                }
                else
                {
                    FileService.AddBitmapToImage(ref image, bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private void ApplyVignette()
        {
            try
            {
                filter = new CustomFilter(bmp, image);
                if (sliderValueRedV != 0 || sliderValueGreenV != 0 || sliderValueBlueV != 0)
                {
                    filter.PaintVignette( new double[] 
                    { SliderValueRedV, SliderValueGreenV, SliderValueBlueV });
                }
                else if (BlackVignetteChecked)
                {
                    filter.PaintVignette(new double[]
                    { 0, 0, 0 });
                }
                else
                {
                    FileService.AddBitmapToImage(ref image, bmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion Methods
    }
}