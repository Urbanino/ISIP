using ISIP.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ISIP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Children.Add(new SplashControl());
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                Task.Run(() => {
                    AllocConsole();
                    while (true)
                    {
                        switch (Console.ReadLine())
                        {
                            case "clear":
                                Console.Clear();
                                break;
                        }
                    }
                });
            }
        }
    }
}
