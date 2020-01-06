/*
Copyright © 2017-2019 César Andrés Morgan
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

namespace TheXDS.Proteus.Pages.Base
{
    /// <summary>
    /// Lógica de interacción para CrudPage.xaml
    /// </summary>
    public partial class CrudPage
    {
        /// <summary>
        ///     Crea una nueva página de CRUD utilizando el servicio y el
        ///     modelo especificados.
        /// </summary>
        /// <typeparam name="TService">
        ///     Tipo del servicio subyacente en el cual realizar las
        ///     operaciones CRUD.
        /// </typeparam>
        /// <typeparam name="TModel">
        ///     Tipo de modelo para el cual diseñar el CRUD.
        /// </typeparam>
        /// <returns>
        ///     Una página de Proteus con funcionalidad CRUD para el modelo
        ///     especificado.
        /// </returns>
        public static CrudPage New<TService,TModel>() where TService:Service,new() where TModel : ModelBase, new()
        {
            var p = new CrudPage();
            var vm = new CrudViewModel<TService>(p, typeof(TModel));
            p.ViewModel = vm;
            return p;
        }

        public static CrudPage New<TService>(CrudElement crud) where TService : Service, new()
        {
            var p = new CrudPage();
            var vm = new CrudViewModel<TService>(p, crud);
            p.ViewModel = vm;
            return p;
        }

        /// <summary>
        ///     Crea una nueva página de CRUD para el modelo especificado.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de modelo para el cual diseñar el CRUD.
        /// </typeparam>
        /// <returns>
        ///     Una página de Proteus con funcionalidad CRUD para el modelo
        ///     especificado.
        /// </returns>
        public static CrudPage New<T>() where T : ModelBase, new()
        {
            return New(typeof(T));
        }

        /// <summary>
        ///     Crea una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        /// <param name="t">
        ///     Modelo a administrar.
        /// </param>
        /// <returns>
        ///     Una nueva instancia de la clase <see cref="CrudPage"/>.
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
        ///     Crea una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de servicio a utilizar para administrar la información.
        /// </typeparam>
        /// <param name="title">
        ///     Título de la ventana.
        /// </param>
        /// <param name="source">
        ///     Origen del árbol de datos.
        /// </param>
        /// <param name="models">
        ///     Modelos de datos a utilizar.
        /// </param>
        /// <returns>
        ///     Una nueva instanci ade la clase <see cref="CrudPage"/>.
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

        public static CrudPage New<T>(string title, IQueryable<ModelBase> source, IEnumerable<CrudElement> cruds) where T : Service, new()
        {
            var p = new CrudPage();
            p.ViewModel = new CrudViewModel<T>(p, source, cruds.ToArray())
            {
                Title = title
            };
            return p;
        }

        public static CrudPage FromDescription<T>() where T:ICrudDescription, new()
        {
            return New(new CrudElement(new T()));

        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase <see cref="CrudPage"/>.
        /// </summary>
        protected CrudPage()
        {
            InitializeComponent();
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
            {
                e.Handled = true;
                var vm = ((ISearchViewModel)ViewModel).SearchCommand;
                if (vm.CanExecute(null)) vm.Execute(null);
            }
        }
    }
}