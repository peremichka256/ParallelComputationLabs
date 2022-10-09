using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PC_Task1
{
    /// <summary>
    /// Класс главной формы с изображением
    /// </summary>
    public partial class MainForm : Form
    {
        private MatrixCompute _picture;

        /// <summary>
        /// Ширина изображения, D
        /// </summary>
        private int D = 13;

        /// <summary>
        /// Высота изображения, M
        /// </summary>
        private int M = 12;

        /// <summary>
        /// Уровень гауссовой матрицы
        /// </summary>
        private int N = 7;

        /// <summary>
        /// Только 1, 2, 4 или 16
        /// </summary>
        private int _streamCount = 4;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _picture = new MatrixCompute(D, M, N, _streamCount);
            Bitmap bmp = new Bitmap(Image.FromFile(@"E:\Рабочий стол\ПВиС\8192x4096.jpg"));

            for (var i = 0; i < _picture.MatrixWidth; i++)
            {
                for (int j = 0; j < _picture.MatrixHeight; j++)
                {
                    _picture.PixelMatrix[i,j].AValue = bmp.GetPixel(i,j).A;
                    _picture.PixelMatrix[i, j].RValue = bmp.GetPixel(i, j).R;
                    _picture.PixelMatrix[i, j].GValue = bmp.GetPixel(i, j).G;
                    _picture.PixelMatrix[i, j].BValue = bmp.GetPixel(i, j).B;
                }
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using (StreamWriter writter = new StreamWriter(@"E:\Рабочий стол\log.txt", true))
            {
                _picture.MatrixCompilation();
                writter.WriteLineAsync($"{ stopWatch.ElapsedMilliseconds}" +
                    $" ms\t{_streamCount} потока");
                writter.Flush();
            }

            pictureBoxMatrix.Width = _picture.MatrixWidth;
            pictureBoxMatrix.Height = _picture.MatrixHeight;
            pictureBoxMatrix.Image = bmp;
            for (var i = 0; i < _picture.MatrixWidth; i++)
            {
                for (int j = 0; j < _picture.MatrixHeight; j++)
                {
                    bmp.SetPixel(i, j, Color.FromArgb(_picture.PixelMatrix[i, j].AValue,
                        _picture.PixelMatrix[i, j].RValue,
                        _picture.PixelMatrix[i, j].GValue,
                        _picture.PixelMatrix[i, j].BValue));
                }
            }
        }
    }
}
