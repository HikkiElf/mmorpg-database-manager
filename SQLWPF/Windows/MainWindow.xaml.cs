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
using System.Text.RegularExpressions;


namespace SQLWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int offsetNumber = 0;
        private int currentPage = 1;

        public MainWindow()
        {
            InitializeComponent();
            UpdateTablesCombo();
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

        private int getNumberOfColumns()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                int count = 0;
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{(string)TablesCombo.SelectedValue}'"
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
            
            AutoCreateTextBoxes();
        }

        private void AutoCreateTextBoxes()
        {
            TextBoxesStack.Children.Clear();

            if((string)TablesCombo.SelectedValue == "Account_To_Character")
            {
                return;
            }

            for (int i = 0; i < getNumberOfColumns() - 1; i++)
            {
                TextBox textBox = new TextBox() { Name = "txtBox" + i.ToString(), Width = 120, };
                Style style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
                textBox.Style = style;
                textBox.Tag = i;
                TextBoxesStack.Children.Add(textBox);
            }
            for(int i = 0; i < getNamesOfColumns().Count; i++)
            {
                MaterialDesignThemes.Wpf.HintAssist.SetHint((DependencyObject)TextBoxesStack.Children[i], getNamesOfColumns()[i]);
            }
        }

        /// <summary>
        /// return List<string> of columns name except first column
        /// </summary>
        /// <returns></returns>
        private List<string> getNamesOfColumns()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {            
                connection.Open();

                SqlCommand selectAllColums = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{(string)TablesCombo.SelectedValue}'"
                };
                List<string> cols = new List<string>();
                SqlDataReader columnReader = selectAllColums.ExecuteReader();
                while (columnReader.Read())
                {
                    cols.Add((string)columnReader[0]);
                }
                columnReader.Close();

                connection.Close();

                cols.RemoveAt(0);

                return cols;
            }
        }


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
                string get_all_tables_sql = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where not TABLE_NAME in ('sysdiagrams') and not TABLE_TYPE = 'VIEW'";
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

        private void DeleteRow()
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
                try
                {
                    var selectedCellInfo = TablesView.SelectedCells[0];
                    var selectedCellValue = (selectedCellInfo.Column.GetCellContent(selectedCellInfo.Item) as TextBlock).Text;

                    SqlCommand deleteSelected = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = $"DELETE FROM [{(string)TablesCombo.SelectedValue}] where id = {Int32.Parse(selectedCellValue)}"
                    };
                    try
                    {
                        deleteSelected.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("files integrity is in danger");
                    }      
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Change isBanned status of selected user by selected id in DataGrid
        /// </summary>
        private void BanUser()
        {
            if ((string)TablesCombo.SelectedValue == "Accounts")
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
                    try
                    {
                        var selectedCellInfo = TablesView.SelectedCells[0];
                        var selectedCellValue = (selectedCellInfo.Column.GetCellContent(selectedCellInfo.Item) as TextBlock).Text;

                        string isBannedStatus = "";
                        string commandBanText = "";

                        SqlCommand getIsBannedStatus = new SqlCommand
                        {
                            Connection = connection,
                            CommandText = $"SELECT isBanned from Accounts where id = {selectedCellValue}"
                        };

                        SqlDataReader readerIsBannedSatus = getIsBannedStatus.ExecuteReader();
                        while (readerIsBannedSatus.Read())
                        {
                            isBannedStatus = readerIsBannedSatus[0].ToString();
                        }
                        readerIsBannedSatus.Close();



                        if (isBannedStatus == "")
                        {
                            commandBanText = $"UPDATE Accounts SET isBanned = 'true' where id = {selectedCellValue}";
                        }
                        else
                        {
                            commandBanText = $"UPDATE Accounts SET isBanned = NULL where id = {selectedCellValue}";
                        }
                        SqlCommand banSelectedUser = new SqlCommand
                        {
                            Connection = connection,
                            CommandText = commandBanText
                        };
                        banSelectedUser.ExecuteNonQuery();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return;
                    }
                    connection.Close();
                }
            }
        }

        private void InsertRow()
        {
            if ((string)TablesCombo.SelectedValue == "Account_To_Character")
            {
                return;
            }
            string commandText = "";
            
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand();

                for (int i = 0; i < getNumberOfColumns() - 1; i++)
                {
                    MessageBox.Show((TextBoxesStack.Children[i] as TextBox).Text);
                    if((TextBoxesStack.Children[i] as TextBox).Text == "")
                    {
                        (TextBoxesStack.Children[i] as TextBox).Text = "NULL";
                    }
                }

                for (int i = 0; i < getNumberOfColumns() - 1; i++)
                {
                    if (i == 0)
                    {
                        commandText = $"INSERT INTO [{(string)TablesCombo.SelectedValue}] ({getNamesOfColumns()[i]}) VALUES ('{(TextBoxesStack.Children[i] as TextBox).Text}')";
                    }
                    else
                    {
                        if (Regex.IsMatch((TextBoxesStack.Children[i] as TextBox).Text, @"^[0-9]*$") || (TextBoxesStack.Children[i] as TextBox).Text == "NULL")
                        {
                            commandText = $"UPDATE [{(string)TablesCombo.SelectedValue}] set {getNamesOfColumns()[i]} = {(TextBoxesStack.Children[i] as TextBox).Text} WHERE id = (SELECT TOP 1 id FROM [{(string)TablesCombo.SelectedValue}] ORDER BY 1 DESC)";              
                        }
                        else
                        {
                            commandText = $"UPDATE [{(string)TablesCombo.SelectedValue}] set {getNamesOfColumns()[i]} = '{(TextBoxesStack.Children[i] as TextBox).Text}' WHERE id = (SELECT TOP 1 id FROM [{(string)TablesCombo.SelectedValue}] ORDER BY 1 DESC)";
                        }
                    }
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = commandText
                    };
                    sqlCommand.ExecuteNonQuery();
                }
                
                connection.Close();
            }
            
        }

        private void AutoInsertIntoTextBoxes()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mmorpgdb"].ConnectionString))
            {
                try
                {
                    connection.Open();

                }
                catch (SqlException) { return; }
                var selectedCellInfo = TablesView.SelectedCells[0];
                var selectedCellValue = (selectedCellInfo.Column.GetCellContent(selectedCellInfo.Item) as TextBlock).Text;

                SqlCommand getDataFromRow = new SqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT * FROM {(string)TablesCombo.SelectedValue} where id = {selectedCellValue}"
                };

                SqlDataReader readData = getDataFromRow.ExecuteReader();
                while (readData.Read())
                {
                    for (int i = 1; i < getNumberOfColumns(); i++)
                    {
                        (TextBoxesStack.Children[i - 1] as TextBox).Text = readData[i].ToString();
                    }
                }
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

        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            BanUser();
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

        private void InsertRegion_Click(object sender, RoutedEventArgs e)
        {
            InsertRow();
            UpdateTableView();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRow();
            UpdateTableView();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AutoInsertIntoTextBoxes();           
        }
    }
}
