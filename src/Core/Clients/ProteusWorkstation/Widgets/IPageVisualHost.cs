/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Widgets
{
    public interface IPageVisualHost : IPage
    {
        void Activate(IPage page);
    }
}