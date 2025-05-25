using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    /// <summary>
    /// Base class for data services.
    /// Provides methods for data saving, 
    /// including both simple async save and debounced save.
    /// Also provides a method for loading data into an
    /// ObservableCollection of item view models.
    /// </summary>
    public abstract class BaseDataService
    {
        #region Private fields

        // Time delay in ms before data is saved (Debounced save)
        private readonly int _debounceTime = 3000;

        private readonly SemaphoreSlim _saveLock = new(1, 1);
        private CancellationTokenSource _debounceCts = new();

        #endregion


        #region Save-related methods

        protected abstract Task SaveCollectionsAsync();

        /// <summary>
        /// Saves all the data managed by the data service asynchronously
        /// (as soon as possible)
        /// </summary>
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

        /// <summary>
        /// Saves all the data managed by the data service asynchronously,
        /// but only if no new save calls arrive within <c>_debounceTime</c>.
        /// </summary>
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
        #endregion


        #region Load method

        /// <summary>
        /// Loads elements from <paramref name="data"/> into <paramref name="collection"/> using the wrapping function <paramref name="wrap"/>, when needed
        /// </summary>
        /// <typeparam name="TModel">The original model class of the data</typeparam>
        /// <typeparam name="TViewModel">Either the same as <typeparamref name="TModel"/>, or the item view model class for the data</typeparam>
        /// <param name="collection">The observable collection, to which the data should be loaded</param>
        /// <param name="data">The original collection of data</param>
        /// <param name="wrap">The optional wrapping function, transforming each item of the data from <typeparamref name="TModel"/> into <typeparamref name="TViewModel"/></param>
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
        #endregion

    }
}
