using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.View;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestsViewModel : ViewModelBase
    {
        private GuestViewModel? _selectedGuest;
        private Booking? _selectedBooking;
        private ServiceBooking? _selectedServiceBooking;
        private string _archiveButtonText = "Archive";
        private string _viewArchiveButtonText = "View Archive";
        private bool _isArchiveHidden = true;
        private DateTime _selectedMenuDate;
        private readonly MainViewModel _mainViewModel;
        private ICollectionView BookingsView { get; set; }
        private ICollectionView ServiceBookingsView { get; set; }

        private Window? _mainWindow;
        // Lazy loading the main window reference (because null when the constructor is called)
        private Window? MainWindow => _mainWindow ??= Window.GetWindow(App.Current.MainWindow) as MainWindow;

        public GuestsViewModel(
            ObservableCollection<GuestViewModel> guests,
            ObservableCollection<Booking> bookings,
            ObservableCollection<ServiceBooking> serviceBookings,
            ObservableCollection<GuestMenu> guestMenus,
            MainViewModel mainViewModel)
        {
            Guests = guests;
            Bookings = bookings;
            ServiceBookings = serviceBookings;
            GuestMenus = guestMenus;
            _mainViewModel = mainViewModel;
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
            ArchiveCommand = new DelegateCommand(Archive, CanArchive);
            ViewArchiveCommand = new DelegateCommand(ViewArchive);
            AddBookingCommand = new DelegateCommand(AddBooking);
            EditBookingCommand = new DelegateCommand(EditBooking, CanEditBooking);
            RemoveBookingCommand = new DelegateCommand(RemoveBooking, CanRemoveBooking);
            _selectedMenuDate = DateTime.Today;

            BookingsView = CollectionViewSource.GetDefaultView(Bookings);
            // Filter the bookings list according to the selected guest
            BookingsView.Filter = (o) => (SelectedGuest is not null) && (((Booking)o).GuestId == SelectedGuest.Id);
            // And sort it by check-in date
            BookingsView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));

            ServiceBookingsView = CollectionViewSource.GetDefaultView(ServiceBookings);
            // Filter the service bookings list according to the selected guest
            ServiceBookingsView.Filter = (o) => (SelectedGuest is not null) && (((ServiceBooking)o).GuestId == SelectedGuest.Id);
            // And sort it by date, then by start time
            ServiceBookingsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            ServiceBookingsView.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));
        }

        public ObservableCollection<GuestViewModel> Guests { get; }
        public ObservableCollection<Booking> Bookings { get; }
        public ObservableCollection<ServiceBooking> ServiceBookings { get; }
        public ObservableCollection<GuestMenu> GuestMenus { get; }

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
                BookingsView.Refresh();
                ServiceBookingsView.Refresh();
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
                // TODO!
                //RemoveServiceBookingCommand.OnCanExecuteChanged();
                //EditServiceBookingCommand.OnCanExecuteChanged();
            }
        }

        // Used for hiding the guest details when no guest is selected
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

        //TODO!
        /*
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
        }*/


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

        // Selectable values for the ComboBox
        public string[] EarFloppinessValues
        {
            get
            {
                var array = Enum.GetValues<EarFloppiness>().Select(x => x.ToString().Replace('_', ' ')).ToArray();
                array[0] = "";
                return array;
            }
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand RemoveCommand { get; }
        public DelegateCommand ArchiveCommand { get; }
        public DelegateCommand ViewArchiveCommand { get; }
        public DelegateCommand AddBookingCommand { get; }
        public DelegateCommand EditBookingCommand { get; }
        public DelegateCommand RemoveBookingCommand { get; }

        private void Add(object? parameter)
        {
            var guest = new Guest { Name = "NEW GUEST" };
            var viewModel = new GuestViewModel(guest, _mainViewModel);
            Guests.Add(viewModel);
            SelectedGuest = viewModel;
            IsArchiveHidden = true;
            // TODO: ID
        }

        private bool CanRemove(object? parameter) => SelectedGuest is not null;
        private void Remove(object? parameter)
        {
            if (SelectedGuest is null)
                return;

            // Remove all bookings, service bookings, menus for the selected guest
            for (int i = Bookings.Count - 1; i >= 0; i--)
                if (Bookings[i].GuestId == SelectedGuest.Id)
                    Bookings.RemoveAt(i);
            for (int i = ServiceBookings.Count - 1; i >= 0; i--)
                if (ServiceBookings[i].GuestId == SelectedGuest.Id)
                    ServiceBookings.RemoveAt(i);
            for (int i = GuestMenus.Count - 1; i >= 0; i--)
                if (GuestMenus[i].GuestId == SelectedGuest.Id)
                    GuestMenus.RemoveAt(i);
            Guests.Remove(SelectedGuest);

            SelectedGuest = null;
        }

        private bool CanArchive(object? parameter) => SelectedGuest is not null;
        private void Archive(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                SelectedGuest.IsArchived = !SelectedGuest.IsArchived;
                SelectedGuest = null;
            }
        }

        private void ViewArchive(object? parameter)
        {
            IsArchiveHidden = !IsArchiveHidden;
            SelectedGuest = null;
        }

        private bool CanEditBooking(object? parameter) => SelectedGuest is not null && SelectedBooking is not null;
        private void EditBooking(object? parameter)
        {
            if (SelectedGuest is null || SelectedBooking is null)
                return;

            BookingDetails bookingDetails = new BookingDetails("Edit Booking", SelectedGuest, SelectedBooking);
            // Dim main window before showing the modal window, then restore it back
            MainWindow.Opacity = 0.4;
            bookingDetails.ShowDialog();
            MainWindow.Opacity = 1.0;
        }

        private bool CanRemoveBooking(object? parameter) => SelectedBooking is not null;
        private void RemoveBooking(object? parameter)
        {
            if (SelectedGuest is null || SelectedBooking is null)
                return;

            Bookings.Remove(SelectedBooking);
            SelectedBooking = null;
        }

        private void AddBooking(object? parameter)
        {
            if (SelectedGuest is null)
                return;

            BookingDetails bookingDetails = new BookingDetails("New Booking", SelectedGuest);
            // Dim main window before showing the modal window, then restore it back
            MainWindow.Opacity = 0.4;
            bookingDetails.ShowDialog();
            MainWindow.Opacity = 1.0;
        }

    }
}
