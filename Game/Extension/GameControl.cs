﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Game.Enums;
using Game.Models;
namespace Game.Extension
{
    // класс расширения 
    // в него входит алгоритм нахождения кратчайшего расстояния между двумя точками A*
    public static partial class GameControl
    {
        public const int square = 4;
        public const int octagon = 8;
        private static int amountOfSidesCell = 4;


        // построение побочной сетки (той, которая будет отображаться)
        // нужно для того, чтобы алгоритм быстрее работал и не включал лишние вычисления
        private static int[,] CreateSubNet(int[,] cellsOfNet, Point BeginNet, int scale)
        {
            int[,] subNet = new int[scale, scale];
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


        // Основной метод вычисления маршрута 
        public static List<Point> FindPath(int[,] cellsOfNet, Point start, Point goal, Point BeginNet, int scale)
        {
            // в случае неверных данных
            if ((start.X > scale) || (start.Y > scale) || (start.X+BeginNet.X >= cellsOfNet.GetLength(0) ) || (start.Y + BeginNet.Y >= cellsOfNet.GetLength(1)))
                return null;
            int[,] subNet = CreateSubNet(cellsOfNet, BeginNet, scale);
            
            if ((subNet[start.X, start.Y] != (int)TypeOfCell.Free) && (subNet[start.X, start.Y] != (int)TypeOfCell.WithMan) && (subNet[start.X, start.Y] != (int)TypeOfCell.Lamp)) 
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
        // Основной метод вычисления маршрута 
        public static List<Point> FindPath(int[,] cellsOfNet, Point start, Point goal, int amountOfSidesCell = 4)
        {
            if ((cellsOfNet[start.X, start.Y] != (int)TypeOfCell.Free) && (cellsOfNet[start.X, start.Y] != (int)TypeOfCell.WithMan) && (cellsOfNet[start.X, start.Y] != (int)TypeOfCell.Finish))
                return null;
            GameControl.amountOfSidesCell = amountOfSidesCell;
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
                foreach (var neighbourNode in GetNeighbours(currentNode, goal, cellsOfNet))
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

        private static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }

        // корень из суммы квадратов для оценки расстояния
        private static double GetHeuristicPathLength(Point from, Point to)
        {
            var dX = Math.Abs(from.X - to.X);
            var dY = Math.Abs(from.Y - to.Y);
            return Math.Sqrt(dX * dX + dY * dY);
        }


        private static Point[] GetneighbourPointsOfSquare(PathNode pathNode)
        {
            Point[] neighbourPoints = new Point[square];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);
            return neighbourPoints;
        }
     

        // Получение списка соседей для точки:
        private static List<PathNode> GetNeighbours(PathNode pathNode, Point goal, int[,] cellsOfNet)
        {
            var result = new List<PathNode>();

            // Соседними точками являются соседние по стороне клетки.
            Point[] neighbourPoints = new Point[amountOfSidesCell];

            if (amountOfSidesCell == square)
                neighbourPoints = GetneighbourPointsOfSquare(pathNode);
            else
                return null;

            foreach (var point in neighbourPoints)
            {
                // Проверяем, что не вышли за границы карты.
                 if ((point.X < 0) || (point.X >= cellsOfNet.GetLength(0)))
                     continue;
                 if ((point.Y < 0) || (point.Y >= cellsOfNet.GetLength(1)))
                     continue;
                // Проверяем, что по клетке можно ходить.
                if ((cellsOfNet[point.X,point.Y] != (int)TypeOfCell.Free) && (cellsOfNet[point.X, point.Y] != (int)TypeOfCell.Lamp) && (cellsOfNet[point.X, point.Y] != (int)TypeOfCell.Finish))
                    continue;
                // если точка не конечнаяя и она -- здание, то по ней нельзя ходить
                if ((point != goal) && (cellsOfNet[point.X, point.Y] == (int)TypeOfCell.Lamp) && (cellsOfNet[point.X, point.Y] == (int)TypeOfCell.Finish))
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
