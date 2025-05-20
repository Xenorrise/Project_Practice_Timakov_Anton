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
using System.Windows.Shapes;

namespace WPFHRDatabase
{
    /// <summary>
    /// Interaction logic for EmployeeDialog.xaml
    /// </summary>
    public partial class EmployeeDialog : Window
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public decimal Salary { get; private set; }

        public EmployeeDialog()
        {
            InitializeComponent();
            FirstNameTextBox.Focus();
        }

        public void SettingTextBox(bool isEditing)
        {
            if (MainWindow.empList?.SelectedItem is Employee d && isEditing)
            {
                FirstNameTextBox.Text = d.FirstName;
                LastNameTextBox.Text = d.LastName;
                SalaryTextBox.Text = d.Salary.ToString();
            }
            else
            {
                FirstNameTextBox.Text = "";
                LastNameTextBox.Text = "";
                SalaryTextBox.Text = "";
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(SalaryTextBox.Text.Trim(), out _))
                Salary = Convert.ToDecimal(SalaryTextBox.Text.Trim());
            else
            {
                System.Windows.MessageBox.Show("Неверный формат зарплаты!");
                return;
            }
            FirstName = FirstNameTextBox.Text.Trim();
            LastName = LastNameTextBox.Text.Trim();
            
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
