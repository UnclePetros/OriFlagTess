/*
 * Copyright 2020 Pietro Vitelli
 * 
 * This file is part of OriFlagTess program.
 * 
 * OriFlagTess is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 * 
 * OriFlagTess is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with OriFlagTess.  If not, see<http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flagstone_Tessellation___Molecule_construction
{
    public static class Utils
    {
        public static double Cos(double value)
        {
            return Math.Round(Math.Cos(value), 5);
        }

        public static double Sin(double value)
        {
            return Math.Round(Math.Sin(value), 5);
        }

        public static double Tan(double value)
        {
            return Math.Round(Math.Tan(value), 5);
        }

        public static double Atan2(double value1, double value2)
        {
            return RadToDeg(Math.Atan2(value1, value2));
        }

        public static Line line(double x1, double y1, double x2, double y2, Brush brush)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            line.StrokeThickness = 1;
            line.Stroke = brush;

            Canvas.SetZIndex(line, 100);

            return line;
        }

        public static Line lineDash(double x1, double y1, double x2, double y2, Brush brush)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            DoubleCollection dashes = new DoubleCollection();
            dashes.Add(5);
            dashes.Add(5);
            line.StrokeDashArray = dashes;

            line.StrokeThickness = 1;
            line.Stroke = brush;

            Canvas.SetZIndex(line, 100);

            return line;
        }

        public static Rectangle rectangle(double width, double height, double left, double top, Brush brush)
        {
            Rectangle r = new Rectangle();
            r.Fill = brush;
            r.Width = width;
            r.Height = height;
            Canvas.SetLeft(r, left);
            Canvas.SetTop(r, top);

            return r;
        }

        public static Polygon polygon(PointCollection points, Brush brush)
        {
            Polygon p = new Polygon();
            p.Fill = brush;
            p.Points = points;

            return p;
        }

        public static Ellipse ellipse(double width, double height, double left, double top, Brush brush)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = width;
            ellipse.Height = height; 
            ellipse.Stroke = brush;
            ellipse.Fill = brush;
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            return ellipse;
        }

        public static double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadToDeg(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static Point RotTras(Point p, double angle, Point tras)
        {
            Point newP = new Point();
            //Rotation
            newP.X = p.X * Utils.Cos(angle) - p.Y * Utils.Sin(angle);
            newP.Y = p.X * Utils.Sin(angle) + p.Y * Utils.Cos(angle);
            
            //Traslation
            newP.X = newP.X + tras.X;
            newP.Y = newP.Y + tras.Y;

            return newP;
        }

        public static Point MultipleRotTras(Point p, List<double> angles, List<Point> tras)
        {
            Point newP = p;

            for (int i = 0; i < angles.Count; i++)
            {
                newP = RotTras(newP, angles[i], tras[i]);
            }

            return newP;
        }

        public static List<double> calculate4Sides(List<double> sides, List<double> angles)
        {
            Point C = new Point();
            C.X = sides[0] - sides[1] * Math.Cos(Math.PI - angles[1]);
            C.Y = sides[1] * Math.Sin(Math.PI - angles[1]);

            double AC = Math.Sqrt(Math.Pow(C.X, 2) + Math.Pow(C.Y, 2));
            double angleDAC = Math.PI - angles[0] - Math.Atan2(C.Y, C.X);
            double angleDCA = Math.PI - angles[2] - (angles[1] - Math.Atan2(C.Y, C.X));

            double CD = AC * Math.Sin(angleDAC) / Math.Sin(Math.PI - angles[3]);
            double AD = AC * Math.Sin(angleDCA) / Math.Sin(Math.PI - angles[3]);

            return new List<double>{ Math.Round(sides[0], 3), Math.Round(sides[1], 3), Math.Round(CD, 3), Math.Round(AD, 3) };
        }

        public static Point Rot(Point p, double angle, Point rotCenter)
        {
            Point newP = new Point();
            //Rotation
            newP.X = (p.X-rotCenter.X) * Utils.Cos(angle) + (p.Y-rotCenter.Y) * Utils.Sin(angle) + rotCenter.X;
            newP.Y = -(p.X - rotCenter.X) * Utils.Sin(angle) + (p.Y-rotCenter.Y) * Utils.Cos(angle) + rotCenter.Y;

            return newP;
        }

        public static Point Tras(Point p, Point trasCenter)
        {
            Point newP = new Point();
            //Rotation
            newP.X = p.X + trasCenter.X;
            newP.Y = p.Y + trasCenter.Y;

            return newP;
        }

        public static List<double> calculatePolygonSides(List<double> sides, List<double> angles)
        {
            Point V = new Point(sides[0],0);
            double rotAngle = 0;
            for (int i = 1; i < angles.Count-2; i++)
            {
                Point init = V; 
                rotAngle -= angles[i];
                V = Tras(V, new Point(sides[i],0));
                V = Rot(V, rotAngle, init);
            }

            rotAngle -= angles[angles.Count - 2];
            sides[angles.Count - 2] = ( - V.Y - V.X * Utils.Tan(angles[0]) ) / (Utils.Cos(rotAngle) * Utils.Tan(angles[0]) - Utils.Sin(rotAngle) );
            if(Utils.Cos(angles[0]) == 0)
                sides[angles.Count - 1] = Math.Round((-sides[angles.Count - 2] * Utils.Sin(rotAngle) + V.Y) / (Utils.Sin(angles[0])),5);
            else
                sides[angles.Count - 1] = Math.Round((sides[angles.Count - 2] * Utils.Cos(rotAngle) + V.X) / (-Utils.Cos(angles[0])),5);

            return sides;
        }

    }
}
