using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class BookingDetailsViewModel : ViewModelBase
    {
        private readonly string _headerText;
        private DateOnly? _checkInDate;
        private DateOnly? _checkOutDate;
        private readonly int _initialGuestId;
        private readonly int _initialRoomId;
        private string? _guestName;
        private string? _roomName;
        private Booking? _booking;
        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;
        private readonly IMessageService _messageService;

        public BookingDetailsViewModel(IGuestDataService guestDataService, IRoomDataService roomDataService, 
            IMessageService messageService, string headerText, bool isGuestSelectable, bool isRoomSelectable, 
            int fixedGuestId, int fixedRoomId, Booking? booking = null)
        {
            _guestDataService = guestDataService;
            _roomDataService = roomDataService;
            _messageService = messageService;
            _headerText = headerText;
            IsGuestSelectable = isGuestSelectable;
            IsRoomSelectable = isRoomSelectable;
            _booking = booking;

            if (_booking is null)
            {
                // This will be a new booking
                _checkInDate = DateOnly.FromDateTime(DateTime.Now);
                _checkOutDate = _checkInDate?.AddDays(1);
                _initialGuestId = fixedGuestId;
                _initialRoomId = fixedRoomId;
            }
            else
            {
                // Edit an old booking
                _checkInDate = _booking.CheckInDate;
                _checkOutDate = _booking.CheckOutDate;
                _initialGuestId = _booking.GuestId;
                _initialRoomId = _booking.RoomId;
            }

            InitializeNames();
            ConfirmCommand = new DelegateCommand(execute => Confirm(), canExecute => CanConfirm());
        }

        public Action? CloseOnConfirmAction { get; set; } // Delegate for closing the window

        public string HeaderText => _headerText;

        public DateOnly? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (value == _checkInDate)
                    return;

                _checkInDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public DateOnly? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (value == _checkOutDate)
                    return;

                _checkOutDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsGuestSelectable { get; }
        public string? GuestName
        {
            get => _guestName;
            set
            {
                if (value == _guestName)
                    return;
                _guestName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public bool IsRoomSelectable { get; }
        public string? RoomName
        {
            get => _roomName;
            set
            {
                if (value == _roomName)
                    return;
                _roomName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonDisabledReason));
                ConfirmCommand.OnCanExecuteChanged();
            }
        }

        public List<string> GuestNames { get; } = new();
        public List<string> RoomNames { get; } = new();

        public DelegateCommand ConfirmCommand { get; }

        public string ButtonDisabledReason
        {
            get
            {
                string result = "";

                if (CheckOutDate is null)
                    result += "Please select a date for check-out.\n\n";
                else if (CheckOutDate < DateOnly.FromDateTime(DateTime.Now))
                    result += "Please select a check-out date that is not in the past.\n\n";

                if (CheckInDate is null)
                    result += "Please select a date for check-in.\n\n";
                else if (CheckOutDate is not null && CheckInDate > CheckOutDate)
                    result += "Please select such dates that check-out doesn't have to happen before check-in.\n\n";

                if (GuestName is null)
                    result += "Please select a guest.\n\n";
                if (RoomName is null)
                    result += "Please select a room.\n\n";

                if (result.Length > 0)
                    result = result[..^2];
                
                return result;
            }
        }

        // Make the Confirm button inactive if one of the dates or the room are missing,
        // or the check-out date is earlier than today, or the check-out date is earlier than the check-in date
        private bool CanConfirm() => !(CheckOutDate is null || CheckInDate is null || GuestName is null || RoomName is null ||
            CheckOutDate < DateOnly.FromDateTime(DateTime.Now) || CheckInDate > CheckOutDate);
        private void Confirm()
        {
            // Race condition check - UI is not guaranteed to check CanConfirm immediately before Confirm
            if (!CanConfirm())
            {
                ConfirmCommand.OnCanExecuteChanged();
                return;
            }

            // If guest was selecteble, parse GuestName (which includes the id - see InitializeNames())
            int guestId = IsGuestSelectable ? int.Parse(GuestName!.Split("#").Last()[..^1]) : _initialGuestId;

            // If room was selectable, parse RoomName (room names always start with RoomId)
            int roomId = IsRoomSelectable ? int.Parse(RoomName!.Split()[0]) : _initialRoomId;

            if (_booking is not null)
            {
                // This is an Edit. Check if nothing was edited
                if (_booking.RoomId == roomId && _booking.GuestId == guestId &&
                    _booking.CheckInDate == _checkInDate && _booking.CheckOutDate == _checkOutDate)
                {
                    // Nothing was edited, nothing to do
                    CloseOnConfirmAction?.Invoke();
                    return;
                }
            }

            // Check if the guest has a booking for this time period already
            foreach (var booking in _roomDataService.Bookings)
            {
                // Ignore this exact booking as well as bookings for other guests
                if (booking == _booking || booking.GuestId != guestId)
                    continue;

                // Check if the date intervals overlap
                if (booking.CheckInDate <= _checkOutDate && booking.CheckOutDate >= _checkInDate)
                {
                    _messageService.ShowMessage("This guest has an overlapping booking!");
                    return;
                }
            }

            // Check if the room has a booking for this time period already

            // This would crash intentionally if no room is found.
            // Rooms can't be deleted, so it would mean something is seriously wrong
            var room = _roomDataService.Rooms.First(x => x.Id == roomId);
            // The dates can't be null - already checked at the start
            var maxOccupants = room.MaxOccupantsWithinDates((DateOnly)_checkInDate!, (DateOnly)_checkOutDate!, _booking);
            if (maxOccupants >= room.MaxGuests)
            {
                _messageService.ShowMessage("This room is full at least on some of the dates.\nThe booking cannot be created.");
                return;
            }
            else if (maxOccupants > 0)
            {
                // The room is neither free nor full, ask the user
                if (!_messageService.ShowConfirmation("Someone else has an overlapping booking for this room.\n\nAdd the booking anyway?"))
                    return;
            }

            // All checks done. Add or edit the booking now
            if (_booking is not null)
            {
                // To easily refresh all derived collection views
                _roomDataService.Bookings.Remove(_booking);
            }
            _booking ??= new Booking();
            
            _booking.GuestId = guestId;
            _booking.RoomId = roomId;
            _booking.CheckInDate = (DateOnly)_checkInDate;
            _booking.CheckOutDate = (DateOnly)_checkOutDate;

            _roomDataService.Bookings.Add(_booking);

            // Close the dialog
            CloseOnConfirmAction?.Invoke();
        }

        private void InitializeNames()
        {
            GuestNames.AddRange(_guestDataService.Guests.Select(x => x.ToString()));

            RoomNames.AddRange(_roomDataService.Rooms.Select(x => x.Name!));

            if (_initialGuestId != -1)
            {
                _guestName = _guestDataService.Guests.FirstOrDefault(x => x.Id == _initialGuestId)?.ToString();
            }

            if (_initialRoomId != -1)
            {
                _roomName = _roomDataService.Rooms.FirstOrDefault(x => x.Id == _initialRoomId)?.Name;
            }
        }
    }
}
