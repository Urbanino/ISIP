using ISIP.Auth;
using ISIP.Program.Collections;
using ISIP.Program.Functional;
using ISIP.Program.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace ISIP.Program
{
    /// <summary>
    /// Логика взаимодействия для MenuControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {
        SqlConnection Connection;
        Grid MainGrid = Application.Current.MainWindow.Content as Grid;
        int Auth_UserID;
        public MenuControl(SqlConnection Connection, int ID)
        {
            InitializeComponent();
            this.Connection = Connection;
            this.Auth_UserID = ID;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Width = 800;
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.MinWidth = 800;
            Application.Current.MainWindow.MinHeight = 600;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            try
            {
                Connection.Open();

                SqlCommand GetEmployeeInfoCmd =
                    new SqlCommand("SELECT Employee_Name, Post_Name FROM EmployeeList WHERE ID=@ID", Connection);
                GetEmployeeInfoCmd.Parameters.AddWithValue("@ID", Auth_UserID);

                DataTable dt = new DataTable();
                dt.Load(GetEmployeeInfoCmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                {
                    Employee.Content = "Гостевой режим";
                    Connection.Close();
                    GeneralGrid.Children.Add(new RuleQueryControl(Connection, Auth_UserID));
                    return;
                }

                Employee.Content = dt.Rows[0].ItemArray[0].ToString();
                Post.Content = dt.Rows[0].ItemArray[1].ToString();

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            GetCollectionList();
        }



        private void ExitButton_Selected(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AuthControl());
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void GetCollectionList()
        {
            GeneralGrid.Children.Clear();
            GeneralGrid.Children.Add(new CollectionMenuControl(Connection, Post.Content.ToString()));
        }

        private void CollectionButton_Selected(object sender, RoutedEventArgs e)
        {
            if (Employee.Content.ToString() == "Гостевой режим") return;
            GetCollectionList();
        }

        private void FunctionButton_Selected(object sender, RoutedEventArgs e)
        {
            GeneralGrid.Children.Clear();
            GeneralGrid.Children.Add(new FunctionMenuControl(Connection, Post.Content.ToString()));
        }

        private void ReportButton_Selected(object sender, RoutedEventArgs e)
        {
            GeneralGrid.Children.Clear();
            GeneralGrid.Children.Add(new ReportMenuControl(Connection, Post.Content.ToString()));
        }
    }
}
