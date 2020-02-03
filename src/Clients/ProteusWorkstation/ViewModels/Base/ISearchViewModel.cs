/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.ComponentModel;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.ViewModels.Base
{
    public interface ISearchViewModel
    {
        ICollectionView? Results { get; }
        IEnumerable<ModelBase>? EnumerableResults { get; }
        string ResultsDetails { get; }
        ObservingCommand SearchCommand { get; }
        string SearchLabel { get; }
        string? SearchQuery { get; set; }
        bool WillSearch { get; }

        void ClearSearch();
    }
}