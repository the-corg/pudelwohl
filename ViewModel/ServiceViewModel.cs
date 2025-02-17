using System.Collections;
using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServiceViewModel : ViewModelBase
    {
        private readonly Service _model;

        public ServiceViewModel(Service model)
        {
            _model = model;
        }

        public int Id => _model.Id;

        public string? Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _model.Description;
            set
            {
                _model.Description = value;
                OnPropertyChanged();
            }
        }
        public string? Duration
        {
            get
            {
                if (_model.DurationMinutes is null)
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
                if (value is null)
                {
                    intValue = 1; // ERROR
                }
                else
                {
                    var split = value.Split(' ');
                    if (split.Length != 2 && split.Length != 4)
                    {
                        intValue = 1; // ERROR
                    }
                    else
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
                _model.DurationMinutes = intValue;
                OnPropertyChanged();
            }
        }

        public string? StartTime
        {
            get => _model.StartTime;
            set
            {
                if (value is null)
                {
                    ArrayList arrayList = Application.Current.FindResource("startTimes") as ArrayList;
                    _model.StartTime = arrayList[0] as string;
                }
                else
                {
                    _model.StartTime = value;
                }
                OnPropertyChanged();
            }
        }
        public string? EndTime
        {
            get => _model.EndTime;
            set
            {
                if (value is null)
                {
                    ArrayList arrayList = Application.Current.FindResource("endTimes") as ArrayList;
                    _model.EndTime = arrayList[arrayList.Count - 1] as string;
                }
                else
                {
                    _model.EndTime = value;
                }
                OnPropertyChanged();
            }
        }
    }
}
