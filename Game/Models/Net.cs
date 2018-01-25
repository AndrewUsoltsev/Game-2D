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
    /// <summary>
    /// Класс сетки, на котоой отображаются объекты
    /// </summary>
    public class Net
    {
        /// <summary>
        /// Сетка
        /// </summary>
        public TypeOfCell[,] CellsOfNet { get; private set; }
        
        /// <summary>
        /// Размерность сетки
        /// </summary>
        public int N { get; } = 60;

        /// <summary>
        /// Финишная клетка
        /// </summary>
        public Point finish { get; private set; }
        
        /// <summary>
        /// Конструктор сетки, генерирование препятствий
        /// </summary>
        public Net()
        {
            CellsOfNet = new TypeOfCell[N,N];
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
                    CellsOfNet[i,j] = TypeOfCell.Free;

            // изначально квадратная область
            for (int i = 0; i < N; i++)
            {
                CellsOfNet[i,0] =   TypeOfCell.Block1;
                CellsOfNet[i,N-1] = TypeOfCell.Block1;
                CellsOfNet[0,i] =   TypeOfCell.Block1;
                CellsOfNet[N-1,i] = TypeOfCell.Block1;
            }
            

        }


        /// <summary>
        /// Перегрузка индексатора
        /// </summary>
        /// <param name="i">Абсцисса клетки</param>
        /// <param name="j">Ордината клетки</param>
        /// <returns></returns>
        public TypeOfCell this[int i, int j]
        {
            get
            {
                return CellsOfNet[i,j];
            }
            set
            {
                CellsOfNet[i,j] = value;
            }

        }


        // coord = true — x
        // coord = false — y
        private int OneCoordBeginPointForCentering(int currentClickNet, int beginRenderNet, int scale)
        {
            int resultCoord = 0;
            int scaleAround = (int)(scale * 0.5);
            // для начала сетки
            if ((currentClickNet + beginRenderNet <= scaleAround) || (currentClickNet - scaleAround <= 0))
                resultCoord = 0;
            // для меньше середины
            else if ((currentClickNet < scaleAround) || ((beginRenderNet + scale > N - 1) && ((int)(currentClickNet + scale * 0.5) < N)))
                resultCoord = (int)(currentClickNet - scale * 0.5);
            // для конца сетки
            else if (currentClickNet + scaleAround > N - 1)
                resultCoord = N - scale;
            // для больше середины
            else if (currentClickNet > scaleAround)
                resultCoord = (int)(currentClickNet - scale * 0.5);
            else resultCoord = beginRenderNet;
            return resultCoord;
        }

        /// <summary>
        /// Перемещение начала отобраемой части сетки
        /// </summary>
        /// <param name="currentClickNet">Текущая точка</param>
        /// <param name="beginRenderNet">Предыдущая точка начала отображаемой сетки</param>
        /// <param name="scale">Масштаб отображаемой части сетки</param>
        /// <returns>Возвращает точку начала отобраемой части сетки</returns>
        public Point BeginPointForCentering(Point currentClickNet, Point beginRenderNet,int scale)
        {
            Point resultNet = new Point();
            resultNet.X = OneCoordBeginPointForCentering(currentClickNet.X, beginRenderNet.X, scale);
            resultNet.Y = OneCoordBeginPointForCentering(currentClickNet.Y, beginRenderNet.Y, scale);
            return resultNet;
        }

        /// <summary>
        /// Генерирование объектов на сетке
        /// </summary>
        /// <param name="N">Размерность сетки</param>
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
                // ищем свободную клетку
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

        /// <summary>
        /// Проверка на нахождение около фонаря 
        /// </summary>
        /// <param name="pers">Координаты персонажа на сетке</param>
        /// <returns>true — если персонаж недалеко от сетки, иначе false</returns>
        public bool IsNearLamp(Point pers)
        {
            Point pesr_n = pers;
            for (int j = -2; j <= 2; j++)
                for (int i = -2; i <= 2; i++)
                    if (IsNearLampSubsidiary(pesr_n, i, j))
                        return true;
            return false;
        }

        /// <summary>
        /// Проверка клетки сетки на препятствие
        /// </summary>
        /// <param name="pointNet">Проверяемая клетка</param>
        /// <returns>true — если клетка block, иначе false</returns>
        public bool IsBlock(Point pointNet)
        {
            if ((CellsOfNet[pointNet.X, pointNet.Y] == TypeOfCell.Block1) || (CellsOfNet[pointNet.X, pointNet.Y] == TypeOfCell.Block2))
                return true;
            return false;
        }

        /// <summary>
        /// Проверка, свободна ли клетка
        /// </summary>
        /// <param name="pointNet">Проверяемая клетка</param>
        /// <returns>true — если клетка свободна, иначе false</returns>
        public bool IsFree(Point pointNet)
        {
            if (((pointNet.X < N) && (pointNet.X > 0)) && ((pointNet.Y < N) && (pointNet.Y > 0)) && (!IsBlock(pointNet)))
                return true;
            return false;
        }

        /// <summary>
        /// Проверка, является ли клетка факелом
        /// </summary>
        /// <param name="pointNet">Проверяемая клетка</param>
        /// <returns>true — если клетка факел, иначе false</returns>
        public bool IsLamp(Point pointNet)
        {
            if (CellsOfNet[pointNet.X, pointNet.Y] == TypeOfCell.Lamp)
                return true;
            return false;
        }
#endregion
        

        #region типы препятствий
        private void Finish(Point beginNet)
        {
            CellsOfNet[beginNet.X, beginNet.Y] = TypeOfCell.Finish;

            finish = beginNet; 
        }
        /// <summary>
        /// Создает факел в указанной клетке
        /// </summary>
        /// <param name="beginNet">Клетка,в которой создается факел</param>
        public void Lamp(Point beginNet)
        {
            CellsOfNet[beginNet.X, beginNet.Y] = TypeOfCell.Lamp;
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
                    if (CellsOfNet[begin.X - 1, begin.Y - i] != TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y - i] != TypeOfCell.Free)
                        return false;
                if (CellsOfNet[begin.X, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X, begin.Y - length] != TypeOfCell.Free)
                    return false;
                // диагонали
                if (CellsOfNet[begin.X - 1, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y + 1] != TypeOfCell.Free)
                    return false;
                if (CellsOfNet[begin.X - 1, begin.Y + length] != TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y + length] != TypeOfCell.Free)
                    return false;

                CellsOfNet[begin.X, begin.Y] = TypeOfCell.Block2; 
                CellsOfNet[begin.X, begin.Y - 1] = TypeOfCell.Block2;
                CellsOfNet[begin.X, begin.Y - 2] = TypeOfCell.Block2;
                CellsOfNet[begin.X, begin.Y - 3] = TypeOfCell.Block2;
            }
            else if (direction == Direction.RIGHT)
            {
                if ((begin.X + length >= N) || (begin.X - length <= 0))
                    return false;
                for (int i = 0; i < length; i++)
                    if (CellsOfNet[begin.X + i, begin.Y - 1] != TypeOfCell.Free || CellsOfNet[begin.X + i, begin.Y + 1] != TypeOfCell.Free)
                        return false;
                if (CellsOfNet[begin.X - 1, begin.Y] != TypeOfCell.Free || CellsOfNet[begin.X + 4, begin.Y] != TypeOfCell.Free)
                    return false;
                // диагонали
                if (CellsOfNet[begin.X - 1, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X - 1, begin.Y - 1] != TypeOfCell.Free)
                    return false;
                if (CellsOfNet[begin.X + length, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X + length, begin.Y - 1] != TypeOfCell.Free)
                    return false;

                CellsOfNet[begin.X, begin.Y] = TypeOfCell.Block2;
                CellsOfNet[begin.X + 1, begin.Y] = TypeOfCell.Block2;
                CellsOfNet[begin.X + 2, begin.Y] = TypeOfCell.Block2;
                CellsOfNet[begin.X + 3, begin.Y] = TypeOfCell.Block2;
            }
            else
                return false;


            return true;
        }


        private bool TriangleFigure(Point begin, Direction direction)
        {
            if (CellsOfNet[begin.X, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X, begin.Y - 1] != TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X + 1, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X + 1, begin.Y - 2] != TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X - 1, begin.Y] != TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y] != TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X + 2, begin.Y - 1] != TypeOfCell.Free || CellsOfNet[begin.X - 1, begin.Y - 1] != TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X - 2, begin.Y + 1] != TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y + 1] != TypeOfCell.Free)
                return false;
            if (CellsOfNet[begin.X, begin.Y - 2] != TypeOfCell.Free || CellsOfNet[begin.X + 2, begin.Y - 2] != TypeOfCell.Free)
                return false;

            CellsOfNet[begin.X, begin.Y]         = TypeOfCell.Block2; 
            CellsOfNet[begin.X + 1, begin.Y]     = TypeOfCell.Block2;
            CellsOfNet[begin.X + 1, begin.Y - 1] = TypeOfCell.Block2;

            return true;
        }
#endregion

    }
}
