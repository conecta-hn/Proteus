/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;
using TheXDS.MCART;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Herramienta que crea un botón de acceso para generar nuevas entidades
    /// en la ventana de Crud.
    /// </summary>
    public class NewEntityCrudTool : CrudTool
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="NewEntityCrudTool"/>.
        /// </summary>
        public NewEntityCrudTool() : base(CrudToolVisibility.Unselected)
        {
        }

        /// <summary>
        /// Obtiene la colección de <see cref="Launcher"/> a presentar en la
        /// ventana de Crud.
        /// </summary>
        /// <param name="models">Modelos de la ventana de Crud.</param>
        /// <param name="vm">
        /// Instancia del <see cref="ICrudViewModel"/> que gestiona el
        /// comportamiento de la ventana de Crud.
        /// </param>
        /// <returns>
        /// Una enumeración de <see cref="Launcher"/> a presentar en las
        /// distintas vistas de la ventana de Crud.
        /// </returns>
        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel vm)
        {
            foreach (var j in models)
            {
                var d = CrudElement.GetDescription(j)?.FriendlyName ?? j.Name;
                var oc = GetMethod<CrudViewModelBase, Action<object>>(p => p.OnCreate);
                yield return new Launcher(
                    $"Nuevo {d}",
                    $"Crea un nuevo {d}",
                    oc.FullName(),
                    new ObservingCommand(vm, t => vm.OnCreate((Type)t!) , t => vm.CanCreate((Type)t!), nameof(vm.SelectedElement)),
                    j);
            }
        }
    }
}