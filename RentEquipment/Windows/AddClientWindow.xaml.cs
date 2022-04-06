using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        private bool IsEdit = false;
        EF.Client editClient = new EF.Client();
        EF.Passport editPassport = new EF.Passport();
        string photostrl;
        public AddClientWindow()
        {
            InitializeComponent();
            cmbGender.ItemsSource = ClassHelper.AppData.Context.Gender.ToList();
            cmbGender.DisplayMemberPath = "NameGender";
            cmbGender.SelectedItem = "0";
            IsEdit = false;
        }
        public AddClientWindow(EF.Client client, EF.Passport passport)
        {
            InitializeComponent();
            IsEdit = true;
            cmbGender.ItemsSource = ClassHelper.AppData.Context.Gender.ToList();
            cmbGender.DisplayMemberPath = "NameGender";
            tbTitle.Text = "Изменение клиента";
            btnAddClient.Content = "Изменить";
            txtLastName.Text = client.LastName;
            txtFirstName.Text = client.FirstName;
            txtMiddleName.Text = client.MiddleName;
            txtPhone.Text = client.Phone;
            txtEmail.Text = client.Email;
            cmbGender.SelectedIndex = client.IDGender - 1;
            dpDateBirth.SelectedDate = client.DateOfBirth;
            editClient = client;
            if (client.Photo != null)
            {
                using (MemoryStream stream = new MemoryStream(client.Photo))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    PhotoUser.Source = bitmapImage;
                }
            }
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            bool IsValidEmail(string email)
            {
                string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
                Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
                return isMatch.Success;
            }
            //валидация
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Поле Фамилия не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Поле Имя не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Поле Телефон не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (dpDateBirth.SelectedDate == null)
            {
                MessageBox.Show("Поле DateOfBirthday не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!isAgeAllowed(18,dpDateBirth.SelectedDate.Value))
            {
                MessageBox.Show("Клиент не может быть младше 18 лет!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Поле Пароль не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtPassword.Text != txtRepeatPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPasport.Text))
            {
                MessageBox.Show("Поле Паспорт не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsValidEmail(txtEmail.Text) == false)
            {
                MessageBox.Show("Введен некорректный Email", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtPhone.Text.Length > 12)
            {
                MessageBox.Show("Поле Телефон содержит больше 12 символов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Int32.TryParse(txtPhone.Text, out int res))
            {
                MessageBox.Show("Поле Телефон должно состоять только из цифр", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsEdit == true)
            {
                var resClick = MessageBox.Show("Вы уверены?", "Подтвержение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resClick == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    string series = txtPasport.Text.Substring(0, 4);
                    string number = txtPasport.Text.Substring(4, 6);
                    if (txtPasport.Text.Length != 10)
                    {
                        MessageBox.Show("Поле Паспорт содержит больше 10 символов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    editClient.LastName = txtLastName.Text;
                    editClient.FirstName = txtFirstName.Text;
                    editClient.MiddleName = txtMiddleName.Text;
                    editClient.Phone = txtPhone.Text;
                    editClient.Email = txtEmail.Text;
                    editClient.IDGender = (cmbGender.SelectedItem as EF.Gender).ID;
                    editPassport.PassportSeries = series;
                    editPassport.PassportNumber = number;
                    if (photostrl != null)
                    {
                        editClient.Photo = File.ReadAllBytes(photostrl);
                    }
                    ClassHelper.AppData.Context.SaveChanges();
                    MessageBox.Show("Пользователь изменен");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                var resClick = MessageBox.Show("Вы уверены?", "Подтвержение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resClick == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    string series = txtPasport.Text.Substring(0, 4);
                    string number = txtPasport.Text.Substring(4, 6);
                    if (txtPasport.Text.Length != 10)
                    {
                        MessageBox.Show("Поле Паспорт содержит больше 10 символов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    EF.Client newClient = new EF.Client();
                    EF.Passport newPasport = new EF.Passport();
                    newClient.LastName = txtLastName.Text;
                    newClient.FirstName = txtFirstName.Text;
                    newClient.MiddleName = txtMiddleName.Text;
                    newClient.Phone = txtPhone.Text;
                    newClient.Email = txtEmail.Text;
                    newClient.IDGender = (cmbGender.SelectedItem as EF.Gender).ID;
                    newPasport.PassportSeries = series;
                    newPasport.PassportNumber = number;
                    if (photostrl != null)
                    {
                        newClient.Photo = File.ReadAllBytes(photostrl);
                    }
                    ClassHelper.AppData.Context.Client.Add(newClient);
                    ClassHelper.AppData.Context.Passport.Add(newPasport);
                    ClassHelper.AppData.Context.SaveChanges();

                    MessageBox.Show("Пользователь добавлен");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                PhotoUser.Source = new BitmapImage(new Uri(openFile.FileName));
                photostrl = openFile.FileName;
            }
        }
        private bool isAgeAllowed(int minAge, DateTime birthDate)
        {
            double age = Math.Round(System.DateTime.Now.Subtract(birthDate).TotalDays / 365.25, 2);
            return (age > minAge);
        }
    }
}
