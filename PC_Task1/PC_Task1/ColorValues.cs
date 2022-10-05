using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Task1
{
    /// <summary>
    /// Класс с значениями ARGB 
    /// </summary>
    public class ColorValues
    {
        /// <summary>
        /// Поле со значением альфа-компонента
        /// </summary>
        private int _aValue;

        /// <summary>
        /// Поле со значением красного компонента
        /// </summary>
        private int _rValue;

        /// <summary>
        /// Поле со значением зеленого компанента
        /// </summary>
        private int _gValue;

        /// <summary>
        /// Поле со значением голубого компанента
        /// </summary>
        private int _bValue;

        /// <summary>
        /// Задаёт/возвращает альфа-компонент
        /// </summary>
        public int AValue
        {
            get => _aValue;
            set => _aValue = value;
        }

        /// <summary>
        /// Задаёт/возвращает красный компонент
        /// </summary>
        public int RValue
        {
            get => _rValue;
            set => _rValue = value;
        }

        /// <summary>
        /// Задаёт/возвращает зеленый компонент
        /// </summary>
        public int GValue
        {
            get => _gValue;
            set => _gValue = value;
        }

        /// <summary>
        /// Задаёт/возвращает синий компонент
        /// </summary>
        public int BValue
        {
            get => _bValue;
            set => _bValue = value;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ColorValues()
        {
            AValue = 0;
            RValue = 0; 
            GValue = 0; 
            BValue = 0;
        }
    }
}
