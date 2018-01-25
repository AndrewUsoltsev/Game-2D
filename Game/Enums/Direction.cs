using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Enums
{
    /// <summary>
    /// Направление
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Вверх
        /// </summary>
        UP,
        /// <summary>
        /// Вниз
        /// </summary>
        DOWN,
        /// <summary>
        /// Вправо
        /// </summary>
        LEFT,
        /// <summary>
        /// Влево
        /// </summary>
        RIGHT,
        /// <summary>
        /// Верхний правый угол
        /// </summary>
        UpperRightCorner,
        /// <summary>
        /// Остальные направления
        /// </summary>
        OTHER
    }
}
