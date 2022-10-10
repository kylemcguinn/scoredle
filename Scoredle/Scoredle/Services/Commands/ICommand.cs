namespace Scoredle.Services.Commands
{
    public interface ICommand<T, T1>
    {
        public T Parameter { get; set; }
        public T1 Configuration { get; set; }
        Task Execute();
    }
}
