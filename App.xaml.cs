using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.DataProviders;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.ViewModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private fields and the constructor

        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        #endregion

        #region Configure services for dependency injection

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            // View Models
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<GuestsViewModel>();
            services.AddSingleton<RoomsViewModel>();
            services.AddSingleton<ServicesViewModel>();
            services.AddSingleton<MealOptionsViewModel>();

            // Data Providers
            services.AddTransient<IGuestDataProvider, GuestDataProvider>();
            services.AddTransient<IRoomDataProvider, RoomDataProvider>();
            services.AddTransient<IServiceDataProvider, ServiceDataProvider>();
            services.AddTransient<IMealOptionDataProvider, MealOptionDataProvider>();
            services.AddTransient<IBookingDataProvider, BookingDataProvider>();
            services.AddTransient<IServiceBookingDataProvider, ServiceBookingDataProvider>();
            services.AddTransient<IGuestMenuDataProvider, GuestMenuDataProvider>();
            services.AddTransient<IDailyMenuDataProvider, DailyMenuDataProvider>();

            // Data Services
            services.AddSingleton<IGuestDataService, GuestDataService>();
            services.AddSingleton<IRoomDataService, RoomDataService>();
            services.AddSingleton<IServiceDataService, ServiceDataService>();
            services.AddSingleton<IMealDataService, MealDataService>();

            // Other Services
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<IBookingDialogService, BookingDialogService>();
            services.AddSingleton<IServiceBookingDialogService, ServiceBookingDialogService>();
            services.AddSingleton<IMealOptionDialogService, MealOptionDialogService>();
        }
        #endregion

        #region OnStartup

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
        #endregion
    }
}
