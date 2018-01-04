using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.DevIl;

using Game.Extension;
using Game.Models;

namespace Game
{
    public partial class Form1 : Form
    {
        int scale = 15; // начальный масштаб отображаемой сетки
        int CountLamp = 10;
        Graphics g;
        Net net;


        void ArrowDirection(PictureBox box, Point Man, Point Finish,int N)
        {
            
            int x0 = Finish.X - Man.X;
            int y0 = Finish.Y - Man.Y;

            double halfWidth =  box.Width * 0.5;
            double halfHeight = box.Height * 0.5;


            Point middle = new Point((int)halfWidth, (int)halfHeight);

            double normX = (double)x0 / N;
            double normY = (double)y0 / N;

            int displayedX = middle.X + (int)(normX * halfWidth);
            // инвертирование
            int displayedY = box.Height - (middle.Y + (int)(normY * halfHeight));


            // box.Invalidate();
            g.DrawLine(new Pen(Brushes.Red, 4), middle,  new Point(displayedX, displayedY));
            g.DrawRectangle(new Pen(Brushes.Red, 4), displayedX, displayedY, 2, 2);

        }

        // откуда начинается отображение сетки (так как отображается только часть сетки)
        Point beginRenderNet = new Point(0, 0);
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

            g = ArrowBox.CreateGraphics();

            net = new Net();
            
            Time.Text = "07:00";
            // если день = 9, то время вышло
            // на все 1 день 15 часов
            DateTime date = new DateTime(2,1,11,23,0,0);
            GameTime.Text = date.ToString("yyyy-MM-dd HH':'mm':'ss");
            GameRendering.RenderNet(net, beginRenderNet, Scene, scale, false);
            lastClickNet.X = 1;
            lastClickNet.Y = 1;
            GameRendering.RenderMouseClickAlgorithmPoint(lastClickNet.X, lastClickNet.Y );
            timer.Start();
        }
        



        Point lastClickNet = new Point();
        Point lastClickNetWithoutOffset = new Point();

      


        void TotalTimeCountingForWay(List<Point> way, Label time, Label gameTime)
        {
            // время суток
            DateTime dayTime = DateTime.Parse(time.Text);
            int totalMinutes = 0;
            totalMinutes = WorkWithTime.SubTotalTimeCountingForWay(way, dayTime, beginRenderNet, net,dayTimeTick);
            time.Text = dayTime.AddMinutes(way.Count * dayTimeTick).ToShortTimeString();
            WorkWithTime.AddTime(gameTime, -totalMinutes);
        }

        // перемещение персонажа в нужную клетку
        private void Scene_MouseClick(object sender, MouseEventArgs e)
        {
           
            int mouseArgX = e.X;
            int mouseArgY = Scene.Height - e.Y; // инвертирование значения ординаты курсора

            var currentClickNet = GameRendering.GraphPointToAlgorithmPoint(mouseArgX, mouseArgY);
            currentClickNet.X += beginRenderNet.X;
            currentClickNet.Y += beginRenderNet.Y;


            if (e.Button == MouseButtons.Right)
            {
                int hx = Math.Abs(currentClickNet.X - lastClickNetWithoutOffset.X);
                int hy = Math.Abs(currentClickNet.Y - lastClickNetWithoutOffset.Y);
                if ((hx < 3) && (hy < 3))
                    if (Convert.ToInt32(label1.Text) > 0)
                    {
                        net.Lamp(currentClickNet);
                        CountLamp--;
                        label1.Text = CountLamp.ToString();
                    }
                    else
                    {
                        string message = "Фонарики кончились!!!";
                        string caption = "Ошибка";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(message, caption, buttons);
                    }
            }
            else
            {
                g.Clear(Color.White);
                ArrowDirection(ArrowBox, currentClickNet, net.finish, net.N);

                if (((currentClickNet.X < net.N) && (currentClickNet.X > 0)) && ((currentClickNet.Y < net.N) && (currentClickNet.Y > 0)) && (!net.IsBlock(currentClickNet)))
                {

                    beginRenderNet = net.BeginPointForCentering(currentClickNet, beginRenderNet,scale);

                    Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                    TotalTimeCountingForWay(way, Time, GameTime);
                    
                    GameRendering.RenderNet(net, beginRenderNet, Scene, scale, WorkWithTime.IsNight(Time));
                    GameRendering.RenderMouseClickAlgorithmPoint(currentClickNet.X - beginRenderNet.X, currentClickNet.Y - beginRenderNet.Y);

                    Scene.Invalidate();

                    // старый клик на алгоритмической сетке
                    lastClickNet.X = currentClickNet.X - beginRenderNet.X;
                    lastClickNet.Y = currentClickNet.Y - beginRenderNet.Y;

                    lastClickNetWithoutOffset.X = currentClickNet.X;
                    lastClickNetWithoutOffset.Y = currentClickNet.Y;

                    if ((currentClickNet.X == net.finish.X) && (currentClickNet.Y == net.finish.Y))
                    {

                        string message = "Победа!";
                        string caption = "!";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(message, caption, buttons);
                        Close();
                    }

                }
            }
        }


