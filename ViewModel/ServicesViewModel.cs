using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServicesViewModel : ViewModelBase
    {
        private ServiceViewModel? _selectedService;
        private readonly MainViewModel _mainViewModel;

        public ServicesViewModel(ObservableCollection<ServiceViewModel> services, MainViewModel mainViewModel)
        {
            Services = services;
            AddCommand = new DelegateCommand(execute => Add());
            RemoveCommand = new DelegateCommand(execute => Remove(), canExecute => CanRemove());
            _mainViewModel = mainViewModel;
        }

        public ObservableCollection<ServiceViewModel> Services { get; }

        public ServiceViewModel? SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService == value)
                    return;

                _selectedService = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsServiceSelected));
                RemoveCommand.OnCanExecuteChanged();
            }
        }

        // Used for hiding the service details when no service is selected
        public bool IsServiceSelected => SelectedService is not null;

        public DelegateCommand AddCommand { get; }

        public DelegateCommand RemoveCommand { get; }

        private void Add()
        {
            var service = new Service { Name = "NEW SERVICE" };
            var viewModel = new ServiceViewModel(service, _mainViewModel);
            Services.Add(viewModel);
            SelectedService = viewModel;
        }

        private bool CanRemove() => SelectedService is not null;
        private void Remove()
        {
            if (SelectedService is null)
                return;

            Services.Remove(SelectedService);
            SelectedService = null;
        }

    }
}
