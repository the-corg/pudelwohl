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
        private DateOnly _selectedMenuDate;
        private readonly MainViewModel _mainViewModel;

        // ICollectionView objects for filtering and sorting of the corresponding collections
        private ICollectionView BookingsCollectionView { get; set; }
        private ICollectionView ServiceBookingsCollectionView { get; set; }

        private Window? _mainWindow;
        // Lazy initialization of the main window reference
        // (needed because it's null at the time when the constructor is called)
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

            // Add() and others are parameterless but ICommand wants methods with one parameter
            // One approach: add a dummy parameter. Leads to awkward manual calls: Archive(null)
            // Better approach: pass a lambda that takes a parameter and simply calls the method.
            AddCommand = new DelegateCommand(execute => Add());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
            ArchiveCommand = new DelegateCommand(execute => Archive(), canExecute => CanArchive());
            ViewArchiveCommand = new DelegateCommand(execute => ViewArchive());
            AddBookingCommand = new DelegateCommand(execute => AddBooking());
            EditBookingCommand = new DelegateCommand(execute => EditBooking(), canExecute => CanEditBooking());
            RemoveBookingCommand = new DelegateCommand(execute => RemoveBooking(), canExecute => CanRemoveBooking());
            _selectedMenuDate = DateOnly.FromDateTime(DateTime.Now);

            BookingsCollectionView = CollectionViewSource.GetDefaultView(Bookings);
            // Filter the bookings list according to the selected guest
            BookingsCollectionView.Filter = 
                o => (SelectedGuest is not null) && (((Booking)o).GuestId == SelectedGuest.Id);
            // And sort it by check-in date
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));

            ServiceBookingsCollectionView = CollectionViewSource.GetDefaultView(ServiceBookings);
            // Filter the service bookings list according to the selected guest
            ServiceBookingsCollectionView.Filter = 
                o => (SelectedGuest is not null) && (((ServiceBooking)o).GuestId == SelectedGuest.Id);
            // And sort it by date, then by start time
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));
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
                if (_selectedGuest == value)
                    return;

                _selectedGuest = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGuestSelected));
                RemoveCommand.OnCanExecuteChanged();
                ArchiveCommand.OnCanExecuteChanged();
                BookingsCollectionView.Refresh();
                ServiceBookingsCollectionView.Refresh();
            }
        }

        public Booking? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                if (_selectedBooking == value)
                    return;

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
                if (_selectedServiceBooking == value)
                    return;

                _selectedServiceBooking = value;
                OnPropertyChanged();
                // TODO!
                //RemoveServiceBookingCommand.OnCanExecuteChanged();
                //EditServiceBookingCommand.OnCanExecuteChanged();
            }
        }

        // Used for hiding the guest details when no guest is selected
        public bool IsGuestSelected => SelectedGuest is not null;


        public DateOnly SelectedMenuDate
        {
            get => _selectedMenuDate;
            set
            {
                if (_selectedMenuDate == value)
                    return;

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
                if (value is null || SelectedGuest is null)
                    return;
                // TODO: Add check for value == old value
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
                if (_isArchiveHidden == value)
                    return;

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
                if (_archiveButtonText == value)
                    return;

                _archiveButtonText = value;
                OnPropertyChanged();
            }
        }

        public string ViewArchiveButtonText
        {
            get => _viewArchiveButtonText;
            set
            {
                if (_viewArchiveButtonText == value)
                    return;

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

        private void Add()
        {
            var guest = new Guest { Name = "NEW GUEST" };
            var viewModel = new GuestViewModel(guest, _mainViewModel);
            Guests.Add(viewModel);
            SelectedGuest = viewModel;
            IsArchiveHidden = true;
            // TODO: ID
        }

        private bool CanRemove() => SelectedGuest is not null;
        private void Remove()
        {
            if (SelectedGuest is null)
                return;

            // Find all bookings and service bookings for the selected guest
            // and ask the user to confirm deletion

            // If user selects "Archive instead", archive the guest
            // Archive();

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

        private bool CanArchive() => SelectedGuest is not null;
        private void Archive()
        {
            if (SelectedGuest is null)
                return;

            SelectedGuest.IsArchived = !SelectedGuest.IsArchived;
            SelectedGuest = null;
        }

        private void ViewArchive()
        {
            IsArchiveHidden = !IsArchiveHidden;
            SelectedGuest = null;
        }

        private bool CanEditBooking() => SelectedGuest is not null && SelectedBooking is not null;
        private void EditBooking()
        {
            if (SelectedGuest is null || SelectedBooking is null)
                return;

            BookingDetails bookingDetails = new BookingDetails(_mainViewModel, "Edit Booking", SelectedGuest.Id, SelectedBooking);
            // Dim main window before showing the modal window, then restore it back
            MainWindow.Opacity = 0.4;
            bookingDetails.ShowDialog();
            MainWindow.Opacity = 1.0;
        }

        private bool CanRemoveBooking() => SelectedGuest is not null && SelectedBooking is not null;
        private void RemoveBooking()
        {
            if (SelectedGuest is null || SelectedBooking is null)
                return;

            Bookings.Remove(SelectedBooking);
            SelectedBooking = null;
        }

        private void AddBooking()
        {
            if (SelectedGuest is null)
                return;

            BookingDetails bookingDetails = new BookingDetails(_mainViewModel, "New Booking", SelectedGuest.Id);
            // Dim main window before showing the modal window, then restore it back
            MainWindow.Opacity = 0.4;
            bookingDetails.ShowDialog();
            MainWindow.Opacity = 1.0;
        }

    }
}
