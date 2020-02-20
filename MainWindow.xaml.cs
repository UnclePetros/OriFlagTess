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

using Svg;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Xceed.Wpf.Toolkit;

namespace Flagstone_Tessellation___Molecule_construction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        const int NUM_SIDES = 4;
        List<double> sides;
        List<double> foldCenters;
        List<double> angles;
        public MainWindow()
        {
            InitializeComponent();

            //Default input values
            this.angle1.Value = 90;
            this.angle2.Value = 90;
            this.angle3.Value = 90;
            this.angle4.Value = 90;
            this.side1.Value = 50;
            this.side2.Value = 50;
            this.centerX.Value = 25;
            this.centerY.Value = 25;

            this.angles = new List<double> { Utils.DegToRad((double)this.angle1.Value), Utils.DegToRad((double)this.angle2.Value), Utils.DegToRad((double)this.angle3.Value), Utils.DegToRad((double)this.angle4.Value) };

            sides = new List<double> { 0.0D, 0.0D, 0.0D, 0.0D };
            sides[0] = (double)this.side1.Value;
            sides[1] = (double)this.side2.Value;
            sides = Utils.calculate4Sides(sides, angles);
            foldCenters = new List<double> { sides[0]/2, sides[1] / 2, sides[2] / 2, sides[3] / 2 };

            //Node View initialization
            this.inputNodeCanvas.ClipToBounds = true;
            this.inputNodeCanvas.Background = new SolidColorBrush(Colors.Beige);

            //Crease Pattern View initialization
            this.creasePatternCanvas.ClipToBounds = true;
            this.creasePatternCanvas.Background = new SolidColorBrush(Colors.Beige);
        }

        private List<Dictionary<string, Point>> renderInputNode() {
            this.inputNodeCanvas.Children.Clear();
            Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
            Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
            Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel

            this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, inputCenter.X + trs.X - 2, -inputCenter.Y + trs.Y - 2, Brushes.Brown));

            List<Dictionary<string, Point>> pleatsRotTras = new List<Dictionary<string, Point>>();
            Point traslation = new Point(0, 0);
            double angle = 0.0D;

            for (int i = 0; i < 4; i++)
            {
                Point A = new Point(0, 0);
                Point B = new Point(sides[i], A.Y);
                Point A_END = new Point(0, -inputNodeCanvas.Width);
                Point B_END = new Point(sides[i], A_END.Y);

                Dictionary<string, Point> pleat = new Dictionary<string, Point>();
                pleat.Add("V1", A);
                pleat.Add("V2", B);
                pleat.Add("V1-END", A_END);
                pleat.Add("V2-END", B_END);
                
                Dictionary<string, Point> pleatRotTras = new Dictionary<string, Point>();

                if (i > 0)
                {
                    angle += angles[i];
                    traslation = new Point(pleatsRotTras[i - 1]["V2"].X, pleatsRotTras[i - 1]["V2"].Y);
                }

                foreach (KeyValuePair<string, Point> item in pleat)
                {
                    pleatRotTras.Add(item.Key, Utils.RotTras(item.Value, angle, traslation));
                }

                pleatsRotTras.Add(pleatRotTras);

                this.inputNodeCanvas.Children.Add(Utils.line(pleatRotTras["V1"].X + trs.X, -pleatRotTras["V1"].Y + trs.Y, pleatRotTras["V2"].X + trs.X, -pleatRotTras["V2"].Y + trs.Y, Brushes.Brown));
                this.inputNodeCanvas.Children.Add(Utils.line(pleatRotTras["V1"].X + trs.X, -pleatRotTras["V1"].Y + trs.Y, pleatRotTras["V1-END"].X + trs.X, -pleatRotTras["V1-END"].Y + trs.Y, Brushes.Brown));
                this.inputNodeCanvas.Children.Add(Utils.line(pleatRotTras["V2"].X + trs.X, -pleatRotTras["V2"].Y + trs.Y, pleatRotTras["V2-END"].X + trs.X, -pleatRotTras["V2-END"].Y + trs.Y, Brushes.Brown));
            }

            for (int i = 0; i < 4; i++)
            {
                this.inputNodeCanvas.Children.Add(Utils.polygon(
                    new PointCollection {
                        new Point(pleatsRotTras[i]["V1"].X + trs.X, -pleatsRotTras[i]["V1"].Y + trs.Y),
                        new Point(pleatsRotTras[(i+3)%4]["V2"].X + trs.X, -pleatsRotTras[(i+3)%4]["V2"].Y + trs.Y),
                        new Point(pleatsRotTras[i]["V1-END"].X + trs.X, -pleatsRotTras[i]["V1-END"].Y + trs.Y),
                        new Point(pleatsRotTras[(i+3)%4]["V2-END"].X + trs.X, -pleatsRotTras[(i+3)%4]["V2-END"].Y + trs.Y)
                    },
                    Brushes.LightYellow));
            }

            return pleatsRotTras;
        }

        public void renderCreasePattern()
        {
            this.creasePatternCanvas.Children.Clear();
            Point moleculeCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value);
            Molecule mol = new Molecule(moleculeCenter, 4);
            mol.setAngles(angles);
            mol.setSides(sides);
            mol.setFoldCenters(new List<double> { sides[0] / 2, sides[1] / 2, sides[2] / 2, sides[3] / 2 });
            mol.calculatePleats();

            Point canvasCenter = new Point(this.creasePatternCanvas.Width / 2, this.creasePatternCanvas.Height / 2); //Center of the crease pattern canvas panel
            Point trs = new Point(canvasCenter.X - mol.getCenter().X, canvasCenter.Y + mol.getCenter().Y);
            this.creasePatternCanvas.Children.Add(Utils.ellipse(4, 4, mol.getCenter().X + trs.X - 2, -mol.getCenter().Y + trs.Y - 2, Brushes.Brown));

            List<Dictionary<string, Point>> pleats, pleatsRotTras;
            pleats = mol.getPleats();
            pleatsRotTras = new List<Dictionary<string, Point>>();

            Point traslation = new Point(0, 0);
            double angle = 0.0D;

            this.Log.Text = "";

            for (int i = 0; i < mol.getNumPleats(); i++)
            {
                Dictionary<string, Point> pleat = pleats[i];
                Dictionary<string, Point> pleatRotTras = new Dictionary<string, Point>();

                //Write log
                this.Log.Text += "Pleat" + (i+1) + ":\t P( " + Math.Round(pleat["P"].X,3) + " ; " + Math.Round(pleat["P"].Y,3) + " )\r\n";
                this.Log.Text += "\t V1( " + Math.Round(pleat["V1"].X, 3) + ";" + Math.Round(pleat["V1"].Y, 3) + " )\r\n";
                this.Log.Text += "\t V2( " + Math.Round(pleat["V2"].X,3) + " ; " + Math.Round(pleat["V2"].Y,3) + " )\r\n";
                this.Log.Text += "\t L (" + Math.Round(pleat["L"].X, 3) + ";" + Math.Round(pleat["L"].Y, 3) + " )\r\n";
                this.Log.Text += "\t M( " + Math.Round(pleat["M"].X, 3) + " ; " + Math.Round(pleat["M"].Y, 3) + " )\r\n";
                this.Log.Text += "\t N( " + Math.Round(pleat["N"].X,3) + " ; " + Math.Round(pleat["N"].Y,3) + " ) \r\n";
                this.Log.Text += "\t Side" + (i + 1) + " = " + mol.getSides()[i] + "\r\n";
                this.Log.Text += "\t Angle1 = " + Utils.RadToDeg(mol.getAngles()[i]) + "\r\n";
                this.Log.Text += "\t Angle2 = " + Utils.RadToDeg(mol.getAngles()[i % mol.getNumPleats()]) + "\r\n";
                this.Log.Text += "\t PleatC" + (i + 1) + " = "+mol.getFoldCenters()[i]+" \r\n";

                if (i > 0)
                {
                    angle += mol.getAngles()[i];
                    traslation = new Point(pleatsRotTras[i - 1]["V2"].X, pleatsRotTras[i - 1]["V2"].Y);
                }

                foreach (KeyValuePair<string, Point> item in pleat)
                {
                    pleatRotTras.Add(item.Key, Utils.RotTras(item.Value, angle, traslation));

                    if (item.Key == "V1" || item.Key == "V2" || item.Key == "M" || item.Key == "N")
                        pleatRotTras.Add(item.Key + "-END", Utils.RotTras(new Point(item.Value.X, item.Value.Y - canvasCenter.Y), angle, traslation));
                }

                pleatsRotTras.Add(pleatRotTras);

                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, pleatRotTras["V1"].X + trs.X, -pleatRotTras["V1"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, pleatRotTras["V2"].X + trs.X, -pleatRotTras["V2"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, pleatRotTras["M"].X + trs.X, -pleatRotTras["M"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, pleatRotTras["N"].X + trs.X, -pleatRotTras["N"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["M"].X + trs.X, -pleatRotTras["M"].Y + trs.Y, pleatRotTras["N"].X + trs.X, -pleatRotTras["N"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["V1"].X + trs.X, -pleatRotTras["V1"].Y + trs.Y, pleatRotTras["M"].X + trs.X, -pleatRotTras["M"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["V2"].X + trs.X, -pleatRotTras["V2"].Y + trs.Y, pleatRotTras["N"].X + trs.X, -pleatRotTras["N"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["V1"].X + trs.X, -pleatRotTras["V1"].Y + trs.Y, pleatRotTras["V1-END"].X + trs.X, -pleatRotTras["V1-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["M"].X + trs.X, -pleatRotTras["M"].Y + trs.Y, pleatRotTras["M-END"].X + trs.X, -pleatRotTras["M-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["N"].X + trs.X, -pleatRotTras["N"].Y + trs.Y, pleatRotTras["N-END"].X + trs.X, -pleatRotTras["N-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["V2"].X + trs.X, -pleatRotTras["V2"].Y + trs.Y, pleatRotTras["V2-END"].X + trs.X, -pleatRotTras["V2-END"].Y + trs.Y, Brushes.Brown));

                if (i > 0)
                {
                    this.creasePatternCanvas.Children.Add(Utils.line(pleatsRotTras[i - 1]["L"].X + trs.X, -pleatsRotTras[i - 1]["L"].Y + trs.Y, pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, Brushes.Brown));
                    if (i == mol.getNumPleats() - 1)
                        this.creasePatternCanvas.Children.Add(Utils.line(pleatsRotTras[0]["L"].X + trs.X, -pleatsRotTras[0]["L"].Y + trs.Y, pleatRotTras["L"].X + trs.X, -pleatRotTras["L"].Y + trs.Y, Brushes.Brown));
                }
            }
        }

        private void renderAllCanvasWithAngleFocus(int inputIndex) {
            List<Dictionary<string, Point>> pleatsRotTras;
            pleatsRotTras = renderInputNode();

            //Highlight focused input on input node canvas
            Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
            Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
            Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
            Line hl1 = Utils.line(pleatsRotTras[inputIndex]["V1"].X + trs.X, -pleatsRotTras[inputIndex]["V1"].Y + trs.Y, pleatsRotTras[inputIndex]["V1-END"].X + trs.X, -pleatsRotTras[inputIndex]["V1-END"].Y + trs.Y, Brushes.Red);
            hl1.StrokeThickness = 1.5d;
            this.inputNodeCanvas.Children.Add(hl1);
            Line hl2 = Utils.line(pleatsRotTras[(inputIndex + 3) % 4]["V2"].X + trs.X, -pleatsRotTras[(inputIndex + 3) % 4]["V2"].Y + trs.Y, pleatsRotTras[(inputIndex + 3) % 4]["V2-END"].X + trs.X, -pleatsRotTras[(inputIndex + 3) % 4]["V2-END"].Y + trs.Y, Brushes.Red);
            hl2.StrokeThickness = 1.5d;
            this.inputNodeCanvas.Children.Add(hl2);
            this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl1.X1 - 2, hl1.Y1 - 2, Brushes.Red));

            var g = new StreamGeometry();
            using (var gc = g.Open())
            {
                Point arcStart = new Point(
                    pleatsRotTras[inputIndex]["V1"].X - (pleatsRotTras[inputIndex]["V1"].X - pleatsRotTras[inputIndex]["V1-END"].X) / 5 + trs.X, 
                    -pleatsRotTras[inputIndex]["V1"].Y - (-pleatsRotTras[inputIndex]["V1"].Y + pleatsRotTras[inputIndex]["V1-END"].Y) / 5 + trs.Y
                );
                Point arcEnd = new Point(
                    pleatsRotTras[(inputIndex + 3) % 4]["V2"].X - (pleatsRotTras[(inputIndex + 3) % 4]["V2"].X - pleatsRotTras[(inputIndex + 3) % 4]["V2-END"].X) / 5 + trs.X, 
                    -pleatsRotTras[(inputIndex + 3) % 4]["V2"].Y - (-pleatsRotTras[(inputIndex + 3) % 4]["V2"].Y + pleatsRotTras[(inputIndex + 3) % 4]["V2-END"].Y) / 5 + trs.Y
                );
                gc.BeginFigure(
                    startPoint: arcStart,
                    isFilled: false,
                    isClosed: false);
                gc.ArcTo(
                    point: arcEnd,
                    size: new Size(Math.Abs(arcStart.X - arcEnd.X), Math.Abs(arcStart.Y - arcEnd.Y)),
                    rotationAngle: 0d,
                    isLargeArc: false,
                    sweepDirection: SweepDirection.Clockwise,
                    isStroked: true,
                    isSmoothJoin: false);
            }
            System.Windows.Shapes.Path hp = new System.Windows.Shapes.Path();
            hp.Stroke = Brushes.Red;
            hp.StrokeThickness = 1.5;
            hp.Data = g;
            this.inputNodeCanvas.Children.Add(hp);

            renderCreasePattern();
        }

        private void side1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                sides[0] = (double)this.side1.Value;
                sides = Utils.calculate4Sides(sides, angles);

                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();
                
                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = Utils.line(pleatsRotTras[0]["V1"].X + trs.X, -pleatsRotTras[0]["V1"].Y + trs.Y, pleatsRotTras[0]["V2"].X + trs.X, -pleatsRotTras[0]["V2"].Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void side2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                sides[1] = (double)this.side2.Value;
                sides = Utils.calculate4Sides(sides, angles);

                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();

                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = Utils.line(pleatsRotTras[1]["V1"].X + trs.X, -pleatsRotTras[1]["V1"].Y + trs.Y, pleatsRotTras[1]["V2"].X + trs.X, -pleatsRotTras[1]["V2"].Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void angle1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                decimal anglesum = (decimal)this.angle1.Value + (decimal)this.angle2.Value + (decimal)this.angle3.Value;
                this.angle4.Value = 360 - anglesum;
                angles[0] = Utils.DegToRad((double)this.angle1.Value); 
                angles[3] = Utils.DegToRad((double)this.angle4.Value);
                sides = Utils.calculate4Sides(sides, angles);
                renderAllCanvasWithAngleFocus(0);
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void angle2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                decimal anglesum = (decimal)this.angle1.Value + (decimal)this.angle2.Value + (decimal)this.angle3.Value;
                this.angle4.Value = 360 - anglesum;
                angles[1] = Utils.DegToRad((double)this.angle2.Value);
                angles[3] = Utils.DegToRad((double)this.angle4.Value);
                sides = Utils.calculate4Sides(sides, angles);
                renderAllCanvasWithAngleFocus(1);
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void angle3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                decimal anglesum = (decimal)this.angle1.Value + (decimal)this.angle2.Value + (decimal)this.angle3.Value;
                this.angle4.Value = 360 - anglesum;
                angles[2] = Utils.DegToRad((double)this.angle3.Value);
                angles[3] = Utils.DegToRad((double)this.angle4.Value);
                sides = Utils.calculate4Sides(sides, angles);
                renderAllCanvasWithAngleFocus(2);
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void centerX_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                (decimalSeparator == "," && e.Key == Key.OemComma) || (decimalSeparator == "." && e.Key == Key.OemPeriod))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void centerX_KeyUp(object sender, KeyEventArgs e)
        {
            Match m = Regex.Match(this.centerX.Text, @"[0-9]+([\.,]([0-9]{1,3})?)?");
            if (m.Value != null)
                this.centerX.Text = "" + m.Value;
            else
                this.centerX.Text = "" + this.centerX.Value;
        }

        private void centerX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();

                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = Utils.line(pleatsRotTras[0]["V1"].X + trs.X, -pleatsRotTras[0]["V1"].Y + trs.Y, inputCenter.X + trs.X, -pleatsRotTras[0]["V1"].Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void centerY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();

                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = Utils.line(pleatsRotTras[0]["V1"].X + trs.X, -pleatsRotTras[0]["V1"].Y + trs.Y, pleatsRotTras[0]["V1"].X + trs.X, -inputCenter.Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            renderInputNode();
            renderCreasePattern();
        }

        private void creasePatternCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.scaleCanvas.CenterX = this.creasePatternCanvas.Width / 2;
            this.scaleCanvas.CenterY = this.creasePatternCanvas.Height / 2;

            if (e.Delta > 0)
            {
                this.scaleCanvas.ScaleX *= 1.1;
                this.scaleCanvas.ScaleY *= 1.1;
            }
            else
            {
                if (this.scaleCanvas.ScaleX > 1.2)
                {
                    this.scaleCanvas.ScaleX /= 1.1;
                    this.scaleCanvas.ScaleY /= 1.1;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Title = "Save";
            saveFileDialog.Filter = "SVG Image (*.svg)|*.svg | PNG Image (*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {

                if (saveFileDialog.FilterIndex == 1)
                {
                    var svg = new SvgDocument();
                    svg.Width = 500;
                    svg.Height = 500;

                    var colorServer = new SvgColourServer(System.Drawing.Color.Black);

                    var group = new SvgGroup { Fill = SvgPaintServer.None, Stroke = colorServer };
                    
                    svg.Children.Add(group);

                    foreach (var draw in this.creasePatternCanvas.Children)
                    {
                        var geometry = new Object();
                        if (draw.GetType() == typeof(Line))
                        {
                            geometry = ((Line)draw).RenderedGeometry;
                        }
                        else
                        {
                            continue;
                        }

                        var pathGeometry = PathGeometry.CreateFromGeometry((Geometry)geometry);
                        var s = XamlWriter.Save(pathGeometry);

                        if (!String.IsNullOrEmpty(s))
                        {
                            var element = XElement.Parse(s);

                            var data = element.Attribute("Figures")?.Value;

                            if (!String.IsNullOrEmpty(data))
                            {
                                group.Children.Add(new SvgPath
                                {
                                    PathData = SvgPathBuilder.Parse(data),
                                    Fill = SvgPaintServer.None,
                                    Stroke = colorServer
                                });
                            }
                        }
                    }

                    svg.Write(saveFileDialog.FileName, false);
                }

                if (saveFileDialog.FilterIndex == 2)
                {
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(creasePatternCanvas);
                    double dpi = 96d;

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        VisualBrush vb = new VisualBrush(creasePatternCanvas);
                        dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                    }

                    rtb.Render(dv);

                    BitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                    try
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();

                        pngEncoder.Save(ms);
                        ms.Close();

                        System.IO.File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
                    }
                    catch (Exception err)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                            err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error
                        );
                    }
                }
            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.angle1.Value = 90; this.angle2.Value = 90; this.angle3.Value = 90;
            this.side1.Value = 50; this.side2.Value = 50;
            this.centerX.Value = 25; this.centerY.Value = 25;
            renderInputNode();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            this.scaleCanvas.CenterX = this.creasePatternCanvas.Width / 2;
            this.scaleCanvas.CenterY = this.creasePatternCanvas.Height / 2;
            this.scaleCanvas.ScaleX *= 1.1;
            this.scaleCanvas.ScaleY *= 1.1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.scaleCanvas.CenterX = this.creasePatternCanvas.Width / 2;
            this.scaleCanvas.CenterY = this.creasePatternCanvas.Height / 2;
            if (this.scaleCanvas.ScaleX > 1.2)
            {
                this.scaleCanvas.ScaleX /= 1.1;
                this.scaleCanvas.ScaleY /= 1.1;
            }
        }

        private void inputNodeCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.inputNodeScaleCanvas.CenterX = this.inputNodeCanvas.Width / 2;
            this.inputNodeScaleCanvas.CenterY = this.inputNodeCanvas.Height / 2;

            if (e.Delta > 0)
            {
                this.inputNodeScaleCanvas.ScaleX *= 1.1;
                this.inputNodeScaleCanvas.ScaleY *= 1.1;
            }
            else
            {
                if (this.inputNodeScaleCanvas.ScaleX > 1.2)
                {
                    this.inputNodeScaleCanvas.ScaleX /= 1.1;
                    this.inputNodeScaleCanvas.ScaleY /= 1.1;
                }
            }
        }
    }
}
