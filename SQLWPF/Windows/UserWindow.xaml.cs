using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace SQLWPF
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private int offsetNumber = 0;
        private int currentPage = 1;


        public UserWindow()
        {
            InitializeComponent();
            UpdateTablesCombo();
            UpdateTableView();
        }

        private int getNumberOfRows()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                int count = 0;
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COUNT(*) FROM [{(string)TablesCombo.SelectedValue}]"
                };
                connection.Open();
                count = (int)command.ExecuteScalar();
                connection.Close();
                return count;
            }
        }

        private void UpdateTableView()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = $"SELECT * FROM [{(string)TablesCombo.SelectedValue}] order by 1 OFFSET {offsetNumber} rows fetch next 5 rows only"
            };
            TablesView.ItemsSource = command.ExecuteReader();

            TableName.Text = (string)TablesCombo.SelectedValue;
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
                string get_all_tables_sql = @"select TABLE_NAME from INFORMATION_SCHEMA.VIEWS";
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

        /// <summary>
        /// Change DataGrid "TablesView" ItemsSource when changing selected table name in ComboBox "TablesCombo"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TablesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            offsetNumber = 0;
            currentPage = 1;
            CurrentPageLabel.Content = currentPage.ToString();
            UpdateTableView();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (offsetNumber < getNumberOfRows() - 5)
            {
                offsetNumber += 5;
                currentPage += 1;
                CurrentPageLabel.Content = (currentPage).ToString();
            }
            UpdateTableView();
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (offsetNumber > 0)
            {
                offsetNumber -= 5;
                currentPage -= 1;
                CurrentPageLabel.Content = (currentPage).ToString();
            }
            UpdateTableView();
        }
    }
}
