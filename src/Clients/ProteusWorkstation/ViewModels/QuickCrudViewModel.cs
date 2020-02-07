/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.ViewModels
{
    public class QuickCrudViewModel : CrudViewModelBasicBase, ICrudEditingViewModel, IReadEntityViewModel<ModelBase>, IPageViewModel
    {
        public enum CrudMode : byte
        {
            Close,
            New
        }

        private readonly ModelBase _parent;
        private readonly CrudMode _mode;

        public override CrudElement SelectedElement { get; }

        /// <summary>
        ///     Obtiene o establece el valor Entity.
        /// </summary>
        /// <value>El valor de Entity.</value>

        public ModelBase Entity
        {
            get => (ModelBase)SelectedElement.ViewModel.Entity!;
            private set
            {
                SelectedElement.ViewModel.Entity = value as ModelBase;
                ClearCtrls(Entity);
            }
        }

        public ICommand CancelCommand => CloseCommand;

        public ICommand SaveCommand { get; }

        /// <summary>
        /// Obtiene un valor que determina si este ViewModel puede ser
        /// cerrado.
        /// </summary>
        public bool Closeable => true;

        /// <summary>
        /// Obtiene un comando de cierre de este ViewModel.
        /// </summary>
        public SimpleCommand CloseCommand { get; }

        /// <summary>
        /// Obtiene al contenedor visual cerrable de este ViewModel.
        /// </summary>
        public ICloseable Host { get; }

        /// <summary>
        /// Obtiene el título de este ViewModel a mostrar en su
        /// correspondiente contenedor visual.
        /// </summary>
        public string Title { get; set; }

        public QuickCrudViewModel(ICloseable host, Type model, ModelBase? parent) : this(host, model, parent, CrudMode.Close)
        {
        }

        public QuickCrudViewModel(ICloseable host, Type model, ModelBase? parent, CrudMode mode)
        {
            SaveCommand = new SimpleCommand(OnSave, true);
            CloseCommand = new SimpleCommand(Close);
            _parent = parent;
            _mode = mode;
            Host = host;
            SelectedElement = new CrudElement(model);
            Entity = model.New<ModelBase>();
            Title = $"Nuevo {SelectedElement.Description.FriendlyName}";
        }

        private void OnSave() => BusyOp(OnSaveAsync());

        private async Task OnSaveAsync()
        {
            if (Precheck()) return;
            var t = await Service!.AddAsync(Entity);
            if (t.Result == Result.Ok) 
            {
                await PostSave(Entity); 
                switch (_mode)
                {
                    case CrudMode.Close:Close(); break;
                    case CrudMode.New:
                        Entity = SelectedElement.Model.New<ModelBase>();
                        break;
                }
            }
        }

        public void Close()
        {
            if (Entity?.IsNew ?? false) Service!.Rollback(Entity!);
            App.UiInvoke(Host.Close);
        }

        protected override ModelBase? GetParent() => _parent;
    }
}