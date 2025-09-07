namespace Game.UI.Presenters
{
    public interface IBindable
    {
        void Bind(object value);
        void Unbind();
    }

    public interface IBindable<in T> : IBindable
    {
        void IBindable.Bind(object value)
        {
            Bind((T)value);
        }

        void Bind(T value);
    }
}