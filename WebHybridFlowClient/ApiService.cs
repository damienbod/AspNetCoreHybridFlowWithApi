using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace WebHybridClient;

public class ApiService
{
    private readonly IOptions<AuthConfigurations> _authConfigurations;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ApiTokenCacheClient _apiTokenClient;

    public ApiService(
        IOptions<AuthConfigurations> authConfigurations, 
        IHttpClientFactory clientFactory,
        ApiTokenCacheClient apiTokenClient)
    {
        _authConfigurations = authConfigurations;
        _clientFactory = clientFactory;
        _apiTokenClient = apiTokenClient;
    }

    public async Task<List<string>> GetApiDataAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri(_authConfigurations.Value.ProtectedApiUrl);

            var access_token = await _apiTokenClient.GetApiToken(
                "CC_FOR_API",
                "scope_used_for_api_in_protected_zone",
                "cc_for_api_secret"
            );

            client.SetBearerToken(access_token);

            var response = await client.GetAsync("api/values");
            if (response.IsSuccessStatusCode)
            {
                var data = await JsonSerializer.DeserializeAsync<List<string>>(
                await response.Content.ReadAsStreamAsync());

                if(data != null)
                    return data;

                return new List<string>();
            }

            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }
}