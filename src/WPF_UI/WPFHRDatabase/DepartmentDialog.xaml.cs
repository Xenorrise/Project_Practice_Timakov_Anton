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
    /// Interaction logic for DepartmentDialog.xaml
    /// </summary>
    public partial class DepartmentDialog : Window
    {
        public string DepartmentName { get; private set; } = string.Empty;

        public DepartmentDialog()
        {
            InitializeComponent();
            NameTextBox.Focus();
        }

        public void SettingTextBox(bool isEditing)
        {
            if (MainWindow.depList?.SelectedItem is Department d && isEditing)
                NameTextBox.Text = d.Name;
            else
                NameTextBox.Text = "";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DepartmentName = NameTextBox.Text.Trim();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
