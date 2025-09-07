namespace Game.JsBridge
{
    public interface IQueryProvider
    {
        T Get<T>() where T : IQuery, new();
    }
}
