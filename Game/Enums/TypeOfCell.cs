using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Enums
{
    // нужен в частности для графов, можно переделать позже
    public enum TypeOfCell
    {
        // блок по краям карты
        Block1,
        // внутренние блоки карты
        Block2,
        Block3,
        // фонарь
        Lamp,
        //Lamp,
        Building2,
        Building3,
        // свободная клетка
        Free,
        //финишная клетка
        Finish
    };    
}
