/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    public abstract class ModelCrudDescriptor<T> : CrudDescriptor<T> where T : ModelBase, new()
    {
        protected abstract string ModelName { get; }
        protected abstract void DescribeSubModel();
    }
}