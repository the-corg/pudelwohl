using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestsViewModel : ViewModelBase
    {
        private GuestViewModel? _selectedGuest;
        private Booking? _selectedBooking;
        private ServiceBooking? _selectedServiceBooking;
        private string _archiveButtonText = "Archive";
        private string _viewArchiveButtonText = "View Archive";
        private DateOnly _selectedMenuDate;
        private readonly IGuestDataService _guestDataService;
        private readonly IBookingDialogService _bookingDialogService;
        private readonly IMessageService _messageService;

        // ICollectionView objects for filtering and sorting of the corresponding collections
        private ICollectionView BookingsCollectionView { get; set; }
        private ICollectionView ServiceBookingsCollectionView { get; set; }

        public GuestsViewModel(IGuestDataService guestDataService, IRoomDataService roomDataService, 
            IServiceDataService serviceDataService, IBookingDialogService bookingDialogService, IMessageService messageService)
        {
            _guestDataService = guestDataService;
            _bookingDialogService = bookingDialogService;
            _messageService = messageService;
            Guests = guestDataService.Guests;
            Bookings = roomDataService.Bookings;
            ServiceBookings = serviceDataService.ServiceBookings;
            //TODO
            //GuestMenus = mainViewModel.GuestMenus;

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
                booking => (SelectedGuest is not null) && (((Booking)booking).GuestId == SelectedGuest.Id);
            // And sort it by check-in date
            BookingsCollectionView.SortDescriptions.Add(new SortDescription("CheckInDate", ListSortDirection.Ascending));

            ServiceBookingsCollectionView = CollectionViewSource.GetDefaultView(ServiceBookings);
            // Filter the service bookings list according to the selected guest
            ServiceBookingsCollectionView.Filter =
                serviceBooking => (SelectedGuest is not null) && (((ServiceBooking)serviceBooking).GuestId == SelectedGuest.Id);
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
            
            //TODO
            /*for (int i = GuestMenus.Count - 1; i >= 0; i--)
                if (GuestMenus[i].GuestId == SelectedGuest.Id)
                    GuestMenus.RemoveAt(i);*/

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

    }
}
