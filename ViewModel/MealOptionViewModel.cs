using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for a single meal option
    /// </summary>
    public class MealOptionViewModel : ViewModelBase
    {
        #region Private field and the constructor 

        private readonly MealOption _model;

        public MealOptionViewModel(MealOption model)
        {
            _model = model;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Id of the meal option
        /// </summary>
        public int Id => _model.Id;

        /// <summary>
        /// Name of the meal option
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
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Name of the meal option with its meal types appended as one-letter tags (B/L/S/D)
        /// </summary>
        public string? DisplayName
        {
            get
            {
                string result = "[";
                result += IsBreakfast ? "Ｂ" : "－";
                result += IsLunch ? "Ｌ" : "－";
                result += IsSnack ? "Ｓ" : "－";
                result += IsDinner ? "Ｄ" : "－";
                result += "] " + _model.Name;
                return result;
            }
        }

        /// <summary>
        /// Name of the meal option together with its Id
        /// </summary>
        public string? NameWithId => ToString();

        /// <summary>
        /// Shows whether the meal option can be selected for breakfast
        /// </summary>
        public bool IsBreakfast
        {
            get => _model.IsBreakfast;
            set
            {
                if (_model.IsBreakfast == value)
                    return;

                _model.IsBreakfast = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Shows whether the meal option can be selected for lunch
        /// </summary>
        public bool IsLunch
        {
            get => _model.IsLunch;
            set
            {
                if (_model.IsLunch == value)
                    return;

                _model.IsLunch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Shows whether the meal option can be selected for snack
        /// </summary>
        public bool IsSnack
        {
            get => _model.IsSnack;
            set
            {
                if (_model.IsSnack == value)
                    return;

                _model.IsSnack = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        /// <summary>
        /// Shows whether the meal option can be selected for dinner
        /// </summary>
        public bool IsDinner
        {
            get => _model.IsDinner;
            set
            {
                if (_model.IsDinner == value)
                    return;

                _model.IsDinner = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Getter for the underlying MealOption object (model)
        /// </summary>
        /// <returns>The underlying MealOption object (model)</returns>
        public MealOption GetMealOption()
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
