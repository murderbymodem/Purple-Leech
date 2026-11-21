using System.Collections.ObjectModel;
using System.ComponentModel;
using PurplePlunder.Core.Models;

namespace PurplePlunder.Gui.Interfaces
{
    public interface ISearchService : INotifyPropertyChanged
    {
        ObservableCollection<Vod> Videos { get; }

        SearchParameters LastSearchParams { get; }

        void PerformSearch(SearchParameters searchParams);
    }
}

