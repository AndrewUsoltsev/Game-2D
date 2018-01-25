using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Enums
{
    /// <summary>
    /// Тип клетки
    /// </summary>
    public enum TypeOfCell
    {
        /// <summary>
        /// Препятствие, расположенное по краям карты
        /// </summary>
        Block1,
        /// <summary>
        /// Внутренние препятствия карты
        /// </summary>
        Block2,
        /// <summary>
        /// Факел
        /// </summary>
        Lamp,
        /// <summary>
        /// Свободная клетка
        /// </summary>
        Free,
        /// <summary>
        /// Финишная клетка
        /// </summary>
        Finish
    };    
}
