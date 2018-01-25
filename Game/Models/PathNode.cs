using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game.Models
{
    /// <summary>
    /// Структура, используемая в алгоритме A*
    /// </summary>
    public class PathNode
    {
        /// <summary>
        /// Координаты точки на карте. 
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Длина пути от старта (G). 
        /// </summary>
        public int PathLengthFromStart { get; set; }

        /// <summary>
        /// Точка, из которой пришли в эту точку. 
        /// </summary>
        public PathNode CameFrom { get; set; }

        /// <summary>
        /// Примерное расстояние до цели (H).
        /// </summary>
        public double HeuristicEstimatePathLength { get; set; }

        /// <summary>
        /// Ожидаемое полное расстояние до цели (F).
        /// </summary>
        public double EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }
}
