# Dotnet-HTTP-Client-Extensions
Extensions to reduce boilerplate code when using .NET HttpClient. The project is designed to make correct usage of sockets and offers integrated JSON Serialization.

## How to use

Simply inject **IHttpRestClient** into the "Service" where you want to use.

```csharp
public  class  Service : IService
{
	private  readonly  IHttpRestClient  _client;
	
	public  Service(IHttpRestClient  client)
	{
		_client = client;
	}

	public  string  GetAny() 
	{
		return  _client.Get("path/to/service");
	}

	public  async  Task<string> AsyncGetAny() 
	{
		return  await  _client.GetAsync("path/to/service");
	}
}
```

### .Net Core / Grace

**AddHttpRestClient()**: With this extension method, the injection of **IHttpRestClient** is configured and the **HttpClient** can be configured.

This method injects HttpClient the most efficient and recommended way for .Net Core.

  
```csharp
public  class  Startup
{
	...
	
	public  void  ConfigureServices(IServiceCollection  services)
	{
		services.AddHttpRestClient<IServicio, Servicio>(CLIENT_NAME, client=>				{
		  client.BaseAddress = new  Uri(urlService)
	  });
    
	  services.AddMvc();

	}
	...
}
```

This way we are telling you to inject IHttpRestClient to the Service class and we specify a name to identify it from the rest of HttpClients.

**IMPORTANT:** The .AddHttpRestClient() method, not only configure the injection of the IHttpRestClient in the Service class, configures the injection of the service in question. 
In the example, IService with the Service implementation. If another binding IService is added to Service, it will generate configuration conflicts and will not work correctly.

It is important to note that internally the injector needs different names to identify different customers. This is for when we need to invoke different services with their respective URIs from the same client application. Example:


```csharp
public class Startup
{
	...
	
	public void ConfigureServices(IServiceCollection  services)
	{
		services.AddHttpRestClient<IServicioUsuarios, ServicioUsuarios>("ClientUsers", client => 
		{
			client.BaseAddress = new Uri(url_api_service)
		});
  

		services.AddHttpRestClient<IServiceProducts, ServicioProductos>("ClientProduct", client =>
		{
			client.BaseAddress = new Uri(url_api_pruducts)
		});
  

		services.AddHttpRestClient<IServicesAny, ServiceAny>("ClientAny", client =>
		{
			client.BaseAddress = new Uri(url_api_pruducts)
		});

		services.AddMvc();

	}
	...
}
```

### .NET Classic 


In the case of .Net Classic 4.6 onwards, extensions were created for the Ninject module. They are found in **HttpClientExtensions.NetClassic**. In our Module we must first bindear the **HttpRestClientFactory**.

For that there is an extension method **AddHttpRestClient**

```csharp
public class Modulo : NinjectModule
{
	public override void Load()
	{
		#region Other injections
		...
		#endregion

		// Configure the binding for the HttpRestClientFactory		
		AddHttpRestClientFactory();
    
    // Configure the IService to Service binding and also the injected client. The name of the client represents an API to invoke.
    // Example if the client used it to invoke the API-Products, the CLIENT_NAME should be "API_Products".
    // If you need to use the same client in another service class, just pass the same name per parameter

		AddHttpRestClient<IService, Service>(API_Products, client => { client.BaseAddress = new  Uri(URL_API_Products)});
		AddHttpRestClient<IService2, Service2>(CLIENT_NAME_2, client => { client.BaseAddress = new  Uri(url_Api_2)});
		AddHttpRestclient<IService3, Service3>(API_Products);
	}
}

```

**IMPORTANT:** As in the Net Core module, the AddHttpRestClient<IService, ServiceImplementation>() method configures the IService binding to the Service Implementation, for example. If another binding is added to that Interface-Implementation pair, the Ninject module will not know how to resolve it causing exception at runtime.
