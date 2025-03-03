using System.Collections.ObjectModel;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Services.Data
{
    public abstract class BaseDataService
    {
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
