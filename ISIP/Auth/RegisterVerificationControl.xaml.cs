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

namespace ISIP.Auth
{
    /// <summary>
    /// Логика взаимодействия для RegisterVerificationControl.xaml
    /// </summary>
    public partial class RegisterVerificationControl : UserControl
    {
        SqlConnection Connection;
        int ID;

        Grid MainGrid = Application.Current.MainWindow.Content as Grid;
        public RegisterVerificationControl(SqlConnection Connection, int ID, MailAddress Email)
        {
            InitializeComponent();
            this.Connection = Connection;
            this.ID = ID;

            string Code = GenerateCode();

            try
            {
                Connection.Open();
                SqlCommand AuthUserUpdateCmd = new SqlCommand("UPDATE Auth_User SET Check_Code = @Check_Code " +
                    "WHERE (ID=@ID)", Connection);
                AuthUserUpdateCmd.Parameters.AddWithValue("@ID", ID);
                AuthUserUpdateCmd.Parameters.AddWithValue("@Check_Code", Code);
                
                AuthUserUpdateCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка записи кода верификации!");
            }
            finally
            {
                Connection.Close();
            }

            SmtpClient Smtp = new SmtpClient("smtp.gmail.com", 587);
            Smtp.Credentials = new NetworkCredential("isip.app@gmail.com", "isip114201514");
            Smtp.EnableSsl = true;
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress("isip.app@gmail.com", "Сервис ISIP");
            Message.To.Add(Email);
            Message.Subject = "Активация учетной записи";
            Message.Body = "Для активации вашей учетной записи введите следующий код подтверждения: " + Code;

            try
            {
                Smtp.Send(Message);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private string GenerateCode()
        {
            string result = "";
            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                int num = rand.Next(0, 36);
                result += "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[num];
            }
            return result;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new AuthControl());
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.Open();
                SqlCommand GetCheckCodeCmd = new SqlCommand("SELECT Check_Code FROM Auth_User " +
                    "WHERE (ID=@ID)", Connection);
                GetCheckCodeCmd.Parameters.AddWithValue("@ID", ID);
                string Code = GetCheckCodeCmd.ExecuteScalar().ToString();
                
                if (CheckCode.Text == Code)
                {
                    SqlCommand AuthUserUpdateCmd = new SqlCommand("UPDATE Auth_User SET Active = @Active " +
                            "WHERE (ID=@ID)", Connection);
                    AuthUserUpdateCmd.Parameters.AddWithValue("@ID", ID);
                    AuthUserUpdateCmd.Parameters.AddWithValue("@Active", 1);

                    AuthUserUpdateCmd.ExecuteNonQuery();

                    MessageBox.Show("Ваш аккаунт активирован!");
                    Connection.Close();
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new AuthControl());
                }
                else
                {
                    MessageBox.Show("Ошибка активации!");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка чтения кода верификации!");
            }
            finally
            {
                Connection.Close();
            }
        }
    }
}
