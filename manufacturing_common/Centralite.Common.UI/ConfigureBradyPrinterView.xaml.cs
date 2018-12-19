using System.Windows.Controls;

namespace Centralite.Common.UI
{
    /// <summary>
    /// Interaction logic for ConfigureBradyPrinterView.xaml
    /// </summary>
    public partial class ConfigureBradyPrinterView : UserControl
    {
        public ConfigureBradyPrinterView()
        {
            InitializeComponent();
        }

        private void configureButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.ShowDialog();
        }
    }
}
