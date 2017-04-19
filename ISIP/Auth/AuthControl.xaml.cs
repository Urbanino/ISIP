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
using WorkWithDatabase;
using WorkWithRegistry;
using WorkWithCrypt;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Net.Mail;
using ISIP.Program;
using System.Net;

namespace ISIP.Auth
{
    /// <summary>
    /// Логика взаимодействия для AuthControl.xaml
    /// </summary>
    public partial class AuthControl : UserControl
    {
        private string DataSource;
        private string InitialCatalog;
        private string UserID;
        private string Password;
        private SqlConnection Connection;
        private int wrongAuth = 0;


        Grid MainGrid = Application.Current.MainWindow.Content as Grid;
        public AuthControl()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new ConnectionControl());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey AuthInfo = RegistryInfo.Key();

                if (AuthInfo.ValueCount.Equals(0))
                {
                    AuthInfo.SetValue("DataSource", Crypter.Encode("localhost"));
                    AuthInfo.SetValue("InitialCatalog", Crypter.Encode("master"));
                    AuthInfo.SetValue("UserID", Crypter.Encode("sa"));
                    AuthInfo.SetValue("Password", Crypter.Encode(""));
                    AuthInfo.SetValue("Login", Crypter.Encode(""));
                }

                DataSource = Crypter.Decode(AuthInfo.GetValue("DataSource").ToString());
                InitialCatalog = Crypter.Decode(AuthInfo.GetValue("InitialCatalog").ToString());
                UserID = Crypter.Decode(AuthInfo.GetValue("UserID").ToString());
                Password = Crypter.Decode(AuthInfo.GetValue("Password").ToString());
                Login.Text = Crypter.Decode(AuthInfo.GetValue("Login").ToString());
                Connection = Connector.Connection(DataSource, UserID, Password, InitialCatalog);
                
