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
    public static class WorkWithTime
    {
        public static bool IsNight(Label Time)
        {
            int hour = DateTime.Parse(Time.Text).Hour;
            if (hour >= 6 && hour < 20)
                return false;
            return true;
        }
        public static bool IsNight(int hour)
        {
            if (hour >= 6 && hour < 20)
                return false;
            return true;
        }

        // надо ли перерисовывать (день изменяется на ночь)
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

        public static void AddTime(string timeOfWay, Label Time)
        {
            DateTime newTime = DateTime.Parse(Time.Text);
            newTime = newTime.AddMinutes(Convert.ToInt32(timeOfWay));
            Time.Text = newTime.ToShortTimeString();
        }
        public static void AddTime(Label Time, int timeOfWay)
        {
            DateTime newTime = DateTime.Parse(Time.Text);
            newTime = newTime.AddMinutes(timeOfWay);
            Time.Text = newTime.ToString("yyyy-MM-dd HH':'mm':'ss");
        }

        // коэффициент времени передвижения
        private static int travelTimeFactor = 20;
        // ходить днем — 20
        // ходить ночью — 40
        public static int SubTotalTimeCountingForWay(List<Point> way, DateTime dayTime, Point beginRenderNet, Net net, int dayTimeTick)
        {
            int totalMinutes = 0;
            for (int i = 1; i < way.Count; i++)
            {
                if (net.IsNearLamp(new Point(way[i].X + beginRenderNet.X, way[i].Y + beginRenderNet.Y)))
                    totalMinutes += WorkWithTime.travelTimeFactor;
                else
                {
                    if (WorkWithTime.IsNight(dayTime.Hour))
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
