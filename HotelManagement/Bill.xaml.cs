using DB.Models;
using HotelManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for Bill.xaml
    /// </summary>
    public partial class Bill : Window
    {
        public Bill()
        {
            InitializeComponent();
            if (Reservation.currentGuest != null && Reservation.currentGuest.cards.Count != 0)
            {
                foreach (var card in Reservation.currentGuest.cards.GroupBy(c=>c.Id))
                {
                    old_cards.Items.Add(new ComboBoxItem() { Content = $"{card.First().card_number}", Tag = card.Key });
                }
            }

            for (int i = 1; i <= 12; i++)
            {
                month.Items.Add(new ComboBoxItem() { Content = i });
            }

            for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 7; i++)
            {
                year.Items.Add(new ComboBoxItem() { Content = i.ToString()[2..] });
            }

            if (Reservation.CurrentReservation != null && Reservation.currentGuest != null)
            {
                int days = (int) (Reservation.CurrentReservation.leaving_time.Date -
                    Reservation.CurrentReservation.arrival_time.Date).TotalDays - 1;

                RoomBill = days * Reservation.CurrentReservation.room.RoomType.NightPrice;

                FoodBill = Reservation.CurrentReservation.break_fast * 8;
                FoodBill += Reservation.CurrentReservation.break_fast * 10;
                FoodBill += Reservation.CurrentReservation.break_fast * 15;

                Tax = 0.14 * (RoomBill + FoodBill);

                total_due = RoomBill + FoodBill + Tax;
                total_payed = total_due - (double)Reservation.CurrentReservation.total_bill;

                room_bill.Text = $"${Math.Round(RoomBill, 2)} USD";                
                food_bill.Text = $"${Math.Round(FoodBill, 2)} USD";                
                tax.Text = $"${Math.Round(Tax, 2)} USD";
                
                total.Text = $"${Math.Round(total_payed,2)} USD";
            }
            else
            {
                this.Close();
            }
        }

        double RoomBill;
        double FoodBill;
        double Tax;
        double total_due;
        double total_payed;

        private void old_cards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (old_cards.SelectedItem is ComboBoxItem selected )
            {
                if (selected.Tag is int tag)
                {
                    Reservation.currentCard = App.DB.cards.Find(tag);
                    PopulateCardInfo();
                }
            }
        }

        private void PopulateCardInfo()
        {
            if (Reservation.currentCard != null)
            {
                card_number.Text = Reservation.currentCard.card_number;
                CVC.Text = Reservation.currentCard.card_cvc;
                month.Text = Reservation.currentCard.card_exp.Split('/')[0];
                year.Text = Reservation.currentCard.card_exp.Split('/')[1];
                //payment_type.Text = Reservation.CurrentReservation!.payment_type.ToString();
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (Int64.TryParse(card_number.Text, out Int64 _) && card_number.Text.Trim().Length == 16)
            {
                if (payment_type.SelectedIndex > 0 && month.SelectedIndex > 0 && year.SelectedIndex > 0)
                {
                    if (Int32.TryParse(CVC.Text, out int _) && CVC.Text.Trim().Length == 3)
                    {
                        if (Math.Round(total_payed, 2) !=  0)
                        {
                            card Card = new();
                        
                            if (old_cards.SelectedIndex > 0)
                                Card = App.DB.cards.FirstOrDefault(c=> c.guest.SSN == Reservation.currentGuest!.SSN
                                    && c.card_number == (string)(old_cards.SelectedItem as ComboBoxItem)!.Content) ?? Card;

                            Card.card_number = card_number.Text;
                            Card.card_type = card_type.Text;
                            Card.card_cvc = CVC.Text;
                            Card.card_exp = month.Text + "/" + year.Text;

                            //Reservation.currentGuest!.cards.Add(Card);

                            Reservation.CurrentReservation!.card = Card;
                            Reservation.CurrentReservation.total_bill = (decimal)(RoomBill + FoodBill + Tax);
                            Reservation.CurrentReservation.food_bill = (decimal)FoodBill;
                            Reservation.CurrentReservation.payment_type = Enum.Parse<PaymentType>(payment_type.Text);

                            this.Close();
                        }
                    }
                }
            }
        }

        private void card_number_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (card_number.Text.Length > 0)
                {
                    switch (int.Parse(""+card_number.Text[0]))
                    {
                        case 3:
                            card_type.Text = "American Express";
                            break;
                        case 4:
                            card_type.Text = "Visa";
                            break;
                        case 5:
                            card_type.Text = "Master Card";
                            break;
                        case 6:
                            card_type.Text = "Discover Card";
                            break;
                        default:
                            card_type.Text = "Unknown";
                            break;
                    }
                }
                else
                {
                    card_type.Text = "Unknown";
                }
            }
            catch { }
        }
    }
}
