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
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;


namespace SQLWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            UpdateTablesCombo();
        }
       
        //                connection.Close();
        //                backgroundWorker.DoWork += (s, fe) => { System.Threading.Thread.Sleep(3000); };
        //                backgroundWorker.RunWorkerCompleted += (s, fe) => { connectionStatusLabel.Content = "Disconnected..."; };
        //                backgroundWorker.RunWorkerAsync();

        /// <summary>
        /// Updating list of tables names in ComboBox "TablesCombo"
        /// </summary>
        private void UpdateTablesCombo()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException)
                {
                    return;
                }
                string get_all_tables_sql = @"SELECT Distinct TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS";
                SqlCommand command = new SqlCommand(get_all_tables_sql, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TablesCombo.Items.Add((string)reader[0]);
                }
                TablesCombo.SelectedValue = TablesCombo.Items[0];
                connection.Close();
            }
        }

        private void BanUser()
        {

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString);
            connection.Open();


          
            var selectedCellInfo = TablesView.SelectedCells[0];
            var selectedCellValue = (selectedCellInfo.Column.GetCellContent(selectedCellInfo.Item) as TextBlock).Text;

            SqlCommand banSelectedUser = new SqlCommand
            {
                Connection = connection,
                CommandText = $"UPDATE Accounts SET isBanned = 'true' where id = {Int32.Parse(selectedCellValue)}"
            };
            if ((string)TablesCombo.SelectedValue == "Accounts")
            {
                banSelectedUser.ExecuteNonQuery();
            }


            //var message = string.Join(Environment.NewLine, content);
                
            //MessageBox.Show(content);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BanUser();
        }


        /// <summary>
        /// Change DataGrid "TablesView" ItemsSource when changing selected table name in ComboBox "TablesCombo"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TablesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString);
            connection.Open();
            TableName.Text = (string)TablesCombo.SelectedValue;
            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = $"SELECT * FROM {(string)TablesCombo.SelectedValue}"
            };
            TablesView.ItemsSource = command.ExecuteReader();
        }
    }
}
