using DB;
using DB.Context;
using DB.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI;

namespace HotelManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            SignInBtn.IsEnabled = false;

            if (Username.Text != "" && Password.Password != "")
            {
                frontend? user;
                kitchen? kitchen;
                if ((user = App.DB.frontends.Find(Username.Text)) != null)
                {
                    if (user.pass_word == Password.Password)
                    {
                        UI.Reservation reservation = new();
                        reservation.Show();
                        this.Close();
                    }
                }
                else if ((kitchen = App.DB.kitchens.Find(Username.Text)) != null)
                {
                    if (kitchen.pass_word == Password.Password)
                    {
                        UI.Kitchen RoomService = new();
                        RoomService.Show();
                        this.Close();
                    }
                }
            }

            SignInBtn.IsEnabled = true;
        }
    }
}
