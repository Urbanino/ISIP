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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISIP.Auth
{
    /// <summary>
    /// Логика взаимодействия для SplashControl.xaml
    /// </summary>
    public partial class SplashControl : UserControl
    {
        Grid MainGrid = Application.Current.MainWindow.Content as Grid;
        public SplashControl()
        {
            InitializeComponent();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AuthControl());
        }
    }
}
