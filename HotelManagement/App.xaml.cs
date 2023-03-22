using DB.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HotelContext DB { get; set; } = new HotelContext();
        public static DbConnection DbConnection { get; set; } = DB.Database.GetDbConnection();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EventManager.RegisterClassHandler(typeof(Window), Window.MouseDownEvent,
                new RoutedEventHandler(Window_MouseDown));
        }

        private void Window_MouseDown(object sender, RoutedEventArgs re)
        {
            if (sender is Window window && re is MouseButtonEventArgs e)
            {
                if (e.ChangedButton == MouseButton.Left)
                    window.DragMove();
            }
        }

        public void Close(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Window.GetWindow(button).Close();
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Window.GetWindow(button).WindowState = WindowState.Minimized;
            }
        }

        private void Hint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock hint)
            {
                if (hint.Parent is Grid grid)
                {
                    grid.Children[0].Focus();
                }
            }
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            DB.Dispose();
            base.OnSessionEnding(e);
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is Control input)
            {
                bool switched = (input is TextBox username && username.Text == "")
                    || (input is PasswordBox password && password.Password == "");

                if (input.Parent is Grid grid)
                {
                    if (switched)
                    {
                        grid.Children[1].Visibility = Visibility.Visible;
                        if (grid.Children.Count > 2)
                            grid.Children[2].Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        grid.Children[1].Visibility = Visibility.Hidden;
                        if (grid.Children.Count > 2)
                            grid.Children[2].Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
