using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private int D = 12;

        /// <summary>
        /// Высота изображения, M
        /// </summary>
        private int M = 11;

        /// <summary>
        /// Уровень гауссовой матрицы
        /// </summary>
        private int N = 5;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _picture = new MatrixCompute(D, M, N);
            Bitmap bmp = new Bitmap(Image.FromFile(@"E:\Рабочий стол\ПВиС\456128.jpg"));

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
            pictureBoxMatrix.Width = _picture.MatrixWidth;
            pictureBoxMatrix.Height = _picture.MatrixHeight;
            pictureBoxMatrix.Image = bmp;

            _picture.MatrixCompilation();

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
