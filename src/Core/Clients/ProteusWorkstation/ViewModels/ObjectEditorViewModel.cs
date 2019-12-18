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

namespace TheXDS.Proteus.ViewModels
{
    public class ObjectEditorViewModel : CrudViewModelBase
    {
        private string _fieldIcon;
        private string _fieldName;

        public ObjectEditorViewModel(IObjectPropertyDescription description, params Type[] models) : base(models)
        {
            FieldName = description.Label;
            FieldIcon = description.Icon;
        }

        protected override void OnDelete(object o)
        {
            throw new NotImplementedException();
        }

        protected override Task<DetailedResult> PerformSave(ModelBase entity)
        {
            throw new NotImplementedException();
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