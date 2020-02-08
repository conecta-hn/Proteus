/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para QuickCrudPage.xaml
    /// </summary>
    public partial class QuickCrudPage
    {
        private QuickCrudPage()
        {
            InitializeComponent();
        }

        public static QuickCrudPage New<T>() where T : ModelBase, new()
        {
            return New(typeof(T));
        }
        public static QuickCrudPage New(Type model)
        {
            return New(model, null);
        }

        public static QuickCrudPage New<T>(ModelBase? parent) where T : ModelBase, new()
        {
            return New(typeof(T), parent);
        }

        public static QuickCrudPage New(Type model, ModelBase? parent)
        {
            var p = new QuickCrudPage();
            p.ViewModel = new QuickCrudViewModel(p, model, parent, QuickCrudViewModel.CrudMode.Close);
            return p;
        }

        public static QuickCrudPage BulkNew(Type model)
        {
            var p = new QuickCrudPage();
            p.ViewModel = new QuickCrudViewModel(p, model, null, QuickCrudViewModel.CrudMode.New);
            return p;
        }
        public static QuickCrudPage BulkNew<T>()
        {
            return BulkNew(typeof(T));
        }

        public static IPage Edit<T>(T entity) where T : ModelBase
        {
            var p = new QuickCrudPage();
            p.ViewModel = new QuickCrudViewModel(p, entity, null, QuickCrudViewModel.CrudMode.Close);
            return p;
        }
    }
}
