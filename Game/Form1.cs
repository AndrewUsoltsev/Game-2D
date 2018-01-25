using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.DevIl;

using Game.Extension;
using Game.Models;
using Game.Enums;

namespace Game
{
    // проект написан совместно с Dmitriy Starostin
    /// <summary>
    /// Класс формы, на которой происходит отображение всех объектов
    /// </summary>
    public partial class Form1 : Form
    {
        // время суток
        DateTime dayTime;
        // игровое время
        DateTime gameTime;
        // начальный масштаб отображаемой сетки
        int scale = 15; 
        // исходное количество факелов
        int CountLamp = 10;
        // объект для отображения направления финиша
        Graphics g;
        // общая сетка, на ней находятся отрисовываемые объекты и сам персонаж
        Net net;

        /// <summary>
        /// Отображение направления от заданной точки к финишу
        /// </summary>
        /// <param name="box">Область отображения стрелки</param>
        /// <param name="Man">Текущее положение</param>
        /// <param name="Finish">Финишная точка</param>
        /// <param name="N">Размерность сетки, на которой происходит расчет, необходим для нормирования значений</param>
        private void ArrowDirection(PictureBox box, Point Man, Point Finish, int N)
        {

            int x0 = Finish.X - Man.X;
            int y0 = Finish.Y - Man.Y;

            double halfWidth = box.Width * 0.5;
            double halfHeight = box.Height * 0.5;


            Point middle = new Point((int)halfWidth, (int)halfHeight);

            double normX = (double)x0 / N;
            double normY = (double)y0 / N;

            int displayedX = middle.X + (int)(normX * halfWidth);
            // инвертирование
            int displayedY = box.Height - (middle.Y + (int)(normY * halfHeight));


            // box.Invalidate();
            g.DrawLine(new Pen(Brushes.Red, 4), middle, new Point(displayedX, displayedY));
            g.DrawRectangle(new Pen(Brushes.Red, 4), displayedX, displayedY, 2, 2);

        }

        // откуда начинается отображение сетки (так как отображается только часть сетки)
        Point beginRenderNet = new Point(0, 0);
        /// <summary>
        /// Инициализация объектов отрисовки, сетки с объектами, текстур и времени
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Scene.InitializeContexts();
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);

            Gl.glClearColor(255, 255, 255, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glViewport(0, 0, Scene.Width, Scene.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluOrtho2D(0.0, Scene.Width, 0.0, Scene.Height);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);


            GameRendering.mGlTextureObject0 = GameTextures.loadImage("..\\..\\Pictures\\pr0.bmp");
            GameRendering.mGlTextureObject1 = GameTextures.loadImage("..\\..\\Pictures\\pers0.bmp");
            GameRendering.mGlTextureObject2 = GameTextures.loadImage("..\\..\\Pictures\\pers.bmp");
            GameRendering.mGlTextureObject3 = GameTextures.loadImage("..\\..\\Pictures\\fac.bmp");
            GameRendering.mGlTextureObject4 = GameTextures.loadImage("..\\..\\Pictures\\grassDay.png");
            GameRendering.mGlTextureObject5 = GameTextures.loadImage("..\\..\\Pictures\\grassNight.png");

            g = ArrowBox.CreateGraphics();

            net = new Net();

            Time.Text = "07:00";
            dayTime = DateTime.Parse("07:00");

            // если день = 9, то время вышло
            // на все 1 день 23 часа
            gameTime = new DateTime(2, 1, 11, 23, 0, 0);
            GameTime.Text = gameTime.ToString("yyyy-MM-dd HH':'mm':'ss");

            GameRendering.RenderNet(net, beginRenderNet, Scene, scale, false);
            lastClickNet.X = 1;
            lastClickNet.Y = 1;
            GameRendering.RenderMouseClickAlgorithmPoint(lastClickNet.X, lastClickNet.Y, Textures.CurrentCharactert);
            timer.Start();
        }




        Point lastClickNet = new Point();
        Point lastClickNetWithoutOffset = new Point();

