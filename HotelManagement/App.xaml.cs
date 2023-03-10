using DB.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            DB.Dispose();
            base.OnSessionEnding(e);
        }
    }
}
