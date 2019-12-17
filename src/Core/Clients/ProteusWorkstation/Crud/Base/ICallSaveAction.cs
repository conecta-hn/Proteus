/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    public interface ICallSaveAction
    {
        void CallSaves(object obj, ModelBase parent);
    }
}