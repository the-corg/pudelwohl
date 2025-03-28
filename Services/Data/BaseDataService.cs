using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public abstract class BaseDataService
    {
        // Time delay in ms before data is saved (Debounced save)
        private readonly int _debounceTime = 3000;

        private readonly SemaphoreSlim _saveLock = new(1, 1);
        private CancellationTokenSource _debounceCts = new();

        protected abstract Task SaveCollectionsAsync();

        public async Task SaveDataAsync()
        {
            // Prevent multiple saves from running at the same time
            await _saveLock.WaitAsync();
            try
            {
                // Cancel any pending debounced save
                _debounceCts.Cancel();

                // Each of the child classes overrides this method to save its collections
                await SaveCollectionsAsync();
            }
            finally
            {
                _saveLock.Release();
            }
        }

        // Save data if no new save calls arrive within _debounceTime
        public void DebouncedSave()
        {
            // Cancel any pending debounced save
            _debounceCts.Cancel();
            _debounceCts = new();
            var token = _debounceCts.Token;

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
