using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IGuestDataService
    {
        ObservableCollection<GuestViewModel> Guests { get; }
        bool IsArchiveHidden { get; set; }
        void UpdateOnGuestDataChange();
        Task LoadAsync();
        Task SaveDataAsync();
        void DebouncedSave();
    }

    public class GuestDataService : BaseDataService, IGuestDataService
    {
        private readonly IGuestDataProvider _guestDataProvider;
        private readonly ListCollectionView _bookingsForRoom;
        private readonly ListCollectionView _serviceBookingsForService;

        public GuestDataService(IGuestDataProvider guestDataProvider, IRoomDataService roomDataService, IServiceDataService serviceDataService)
        {
            _guestDataProvider = guestDataProvider;
            _bookingsForRoom = roomDataService.BookingsForRoom;
            _serviceBookingsForService = serviceDataService.ServiceBookingsForService;
        }

        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public bool IsArchiveHidden { get; set; } = true;

        public void UpdateOnGuestDataChange()
        {
            _bookingsForRoom.Refresh();
            _serviceBookingsForService.Refresh();
        }

        public async Task LoadAsync()
        {
            var guests = await _guestDataProvider.LoadAsync();
            if (guests is null)
                return;
            IdGenerator.Instance.CalculateMaxId(guests);
            LoadCollection(Guests, guests, guest => new GuestViewModel(guest, this));
        }

        protected override async Task SaveCollectionsAsync()
        {
            await _guestDataProvider.SaveAsync(Guests.Select(x => x.GetGuest()));
        }

    }
}
