using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestsViewModel : ViewModelBase
    {
        private GuestViewModel? _selectedGuest;
        private Booking? _selectedBooking;
        private ServiceBooking? _selectedServiceBooking;
        private GuestMenu? _currentGuestMenu;
        private string _archiveButtonText = "Archive";
        private string _viewArchiveButtonText = "View Archive";
        private DateOnly _selectedMenuDate;
        private string?[] _breakfastOptions = new string?[3];
        private string?[] _lunchOptions = new string?[3];
        private string?[] _snackOptions = new string?[3];
        private string?[] _dinnerOptions = new string?[3];
        private string? _selectedBreakfastOption;
        private string? _selectedLunchOption;
        private string? _selectedSnackOption;
        private string? _selectedDinnerOption;
        private readonly IGuestDataService _guestDataService;
        private readonly IBookingDialogService _bookingDialogService;
        private readonly IServiceBookingDialogService _serviceBookingDialogService;
        private readonly IMealDataService _mealDataService;
        private readonly IMessageService _messageService;

        public GuestsViewModel(IGuestDataService guestDataService, IRoomDataService roomDataService, 
            IServiceDataService serviceDataService, IBookingDialogService bookingDialogService,
            IServiceBookingDialogService serviceBookingDialogService, IMealDataService mealDataService,
            IMessageService messageService)
        {
            _guestDataService = guestDataService;
            _bookingDialogService = bookingDialogService;
            _serviceBookingDialogService = serviceBookingDialogService;
            _mealDataService = mealDataService;
            _messageService = messageService;
            Guests = guestDataService.Guests;
            Bookings = roomDataService.Bookings;
            ServiceBookings = serviceDataService.ServiceBookings;
            GuestMenus = mealDataService.GuestMenus;

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
            AddServiceBookingCommand = new DelegateCommand(execute => AddServiceBooking());
            RemoveServiceBookingCommand = new DelegateCommand(execute => RemoveServiceBooking(), canExecute => CanRemoveServiceBooking());
            _selectedMenuDate = DateOnly.FromDateTime(DateTime.Now);
            
            BookingsCollectionView = roomDataService.BookingsForGuest;
            // Filter bookings based on the selected guest
            BookingsCollectionView.Filter =
                booking => (SelectedGuest is not null) && (((Booking)booking).GuestId == SelectedGuest.Id);
            // And sort them by check-in date
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));

            ServiceBookingsCollectionView = serviceDataService.ServiceBookingsForGuest;
            // Filter service bookings based on the selected guest
            ServiceBookingsCollectionView.Filter =
                serviceBooking => (SelectedGuest is not null) && (((ServiceBooking)serviceBooking).GuestId == SelectedGuest.Id);
            // And sort it by date, then by start time
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
            ServiceBookingsCollectionView.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));

            mealDataService.GuestMenuUpdated = UpdateGuestMenuOptions;
        }

        public ListCollectionView BookingsCollectionView { get; }
        public ListCollectionView ServiceBookingsCollectionView {  get; }
        public ObservableCollection<GuestViewModel> Guests { get; }
        public ObservableCollection<Booking> Bookings { get; }
        public ObservableCollection<ServiceBooking> ServiceBookings { get; }
        public Dictionary<(DateOnly, int), GuestMenu> GuestMenus { get; }

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
                UpdateGuestMenuOptions();
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
                RemoveServiceBookingCommand.OnCanExecuteChanged();
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
                UpdateGuestMenuOptions();
            }
        }

        public string?[] BreakfastOptions => _breakfastOptions;
        public string?[] LunchOptions => _lunchOptions;
        public string?[] SnackOptions => _snackOptions;
        public string?[] DinnerOptions => _dinnerOptions;

        public string? SelectedBreakfastOption
        {
            get => _selectedBreakfastOption;
            set
            {
                if (_selectedBreakfastOption == value) 
                    return;

                _selectedBreakfastOption = value;
                OnPropertyChanged();

                if (_currentGuestMenu is null)
                    return;
                _currentGuestMenu.Breakfast = value is null ? 0 : int.Parse(value.Split("#").Last()[..^1]);
            }
        }

        public string? SelectedLunchOption
        {
            get => _selectedLunchOption;
            set
            {
                if (_selectedLunchOption == value)
                    return;

                _selectedLunchOption = value;
                OnPropertyChanged();

                if (_currentGuestMenu is null)
                    return;
                _currentGuestMenu.Lunch = value is null ? 0 : int.Parse(value.Split("#").Last()[..^1]);
            }
        }

        public string? SelectedSnackOption
        {
            get => _selectedSnackOption;
            set
            {
                if (_selectedSnackOption == value)
                    return;

                _selectedSnackOption = value;
                OnPropertyChanged();

                if (_currentGuestMenu is null)
                    return;
                _currentGuestMenu.Snack = value is null ? 0 : int.Parse(value.Split("#").Last()[..^1]);
            }
        }

        public string? SelectedDinnerOption
        {
            get => _selectedDinnerOption;
            set
            {
                if (_selectedDinnerOption == value)
                    return;

                _selectedDinnerOption = value;
                OnPropertyChanged();

                if (_currentGuestMenu is null)
                    return;
                _currentGuestMenu.Dinner = value is null ? 0 : int.Parse(value.Split("#").Last()[..^1]);
            }
        }

        public bool IsArchiveHidden
        {
            get => _guestDataService.IsArchiveHidden;
            set
            {
                if (_guestDataService.IsArchiveHidden == value)
                    return;

                _guestDataService.IsArchiveHidden = value;
                if (_guestDataService.IsArchiveHidden)
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
        public DelegateCommand AddServiceBookingCommand { get; }
        public DelegateCommand RemoveServiceBookingCommand { get; }

        private void Add()
        {
            var guest = new Guest { Name = "NEW GUEST" };
            var viewModel = new GuestViewModel(guest, _guestDataService);
            Guests.Add(viewModel);
            SelectedGuest = viewModel;
            IsArchiveHidden = true;
        }

        private bool CanRemove() => SelectedGuest is not null;
        private void Remove()
        {
            if (SelectedGuest is null)
                return;

            // Count all bookings and service bookings for the selected guest
            // and ask the user to confirm deletion
            var bookings = Bookings.Where(x => x.GuestId == SelectedGuest.Id).ToList();
            var serviceBookings = ServiceBookings.Where(x => x.GuestId == SelectedGuest.Id).ToList();
            int numberOfBookings = bookings.Count;
            int numberOfServiceBookings = serviceBookings.Count;

            if (numberOfBookings > 0 || numberOfServiceBookings > 0)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                int numberOfFutureBookings = bookings.Where(x => x.CheckOutDate >= today).Count();
                int numberOfFutureServiceBookings = serviceBookings.Where(x => x.Date >= today).Count();

                // Cook up another localization engineer's nightmare
                string message = $"This guest has {numberOfBookings} booking" + (numberOfBookings != 1 ? "s" : "") +
                    $" and {numberOfServiceBookings} service booking" + (numberOfServiceBookings != 1 ? "s" : "") +
                    $", including {numberOfFutureBookings} active booking" + (numberOfFutureBookings != 1 ? "s" : "") +
                    $" and {numberOfFutureServiceBookings} active service booking" + (numberOfFutureServiceBookings != 1 ? "s" : "") +
                    $".\n\nAre you sure you want to delete the guest and cancel all their bookings?";
                if (!SelectedGuest.IsArchived)
                    message += $"\n\n(Another option would be to click \"No\" and archive the guest instead.)";

                if (!_messageService.ShowConfirmation(message))
                    return;

                // Remove all bookings and service bookings for the selected guest
                foreach (var booking in bookings)
                    Bookings.Remove(booking);
                foreach (var serviceBooking in serviceBookings)
                    ServiceBookings.Remove(serviceBooking);
            }

            // GuestMenus are not important, delete them without asking the user
            foreach (var key in GuestMenus.Keys.ToList())
            {
                if (key.Item2 == SelectedGuest.Id)
                {
                    GuestMenus.Remove(key);
                }
            }

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

            _bookingDialogService.ShowBookingDialog("Edit Booking", false, true, SelectedGuest.Id, -1, SelectedBooking);
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

            _bookingDialogService.ShowBookingDialog("New Booking", false, true, SelectedGuest.Id, -1);
        }

        private bool CanRemoveServiceBooking() => SelectedGuest is not null && SelectedServiceBooking is not null;
        private void RemoveServiceBooking()
        {
            if (SelectedGuest is null || SelectedServiceBooking is null)
                return;

            ServiceBookings.Remove(SelectedServiceBooking);
            SelectedServiceBooking = null;
        }

        private void AddServiceBooking()
        {
            if (SelectedGuest is null)
                return;

            _serviceBookingDialogService.ShowServiceBookingDialog(false, true, true, SelectedGuest.Id, -1, null);
        }

        public void UpdateGuestMenuOptions()
        {
            if (SelectedGuest is null)
                return;
            _breakfastOptions = new string?[3];
            _lunchOptions = new string?[3];
            _snackOptions = new string?[3];
            _dinnerOptions = new string?[3];
            if (_mealDataService.DailyMenus.TryGetValue(_selectedMenuDate, out DailyMenu? dailyMenu))
            {
                for (int i = 0; i < 12; i++)
                {
                    string? mealName = dailyMenu.Menu[i] == 0 ? null :
                        _mealDataService.GetMealOptionById(dailyMenu.Menu[i])?.NameWithId;
                    switch (i)
                    {
                        case 0: case 1: case 2:
                            _breakfastOptions[i] = mealName;
                            break;
                        case 3: case 4: case 5:
                            _lunchOptions[i - 3] = mealName;
                            break;
                        case 6: case 7: case 8:
                            _snackOptions[i - 6] = mealName;
                            break;
                        case 9: case 10: case 11:
                            _dinnerOptions[i - 9] = mealName;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!GuestMenus.TryGetValue((_selectedMenuDate, SelectedGuest.Id), out _currentGuestMenu))
            {
                _currentGuestMenu = new GuestMenu()
                {
                    Date = _selectedMenuDate,
                    GuestId = SelectedGuest.Id
                };
                GuestMenus.Add((_selectedMenuDate, SelectedGuest.Id), _currentGuestMenu);
            }
            _selectedBreakfastOption = _mealDataService.GetMealOptionById(_currentGuestMenu.Breakfast)?.NameWithId;
            _selectedLunchOption = _mealDataService.GetMealOptionById(_currentGuestMenu.Lunch)?.NameWithId;
            _selectedSnackOption = _mealDataService.GetMealOptionById(_currentGuestMenu.Snack)?.NameWithId;
            _selectedDinnerOption = _mealDataService.GetMealOptionById(_currentGuestMenu.Dinner)?.NameWithId;
            OnPropertyChanged(nameof(BreakfastOptions));
            OnPropertyChanged(nameof(LunchOptions));
            OnPropertyChanged(nameof(SnackOptions));
            OnPropertyChanged(nameof(DinnerOptions));
            OnPropertyChanged(nameof(SelectedBreakfastOption));
            OnPropertyChanged(nameof(SelectedLunchOption));
            OnPropertyChanged(nameof(SelectedSnackOption));
            OnPropertyChanged(nameof(SelectedDinnerOption));
        }
    }
}
