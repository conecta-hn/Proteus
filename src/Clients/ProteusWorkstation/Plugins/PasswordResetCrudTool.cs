/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Dialogs;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Herramienta que permite restablecer contraseñas a los usuarios.
    /// </summary>
    public class PasswordResetCrudTool : CrudTool<IUser>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PasswordResetCrudTool"/>.
        /// </summary>
        public PasswordResetCrudTool() : base(CrudToolVisibility.Selected)
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
        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            if (vm is null) yield break;
            yield return new Launcher(
                "Restablecer contraseña",
                "Restablece la contraseña del usuario en caso de haberla olvidado o perdido.",
                GetMethod<PasswordResetCrudTool, Action<ICrudViewModel>>(p => p.ResetPassword).FullName(),
                new SimpleCommand(() => ResetPassword(vm)));
        }

        private void ResetPassword(ICrudViewModel? vm)
        {
            if (!(vm.Selection is IUser user) || !PasswordDialog.ConfirmPassword(out var p)) return;
            vm.BusyDo(SetPw(user, p));
            vm.SaveCommand.Execute(user);
        }

        private static async Task SetPw(IUser u, UserData p)
        {
            u.PasswordHash = await Task.Run(() => PasswordStorage.CreateHash(p.Password));
        }
    }
}