using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game.Models
{
    public class PathNode
    {
        // Координаты точки на карте.
        public Point Position { get; set; }

        // Длина пути от старта (G).
        public int PathLengthFromStart { get; set; }

        // Точка, из которой пришли в эту точку.
        public PathNode CameFrom { get; set; }

        // Примерное расстояние до цели (H).
        public double HeuristicEstimatePathLength { get; set; }

        // Ожидаемое полное расстояние до цели (F).
        public double EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }
}
