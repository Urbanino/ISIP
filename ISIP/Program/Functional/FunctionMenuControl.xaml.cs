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

namespace ISIP.Program.Functional
{
    /// <summary>
    /// Логика взаимодействия для FunctionMenuControl.xaml
    /// </summary>
    public partial class FunctionMenuControl : UserControl
    {
        SqlConnection Connection;

        public FunctionMenuControl(SqlConnection Connection, string Post)
        {
            InitializeComponent();
            this.Connection = Connection;

            try
            {
                Connection.Open();
                SqlCommand GetFunctionList = new SqlCommand("SELECT * FROM FunctionAccessList WHERE Post_Name=@Post_Name", Connection);
                GetFunctionList.Parameters.AddWithValue("@Post_Name", Post);
                DataTable dt = new DataTable();
                dt.Load(GetFunctionList.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {

                    FunctionList.Items.Add(new CollectionItem(row.ItemArray[0].ToString()));
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

        private void FunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FunctionList.SelectedItem == null) return;
            CollectionItem SelectedItem = new CollectionItem();
            SelectedItem = FunctionList.SelectedItem as CollectionItem;

            switch (SelectedItem.Content.ToString())
            {
                case "Права доступа":
                   
                    break;
            }

            FunctionList.SelectedItem = null;
        }
    }
}
