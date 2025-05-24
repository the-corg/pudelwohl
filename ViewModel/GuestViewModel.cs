using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for a single guest
    /// </summary>
    public class GuestViewModel : ViewModelBase
    {
        #region Private fields and the constructor

        private readonly Guest _model;
        private readonly IGuestDataService _guestDataService;

        public GuestViewModel(Guest model, IGuestDataService guestDataService)
        {
            _model = model;
            _guestDataService = guestDataService;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// ID of the guest
        /// </summary>
        public int Id => _model.Id;

        /// <summary>
        /// Name of the guest
        /// </summary>
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

        /// <summary>
        /// Breed of the guest (note that the guest is a dog)
        /// </summary>
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

        /// <summary>
        /// Shows whether the Male gender radio button is currently active
        /// </summary>
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

        /// <summary>
        /// Shows whether the Female gender radio button is currently active
        /// </summary>
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

        /// <summary>
        /// Shows whether the Other gender radio button is currently active
        /// </summary>
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

        /// <summary>
        /// Coat color of the guest
        /// </summary>
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

        /// <summary>
        /// Date of birth of the guest
        /// </summary>
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

        /// <summary>
        /// Favorite toy of the guest
        /// </summary>
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

        /// <summary>
        /// String representation of the ear floppiness of the guest
        /// (converted from the EarFloppiness enum)
        /// </summary>
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

        /// <summary>
        /// Special requests of the guest
        /// </summary>
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

        /// <summary>
        /// Shows whether the guest is currently archived
        /// </summary>
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

        /// <summary>
        /// Shows whether the guest should be currently visible
        /// (the list of guests uses this to show only archived/not archived guests)
        /// </summary>
        public bool IsVisible => _guestDataService.IsArchiveHidden ? !IsArchived : IsArchived;

        #endregion


        #region Public methods

        /// <summary>
        /// Getter for the underlying Guest object (model)
        /// </summary>
        /// <returns>The underlying Guest object (model)</returns>
        public Guest GetGuest()
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
