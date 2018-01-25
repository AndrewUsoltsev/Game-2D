using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Enums
{
    
    /// <summary>
    /// Обозначния текстур
    /// </summary>
    public enum Textures
    {
        /// <summary>
        /// Препятствие
        /// </summary>
        Block = 0,
        /// <summary>
        /// Персонаж прозрачный
        /// </summary>
        TransparentCharacter = 1,
        /// <summary>
        /// Персонаж текущий
        /// </summary>
        CurrentCharactert = 2,  
        /// <summary>
        /// Факел
        /// </summary>
        Toarch = 3, 
        /// <summary>
        /// Трава Днем
        /// </summary>
        GrassDay = 4, 
        /// <summary>
        /// Трава Ночью
        /// </summary>
        GrassNight = 5
    }
}
