using PhotoEditor.ViewModels;
using System.Windows;

namespace PhotoEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PictureView : Window
    {
        public PictureView()
        {
            InitializeComponent();
            DataContext = new PictureViewModel(image);
        }
    }
}