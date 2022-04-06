using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEquipWindow.xaml
    /// </summary>
    public partial class AddEquipWindow : Window
    {
            private bool IsEdit = false;
            EF.Product editEquip = new EF.Product();
            string photostrl;
            public AddEquipWindow()
            {
                InitializeComponent();
                cmbType.ItemsSource = ClassHelper.AppData.Context.Type.ToList();
                cmbType.DisplayMemberPath = "NameType";
                cmbType.SelectedItem = "0";
                cmbStatus.ItemsSource = ClassHelper.AppData.Context.Status.ToList();
                cmbStatus.DisplayMemberPath = "StatusName";
                cmbType.SelectedItem = "0";
                IsEdit = false;
            }
            public AddEquipWindow(EF.Product equip)
            {
                InitializeComponent();
                IsEdit = true;
                cmbType.ItemsSource = ClassHelper.AppData.Context.Type.ToList();
                cmbType.DisplayMemberPath = "NameType";
                cmbStatus.ItemsSource = ClassHelper.AppData.Context.Status.ToList();
                cmbStatus.DisplayMemberPath = "StatusName";
                tbTitle.Text = "Изменение сотрудника";
                btnAdd.Content = "Изменить";
                txtName.Text = equip.NameProduct;
                txtPrice.Text = Convert.ToString(equip.Price);
                txtWarranty.Text = Convert.ToString(equip.Warranty);
                cmbType.SelectedIndex = equip.IDType - 1;
                cmbStatus.SelectedIndex = equip.IDStatus - 1;
                editEquip = equip;
                if (equip.Photo != null)
                {
                    using (MemoryStream stream = new MemoryStream(equip.Photo))
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

            private void btnAddEquip_Click(object sender, RoutedEventArgs e)
            {
                //валидация
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Поле Наименование не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Поле Стоимость не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtWarranty.Text))
                {
                    MessageBox.Show("Поле Гарантия не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //код
                var authUser = ClassHelper.AppData.Context.Product.ToList().
                Where(i => i.NameProduct == txtName.Text).FirstOrDefault();
                if (authUser != null && IsEdit == false)
                {
                    MessageBox.Show("Данный логин занят!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        editEquip.NameProduct = txtName.Text;
                        editEquip.IDType = (cmbType.SelectedItem as EF.Type).ID;
                        editEquip.Price = Convert.ToDecimal(txtPrice.Text);
                        editEquip.Warranty = Convert.ToDateTime(txtWarranty.Text);
                        editEquip.IDStatus = (cmbStatus.SelectedItem as EF.Status).ID;
                        editEquip.IsDeleted = false;
                        if (photostrl != null)
                        {
                            editEquip.Photo = File.ReadAllBytes(photostrl);
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
                        EF.Product newequip = new EF.Product();
                        newequip.NameProduct = txtName.Text;
                        newequip.IDType = (cmbType.SelectedItem as EF.Type).ID;
                        newequip.Price = Convert.ToDecimal(txtPrice.Text);
                        newequip.Warranty = Convert.ToDateTime(txtWarranty.Text);
                        newequip.IDStatus = (cmbStatus.SelectedItem as EF.Status).ID;
                        newequip.IsDeleted = false;
                        if (photostrl != null)
                        {
                            newequip.Photo = File.ReadAllBytes(photostrl);
                        }
                        ClassHelper.AppData.Context.Product.Add(newequip);
                        ClassHelper.AppData.Context.SaveChanges();
                        MessageBox.Show("Пользователь добавлен");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                //доб


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
    }
}

