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
    // TODO 3d координаты??? 
    // Первый аргумент по X, второй аргумент по Y
    public static class GameRendering
    {
        private static SimpleOpenGlControl Scene;
        private static int scaleX = 1; 
        private static int scaleY = 1;

        public static uint mGlTextureObject0 = 0; //блок препятствие
        public static uint mGlTextureObject1 = 0; // персонаж прозрачный
        public static uint mGlTextureObject2 = 0; // персонаж текущий
        public static uint mGlTextureObject3 = 0; // факел
        public static uint mGlTextureObject4 = 0; // трава днем
        public static uint mGlTextureObject5 = 0; // трава ночью
        public static bool textureIsLoad = false;


        #region отрисовка сетки



        // дополнительный метод для отрисовки сетки
        private static bool RenderTypeOfCell(Net net, Point testPoint, Point beginGraph, bool isNigth)
        {
            if (net[testPoint.X, testPoint.Y] == TypeOfCell.Free)
                RenderSquareGraphPoint(beginGraph, isNigth);
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

        // отрисовка сетки и ячеек в сетке
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
        

        // перевод графической точки в точку на алгоритмической сетке
        public static Point GraphPointToAlgorithmPoint(int x, int y)
        {
            int Nx = x / scaleX; // число клеток по х
            int Ny = y / scaleY; // число клеток по y
            return new Point(Nx, Ny);
        }

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
                Gl.glBegin(Gl.GL_QUADS);

                Gl.glTexCoord2f(0, 0);
                Gl.glVertex3d(0, 0, 0);
                Gl.glTexCoord2f(1, 0);
                Gl.glVertex3d(width, 0, 0);
                Gl.glTexCoord2f(1, 1);
                Gl.glVertex3d(width, height, 0);
                Gl.glTexCoord2f(0, 1);
                Gl.glVertex3d(0, height, 0);

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
        // отрисовка клетки, куда было произведено нажатие мыши, 
        // возвращается точка на алгоритмической сетки
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

        // отрисовка клетки, куда было произведено нажатие мыши, на алгоритмической сетке 
        public static void RenderMouseClickAlgorithmPoint(int x, int y, Textures type)
        {
           if (x >= 0 && y >= 0)
           {
             Gl.glColor3ub(0, 0, 0);
             RenderFillSquareGraphPoint(new Point(scaleX * x, scaleY * y), type);
           }
        }

        #region отрисовка ячеек
        // засовываем сюда координаты на сцене
        // начиная с левого нижнего угла
        public static void RenderFinishSquareGraphPoint(Point begin) //свободная клетка
        {

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glColor3ub(0, 50, 100);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X, begin.Y + scaleY);
            Gl.glVertex2d(begin.X + scaleX, begin.Y + scaleY);
            Gl.glVertex2d(begin.X + scaleX, begin.Y);
            Gl.glEnd();


            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glColor3ub(0, 255, 0);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X + scaleX, begin.Y);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glColor3ub(0, 255, 0);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X, begin.Y + scaleY);
            Gl.glEnd();

        }
        public static void RenderSquareGraphPoint(Point begin, bool isNight) //свободная клетка
        {

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glColor3ub(0, 255, 0);
            Gl.glVertex2d(begin.X, begin.Y + scaleY);
            Gl.glVertex2d(begin.X, begin.Y);
            Gl.glVertex2d(begin.X + scaleX, begin.Y);
            Gl.glEnd();

        }
        private static void RenderFillSquareGraphPoint(Point begin, Textures type) //препятствие
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
                Gl.glBegin(Gl.GL_QUADS);

                Gl.glTexCoord2f(0, 0);
                Gl.glVertex3d(begin.X, begin.Y, 0);
                Gl.glTexCoord2f(1, 0);
                Gl.glVertex3d(begin.X + scaleX, begin.Y, 0);
                Gl.glTexCoord2f(1, 1);
                Gl.glVertex3d(begin.X + scaleX, begin.Y + scaleY, 0);
                Gl.glTexCoord2f(0, 1);
                Gl.glVertex3d(begin.X, begin.Y + scaleY, 0);

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

        // пока не используется
        public static void RenderLampAndLightNetPoint(Point beginNet)
        {
            Point tmp = new Point(beginNet.X * scaleX, beginNet.Y * scaleY);
            Gl.glColor3ub(0, 255, 0);
            RenderFillSquareGraphPoint(tmp,0);
            Gl.glColor4ub(255, 255, 0, 122);
            tmp.X += (int)(scaleX * 0.5);
            tmp.Y += (int)(scaleY * 0.5);

            DrawCircle(tmp, scaleX * 2, scaleY * 2);

        }
        

        // отрисовка фонаря
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

        // отрисовка пути 
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
        /// отрисовка линии между соседними ячейками
        /// для использования требуются координаты "алгоритмической" сетки (не пиксельные)
        /// </summary>
        /// <param name="begin">исходная ячейка, из которой должны провести линию</param>
        /// <param name="end">конечная ячейка</param>
        private static void RenderLineBetweenCell(Point begin, Point end)
        {
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(scaleX * (begin.X + 0.5) , scaleY * (begin.Y + 0.5));
            Gl.glVertex2d(scaleX * (end.X + 0.5), scaleY * (end.Y + 0.5));
            Gl.glEnd();
        }

    }
}


