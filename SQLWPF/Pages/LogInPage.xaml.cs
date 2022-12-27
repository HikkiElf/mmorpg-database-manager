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
                    CommandText = $"SELECT accountName, accountPasswd, status_id, isBanned FROM Accounts WHERE accountName = '{AccountNameField.Text}' and accountPasswd = '{PasswordField.Password}'"
                };

                SqlDataReader readerData = takeData.ExecuteReader();
                while (readerData.Read())
                {
                    allData.Add((string)readerData[0]);
                    allData.Add((string)readerData[1]);
                    allData.Add(readerData[2].ToString());
                    if (readerData[3] != DBNull.Value)
                    {
                        allData.Add((string)readerData[3]);
                    }
                }
                readerData.Close();
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ValidationMethod();
            if (allData.Contains("true"))
            {
                MessageBox.Show("VAC BAN");
                allData.Clear();
                return;
            }

            else if (allData.Contains(AccountNameField.Text) && allData.Contains(PasswordField.Password))
            {
                Window pr = Window.GetWindow(this);
                if (allData.Contains("2"))
                {
                    ModeratorWindow modwin = new ModeratorWindow();
                    modwin.Show();
                    pr.Owner = modwin;
                }
                else if (allData.Contains("1"))
                {
                    UserWindow userwin = new UserWindow();
                    userwin.Show();
                    pr.Owner = userwin;
                }

                else
                {
                    MainWindow mainwin = new MainWindow();
                    mainwin.Show();
                    pr.Owner = mainwin;
                }
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

        private void AccountNameField_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
