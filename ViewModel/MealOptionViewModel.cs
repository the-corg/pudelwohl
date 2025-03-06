using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionViewModel : ViewModelBase
    {
        private readonly MealOption _model;

        public MealOptionViewModel(MealOption model)
        {
            _model = model;
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
                OnPropertyChanged(nameof(DisplayName));
            }
        }

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

        public string? NameWithId => ToString();

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

        public override string ToString()
        {
            return _model.Name + " (#" + _model.Id + ")";
        }

    }
}
