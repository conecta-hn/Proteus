/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Pages.Base
{
    /// <summary>
    /// Lógica de interacción para CrudPage.xaml
    /// </summary>
    public partial class CrudPage
    {
        /// <summary>
        /// Crea una nueva página de CRUD utilizando el servicio y el
        /// modelo especificados.
        /// </summary>
        /// <typeparam name="TService">
        /// Tipo del servicio subyacente en el cual realizar las
        /// operaciones CRUD.
        /// </typeparam>
        /// <typeparam name="TModel">
        /// Tipo de modelo para el cual diseñar el CRUD.
        /// </typeparam>
        /// <returns>
        /// Una página de Proteus con funcionalidad CRUD para el modelo
        /// especificado.
        /// </returns>
        public static CrudPage New<TService, TModel>() where TService:Service,new() where TModel : ModelBase, new()
        {
            var p = new CrudPage();
            var vm = new CrudViewModel<TService>(p, typeof(TModel));
            p.ViewModel = vm;
            return p;
        }

        /// <summary>
        /// Crea una nueva página de CRUD para el modelo especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de modelo para el cual diseñar el CRUD.
        /// </typeparam>
        /// <returns>
        /// Una página de Proteus con funcionalidad CRUD para el modelo
        /// especificado.
        /// </returns>
        public static CrudPage New<T>() where T : ModelBase, new()
        {
            return New(typeof(T));
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        /// <param name="t">
        /// Modelo a administrar.
        /// </param>
        /// <returns>
        /// Una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </returns>
        public static CrudPage New(Type t)
        {
            var p = new CrudPage();
            p.ViewModel = typeof(CrudViewModel<>).MakeGenericType(Proteus.InferService(t)?.GetType()
                ?? throw new InvalidOperationException($"Ningún servicio hospeda entidades de tipo '{t.NameOf()}'"))
                .New<PageViewModel>(p, t);
            p.ViewModel.Title = $"Administrador de {CrudElement.GetDescription(t)?.FriendlyName ?? t.NameOf()}";
            return p;
        }

        public static CrudPage New(CrudElement t)
        {
            var p = new CrudPage();
            p.ViewModel = typeof(CrudViewModel<>).MakeGenericType(Proteus.InferService(t.Model)?.GetType()
                ?? throw new InvalidOperationException($"Ningún servicio hospeda entidades de tipo '{t.Model.NameOf()}'"))
                .New<PageViewModel>(p, t);
            p.ViewModel.Title = $"Administrador de {t.Description.FriendlyName ?? t.Model.NameOf()}";
            return p;
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio a utilizar para administrar la información.
        /// </typeparam>
        /// <param name="title">
        /// Título de la ventana.
        /// </param>
        /// <param name="source">
        /// Origen del árbol de datos.
        /// </param>
        /// <param name="models">
        /// Modelos de datos a utilizar.
        /// </param>
        /// <returns>
        /// Una nueva instanci ade la clase <see cref="CrudPage"/>.
        /// </returns>
        public static CrudPage New<T>(string title, IQueryable<ModelBase> source, IEnumerable<Type> models) where T : Service, new()
        {
            var p = new CrudPage();
            p.ViewModel = new CrudViewModel<T>(p, source, models.ToArray())
            {
                Title = title
            };
            return p;
        }

        public static CrudPage NewBase<T>(string title) where T : ModelBase
        {
            return NewBase(title, typeof(T));
        }

        public static CrudPage NewBase(string title, Type baseModel, params Type[] models)
        {
            if (!(baseModel ?? throw new ArgumentNullException(nameof(baseModel))).Implements<ModelBase>())
            {
                throw new Exception($"El tipo base {baseModel} no es un modelo.");
            }
            if (!models.Any()) throw new EmptyCollectionException(models);
            var s = Proteus.InferBaseService(baseModel);
            if (models.FirstOrDefault(p => !s.Hosts(p)) is { } m1)
            {
                throw new Exception($"El modelo {m1} no es válido para el servicio solicitado.");
            }
            if (models.FirstOrDefault(p => !p.Implements(baseModel)) is { } m2)
            { 
                throw new Exception($"El modelo {m2} no es válido para el tipo base {baseModel}");
            }

            var source = s.AllBase(baseModel);
            var p = new CrudPage();
            var vm = typeof(CrudViewModel<>).MakeGenericType(s.GetType()).New<IPageViewModel>(p, source, models.ToArray());
            vm.Title = title;
            p.ViewModel = vm;
            return p;
        }

        public static CrudPage NewBase(string title, Type baseModel)
        {
            return NewBase(title, baseModel, AppInternal.GetModels(baseModel));
        }

        public static CrudPage New(string title, params Type[] models)
        {
            if (!models.Any()) throw new EmptyCollectionException(models);
            var baseModel = models.First().BaseType!;
            return NewBase(title, baseModel, models);
        }

        public static CrudPage New<T, TModel>(string title, Func<IQueryable<TModel>, IQueryable<TModel>> filters) where T : Service, new() where TModel : ModelBase, new()
        {
            var q = Proteus.Service<T>()!.All<TModel>();
            if (filters != null)
            {
                q = filters(q);
            }
            return New<T>(title, q, new[] { typeof(TModel) });
        }

        public static CrudPage FromDescription<T>() where T:ICrudDescription, new()
        {
            return New(new CrudElement(new T()));

        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        protected CrudPage()
        {
            InitializeComponent();
            //PreviewKeyUp += CrudPage_PreviewKeyUp;
        }

        private void CrudPage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Alt) return;
            var sel = ((ICrudViewModel)ViewModel).Selection;
            
            switch (e.Key)
            {
                case Key.B:
                    TxtSearch.Focus();
                    break;

                case Key.N:
                    var cc = ((ICrudViewModel)ViewModel).CreateNew;
                    if (cc.CanExecute(null)) cc.Execute(null);
                    break;

                case Key.E:
                    var ec = ((ICrudViewModel)ViewModel).EditCurrent;
                    if (ec.CanExecute(sel)) ec.Execute(sel);
                    break;

                case Key.D:
                    var dc = ((ICrudViewModel)ViewModel).DeleteCurrent;
                    if (dc.CanExecute(sel)) dc.Execute(sel);
                    break;

                default:
                    return;
            }
            e.Handled = true;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            var vm = (ISearchViewModel)ViewModel;

            switch (e.Key)
            {
                case Key.Enter:
                    vm.SearchCommand.TryExecute();
                    ((TextBox)sender).SelectAll();
                    break;
                case Key.Escape:
                    vm.ClearSearch();
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }
    }
}