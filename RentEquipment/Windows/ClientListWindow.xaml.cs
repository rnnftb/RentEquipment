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

namespace RentEquipment.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClientListWindow.xaml
    /// </summary>
    public partial class ClientListWindow : Window
    {
        List<string> ListSort = new List<string>()
    {
        "По умолчанию","По фамилии","По имени","По телефону","По почте","По должности"
    };
        public ClientListWindow()
        {
            InitializeComponent();
            Filter();
            lvClientList.ItemsSource = ClassHelper.AppData.Context.Staff.ToList();
            cmbSort.ItemsSource = ListSort;
            cmbSort.SelectedIndex = 0;
        }
        private void Filter()
        {
            List<EF.Client> ListClient = new List<EF.Client>();
            ListClient = ClassHelper.AppData.Context.Client.Where(i => i.IsDeleted == false).ToList();
            ListClient = ListClient.Where(i =>
            i.LastName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FirstName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.MiddleName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FIO.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Phone.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Email.ToLower().Contains(txtSearch.Text.ToLower())).ToList();

            switch (cmbSort.SelectedIndex)
            {
                case 0:
                    ListClient = ListClient.OrderBy(i => i.ID).ToList();
                    break;
                case 1:
                    ListClient = ListClient.OrderBy(i => i.LastName).ToList();
                    break;
                case 2:
                    ListClient = ListClient.OrderBy(i => i.FirstName).ToList();
                    break;
                case 3:
                    ListClient = ListClient.OrderBy(i => i.Phone).ToList();
                    break;
                case 4:
                    ListClient = ListClient.OrderBy(i => i.Email).ToList();
                    break;
                default:
                    ListClient = ListClient.OrderBy(i => i.ID).ToList();
                    break;
            }
            lvClientList.ItemsSource = ListClient;
        }
        private void lvClientList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                try
                {
                    if (lvClientList.SelectedItem is EF.Client)
                    {
                        var resmsg = MessageBox.Show("Удалить пользователя?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (resmsg == MessageBoxResult.No)
                        {
                            return;
                        }
                        var clt = lvClientList.SelectedItem as EF.Client;
                        clt.IsDeleted = true;
                        //ClassHelper.AppData.Context.Staff.Remove(stf);
                        ClassHelper.AppData.Context.SaveChanges();
                        MessageBox.Show("Пользователь успешно удален", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        Filter();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void lvClientList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvClientList.SelectedItem is EF.Client)
            {
                var clt = lvClientList.SelectedItem as EF.Client;
                var cltpass = lvClientList.SelectedItem as EF.Passport;
                AddClientWindow addClientWindow = new AddClientWindow(clt, cltpass);
                addClientWindow.ShowDialog();
                Filter();

            }
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow addClientWindow = new AddClientWindow();
            addClientWindow.ShowDialog();
            lvClientList.ItemsSource = ClassHelper.AppData.Context.Client.ToList();
            Filter();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
    }
}