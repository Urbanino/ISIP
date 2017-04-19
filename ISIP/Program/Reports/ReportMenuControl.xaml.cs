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

namespace ISIP.Program.Reports
{
    /// <summary>
    /// Логика взаимодействия для ReportMenuControl.xaml
    /// </summary>
    public partial class ReportMenuControl : UserControl
    {
        SqlConnection Connection;

        public ReportMenuControl(SqlConnection Connection, string Post)
        {
            InitializeComponent();
            this.Connection = Connection;

            try
            {
                Connection.Open();
                SqlCommand GetReportList = new SqlCommand("SELECT * FROM ReportAccessList WHERE Post_Name=@Post_Name", Connection);
                GetReportList.Parameters.AddWithValue("@Post_Name", Post);
                DataTable dt = new DataTable();
                dt.Load(GetReportList.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {

                    ReportList.Items.Add(new CollectionItem(row.ItemArray[1].ToString()));
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

        private void ReportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportList.SelectedItem == null) return;
            CollectionItem SelectedItem = new CollectionItem();
            SelectedItem = ReportList.SelectedItem as CollectionItem;

            switch (SelectedItem.Content.ToString())
            {
                case "Права доступа":
                    
                    break;
            }

            ReportList.SelectedItem = null;
        }
    }
}
