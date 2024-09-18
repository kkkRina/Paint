using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    
    public class Rect
    {
        private Point point1;
        private Point point2;

        public double X => Math.Min(point1.X, point2.X);
        public double Y => Math.Min(point1.Y, point2.Y);

        public double Width => Math.Abs(point1.X - point2.X);
        public double Heigth => Math.Abs(point1.Y - point2.Y);
        public Rect(Point p1, Point p2)
        {
            point1 = p1;
            point2 = p2;
        }
    }
}
