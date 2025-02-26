﻿using System.Collections.ObjectModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly Room _model;
        private readonly MainViewModel _mainViewModel;

        public RoomViewModel(Room model, MainViewModel mainViewModel)
        {
            _model = model;
            _mainViewModel = mainViewModel;
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
                    if (booking.CheckInDate <= _mainViewModel.RoomsViewModel.OccupancyDate && booking.CheckOutDate >= _mainViewModel.RoomsViewModel.OccupancyDate)
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
                    if (booking.CheckInDate <= _mainViewModel.RoomsViewModel.OccupancyDate && booking.CheckOutDate >= _mainViewModel.RoomsViewModel.OccupancyDate)
                        return false;
                }
                return true;
            }
        }

        // TODO!
        /*
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
        }*/

        internal void BookingsChanged()
        {
            CollectionViewSource.GetDefaultView(Bookings).Refresh();
            OnPropertyChanged(nameof(IsFull));
            OnPropertyChanged(nameof(IsFree));
            //TODO!
            //OnPropertyChanged(nameof(BookingsString));
        }
    }
}
