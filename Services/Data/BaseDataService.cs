using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public abstract class BaseDataService
    {
        // Time delay in ms before data is saved (Debounced save)
        private readonly int _debounceTime = 3000;

        protected abstract CancellationTokenSource DebounceCts { get; set; }

        public abstract Task SaveDataAsync();

        // Save data if no new save calls arrive within _debounceTime
        public void DebouncedSave()
        {
            // Cancel any pending debounced save
            DebounceCts.Cancel();
            DebounceCts = new();
            var token = DebounceCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    // Wait before saving
                    await Task.Delay(_debounceTime, token); 
                    if (!token.IsCancellationRequested)
                    {
                        // Actually save now because Task wasn't cancelled by a newer call
                        await SaveDataAsync();
                    }
                }
                catch (TaskCanceledException)
                {
                    // Do nothing because Task was canceled
                }
            });
        }

        // Load elements from data into the corresponding collection using the wrapping function, when needed
        protected static void LoadCollection<TModel, TViewModel>(ObservableCollection<TViewModel> collection,
            IEnumerable<TModel>? data, Func<TModel, TViewModel>? wrap = null)
            where TModel : class
            where TViewModel : class
        {
            if (collection.Count > 0 || data is null)
                return;

            foreach (var item in data)
            {
                collection.Add(wrap == null ? (item as TViewModel)! : wrap(item));
            }
        }
    }
}
