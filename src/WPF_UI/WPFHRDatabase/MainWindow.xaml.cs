using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using HRDatabase;

namespace WPFHRDatabase
{
    public partial class MainWindow : Window
    {
        private DBControl dB;
        public static System.Windows.Controls.ListBox? depList, empList;
        public bool isInMemory;

        public MainWindow()
        {
            InitializeComponent();
            dB = new DBControl();
            depList = DepartmentsListBox;
            empList = EmployeesListBox;
        }

        private void InMemoryStorageActive(object sender, RoutedEventArgs e) => isInMemory = true;

        private void JsonStorageActive(object sender, RoutedEventArgs e) => isInMemory = false;

        private void AddDepartment(object sender, RoutedEventArgs e)
        {
            var dialog = new DepartmentDialog();
            dialog.SettingTextBox(false);
            if (dialog.ShowDialog() == true)
            {
                dB.AddDepartment(dialog.DepartmentName, isInMemory);
            }
        }

        private void EditDepartment(object sender, RoutedEventArgs e)
        {
            if (DepartmentsListBox.SelectedItem is Department d)
            {
                var dialog = new DepartmentDialog();
                dialog.SettingTextBox(true);
                dialog.Name = d.Name;
                if (dialog.ShowDialog() == true)
                {
                    d.Name = dialog.DepartmentName;
                }
            }
        }

        private void DeleteDepartment(object sender, RoutedEventArgs e)
        {
            if (DepartmentsListBox.SelectedItem is Department d)
            {
                dB.DeleteDepartment(d.Id, isInMemory);
            }
        }

        private void RefreshLists_Departments(object sender, RoutedEventArgs e)
        {
            DepartmentsListBox.ItemsSource = null;
            DepartmentsListBox.ItemsSource = dB.ReadDepatment(isInMemory).ToList();
        }

        private void AddEmployee(object sender, RoutedEventArgs e)
        {
            if (DepartmentsListBox.SelectedItem is Department d)
            {
                var dialog = new EmployeeDialog();
                dialog.SettingTextBox(false);
                if (dialog.ShowDialog() == true)
                {
                    dB.AddEmployee(dialog.FirstName, dialog.LastName, dialog.Salary, d.Id, isInMemory);
                }
            }
            else
                System.Windows.MessageBox.Show("Выберите отдел перед добавлением сотрудника.");
        }

        private void EditEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeesListBox.SelectedItem is Employee emp)
            {
                var dialog = new EmployeeDialog();
                dialog.SettingTextBox(true);
                if (dialog.ShowDialog() == true)
                {
                    dB.UpdateEmployee(emp.Id, dialog.FirstName, dialog.LastName, dialog.Salary, DepartmentsListBox.SelectedItem is Department d ? d.Id : emp.DepartmentId, isInMemory);
                }
            }
            else
                System.Windows.MessageBox.Show("Выберите сотрудника для редактирования.");
        }

        private void DeleteEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeesListBox.SelectedItem is Employee emp)
            {
                dB.DeleteEmployee(emp.Id, isInMemory);
            }
        }

        private void RefreshLists_Employees(object sender, RoutedEventArgs e)
        {
            EmployeesListBox.ItemsSource = null;
            EmployeesListBox.ItemsSource = dB.ReadEmployee(isInMemory).ToList();
        }

        private void SelectFolderButton(object sender, RoutedEventArgs e)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку";
                dialog.UseDescriptionForTitle = true; // Только в новых версиях .NET
                dialog.ShowNewFolderButton = true;

                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    dB.ChangeFolder(dialog.SelectedPath);
                }
            }
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
