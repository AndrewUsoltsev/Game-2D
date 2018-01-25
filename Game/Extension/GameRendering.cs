using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.DevIl;
using Game.Models;
using Game.Enums;

namespace Game.Extension
{
    // Первый аргумент по X, второй аргумент по Y
    /// <summary>
    /// Класс расширения
    /// Отрисовка объектов
    /// </summary>
    public static class GameRendering
    {
        private static SimpleOpenGlControl Scene;
        private static int scaleX = 1; 
        private static int scaleY = 1;

        #region поля для отображения текстур
        /// <summary>
        /// Блок препятствие
        /// </summary>
        public static uint mGlTextureObject0 = 0;
        /// <summary>
        /// Прозрачный персонаж 
        /// </summary>
        public static uint mGlTextureObject1 = 0;
        /// <summary>
        /// Текущий персонаж 
        /// </summary>
        public static uint mGlTextureObject2 = 0;
        /// <summary>
        /// Факел
        /// </summary>
        public static uint mGlTextureObject3 = 0;
        /// <summary>
        /// Трава днем
        /// </summary>
        public static uint mGlTextureObject4 = 0;
        /// <summary>
        /// Трава ночью
        /// </summary>
        public static uint mGlTextureObject5 = 0; 
        /// <summary>
        /// Загружена ли текстура
        /// </summary>
        public static bool textureIsLoad = false;
#endregion 

        #region отрисовка сетки



        // дополнительный метод для отрисовки сетки
        private static bool RenderTypeOfCell(Net net, Point testPoint, Point beginGraph, bool isNigth)
        {
            if (net[testPoint.X, testPoint.Y] == TypeOfCell.Free)
                RenderSquareGraphPoint(beginGraph);
            else if (net[testPoint.X, testPoint.Y] == TypeOfCell.Block1)
                RenderFillSquareGraphPoint(beginGraph,0);
            else if (net[testPoint.X, testPoint.Y] == TypeOfCell.Block2)
                RenderFillSquareGraphPoint(beginGraph,0);
            else if (net[testPoint.X, testPoint.Y] == TypeOfCell.Finish)
                RenderFinishSquareGraphPoint(beginGraph);
            else if (net[testPoint.X, testPoint.Y] == TypeOfCell.Lamp)
                return true;
            return false;
        }

        /// <summary>
        /// Отрисовка сетки и ячеек в сетке 
        /// </summary>
        /// <param name="net">Отрисовываемая сетка</param>
        /// <param name="first">Начальное положение отрисовываемой части сетки</param>
        /// <param name="Scene">Графическая область для отрисовки</param>
        /// <param name="scale">Масштаб сетки</param>
        /// <param name="isNigth">Текущее время суток (true — ночь, false — день)</param>
        public static void RenderNet(Net net, Point first, SimpleOpenGlControl Scene, int scale, bool isNigth)
        {
            GameRendering.Scene = Scene; 
            int N = net.N;
            scaleX = Scene.Width / scale;
            scaleY = Scene.Height / scale;
            List<Point> Lamps = new List<Point>();

            RenderField(Scene, isNigth);
            for (int i = 0; i < scale; i++)
            {
                Point beginGraph = new Point(0, i * scaleY);
                for (int j = 0; j < scale; j++)
                {
                    Gl.glColor3ub(255, 0, 0);
                    // первый аргумент по Х, второй по Y
                    if (((first.X + j < N) && (first.Y + i < N)) && ((first.X + j >= 0) && (first.Y + i >= 0)))
                    {
                        if (RenderTypeOfCell(net, new Point(first.X + j, first.Y + i), beginGraph, isNigth))
                            Lamps.Add(beginGraph);
                        beginGraph.X += scaleX;
                    }
                    else
                        break;
                }
            }

            // отдельно факелы
            foreach (Point lampCoord in Lamps)
                RenderLampAndLightGraphPoint(lampCoord, Textures.Toarch);

        }


        #endregion


