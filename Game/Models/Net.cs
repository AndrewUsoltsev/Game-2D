using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Game.Enums;
using Game.Extension;

namespace Game.Models
{
    public class Net
    {

        public int[,] CellsOfNet { get; private set; }
        // можно будет редактировать
        public int N { get; } = 60;
        public Point finish { get; private set; }
        // в конструкторе написать количество сторон у ячейки
        public Net()
        {
            CellsOfNet = new int[N,N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    CellsOfNet[i, j] = new int();
            
            EdgesFilling();

            GenerateGlobalNet(N);
           

        }


        // сетка без препятствий, препятствия только по краям карты
        private void EdgesFilling()
        {
            for (int i = 1; i < N-1; i++)
                for (int j = 1; j < N-1; j++) 
                    CellsOfNet[i,j] = (int)TypeOfCell.Free;

            // изначально квадратная область
            for (int i = 0; i < N; i++)
            {
                CellsOfNet[i,0] =   (int)TypeOfCell.Block1;
                CellsOfNet[i,N-1] = (int)TypeOfCell.Block1;
                CellsOfNet[0,i] =   (int)TypeOfCell.Block1;
                CellsOfNet[N-1,i] = (int)TypeOfCell.Block1;
            }
            

        }


        // перегрузка индексатора, нужна, так как наш массив приватный
        // обращение происходит как
        // Net A = new Net()
        // int cell = A[i,j], присваивание значения cell
        //
        // A[i,j] = changeCell, присваивание значения в массив cellsOfNet
        public int this[int i, int j]
        {
            get
            {
                return CellsOfNet[i,j];
            }
            // вообще, set здесь сомнителен, но пока можно оставить
            set
            {
                CellsOfNet[i,j] = value;
            }

        }

        public Point BeginPointForCentering(Point currentClickNet, Point beginRenderNet,int scale)
        {

            Point resultNet = new Point();
            int scaleAround = (int)(scale * 0.5);
            // для начала сетки
            if ((currentClickNet.X + beginRenderNet.X <= scaleAround) || (currentClickNet.X - scaleAround <= 0))
                resultNet.X = 0;
            // для меньше середины
            else if ((currentClickNet.X < scaleAround) || ((beginRenderNet.X + scale > N - 1) && ((int)(currentClickNet.X + scale * 0.5) < N)))
                resultNet.X = (int)(currentClickNet.X - scale * 0.5);
            // для конца сетки
            else if (currentClickNet.X + scaleAround > N - 1)
                resultNet.X = N - scale;
            // для больше середины
            else if (currentClickNet.X > scaleAround)
                resultNet.X = (int)(currentClickNet.X - scale * 0.5);
            else resultNet.X = beginRenderNet.X;

            // для начала сетки
            if ((currentClickNet.Y + beginRenderNet.Y <= scaleAround) || (currentClickNet.Y - scaleAround <= 0))
                resultNet.Y = 0;
            // для меньше середины
            else if ((currentClickNet.Y < scaleAround) || ((beginRenderNet.Y + scale > N - 1) && ((int)(currentClickNet.Y + scale * 0.5) < N)))
                resultNet.Y = (int)(currentClickNet.Y - scale * 0.5);
            // для конца сетки
            else if (currentClickNet.Y + scaleAround > N - 1)
                resultNet.Y = N - scale;
            // для больше середины
            else if (currentClickNet.Y > scaleAround)
                resultNet.Y = (int)(currentClickNet.Y - scale * 0.5);
            else resultNet.Y = beginRenderNet.Y;

            return resultNet;
        }
        public void GenerateGlobalNet(int N)
        {
            Random random = new Random();
            for (int i = 0; i < 150; i++)
            {
                int rX = random.Next(N - 2) + 1;
                int rY = random.Next(N - 2) + 1;

                int type = random.Next(2);
                if (type == 1)
                    StickFigure(new Point(rX, rY), Direction.DOWN);
                else
                    TriangleFigure(new Point(rX, rY), Direction.DOWN);
            }
            Lamp(new Point(4, 4));
            Point p = (new Point(0, 0));
            Random randx = new Random();
            Random randy = new Random();
            do
            {
                p.X = randx.Next(1, N-1);
                p.Y = randy.Next(2 * N / 3, N - 1);
            } while (IsBlock(p));
            Finish(p);
        }

        #region проверки на объекты рядом


        // вспомогательная проверка  
        private bool IsNearLampSubsidiary(Point pers, int xi, int yi)
        {
            Point pesr_n = pers;
            pesr_n.X = pesr_n.X + xi;
            pesr_n.Y = pesr_n.Y + yi;

            if ((pesr_n.X >= 0) && (pesr_n.Y >= 0) && (pesr_n.X < N) && (pesr_n.Y < N) && (IsLamp(pesr_n)))
                return true;
            return false;
        }

        //проверка на нахождение около фонаря 
        public bool IsNearLamp(Point pers)
        {
            Point pesr_n = pers;
            for (int j = -2; j <= 2; j++)
                for (int i = -2; i <= 2; i++)
                    if (IsNearLampSubsidiary(pesr_n, i, j))
                        return true;
            return false;
        }

        public bool IsBlock(Point pointNet)
        {
            if ((CellsOfNet[pointNet.X, pointNet.Y] == (int)TypeOfCell.Block1) || (CellsOfNet[pointNet.X, pointNet.Y] == (int)TypeOfCell.Block2))
                return true;
            return false;
        }

        public bool IsLamp(Point pointNet)
        {
            if (CellsOfNet[pointNet.X, pointNet.Y] == (int)TypeOfCell.Lamp)
                return true;
            return false;
        }
#endregion
        

        #region типы препятствий
        public void Finish(Point beginNet)
        {
            CellsOfNet[beginNet.X, beginNet.Y] = (int)TypeOfCell.Finish;

            finish = beginNet; 
        }
        public void Lamp(Point beginNet)
        {
            CellsOfNet[beginNet.X, beginNet.Y] = (int)TypeOfCell.Lamp;
        }

        private bool StickFigure(Point begin, Direction direction)
        {
            int length = 4;
            if (direction == Direction.DOWN)
            {
                if ((begin.Y + length >= N) || (begin.Y - length <= 0))
                    return false;
                // проверка соседних клеток
                for (int i = 0; i < length; i++)
                {
                    if (CellsOfNet[begin.X - 1, begin.Y - i] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y - i] != (int)TypeOfCell.Free)
                        return false;
                }
                if (CellsOfNet[begin.X, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X, begin.Y - length] != (int)TypeOfCell.Free)
                    return false;
                // диагонали
                if (CellsOfNet[begin.X - 1, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y + 1] != (int)TypeOfCell.Free)
                    return false;
                if (CellsOfNet[begin.X - 1, begin.Y + length] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y - length] != (int)TypeOfCell.Free)
                    return false;

                CellsOfNet[begin.X, begin.Y] = (int)TypeOfCell.Block2; 
                CellsOfNet[begin.X, begin.Y - 1] = (int)TypeOfCell.Block2;
                CellsOfNet[begin.X, begin.Y - 2] = (int)TypeOfCell.Block2;
                CellsOfNet[begin.X, begin.Y - 3] = (int)TypeOfCell.Block2;
            }
            else if (direction == Direction.RIGHT)
            {
                if ((begin.X + length >= N) || (begin.X - length <= 0))
                    return false;
                for (int i = 0; i < length; i++)
                {
                    if (CellsOfNet[begin.X + i, begin.Y - 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X + i, begin.Y + 1] != (int)TypeOfCell.Free)
                        return false;
                }
                if (CellsOfNet[begin.X - 1, begin.Y] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 4, begin.Y] != (int)TypeOfCell.Free)
                    return false;
                // диагонали
                if (CellsOfNet[begin.X - 1, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X - 1, begin.Y - 1] != (int)TypeOfCell.Free)
                    return false;
                if (CellsOfNet[begin.X + length, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X + length, begin.Y - 1] != (int)TypeOfCell.Free)
                    return false;

