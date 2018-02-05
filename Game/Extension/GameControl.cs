using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Game.Enums;
using Game.Models;
namespace Game.Extension
{
    /// <summary>
    /// Класс расширения 
    /// Алгоритм нахождения кратчайшего расстояния между двумя точками — A*
    /// </summary>

    public static partial class GameControl
    {
        private const int amountOfSidesCell = 4;

        // построение побочной сетки (той, которая будет отображаться)
        // нужно для того, чтобы алгоритм быстрее работал и не включал лишние вычисления
        private static TypeOfCell[,] CreateSubNet(TypeOfCell[,] cellsOfNet, Point BeginNet, int scale)
        {
            TypeOfCell[,] subNet = new TypeOfCell[scale, scale];
            Point begin = new Point(BeginNet.X,BeginNet.Y);
            if (cellsOfNet.GetLength(0) - (BeginNet.X + scale) < 0)
                begin.X = cellsOfNet.GetLength(0) - scale;
            if (cellsOfNet.GetLength(1) - (BeginNet.Y + scale) < 0)
                begin.X = cellsOfNet.GetLength(0) - scale;

            for (int i = 0; i < scale; i++)
                for (int j = 0; j < scale; j++)
                    subNet[j, i] = cellsOfNet[begin.X + j, begin.Y + i];
            return subNet;
        }


        /// <summary>
        /// Основной метод вычисления маршрута, кратчайшего пути
        /// </summary>
        /// <param name="cellsOfNet">Исходная сетка, на которой ищей кратчайший путь</param>
        /// <param name="start">Клетка, от которой начинаем поиск пути</param>
        /// <param name="goal">Клетка, до которой ищем путь</param>
        /// <param name="BeginNet">Начальное положение отображаемой сетки</param>
        /// <param name="scale">Масштаб отображаемой области, на которой ищется кратчайший путь</param>
        /// <returns>Возвращает найденный путь, список вершин</returns>
        public static List<Point> FindPath(TypeOfCell[,] cellsOfNet, Point start, Point goal, Point BeginNet, int scale)
        {
            // в случае неверных данных
            if ((start.X > scale) || (start.Y > scale) || (start.X+BeginNet.X >= cellsOfNet.GetLength(0) ) || (start.Y + BeginNet.Y >= cellsOfNet.GetLength(1)))
                return null;
            TypeOfCell[,] subNet = CreateSubNet(cellsOfNet, BeginNet, scale);
            
            if ((subNet[start.X, start.Y] != TypeOfCell.Free)  && (subNet[start.X, start.Y] != TypeOfCell.Lamp)) 
                return null;
            // Шаг 1.
            var closedSet = new List<PathNode>();
            var openSet = new List<PathNode>();
            // Шаг 2.
            var startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                // Шаг 3.
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();
                // Шаг 4.
                if (currentNode.Position == goal)
                    return GetPathForNode(currentNode);
                // Шаг 5.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                // Шаг 6.
                foreach (var neighbourNode in GetNeighbours(currentNode, goal, subNet))
                {
                    // Шаг 7.
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    var openNode = openSet.FirstOrDefault(node =>
                      node.Position == neighbourNode.Position);
                    // Шаг 8.
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else
                      if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        // Шаг 9.
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            // Шаг 10.
            return null;
        }

        #region не используемая оптимизация пути
        private static Direction DeterminationOfPositionOfPoint(Point begin, Point end)
        {
            // интересуют 3 направления
            if (end.Y > begin.Y && begin.X == end.X)
                return Direction.Up;
            if (end.X > begin.X)
                if (end.Y > begin.Y)
                    return Direction.UpperRightCorner;
                else
                    return Direction.Right;
            return Direction.Other;

        }

       private static void AddPointToOptimizedPath(TypeOfCell[,] cellsOfNet, ref List<Point> previousWay, Direction direction, Point refPoint)
       {
           switch (direction)
           {
               case Direction.Right:
                   previousWay.Add(new Point(refPoint.X + 1, refPoint.Y));
                   break;
               case Direction.Up:
                   previousWay.Add(new Point(refPoint.X, refPoint.Y+1));
                   break;
               case Direction.UpperRightCorner:
                    // считаем, что препятствия не соприкасаются по диагонали
                    if (cellsOfNet[refPoint.X + 1, refPoint.Y] == TypeOfCell.Free)
                        previousWay.Add(new Point(refPoint.X + 1, refPoint.Y));
                    else if (cellsOfNet[refPoint.X, refPoint.Y + 1] == TypeOfCell.Free)
                        previousWay.Add(new Point(refPoint.X, refPoint.Y + 1));
                    previousWay.Add(new Point(refPoint.X + 1, refPoint.Y + 1));
                    break;
            }
       }

        /// <summary>
        /// Основной метод вычисления маршрута, кратчайшего пути
        /// </summary>
        /// <param name="previousWay">Предыдущий найденный путь</param>
        /// <param name="cellsOfNet">Исходная сетка, на которой ищей кратчайший путь</param>
        /// <param name="start">Клетка, от которой начинаем поиск пути</param>
        /// <param name="goal">Клетка, до которой ищем путь</param>
        /// <param name="BeginNet">Начальное положение отображаемой сетки</param>
        /// <param name="scale">Масштаб отображаемой области, на которой ищется кратчайший путь</param>
        /// <returns>Возвращает найденный путь, список вершин</returns>
        public static List<Point> OptimizedFindPath(List<Point> previousWay, TypeOfCell[,] cellsOfNet, Point start, Point goal, Point BeginNet, int scale)
        {
            // в случае неверных данных
            if ((start.X > scale) || (start.Y > scale) || (start.X + BeginNet.X >= cellsOfNet.GetLength(0)) || (start.Y + BeginNet.Y >= cellsOfNet.GetLength(1)))
                return null;

            if (cellsOfNet[goal.X + BeginNet.X, goal.Y + BeginNet.Y] == TypeOfCell.Block1 || cellsOfNet[goal.X + BeginNet.X, goal.Y + BeginNet.Y] == TypeOfCell.Block2) 
                return null;

            // условие для выхода с препятствий
            if (previousWay?.Count > 1)
            {
                // рассматриваем предыдущую точку
                int index = previousWay.Count - 1;
                Point refPoint = previousWay[index];
                if (previousWay[index].X > start.X && previousWay[index].Y > start.Y)
                {
                    Direction direction = DeterminationOfPositionOfPoint(refPoint, goal);
                    if (direction != Direction.Other)
                    {
                        AddPointToOptimizedPath(cellsOfNet, ref previousWay, direction, refPoint);
                        return previousWay;
                    }
                }
            }
            return FindPath(cellsOfNet, start, goal, BeginNet, scale);
        }

#endregion

        private static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }

