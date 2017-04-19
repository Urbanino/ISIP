using ISIP.Program.Collections.Model;
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
using System.Windows.Shapes;

namespace ISIP.Program.Collections
{
    /// <summary>
    /// Логика взаимодействия для ClientRecordsWindow.xaml
    /// </summary>
    public partial class ClientRecordsWindow : Window
    {
        SqlConnection Connection;

        public ClientRecordsWindow(SqlConnection Connection, int Modified)
        {
            InitializeComponent();
            this.Connection = Connection;

            if (Modified == 0)
            {
                AccountControls.Visibility = Visibility.Hidden;
                PassportControls.Visibility = Visibility.Hidden;
                ContractControls.Visibility = Visibility.Hidden;
                ClientControls.Visibility = Visibility.Hidden;
                LocationControls.Visibility = Visibility.Hidden;
            }

            LoadRecords();
            LoadCity();
            LoadTariff();
        }

        private void LoadCity()
        {
            try
            {
                Connection.Open();
                SqlCommand GetCityListCmd = new SqlCommand("SELECT ID, City_Name as 'Город' FROM City", Connection);
                DataTable dt = new DataTable();
                dt.Load(GetCityListCmd.ExecuteReader());
                CityList.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            LoadStreet();
        }

        private void LoadHouse()
        {
            if (StreetList.SelectedItem == null) return;

            DataRowView row = (DataRowView)StreetList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetHouseListCmd = new SqlCommand("SELECT ID, House_Num as 'Дом' FROM House WHERE Street_ID=@Street_ID", Connection);
                GetHouseListCmd.Parameters.AddWithValue("@Street_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetHouseListCmd.ExecuteReader());
                HouseList.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            LoadLocation();
        }

        private void LoadLocation()
        {
            if (HouseList.SelectedItem == null) return;

            DataRowView row = (DataRowView)HouseList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetLocationListCmd = new SqlCommand("SELECT ID, Front_Num as 'Подъезд', " +
                    "Floor_Num as 'Этаж', Flat_Num as 'Квартира' FROM Location WHERE House_ID=@House_ID", Connection);
                GetLocationListCmd.Parameters.AddWithValue("@House_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetLocationListCmd.ExecuteReader());
                LocationList.ItemsSource = dt.DefaultView;
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

        private void LoadPassport()
        {
            if (ClientList.SelectedItem == null) return;

            DataRowView row = (DataRowView)ClientList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetPassportListCmd = new SqlCommand("SELECT ID, Pass_Series as 'Серия', " +
                    "Pass_Num as 'Номер', Issue_Date as 'Дата выдачи', Issue_Place as 'Место выдачи'  FROM Client_Pass WHERE Client_ID=@Client_ID", Connection);
                GetPassportListCmd.Parameters.AddWithValue("Client_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetPassportListCmd.ExecuteReader());
                PassportList.ItemsSource = dt.DefaultView;
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

        private void LoadStreet()
        {
            if (CityList.SelectedItem == null) return;

            DataRowView row = (DataRowView)CityList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetStreetListCmd = new SqlCommand("SELECT ID, Street_Name as 'Улица' FROM Street WHERE City_ID=@City_ID", Connection);
                GetStreetListCmd.Parameters.AddWithValue("@City_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetStreetListCmd.ExecuteReader());
                StreetList.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            LoadHouse();
        }

        private void LoadRecords()
        {
            try
            {
                Connection.Open();
                SqlCommand GetClientListCmd = new SqlCommand("SELECT ID, Last_Name as 'Фамилия', First_Name as 'Имя', " +
                    "Middle_Name as 'Отчество', Birth_Date as 'Дата рождения', Email  FROM Client", Connection);
                DataTable dt = new DataTable();
                dt.Load(GetClientListCmd.ExecuteReader());
                ClientList.ItemsSource = dt.DefaultView;


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

        private void LoadContract()
        {
            if ((LocationList.SelectedItem == null) || (ClientList.SelectedItem == null)) return;

            DataRowView location = (DataRowView)LocationList.SelectedItem;
            DataRowView client = (DataRowView)ClientList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetContractListCmd = new SqlCommand("SELECT ID, Contract_Num as 'Номер', " +
                    "Create_Date as 'Дата заключения', Close_Date as 'Дата расторжения' FROM Contract WHERE Client_ID=@Client_ID AND Location_ID=@Location_ID", Connection);
                GetContractListCmd.Parameters.AddWithValue("@Client_ID", (int)client["ID"]);
                GetContractListCmd.Parameters.AddWithValue("@Location_ID", (int)location["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetContractListCmd.ExecuteReader());
                ContractList.ItemsSource = dt.DefaultView;
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

        private void CityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStreet();
        }

        private void StreetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHouse();
        }

        private void HouseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadLocation();
        }

        private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPassport();
            LoadContract();
            if (ClientList.SelectedItem == null) return;
            DataRowView row = (DataRowView)ClientList.SelectedItem;
            Last_Name.Text = row["Фамилия"].ToString();
            First_Name.Text = row["Имя"].ToString();
            Middle_Name.Text = row["Отчество"].ToString();
            Birth_Date.Text = row["Дата рождения"].ToString();
            Email.Text = row["Email"].ToString();

            try
            {
                Connection.Open();
                SqlCommand GetAddressCmd = new SqlCommand("SELECT Location_ID, House_ID, Street_ID, City_ID FROM ClientRecordList WHERE Client_ID=@Client_ID", Connection);
                GetAddressCmd.Parameters.AddWithValue("@Client_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetAddressCmd.ExecuteReader());

                if (dt.Rows.Count == 0) return;

                int Location_ID = (int)dt.Rows[0].ItemArray[0];
                int House_ID = (int)dt.Rows[0].ItemArray[1];
                int Street_ID = (int)dt.Rows[0].ItemArray[2];
                int City_ID = (int)dt.Rows[0].ItemArray[3];

                Connection.Close();

                foreach (DataRowView city in CityList.Items)
                {
                    if (((int)city[0]) == City_ID)
                    {
                        CityList.SelectedIndex = CityList.Items.IndexOf(city);
                    }
                }

                foreach (DataRowView street in StreetList.Items)
                {
                    if (((int)street[0]) == Street_ID)
                    {
                        StreetList.SelectedIndex = StreetList.Items.IndexOf(street);
                    }
                }

                foreach (DataRowView house in HouseList.Items)
                {
                    if (((int)house[0]) == House_ID)
                    {
                        HouseList.SelectedIndex = HouseList.Items.IndexOf(house);
                    }
                }

                foreach (DataRowView location in LocationList.Items)
                {
                    if ( ((int) location[0]) == Location_ID)
                    {
                        LocationList.SelectedIndex = LocationList.Items.IndexOf(location);
                    }
                }
                
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

        private void LocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadContract();
            if (LocationList.SelectedItem == null) return;

            DataRowView row = (DataRowView)LocationList.SelectedItem;
            Front_Num.Text = row["Подъезд"].ToString();
            Floor_Num.Text = row["Этаж"].ToString();
            Flat_Num.Text = row["Квартира"].ToString();
        }

        private void LoadAccount()
        {
            if (ContractList.SelectedItem == null) return;

            DataRowView row = (DataRowView)ContractList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetAccountListCmd = new SqlCommand("SELECT ID, Login as 'Логин', Password as 'Пароль', " +
                    "Balance as 'Баланс', Access as 'Доступ' FROM Account WHERE Contract_ID=@Contract_ID", Connection);
                GetAccountListCmd.Parameters.AddWithValue("@Contract_ID", (int)row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetAccountListCmd.ExecuteReader());
                AccountList.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            LoadHouse();
        }

        private void ContractList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAccount();

            if (ContractList.SelectedItem == null) return;

            DataRowView row = (DataRowView)ContractList.SelectedItem;
            Contract_Num.Text = row["Номер"].ToString();
            Create_Date.Text = row["Дата заключения"].ToString();
            Close_Date.Text = row["Дата расторжения"].ToString();
        }

        private void AccountList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountList.SelectedItem == null) return;
            DataRowView row = (DataRowView)AccountList.SelectedItem;
            Login.Text = row["Логин"].ToString();
            Password.Text = row["Пароль"].ToString();
            Balance.Text = row["Баланс"].ToString();
            Access.Text = row["Доступ"].ToString();
            LoadTariffHistory();
        }

        private void PassportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PassportList.SelectedItem == null) return;

            DataRowView row = (DataRowView)PassportList.SelectedItem;
            Pass_Series.Text = row["Серия"].ToString();
            Pass_Num.Text = row["Номер"].ToString();
            Issue_Date.Text = row["Дата выдачи"].ToString();
            Issue_Place.Text = row["Место выдачи"].ToString();
        }

        private void LoadTariff()
        {
            try
            {
                Connection.Open();
                SqlCommand GetTariffListCmd = new SqlCommand("SELECT ID, Tariff_Name as 'Тариф' FROM Tariff", Connection);
                DataTable dt = new DataTable();
                dt.Load(GetTariffListCmd.ExecuteReader());
                TariffList.ItemsSource = dt.DefaultView;
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

        private void LoadTariffHistory()
        {
            if (AccountList.SelectedItem == null) return;

            DataRowView row = (DataRowView)AccountList.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand GetTariffHistoryCmd = new SqlCommand("SELECT Tariff_Name as 'Тариф', Connect_Date as 'Дата подключения', " +
                    "Disconnect_Date as 'Дата отключения' FROM AccountTariffHistoryList WHERE Account_ID=@Account_ID", Connection);
                GetTariffHistoryCmd.Parameters.AddWithValue("Account_ID", (int) row["ID"]);
                DataTable dt = new DataTable();
                dt.Load(GetTariffHistoryCmd.ExecuteReader());
                TariffHistoryList.ItemsSource = dt.DefaultView;
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

        private void LoadTariffDiscount()
        {
            try
            {
                Connection.Open();
                SqlCommand GetTariffDiscountCmd = new SqlCommand("GetTariffDiscount", Connection);
                GetTariffDiscountCmd.CommandType = CommandType.StoredProcedure;
                GetTariffDiscountCmd.Parameters.AddWithValue("@date", DateTime.Today);
                DataTable dt = new DataTable();
                dt.Load(GetTariffDiscountCmd.ExecuteReader());
                TariffDiscountList.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    TariffDiscountList.Items.Add(new IdentItem( (int) row["ID"], row["Dis_Name"].ToString()));
                }
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

        private void TariffDiscountList_DropDownOpened(object sender, EventArgs e)
        {
            LoadTariffDiscount();
        }

        private void InsertClientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.Open();
                SqlCommand InsertClientCmd = new SqlCommand("ClientInsertCommand", Connection);
                InsertClientCmd.CommandType = CommandType.StoredProcedure;
                InsertClientCmd.Parameters.AddWithValue("@Last_Name", Last_Name.Text);
                InsertClientCmd.Parameters.AddWithValue("@First_Name", First_Name.Text);
                InsertClientCmd.Parameters.AddWithValue("@Middle_Name", Middle_Name.Text);
                InsertClientCmd.Parameters.AddWithValue("@Birth_Date", Birth_Date.Text);
                InsertClientCmd.Parameters.AddWithValue("@Email", Email.Text);
                DataTable dt = new DataTable();
                InsertClientCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            LoadRecords();
        }
    }
}
