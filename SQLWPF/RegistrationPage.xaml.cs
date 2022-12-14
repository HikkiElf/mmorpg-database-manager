using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Xml;
using System.Text.RegularExpressions;

namespace SQLWPF
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void Registration()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(SqlException) { return; }

                SqlCommand takeNames = new SqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT accountName from Accounts"
                };
                SqlDataReader reader = takeNames.ExecuteReader();

                List<string> allNames = new List<string>();

                while (reader.Read())
                {
                    allNames.Add((string)reader[0]);
                }
                reader.Close();

                if (allNames.Contains(AccountNameField.Text))
                {
                    MessageBox.Show("Name already taken");

                    return;
                }

                else if (AccountNameField.Text == "")
                {
                    MessageBox.Show("Please, write name that you want to use");

                    return;
                }

                else if (!Regex.IsMatch(AccountNameField.Text, @"^[a-zA-Z]*$"))
                {
                    MessageBox.Show(@"Please, use only english characters without spaces");

                    return;
                }

                else if ((AccountNameField.Text.Length < 2) || (AccountNameField.Text.Length > 12))
                {

                    if (AccountNameField.Text.Length < 2)
                    {
                        MessageBox.Show("Name is too short");
                    }
                    else { MessageBox.Show("Name is too long"); }

                    return;
                       
                }

                else if (PasswordField.Password.Length == 0 || PasswordField.Password.Length > 20)
                {
                    if (PasswordField.Password.Length == 0)
                    {
                        MessageBox.Show("Please, insert password");
                    }
                    else { MessageBox.Show("Your password is to long"); }

                    return;
                }

                else if (PasswordField.Password != RepeatPasswordField.Password)
                {
                    MessageBox.Show("Your password repeat is incorrect");

                    return;
                }


                SqlCommand insertAccount = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"INSERT INTO Accounts (accountName, accountPasswd) values ('{AccountNameField.Text}', '{PasswordField.Password}')"
                };
                insertAccount.ExecuteNonQuery();



                var message = string.Join(Environment.NewLine ,allNames);
                MessageBox.Show(message);


            }

        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            Window pr = Window.GetWindow(this);
            pr.Content = new LogInPage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Registration();
        }
    }
}
