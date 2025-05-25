using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    /// <summary>
    /// Manages all data related to guests
    /// </summary>
    public interface IGuestDataService
    {
        /// <summary>
        /// The collection of guests
        /// </summary>
        ObservableCollection<GuestViewModel> Guests { get; }

        /// <summary>
        /// Shows whether the archive is currently hidden
        /// </summary>
        bool IsArchiveHidden { get; set; }

        /// <summary>
        /// Updates all the data that need to be updated after a guest's information has been changed
        /// </summary>
        void UpdateOnGuestDataChange();

        /// <summary>
        /// Loads all the data managed by the data service from the corresponding data providers
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously
        /// (as soon as possible)
        /// </summary>
        Task SaveDataAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously,
        /// but only if no new save calls arrive within <c>_debounceTime</c>.
        /// </summary>
        void DebouncedSave();
    }

    public class GuestDataService : BaseDataService, IGuestDataService
    {
        #region Private fields and the constructor

        private readonly IGuestDataProvider _guestDataProvider;
        private readonly ListCollectionView _bookingsForRoom;
        private readonly ListCollectionView _serviceBookingsForService;

        public GuestDataService(IGuestDataProvider guestDataProvider, IRoomDataService roomDataService, IServiceDataService serviceDataService)
        {
            _guestDataProvider = guestDataProvider;
            _bookingsForRoom = roomDataService.BookingsForRoom;
            _serviceBookingsForService = serviceDataService.ServiceBookingsForService;
        }
        #endregion


        #region Public properties (see interface)

        public ObservableCollection<GuestViewModel> Guests { get; } = new();
        public bool IsArchiveHidden { get; set; } = true;

        #endregion


        #region Public methods (see interface)

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
        #endregion


        #region Protected method used by the base class for saving the data

        protected override async Task SaveCollectionsAsync()
        {
            await _guestDataProvider.SaveAsync(Guests.Select(x => x.GetGuest()));
        }
        #endregion

    }
}
