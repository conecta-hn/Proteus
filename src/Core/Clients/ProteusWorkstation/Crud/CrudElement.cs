/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using static TheXDS.MCART.Objects;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Describe una propiedad y su respectivo control de edición dentro de
    /// un editor de Crud.
    /// </summary>
    public class CrudElement
    {
        private static readonly HashSet<ICrudDescription> _descriptions = new HashSet<ICrudDescription>();

        /// <summary>
        /// Obtiene un <see cref="CrudElement"/> para el modelo
        /// especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de modelo para el cual obtener un 
        /// <see cref="CrudElement"/>.
        /// </typeparam>
        /// <returns>
        /// Un <see cref="CrudElement"/> para el modelo especificado.
        /// </returns>
        public static CrudElement ForModel<T>() where T : ModelBase, new()
        {
            return new CrudElement(typeof(T));
        }

        /// <summary>
        /// Referencia al modelo a partir de cual se ha construido este
        /// <see cref="CrudElement"/>.
        /// </summary>
        public Type Model { get; }

        /// <summary>
        /// Elemento visual que contiene al editor generado para modificar
        /// a una entidad del modelo para el cual se ha construido este
        /// <see cref="CrudElement"/>.
        /// </summary>
        public FrameworkElement Editor { get; }

        /// <summary>
        /// Elemento visual que contiene una vista de detalles para una 
        /// entidad del modelo para el cual se ha construido este
        /// <see cref="CrudElement"/>.
        /// </summary>
        public FrameworkElement Details { get; }

        /// <summary>
        /// ViewModel que controla la edición de las propiedades del modelo
        /// para el cual se ha construido este <see cref="CrudElement"/>.
        /// </summary>
        public IDynamicViewModel ViewModel { get; }

        /// <summary>
        /// Descripción bajo la cual se han generado el editor y la vista
        /// de detalles de este <see cref="CrudElement"/>.
        /// </summary>
        internal ICrudDescription Description { get; }

        /// <summary>
        /// Enumera los controles de edición asociados a este
        /// <see cref="CrudElement"/>.
        /// </summary>
        public IEnumerable<IPropertyMapping> EditControls { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudElement"/>, construyendo una vista de editor y
        /// de detalles para el modelo especificado.
        /// </summary>
        /// <param name="model">
        /// Modelo para el cual generar las vistas del editor y de
        /// detalles.
        /// </param>
        public CrudElement(Type model)
            :this(GetDescription(model.ResolveCollectionType().ResolveToDefinedType()), model)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudElement"/>, construyendo una vista de editor y
        /// de detalles para el modelo especificado.
        /// </summary>
        /// <param name="description">
        /// Descripción de modelo a utilizar.
        /// </param>
        /// <param name="model">
        /// Modelo para el cual generar las vistas del editor y de
        /// detalles.
        /// </param>
        public CrudElement(ICrudDescription description, Type model = null)
        {
            Model = model?.ResolveCollectionType()?.ResolveToDefinedType() ?? description?.DescribedModel;
            Description = description;

            ViewModel = Description?.BaseViewModelType is null
                ? ViewModelFactory.BuildViewModel(Model).New<IDynamicViewModel>()
                : ViewModelFactory.BuildViewModel(Description.BaseViewModelType, Model).New<IDynamicViewModel>();

            if (!(Description is null))
            {
                Editor = CrudBuilder.BuildEditor(Description, out var editControls);
                Details = Description.Details ?? CrudBuilder.BuildDetails(Model, Description);
                EditControls = editControls;
            }
            else
            {
                Editor = CrudBuilder.BuildWarning(Model);
                Details = CrudBuilder.BuildWarning(Model);
                EditControls = Array.Empty<IPropertyMapping>();
            }

            if (!(Editor is null)) Editor.DataContext = ViewModel;
            if (!(Details is null)) Details.DataContext = ViewModel;
        }

        /// <summary>
        /// Crea una nueva instancia del modelo controlado por este
        /// <see cref="CrudElement"/>.
        /// </summary>
        /// <returns>
        /// Una nueva instancia del modelo controlado por este
        /// <see cref="CrudElement"/>.
        /// </returns>
        public ModelBase New()
        {
            var entity =  Model.New() as ModelBase;
            foreach (var k in EditControls)
            {
                if (k.Description.UseDefault)
                {
                    k.Property.SetValue(entity, k.Description.Default);
                    (ViewModel as NotifyPropertyChangeBase)?.Notify(k.Property.Name);
                    k.GetValue(entity);
                }
                else k.ClearControlValue();
            }
            return entity;
        }

        /// <summary>
        /// Crea una nueva instancia del modelo controlado por este
        /// <see cref="CrudElement"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo al cual castear el modelo luego de instanciarlo.
        /// </typeparam>
        /// <returns>
        /// Una nueva instancia de tipo <typeparamref name="T"/> del modelo
        /// controlado por este <see cref="CrudElement"/>.
        /// </returns>
        public ModelBase New<T>() where T : ModelBase, new()
        {
            return New() as T;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudElement"/>, construyendo una vista de editor 
        /// para el modelo especificado.
        /// </summary>
        /// <param name="model">
        /// Modelo para el cual generar la vista del editor.
        /// </param>
        /// <param name="details">
        /// Vista personalizada de detalles a asociar a este
        /// <see cref="CrudElement"/>.
        /// </param>
        public CrudElement(Type model, FrameworkElement details)
        {
            Model = model;
            Description = GetDescription(model);

            ViewModel = Description?.BaseViewModelType is null 
                ? ViewModelFactory.BuildViewModel(model).New<IDynamicViewModel>()
                : ViewModelFactory.BuildViewModel(Description.BaseViewModelType, model).New<IDynamicViewModel>();

            if (!(Description is null))
            {
                Editor = CrudBuilder.BuildEditor(Description, out var editControls);
                EditControls = editControls;
            }
            else
            {
                Editor = CrudBuilder.BuildWarning(model);
                EditControls = Array.Empty<IPropertyMapping>();
            }

            Details = details;

            if (!(Editor is null)) Editor.DataContext = ViewModel;
            if (!(Details is null)) Details.DataContext = ViewModel;
        }

        /// <summary>
        /// Escribe los cambios realizados en el editor sobre la entidad especificada.
        /// </summary>
        public void Commit()
        {
            foreach (var j in EditControls)
            {
                j.SetValue(j.Description.PropertySource == PropertyLocation.Model ? ViewModel.Entity : ViewModel);
                (ViewModel as INotifyPropertyChangeBase)?.Notify(j.Property.Name);
            }
        }

        /// <summary>
        /// Obtiene una descripción de Crud para el modelo especificado.
        /// </summary>
        /// <param name="model">
        /// Tipo del modelo del cual obtener la descripción.
        /// </param>
        /// <returns>
        /// Una descripción del modelo de datos.
        /// </returns>
        public static ICrudDescription GetDescription(Type model)
        {
            var r = _descriptions.FirstOrDefault(p => p.DescribedModel == model);
            if (r is null)
            {
                r = FindFirstObject<ICrudDescription>(p => CrudBuilder.DescribesModel(p, model));
                if (!(r is null)) _descriptions.Add(r);
            }
            return r;                
        }

        /// <summary>
        /// Carga prematuramente todas las descripciones de modelos dentro
        /// del dominio de la aplicación.
        /// </summary>
        public static void Preload()
        {
            Proteus.CommonReporter?.UpdateStatus("Pre-construyendo elementos de UI...");
            foreach (var j in FindAllObjects<ICrudDescription>())
            {
                _descriptions.Add(j);
            }
        }

        /// <summary>
        /// Limpia el estado de todos los controles de edición.
        /// </summary>
        public void ClearAll()
        {
            foreach (var j in EditControls)
            {
                if (j.ContainingControl is ToggleButton chkb) chkb.IsChecked = false;
                j.ClearControlValue();
            }
        }

        /// <summary>
        /// Convierte este objeto en su representación como una cadena.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description?.FriendlyName
                ?? GetDescription(Model)?.FriendlyName
                ?? Model?.NameOf()
                ?? GetType().NameOf();
        }
    }
}