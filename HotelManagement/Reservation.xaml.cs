using DB.Models;
using HotelManagement;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DB.Manager;

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

            #region Reservation Management
            for (int year = DateTime.Now.Year - 136; year <= DateTime.Now.Year - 16; year++)
            {
                Year.Items.Add(new ComboBoxItem() { Content = year });
            }

            for (int month = 1; month <= 12; month++)
            {
                Month.Items.Add(new ComboBoxItem() { Content = month });
            }

            for (int day = 1; day <= 31; day++)
            {
                Day.Items.Add(new ComboBoxItem() { Content = day });
            }

            Month.SelectionChanged += Day_SelectionChanged;
            Year.SelectionChanged += Day_SelectionChanged;

            var reservationsList = App.DbConnection.Query(
                @"SELECT r.Id, g.first_name, g.last_name, r.arrival_time, rt.Type as RoomType
                  FROM reservation r
                  INNER JOIN guest g ON r.guestSSN = g.SSN
                  INNER JOIN rooms rm ON r.roomId = rm.Id
                  INNER JOIN roomtypes rt ON rm.RoomTypeId = rt.Id
                  WHERE r.leaving_time >= @Now",
                new { Now = DateTime.Now });

            foreach (var reservation in reservationsList)
            {
                OldReservations.Items.Add(new ComboBoxItem()
                {
                    Tag = reservation.Id,
                    Content = $"{reservation.Id} - {(DB.Models.Type) reservation.RoomType} - {reservation.first_name} {reservation.last_name} - {reservation.arrival_time.ToShortDateString()}"
                });
            }

            var roomTypes = App.DbConnection.Query<RoomType>( @"SELECT * FROM RoomTypes" );

            foreach (var type in roomTypes)
            {
                room_type.Items.Add(new ComboBoxItem() { Tag = type.Id, Content = type.Type });
            }

            for (int i = 1; i <= 6; i++)
            {
                number_guest.Items.Add(new ComboBoxItem() { Tag = i, Content = i });
            }

            arrival_time.DisplayDateStart = DateTime.Now;
            #endregion

            #region Grids
            ReservationGrid.Loaded += ReservationGrid_Loaded;

            GuestsGrid.Loaded += GuestsGrid_Loaded;

            ReservedGrid.Loaded += ReservedGrid_Loaded;
            #endregion
        }

        #region Grids
        private void ReservedGrid_Loaded(object sender, RoutedEventArgs e)
        {
            string query = @"SELECT Rooms.Number, RoomTypes.Type, Reservation.Id, Guest.first_name, Guest.last_name, Guest.phone_number,
                 Reservation.check_in
                 FROM Reservation 
                 INNER JOIN Guest ON Reservation.guestSSN = Guest.SSN
                 INNER JOIN Rooms ON Reservation.RoomId = Rooms.Id 
                 INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id 
                 WHERE Reservation.arrival_time <= @now AND Reservation.leaving_time > @now";

            var data = App.DbConnection.Query(query, param: new { now = DateTime.Now });

            var result = data.Select(d => new
            {
                d.Number,
                Type = (DB.Models.Type)d.Type,
                d.Id,
                d.first_name,
                d.last_name,
                d.phone_number,
                check_in = (bool)d.check_in
            });

            var reserved = result.Where(r => !r.check_in).ToList();
            var occupupied = result.Where(r => r.check_in).ToList();

            ReservedGrid.ItemsSource = reserved;

            OccupiedGrid.ItemsSource = occupupied;
        }

        private void GuestsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var guests = App.DbConnection.Query<guest>("SELECT * FROM guest");
            GuestsGrid.ItemsSource = guests;

            if (GuestsGrid.Columns.Count > 0)
            {
                GuestsGrid.Columns[11].Visibility = Visibility.Hidden;
            }
        }

        private void ReservationGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var reservationsList = App.DbConnection.Query<reservation, guest, card, Room, RoomType, reservation>(
                @"SELECT r.*, g.*, c.*, rm.*, rt.*
                    FROM Reservation r
                    INNER JOIN Guest g ON r.GuestSSN = g.SSN
                    INNER JOIN Cards c ON r.CardId = c.Id
                    INNER JOIN rooms rm ON r.roomId = rm.Id
                    INNER JOIN RoomTypes rt ON rm.RoomTypeId = rt.Id",
                (reservation, guest, card, room, roomType) =>
                {
                    reservation.guest = guest;
                    reservation.card = card;
                    reservation.room = room;
                    reservation.room.RoomType = roomType;
                    return reservation;
                },
                splitOn: "Id,SSN,Id,Id,Id");

            ReservationGrid.ItemsSource = reservationsList;

            ReservationGrid.IsReadOnly = true;
            //if (ReservationGrid.Columns.Count > 0)
            //{
            //    ReservationGrid.Columns[0].IsReadOnly = true;
            //    ReservationGrid.Columns[1].IsReadOnly = true;
            //    ReservationGrid.Columns[2].IsReadOnly = true;
            //    ReservationGrid.Columns[3].IsReadOnly = true;
            //    ReservationGrid.Columns[9].IsReadOnly = true;
            //    ReservationGrid.Columns[16].IsReadOnly = true;
            //}
        }
        private void SaveGrid_Click(object sender, RoutedEventArgs e)
        {
            //ReservationGrid.CommitEdit();
            //App.DB.SaveChanges();
        }
        #endregion

        #region Reservation Management
        public static reservation? CurrentReservation;
        public static guest? currentGuest;
        public static card? currentCard;

        private void PopulateDays()
        {
            if (Month.SelectedIndex == 0 || Year.SelectedIndex == 0) { return; }

            int index = Day.SelectedIndex;

            var temp = Day.Items[0];
            Day.Items.Clear();
            Day.Items.Add(temp);


            int month = (int)((ComboBoxItem)Month.SelectedItem).Content;
            int year = (int)((ComboBoxItem)Year.SelectedItem).Content;

            int days = DateTime.DaysInMonth(year, month);
            index = Math.Min(index, days);
            for (int day = 1; day <= days; day++)
            {
                Day.Items.Add(new ComboBoxItem() { Content = day });
            }

            Day.SelectedIndex = index;
        }

        private void SSN_TextChanged(object sender, TextChangedEventArgs e)
        {
            long ssn;
            if (SSN.Text.Length == 14 && long.TryParse(SSN.Text, out ssn))
            {
                currentGuest = App.DbConnection.QueryFirstOrDefault<guest>(
                    @"SELECT * FROM Guest WHERE SSN = @ssn",
                    new { ssn });

                if (currentGuest != null)
                {
                    currentGuest.cards = App.DbConnection.Query<card>(
                        @"SELECT * FROM Cards WHERE GuestSSN = @ssn",
                        new { ssn }).ToList();

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
                    room_type.SelectedIndex = 0;
                    CurrentReservation = App.DbConnection.Query<reservation, guest, card, Room, RoomType, reservation>(
                        @"SELECT r.*, g.*, c.*, rm.*, rt.*
                          FROM Reservation r
                          INNER JOIN Guest g ON r.GuestSSN = g.SSN
                          LEFT JOIN Cards c ON r.CardId = c.Id
                          INNER JOIN rooms rm ON r.roomId = rm.Id
                          INNER JOIN RoomTypes rt ON rm.RoomTypeId = rt.Id
                          WHERE r.Id = @id",
                        (reservation, guest, card, room, roomType) =>
                        {
                            reservation.guest = guest;
                            reservation.card = card;
                            reservation.room = room;
                            reservation.room.RoomType = roomType;
                            return reservation;
                        },
                        new { id = tag },
                        splitOn: "SSN,Id,Id,Id").FirstOrDefault();

                    if (CurrentReservation != null)
                    {
                        currentGuest = CurrentReservation.guest;
                        currentCard = CurrentReservation.card;
                        FillUserData();
                        FillReservationData();
                    }
                }
            }
        }

        private void room_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (floor != null && room_type.SelectedItem != null)
            {
                var temp = number_guest.Items[0];
                number_guest.Items.Clear();
                number_guest.Items.Add(temp);

                temp = floor.Items[0];
                floor.Items.Clear();
                floor.Items.Add(temp);

                floor.SelectedIndex = 0;
                number_guest.SelectedIndex = 0;
            }

            if (room_type.SelectedItem is ComboBoxItem selected && floor != null)
            {

                if (selected.Tag is int tag)
                {
                    var roomType = App.DbConnection.QueryFirstOrDefault<RoomType>(
                        @"SELECT * FROM RoomTypes WHERE Id = @tag",
                        new { tag }
                    );

                    if (roomType != null)
                    {
                        int index = number_guest.SelectedIndex;
                        int max = roomType.Capacity;
                        index = Math.Min(index, max);

                        for (int i = 1; i <= max; i++)
                        {
                            number_guest.Items.Add(new ComboBoxItem() { Content = i });
                        }

                        number_guest.SelectedIndex = index;

                        var floors = App.DbConnection.QueryAsync<int>(
                            @"SELECT DISTINCT Floor FROM Rooms WHERE RoomTypeId = @roomTypeId",
                            new { roomTypeId = roomType.Id }
                        ).Result.ToList();

                        foreach (var floorNumber in floors)
                        {
                            floor.Items.Add(new ComboBoxItem() { Tag = floorNumber, Content = floorNumber });
                        }
                    }

                    floor.SelectedIndex = 0;
                    number_guest.SelectedIndex = 0;
                }
            }
        }

        private void floor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (room_number != null && floor.SelectedItem != null)
            {
                var temp = room_number.Items[0];
                room_number.Items.Clear();
                room_number.Items.Add(temp);

                room_number.SelectedIndex = 0;
            }
            if (floor.SelectedItem is ComboBoxItem selected && room_type.SelectedItem is ComboBoxItem roomType && room_number != null)
            {

                if (selected.Tag is int tag && roomType.Tag is int typeID)
                {
                    var rooms = App.DbConnection.Query<Room>(
                        "SELECT * FROM Rooms WHERE Floor = @Floor AND RoomTypeId = @RoomTypeId",
                        new { Floor = tag, RoomTypeId = typeID }
                    ).ToList();

                    foreach (var room in rooms)
                    {
                        if (IsRoomAvailable(room, arrival_time.SelectedDate, leaving_time.SelectedDate))
                        {
                            room_number.Items.Add(new ComboBoxItem() { Tag = room.Number, Content = room.Number });
                        }
                    }
                }
                
                room_number.SelectedIndex = 0;
            }
        }

        bool IsRoomAvailable(Room room, DateTime? arrival, DateTime? leaving)
        {
            if (arrival == null || leaving == null) return false;

            int currentID = CurrentReservation?.Id??-1;

            var existingReservations = App.DbConnection.Query(
                "SELECT Id, RoomId, Arrival_Time, Leaving_Time FROM Reservation WHERE Id != @CurrentId AND RoomId = @RoomId AND Arrival_Time < @Leaving AND @Arrival < Leaving_Time",
                new { CurrentId = currentID, RoomId = room.Id, Arrival = arrival, Leaving = leaving }
            ).ToList();

            return !existingReservations.Any();
        }

        private void arrival_time_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrival_time.SelectedDate != null)
            {
                leaving_time.DisplayDateStart = arrival_time.SelectedDate.Value.AddDays(1);
                room_type.SelectedIndex = 0;
            }
        }

        bool SyncInfo()
        {
            reservation? temp = null;
            if (CurrentReservation != null && CurrentReservation.card != null)
            {
                temp = CurrentReservation;
            }
            try
            {
                if (CurrentReservation == null)
                {
                    CurrentReservation = new();
                }

                CurrentReservation.guest = currentGuest;

                if (CurrentReservation.guest == null) 
                { 
                    CurrentReservation.guest = new();
                }

                if (temp == null)
                {
                    CurrentReservation.payment_type = PaymentType.Pending;
                }
                else
                {
                    CurrentReservation.card = temp.card;
                    CurrentReservation.total_bill = temp.total_bill;
                    CurrentReservation.food_bill = temp.food_bill;
                    CurrentReservation.payment_type = temp.payment_type;
                    CurrentReservation.guest.cards.Add(temp.card);
                }

                //if (CurrentReservation.room != null) { App.DB.Entry(CurrentReservation.room).State = EntityState.Modified; }
                //else { CurrentReservation.room = new(); }

                //if (CurrentReservation.card != null) { App.DB.Entry(CurrentReservation.card).State = EntityState.Modified; }
                //else { CurrentReservation.card = new(); }

                CurrentReservation.guest.SSN = long.Parse(SSN.Text);
                CurrentReservation.guest.first_name = first_name.Text;
                CurrentReservation.guest.last_name = last_name.Text;
                CurrentReservation.guest.birth_day = new DateTime(Int32.Parse(Year.Text), Int32.Parse(Month.Text), Int32.Parse(Day.Text));
                CurrentReservation.guest.gender = gender.Text;
                CurrentReservation.guest.phone_number = phone_number.Text;
                CurrentReservation.guest.email_address = email_address.Text;
                CurrentReservation.guest.street_address = street_address.Text;
                CurrentReservation.guest.city = city.Text;
                CurrentReservation.guest.state = state.Text;
                CurrentReservation.guest.apt_suite = apt_suite.Text;
                CurrentReservation.guest.zip_code = zip_code.Text;

                CurrentReservation.number_guest = Int32.Parse(number_guest.Text);
                
                CurrentReservation.room = CurrentReservation.room = App.DbConnection
                    .QueryFirstOrDefault<Room>(@"SELECT * FROM Rooms
                        WHERE Number = @Number",
                    new { Number = (int)((ComboBoxItem)room_number.SelectedItem).Tag });
                CurrentReservation.roomId = CurrentReservation.room.Id;

                CurrentReservation.room.RoomType = App.DbConnection
                    .QueryFirstOrDefault<RoomType>(@"SELECT * FROM Roomtypes rt
                        inner join rooms r on r.roomtypeid = rt.id
                        WHERE r.id = @id",
                    new { id = CurrentReservation.room.Id});

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

                currentGuest = CurrentReservation.guest;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            try
            {
                int id = CurrentReservation?.Id ?? -1;
                if (SyncInfo() && CurrentReservation != null)
                {
                    if (GuestManager.Update(currentGuest!, out long gid)
                        && (CardManager.Update(currentCard!, out long cid) || true)
                        && ReservationManager.Update(CurrentReservation, out long rid))
                    {
                        currentGuest!.SSN = gid;
                        if (currentCard != null) currentCard.Id = (int) cid;
                        CurrentReservation.Id = (int) rid;
                    }
                    else
                    {
                        throw new Exception();
                    }

                    ComboBoxItem comboBoxItem = new ComboBoxItem()
                    {
                        Tag = CurrentReservation.Id,
                        IsSelected = true,
                        Content = $"{CurrentReservation.Id} - {CurrentReservation.room!.RoomType.Type} - {CurrentReservation.guest.first_name} {CurrentReservation.guest.last_name} - {CurrentReservation.arrival_time.ToShortDateString()}"
                    };
                    
                    if (id != CurrentReservation.Id)
                    {
                        OldReservations.Items.Add(comboBoxItem);
                    }
                    else
                    {
                        int temp = OldReservations.SelectedIndex;
                        OldReservations.Items[OldReservations.SelectedIndex] = comboBoxItem;
                        OldReservations.SelectedIndex = temp;
                    }

                    MessageBox.Show("Reservation Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't Save Data, please check that all fields are valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                //FixEntity(CurrentReservation!);
                //FixEntity(CurrentReservation!.guest!);
                //FixEntity(CurrentReservation!.card!);
            }
        }

        void FixEntity(dynamic entity)
        {
            try
            {
                if (App.DB.Entry(entity).State == EntityState.Added)
                {
                    App.DB.Entry(entity).State = EntityState.Detached;
                }
                else
                {
                    App.DB.Entry(entity).Reload();
                    FillReservationData();
                }
            }
            catch { }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ResetReservationData();
            ResetUserInfo();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReservation != null && check_in.IsChecked != true)
            {
                if (!ReservationManager.Delete(CurrentReservation.Id))
                {
                    throw new Exception();
                }

                OldReservations.Items.RemoveAt(OldReservations.SelectedIndex);

                OldReservations.SelectedIndex = 0;
                CurrentReservation = null;

                ResetReservationData();
                ResetUserInfo();

                MessageBox.Show("Reservation Deleted Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Can't Delete Reservation", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FinalizeBill_Click(object sender, RoutedEventArgs e)
        {
            if (SyncInfo())
            {
                Bill bill = new Bill();
                bill.ShowDialog();
                SaveData();
            }
        }
        private void leaving_time_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (leaving_time.SelectedDate !=  null)
            {
                arrival_time.DisplayDateEnd = leaving_time.SelectedDate;
                room_type.SelectedIndex = 0;
            }
        }
        #endregion

        #region Search
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var query = UniversalQuery.Text.Trim().ToLower();

            UniversalGrid.ItemsSource = App.DB.reservations.Select(r => new
                {
                    r.Id, r.food_bill, r.total_bill,
                    r.guest.SSN, r.guest.first_name, r.guest.last_name, r.guest.birth_day,
                    r.guest.email_address, r.guest.phone_number, r.guest.street_address,
                    r.guest.city, r.guest.apt_suite, r.guest.zip_code, r.arrival_time,
                    r.leaving_time, r.lunch, r.break_fast, r.dinner, r.number_guest,
                    r.supply_status, r.check_in, r.payment_type,
                    r.card.card_type, r.card.card_exp, r.card.card_number, r.card.card_cvc,
                    r.cleaning, r.towel, r.s_surprise, r.room.Number, r.room.RoomType.Type,
                    r.room.Floor
                }).ToList()
                .Where(r => SearchObjectQuery(r, query)).ToList();
        }

        bool SearchObjectQuery(dynamic obj, string query)
        {
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.GetValue(obj, null)?.ToString().Trim().ToLower().Contains(query) == true) return true;
            }
            return false;
        }
        #endregion
    }
}