                Connection.Open();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Не удалось подключиться к источнику данных!", "Ошибка подключения");
            }
            finally
            {
                if (Connection.State.ToString() == "Closed")
                {
                    StatusLabel.Content = "Не подключено";
                    StatusLabel.Foreground = new SolidColorBrush(Colors.Red);
                    RegisterButton.IsEnabled = false;
                    LoginButton.IsEnabled = false;
                }
                else
                {
                    StatusLabel.Content = "Подключено";
                    StatusLabel.Foreground = new SolidColorBrush(Colors.Green);
                    RegisterButton.IsEnabled = true;
                    LoginButton.IsEnabled = true;
                }
                Connection.Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < Pass.Password.Length; i++)
                for (char c = 'A'; c <= 'Z'; c++)
                    {
                    if (Pass.Password[i] == c) break;
                    }



                try
                {
                Connection.Open();

                SqlCommand CheckLoginCmd = new SqlCommand(@"SELECT Count(*) FROM Auth_User
                                        WHERE Login=@Login COLLATE Cyrillic_General_CS_AS", Connection);
                CheckLoginCmd.Parameters.AddWithValue("@Login", Login.Text);
                int LoginCount = (int)CheckLoginCmd.ExecuteScalar();
                if (LoginCount > 0)
                {
                    SqlCommand CheckPasswordCmd = new SqlCommand(@"SELECT Count(*) FROM Auth_User
                                        WHERE Login=@Login and Password=@Password COLLATE Cyrillic_General_CS_AS", Connection);
                    CheckPasswordCmd.Parameters.AddWithValue("@Login", Login.Text);
                    CheckPasswordCmd.Parameters.AddWithValue("@Password", Crypter.Encode(Pass.Password));
                    int PasswordCount = (int)CheckPasswordCmd.ExecuteScalar();
                    if (PasswordCount > 0)
                    {
                        int ID;

                        SqlCommand GetAuthUserIDCmd = new SqlCommand(@"SELECT ID FROM Auth_User
                                        WHERE Login=@Login COLLATE Cyrillic_General_CS_AS", Connection);
                        GetAuthUserIDCmd.Parameters.AddWithValue("@Login", Login.Text);

                        ID = (int) GetAuthUserIDCmd.ExecuteScalar();

                        SqlCommand CheckActiveCmd = new SqlCommand(@"SELECT Count(*) FROM Auth_User
                                        WHERE ID=@ID and Active=1", Connection);
                        CheckActiveCmd.Parameters.AddWithValue("@ID", ID);
                        CheckActiveCmd.Parameters.AddWithValue("@Login", Login.Text);
                        int ActiveCount = (int)CheckActiveCmd.ExecuteScalar();

                        if (ActiveCount > 0)
                        {
                            SqlCommand CheckComputerCmd = new SqlCommand(@"SELECT Computer_Name FROM Auth_User
                                        WHERE ID=@ID", Connection);
                            CheckComputerCmd.Parameters.AddWithValue("@ID", ID);
                            string Computer_Name = CheckComputerCmd.ExecuteScalar().ToString();

                            if (String.IsNullOrEmpty(Computer_Name))
                            {
                                SqlCommand SetComputerCmd = new SqlCommand("UPDATE Auth_User SET Computer_Name = @Computer_Name " +
                                    "WHERE (ID=@ID)", Connection);
                                SetComputerCmd.Parameters.AddWithValue("@ID", ID);
                                SetComputerCmd.Parameters.AddWithValue("@Computer_Name", Environment.MachineName);
                                SetComputerCmd.ExecuteNonQuery();
                            }

                            Computer_Name = CheckComputerCmd.ExecuteScalar().ToString();

                            if (Computer_Name == Environment.MachineName)
                            {
                                //MessageBox.Show("Вход выполнен!", "Авторизация");
                                RegistryKey AuthInfo = RegistryInfo.Key();
                                AuthInfo.SetValue("Login", Crypter.Encode(Login.Text));

                                Connection.Close();
                                MainGrid.Children.Clear();
                                MainGrid.Children.Add(new MenuControl(Connection, ID));
                            } else
                            {
                                Connection.Close();
                                MainGrid.Children.Clear();
                                MainGrid.Children.Add(new AuthVerificationControl(Connection, ID, new MailAddress(Login.Text)));
                            }
                        }
                        else
                        {
                            Connection.Close();
                            MainGrid.Children.Clear();
                            MainGrid.Children.Add(new RegisterVerificationControl(Connection, ID, new MailAddress(Login.Text)));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль!", "Ошибка аутентификации");
                        wrongAuth++;
                        if (wrongAuth >= 3)
                        {
                            MessageBoxResult result = MessageBox.Show("Отправить пароль на вашу почту?", "Восстановление пароля", MessageBoxButton.YesNo);
                            switch (result)
                            {
                                case MessageBoxResult.Yes:
                                    SqlCommand GetPasswordCmd = new SqlCommand("SELECT Password FROM Auth_User WHERE Login=@Login", Connection);
                                    GetPasswordCmd.Parameters.AddWithValue("@Login", Login.Text);
                                    string Password = Crypter.Decode(GetPasswordCmd.ExecuteScalar().ToString());

                                    SmtpClient Smtp = new SmtpClient("smtp.gmail.com", 587);
                                    Smtp.Credentials = new NetworkCredential("isip.app@gmail.com", "isip114201514");
                                    Smtp.EnableSsl = true;
                                    MailMessage Message = new MailMessage();
                                    Message.From = new MailAddress("support.isip@yandex.ru", "Сервис ISIP");
                                    Message.To.Add(new MailAddress(Login.Text));
                                    Message.Subject = "Восстановление пароля";
                                    Message.Body = "Ваш пароль для входа в информационную систему: " + Password;

                                    try
                                    {
                                        Smtp.Send(Message);
                                    }
                                    catch (SmtpException ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                    wrongAuth = 0;


                                    break;
                            }
                        }
                            
                    }
                        
                }
                else
                    MessageBox.Show("Неверный логин!", "Ошибка аутентификации");
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new RegisterControl(Connection));
        }
    }
}
