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

        private void Hint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock hint)
            {
                if (hint.Parent is Grid grid)
                {
                    grid.Children[1].Focus();
                }
            }
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Control input)
            {
                if (input.Parent is Grid grid)
                {
                    grid.Children[2].Visibility = Visibility.Hidden;
                    grid.Children[0].Visibility = Visibility.Visible;
                }
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Control input)
            {
                bool switched = (input is TextBox username && username.Text == "")
                    || (input is PasswordBox password && password.Password == "");

                if (switched && input.Parent is Grid grid)
                {
                    grid.Children[2].Visibility = Visibility.Visible;
                    grid.Children[0].Visibility = Visibility.Hidden;
                }
            }
        }

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            SignInBtn.IsEnabled = false;

            if (Username.Text != "" &&  Password.Password != "")
            {
                frontend? user;
                if ((user = App.DB.frontends.Find(Username.Text)) != null) 
                {
                    if (user.pass_word == Password.Password)
                    {
                        UI.Reservation reservation = new UI.Reservation();
                        reservation.Show();
                        this.Close();
                    }
                }
            }

            SignInBtn.IsEnabled = true;
        }
    }
}
