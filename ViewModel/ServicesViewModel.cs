
using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.MVVM;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class ServicesViewModel : ViewModelBase
    {
        private readonly IServiceDataProvider _serviceDataProvider;
        private ServiceViewModel? _selectedService;

        public ServicesViewModel(IServiceDataProvider serviceDataProvider)
        {
            _serviceDataProvider = serviceDataProvider;
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove, CanRemove);
        }

        public static ObservableCollection<ServiceViewModel> Services { get; } = new();

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

        public bool IsServiceSelected => SelectedService is not null;

        public DelegateCommand AddCommand { get; }

        public DelegateCommand RemoveCommand { get; }

        public async Task LoadAsync()
        {
            if (Services.Count > 0) 
                return;

            var services = await _serviceDataProvider.GetAllAsync();
            if (services is not null)
            {
                foreach (var service in services)
                {
                    Services.Add(new ServiceViewModel(service));
                }
            }
        }

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
