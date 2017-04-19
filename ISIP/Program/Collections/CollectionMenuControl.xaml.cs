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

namespace ISIP.Program.Collections
{
    /// <summary>
    /// Логика взаимодействия для CollectionMenuControl.xaml
    /// </summary>
    public partial class CollectionMenuControl : UserControl
    {
        SqlConnection Connection;
        string Post;

        public CollectionMenuControl(SqlConnection Connection, string Post)
        {
            InitializeComponent();
            this.Connection = Connection;
            this.Post = Post;

            try
            {
                Connection.Open();
                SqlCommand GetCollectionList = new SqlCommand("SELECT * FROM CollectionAccessList WHERE Post_Name=@Post_Name", Connection);
                GetCollectionList.Parameters.AddWithValue("@Post_Name", Post);
                DataTable dt = new DataTable();
                dt.Load(GetCollectionList.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {

                    CollectionList.Items.Add(new CollectionItem(row.ItemArray[0].ToString(), (int)row.ItemArray[2]));
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

        private void CollectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionList.SelectedItem == null) return;
            CollectionItem SelectedItem = new CollectionItem();
            SelectedItem = CollectionList.SelectedItem as CollectionItem;

            int Modified = 0;
            try
            {
                Connection.Open();
                SqlCommand GetModifiedCmd = new SqlCommand("SELECT Modified FROM CollectionAccessList WHERE Post_Name=@Post_Name AND Collection_Name=@Collection_Name", Connection);
                GetModifiedCmd.Parameters.AddWithValue("@Post_Name", Post);
                GetModifiedCmd.Parameters.AddWithValue("@Collection_Name", SelectedItem.Content.ToString());
                Modified = (int)GetModifiedCmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            switch (SelectedItem.Content.ToString())
            {
                case "Права доступа":
                    new AccessRightsWindow(Connection).Show();
                    break;
                case "Учет клиентов":
                    new ClientRecordsWindow(Connection, Modified).Show();
                    break;
            }

            CollectionList.SelectedItem = null;
        }
            
    }
}
