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
            UpdateTableList();
            //UpdateViewer();
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
        //    {
        //        connectionStatusLabel.Content = "";
        //        try
        //        {
        //            connection.Open();
        //            connectionStatusLabel.Content = ("Connected! " + connection.Database);

        //        }
        //        catch (SqlException)
        //        {

        //        }
        //        finally
        //        {
        //            if (connection.State == ConnectionState.Open)
        //            {
        //                connection.Close();
        //                backgroundWorker.DoWork += (s, fe) => { System.Threading.Thread.Sleep(3000); };
        //                backgroundWorker.RunWorkerCompleted += (s, fe) => { connectionStatusLabel.Content = "Disconnected..."; };
        //                backgroundWorker.RunWorkerAsync();

        //            }
        //        }
        //    }
        //}

        private void UpdateTableList()
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
            ListHistory.ItemsSource = command.ExecuteReader();

        }
    }


        //private void UpdateViewer()
        //{
        //    //string table_name = (string)TablesCombo.SelectedValue;
        //    string table_name = "Accounts";


        //    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
        //    {
        //        try
        //        {
        //            connection.Open();
        //        }
        //        catch(SqlException)
        //        {
        //            return;
        //        }

        //        SqlCommand command = new SqlCommand
        //        {
        //            CommandText = $"SELECT * FROM {table_name}",
        //            Connection = connection

        //        };

        //        ListHistory.ItemsSource = command.ExecuteReader();

        //        connection.Close();

        //    }
        //}


        //private void ChanchedSelected(object sender, SelectionChangedEventArgs e)
        //{
        //    UpdateViewer();
        //}
}
