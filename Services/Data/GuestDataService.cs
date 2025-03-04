using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IGuestDataService
    {
        ObservableCollection<GuestViewModel> Guests { get; }
        bool IsArchiveHidden { get; set; }
        IRoomDataService RoomDataService { get; }
        IServiceDataService ServiceDataService { get; }
        Task LoadAsync();
    }

    public class GuestDataService : BaseDataService, IGuestDataService
    {
        private readonly IGuestDataProvider _guestDataProvider;


        public GuestDataService(IGuestDataProvider guestDataProvider, IRoomDataService roomDataService, IServiceDataService serviceDataService)
        {
            _guestDataProvider = guestDataProvider;
            RoomDataService = roomDataService;
            ServiceDataService = serviceDataService;
        }

        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public bool IsArchiveHidden { get; set; } = true;
        public IRoomDataService RoomDataService { get; }
        public IServiceDataService ServiceDataService { get; }

        public async Task LoadAsync()
        {
            var guests = await _guestDataProvider.GetAllAsync();
            LoadCollection(Guests, guests, guest => new GuestViewModel(guest, this));
        }
    }
}
