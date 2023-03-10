using DB;
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
            UsernameLabel.Visibility = Visibility.Hidden;
            PasswordLabel.Visibility = Visibility.Hidden;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void UsernameHint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Username.Focus();
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernameHint.Visibility = Visibility.Hidden;
            UsernameLabel.Visibility = Visibility.Visible;
        }

        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "")
            {
                UsernameHint.Visibility = Visibility.Visible;
                UsernameLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "")
            {
                PasswordHint.Visibility = Visibility.Visible;
                PasswordLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordHint.Visibility = Visibility.Hidden;
            PasswordLabel.Visibility = Visibility.Visible;
        }

        private void PasswordHint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Password.Focus();
        }
    }
}
