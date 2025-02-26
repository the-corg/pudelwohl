using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestViewModel : ViewModelBase
    {
        private readonly Guest _model;
        private readonly MainViewModel _mainViewModel;

        public GuestViewModel(Guest model, MainViewModel mainViewModel)
        {
            _model = model;
            _mainViewModel = mainViewModel;
        }

        public int Id => _model.Id;

        public string? Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                OnPropertyChanged();
                // TODO! RoomsViewModel.BookingsChanged();
            }
        }

        public string? Breed
        {
            get => _model.Breed;
            set
            {
                _model.Breed = value;
                OnPropertyChanged();
                // TODO! RoomsViewModel.BookingsChanged();
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
                // Replace _ from the Enum with spaces for the ComboBox
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
                    // Replace space from the ComboBox item with _ to find the required Enum value
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

        // ListView binds to this to show only archived/not archived guests
        public bool IsVisible => _mainViewModel.GuestsViewModel.IsArchiveHidden ? !IsArchived : IsArchived;

    }
}
