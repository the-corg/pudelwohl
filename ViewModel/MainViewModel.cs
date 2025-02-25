using System.Collections.ObjectModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGuestDataProvider _guestDataProvider;
        private readonly IRoomDataProvider _roomDataProvider;
        private readonly IServiceDataProvider _serviceDataProvider;
        private readonly IMealOptionDataProvider _mealOptionDataProvider;

        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public ObservableCollection<RoomViewModel> Rooms { get; } = new();
        public ObservableCollection<ServiceViewModel> Services { get; } = new();
        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();
        public ObservableCollection<Booking> Bookings { get; } = new();
        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();
        public ObservableCollection<GuestMenu> GuestMenus { get; } = new();



        public void BookingsChanged()
        {
            foreach (var room in Rooms)
            {
                room.BookingsChanged();
            }
            CollectionViewSource.GetDefaultView(Rooms).Refresh();
            StaticPropertyChanged?.Invoke(null, FreeRoomsTodayPropertyEventArgs);
        }


        public async Task LoadAsync()
        {
            if (Guests.Count > 0)
                return;

            var guests = await _guestDataProvider.GetAllAsync();
            if (guests is not null)
            {
                foreach (var guest in guests)
                {
                    Guests.Add(new GuestViewModel(guest, this));
                }
            }

            var bookings = await BookingDataProvider.GetAllAsync();
            if (bookings is not null)
            {
                foreach (var booking in bookings)
                {
                    Bookings.Add(booking);
                    Guests.First(x => x.Id == booking.GuestId).Bookings.Add(booking);
                }
            }

            foreach (var guest in Guests)
            {
                GuestsViewModel.SortBookings(guest.Bookings);
            }

            var serviceBookings = await ServiceBookingDataProvider.GetAllAsync();
            if (serviceBookings is not null)
            {
                foreach (var serviceBooking in serviceBookings)
                {
                    ServiceBookings.Add(serviceBooking);
                    Guests.First(x => x.Id == serviceBooking.GuestId).ServiceBookings.Add(serviceBooking);
                }
            }

            var guestMenus = await GuestMenuDataProvider.GetAllAsync();
            if (guestMenus is not null)
            {
                foreach (var guestMenu in guestMenus)
                {
                    GuestMenus.Add(guestMenu);
                    Guests.First(x => x.Id == guestMenu.GuestId).GuestMenus.Add(guestMenu);
                }
            }
        }

        public async Task LoadAsync()
        {
            if (Rooms.Count > 0)
                return;

            var rooms = await _roomDataProvider.GetAllAsync();
            if (rooms is not null)
            {
                foreach (var room in rooms)
                {
                    Rooms.Add(new RoomViewModel(room, this));
                }
            }

            var bookings = await BookingDataProvider.GetAllAsync();
            if (bookings is not null)
            {
                foreach (var booking in bookings)
                {
                    Rooms.First(x => x.Id == booking.RoomId).Bookings.Add(booking);
                }
            }
            RoomsViewModel.BookingsChanged();
        }
        public async Task LoadAsync()
        {
            if (Services.Count > 0)
                return;

            var services = await _serviceDataProvider.GetAllAsync();
            if (services is not null)
            {
                foreach (var service in services)
                {
                    Services.Add(new ServiceViewModel(service));
                }
            }
        }

        public async Task LoadAsync()
        {
            if (MealOptions.Count > 0)
                return;

            var mealOptions = await _mealOptionDataProvider.GetAllAsync();
            if (mealOptions is not null)
            {
                foreach (var mealOption in mealOptions)
                {
                    MealOptions.Add(new MealOptionViewModel(mealOption));
                }
            }
        }

    }
}
