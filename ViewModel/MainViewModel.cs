using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMealDataService _mealDataService;

        public MainViewModel(IGuestDataService guestDataService, IRoomDataService roomDataService,
            IServiceDataService serviceDataService, IMealDataService mealDataService, GuestsViewModel guestsViewModel,
            RoomsViewModel roomsViewModel, ServicesViewModel servicesViewModel,  MealOptionsViewModel mealOptionsViewModel)
        {
            _guestDataService = guestDataService;
            _roomDataService = roomDataService;
            _serviceDataService = serviceDataService;
            _mealDataService = mealDataService;
            GuestsViewModel = guestsViewModel;
            RoomsViewModel = roomsViewModel;
            ServicesViewModel = servicesViewModel;
            MealOptionsViewModel = mealOptionsViewModel;
            _roomDataService.FreeRoomsUpdated = () => OnPropertyChanged(nameof(FreeRoomsToday));
        }

        public GuestsViewModel GuestsViewModel { get; }
        public RoomsViewModel RoomsViewModel { get; }
        public ServicesViewModel ServicesViewModel { get; }
        public MealOptionsViewModel MealOptionsViewModel { get; }

        // These services are sent to IValueConverters
        public IGuestDataService GuestDataService => _guestDataService;
        public IRoomDataService RoomDataService => _roomDataService;
        public IServiceDataService ServiceDataService => _serviceDataService;

        // Property to show the number of free rooms (and occupied rooms) in the main window's status bar
        public string FreeRoomsToday => _roomDataService.FreeRoomsToday;

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _guestDataService.LoadAsync(),
                _roomDataService.LoadAsync(),
                _serviceDataService.LoadAsync(),
                _mealDataService.LoadAsync()
                );
        }

        public async Task SaveDataAsync()
        {
            await Task.WhenAll(
                _guestDataService.SaveDataAsync(),
                _roomDataService.SaveDataAsync(),
                _serviceDataService.SaveDataAsync(),
                _mealDataService.SaveDataAsync()
                );
        }
    }
}
