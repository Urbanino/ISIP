using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
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
using WorkWithDatabase;
using WorkWithRegistry;
using WorkWithCrypt;

namespace ISIP.Auth
{
    /// <summary>
    /// Логика взаимодействия для ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl
    {
        Grid MainGrid = Application.Current.MainWindow.Content as Grid;

        public ConnectionControl()
        {
            InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsEnabled = false;
            DataSourceList.Items.Clear();
            DataSourceList.Items.Add("localhost");
            SqlDataSourceEnumerator ServerList = SqlDataSourceEnumerator.Instance;
            Task.Run(() =>
            {
                DataTable ServerTable = ServerList.GetDataSources();
                foreach (DataRow row in ServerTable.Rows)
                {
                    DataSourceList.Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        DataSourceList.Items.Add(row[0]);
                    }));
                }

                RefreshButton.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    RefreshButton.IsEnabled = true;
                }));

            });

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                SqlConnection TestConnection = new SqlConnection();
                TestConnection = Connector.Connection(DataSourceList.SelectedItem.ToString(),
                    User.Text, Pass.Password, "master");
                TestConnection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", TestConnection))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DatabaseList.Items.Add(dr[0].ToString());
                        }
                    }
                }
                TestConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataSourceList.Items.Add("localhost");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ToAuthControl();
        }

        private void ToAuthControl()
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AuthControl());
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey AuthInfo = RegistryInfo.Key();

                AuthInfo.SetValue("DataSource", Crypter.Encode(DataSourceList.SelectedItem.ToString()));
                AuthInfo.SetValue("InitialCatalog", Crypter.Encode(DatabaseList.SelectedItem.ToString()));
                AuthInfo.SetValue("UserID", Crypter.Encode(User.Text));
                AuthInfo.SetValue("Password", Crypter.Encode(Pass.Password));

                ToAuthControl();
            }
            catch
            {
                MessageBox.Show("Не удалось записать данные в реестр!", "Ошибка сохранения подключения");
            }


        }
    }
}