        /// <summary>
        /// Перевод графической точки в точку на алгоритмической сетке
        /// </summary>
        /// <param name="x">Абсцисса точки</param>
        /// <param name="y">Ордината точки</param>
        /// <returns></returns>
        public static Point GraphPointToAlgorithmPoint(int x, int y)
        {
            int Nx = x / scaleX; // число клеток по х
            int Ny = y / scaleY; // число клеток по y
            return new Point(Nx, Ny);
        }

        /// <summary>
        /// отрисовка поля по всей области
        /// </summary>
        /// <param name="Scene">области отрисовки</param>
        /// <param name="isNigth">текущее время суток (true — ночь, false — день)</param>
        public static void RenderField(SimpleOpenGlControl Scene, bool isNigth)
        {
            int height = Scene.Height;
            int width = Scene.Width;

            // если текстура загружена
            if (textureIsLoad)
            {
                // очищение текущей матрицы 
                Gl.glLoadIdentity();

                // включаем режим текстурирования
                Gl.glEnable(Gl.GL_TEXTURE_2D);

                if (!isNigth)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject4);
                else
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject5);

                // включаем режим текстурирования , указывая индификатор mGlTextureObject

                // сохраняем состояние матрицы
                Gl.glPushMatrix();


                // отрисовываем полигон
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);

                Gl.glTexCoord2f(0, 0);
                Gl.glVertex2d(0, 0);

                Gl.glTexCoord2f(1, 0);
                Gl.glVertex2d(width, 0);

                Gl.glTexCoord2f(1, 1);
                Gl.glVertex2d(width, height);

                Gl.glTexCoord2f(0, 1);
                Gl.glVertex2d(0, height);

                // завершаем отрисовку
                Gl.glEnd();

                // возвращаем матрицу
                Gl.glPopMatrix();
                // отключаем режим текстурирования
                Gl.glDisable(Gl.GL_TEXTURE_2D);

