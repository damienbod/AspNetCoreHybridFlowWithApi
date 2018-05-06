using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVCClient;

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
                var discoClient = new DiscoveryClient(_authConfigurations.Value.StsServer);
                var disco = await discoClient.GetAsync();
                if (disco.IsError)
                {
                    throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
                }

                var tokenClient = new TokenClient(disco.TokenEndpoint, "ProtectedApi", "api_in_protected_zone_secret");
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("scope_used_for_api_in_protected_zone");

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
