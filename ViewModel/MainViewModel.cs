using System.Collections.ObjectModel;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGuestDataService _guestDataService;
        private readonly IRoomDataService _roomDataService;
        private readonly IServiceDataService _serviceDataService;

        public MainViewModel(IGuestDataService guestDataService, IRoomDataService roomDataService,
            IServiceDataService serviceDataService, GuestsViewModel guestsViewModel,
            RoomsViewModel roomsViewModel, ServicesViewModel servicesViewModel) //TODO  MealOptionsViewModel mealOptionsViewModel)
        {
            _guestDataService = guestDataService;
            _roomDataService = roomDataService;
            _serviceDataService = serviceDataService;
            GuestsViewModel = guestsViewModel;
            RoomsViewModel = roomsViewModel;
            ServicesViewModel = servicesViewModel;
            MealOptionsViewModel = new MealOptionsViewModel(this);
        }

        public GuestsViewModel GuestsViewModel { get; }
        public RoomsViewModel RoomsViewModel { get; }
        public ServicesViewModel ServicesViewModel { get; }
        public MealOptionsViewModel MealOptionsViewModel { get; }

        public IRoomDataService RoomDataService => _roomDataService;
        public IServiceDataService ServiceDataService => _serviceDataService;


        //TODO: remove from here
        public ObservableCollection<MealOptionViewModel> MealOptions { get; } = new();


        // Property to show the number of free rooms (and occupied rooms) in the main window's status bar
        public string FreeRoomsToday => _roomDataService.FreeRoomsToday;

        //TODO
        //public void BookingsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{
            /*OnPropertyChanged(nameof(FreeRoomsToday));

            // Update room occupancy colors on the Rooms tab
            foreach (var room in Rooms)
                room.BookingsChanged();*/
        //}

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _guestDataService.LoadAsync(),
                _roomDataService.LoadAsync(),
                _serviceDataService.LoadAsync()
                );
        }

    }
}
