using DB.Models;
using HotelManagement;
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
        }

        private void FillUserData(guest user)
        {
            first_name.Text = user.first_name;
        }
    }
}
