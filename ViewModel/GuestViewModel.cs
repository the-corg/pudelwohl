using System;
using System.Collections.ObjectModel;
using System.Windows;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestViewModel : ViewModelBase
    {
        private readonly Guest _model;
        private readonly GuestsViewModel _parentViewModel;

        public GuestViewModel(Guest model, GuestsViewModel parentViewModel)
        {
            _model = model;
            _parentViewModel = parentViewModel;
        }

        public int Id => _model.Id;

        public string? Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                OnPropertyChanged();
                RoomsViewModel.BookingsChanged();
            }
        }

        public string? Breed
        {
            get => _model.Breed;
            set
            {
                _model.Breed = value;
                OnPropertyChanged();
                RoomsViewModel.BookingsChanged();
            }
        }
        public bool IsGenderMaleButtonChecked
        {
            get => _model.Gender == Model.Gender.Male;
            set
            {
                if (value) _model.Gender = Model.Gender.Male;
                OnPropertyChanged();
            }
        }

        public bool IsGenderFemaleButtonChecked
        {
            get => _model.Gender == Model.Gender.Female;
            set
            {
                if (value) _model.Gender = Model.Gender.Female;
                OnPropertyChanged();
            }
        }

        public bool IsGenderOtherButtonChecked
        {
            get => _model.Gender == Model.Gender.Other;
            set
            {
                if (value) _model.Gender = Model.Gender.Other;
                OnPropertyChanged();
            }
        }

        public string? CoatColor
        {
            get => _model.CoatColor;
            set
            {
                _model.CoatColor = value;
                OnPropertyChanged();
            }
        }
        public DateTime? DateOfBirth
        {
            get => _model.DateOfBirth;
            set
            {
                _model.DateOfBirth = value;
                OnPropertyChanged();
            }
        }
        public string? FavoriteToy
        {
            get => _model.FavoriteToy;
            set
            {
                _model.FavoriteToy = value;
                OnPropertyChanged();
            }
        }
        public string? EarFloppiness
        {
            get
            {
                if (_model.EarFloppiness == Model.EarFloppiness.Unknown) 
                    return null;
                return _model.EarFloppiness.ToString().Replace('_', ' ');
            }
            set
            {
                if (value is null || value == "")
                {
                    _model.EarFloppiness = Model.EarFloppiness.Unknown;
                    OnPropertyChanged();
                }
                else
                {
                    var newValue = value.Replace(" ", "_");
                    if (_model.EarFloppiness.ToString() != newValue)
                    {
                        _model.EarFloppiness = Enum.Parse<EarFloppiness>(newValue);
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string? SpecialRequests
        {
            get => _model.SpecialRequests;
            set
            {
                _model.SpecialRequests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Booking> Bookings { get; } = new();

        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();

        public List<GuestMenu> GuestMenus { get; } = new();

        public bool IsArchived
        {
            get => _model.IsArchived;
            set
            {
                _model.IsArchived = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public bool IsVisible
        {
            get
            {
                return _parentViewModel.IsArchiveHidden ? !IsArchived : IsArchived;
            }
        }

    }
}
