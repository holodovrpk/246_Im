using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace _246_Im
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Student> students = new ObservableCollection<Student>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(dialog.FileName));
                myImg.Source = bitmap;
            }

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap wb = new WriteableBitmap((BitmapSource)myImg.Source);

            int stride = wb.PixelWidth * 4; // Кол-во байтов в строке (4 байта на пиксель — BGRA)

            byte[] pixels = new byte[wb.PixelHeight * stride]; // Буфер для всех пикселей

            wb.CopyPixels(pixels, stride, 0); // Копируем пиксели в массив pixels

            for (int y = 0; y < wb.PixelHeight; y++) // Проход по строкам (y) и столбцам (x)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    // Индекс текущего пикселя в массиве (смещение по y и x)
                    int index = y * stride + x * 4;
                    // Среднее значение трёх каналов RGB
                    //  byte gray = (byte)((pixels[index] + pixels[index + 1] + pixels[index + 2]) / 3);
                    // Присваиваем серое значение всем трём каналам (B, G, R)
                    //  pixels[index] = pixels[index + 1] = pixels[index + 2] = gray;
                    // Alpha (pixels[index + 3]) не трогаем

                    pixels[index+2] = (byte)(255 - pixels[index + 2]);
                   // pixels[index + 1] = (byte)(255 - pixels[index + 1]);
                   // pixels[index] = (byte)(255 - pixels[index]);

                }
            }
            // Записываем изменённые пиксели обратно в изображение
            wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), pixels, stride, 0);
            myImg.Source = wb; // Отображаем результат

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG|*.png|JPEG|*.jpg";

            if (dialog.ShowDialog() == true)
            {
                BitmapEncoder encoder;

                // Проверяем, какой пункт фильтра выбрал пользователь:
                if (dialog.FilterIndex == 1)
                    encoder = new PngBitmapEncoder();   // PNG
                else
                    encoder = new JpegBitmapEncoder();  // JPEG


                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)myImg.Source));
                using FileStream stream = new FileStream(dialog.FileName, FileMode.Create);
                encoder.Save(stream);
            }

        }
    }
}