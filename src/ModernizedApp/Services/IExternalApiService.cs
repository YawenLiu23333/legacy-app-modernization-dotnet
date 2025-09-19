namespace ModernizedApp.Services;

public interface IExternalApiService
{
    Task<object> GetTodoAsync();
}
