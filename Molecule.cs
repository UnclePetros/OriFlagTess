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

namespace Flagstone_Tessellation___Molecule_construction
{
    public class Molecule
    {
        Point center;
        int numPleats;
        List<double> sides;
        List<double> angles;
        List<double> foldCenters;
        double spacing;
        List< Dictionary<string, Point> > pleats;

        public Molecule()
        {
            this.spacing = 0.0D;
            this.pleats = new List<Dictionary<string, Point>>();

            //Init pleats list
            for (int i = 0; i < numPleats; i++)
            {
                pleats.Add(new Dictionary<string, Point>());
            }
        }

        /// <summary>
        /// Molecule constructor that accept the center of the molecule as parameter
        ///
        ///          |  Pleat3   |
        ///   angle3 |           | angle2
        /// ---------D   Side3   C-----------
        ///          
        ///  Pleat4       O        Pleat2
        /// 
        /// ---------A   Side1   B-----------
        ///   angle4 |           | angle1
        ///          |  Pleat1   | 
        /// 
        /// </summary>
        /// <param name="center">Is the center O of the molecule expressed as a point in a cartesian system with origin in A(0,0)</param>
        public Molecule(Point center, int numPleats)
        {
            this.spacing = 0.0D;
            this.center = center;
            this.numPleats = numPleats;

            this.pleats = new List<Dictionary<string, Point>>();

            //Init pleats list
            for (int i = 0; i < numPleats; i++)
            {
                pleats.Add( new Dictionary<string, Point>() );
            }
        }

        /// <summary>
        /// Calculate pleat construction for a specified center O and side AB:
        /// 
        ///          |                   |
        ///          |                   |
        /// ---------                     -----------
        /// 
        /// 
        ///                    P        
        /// 
        /// 
        /// ---------V1                  V2-----------
        ///          |         L         |           
        ///          |                   |
        ///          |   M     Q     N   |
        ///          |                   |
        ///          |         R         |
        /// 
        /// </summary>
        /// <param name="numPleat">The number of the pleat to be calculated. Starts from 0 to n-1, where n is the number of pleats of the molecule</param>
        /// <param name="side">It is the width of the pleat</param>
        public void calculatePleat(int numPleat) {

            Point P = getRecursiveCenter(numPleat);
            double l = this.sides[numPleat];
            double c = this.foldCenters[numPleat];
            double s = this.spacing;

            Point V1 = new Point( 0, 0 );
            Point V2 = new Point( Math.Round(l,5), Math.Round(V1.Y,5) );
            Point L = new Point( Math.Round(l/2,5), Math.Round((Math.Pow(P.X,2)-P.X*l+Math.Pow(P.Y,2))/(2*P.Y),5) );
            Point M = new Point( Math.Round((c-s/2)/2,5), Math.Round((Math.Pow(P.X, 2) - P.X * l) / (2 * P.Y) + ((l-P.X)/2)*(P.X-(c-s/2))/P.Y,5) );
            Point N = new Point(Math.Round((c+s/2+l)/2,5), Math.Round((Math.Pow(P.X, 2) - P.X * l) / (2 * P.Y) - (P.X/2)*((P.X -(c+s/2))/P.Y),5) );
            Point Q = new Point(); //TODO
            Point R = new Point(); //TODO

            pleats[numPleat].Add("P", P);
            pleats[numPleat].Add("V1",V1);
            pleats[numPleat].Add("V2", V2);
            pleats[numPleat].Add("L", L);
            pleats[numPleat].Add("Q", Q);
            pleats[numPleat].Add("R", R);
            pleats[numPleat].Add("M", M);
            pleats[numPleat].Add("N", N);

            Console.WriteLine("----------{0}---------", numPleat);
            Console.WriteLine("P({0},{1})",P.X,P.Y);
            Console.WriteLine("V1({0},{1})", V1.X, V1.Y);
            Console.WriteLine("V2({0},{1})", V2.X, V2.Y);
            Console.WriteLine("L({0},{1})", L.X, L.Y);
            Console.WriteLine("Q({0},{1})", Q.X, Q.Y);
            Console.WriteLine("R({0},{1})", R.X, R.Y);
            Console.WriteLine("M({0},{1})", M.X, M.Y);
            Console.WriteLine("N({0},{1})", N.X, N.Y);
            Console.WriteLine("--------------------");
        }

        private Point getRecursiveCenter(int numPleat) {

            int n = numPleat;
            Point C = new Point();
            Point prevC = new Point();
            if (numPleat == 0)
                C = this.center;
            else
            {
                prevC = getRecursiveCenter(n - 1);
                C.X = Math.Round((this.sides[n - 1] - prevC.X) * Math.Round(Math.Cos(Math.PI - this.angles[n]),5) + prevC.Y * Math.Round(Math.Sin(Math.PI - this.angles[n]),5),5);
                C.Y = Math.Round((this.sides[n - 1] - prevC.X) * Math.Round(Math.Sin(Math.PI - this.angles[n]),5) - prevC.Y * Math.Round(Math.Cos(Math.PI - this.angles[n]),5),5);
            }

            return C;
        }

        public void calculatePleats()
        {
            for (int i = 0; i < this.numPleats; i++)
            {
                this.calculatePleat(i);
            }
        }

        public List<double> getAngles()
        {
            return this.angles;
        }

        public void setAngles(List<double> angles) {
            this.angles = angles;
        }

        public List<double> getSides()
        {
            return this.sides;
        }

        public void setSides(List<double> sides)
        {
            this.sides = sides;
        }

        public List<double> getFoldCenters()
        {
            return this.foldCenters;
        }

        public void setFoldCenters(List<double> foldCenters)
        {
            this.foldCenters = foldCenters;
        }

        public void setSpacing(double spacing)
        {
            this.spacing = spacing;
        }

        public Point getCenter()
        {
            return this.center;
        }

        public Dictionary<string, Point> getPleat(int numPleat)
        {
            return this.pleats[numPleat];
        }

        public int getNumPleats()
        {
            return this.numPleats;
        }

        public List<Dictionary<String,Point>> getPleats()
        {
            return this.pleats;
        }
    }
}
