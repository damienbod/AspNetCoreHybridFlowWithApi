using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceFlowWeb
{
    public class DeviceFlowService
    {
        private readonly IOptions<AuthConfigurations> _authConfigurations;
        private readonly IHttpClientFactory _clientFactory;

        public DeviceFlowService(IOptions<AuthConfigurations> authConfigurations, IHttpClientFactory clientFactory)
        {
            _authConfigurations = authConfigurations;
            _clientFactory = clientFactory;
        }

        internal async Task<DeviceAuthorizationResponse> BeginLogin()
        {
            var client = _clientFactory.CreateClient();

            var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(client, _authConfigurations.Value.StsServer);

            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var deviceAuthorizationRequest = new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "deviceFlowWebClient"
            };
            deviceAuthorizationRequest.Scope = "email profile openid";
            var response = await client.RequestDeviceAuthorizationAsync(deviceAuthorizationRequest);

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response;
        }

        internal async Task<TokenResponse> RequestTokenAsync(string deviceCode, int interval)
        {
            var discoClient = new DiscoveryClient(_authConfigurations.Value.StsServer);
            var disco = await discoClient.GetAsync();
            if (disco.IsError)
            {
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var client = _clientFactory.CreateClient();

            while (true)
            {
                if(!string.IsNullOrWhiteSpace(deviceCode))
                {
                    var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "deviceFlowWebClient",
                        DeviceCode = deviceCode
                    });

                    if (response.IsError)
                    {
                        if (response.Error == "authorization_pending" || response.Error == "slow_down")
                        {
                            Console.WriteLine($"{response.Error}...waiting.");
                            Thread.Sleep(interval * 1000);
                        }
                        else
                        {
                            throw new Exception(response.Error);
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
                else
                {
                    // lets wait
                    Thread.Sleep(interval * 1000);
                }
                
            }
        }

    }
}
