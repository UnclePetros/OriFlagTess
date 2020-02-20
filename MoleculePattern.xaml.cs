using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Flagstone_Tessellation___Molecule_construction
{
    /// <summary>
    /// Interaction logic for MoleculePattern.xaml
    /// </summary>
    public partial class MoleculePattern : Window
    {
        double angle1 = 0.0D;
        double angle2 = 0.0D;
        double angle3 = 0.0D;
        double angle4 = 0.0D;
        double side1 = 0.0D;
        double side2 = 0.0D;
        double centerX = 0.0D;
        double centerY = 0.0D;

        public MoleculePattern()
        {
            InitializeComponent();
        }

        public MoleculePattern(Dictionary<string,decimal> inputData)
        {
            InitializeComponent();

            this.angle1 = (double)inputData["angle1"];
            this.angle2 = (double)inputData["angle2"];
            this.angle3 = (double)inputData["angle3"];
            this.angle4 = (double)inputData["angle4"];
            this.side1 = (double)inputData["side1"];
            this.side2 = (double)inputData["side2"];
            this.centerX = (double)inputData["centerX"];
            this.centerY = (double)inputData["centerY"];

            Point moleculeCenter = new Point(this.centerX, this.centerY);
            Molecule mol = new Molecule(moleculeCenter, 4);
            mol.setAngles(new List<double> { this.angle1, this.angle2, this.angle3, this.angle4 });
            mol.setSides(Utils.calculate4Sides(new List<double> { this.side1, this.side2}, mol.getAngles()));
            mol.calculatePleats();

            this.creasePatternCanvas.ClipToBounds = true;
            this.creasePatternCanvas.Background = new SolidColorBrush(Colors.Beige);

            Point canvasCenter = new Point(this.creasePatternCanvas.Width / 2, this.creasePatternCanvas.Height / 2);
            Point trs = new Point(canvasCenter.X - mol.getCenter().X, canvasCenter.Y - mol.getCenter().Y);
            this.creasePatternCanvas.Children.Add(Utils.ellipse(4, 4, canvasCenter.X - 2, canvasCenter.Y - 2, Brushes.Brown));

            List<Dictionary<string, Point>> pleats, pleatsRotTras;
            pleats = mol.getPleats();
            pleatsRotTras = new List<Dictionary<string, Point>>();

            Point traslation = new Point(0,0);
            double angle = 0.0D;

            for (int i = 0; i < mol.getNumPleats(); i++)
            {
                Dictionary<string, Point> pleat = pleats[i];
                Dictionary<string, Point> pleatRotTras = new Dictionary<string, Point>();

                if(i > 0) {
                    angle += mol.getAngles()[i];
                    traslation = new Point(pleatsRotTras[i-1]["B"].X, pleatsRotTras[i-1]["B"].Y);
                }

                foreach (KeyValuePair<string, Point> item in pleat)
                {
                    pleatRotTras.Add(item.Key, Utils.RotTras(item.Value, angle, traslation));

                    if (item.Key == "A" || item.Key == "B" || item.Key == "PA" || item.Key == "PB")
                        pleatRotTras.Add(item.Key + "-END", Utils.RotTras(new Point(item.Value.X, item.Value.Y - 500), angle, traslation));
                }

                pleatsRotTras.Add(pleatRotTras);

                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, pleatRotTras["A"].X + trs.X, pleatRotTras["A"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, pleatRotTras["B"].X + trs.X, pleatRotTras["B"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, pleatRotTras["PA"].X + trs.X, pleatRotTras["PA"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, pleatRotTras["PB"].X + trs.X, pleatRotTras["PB"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PA"].X + trs.X, pleatRotTras["PA"].Y + trs.Y, pleatRotTras["PB"].X + trs.X, pleatRotTras["PB"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["A"].X + trs.X, pleatRotTras["A"].Y + trs.Y, pleatRotTras["PA"].X + trs.X, pleatRotTras["PA"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["B"].X + trs.X, pleatRotTras["B"].Y + trs.Y, pleatRotTras["PB"].X + trs.X, pleatRotTras["PB"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["A"].X + trs.X, pleatRotTras["A"].Y + trs.Y, pleatRotTras["A-END"].X + trs.X, pleatRotTras["A-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PA"].X + trs.X, pleatRotTras["PA"].Y + trs.Y, pleatRotTras["PA-END"].X + trs.X, pleatRotTras["PA-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["PB"].X + trs.X, pleatRotTras["PB"].Y + trs.Y, pleatRotTras["PB-END"].X + trs.X, pleatRotTras["PB-END"].Y + trs.Y, Brushes.Brown));
                this.creasePatternCanvas.Children.Add(Utils.line(pleatRotTras["B"].X + trs.X, pleatRotTras["B"].Y + trs.Y, pleatRotTras["B-END"].X + trs.X, pleatRotTras["B-END"].Y + trs.Y, Brushes.Brown));

                if (i > 0)
                {
                    this.creasePatternCanvas.Children.Add(Utils.line(pleatsRotTras[i - 1]["PO"].X + trs.X, pleatsRotTras[i - 1]["PO"].Y + trs.Y, pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, Brushes.Brown));
                    if(i == mol.getNumPleats()-1)
                        this.creasePatternCanvas.Children.Add(Utils.line(pleatsRotTras[0]["PO"].X + trs.X, pleatsRotTras[0]["PO"].Y + trs.Y, pleatRotTras["PO"].X + trs.X, pleatRotTras["PO"].Y + trs.Y, Brushes.Brown));
                }
            }
        }

        private void creasePatternCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.scaleCanvas.CenterX = this.creasePatternCanvas.Width / 2;
            this.scaleCanvas.CenterY = this.creasePatternCanvas.Height / 2;
            
            if (e.Delta > 0)
            {
                this.scaleCanvas.ScaleX *= 1.1;
                this.scaleCanvas.ScaleY *= 1.1;
                this.translateCanvas.Y = 0;
            }
            else
            {
                if (this.scaleCanvas.ScaleX > 1)
                {
                    this.scaleCanvas.ScaleX /= 1.1;
                    this.scaleCanvas.ScaleY /= 1.1;
                }
            }
        }
    }
}
