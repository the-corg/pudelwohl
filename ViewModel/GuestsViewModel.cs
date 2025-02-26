using System.Collections.ObjectModel;
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

        private Window? _mainWindow;
        // Lazy loading the main window reference (because null when the constructor is called)
        private Window? MainWindow => _mainWindow ??= Window.GetWindow(App.Current.MainWindow) as MainWindow;

        public GuestsViewModel(
            ObservableCollection<GuestViewModel> guests,
            ObservableCollection<Booking> bookings,
            ObservableCollection<ServiceBooking> serviceBookings,
            ObservableCollection<GuestMenu> guestMenus)
        {
            Guests = guests;
            Bookings = bookings;
            ServiceBookings = serviceBookings;
            GuestMenus = guestMenus;
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
            ArchiveCommand = new DelegateCommand(Archive, CanArchive);
            ViewArchiveCommand = new DelegateCommand(ViewArchive);
            AddBookingCommand = new DelegateCommand(AddBooking);
            EditBookingCommand = new DelegateCommand(EditBooking, CanEditBooking);
            RemoveBookingCommand = new DelegateCommand(RemoveBooking, CanRemoveBooking);
            _selectedMenuDate = DateTime.Today;
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

        // Helper method to show bookings sorted by the check-in date
        public static ObservableCollection<Booking> SortBookings(ObservableCollection<Booking> observableCollection)
        {
            ObservableCollection<Booking> temp;
            temp = new ObservableCollection<Booking>(observableCollection.OrderBy(x => x.CheckInDate));
            observableCollection.Clear();
            foreach (var booking in temp)
                observableCollection.Add(booking);
            return observableCollection;
        }

        // Helper method to show service bookings sorted by the date, then by the start time
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

        private void Add(object? parameter)
        {
            // TODO!
            /*
            var guest = new Guest { Name = "NEW GUEST" };
            var viewModel = new GuestViewModel(guest, this);
            Guests.Add(viewModel);
            SelectedGuest = viewModel;
            IsArchiveHidden = true;*/
        }

        private bool CanRemove(object? parameter) => SelectedGuest is not null;
        private void Remove(object? parameter)
        {
            if (SelectedGuest is not null)
            {
                // TODO: Remove all bookings, services, meal options for the selected guest
                Guests.Remove(SelectedGuest);
                SelectedGuest = null;
            }
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
