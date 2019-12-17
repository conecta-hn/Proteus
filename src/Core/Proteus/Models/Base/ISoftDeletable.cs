/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/


namespace TheXDS.Proteus.Models.Base
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
