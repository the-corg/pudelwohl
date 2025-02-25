using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServicesViewModel : ViewModelBase
    {
        private ServiceViewModel? _selectedService;

        public ServicesViewModel(ObservableCollection<ServiceViewModel> services)
        {
            Services = services;
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
        }

        public ObservableCollection<ServiceViewModel> Services { get; }

        public ServiceViewModel? SelectedService
        {
            get => _selectedService;
            set
            {
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

        private void Add(object? parameter)
        {
            var service = new Service { Name = "NEW SERVICE" };
            var viewModel = new ServiceViewModel(service);
            Services.Add(viewModel);
            SelectedService = viewModel;
        }

        private void Remove(object? parameter)
        {
            if (SelectedService is not null)
            {
                Services.Remove(SelectedService);
                SelectedService = null;
            }
        }

        private bool CanRemove(object? parameter) => SelectedService is not null;

    }
}