                // обновлеям элемент со сценой
                Scene.Invalidate();

            }
        }

        /// <summary>
        /// Отрисовка клетки, куда было произведено нажатие мыши, 
        /// </summary>
        /// <param name="x">Абсцисса точки</param>
        /// <param name="y">Ордината точки</param>
        /// <param name="type">Тип отрисовываемой текстуры</param>
        /// <returns>Возвращается точка на алгоритмической сетки</returns>
        public static Point RenderMouseClickGraphPoint(int x,int y, Textures type)
        {
            if (x >= 0 && y >= 0)
            {
              Point tmp = GraphPointToAlgorithmPoint(x, y);
              Gl.glColor3ub(0, 0, 0);
              RenderFillSquareGraphPoint(new Point(scaleX * tmp.X, scaleY * tmp.Y), type);
              return tmp;
            }
            return new Point();
        }

        /// <summary>
        /// Отрисовка клетки, куда было произведено нажатие мыши, на алгоритмической сетке 
        /// </summary>
        /// <param name="x">Абсцисса точки</param>
        /// <param name="y">Ордината точки</param>
        /// <param name="type">Тип отрисовываемой текстуры</param>
        public static void RenderMouseClickAlgorithmPoint(int x, int y, Textures type)
        {
           if (x >= 0 && y >= 0)
           {
             Gl.glColor3ub(0, 0, 0);
             RenderFillSquareGraphPoint(new Point(scaleX * x, scaleY * y), type);
           }
        }

        #region отрисовка ячеек

        /// <summary>
        /// Отрисовка финишной клетки
        /// </summary>
        /// <param name="begin">Координаты финишной клетки</param>
        public static void RenderFinishSquareGraphPoint(Point begin) //свободная клетка
        {

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glColor3ub(0, 50, 100);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X, begin.Y + scaleY);
            Gl.glVertex2d(begin.X + scaleX, begin.Y + scaleY);
            Gl.glVertex2d(begin.X + scaleX, begin.Y);
            Gl.glEnd();

            RenderSquareGraphPoint(begin);

        }
        private static void RenderSquareGraphPoint(Point begin) //свободная клетка
        {

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glColor3ub(0, 255, 0);
            Gl.glVertex2d(begin.X, begin.Y + scaleY);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X + scaleX, begin.Y);
            Gl.glEnd();

        }

        //препятствие
        private static void RenderFillSquareGraphPoint(Point begin, Textures type) 
        {
            // если текстура загружена
            if (textureIsLoad)
            {
                // очищение текущей матрицы 
                Gl.glLoadIdentity();

                // включаем режим текстурирования
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                // включаем режим текстурирования , указывая индификатор mGlTextureObject
                switch( type)
                {
                    case Textures.Block:
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject0);
                        break;
                    case Textures.TransparentCharacter:
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject1);
                        break;
                    case Textures.CurrentCharactert:
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject2);
                        break;
                    case Textures.Toarch:
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject3);
                        break;
                }

                // сохраняем состояние матрицы
                Gl.glPushMatrix();


                // отрисовываем полигон
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);

                Gl.glTexCoord2f(0, 0);
                Gl.glVertex2d(begin.X, begin.Y);

                Gl.glTexCoord2f(1, 0);
                Gl.glVertex2d(begin.X + scaleX, begin.Y);

                Gl.glTexCoord2f(1, 1);
                Gl.glVertex2d(begin.X + scaleX, begin.Y + scaleY);

                Gl.glTexCoord2f(0, 1);
                Gl.glVertex2d(begin.X, begin.Y + scaleY);

                // завершаем отрисовку
                Gl.glEnd();

                // возвращаем матрицу
                Gl.glPopMatrix();
                // отключаем режим текстурирования
                Gl.glDisable(Gl.GL_TEXTURE_2D);

                // обновлеям элемент со сценой
                Scene.Invalidate();
            }

        }


        /// <summary>
        /// Отрисовка факела
        /// </summary>
        /// <param name="beginGraph">Начальное положение отрисовки (графические координаты)</param>
        /// <param name="type">Тип отрисовываемой текстуры</param>
        public static void RenderLampAndLightGraphPoint(Point beginGraph, Textures type)
        {
            Gl.glColor3ub(0, 255, 0);
            RenderFillSquareGraphPoint(beginGraph,type);
            Gl.glColor4ub(255, 255, 0, 122);
            Point tmp = new Point(beginGraph.X + (int)(scaleX * 0.5), beginGraph.Y + (int)(scaleY * 0.5));
            DrawCircle(tmp, scaleX * 2.5f, scaleY * 2.5f);

        }
        #endregion
        private static void DrawCircle(Point begin, float rx,float ry)
        {
            int amountSegments = 360;
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);

            for (int i = 0; i < amountSegments; i++)
            {
                double angle = 2.0 * Math.PI * i / amountSegments;

                float dx = rx * (float)Math.Cos(angle);
                float dy = ry * (float)Math.Sin(angle);

                Gl.glVertex2f(begin.X + dx, begin.Y + dy);
            }

            Gl.glEnd();
        }

        /// <summary>
        /// Отрисовка пути через центры ячеек пути
        /// </summary>
        /// <param name="way">Путь, который необходимо отрисовать</param>
        public static void RenderWayByAlgorithmPoint(List<Point> way)
        {
            if (way != null)
            {
                Gl.glColor3ub(255, 0, 122);
                Gl.glEnable(Gl.GL_LINE_STIPPLE);
                Gl.glLineWidth(3);
                Gl.glLineStipple(1, 0x00F0); // штрихи
                for (int i = 0; i < way.Count - 1; i++)
                    RenderLineBetweenCell(way[i], way[i + 1]);
                Gl.glLineWidth(1);
                Gl.glDisable(Gl.GL_LINE_STIPPLE);
            }
        }
        
        /// <summary>
        /// Отрисовка линии между соседними ячейками
        /// Для использования требуются координаты "алгоритмической" сетки (не пиксельные)
        /// </summary>
        /// <param name="begin">Исходная ячейка, из которой должны провести линию</param>
        /// <param name="end">Конечная ячейка</param>
        private static void RenderLineBetweenCell(Point begin, Point end)
        {
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(scaleX * (begin.X + 0.5) , scaleY * (begin.Y + 0.5));
            Gl.glVertex2d(scaleX * (end.X + 0.5), scaleY * (end.Y + 0.5));
            Gl.glEnd();
        }

    }
}


