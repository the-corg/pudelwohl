using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServiceViewModel : ViewModelBase
    {
        private readonly Service _model;

        public ServiceViewModel(Service model, IServiceDataService serviceDataService)
        {
            _model = model;
            CalculateTimeSlots();
        }

        public int Id => _model.Id;

        public string? Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name == value)
                    return;

                _model.Name = value!;
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _model.Description;
            set
            {
                if (_model.Description == value)
                    return;

                _model.Description = value;
                OnPropertyChanged();
            }
        }
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

                _model.DurationMinutes = intValue;
                OnPropertyChanged();
                CalculateTimeSlots();
            }
        }

        public string? StartTime
        {
            get => _model.StartTime;
            set
            {
                if (_model.StartTime == value)
                    return;

                _model.StartTime = value;
                OnPropertyChanged();
                CalculateTimeSlots();
            }
        }
        public string? EndTime
        {
            get => _model.EndTime;
            set
            {
                if (_model.EndTime == value)
                    return;

                _model.EndTime = value;
                OnPropertyChanged();
                CalculateTimeSlots();
            }
        }

        public ObservableCollection<TimeSlot> TimeSlots { get; } = new();

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
    }
}