        List<Point> way= new List<Point>();
        Point currentClickGraph; 
        // для отображения пути, по которому будет передвигаться персонаж
        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            int mouseArgX = e.X;
            int mouseArgY = Scene.Height - e.Y; // инвертирование значения ординаты курсора
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            GameRendering.RenderNet(net, beginRenderNet, Scene, scale, WorkWithTime.IsNight(Time));

            // текущий
            currentClickGraph = new Point(mouseArgX, mouseArgY);
            var currentClickNet = GameRendering.RenderMouseClickGraphPoint(mouseArgX, mouseArgY);
            // предыдущий
            GameRendering.RenderMouseClickAlgorithmPoint(lastClickNet.X, lastClickNet.Y);

            way = GameControl.FindPath(net.CellsOfNet, lastClickNet, currentClickNet, beginRenderNet, scale);
            GameRendering.RenderWayByAlgorithmPoint(way);

            Scene.Invalidate();


            // ? — проверка на Null
            if (way?.Count > 1)
            {
                int distance = WorkWithTime.SubTotalTimeCountingForWay(way, DateTime.Parse(Time.Text), beginRenderNet, net, dayTimeTick);
                TravelTime.Text = distance.ToString();
            }
            else
                TravelTime.Text = "0";
        }

        

        int tickInterval = 10;
        int goingTime = 10;
        int dayTimeTick = 10;
        int tick = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick == tickInterval)
            {
                

                DateTime dayTime = DateTime.Parse(Time.Text);
                int hourBefore = dayTime.Hour;
                dayTime = dayTime.AddMinutes(dayTimeTick);

                if (WorkWithTime.IsNight(dayTime.Hour) && !net.IsNearLamp(lastClickNetWithoutOffset))
                    goingTime = dayTimeTick * 2;
                else
                    goingTime = dayTimeTick;

                DateTime gameTime = DateTime.Parse(GameTime.Text);
                gameTime = gameTime.AddMinutes(-goingTime);
                // условие проигрыша
               if (gameTime.Day == 9)
               {
                   string message = "время закончилось!!!";
                   string caption = "Game end";
                   MessageBoxButtons buttons = MessageBoxButtons.OK;
                   MessageBox.Show(message, caption, buttons);
                   Close();
                       
               }
                GameTime.Text = gameTime.ToString("yyyy-MM-dd HH':'mm':'ss");

               
                Time.Text = dayTime.ToShortTimeString();
                // перерисовываем, если день сменился ночью или наоборот
                if (WorkWithTime.TimesOfDay(hourBefore, dayTime.Hour))
                {
                    GameRendering.RenderNet(net, beginRenderNet, Scene, scale, WorkWithTime.IsNight(dayTime.Hour));
                    GameRendering.RenderMouseClickAlgorithmPoint(lastClickNet.X, lastClickNet.Y);
                    GameRendering.RenderWayByAlgorithmPoint(way);
                    GameRendering.RenderMouseClickGraphPoint(currentClickGraph.X, currentClickGraph.Y);
                }
                tick = 0;
            }
        }

    }
}
