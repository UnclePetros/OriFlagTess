using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            this.mountainFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25 });
            this.mountainFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(1) });
            this.mountainFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(2) });
            this.mountainFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(3) });
            this.valleyFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25 });
            this.valleyFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(1) });
            this.valleyFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(2) });
            this.valleyFoldComboBox.Items.Add(new Line { X1 = 1, X2 = 200, Y1 = 10, Y2 = 10, Stroke = Brushes.Black, StrokeThickness = 1, Height = 25, StrokeDashArray = Utils.toDashArrayPattern(3) });
            this.mountainFoldComboBox.SelectedIndex = int.Parse(System.Configuration.ConfigurationManager.AppSettings["mountainFold"]);
            this.valleyFoldComboBox.SelectedIndex = int.Parse(System.Configuration.ConfigurationManager.AppSettings["valleyFold"]);
            var mountainFoldHtmlColor = System.Configuration.ConfigurationManager.AppSettings["mountainFoldColor"];
            this.mountainFoldColorPicker.SelectedColor = ((SolidColorBrush)new BrushConverter().ConvertFromString(mountainFoldHtmlColor)).Color;
            var valleyFoldHtmlColor = System.Configuration.ConfigurationManager.AppSettings["valleyFoldColor"];
            this.valleyFoldColorPicker.SelectedColor = ((SolidColorBrush)new BrushConverter().ConvertFromString(valleyFoldHtmlColor)).Color;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mountainFoldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["mountainFold"].Value = "" + this.mountainFoldComboBox.SelectedIndex;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void valleyFoldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["valleyFold"].Value = "" + this.valleyFoldComboBox.SelectedIndex;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void mountainFoldColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["mountainFoldColor"].Value = "" + this.mountainFoldColorPicker.SelectedColor.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void valleyFoldColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["valleyFoldColor"].Value = "" + this.valleyFoldColorPicker.SelectedColor.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
