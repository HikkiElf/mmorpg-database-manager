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
using MaterialDesignThemes.Wpf;

namespace SQLWPF
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private List<string> allData = new List<string>();

        public LogInWindow()
        {
            InitializeComponent();
        }

        private void ValidationMethod()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(SqlException)
                {
                    return;
                }

                SqlCommand takeData = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT accountName, accountPasswd FROM Accounts WHERE accountName = '{AccountNameField.Text}' and accountPasswd = '{PasswordField.Password}'"
                };

                SqlDataReader readerData = takeData.ExecuteReader();
                while(readerData.Read())
                {
                    allData.Add((string)readerData[0]);
                    allData.Add((string)readerData[1]);
                }
                readerData.Close();
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ValidationMethod();

            if (allData.Contains(AccountNameField.Text) && allData.Contains(PasswordField.Password))
            {
                
                MainWindow mainwin = new MainWindow();
                mainwin.Show();
                this.Owner = mainwin;
                this.Close();
            }
            else { MessageBox.Show("Wrong user name or password!"); }
            allData.Clear();
        }
    }
}
