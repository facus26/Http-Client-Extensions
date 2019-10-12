namespace ClienteHttp.Abstractions
{
    public interface IServiceClientFactory<TClient>
    {
        TClient CreateClient(HttpRestClient client);
    }
}
