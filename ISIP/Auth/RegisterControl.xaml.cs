using System;
using System.Collections.Generic;
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
using System.Net.Mail;
using System.Net;
using System.Data;
using WorkWithCrypt;
using System.Text.RegularExpressions;

namespace ISIP.Auth
{
    /// <summary>
    /// Логика взаимодействия для RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl
    {
        Grid MainGrid = Application.Current.MainWindow.Content as Grid;
        SqlConnection Connection;

        public RegisterControl(SqlConnection Connection)
        {
            InitializeComponent();
            this.Connection = Connection;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AuthControl());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.Open();

                SqlCommand CheckLoginCmd = new SqlCommand(@"SELECT Count(*) FROM Auth_User
                                        WHERE Login=@Login COLLATE Cyrillic_General_CS_AS", Connection);
                CheckLoginCmd.Parameters.AddWithValue("@Login", Email.Text);
                int LoginCount = (int)CheckLoginCmd.ExecuteScalar();

                MailAddress EMail;
                try
                {
                    EMail = new MailAddress(Email.Text);
                }
                catch
                {
                    MessageBox.Show("Неверный электронный адрес!", "Ошибка регистрации");
                    return;
                }

                if (Pass.Password.Length < 8)
                {
                    MessageBox.Show("Слишком короткий пароль!", "Ошибка регистрации");
                    return;
                }

                if (Pass.Password != PassCheck.Password)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка регистрации");
                    return;
                }

                Regex PassRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[0-9]).{8,16}$");

                if (!PassRegex.IsMatch(Pass.Password))
                {
                    MessageBox.Show("Слишком простой пароль!", "Ошибка регистрации");
                    return;
                }

                if (LoginCount == 0)
                {
                    SqlCommand AuthUserInsertCmd = new SqlCommand("Auth_UserInsertCommand", Connection);
                    AuthUserInsertCmd.CommandType = CommandType.StoredProcedure;
                    AuthUserInsertCmd.Parameters.AddWithValue("@Login", Email.Text);
                    AuthUserInsertCmd.Parameters.AddWithValue("@Password", Crypter.Encode(Pass.Password));
                    AuthUserInsertCmd.Parameters.AddWithValue("@Active", 0);
                    AuthUserInsertCmd.Parameters.AddWithValue("@Employee_ID", DBNull.Value);
                    AuthUserInsertCmd.Parameters.AddWithValue("@Computer_Name", DBNull.Value);
                    AuthUserInsertCmd.Parameters.AddWithValue("@Check_Code", DBNull.Value);
                    AuthUserInsertCmd.ExecuteNonQuery();

                    SqlCommand GetLastAuthUserCmd = new SqlCommand("SELECT IDENT_CURRENT('Auth_User')", Connection);
                    int ID = Int32.Parse(GetLastAuthUserCmd.ExecuteScalar().ToString());

                    Connection.Close();

                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new RegisterVerificationControl(Connection, ID, EMail));
                }
                else
                    MessageBox.Show("Такой пользователь уже существует!", "Ошибка регистрации");
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
