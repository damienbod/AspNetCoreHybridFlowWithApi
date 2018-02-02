using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebHybridClient
{
    public class ApiService
    {

        public async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var discoClient = new DiscoveryClient("https://localhost:44352");
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

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44342");
                    client.SetBearerToken(tokenResponse.AccessToken);

                    var response = await client.GetAsync("/api/values");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var data = JArray.Parse(responseContent);

                        return data;
                    }

                    throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
