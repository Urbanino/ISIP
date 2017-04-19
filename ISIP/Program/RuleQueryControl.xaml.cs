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
    /// Логика взаимодействия для RuleQueryControl.xaml
    /// </summary>
    public partial class RuleQueryControl : UserControl
    {
        SqlConnection Connection;
        int Auth_UserID;
        Grid MainGrid = Application.Current.MainWindow.Content as Grid;

        public RuleQueryControl(SqlConnection Connection, int Auth_UserID)
        {
            InitializeComponent();
            this.Connection = Connection;
            this.Auth_UserID = Auth_UserID;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.Open();
                SqlCommand Rule_QueryInsertCmd = new SqlCommand("Rule_QueryInsertCommand", Connection);
                Rule_QueryInsertCmd.CommandType = CommandType.StoredProcedure;
                Rule_QueryInsertCmd.Parameters.AddWithValue("Employee", FIO.Text);
                Rule_QueryInsertCmd.Parameters.AddWithValue("Post", Post.Text);
                Rule_QueryInsertCmd.Parameters.AddWithValue("Result", Auth_UserID);
                Rule_QueryInsertCmd.ExecuteNonQuery();
                MessageBox.Show("Запрос отправлен","Запрос выдачи прав");
                (Parent as Grid).Children.Clear();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

        }

    }
}
