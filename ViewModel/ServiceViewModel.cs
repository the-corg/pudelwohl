using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for a single service
    /// </summary>
    public class ServiceViewModel : ViewModelBase
    {
        #region Private fields

        private readonly Service _model;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMessageService _messageService;
        #endregion


        #region Constructor

        public ServiceViewModel(Service model, IServiceDataService serviceDataService)
        {
            _model = model;
            _serviceDataService = serviceDataService;
            _messageService = serviceDataService.MessageService;
            CalculateTimeSlots();
        }
        #endregion


        #region Public properties

        /// <summary>
        /// ID of the service
        /// </summary>
        public int Id => _model.Id;

        /// <summary>
        /// Name of the service
        /// </summary>
        public string? Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name == value)
                    return;

                _model.Name = value!;
                OnPropertyChanged();
                _serviceDataService.DebouncedSave();
                _serviceDataService.ServiceBookingsForGuest.Refresh();
            }
        }

        /// <summary>
        /// Description for the service
        /// </summary>
        public string? Description
        {
            get => _model.Description;
            set
            {
                if (_model.Description == value)
                    return;

                _model.Description = value;
                OnPropertyChanged();
                _serviceDataService.DebouncedSave();
            }
        }

        /// <summary>
        /// Duration of the service as a formatted string (e.g., "1 hour 30 minutes")
        /// </summary>
        public string? Duration
        {
            get
            {
                if (_model.DurationMinutes < 1)
                    return null;

                int hours = (int)_model.DurationMinutes / 60;
                int minutes = (int)_model.DurationMinutes % 60;
                string result = "";

                if (hours > 0)
                {
                    result += hours.ToString();
                    result += " hour";
                    if (hours > 1)
                    {
                        result += "s";
                    }
                    if (minutes > 0)
                    {
                        result += " ";
                    }
                }

                if (minutes > 0)
                {
                    result += minutes.ToString();
                    result += " minutes";
                }

                return result;
            }
            set
            {
                int intValue = 0;
                if (value is not null)
                {
                    var split = value.Split(' ');
                    if (split.Length == 2 || split.Length == 4)
                    {
                        intValue = int.Parse(split[0]);
                        if (split[1].StartsWith("hour"))
                        {
                            intValue *= 60;
                            if (split.Length == 4)
                            {
                                intValue += int.Parse(split[2]);
                            }
                        }
                    }
                }
                if (_model.DurationMinutes == intValue)
                    return;

                // If the new duration is longer, it might cause overlapping time slots for this service or for a guest 
                if (_model.DurationMinutes < intValue)
                {
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    // Find all service bookings that are not in the past
                    var allActive = _serviceDataService.ServiceBookings.Where(x => x.Date >= today).ToList();
                    if (allActive.Count > 1) // If there's only one, it can't overlap with anything
                    {
                        // Check if new duration causes overlap of existing active bookings for the same service
                        var sBookingsForThisService = allActive.Where(x => x.ServiceId == Id).ToList();
                        if (sBookingsForThisService.Count > 1)
                        {
                            for (int i = 0; i < sBookingsForThisService.Count - 1; i++) // Not checking the last one, it has no pair
                            {
                                var startTime1 = TimeOnly.Parse(sBookingsForThisService[i].StartTime!);
                                var endTime1 = startTime1.AddMinutes(intValue);
                                for (int j = i + 1; j < sBookingsForThisService.Count; j++)
                                {
                                    if (sBookingsForThisService[i].Date != sBookingsForThisService[j].Date)
                                        continue;

                                    // Found two active service bookings for the same service on the same date, checking overlap
                                    var startTime2 = TimeOnly.Parse(sBookingsForThisService[j].StartTime!);
                                    var endTime2 = startTime2.AddMinutes(intValue);
                                    if (startTime1 < endTime2 && startTime2 < endTime1)
                                    {
                                        _messageService.ShowMessage($"Sorry, the new longer duration would cause an overlap between at least two existing active service bookings for this service.");
                                        return;
                                    }
                                }
                            }
                        }

                        // Check if new duration causes overlap of existing active bookings for the same guest
                        for (int i = 0; i < allActive.Count - 1; i++) // Not checking the last one, it has no pair
                        {
                            for (int j = i + 1; j < allActive.Count; j++)
                            {
                                if (allActive[j].GuestId != allActive[i].GuestId || allActive[j].Date != allActive[i].Date)
                                    continue;
                                if (allActive[j].ServiceId != Id && allActive[i].ServiceId != Id)
                                    continue; // the service being changed is not one of the pair

                                // Found two active service bookings for the same guest on the same date,
                                // at least one of which is for the service being changed. Checking overlap
                                var startTime1 = TimeOnly.Parse(allActive[i].StartTime!);
                                var endTime1 = startTime1.AddMinutes(allActive[i].ServiceId == Id ? intValue :
                                    _serviceDataService.Services.First(x => x.Id == allActive[i].ServiceId).DurationMinutes);

                                var startTime2 = TimeOnly.Parse(allActive[j].StartTime!);
                                var endTime2 = startTime2.AddMinutes(allActive[j].ServiceId == Id ? intValue :
                                    _serviceDataService.Services.First(x => x.Id == allActive[j].ServiceId).DurationMinutes);

                                if (startTime1 < endTime2 && startTime2 < endTime1)
                                {
                                    _messageService.ShowMessage($"Sorry, the new longer duration would cause an overlap between at least two existing active service bookings for a client.");
                                    return;
                                }
                            }
                        }
                    }
                }

                _model.DurationMinutes = intValue;
                OnPropertyChanged();
                _serviceDataService.DebouncedSave();
                CalculateTimeSlots();
            }
        }

        /// <summary>
        /// Duration of the service in minutes
        /// </summary>
        public int DurationMinutes => _model.DurationMinutes;

        /// <summary>
        /// The starting time of the service
        /// </summary>
        public string? StartTime
        {
            get => _model.StartTime;
            set
            {
                if (_model.StartTime == value)
                    return;

                // If the change causes existing service bookings (except those in the past) to fall out of range, prohibit it
                var newTime = TimeOnly.Parse(value!);
                var today = DateOnly.FromDateTime(DateTime.Now);
                foreach (var serviceBooking in _serviceDataService.ServiceBookings)
                {
                    if (serviceBooking.ServiceId == Id &&
                        serviceBooking.Date >= today &&
                        TimeOnly.Parse(serviceBooking.StartTime!) < newTime)
                    {
                        _messageService.ShowMessage($"Sorry, the new start time (" + value! +
                            ") would cause the start time of an active service booking (" + serviceBooking.StartTime! +
                            ") to fall out of range.");
                        return;
                    }
                }

                _model.StartTime = value;
                OnPropertyChanged();
                _serviceDataService.DebouncedSave();
                CalculateTimeSlots();
            }
        }

        /// <summary>
        /// The ending time of the service
        /// </summary>
        public string? EndTime
        {
            get => _model.EndTime;
            set
            {
                if (_model.EndTime == value)
                    return;

                // If the change causes existing service bookings (except those in the past) to fall out of range, prohibit it
                var newTime = TimeOnly.Parse(value!);
                var today = DateOnly.FromDateTime(DateTime.Now);
                foreach (var serviceBooking in _serviceDataService.ServiceBookings)
                {
                    var sBookingEndTime = TimeOnly.Parse(serviceBooking.StartTime!).AddMinutes(DurationMinutes);
                    if (serviceBooking.ServiceId == Id && serviceBooking.Date >= today && sBookingEndTime > newTime)
                    {
                        _messageService.ShowMessage($"Sorry, the new end time (" + value! +
                            ") would cause the end time of an active service booking (" +
                            sBookingEndTime.ToString("HH:mm") + ") to fall out of range.");
                        return;
                    }
                }

                _model.EndTime = value;
                OnPropertyChanged();
                _serviceDataService.DebouncedSave();
                CalculateTimeSlots();
            }
        }

        /// <summary>
        /// The collection of the time slots for this particular service
        /// (differs based on the services duration and start/end times)
        /// </summary>
        public ObservableCollection<TimeSlot> TimeSlots { get; } = new();

        #endregion


        #region Private helper method

        /// <summary>
        /// Calculates the time slots for the service based on its start time, end time, and duration
        /// </summary>
        private void CalculateTimeSlots()
        {
            TimeSlots.Clear();

            if (_model.DurationMinutes < 1)
            {
                // Impossible to create time slots
                return;
            }

            if (StartTime is null)
            {
                // Using the earliest start time as the default one
                var arrayList = (ArrayList)Application.Current.FindResource("startTimes");
                StartTime = arrayList[0] as string;
            }

            if (EndTime is null)
            {
                // Using the latest end time as the default one
                var arrayList = (ArrayList)Application.Current.FindResource("endTimes");
                EndTime = arrayList[arrayList.Count - 1] as string;
            }

            TimeOnly start = TimeOnly.Parse(StartTime!);
            TimeOnly end = TimeOnly.Parse(EndTime!);

            while (start < end)
            {
                var tmpEnd = start.AddMinutes(_model.DurationMinutes);
                if (tmpEnd > end)
                    break;
                var newTimeSlot = new TimeSlot() { StartTime = start.ToString("HH:mm"), EndTime = tmpEnd.ToString("HH:mm") };
                TimeSlots.Add(newTimeSlot);
                start = tmpEnd;
            }

        }
        #endregion


        #region Public methods

        /// <summary>
        /// Getter for the underlying Service object (model)
        /// </summary>
        /// <returns>The underlying Service object (model)</returns>
        public Service GetService()
        {
            return _model;
        }


        public override string ToString()
        {
            return _model.Name + " (#" + _model.Id + ")";
        }
        #endregion

    }
}
