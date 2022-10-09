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
        /// Количество потоков
        /// </summary>
        private int _streamCount;

        /// <summary>
        /// Переменная определяющая на каком индексе 
        /// разделять матрицу по горизонтали
        /// </summary>
        private int _horizontalSplitter;

        /// <summary>
        /// Переменная определяющая на каком индексе 
        /// разделять матрицу по вертикали
        /// </summary>
        private int _verticalSplitter;

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
        private ColorValues[,] _pixelMatrix;

        /// <summary>
        /// Количество потоков
        /// </summary>
        private int StreamCount
        {
            get => _streamCount;
            set 
            { 
                if(value < 0)
                {
                    throw new ArgumentException("Wrong stream count");
                }
                _streamCount = value; 
            }
        }

        /// <summary>
        /// Свойство определяющие горизонтальный 
        /// разделитель в зависимости от потоков
        /// </summary>
        private int HorizontalSplitter
        {
            get => _horizontalSplitter;
            set
            {
                if(value == 1)
                {
                    _horizontalSplitter = 1;
                }
                else if (value == 2)
                {
                    _horizontalSplitter = 2;
                }
                else if (value == 4)
                {
                    _horizontalSplitter = 2;
                }
                else if (value == 16)
                {
                    _horizontalSplitter = 4;
                }
            }
        }

        /// <summary>
        /// Свойство определяющие вертикальный 
        /// разделитель в зависимости от потоков
        /// </summary>
        private int VerticalSplitter
        {
            get => _verticalSplitter;
            set
            {
                if (value == 1)
                {
                    _verticalSplitter = 1;
                }
                if (value == 2)
                {
                    _verticalSplitter = 1;
                }
                else if (value == 4)
                {
                    _verticalSplitter = 2;
                }
                else if (value == 16)
                {
                    _verticalSplitter = 4;
                }
            }
        }

        /// <summary>
        /// Задаёт и возвращает количество столбцов матрицы
        /// </summary>
        public int MatrixWidth
        { 
            get => _matrixWidth;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Wrong matrix width");
                }
                _matrixWidth = value;
            }
        }

        /// <summary>
        /// Задает и возвращает оличество строк матрицы
        /// </summary>
        public int MatrixHeight
        {
            get => _matrixHeight;
            set 
            {
                if(value < 0)
                {
                    throw new ArgumentException("Wrong matrix height");
                }
                _matrixHeight = value; 
            }
        }

        /// <summary>
        /// Задает и возврашает уровень гауссовой пирамиды
        /// </summary>
        public int PyramidLevel
        {
            get => _pyramidLevel;
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
        public ColorValues[,] PixelMatrix
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
                var extendedMatrix = new ColorValues[MatrixWidth + 2, MatrixHeight + 2];
                extendedMatrix[0, 0] = PixelMatrix[0, 0];
                extendedMatrix[MatrixWidth + 1, MatrixHeight + 1] =
                    PixelMatrix[MatrixWidth - 1, MatrixHeight - 1];
                extendedMatrix[0, MatrixHeight+1] = PixelMatrix[0, MatrixHeight - 1];
                extendedMatrix[MatrixWidth+1, 0] =
                    PixelMatrix[MatrixWidth - 1, 0];

                for (var i = 0; i < MatrixWidth; i++)
                {
                    extendedMatrix[i + 1, 0] = _pixelMatrix[i, 0];
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
        public void ComputeReducedLevel(ColorValues[,] extendedMatrix)
        {
            var reducedMatrix = new ColorValues[MatrixWidth / 2,
                MatrixHeight / 2];

            for (var i = 0; i < MatrixWidth / 2; i++)
            {
                for (var j = 0; j < MatrixHeight / 2; j++)
                {
                    reducedMatrix[i, j] = new ColorValues();
                }
            }

            if (StreamCount == 1)
            {
                //Один поток
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, 0);
            }
            else if (StreamCount == 2)
            {
                //Два потока
                Task task1 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, 0));
                Task task2 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, 0));

                task1.Wait();
                task2.Wait();
            }
            else if (StreamCount == 4)
            {
                //Четыре потока
                Task task1 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, 0));
                Task task2 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, MatrixHeight / 2));
                Task task3 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, 0));
                Task task4 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2,
                    MatrixHeight / 2));

                task1.Wait();
                task2.Wait();
                task3.Wait();
                task4.Wait();
            }
            else if (StreamCount == 16)
            {
                //Шестнадцать потоков
                Task task1 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, 0));
                Task task2 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, (MatrixHeight / 4) * 1));
                Task task3 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, MatrixHeight / 2));
                Task task4 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, 0, (MatrixHeight / 4) * 3));

                Task task5 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 1, 0));
                Task task6 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 1, (MatrixHeight / 4) * 1));
                Task task7 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 1, MatrixHeight / 2));
                Task task8 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 1, (MatrixHeight / 4) * 3));

                Task task9 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, 0));
                Task task10 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, (MatrixHeight / 4) * 1));
                Task task11 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, MatrixHeight / 2));
                Task task12 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, MatrixWidth / 2, (MatrixHeight / 4) * 3));

                Task task13 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 3, 0));
                Task task14 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 3, (MatrixHeight / 4) * 1));
                Task task15 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 3, MatrixHeight / 2));
                Task task16 = Task.Factory.StartNew(() =>
                CalculateAverageValues(extendedMatrix, reducedMatrix, (MatrixWidth / 4) * 3, (MatrixHeight / 4) * 3));

                task1.Wait();
                task2.Wait();
                task3.Wait();
                task4.Wait();
                task5.Wait();
                task6.Wait();
                task7.Wait();
                task8.Wait();
                task9.Wait();
                task10.Wait();
                task11.Wait();
                task12.Wait();
                task13.Wait();
                task14.Wait();
                task15.Wait();
                task16.Wait();
            }

            _pixelMatrix = reducedMatrix;
        }

        /// <summary>
        /// Метод расчитывающий 
        /// </summary>
        /// <param name="extendedMatrix"></param>
        /// <param name="reducedMatrix"></param>
        /// <param name="indexI"></param>
        /// <param name="indexJ"></param>
        private void CalculateAverageValues(ColorValues[,] extendedMatrix, 
            ColorValues[,] reducedMatrix, int indexI, int indexJ)
        {
            for (var i = indexI; i < indexI + (MatrixWidth / HorizontalSplitter); i += 2)
            {
                for (var j = indexJ; j < indexJ + (MatrixHeight / VerticalSplitter); j += 2)
                {
                    //Рассчитывается среднее значение в блоке
                    var averageA = 0;
                    var averageR = 0;
                    var averageG = 0;
                    var averageB = 0;
        
                    for (var k = i; k < i + BLOCK_SIZE; k++)
                    {
                        for (var m = j; m < j + BLOCK_SIZE; m++)
                        {
                            averageA += extendedMatrix[k, m].AValue;
                            averageR += extendedMatrix[k, m].RValue;
                            averageG += extendedMatrix[k, m].GValue;
                            averageB += extendedMatrix[k, m].BValue;
                        }
                    }
        
                    averageA /= (BLOCK_SIZE * BLOCK_SIZE);
                    averageR /= (BLOCK_SIZE * BLOCK_SIZE);
                    averageG /= (BLOCK_SIZE * BLOCK_SIZE);
                    averageB /= (BLOCK_SIZE * BLOCK_SIZE);
        
                    reducedMatrix[i / 2, j / 2].AValue = averageA;
                    reducedMatrix[i / 2, j / 2].RValue = averageR;
                    reducedMatrix[i / 2, j / 2].GValue = averageG;
                    reducedMatrix[i / 2, j / 2].BValue = averageB;
                }
            }
        }

        /// <summary>
        /// Конструктор объекта матрицы
        /// </summary>
        /// <param name="matrixWidth">Ширина изображения</param>
        /// <param name="matrixHeight">Высота изображения</param>
        /// <param name="pyramidLevel">Уровень уменьшения</param>
        public MatrixCompute(int matrixWidth, int matrixHeight, int pyramidLevel, int streamCount)
        {
            MatrixWidth = (int)Math.Pow(2, matrixWidth);
            MatrixHeight = (int)Math.Pow(2, matrixHeight);
            PyramidLevel = pyramidLevel;
            StreamCount = streamCount;
            HorizontalSplitter = StreamCount;
            VerticalSplitter = StreamCount;
            PixelMatrix = new ColorValues[MatrixWidth, MatrixHeight];

            for (var i = 0; i < MatrixWidth; i++)
            {
                for (var j = 0; j < MatrixHeight; j++)
                {
                    PixelMatrix[i, j] = new ColorValues();
                }
            }
            StreamCount = streamCount;
        }
    }
}
