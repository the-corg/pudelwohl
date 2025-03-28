using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class GuestViewModel : ViewModelBase
    {
        private readonly Guest _model;
        private readonly IGuestDataService _guestDataService;

        public GuestViewModel(Guest model, IGuestDataService guestDataService)
        {
            _model = model;
            _guestDataService = guestDataService;
        }

        public int Id => _model.Id;

        public string? Name
        {
            get => _model.Name;
            set
            {
                if (value == _model.Name)
                    return;

                _model.Name = value!;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
                _guestDataService.UpdateOnGuestDataChange();
            }
        }

        public string? Breed
        {
            get => _model.Breed;
            set
            {
                if (value == _model.Breed)
                    return;

                _model.Breed = value;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
                _guestDataService.UpdateOnGuestDataChange();
            }
        }
        public bool IsGenderMaleButtonChecked
        {
            get => _model.Gender == Gender.Male;
            set
            {
                if (!value || _model.Gender == Gender.Male)
                    return;

                _model.Gender = Gender.Male;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }

        public bool IsGenderFemaleButtonChecked
        {
            get => _model.Gender == Gender.Female;
            set
            {
                if (!value || _model.Gender == Gender.Female)
                    return;

                _model.Gender = Gender.Female;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }

        public bool IsGenderOtherButtonChecked
        {
            get => _model.Gender == Gender.Other;
            set
            {
                if (!value || _model.Gender == Gender.Other)
                    return;

                _model.Gender = Gender.Other;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }

        public string? CoatColor
        {
            get => _model.CoatColor;
            set
            {
                if (_model.CoatColor == value)
                    return;

                _model.CoatColor = value;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }
        public DateOnly? DateOfBirth
        {
            get => _model.DateOfBirth;
            set
            {
                if (_model.DateOfBirth == value)
                    return;

                _model.DateOfBirth = value;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }
        public string? FavoriteToy
        {
            get => _model.FavoriteToy;
            set
            {
                if (_model.FavoriteToy == value)
                    return;

                _model.FavoriteToy = value;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
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
                    _guestDataService.DebouncedSave();
                }
                else
                {
                    // Replace space from the ComboBox item with _ to find the required Enum value
                    var newValue = value.Replace(" ", "_");
                    if (_model.EarFloppiness.ToString() != newValue)
                    {
                        _model.EarFloppiness = Enum.Parse<EarFloppiness>(newValue);
                        OnPropertyChanged();
                        _guestDataService.DebouncedSave();
                    }
                }
            }
        }
        public string? SpecialRequests
        {
            get => _model.SpecialRequests;
            set
            {
                if (_model.SpecialRequests == value)
                    return;

                _model.SpecialRequests = value;
                OnPropertyChanged();
                _guestDataService.DebouncedSave();
            }
        }

        public bool IsArchived
        {
            get => _model.IsArchived;
            set
            {
                if (_model.IsArchived == value)
                    return;

                _model.IsArchived = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        // ListView binds to this to show only archived/not archived guests
        public bool IsVisible => _guestDataService.IsArchiveHidden ? !IsArchived : IsArchived;

        public Guest GetGuest()
        {
            return _model;
        }

        public override string ToString()
        {
            return _model.Name + " (#" + _model.Id + ")";
        }

    }
}
