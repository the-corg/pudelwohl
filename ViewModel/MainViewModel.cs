using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    /// <summary>
    /// View model for the main window
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Private fields

        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;
        private readonly IServiceDataService _serviceDataService;
        private readonly IMealDataService _mealDataService;
        #endregion


        #region Constructor

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
        #endregion


        #region Public properties

        /// <summary>
        /// View model for the Guests tab
        /// </summary>
        public GuestsViewModel GuestsViewModel { get; }

        /// <summary>
        /// View model for the Rooms tab
        /// </summary>
        public RoomsViewModel RoomsViewModel { get; }

        /// <summary>
        /// View model for the Services tab
        /// </summary>
        public ServicesViewModel ServicesViewModel { get; }

        /// <summary>
        /// View model for the Cuisine tab
        /// </summary>
        public MealOptionsViewModel MealOptionsViewModel { get; }

        // These services are sent to IValueConverters
        public IGuestDataService GuestDataService => _guestDataService;
        public IRoomDataService RoomDataService => _roomDataService;
        public IServiceDataService ServiceDataService => _serviceDataService;

        /// <summary>
        /// String representation of the number of free rooms 
        /// with a list of occupied rooms for the main window's status bar
        /// </summary>
        public string FreeRoomsToday => _roomDataService.FreeRoomsToday;

        #endregion


        #region Public methods

        /// <summary>
        /// Initializes all the data asynchronously
        /// </summary>
        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _guestDataService.LoadAsync(),
                _roomDataService.LoadAsync(),
                _serviceDataService.LoadAsync(),
                _mealDataService.LoadAsync()
                );
        }

        /// <summary>
        /// Saves all the data asynchronously
        /// </summary>
        public async Task SaveDataAsync()
        {
            await Task.WhenAll(
                _guestDataService.SaveDataAsync(),
                _roomDataService.SaveDataAsync(),
                _serviceDataService.SaveDataAsync(),
                _mealDataService.SaveDataAsync()
                );
        }
        #endregion

    }
}
