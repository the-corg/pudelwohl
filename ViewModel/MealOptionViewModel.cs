using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MealOptionViewModel : ViewModelBase
    {
        private readonly MealOption _model;

        public MealOptionViewModel(MealOption model, MainViewModel mainViewModel)
        {
            _model = model;
            _ = mainViewModel;
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

        public bool IsBreakfast
        { 
            get => _model.IsBreakfast;
            set
            {
                _model.IsBreakfast = value;
                OnPropertyChanged();
            }
        }
        public bool IsLunch
        {
            get => _model.IsLunch;
            set
            {
                _model.IsLunch = value;
                OnPropertyChanged();
            }
        }
        public bool IsSnack
        {
            get => _model.IsSnack;
            set
            {
                _model.IsSnack = value;
                OnPropertyChanged();
            }
        }
        public bool IsDinner
        {
            get => _model.IsDinner;
            set
            {
                _model.IsDinner = value;
                OnPropertyChanged();
            }
        }

    }
}
