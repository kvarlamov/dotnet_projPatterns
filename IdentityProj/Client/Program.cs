using System.Text.Json;
using IdentityModel.Client;

var client = new HttpClient();
var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (discovery.IsError)
{
    Console.WriteLine(discovery.Error);
    return;
}

// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discovery.TokenEndpoint,

    ClientId = "client1",
    ClientSecret = "api1",
    Scope = "api1"
});

if (tokenResponse.IsError)
{
    Console.WriteLine($"\naccess token: {tokenResponse.Error}");
    return;
}

Console.WriteLine(tokenResponse.AccessToken);
Console.WriteLine("\n\n");

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:6001/identity/auth-method");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions(){WriteIndented = true}));
}


