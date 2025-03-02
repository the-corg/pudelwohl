using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;
using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Data.DataServices
{
    public interface IServiceDataService
    {
        ObservableCollection<ServiceViewModel> Services { get; }
        ObservableCollection<ServiceBooking> ServiceBookings { get; }
        Task LoadAsync();
    }
    public class ServiceDataService : BaseDataService, IServiceDataService
    {
        private readonly IServiceDataProvider _serviceDataProvider;
        private readonly IServiceBookingDataProvider _serviceBookingDataProvider;

        public ServiceDataService(IServiceDataProvider serviceDataProvider, IServiceBookingDataProvider serviceBookingDataProvider)
        {
            _serviceDataProvider = serviceDataProvider;
            _serviceBookingDataProvider = serviceBookingDataProvider;
        }

        public ObservableCollection<ServiceViewModel> Services { get; } = new();
        public ObservableCollection<ServiceBooking> ServiceBookings { get; } = new();

        public async Task LoadAsync()
        {
            var services = await _serviceDataProvider.GetAllAsync();
            LoadCollection(Services, services, service => new ServiceViewModel(service, this));

            var serviceBookings = await _serviceBookingDataProvider.GetAllAsync();
            LoadCollection(ServiceBookings, serviceBookings);
        }
    }
}
