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
    /// Interaction logic for Reservation.xaml
    /// </summary>
    public partial class Reservation : Window
    {
        public Reservation()
        {
            InitializeComponent();

            for (int year = DateTime.Now.Year - 136; year <= DateTime.Now.Year - 16; year++)
            {
                Year.Items.Add(new ComboBoxItem() { Content = year});
            }

            for (int month = 1; month <= 12; month++)
            {
                Month.Items.Add(new ComboBoxItem() { Content = month});
            }

            for (int day = 1; day <= 31; day++)
            {
                Day.Items.Add(new ComboBoxItem() { Content = day });
            }

            Month.SelectionChanged += Day_SelectionChanged;
            Year.SelectionChanged += Day_SelectionChanged;

            foreach (var reservation in App.DB.reservations.Where(r => r.leaving_time >= DateTime.Now).Select(r => new { r.guest.first_name, r.guest.last_name, r.Id, r.arrival_time, r.room.RoomType }))
            {
                OldReservations.Items.Add(new ComboBoxItem() { Tag = reservation.Id, Content = $"{reservation.Id} - {reservation.RoomType.Type} - {reservation.first_name} {reservation.last_name} - {reservation.arrival_time.ToShortDateString()}" });
            }

            foreach (var type in App.DB.RoomTypes)
            {
                room_type.Items.Add(new ComboBoxItem() { Tag = type.Id, Content = type.Type });
            }

            for (int i = 1; i <= 6; i++)
            {
                number_guest.Items.Add(new ComboBoxItem() { Tag = i, Content = i });
            }

            arrival_time.DisplayDateStart = DateTime.Now;
        }

        reservation? CurrentReservation;
        guest? currentGuest;

        private void PopulateDays()
        {
            if (Month.SelectedIndex == 0 || Year.SelectedIndex == 0) { return; }

            int index = Day.SelectedIndex;

            var temp = Day.Items[0];
            Day.Items.Clear();
            Day.Items.Add(temp);


            int month = (int) ((ComboBoxItem)Month.SelectedItem).Content;
            int year = (int)((ComboBoxItem)Year.SelectedItem).Content;

            int days = DateTime.DaysInMonth(year, month);
            index = Math.Min(index, days);
            for (int day = 1; day <= days; day++)
            {
                Day.Items.Add(new ComboBoxItem() { Content = day});
            }

            Day.SelectedIndex = index;
        }

        private void SSN_TextChanged(object sender, TextChangedEventArgs e)
        {
            long ssn;
            if (SSN.Text.Length == 14 && long.TryParse(SSN.Text, out ssn))
            {
                currentGuest = App.DB.guests.Find(ssn);
                if (currentGuest != null)
                {
                    FillUserData();
                }
            }
        }

        private void ResetUserInfo()
        {
            currentGuest = null;

            SSN.Text = "";
            first_name.Text = "";
            last_name.Text = "";
            Year.Text = "Year";
            Month.Text = "Month";
            Day.Text = "Day";
            phone_number.Text = "";
            email_address.Text = "";
            street_address.Text = "";
            apt_suite.Text = "";
            city.Text = "";
            state.Text = "";
            zip_code.Text = "";
        }

        private void FillUserData()
        {
            if (currentGuest == null) return;
            
            SSN.Text = currentGuest.SSN.ToString();
            first_name.Text = currentGuest.first_name;
            last_name.Text = currentGuest.last_name;
            Year.Text = currentGuest.birth_day.Year.ToString();
            Month.Text = currentGuest.birth_day.Month.ToString();
            Day.Text = currentGuest.birth_day.Day.ToString();
            phone_number.Text = currentGuest.phone_number;
            email_address.Text = currentGuest.email_address;
            street_address.Text = currentGuest.street_address;
            apt_suite.Text = currentGuest.apt_suite;
            city.Text = currentGuest.city;
            state.Text = currentGuest.state;
            zip_code.Text = currentGuest.zip_code;
        }

        private void FillReservationData()
        {
            if (CurrentReservation == null) return;

            arrival_time.SelectedDate = CurrentReservation.arrival_time;
            leaving_time.SelectedDate = CurrentReservation.leaving_time;
            
            room_type.Text = CurrentReservation.room.RoomType.Type.ToString();
            floor.Text = CurrentReservation.room.Floor.ToString();
            room_number.Text = CurrentReservation.room.Number.ToString();

            number_guest.Text = CurrentReservation.number_guest.ToString();

            check_in.IsChecked = CurrentReservation.check_in;
            supply_status.IsChecked = CurrentReservation.supply_status;

            breakfast.Text = CurrentReservation.break_fast.ToString();
            lunch.Text = CurrentReservation.lunch.ToString();
            dinner.Text = CurrentReservation.dinner.ToString();

            cleaning.IsChecked = CurrentReservation.cleaning;
            towels.IsChecked = CurrentReservation.towel;
            s_surprise.IsChecked = CurrentReservation.s_surprise;
        }

        private void ResetReservationData()
        {
            CurrentReservation = null;
            OldReservations.SelectedIndex = 0;

            room_type.SelectedIndex = 0;
            floor.SelectedIndex = 0;
            room_number.SelectedIndex = 0;

            number_guest.SelectedIndex = 0;

            arrival_time.SelectedDate = null;
            leaving_time.SelectedDate = null;

            check_in.IsChecked = false;
            supply_status.IsChecked = false;

            breakfast.Text = "";
            lunch.Text = "";
            dinner.Text = "";

            cleaning.IsChecked = false;
            towels.IsChecked = false;
            s_surprise.IsChecked = false;
        }

        private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDays();
        }

        private void OldReservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OldReservations.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Tag is int tag)
                {
                    CurrentReservation = App.DB.reservations.Where(r => r.Id == tag).Include(r => r.guest).Include(r => r.room.RoomType).FirstOrDefault() ;
                    if (CurrentReservation != null)
                    {
                        FillUserData();
                        FillReservationData();
                    }
                }
            }
        }

        private void room_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (room_type.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Tag is int tag)
                {
                    RoomType? roomType = App.DB.RoomTypes.Find(tag);
                    if (roomType != null)
                    {   
                        int index = number_guest.SelectedIndex;

                        var temp = number_guest.Items[0];
                        number_guest.Items.Clear();
                        number_guest.Items.Add(temp);

                        int max = roomType.Capacity;

                        index = Math.Min(index, max);
                        for (int i = 1; i <= max; i++)
                        {
                            number_guest.Items.Add(new ComboBoxItem() { Content = i });
                        }

                        number_guest.SelectedIndex = index;

                        temp = floor.Items[0];
                        floor.Items.Clear();
                        floor.Items.Add(temp);

                        foreach (var f in App.DB.Rooms.Where(r => r.RoomType == roomType).GroupBy(r => r.Floor))
                        {
                            floor.Items.Add(new ComboBoxItem() { Tag = f.Key, Content = f.Key });
                        }

                        floor.SelectedIndex = 0;

                        temp = room_number.Items[0];
                        room_number.Items.Clear();
                        room_number.Items.Add(temp);
                        room_number.SelectedIndex = 0;
                    }
                }
            }
        }

        private void floor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (floor.SelectedItem is ComboBoxItem selected && room_type.SelectedItem is ComboBoxItem roomType)
            {
                if (selected.Tag is int tag && roomType.Tag is int typeID)
                {
                    var temp = room_number.Items[0];
                    room_number.Items.Clear();
                    room_number.Items.Add(temp);

                    foreach (var room in App.DB.Rooms.Where(r => r.Floor == tag && r.RoomType.Id == typeID))
                    {
                        room_number.Items.Add(new ComboBoxItem() { Tag = room.Number, Content = room.Number });
                    }
                    
                    room_number.SelectedIndex = 0;
                }
            }
        }

        private void arrival_time_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrival_time.SelectedDate != null)
            {
                leaving_time.DisplayDateStart = arrival_time.SelectedDate.Value.AddDays(1);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                bool added = false;

                if (CurrentReservation != null && (App.DB.Entry(CurrentReservation).State == EntityState.Unchanged || App.DB.Entry(CurrentReservation).State == EntityState.Modified))
                { App.DB.Entry(CurrentReservation).State = EntityState.Modified; }
                else {
                    CurrentReservation = new();
                    added = true;
                }

                CurrentReservation.guest = currentGuest;

                if (CurrentReservation.guest != null) { App.DB.Entry(CurrentReservation.guest).State = EntityState.Modified; }
                else { CurrentReservation.guest = new(); }

                if (CurrentReservation.room != null) { App.DB.Entry(CurrentReservation.room).State = EntityState.Modified; }
                else { CurrentReservation.room = new(); }

                /*if (CurrentReservation.card != null) { App.DB.Entry(CurrentReservation.card).State = EntityState.Modified; }
                else { CurrentReservation.card = new(); }*/

                CurrentReservation.guest.SSN = long.Parse(SSN.Text);
                CurrentReservation.guest.first_name = first_name.Text;
                CurrentReservation.guest.last_name = last_name.Text;
                CurrentReservation.guest.birth_day = new DateTime(Int32.Parse(Year.Text), Int32.Parse(Month.Text), Int32.Parse(Day.Text));
                CurrentReservation.guest.phone_number = phone_number.Text;
                CurrentReservation.guest.email_address = email_address.Text;
                CurrentReservation.guest.street_address = street_address.Text;
                CurrentReservation.guest.city = city.Text;
                CurrentReservation.guest.state = state.Text;
                CurrentReservation.guest.apt_suite = apt_suite.Text;
                CurrentReservation.guest.zip_code = zip_code.Text;

                CurrentReservation.number_guest = Int32.Parse(number_guest.Text);
                CurrentReservation.room = App.DB.Rooms.Where(r => r.Number == Int32.Parse(room_number.Text)).FirstOrDefault();
                CurrentReservation.arrival_time = arrival_time.SelectedDate ?? DateTime.Now;
                CurrentReservation.leaving_time = leaving_time.SelectedDate ?? DateTime.Now.AddDays(1);

                CurrentReservation.check_in = check_in.IsChecked ?? false;
                CurrentReservation.supply_status = supply_status.IsChecked ?? false;

                CurrentReservation.break_fast = Int32.Parse("0" + breakfast.Text);
                CurrentReservation.lunch = Int32.Parse("0" + lunch.Text);
                CurrentReservation.dinner = Int32.Parse("0" + dinner.Text);

                CurrentReservation.cleaning = cleaning.IsChecked ?? false;
                CurrentReservation.towel = towels.IsChecked ?? false;
                CurrentReservation.s_surprise = s_surprise.IsChecked ?? false;

                CurrentReservation.payment_type = PaymentType.Pending;

                if (added)
                    App.DB.reservations.Add(CurrentReservation);

                App.DB.SaveChanges();

                added = App.DB.Entry(CurrentReservation).State == EntityState.Added;

                ComboBoxItem comboBoxItem = new ComboBoxItem()
                {
                    Tag = CurrentReservation.Id,
                    IsSelected = true,
                    Content = $"{CurrentReservation.Id} - {CurrentReservation.room!.RoomType.Type} - {CurrentReservation.guest.first_name} {CurrentReservation.guest.last_name} - {CurrentReservation.arrival_time.ToShortDateString()}"
                };

                if (added)
                {
                    OldReservations.Items.Add(comboBoxItem);
                }
                else
                {
                    int temp = OldReservations.SelectedIndex;
                    OldReservations.Items[OldReservations.SelectedIndex] = comboBoxItem;
                    OldReservations.SelectedIndex = temp;
                }

                currentGuest = CurrentReservation.guest;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (App.DB.Entry(CurrentReservation!).State == EntityState.Added)
                    App.DB.reservations.Remove(CurrentReservation!);
                else
                {
                    App.DB.Entry(CurrentReservation!).Reload();
                    FillReservationData();
                }
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ResetReservationData();
            ResetUserInfo();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReservation != null)
            {
                App.DB.reservations.Remove(CurrentReservation);

                if (App.DB.Entry(CurrentReservation).State == EntityState.Deleted)
                {
                    OldReservations.Items.RemoveAt(OldReservations.SelectedIndex);
                    OldReservations.SelectedIndex = 0;
                    CurrentReservation = null;
                }

                ResetReservationData();
                ResetUserInfo();
                
                App.DB.SaveChanges();
            }
        }
    }
}
