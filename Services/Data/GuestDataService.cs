using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public interface IGuestDataService
    {
        ObservableCollection<GuestViewModel> Guests { get; }
        bool IsArchiveHidden { get; set; }
        Task LoadAsync();
    }

    public class GuestDataService : BaseDataService, IGuestDataService
    {
        private readonly IGuestDataProvider _guestDataProvider;

        public GuestDataService(IGuestDataProvider guestDataProvider)
        {
            _guestDataProvider = guestDataProvider;
        }

        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public bool IsArchiveHidden { get; set; } = true;

        public async Task LoadAsync()
        {
            var guests = await _guestDataProvider.GetAllAsync();
            LoadCollection(Guests, guests, guest => new GuestViewModel(guest, this));
        }
    }
}
