using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            // View Models
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<GuestsViewModel>();
            services.AddSingleton<RoomsViewModel>();
            services.AddSingleton<ServicesViewModel>();
            //services.AddSingleton<MealOptionsViewModel>();

            // Data Providers
            services.AddTransient<IGuestDataProvider, GuestDataProvider>();
            services.AddTransient<IRoomDataProvider, RoomDataProvider>();
            services.AddTransient<IServiceDataProvider, ServiceDataProvider>();
            services.AddTransient<IMealOptionDataProvider, MealOptionDataProvider>();
            services.AddTransient<IBookingDataProvider, BookingDataProvider>();
            services.AddTransient<IServiceBookingDataProvider, ServiceBookingDataProvider>();
            services.AddTransient<IGuestMenuDataProvider, GuestMenuDataProvider>();

            // Data Services
            services.AddSingleton<IGuestDataService, GuestDataService>();
            services.AddSingleton<IRoomDataService, RoomDataService>();
            services.AddSingleton<IServiceDataService, ServiceDataService>();
            //services.AddSingleton<IMenuDataService, MenuDataService>();

            // Other Services
            services.AddSingleton<IBookingDialogService, BookingDialogService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
