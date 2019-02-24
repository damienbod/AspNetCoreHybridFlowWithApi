using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebHybridClient
{
    public class ApiService
    {
        private readonly IOptions<AuthConfigurations> _authConfigurations;
        private readonly IHttpClientFactory _clientFactory;


        public ApiService(IOptions<AuthConfigurations> authConfigurations, IHttpClientFactory clientFactory)
        {
            _authConfigurations = authConfigurations;
            _clientFactory = clientFactory;
        }

        public async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var tokenclient = _clientFactory.CreateClient();

                var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(tokenclient, _authConfigurations.Value.StsServer);

                if (disco.IsError)
                {
                    throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
                }

                var tokenResponse = await HttpClientTokenRequestExtensions.RequestClientCredentialsTokenAsync(tokenclient, new ClientCredentialsTokenRequest
                {
                    Scope = "scope_used_for_api_in_protected_zone",
                    ClientSecret = "api_in_protected_zone_secret",
                    Address = disco.TokenEndpoint,
                    ClientId = "ProtectedApi"
                });

                if (tokenResponse.IsError)
                {
                    throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
                }

                var client = _clientFactory.CreateClient();

                client.BaseAddress = new Uri(_authConfigurations.Value.ProtectedApiUrl);
                client.SetBearerToken(tokenResponse.AccessToken);

                var response = await client.GetAsync("api/values");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
