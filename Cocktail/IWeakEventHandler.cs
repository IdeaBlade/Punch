using System;

namespace Cocktail
{
    public interface IWeakEventHandler<E> where E : EventArgs
    {
        // Properties
        EventHandler<E> Handler { get; }
    }

}
