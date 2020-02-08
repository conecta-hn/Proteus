/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;

namespace TheXDS.Proteus.Plugins
{
    public class AddPcCrudTool : CrudTool<EstacionBase>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PasswordResetCrudTool"/>.
        /// </summary>
        public AddPcCrudTool() : base(CrudToolVisibility.Unselected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            foreach(var j in models)
            {
                yield return new Launcher(
                    "Agregar este equipo...",
                    $"Agrega este equipo como un {CrudElement.GetDescription(j)?.FriendlyName ?? j.Name} a la lista.",
                    GetMethod<AddPcCrudTool, Action<ICrudViewModel, Type>>(p => p.AddThisPC).FullName(),
                    new SimpleCommand(() => AddThisPC(vm, j)));
            }
        }

        private void AddThisPC(ICrudViewModel? vm, Type model)
        {
            vm?.CreateNew.Execute(model);            
        }
    }

}