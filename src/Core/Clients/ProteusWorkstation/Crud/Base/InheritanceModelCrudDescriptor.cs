/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    public abstract class InheritanceModelCrudDescriptor<T> : ModelCrudDescriptor<T> where T : ModelBase, new()
    {
        protected abstract void DescribeBaseModel();

        protected override sealed void DescribeModel()
        {
            FriendlyName(ModelName);
            DescribeBaseModel();
            DescribeSubModel();
        }
    }
}