                CellsOfNet[begin.X, begin.Y] = (int)TypeOfCell.Block1;
                CellsOfNet[begin.X + 1, begin.Y] = (int)TypeOfCell.Block1;
                CellsOfNet[begin.X + 2, begin.Y] = (int)TypeOfCell.Block1;
                CellsOfNet[begin.X + 3, begin.Y] = (int)TypeOfCell.Block1;
            }
            else
                return false;


            return true;
        }


        private bool TriangleFigure(Point begin, Direction direction)
        {
            if (CellsOfNet[begin.X, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X, begin.Y - 1] != (int)TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X + 1, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y - 2] != (int)TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X - 1, begin.Y] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y] != (int)TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X + 2, begin.Y - 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X - 1, begin.Y - 1] != (int)TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X - 2, begin.Y + 1] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y + 1] != (int)TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X, begin.Y - 2] != (int)TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y - 2] != (int)TypeOfCell.Free)
                return false;

            CellsOfNet[begin.X, begin.Y]         = (int)TypeOfCell.Block1; 
            CellsOfNet[begin.X + 1, begin.Y]     = (int)TypeOfCell.Block1;
            CellsOfNet[begin.X + 1, begin.Y - 1] = (int)TypeOfCell.Block1;

            return true;
        }
#endregion

    }
}
