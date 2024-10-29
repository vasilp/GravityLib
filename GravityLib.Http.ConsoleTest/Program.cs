using GravityLib.Http.Config;
using GravityLib.Http.ConsoleTest;
using GravityLib.Http.OAuth2;
using Microsoft.Extensions.Caching.Memory;


var cache = new MemoryCache(new MemoryCacheOptions());
var logger = new ConsoleLogger();
var config = new APIClientConfig()
{
    AuthType = AuthType.OAuth2,
    Url = "",
    ClientId = "",
    ClientSecret = "",
};
var apiClient = new OAuth2ApiClient(cache, config, logger);

try
{
    Console.WriteLine($"Calling \"{config.Url}\" ...");
    var response = await apiClient.PostAsync("api", new { });

    Console.WriteLine("Response content: " + response.Content);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
