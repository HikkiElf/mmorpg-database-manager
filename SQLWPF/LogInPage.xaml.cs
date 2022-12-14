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
using System.Data.SqlClient;
using System.Configuration;

namespace SQLWPF
{
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private List<string> allData = new List<string>();
        public LogInPage()
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
                catch (SqlException)
                {
                    return;
                }

                SqlCommand takeData = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT accountName, accountPasswd FROM Accounts WHERE accountName = '{AccountNameField.Text}' and accountPasswd = '{PasswordField.Password}'"
                };

                SqlDataReader readerData = takeData.ExecuteReader();
                while (readerData.Read())
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
                Window pr = Window.GetWindow(this);
                pr.Owner = mainwin;
                pr.Close();
            }
            else { MessageBox.Show("Wrong user name or password!"); }
            allData.Clear();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Window pr = Window.GetWindow(this);
            pr.Content = new RegistrationPage();
        }
    }
}
