/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Crud.Base;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using System.Windows.Input;
using System.Linq;

namespace TheXDS.Proteus.ViewModels
{
    public class ObjectEditorViewModel : CrudViewModelBase
    {
        private string _fieldIcon;
        private string _fieldName;
        private bool _canSelect;
        private bool _selectMode;
        private object? _tempSelection;

        /// <summary>
        ///     Obtiene el origen de selección de este <see cref="ListEditorViewModel"/>.
        /// </summary>
        public ICollection<ModelBase> SelectionSource { get; }

        /// <summary>
        ///     Obtiene el comando que agrega elementos desde la lista de
        ///     origen de selección a la colección del modelo de datos.
        /// </summary>
        public SimpleCommand SelectCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand OkSelectCommand { get; }

        /// <summary>
        ///     Obtiene el comando que cancela la adición de elementos
        ///     seleccionados desde una lista.
        /// </summary>
        public ICommand CancelSelectCommand { get; }

        /// <summary>
        ///     Obtiene o establece un valor que configura este
        ///     <see cref="ListEditorViewModel"/> para agregar elementos desde
        ///     una lista.
        /// </summary>
        public bool SelectMode
        {
            get => _selectMode;
            set => Change(ref _selectMode, value);
        }

        public object? TempSelection
        { 
            get=> _tempSelection; 
            set=>Change(ref _tempSelection, value);
        }


        public ObjectEditorViewModel(IObjectPropertyDescription description, params Type[] models) : this(description.Source?.ToList(), description, models) { }

        private void OnCancelSelect()
        {
            SelectMode = false;
            IsBusy = false;
            TempSelection = null;
        }

        private void OnOkSelect()
        {
            Selection = TempSelection;
            OnCancelSelect();
        }

        public ObjectEditorViewModel(ICollection<ModelBase> selectionSource, IObjectPropertyDescription description, params Type[] models) : base(models)
        {
            FieldName = description.Label;
            FieldIcon = description.Icon;
            CanSelect = description.Selectable;

            SelectionSource = selectionSource;
            SelectCommand = new SimpleCommand(OnSelect);
            OkSelectCommand = new SimpleCommand(OnOkSelect);
            CancelSelectCommand = new SimpleCommand(OnCancelSelect);

            RegisterPropertyChangeBroadcast(nameof(Selection), nameof(DisplayValue));
        }
        private void OnSelect()
        {
            OnCancel();
            IsBusy = true;
            SelectMode = true;
        }

        /// <summary>
        ///     Obtiene un valor que indica si este
        ///     <see cref="ListEditorViewModel"/> permite adicionar elementos
        ///     existentes a la colección del modelo de datos.
        /// </summary>
        public bool CanSelect
        {
            get => _canSelect;
            internal set
            {
                _canSelect = value;
                SelectCommand.SetCanExecute(value);
            }
        }

        protected override void OnDelete(object o)
        {
            Selection = null;
        }

        protected override Task<DetailedResult> PerformSave(ModelBase entity)
        {
            return Task.FromResult(DetailedResult.Ok);
        }

        protected override ModelBase? GetParent()
        {
            return null;
        }

        protected override void AfterSave()
        {
        }

        /// <summary>
        ///     Obtiene el ícono configurado para mostrar del campo
        ///     correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldIcon
        {
            get => _fieldIcon;
            internal set => Change(ref _fieldIcon, value);
        }

        /// <summary>
        ///     Obtiene un valor amigable para mostrar en la UI de la aplicación.
        /// </summary>
        public string DisplayValue => Selection?.ToString() ?? "-";

        /// <summary>
        ///     Obtiene el nombre configurado para mostrar del campo
        ///     correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldName
        {
            get => _fieldName;
            internal set => Change(ref _fieldName, value);
        }
    }
}