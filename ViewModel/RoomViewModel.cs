using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly Room _model;
        private readonly RoomsViewModel _parentViewModel;

        public RoomViewModel(Room model, RoomsViewModel parentViewModel)
        {
            _model = model;
            _parentViewModel = parentViewModel;
        }

        public int Id => _model.Id;

        public string? Name => _model.Name;

        public string Type => _model.Type;

        public string? Description => _model.Description;

        public int MaxGuests => _model.MaxGuests;

        public ObservableCollection<Booking> Bookings { get; } = new();

        public bool IsFull
        {
            get
            {
                if (Bookings.Count == 0) return false;
                
                int occupants = 0;

                foreach (Booking booking in Bookings)
                {
                    if (booking.CheckInDate <= _parentViewModel.OccupancyDate && booking.CheckOutDate >= _parentViewModel.OccupancyDate)
                        occupants++;
                }

                if (occupants >= MaxGuests)
                    return true; 
                else
                    return false;
            }
        }

        public bool IsFree
        {
            get
            {
                if (Bookings.Count == 0)
                    return true;
                foreach (Booking booking in Bookings)
                {
                    if (booking.CheckInDate <= _parentViewModel.OccupancyDate && booking.CheckOutDate >= _parentViewModel.OccupancyDate)
                        return false;
                }
                return true;
            }
        }

        public string? BookingsString
        {
            get
            {
                if (Bookings.Count == 0)
                {
                    return "None";
                }

                var sortedBookings = Bookings.OrderBy(x => x.CheckInDate).ToList();

                string result = "";
                foreach (var booking in sortedBookings)
                {
                    if (booking.CheckOutDate < DateTime.Now)
                    {
                        result += "(PAST BOOKING) ";
                    }
                    var guest = GuestsViewModel.Guests.First(x => x.Id == booking.GuestId);
                    result += $"{booking.CheckInDate.ToLongDateString()} - {booking.CheckOutDate.ToLongDateString()}:\n        {guest.Name} ({guest.Breed})\n\n";
                }

                result = result.Substring(0, result.Length - 2); // delete the last two line breaks

                return result;
            }
        }

        internal void BookingsChanged()
        {
            CollectionViewSource.GetDefaultView(Bookings).Refresh();
            OnPropertyChanged(nameof(IsFull));
            OnPropertyChanged(nameof(IsFree));
            OnPropertyChanged(nameof(BookingsString));
        }
    }
}
