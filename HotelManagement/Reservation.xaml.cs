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

            foreach (var reservation in App.DB.reservations.Select(r => new { FName = r.guest.first_name, LName = r.guest.last_name, Id = r.Id, Date = r.arrival_time }))
            {
                OldReservations.Items.Add(new ComboBoxItem() { Tag = reservation.Id, Content = $"{reservation.Id} - {reservation.FName} {reservation.LName} - {reservation.Date.ToShortDateString()}" });
            }
        }

        reservation? CurrentReservation;

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
                guest? user = App.DB.guests.Find(ssn);
                if (user != null)
                {
                    FillUserData(user);
                }
            }
            else
            {
                ResetUserInfo();
            }
        }

        private void ResetUserInfo()
        {
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

        private void FillUserData(guest user)
        {
            SSN.Text = user.SSN.ToString();
            first_name.Text = user.first_name;
            last_name.Text = user.last_name;
            Year.Text = user.birth_day.Year.ToString();
            Month.Text = user.birth_day.Month.ToString();
            Day.Text = user.birth_day.Day.ToString();
            phone_number.Text = user.phone_number;
            email_address.Text = user.email_address;
            street_address.Text = user.street_address;
            apt_suite.Text = user.apt_suite;
            city.Text = user.city;
            state.Text = user.state;
            zip_code.Text = user.zip_code;
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
                    CurrentReservation = App.DB.reservations.Where(r => r.Id == tag).Include(r => r.guest).FirstOrDefault() ;
                    if (CurrentReservation != null)
                    {
                        FillUserData(CurrentReservation.guest);
                    }
                }
            }
        }
    }
}