        // корень из суммы квадратов для оценки расстояния
        private static double GetHeuristicPathLength(Point from, Point to)
        {
            var dX = Math.Abs(from.X - to.X);
            var dY = Math.Abs(from.Y - to.Y);
            return  Math.Sqrt(dX * dX + dY * dY);
        }


        private static Point[] GetneighbourPointsOfSquare(PathNode pathNode)
        {
            Point[] neighbourPoints = new Point[amountOfSidesCell];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);
            return neighbourPoints;
        }
     

        // Получение списка соседей для точки:
        private static List<PathNode> GetNeighbours(PathNode pathNode, Point goal, TypeOfCell[,] cellsOfNet)
        {
            var result = new List<PathNode>();

            // Соседними точками являются соседние по стороне клетки.
            Point[] neighbourPoints = new Point[amountOfSidesCell];
            neighbourPoints = GetneighbourPointsOfSquare(pathNode);

            foreach (var point in neighbourPoints)
            {
                // Проверяем, что не вышли за границы карты.
                 if ((point.X < 0) || (point.X >= cellsOfNet.GetLength(0)))
                     continue;
                 if ((point.Y < 0) || (point.Y >= cellsOfNet.GetLength(1)))
                     continue;
                // Проверяем, что по клетке можно ходить.
                if ((cellsOfNet[point.X,point.Y] != TypeOfCell.Free) && (cellsOfNet[point.X, point.Y] != TypeOfCell.Lamp) && (cellsOfNet[point.X, point.Y] != TypeOfCell.Finish))
                    continue;
                // если точка не конечнаяя и она -- здание, то по ней нельзя ходить
                if ((point != goal) && (cellsOfNet[point.X, point.Y] == TypeOfCell.Lamp) && (cellsOfNet[point.X, point.Y] == TypeOfCell.Finish))
                    continue;
                // Заполняем данные для точки маршрута.
                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        // Получения маршрута. Маршрут представлен в виде списка координат точек
        private static List<Point> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        private static List<Point> GetPathForNode(PathNode pathNode, Point beginNet)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();

            return result;
        }

        
    }
}
