using DB.Models;
using HotelManagement;
using Microsoft.EntityFrameworkCore;
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

namespace UI
{
    /// <summary>
    /// Interaction logic for Kitchen.xaml
    /// </summary>
    public partial class Kitchen : Window
    {
        public Kitchen()
        {
            InitializeComponent();
        }

        reservation? CurrentReservation;

        private void Overview_Loaded(object sender, RoutedEventArgs e)
        {
            Overview.ItemsSource = App.DB.reservations.Select(r => new
            {
                r.Id,
                r.guest.first_name,
                r.guest.last_name,
                r.guest.phone_number,
                r.room.RoomType.Type,
                r.room.Floor,
                r.room.Number,
                r.break_fast,
                r.lunch,
                r.dinner,
                r.cleaning,
                r.towel,
                r.s_surprise,
                r.supply_status,
                r.food_bill
            }).ToList();
        }

        private void online_Loaded(object sender, RoutedEventArgs e)
        {
            online.ItemsSource =  App.DB.reservations
                .Where(r => r.check_in && r.arrival_time <= DateTime.Now && r.leaving_time >= DateTime.Now)
                .Select(
                    r => new ListViewItem()
                    {
                        Content =
                            $"{r.Id} {r.guest.first_name} {r.guest.last_name} {r.guest.phone_number}",
                        Tag = r.Id
                    }
                ).ToList();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentReservation != null)
                {
                    CurrentReservation.break_fast = Int32.Parse(breakfast.Text);
                    CurrentReservation.lunch = Int32.Parse(lunch.Text);
                    CurrentReservation.dinner = Int32.Parse(dinner.Text);

                    CurrentReservation.cleaning = cleaning.IsChecked == true;
                    CurrentReservation.towel = towels.IsChecked == true;
                    CurrentReservation.s_surprise = s_surprise.IsChecked == true;

                    CurrentReservation.supply_status = supply_status.IsChecked == true;

                    App.DB.Update(CurrentReservation);
                    App.DB.SaveChanges();

                    MessageBox.Show("Data Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.DB.Entry(CurrentReservation!).Reload();
            }
        }

        private void online_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (online.SelectedItems.Count == 1 && online.SelectedItems[0] is ListViewItem selected)
            {
                if (selected.Tag is int tag)
                {
                    CurrentReservation = App.DB.reservations
                        .Include(r => r.guest)
                        .Include(r => r.room)
                        .ThenInclude(r => r.RoomType)
                        .FirstOrDefault(r => r.Id == tag);

                    PopulateInfo();
                }
            }
        }

        private void PopulateInfo()
        {
            if (CurrentReservation != null)
            {
                first_name.Text = CurrentReservation.guest.first_name;
                last_name.Text = CurrentReservation.guest.last_name;
                phone.Text = CurrentReservation.guest.phone_number;
                room_number.Text = CurrentReservation.room.Number.ToString();
                floor.Text = CurrentReservation.room.Floor.ToString();
                room_type.Text = CurrentReservation.room.RoomType.Type.ToString();

                breakfast.Text = CurrentReservation.break_fast.ToString();
                lunch.Text = CurrentReservation.lunch.ToString();
                dinner.Text = CurrentReservation.dinner.ToString();

                cleaning.IsChecked = CurrentReservation.cleaning;
                towels.IsChecked = CurrentReservation.towel;
                s_surprise.IsChecked = CurrentReservation.s_surprise;

                supply_status.IsChecked = CurrentReservation.supply_status;
            }
        }

        private void supply_status_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReservation != null && supply_status.IsChecked == false)
            {
                PopulateInfo();
            }
            else
            {
                cleaning.IsChecked = false;
                towels.IsChecked = false;
                s_surprise.IsChecked = false;
            }
        }
    }
}
