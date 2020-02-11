/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;

namespace TheXDS.Proteus.Plugins
{
    public class AddThisUserCrudTool : CrudTool<IUserBase>
    {
        public AddThisUserCrudTool() : base(CrudToolVisibility.Unselected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            if (vm is null) yield break;
            foreach (var j in models)
            {
                yield return new Launcher(
                    "Añadir usuario",
                    "Añade al usuario actual a la lista.",
                    GetMethod<AddThisUserCrudTool, Action<Type, ICrudViewModel>>(p => p.AddThisUser).FullName(),
                    new SimpleCommand(() => AddThisUser(j, vm)));
            }
        }

        private void AddThisUser(Type model, ICrudViewModel vm)
        {
            if (!(Proteus.Session is { } u)) return;
            vm.CreateNew.Execute(model);
            ((IUserBase)vm.Selection).UserId = u.Id;
            (vm as NotifyPropertyChangeBase)?.Notify("Entity.UserId");        
        }
    }
}