        // перерисовка всех отображаемых в данный момент объектов
        private void AllRender()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            GameRendering.RenderNet(net, beginRenderNet, Scene, scale, WorkWithTime.IsNight(dayTime.Hour));
            GameRendering.RenderMouseClickAlgorithmPoint(lastClickNet.X, lastClickNet.Y, Textures.CurrentCharactert);
            GameRendering.RenderWayByAlgorithmPoint(way);
            GameRendering.RenderMouseClickGraphPoint(currentClickGraph.X, currentClickGraph.Y, Textures.TransparentCharacter);
            GameRendering.RenderMouseClickAlgorithmPoint(subClickNet.X, subClickNet.Y, Textures.TransparentCharacter);
            Scene.Invalidate();
        }

        /// <summary>
        /// Вывод сообщения окна сообщений с параметрами
        /// </summary>
        /// <param name="message">Само сообщение</param>
        /// <param name="caption">Заголовок окна</param>
        private void OutMessageBox(string message, string caption)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }

        // перерасчет времени: добавление travelTime ко времени суток
        // и уменьшение на travelTime остаток игрового времени
        private void TotalTimeCountingForWay(int travelTime, Label time, Label GameTime)
        {
            dayTime = dayTime.AddMinutes(travelTime);
            time.Text = dayTime.ToShortTimeString();

            gameTime = gameTime.AddMinutes(-travelTime);
            GameTime.Text = gameTime.ToString("yyyy-MM-dd HH':'mm':'ss");
        }

        Point subClickNet = new Point(-1, -1);
        bool flagForFirstDrawing = true;

        // перемещение персонажа в нужную клетку, либо отображение пути, по которому может пройти персонаж
        // а так же растановка факелов
        private void Scene_MouseClick(object sender, MouseEventArgs e)
        {

            int mouseArgX = e.X;
            int mouseArgY = Scene.Height - e.Y; // инвертирование значения ординаты курсора
            var currentClickNet = GameRendering.GraphPointToAlgorithmPoint(mouseArgX, mouseArgY);

            lastClickNetWithoutOffset.X = currentClickNet.X;
            lastClickNetWithoutOffset.Y = currentClickNet.Y;

            currentClickNet.X += beginRenderNet.X;
            currentClickNet.Y += beginRenderNet.Y;

            if (net.IsFree(currentClickNet))
            {
                if (e.Button == MouseButtons.Right)
                {
                    OnRightClickForToarch(currentClickNet);
                }
                else
                {
                    g.Clear(Color.White);
                    ArrowDirection(ArrowBox, currentClickNet, net.finish, net.N);

                    if (lastClickNetWithoutOffset == subClickNet)
                    {
                        MoveCharacterAndRecountingPath(currentClickNet);
                        flagForFirstDrawing = true;
                    }
                    else
                        CalculationPathForCurrentCell();

                }
            }

        }

        // вспомогательный метод, используется при нажатии правой кнопки мыши
        // создает факел в указанной точке
        private void OnRightClickForToarch(Point currentClickNet)
        {
            int hx = Math.Abs(lastClickNetWithoutOffset.X - lastClickNet.X);
            int hy = Math.Abs(lastClickNetWithoutOffset.Y - lastClickNet.Y);
            if ((hx <= 2) && (hy <= 2))
                if (Convert.ToInt32(label1.Text) > 0)
                {
                    net.Lamp(currentClickNet);
                    CountLamp--;
                    label1.Text = CountLamp.ToString();
                }
                else
                    OutMessageBox("Фонарики кончились!!!", "Ошибка");
            AllRender();
        }

        // вспомогательный метод, перемещение персонажа на новую клетку
        private void MoveCharacterAndRecountingPath(Point currentClickNet)
        {
            beginRenderNet = net.BeginPointForCentering(currentClickNet, beginRenderNet, scale); // условие надо

            TotalTimeCountingForWay(distance, Time, GameTime);

            way = null;
            TravelTime.Text = "0";

            // старый клик на алгоритмической сетке (расположение персонажа)
            lastClickNet.X = currentClickNet.X - beginRenderNet.X;
            lastClickNet.Y = currentClickNet.Y - beginRenderNet.Y;

            if ((currentClickNet.X == net.finish.X) && (currentClickNet.Y == net.finish.Y))
            {
                OutMessageBox("Победа!", "!");
                Close();
            }

            subClickNet = new Point(-1, -1);
        }

        // вспомогательный метод, расчет пути по предполагаемому расположению персонажа
        // и отображение минут, которых потребуется на этот переход
        private void CalculationPathForCurrentCell()
        {
            subClickNet.X = lastClickNetWithoutOffset.X;
            subClickNet.Y = lastClickNetWithoutOffset.Y;
            way = GameControl.FindPath(net.CellsOfNet, lastClickNet, subClickNet, beginRenderNet, scale);
            if (way?.Count > 1)
            {
                distance = WorkWithTime.SubTotalTimeCountingForWay(way, DateTime.Parse(Time.Text), beginRenderNet, net, dayTimeTick);
                TravelTime.Text = distance.ToString();
            }
            else
            {
                TravelTime.Text = "0";
                distance = 0;
            }
            AllRender();
        }

        List<Point> way = new List<Point>();
        Point currentClickGraph;
        Point lastMoveNet;
        int distance = 0;

        // отображение мыши на области, а так же перерисовка области
        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            int mouseArgX = e.X;
            int mouseArgY = Scene.Height - e.Y; // инвертирование значения ординаты курсора

            // текущий
            currentClickGraph = new Point(mouseArgX, mouseArgY);
            var currentClickNet = GameRendering.GraphPointToAlgorithmPoint(mouseArgX, mouseArgY);

            if ((lastMoveNet != currentClickNet) || flagForFirstDrawing)
            {
                AllRender();
                flagForFirstDrawing = false;
            }
            lastMoveNet = currentClickNet;

        }



        int tickInterval = 10;
        int goingTime = 10;
        int dayTimeTick = 10;
        int tick = 0;
        // расчет времени суток, а так же смена его, при необходимости
        private void timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick == tickInterval)
            {
                int hourBefore = dayTime.Hour;
                dayTime = dayTime.AddMinutes(dayTimeTick);
                if (WorkWithTime.IsNight(dayTime.Hour) && !net.IsNearLamp(new Point(lastClickNet.X + beginRenderNet.X, lastClickNet.Y + beginRenderNet.Y)))
                    goingTime = dayTimeTick * 2;
                else
                    goingTime = dayTimeTick;

                gameTime = gameTime.AddMinutes(-goingTime);
                // условие проигрыша
                if (gameTime.Day == 9)
                {
                    OutMessageBox("время закончилось!!!", "Game end");
                    Close();

                }
                GameTime.Text = gameTime.ToString("yyyy-MM-dd HH':'mm':'ss");


                Time.Text = dayTime.ToShortTimeString();
                // перерисовываем, если день сменился ночью или наоборот
                if (WorkWithTime.TimesOfDay(hourBefore, dayTime.Hour))
                    AllRender();
                tick = 0;
            }
        }

        // двойной клик на клетку, расчет времени и отображение персонажа в новой позиции
        private void Scene_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int mouseArgX = e.X;
            int mouseArgY = Scene.Height - e.Y; // инвертирование значения ординаты курсора
            var currentClickNet = GameRendering.GraphPointToAlgorithmPoint(mouseArgX, mouseArgY);

            lastClickNetWithoutOffset.X = currentClickNet.X;
            lastClickNetWithoutOffset.Y = currentClickNet.Y;
            
            currentClickNet.X += beginRenderNet.X;
            currentClickNet.Y += beginRenderNet.Y;

            if (net.IsFree(currentClickNet))
            {
                subClickNet.X = lastClickNetWithoutOffset.X;
                subClickNet.Y = lastClickNetWithoutOffset.Y;
                way = GameControl.FindPath(net.CellsOfNet, lastClickNet, subClickNet, beginRenderNet, scale);
                if (way?.Count > 1)
                    distance = WorkWithTime.SubTotalTimeCountingForWay(way, DateTime.Parse(Time.Text), beginRenderNet, net, dayTimeTick);
                MoveCharacterAndRecountingPath(currentClickNet);

                flagForFirstDrawing = true;
            }
        }
    }
}