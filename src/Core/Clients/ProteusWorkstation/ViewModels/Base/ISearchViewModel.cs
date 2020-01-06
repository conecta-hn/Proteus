/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.ComponentModel;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.ViewModels.Base
{
    public interface ISearchViewModel
    {
        ICollectionView Results { get; }
        string ResultsDetails { get; }
        ObservingCommand SearchCommand { get; }
        string SearchLabel { get; }
        string SearchQuery { get; set; }
        bool WillSearch { get; }
    }
}