
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestsViewModel : ViewModelBase
    {
        private readonly IGuestDataProvider _guestDataProvider;
        private GuestViewModel? _selectedGuest;
        private Booking? _selectedBooking;
        private ServiceBooking? _selectedServiceBooking;
        private string _archiveButtonText = "Archive";
        private string _viewArchiveButtonText = "View Archive";
        private bool _isArchiveHidden = true;
        private DateTime _selectedMenuDate;
        private Window? _mainWindow;

        public GuestsViewModel(IGuestDataProvider guestDataProvider)
        {
            _guestDataProvider = guestDataProvider;
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
            ArchiveCommand = new DelegateCommand(Archive, CanArchive);
            ViewArchiveCommand = new DelegateCommand(ViewArchive);
            AddBookingCommand = new DelegateCommand(AddBooking);
            EditBookingCommand = new DelegateCommand(EditBooking, CanEditBooking);
            RemoveBookingCommand = new DelegateCommand(RemoveBooking, CanRemoveBooking);
            _selectedMenuDate = DateTime.Today;
            _mainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
        }

        public static ObservableCollection<GuestViewModel> Guests { get; } = new();

        public static List<Booking> Bookings { get; } = new();

        public static List<ServiceBooking> ServiceBookings { get; } = new();

        public static List<GuestMenu> GuestMenus { get; } = new();

        public GuestViewModel? SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGuestSelected));
                RemoveCommand.OnCanExecuteChanged();
                ArchiveCommand.OnCanExecuteChanged();
            }
        }

        public Booking? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged();
                RemoveBookingCommand.OnCanExecuteChanged();
                EditBookingCommand.OnCanExecuteChanged();
            }

        }

        public ServiceBooking? SelectedServiceBooking
        {
            get => _selectedServiceBooking;
            set
            {
                _selectedServiceBooking = value;
                OnPropertyChanged();
                //RemoveBookingCommand.OnCanExecuteChanged();
                //EditBookingCommand.OnCanExecuteChanged();
            }

        }

        public bool IsGuestSelected => SelectedGuest is not null;


        public DateTime SelectedMenuDate
        {
            get => _selectedMenuDate;
            set
            {
                _selectedMenuDate = value;
                OnPropertyChanged();
            }
        }

        public string? BreakfastOption
        {
            get
            {
                if (SelectedGuest is null)
                {
                    return null;
                }
                foreach (GuestMenu guestMenu in SelectedGuest.GuestMenus)
                {
                    if (guestMenu.Date == SelectedMenuDate)
                    {
                        if (guestMenu.Breakfast.Length == 0)
                            return null;
                        return "Option " + guestMenu.Breakfast;
                    }
                }
                return null;
            }
            set
            {
                if (value is null) return;
                if (SelectedGuest is null) return;
                bool dateFound = false;
                foreach (GuestMenu guestMenu in SelectedGuest.GuestMenus)
                {
                    if (guestMenu.Date == SelectedMenuDate)
                    {
                        guestMenu.Breakfast = value.Split()[1];
                        dateFound = true;
                        break;
                    }
                }
                if (!dateFound)
                {
                    GuestMenu g = new GuestMenu();
                    g.Date = SelectedMenuDate;
                    g.Breakfast = value.Split()[1];
                    SelectedGuest.GuestMenus.Add(g);
                }
                OnPropertyChanged();
            }
        }


        public bool IsArchiveHidden
        {
            get => _isArchiveHidden;
            set
            {
                _isArchiveHidden = value;
                if (IsArchiveHidden)
                {
                    ArchiveButtonText = "Archive";
                    ViewArchiveButtonText = "View Archive";
                }
                else
                {
                    ArchiveButtonText = "Unarchive";
                    ViewArchiveButtonText = "View Current Guests";
                }
                OnPropertyChanged();
                CollectionViewSource.GetDefaultView(Guests).Refresh();
            }
        }

        public string ArchiveButtonText
        {
            get => _archiveButtonText;
            set
            {
                _archiveButtonText = value;
                OnPropertyChanged();
            }
        }

        public string ViewArchiveButtonText
        {
            get => _viewArchiveButtonText;
            set
            {
                _viewArchiveButtonText = value;
                OnPropertyChanged();
            }
        }

        public string[] EarFloppinessValues
        {
            get
            {
                var array = Enum.GetValues<EarFloppiness>().Select(x => x.ToString().Replace('_', ' ')).ToArray();
                array[0] = "";
                return array;
            }
        }

        public static ObservableCollection<Booking> SortBookings(ObservableCollection<Booking> observableCollection)
        {
            ObservableCollection<Booking> temp;
            temp = new ObservableCollection<Booking>(observableCollection.OrderBy(x => x.CheckInDate));
            observableCollection.Clear();
            foreach (var booking in temp)
                observableCollection.Add(booking);
            return observableCollection;
        }

        public static ObservableCollection<ServiceBooking> SortServiceBookings(ObservableCollection<ServiceBooking> observableCollection)
        {
            ObservableCollection<ServiceBooking> temp;
            temp = new ObservableCollection<ServiceBooking>(observableCollection.OrderBy(x => x.Date).ThenBy(x => x.StartTime));
            observableCollection.Clear();
            foreach (var serviceBooking in temp)
                observableCollection.Add(serviceBooking);
            return observableCollection;
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand RemoveCommand { get; }
        public DelegateCommand ArchiveCommand { get; }
        public DelegateCommand ViewArchiveCommand { get; }
        public DelegateCommand AddBookingCommand { get; }
        public DelegateCommand EditBookingCommand { get; }
        public DelegateCommand RemoveBookingCommand { get; }

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
                foreach (var serviceBooking in  serviceBookings)
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

        private void Add(object? parameter)
        {
            var guest = new Guest { Name = "NEW GUEST" };
            var viewModel = new GuestViewModel(guest, this);
            Guests.Add(viewModel);
            SelectedGuest = viewModel;
            IsArchiveHidden = true;
        }

        private void Remove(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                foreach (var booking in SelectedGuest.Bookings)
                {
                    Bookings.Remove(booking);
                    foreach (var room in RoomsViewModel.Rooms)
                    {
                        room.Bookings.Remove(booking);
                    }
                }
                Guests.Remove(SelectedGuest);
                SelectedGuest = null;
                RoomsViewModel.BookingsChanged();
            }
        }

        private bool CanRemove(object? parameter) => SelectedGuest is not null;

        private void Archive(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                SelectedGuest.IsArchived = !SelectedGuest.IsArchived;
                SelectedGuest = null;
            }
        }

        private bool CanArchive(object? parameter) => SelectedGuest is not null;

        private void ViewArchive(object? parameter)
        {
            IsArchiveHidden = !IsArchiveHidden;
            SelectedGuest = null;
        }

        private bool CanEditBooking(object? parameter) => SelectedGuest is not null && SelectedBooking is not null;

        private void EditBooking(object? parameter)
        {
            if (SelectedGuest is not null && SelectedBooking is not null)
            {
                BookingDetails bookingDetails = new BookingDetails("Edit Booking", SelectedGuest, SelectedBooking);
                if (_mainWindow is not null)
                    _mainWindow.Opacity = 0.4;
                bookingDetails.ShowDialog();
                if (_mainWindow is not null)
                    _mainWindow.Opacity = 1.0;
            }
        }

        private bool CanRemoveBooking(object? parameter) => SelectedBooking is not null;

        private void RemoveBooking(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                if (SelectedBooking is not null)
                {
                    Booking booking = SelectedBooking;

                    SelectedGuest.Bookings.Remove(booking);
                    foreach (var room in RoomsViewModel.Rooms)
                    {
                        room.Bookings.Remove(booking);
                    }
                    Bookings.Remove(booking);

                    SelectedBooking = null;
                    RoomsViewModel.BookingsChanged();

                }
            }
        }

        private void AddBooking(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                BookingDetails bookingDetails = new BookingDetails("New Booking", SelectedGuest);
                if (_mainWindow is not null)
                    _mainWindow.Opacity = 0.4;
                bookingDetails.ShowDialog();
                if (_mainWindow is not null)
                    _mainWindow.Opacity = 1.0;
            }
        }
    }
}
