using ISIP.Program.Collections.Model;
using System;
using System.Collections;
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
    /// Логика взаимодействия для AccessRightsWindow.xaml
    /// </summary>
    public partial class AccessRightsWindow : Window
    {
        SqlConnection Connection;

        public AccessRightsWindow(SqlConnection Connection)
        {
            InitializeComponent();
            this.Connection = Connection;
            GetPostList();
        }

        private void GetCollectionList()
        {
            try
            {
                Connection.Open();
                SqlCommand GetEnabledCollectionsCmd = new SqlCommand("SELECT Collection_ID as 'ID', Collection_Name as 'Справочник', Modified as 'Редактирование' FROM CollectionAccessList WHERE Post_Name=@Post_Name", Connection);
                GetEnabledCollectionsCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);

                DataTable dt = new DataTable();
                dt.Load(GetEnabledCollectionsCmd.ExecuteReader());
                EnabledCollections.ItemsSource = dt.DefaultView;

                SqlCommand GetAllCollectionsCmd = new SqlCommand("SELECT ID, Collection_Name as 'Справочник' FROM Collection WHERE Collection_Name NOT IN (SELECT Collection_Name FROM CollectionAccessList WHERE Post_Name=@Post_Name)", Connection);
                GetAllCollectionsCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);
                DataTable dt2 = new DataTable();
                dt2.Load(GetAllCollectionsCmd.ExecuteReader());
                DisabledCollections.ItemsSource = dt2.DefaultView;

                try
                {
                    EnabledCollections.Columns[0].Visibility = Visibility.Hidden;
                    DisabledCollections.Columns[0].Visibility = Visibility.Hidden;
                }
                catch
                {
                    // 
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

        private void GetFunctionList()
        {
            try
            {
                Connection.Open();
                SqlCommand GetEnabledFunctionsCmd = new SqlCommand("SELECT Function_ID as 'ID', Function_Name as 'Функция' FROM FunctionAccessList WHERE Post_Name=@Post_Name", Connection);
                GetEnabledFunctionsCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);

                DataTable dt = new DataTable();
                dt.Load(GetEnabledFunctionsCmd.ExecuteReader());
                EnabledFunctions.ItemsSource = dt.DefaultView;

                SqlCommand GetDisabledFunctionssCmd = new SqlCommand("SELECT ID, Function_Name as 'Функция' FROM Functional WHERE Function_Name NOT IN (SELECT Function_Name FROM FunctionAccessList WHERE Post_Name=@Post_Name)", Connection);
                GetDisabledFunctionssCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);
                DataTable dt2 = new DataTable();
                dt2.Load(GetDisabledFunctionssCmd.ExecuteReader());
                DisabledFunctions.ItemsSource = dt2.DefaultView;

                try
                {
                    EnabledFunctions.Columns[0].Visibility = Visibility.Hidden;
                    DisabledFunctions.Columns[0].Visibility = Visibility.Hidden;
                } catch
                {
                    //
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

        private void GetReportList()
        {
            try
            {
                Connection.Open();
                SqlCommand GetEnabledReportsCmd = new SqlCommand("SELECT Report_ID as 'ID', Report_Name as 'Отчет' FROM ReportAccessList WHERE Post_Name=@Post_Name", Connection);
                GetEnabledReportsCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);

                DataTable dt = new DataTable();
                dt.Load(GetEnabledReportsCmd.ExecuteReader());
                EnabledReports.ItemsSource = dt.DefaultView;

                SqlCommand GetDisabledReportsCmd = new SqlCommand("SELECT ID, Report_Name as 'Отчет' FROM Report WHERE Report_Name NOT IN (SELECT Report_Name FROM ReportAccessList WHERE Post_Name=@Post_Name)", Connection);
                GetDisabledReportsCmd.Parameters.AddWithValue("@Post_Name", (PostList.SelectedItem as Post).Post_Name);
                DataTable dt2 = new DataTable();
                dt2.Load(GetDisabledReportsCmd.ExecuteReader());
                DisabledReports.ItemsSource = dt2.DefaultView;

                try
                {
                    EnabledReports.Columns[0].Visibility = Visibility.Hidden;
                    DisabledReports.Columns[0].Visibility = Visibility.Hidden;
                }
                catch { }


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

        private void GetPostList()
        {
            try
            {
                Connection.Open();
                SqlCommand GetPostListCmd = new SqlCommand("PostSelectCommand", Connection);

                DataTable dt = new DataTable();
                dt.Load(GetPostListCmd.ExecuteReader());
                
                foreach(DataRow row in dt.Rows)
                {
                    PostList.Items.Add(new Post{ ID = (int)row[0], Post_Name = row[1].ToString() });
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

        private void PostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCollectionList();
            GetFunctionList();
            GetReportList();
        }

        private void EnabledCollections_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EnabledCollections.SelectedItem == null) return;

            DataRowView row = (DataRowView)EnabledCollections.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand DisableCollectionCmd = new SqlCommand("DELETE FROM Collection_Access WHERE ((Post_ID=@Post_ID) AND (Collection_ID=@Collection_ID))", Connection);
                DisableCollectionCmd.Parameters.AddWithValue("@Collection_ID", (int) row["ID"]);
                DisableCollectionCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                DisableCollectionCmd.ExecuteNonQuery();
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

        private void DisabledCollections_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DisabledCollections.SelectedItem == null) return;

            DataRowView row = (DataRowView)DisabledCollections.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand EnableCollectionCmd = new SqlCommand("Collection_AccessInsertCommand", Connection);
                EnableCollectionCmd.CommandType = CommandType.StoredProcedure;
                EnableCollectionCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                EnableCollectionCmd.Parameters.AddWithValue("@Viewed", 1);
                EnableCollectionCmd.Parameters.AddWithValue("@Modified", 0);
                EnableCollectionCmd.Parameters.AddWithValue("@Collection_ID", (int) row["ID"]);
                EnableCollectionCmd.ExecuteNonQuery();
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

        private void EnabledCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnabledCollections.SelectedItem == null) return;

            DataRowView row = (DataRowView)EnabledCollections.SelectedItem;

            if ((int)row[2] == 1) ModifiedYes.IsChecked = true;
            else ModifiedNo.IsChecked = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (EnabledCollections.SelectedItem == null) return;
            DataRowView row = (DataRowView)EnabledCollections.SelectedItem;

            int s = EnabledCollections.SelectedIndex;

            try
            {
                Connection.Open();
                SqlCommand UpdateModifiedCmd = new SqlCommand("UPDATE Collection_Access SET Modified = @Modified WHERE Post_ID=@Post_ID AND Collection_ID=@Collection_ID", Connection);
                UpdateModifiedCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                UpdateModifiedCmd.Parameters.AddWithValue("@Collection_ID", (int)row["ID"]);
                if (ModifiedYes.IsChecked == true)
                    UpdateModifiedCmd.Parameters.AddWithValue("@Modified", 1);
                else
                    UpdateModifiedCmd.Parameters.AddWithValue("@Modified", 0);
                UpdateModifiedCmd.ExecuteNonQuery();
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
            EnabledCollections.SelectedIndex = s;
        }

        private void EnabledFunctions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EnabledFunctions.SelectedItem == null) return;

            DataRowView row = (DataRowView)EnabledFunctions.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand DisableFunctionCmd = new SqlCommand("DELETE FROM Function_Access WHERE ((Post_ID=@Post_ID) AND (Function_ID=@Function_ID))", Connection);
                DisableFunctionCmd.Parameters.AddWithValue("@Function_ID", (int)row["ID"]);
                DisableFunctionCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                DisableFunctionCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            GetFunctionList();

        }

        private void DisabledFunctions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DisabledFunctions.SelectedItem == null) return;

            DataRowView row = (DataRowView)DisabledFunctions.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand EnableFunctionCmd = new SqlCommand("Function_AccessInsertCommand", Connection);
                EnableFunctionCmd.CommandType = CommandType.StoredProcedure;
                EnableFunctionCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                EnableFunctionCmd.Parameters.AddWithValue("@Executed", 1);
                EnableFunctionCmd.Parameters.AddWithValue("@Function_ID", (int)row["ID"]);
                EnableFunctionCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            GetFunctionList();
        }

        private void EnabledReports_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EnabledReports.SelectedItem == null) return;

            DataRowView row = (DataRowView)EnabledReports.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand DisableReportCmd = new SqlCommand("DELETE FROM Report_Access WHERE ((Post_ID=@Post_ID) AND (Report_ID=@Report_ID))", Connection);
                DisableReportCmd.Parameters.AddWithValue("@Report_ID", (int)row["ID"]);
                DisableReportCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                DisableReportCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            GetReportList();
        }

        private void DisabledReports_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DisabledReports.SelectedItem == null) return;

            DataRowView row = (DataRowView)DisabledReports.SelectedItem;

            try
            {
                Connection.Open();
                SqlCommand EnableReportCmd = new SqlCommand("Report_AccessInsertCommand", Connection);
                EnableReportCmd.CommandType = CommandType.StoredProcedure;
                EnableReportCmd.Parameters.AddWithValue("@Post_ID", (PostList.SelectedItem as Post).ID);
                EnableReportCmd.Parameters.AddWithValue("@Executed", 1);
                EnableReportCmd.Parameters.AddWithValue("@Report_ID", (int)row["ID"]);
                EnableReportCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            GetReportList();
        }
    }
}
