using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Task1
{
    /// <summary>
    /// Класс создания и расчета матрицы изображения
    /// </summary>
    public class MatrixCompute
    {
        /// <summary>
        /// Размерность одного блока
        /// </summary>
        const int BLOCK_SIZE = 4;

        /// <summary>
        /// Количество столбцов матрицы
        /// </summary>
        private int _matrixWidth;

        /// <summary>
        /// Количество строк матрицы
        /// </summary>
        private int _matrixHeight;

        /// <summary>
        /// Уровень гауссовой пирамиды
        /// </summary>
        private int _pyramidLevel;

        /// <summary>
        /// Матрица пикселей
        /// </summary>
        private int[,] _pixelMatrix;

        /// <summary>
        /// Задаёт и возвращает количество столбцов матрицы
        /// </summary>
        public int MatrixWidth
        { 
            get { return _matrixWidth; }
            set { _matrixWidth = value; }
        }

        /// <summary>
        /// Задает и возвращает оличество строк матрицы
        /// </summary>
        public int MatrixHeight
        {
            get { return _matrixHeight; }
            set { _matrixHeight = value; }
        }

        /// <summary>
        /// Задает и возврашает уровень гауссовой пирамиды
        /// </summary>
        public int PyramidLevel
        {
            get { return _pyramidLevel; }
            set 
            { 
                if(value < 0)
                {
                    throw new ArgumentException("Wrong pyramid level");
                }
                _pyramidLevel = value; 
            }
        }

        /// <summary>
        /// Задаёт и возвращает матрицу пикселей
        /// </summary>
        public int[,] PixelMatrix
        {
            get { return _pixelMatrix; }
            set { _pixelMatrix = value; }
        }

        /// <summary>
        /// Создает расширенную матрицу
        /// </summary>
        public void MatrixCompilation()
        {
            while (_pyramidLevel > 1)
            {
                //Создается рамка вокруг начальной матрицы
                var extendedMatrix = new int[MatrixWidth + 2, MatrixHeight + 2];
                extendedMatrix[0, 0] = PixelMatrix[0, 0];
                extendedMatrix[MatrixWidth + 1, MatrixHeight + 1] =
                    PixelMatrix[MatrixWidth - 1, MatrixHeight - 1];
                extendedMatrix[0, MatrixHeight+1] = PixelMatrix[0, MatrixHeight - 1];
                extendedMatrix[MatrixWidth+1, 0] =
                    PixelMatrix[MatrixWidth - 1, 0];

                for (var i = 0; i < MatrixWidth; i++)
                {
                    extendedMatrix[i+1, 0] = _pixelMatrix[i, 0];
                    extendedMatrix[i + 1, MatrixHeight + 1] 
                        = _pixelMatrix[i, MatrixHeight - 1];
                }

                for (var i = 0; i < MatrixHeight; i++)
                {
                    extendedMatrix[0, i+1] = _pixelMatrix[0, i];
                    extendedMatrix[MatrixWidth + 1, i + 1] 
                        = _pixelMatrix[MatrixWidth - 1, i];
                }

                //Перенос данных их старой матрицы в расширенную

                for (var i = 0; i < MatrixWidth; i++)
                {
                    for (var j = 0; j < MatrixHeight; j++)
                    {
                        extendedMatrix[i+1,j+1] = _pixelMatrix[i, j];
                    }
                }

                ComputeReducedLevel(extendedMatrix);
                MatrixWidth /= 2;
                MatrixHeight /= 2;
                PyramidLevel--;
            }
            return;
        }

        /// <summary>
        /// Уменьшает изначальную матриц в два раза
        /// и заполняет усредненными ARGB
        /// </summary>
        /// <param name="extendedMatrix"></param>
        public void ComputeReducedLevel(int[,] extendedMatrix)
        {
            var reducedMatrix = new int[MatrixWidth / 2,
                MatrixHeight / 2];

            for (var i = 0; i < MatrixWidth; i+=2)
            {
                for (var j = 0; j < MatrixHeight; j+=2)
                {

                    //Рассчитывается среднее значение в блоке
                    var averageBlockValue = 0;
                    for (var k = i; k < i + BLOCK_SIZE; k++)
                    {
                        for (var m = j; m < j + BLOCK_SIZE; m++)
                        {
                            averageBlockValue += extendedMatrix[k, m];
                        }
                    }
                    averageBlockValue /= (BLOCK_SIZE * BLOCK_SIZE);
                    reducedMatrix[i/2, j/2] = averageBlockValue;    
                }
            }

            _pixelMatrix = reducedMatrix;
        }

        /// <summary>
        /// Конструктор объекта матрицы
        /// </summary>
        /// <param name="matrixWidth">Ширина изображения</param>
        /// <param name="matrixHeight">Высота изображения</param>
        /// <param name="pyramidLevel">Уровень уменьшения</param>
        public MatrixCompute(int matrixWidth, int matrixHeight, int pyramidLevel)
        {
            MatrixWidth = (int)Math.Pow(2, matrixWidth);
            MatrixHeight = (int)Math.Pow(2, matrixHeight);
            PyramidLevel = pyramidLevel;
            PixelMatrix = new int[MatrixWidth, MatrixHeight];
        }
    }
}
