/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.ViewModels.Base
{
    public abstract class CrudViewModelBasicBase : ProteusViewModel
    {
        /// <summary>
        /// Obtiene una referencia al servicio a utilizar para las
        /// operaciones CRUD de este ViewModel.
        /// </summary>
        protected Service? Service => Proteus.Infer(SelectedElement?.Model);

        /// <summary>
        /// Obtiene, de ser posible, a la entidad padre de la que se
        /// encuentra actualmente seleccionada.
        /// </summary>
        /// <returns>
        /// La entidad padre de la actualmente seleccionasa, o 
        /// <see langword="null"/> si la entidad seleccionada no puede
        /// tener información sobre su padre en este contexto.
        /// </returns>
        protected abstract ModelBase? GetParent();
        public abstract CrudElement SelectedElement { get; }

        protected bool Precheck()
        {
            var parent = GetParent();
            bool fail = false;
            if (!(SelectedElement.ViewModel.Entity is ModelBase e)) return true;
            SelectedElement.Commit();
            try
            {
                foreach (var j in SelectedElement.Description.BeforeSave)
                {
                    j.CallSaves(e!, parent);
                }
                if (SelectedElement.Description is IVmCrudDescription ivm)
                {
                    foreach (var j in ivm.VmBeforeSave)
                    {
                        j.CallSaves(SelectedElement.ViewModel, parent);
                    }
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Error(ex.Message);
                fail = true;
            }

            foreach (var j in SelectedElement.Description.Descriptions)
            {
                var f = j.Validator?.Invoke(e!, j.Property)?.ToList();
                if (f?.Any() ?? false)
                {
                    fail = true;
                    Proteus.MessageTarget?.Stop(string.Concat(f.Select(p => $"{p.FailedProperty?.NameOf().OrNull("{0}: ")}{p.Message}\n")));
                }
            }

            if (Settings.Default.CheckExists)
            {
                if (Service!.Exists(e))
                {
                    fail = true;
                    Proteus.MessageTarget?.Stop($"Ya existe un elemento con el Id '{e.StringId}' en la base de datos.");
                }
            }
            return fail;
        }

        /// <summary>
        /// Ejecuta una acción a realizar justo después de guardar la
        /// información en la base de datos.
        /// </summary>
        protected virtual async Task PostSave(ModelBase e)
        {
            try
            {
                foreach (var j in SelectedElement!.Description.AfterSave)
                {
                    j.CallSaves(e, GetParent());
                }
                if (SelectedElement?.Description is IVmCrudDescription ivm)
                {
                    foreach (var j in ivm.VmBeforeSave)
                    {
                        j.CallSaves(SelectedElement.ViewModel, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Critical(ex);
            }
            await FullRefreshVmAsync(SelectedElement?.ViewModel as ProteusViewModel);
            await FullRefreshVmAsync(this);
            SelectedElement?.ViewModel.Refresh();
            EnableEditCtrls();
        }

        protected void EnableEditCtrls()
        {
            foreach (var j in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>()) j.ContainingControl.IsEnabled = true;
        }
        protected void DisableIdCtrls()
        {
            foreach (var j in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
            {
                if (j.Property.HasAttr<KeyAttribute>())
                {
                    j.ContainingControl.IsEnabled = false;
                }
            }
        }
        protected void ClearCtrls(ModelBase entity)
        {
            foreach (var k in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
            {
                if (k.Description.UseDefault && k.Property.CanWrite)
                {
                    k.Property.SetValue(entity, k.Description.Default);
                    (SelectedElement!.ViewModel as NotifyPropertyChangeBase)?.Notify(k.Property.Name);
                    k.GetValue(entity);
                }
                else k.ClearControlValue();
            }
        }

    }
}