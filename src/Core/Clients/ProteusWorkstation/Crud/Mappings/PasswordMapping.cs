/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.Windows.Controls;

namespace TheXDS.Proteus.Crud.Mappings
{
    /// <summary>
    /// Mapea propiedades de contraseña en un control
    /// <see cref="PasswordBox"/>.
    /// </summary>
    public class PasswordMapping : PropertyMapping
    {
        private readonly PasswordBox _pwdBox;
        private object _instance;
        private bool _changed;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PasswordMapping"/>.
        /// </summary>
        /// <param name="property">
        /// Propiedad a enlazar a este <see cref="PropertyMapping"/>.
        /// </param>
        public PasswordMapping(IPropertyDescription property) : base(property, new PasswordBox())
        {
            _pwdBox = (PasswordBox)Control;
            _pwdBox.PasswordChanged += (sender, e) => _changed = true;
        }

        /// <summary>
        /// Obtiene o establce manualmente el valor de este control.
        /// </summary>
        public override object ControlValue
        {
            get
            {
                return _changed
                    ? MCART.Security.Password.PasswordStorage.CreateHash(_pwdBox.SecurePassword)
                    : Description.Property.GetMethod.Invoke(_instance, System.Array.Empty<object>());
            }
            set
            {
                /* No hacer nada. El control no debe admitir contraseñas */
            }
        }

        /// <summary>
        /// Obtiene el valor de una entidad de datos y lo establece en este
        /// control.
        /// </summary>
        /// <param name="instance">
        /// Instancia desde la cual obtener el valor.
        /// </param>
        public override void GetValue(object instance)
        {
            _instance = instance;
            base.GetValue(instance);
            _changed = false;
        }

        /// <summary>
        /// Limpia el contenido del control generado.
        /// </summary>
        public override void ClearControlValue()
        {
            _pwdBox.Clear();
        }
    }
}