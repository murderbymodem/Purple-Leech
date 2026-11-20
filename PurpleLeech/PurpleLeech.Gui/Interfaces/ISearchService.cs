using System.Collections.ObjectModel;
using System.ComponentModel;
using PurpleLeech.Core.Models;

namespace PurpleLeech.Gui.Interfaces
{
    public interface ISearchService : INotifyPropertyChanged
    {
        ObservableCollection<Vod> Videos { get; }

        SearchParameters LastSearchParams { get; }

        void PerformSearch(SearchParameters searchParams);
    }
}

