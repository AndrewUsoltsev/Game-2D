using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Game.Models;

namespace Game.Extension
{
    /// <summary>
    /// Класс расширения
    /// Работа со временем
    /// </summary>
    public static class WorkWithTime
    {
        /// <summary>
        /// Определяет текущее время суток
        /// </summary>
        /// <param name="hour">Часы, для определения времени суток</param>
        /// <returns>Возвращает true если ночь, день — false</returns>
        public static bool IsNight(int hour)
        {
            if (hour >= 6 && hour < 20)
                return false;
            return true;
        }

        /// <summary>
        /// Определяет, сменилось ли время суток
        /// </summary>
        /// <param name="hourBefore">Часы до события</param>
        /// <param name="hourAfter">Часы после события</param>
        /// <returns></returns>
        public static bool TimesOfDay(int hourBefore, int hourAfter)
        {
            if (hourBefore >= 6 && hourBefore < 20)
            {
                if (!(hourAfter >= 6 && hourAfter < 20))
                    return true;
                else
                    return false;
            }
            else
            {
                if (hourAfter >= 6 && hourAfter < 20)
                    return true;
                else
                    return false;
            }
        }



        // коэффициент времени передвижения
        private static int travelTimeFactor = 20;
        // ходить днем — 20
        // ходить ночью — 40
        /// <summary>
        /// Подсчет времени по пути, пройденным персонажем
        /// </summary>
        /// <param name="way">Путь персонажа</param>
        /// <param name="dayTime">Текущее время суток</param>
        /// <param name="beginRenderNet"></param>
        /// <param name="net">Сетка, по которой передвигается персонаж</param>
        /// <param name="dayTimeTick">Модификатор времени суток</param>
        /// <returns>Возвращает число минут — затраченное персонажем на прохождение данного пути</returns>
        public static int SubTotalTimeCountingForWay(List<Point> way, DateTime dayTime, Point beginRenderNet, Net net, int dayTimeTick)
        {
            int totalMinutes = 0;
            for (int i = 1; i < way.Count; i++)
            {
                if (net.IsNearLamp(new Point(way[i].X + beginRenderNet.X, way[i].Y + beginRenderNet.Y)))
                    totalMinutes += WorkWithTime.travelTimeFactor;
                else
                {
                    if (IsNight(dayTime.Hour))
                        totalMinutes += travelTimeFactor * 2;
                    else
                        totalMinutes += travelTimeFactor;
                }

                dayTime = dayTime.AddMinutes(dayTimeTick);
            }
            return totalMinutes;
        }
    }
}
