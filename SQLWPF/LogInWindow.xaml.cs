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
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private List<string> allNames = new List<string>();
        private List<string> allPasswords = new List<string>();

        public LogInWindow()
        {
            InitializeComponent();
            ValidationMethod();
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

                SqlCommand takeAllNames = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT accountName FROM Accounts"
                };

                SqlDataReader readerNames = takeAllNames.ExecuteReader();
                while(readerNames.Read())
                {
                    allNames.Add((string)readerNames[0]);
                }
                var message = string.Join(Environment.NewLine, allNames);
                MessageBox.Show(message);
                readerNames.Close();

                SqlCommand takeAllPasswords = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT accountPasswd FROM Accounts"
                };

                SqlDataReader readerPasswords = takeAllPasswords.ExecuteReader();
                while (readerPasswords.Read())
                {
                    allPasswords.Add((string)readerPasswords[0]);
                }
                message = string.Join(Environment.NewLine, allPasswords);
                MessageBox.Show(message);

                


            }
            
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            if (allNames.Contains(AccountNameField.Text))
            {
                MessageBox.Show("Success");
            }
            else { MessageBox.Show("Wrong Account Name"); }

        }
    }
}
