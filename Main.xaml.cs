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
using System.Windows.Shapes;
using System.Xml.Linq;
using Xceed.Wpf.Toolkit;

namespace Flagstone_Tessellation___Molecule_construction
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        int MAX_PLEAT = int.Parse(System.Configuration.ConfigurationManager.AppSettings["maxPleat"]);
        int numPleats = 4; 
        List<double> sides;
        List<double> pleatCenters;
        List<double> angles;
        List<Dictionary<string, Point>> pleatsRotTras;
        public Main()
        {
            NameScope.SetNameScope(this, new NameScope());
            InitializeComponent();
            InitUI(4); //Initialize UI with a 4-pleat molecule

            //Node View initialization
            this.inputNodeCanvas.ClipToBounds = true;
            this.inputNodeCanvas.Background = new SolidColorBrush(Colors.Beige);

            //Crease Pattern View initialization
            this.creasePatternCanvas.ClipToBounds = true;
            this.creasePatternCanvas.Background = new SolidColorBrush(Colors.Beige);

            for (int i = 3; i <= MAX_PLEAT; i++)
            {
                this.inputMoleculeTypeComboBox.Items.Add(i + "-pleat flagstone molecule");
            }
            this.inputMoleculeTypeComboBox.SelectedIndex = numPleats - 3;
        }

        private void InitUI(int numPleats) {
            this.numPleats = numPleats;
            this.angles = new List<double>();
            this.sides = new List<double>();
            this.pleatCenters = new List<double>();

            //Set input angles wpf elements
            this.inputAnglesGrid.RowDefinitions.Clear();
            this.inputAnglesGrid.ColumnDefinitions.Clear();
            this.inputSidesGrid.RowDefinitions.Clear();
            this.inputSidesGrid.ColumnDefinitions.Clear();
            this.inputPleatCenterGrid.RowDefinitions.Clear();
            this.inputPleatCenterGrid.ColumnDefinitions.Clear();
            
            this.inputAnglesGrid.RowDefinitions.Add(new RowDefinition());
            this.inputSidesGrid.RowDefinitions.Add(new RowDefinition());
            this.inputPleatCenterGrid.RowDefinitions.Add(new RowDefinition());
            
            for (int i = 0; i < numPleats*2; i++)
            {
                this.inputAnglesGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.inputSidesGrid.ColumnDefinitions.Add(new ColumnDefinition());
                this.inputPleatCenterGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < numPleats; i++)
            {
                Label angleLabel = new Label();
                angleLabel.Content = "Angle"+(i+1);
                angleLabel.VerticalAlignment = VerticalAlignment.Center;
                angleLabel.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(angleLabel, 0);
                Grid.SetColumn(angleLabel, i*2);
                this.inputAnglesGrid.Children.Add(angleLabel);

                DecimalUpDown angleDUP = new DecimalUpDown();
                angleDUP.Name = "angle"+(i+1);
                angleDUP.Width = 50;
                angleDUP.Height = 30;
                angleDUP.Margin = new Thickness(0, 10, 10, 10);
                angleDUP.Minimum = 0.01M;
                angleDUP.Maximum = 179.99M;
                angleDUP.Increment = 0.1M;
                angleDUP.Value = 180-180*(numPleats-2)/numPleats;
                angleDUP.Tag = i;
                angleDUP.ValueChanged += angle_ValueChanged;
                if (i == numPleats - 1)
                    angleDUP.IsEnabled = false;
                Grid.SetRow(angleDUP, 0);
                Grid.SetColumn(angleDUP, i*2+1);
                this.inputAnglesGrid.Children.Add(angleDUP);
                NameScope.GetNameScope(this).RegisterName(angleDUP.Name, angleDUP);

                Label sideLabel = new Label();
                sideLabel.Content = "Side" + (i + 1);
                sideLabel.VerticalAlignment = VerticalAlignment.Center;
                sideLabel.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(sideLabel, 0);
                Grid.SetColumn(sideLabel, i * 2);
                this.inputSidesGrid.Children.Add(sideLabel);

                DecimalUpDown sideDUP = new DecimalUpDown();
                sideDUP.Name = "side" + (i + 1);
                sideDUP.Width = 50;
                sideDUP.Height = 30;
                sideDUP.Margin = new Thickness(0, 10, 10, 10);
                sideDUP.Minimum = 0.01M;
                sideDUP.Maximum = 1000M;
                sideDUP.Increment = 0.1M;
                sideDUP.Value = 50;
                sideDUP.Tag = i;
                sideDUP.ValueChanged += side_ValueChanged;
                if (i == numPleats - 1 || i == numPleats - 2)
                    sideDUP.IsEnabled = false;
                Grid.SetRow(sideDUP, 0);
                Grid.SetColumn(sideDUP, i * 2 + 1);
                this.inputSidesGrid.Children.Add(sideDUP);
                NameScope.GetNameScope(this).RegisterName(sideDUP.Name, sideDUP);

                Label pleatCenterLabel = new Label();
                pleatCenterLabel.Content = "PleatC" + (i + 1);
                pleatCenterLabel.VerticalAlignment = VerticalAlignment.Center;
                pleatCenterLabel.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(pleatCenterLabel, 0);
                Grid.SetColumn(pleatCenterLabel, i * 2);
                this.inputPleatCenterGrid.Children.Add(pleatCenterLabel);

                DecimalUpDown pleatCenterDUP = new DecimalUpDown();
                pleatCenterDUP.Name = "pleatCenter" + (i + 1);
                pleatCenterDUP.Width = 50;
                pleatCenterDUP.Height = 30;
                pleatCenterDUP.Margin = new Thickness(0, 10, 10, 10);
                pleatCenterDUP.Minimum = 0.01M;
                pleatCenterDUP.Maximum = 1000M;
                pleatCenterDUP.Increment = 0.1M;
                pleatCenterDUP.Value = sideDUP.Value/2;
                pleatCenterDUP.Tag = i;
                pleatCenterDUP.ValueChanged += pleatCenter_ValueChanged;
                if (this.pleatCenterManualCheckBox.IsChecked.Value)
                    pleatCenterDUP.IsEnabled = false;
                Grid.SetRow(pleatCenterDUP, 0);
                Grid.SetColumn(pleatCenterDUP, i * 2 + 1);
                this.inputPleatCenterGrid.Children.Add(pleatCenterDUP);
                NameScope.GetNameScope(this).RegisterName(pleatCenterDUP.Name, pleatCenterDUP);

                this.angles.Add(Utils.DegToRad((double)angleDUP.Value));
                this.sides.Add((double)sideDUP.Value);
                this.pleatCenters.Add((double)pleatCenterDUP.Value);
            }
            this.pleatCenterManualCheckBox.IsChecked = true;
            //Assign default center with polygon apotema
            this.centerX.Value = (decimal)50/2;
            this.centerY.Value = (decimal)Math.Round((50 / (2 * Utils.Tan(Math.PI / numPleats))),3);

            renderInputNode();
            renderCreasePattern();
        }

        private void DisposeUI(int numPleats)
        {
            this.inputAnglesGrid.Children.Clear();
            this.inputSidesGrid.Children.Clear();
            this.inputPleatCenterGrid.Children.Clear();
            for (int i = 0; i < numPleats; i++)
            {
                NameScope.GetNameScope(this).UnregisterName("angle"+(i + 1));
                NameScope.GetNameScope(this).UnregisterName("side" + (i + 1));
                NameScope.GetNameScope(this).UnregisterName("pleatCenter" + (i + 1));
            }
        }

        private List<Dictionary<string, Point>> renderInputNode()
        {
            this.inputNodeCanvas.Children.Clear();
            Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
            Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
            Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel

            this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, inputCenter.X + trs.X - 2, -inputCenter.Y + trs.Y - 2, Brushes.Brown));

            List<Dictionary<string, Point>> pleatsRotTras = new List<Dictionary<string, Point>>();
            Point traslation = new Point(0, 0);
            double angle = 0.0D;

            for (int i = 0; i < this.numPleats; i++)
            {
                Point A = new Point(0, 0);
                Point B = new Point(sides[i], A.Y);
                Point A_END = new Point(0, -inputNodeCanvas.Width);
                Point B_END = new Point(sides[i], A_END.Y);
                Point C = new Point(pleatCenters[i], A.Y);
                Point C_END = new Point(pleatCenters[i], A_END.Y);

                Dictionary<string, Point> pleat = new Dictionary<string, Point>();
                pleat.Add("V1", A);
                pleat.Add("V2", B);
                pleat.Add("V1-END", A_END);
                pleat.Add("V2-END", B_END);
                pleat.Add("C", C);
                pleat.Add("C-END", C_END);

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
                this.inputNodeCanvas.Children.Add(Utils.lineDash(pleatRotTras["C"].X + trs.X, -pleatRotTras["C"].Y + trs.Y, pleatRotTras["C-END"].X + trs.X, -pleatRotTras["C-END"].Y + trs.Y, Brushes.Brown));
            }

            for (int i = 0; i < this.numPleats; i++)
            {
                this.inputNodeCanvas.Children.Add(Utils.polygon(
                    new PointCollection {
                        new Point(pleatsRotTras[i]["V1"].X + trs.X, -pleatsRotTras[i]["V1"].Y + trs.Y),
                        new Point(pleatsRotTras[(i+numPleats-1)%numPleats]["V2"].X + trs.X, -pleatsRotTras[(i+numPleats-1)%numPleats]["V2"].Y + trs.Y),
                        new Point(pleatsRotTras[i]["V1-END"].X + trs.X, -pleatsRotTras[i]["V1-END"].Y + trs.Y),
                        new Point(pleatsRotTras[(i+numPleats-1)%numPleats]["V2-END"].X + trs.X, -pleatsRotTras[(i+numPleats-1)%numPleats]["V2-END"].Y + trs.Y)
                    },
                    Brushes.LightYellow));
            }

            return pleatsRotTras;
        }

        public void renderCreasePattern()
        {
            this.creasePatternCanvas.Children.Clear();
            Point moleculeCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value);
            Molecule mol = new Molecule(moleculeCenter, this.numPleats);
            mol.setAngles(angles);
            mol.setSides(sides);
            mol.setFoldCenters(pleatCenters);
            mol.setSpacing((double)this.spacing.Value);
            mol.calculatePleats();

            Point canvasCenter = new Point(this.creasePatternCanvas.Width / 2, this.creasePatternCanvas.Height / 2); //Center of the crease pattern canvas panel
            Point trs = new Point(canvasCenter.X - mol.getCenter().X, canvasCenter.Y + mol.getCenter().Y);
            this.creasePatternCanvas.Children.Add(Utils.ellipse(4, 4, mol.getCenter().X + trs.X - 2, -mol.getCenter().Y + trs.Y - 2, Brushes.Brown));

            List<Dictionary<string, Point>> pleats;//, pleatsRotTras;
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
                this.Log.Text += "Pleat" + (i + 1) + ":\t P( " + Math.Round(pleat["P"].X, 3) + " ; " + Math.Round(pleat["P"].Y, 3) + " )\r\n";
                this.Log.Text += "\t V1( " + Math.Round(pleat["V1"].X, 3) + ";" + Math.Round(pleat["V1"].Y, 3) + " )\r\n";
                this.Log.Text += "\t V2( " + Math.Round(pleat["V2"].X, 3) + " ; " + Math.Round(pleat["V2"].Y, 3) + " )\r\n";
                this.Log.Text += "\t L (" + Math.Round(pleat["L"].X, 3) + ";" + Math.Round(pleat["L"].Y, 3) + " )\r\n";
                this.Log.Text += "\t M( " + Math.Round(pleat["M"].X, 3) + " ; " + Math.Round(pleat["M"].Y, 3) + " )\r\n";
                this.Log.Text += "\t N( " + Math.Round(pleat["N"].X, 3) + " ; " + Math.Round(pleat["N"].Y, 3) + " ) \r\n";
                this.Log.Text += "\t Side" + (i + 1) + " = " + mol.getSides()[i] + "\r\n";
                this.Log.Text += "\t Angle1 = " + Utils.RadToDeg(mol.getAngles()[i]) + "\r\n";
                this.Log.Text += "\t Angle2 = " + Utils.RadToDeg(mol.getAngles()[(i+1) % mol.getNumPleats()]) + "\r\n";
                this.Log.Text += "\t PleatC" + (i + 1) + " = " + mol.getFoldCenters()[i] + " \r\n";

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

        private void renderAllCanvasWithAngleFocus(int inputIndex)
        {
            List<Dictionary<string, Point>> pleatsRotTras;
            pleatsRotTras = renderInputNode();

            //Highlight focused input on input node canvas
            Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
            Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
            Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
            Line hl1 = Utils.line(pleatsRotTras[inputIndex]["V1"].X + trs.X, -pleatsRotTras[inputIndex]["V1"].Y + trs.Y, pleatsRotTras[inputIndex]["V1-END"].X + trs.X, -pleatsRotTras[inputIndex]["V1-END"].Y + trs.Y, Brushes.Red);
            hl1.StrokeThickness = 1.5d;
            this.inputNodeCanvas.Children.Add(hl1);
            Line hl2 = Utils.line(pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].X + trs.X, -pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].Y + trs.Y, pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2-END"].X + trs.X, -pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2-END"].Y + trs.Y, Brushes.Red);
            hl2.StrokeThickness = 1.5d;
            this.inputNodeCanvas.Children.Add(hl2);
            this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl1.X1 - 2, hl1.Y1 - 2, Brushes.Red));

            var g = new StreamGeometry();
            using (var gc = g.Open())
            {
                Point arcStart = new Point(
                    pleatsRotTras[inputIndex]["V1"].X - (pleatsRotTras[inputIndex]["V1"].X - pleatsRotTras[inputIndex]["V1-END"].X) / 18 + trs.X,
                    -pleatsRotTras[inputIndex]["V1"].Y - (-pleatsRotTras[inputIndex]["V1"].Y + pleatsRotTras[inputIndex]["V1-END"].Y) / 18 + trs.Y
                );
                Point arcEnd = new Point(
                    pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].X - (pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].X - pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2-END"].X) / 18 + trs.X,
                    -pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].Y - (-pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2"].Y + pleatsRotTras[(inputIndex + numPleats - 1) % numPleats]["V2-END"].Y) / 18 + trs.Y
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

        private void pleatCenterManualCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.numPleats; i++)
            {
                DecimalUpDown pleatCenterDUP = ((DecimalUpDown)this.mainGrid.FindName("pleatCenter" + (i + 1)));
                DecimalUpDown sideDUP = ((DecimalUpDown)this.mainGrid.FindName("side" + (i + 1)));
                pleatCenterDUP.IsEnabled = false;
                pleatCenterDUP.Value = sideDUP.Value / 2;
            }
        }

        private void pleatCenterManualCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.numPleats; i++)
            {
                ((DecimalUpDown)this.mainGrid.FindName("pleatCenter" + (i + 1))).IsEnabled = true;
            }
        }

        private void angle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DecimalUpDown angleDUP = (DecimalUpDown)sender;
            if (e.NewValue != null)
            {
                decimal anglesum = 0.0M;
                for (int i = 0; i < this.numPleats-1; i++)
                {
                    anglesum += (decimal)((DecimalUpDown)this.mainGrid.FindName( "angle"+(i+1) )).Value;
                }
                ((DecimalUpDown)this.mainGrid.FindName("angle" + this.numPleats)).Value = 360 - anglesum;
                this.angles[(int)angleDUP.Tag] = Utils.DegToRad((double)angleDUP.Value);
                this.angles[this.numPleats-1] = Utils.DegToRad((double)(360 - anglesum));
                sides = Utils.calculatePolygonSides(sides, angles);
                ((DecimalUpDown)this.mainGrid.FindName("side" + (this.numPleats))).Value = (decimal)sides[this.numPleats - 1];
                ((DecimalUpDown)this.mainGrid.FindName("side" + (this.numPleats - 1))).Value = (decimal)sides[this.numPleats - 2];
                renderAllCanvasWithAngleFocus((int)angleDUP.Tag);
            }
            else
                angleDUP.Value = (decimal)e.OldValue;
        }

        private void side_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DecimalUpDown sideDUP = (DecimalUpDown)sender;
            if (e.NewValue != null)
            {
                this.sides[(int)sideDUP.Tag] = (double)sideDUP.Value;
                sides = Utils.calculatePolygonSides(sides, angles);
                ((DecimalUpDown)this.mainGrid.FindName("side" + (this.numPleats))).Value = (decimal)sides[this.numPleats - 1];
                ((DecimalUpDown)this.mainGrid.FindName("side" + (this.numPleats - 1))).Value = (decimal)sides[this.numPleats - 2];
                
                if (this.pleatCenterManualCheckBox.IsChecked.Value)
                    for (int i = 0; i < this.numPleats; i++)
                    {
                        DecimalUpDown pleatCenterDUP = ((DecimalUpDown)this.mainGrid.FindName("pleatCenter" + (i + 1)));
                        DecimalUpDown currSideDUP = ((DecimalUpDown)this.mainGrid.FindName("side" + (i + 1)));
                        pleatCenterDUP.Value = currSideDUP.Value / 2;
                    }

                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();

                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = Utils.line(pleatsRotTras[(int)sideDUP.Tag]["V1"].X + trs.X, -pleatsRotTras[(int)sideDUP.Tag]["V1"].Y + trs.Y, pleatsRotTras[(int)sideDUP.Tag]["V2"].X + trs.X, -pleatsRotTras[(int)sideDUP.Tag]["V2"].Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                sideDUP.Value = (decimal)e.OldValue;
        }

        private void pleatCenter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DecimalUpDown pleatCenterDUP = (DecimalUpDown)sender;
            if (e.NewValue != null)
            {
                this.pleatCenters[(int)pleatCenterDUP.Tag] = (double)pleatCenterDUP.Value;

                List<Dictionary<string, Point>> pleatsRotTras;
                pleatsRotTras = renderInputNode();

                //Highlight focused input on input node canvas
                Point canvasCenter = new Point(this.inputNodeCanvas.Width / 2, this.inputNodeCanvas.Height / 2);  //Center of the node canvas panel
                Point inputCenter = new Point((double)this.centerX.Value, (double)this.centerY.Value); //Center of the node
                Point trs = new Point(canvasCenter.X - inputCenter.X, canvasCenter.Y + inputCenter.Y); //Translation needed to move the node figure to the center of the node canvas panel
                Line hl = new Line();
                if ((int)pleatCenterDUP.Tag % 2 == 0)
                    hl = Utils.line(pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].X + trs.X, -pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].Y + trs.Y, pleatsRotTras[(int)pleatCenterDUP.Tag]["C"].X + trs.X, -pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].Y + trs.Y, Brushes.Red);
                else
                    hl = Utils.line(pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].X + trs.X, -pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].Y + trs.Y, pleatsRotTras[(int)pleatCenterDUP.Tag]["V1"].X + trs.X, -pleatsRotTras[(int)pleatCenterDUP.Tag]["C"].Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                pleatCenterDUP.Value = (decimal)e.OldValue;
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
                Line hl = Utils.line(pleatsRotTras[0]["V1"].X + trs.X, -pleatsRotTras[0]["V1"].Y - inputCenter.Y + trs.Y, inputCenter.X + trs.X, -pleatsRotTras[0]["V1"].Y - inputCenter.Y + trs.Y, Brushes.Red);
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
                Line hl = Utils.line(pleatsRotTras[0]["V1"].X + inputCenter.X + trs.X, -pleatsRotTras[0]["V1"].Y + trs.Y, pleatsRotTras[0]["V1"].X + inputCenter.X + trs.X, -inputCenter.Y + trs.Y, Brushes.Red);
                hl.StrokeThickness = 1.5d;
                this.inputNodeCanvas.Children.Add(hl);
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X1 - 2, hl.Y1 - 2, Brushes.Red));
                this.inputNodeCanvas.Children.Add(Utils.ellipse(4, 4, hl.X2 - 2, hl.Y2 - 2, Brushes.Red));

                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
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
            DisposeUI(this.numPleats);
            InitUI(this.numPleats);
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

        private void inputMoleculeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisposeUI(this.numPleats);
            this.numPleats = ((ComboBox)sender).SelectedIndex + 3;
            //System.Windows.MessageBox.Show(""+e.AddedItems[0]);
            InitUI(this.numPleats);
        }

        private void autoCenter_Click(object sender, RoutedEventArgs e)
        {
            double x = 0.0D, y = 0.0D;
            //calculate centroid
            for (int i = 0; i < this.numPleats; i++)
            {
                x += pleatsRotTras[i]["V2"].X;
                y += pleatsRotTras[i]["V2"].Y;
            }
            this.centerX.Value = (decimal)Math.Round((x / this.numPleats),3);
            this.centerY.Value = (decimal)Math.Round((y / this.numPleats),3);
        }

        private void spacing_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                renderCreasePattern();
            }
            else
                ((DecimalUpDown)sender).Value = (decimal)e.OldValue;
        }
    }
}
